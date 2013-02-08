using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

using System.Diagnostics;
using System.IO;

namespace ChemSW.Nbt
{
    public class CAFImportManager
    {
        private CswNbtResources _NBTResources;
        //private CswResources _CAFResources;
        private CswNbtResources _CAFResources;
        private int _NumberToProcess;
        private Dictionary<string, CAFMapping> _Mappings;

        private CafTranslator _CafTranslator;

        private Collection<CswNbtObjClassMaterial> _createdMaterials = new Collection<CswNbtObjClassMaterial>();
        private Collection<CswNbtObjClassSize> _createdSizes = new Collection<CswNbtObjClassSize>();
        private CswCommaDelimitedString vendorIds = new CswCommaDelimitedString();
        private CswCommaDelimitedString packageIds = new CswCommaDelimitedString();
        private CswCommaDelimitedString matSynIds = new CswCommaDelimitedString();
        private CswCommaDelimitedString unitIds = new CswCommaDelimitedString();
        private CswCommaDelimitedString materialIds = new CswCommaDelimitedString();
        private CswCommaDelimitedString sizeIds = new CswCommaDelimitedString();

        CswNbtMetaDataNodeType vendorNT;
        CswNbtMetaDataNodeType chemicalNT;

        CswNbtMetaDataNodeType materialSynNT;
        CswNbtMetaDataNodeTypeProp synonymLegacyIdNTP;
        CswNbtMetaDataNodeTypeProp synonymMaterialNTP;

        CswNbtMetaDataNodeTypeProp tradenameNTP;
        CswNbtMetaDataNodeTypeProp supplierNTP;
        CswNbtMetaDataNodeTypeProp partNoNTP;
        CswNbtMetaDataNodeTypeProp materialLegacyIdNTP;

        CswNbtMetaDataObjectClass unitOfMeasureOC;

        CswNbtMetaDataNodeTypeProp vendorNameNTP;
        CswNbtMetaDataNodeTypeProp vendorLegacyIdNTP;

        CswNbtMetaDataNodeType UoM_weight_NT;
        CswNbtMetaDataNodeType UoM_each_NT;
        CswNbtMetaDataNodeType UoM_vol_NT;
        CswNbtMetaDataObjectClassProp nameOCP;
        CswNbtMetaDataNodeTypeProp legacyIdNTP;

        CswNbtMetaDataNodeType sizeNT;
        CswNbtMetaDataNodeTypeProp catalogNTP;
        CswNbtMetaDataNodeTypeProp materialNTP;
        CswNbtMetaDataNodeTypeProp quantNTP;
        CswNbtMetaDataNodeTypeProp sizeLegacyId;

        public CAFImportManager( CswNbtResources NBTResources, Int32 NumberToProcess )
        {
            _NBTResources = NBTResources;
            _NumberToProcess = NumberToProcess;
            _initMappings();
            _CafTranslator = new CafTranslator();

            //fetch NTs upfront
            vendorNT = _NBTResources.MetaData.getNodeType( "Vendor" );
            chemicalNT = _NBTResources.MetaData.getNodeType( "Chemical" );

            tradenameNTP = chemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Tradename );
            supplierNTP = chemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
            partNoNTP = chemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.PartNumber );
            materialLegacyIdNTP = chemicalNT.getNodeTypeProp( "Legacy Id" );

            vendorNameNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
            vendorLegacyIdNTP = vendorNT.getNodeTypeProp( "Legacy Id" );

            UoM_weight_NT = _NBTResources.MetaData.getNodeType( "Unit (Weight)" );
            UoM_each_NT = _NBTResources.MetaData.getNodeType( "Unit (Each)" );
            UoM_vol_NT = _NBTResources.MetaData.getNodeType( "Unit (Volume)" );

            unitOfMeasureOC = _NBTResources.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            nameOCP = unitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );

            materialSynNT = _NBTResources.MetaData.getNodeType( "Material Synonym" );
            synonymLegacyIdNTP = materialSynNT.getNodeTypeProp( "Legacy Id" );
            synonymMaterialNTP = materialSynNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialSynonym.PropertyName.Material );

            sizeNT = _NBTResources.MetaData.getNodeType( "Size" );

            catalogNTP = sizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );
            materialNTP = sizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            quantNTP = sizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );
            sizeLegacyId = sizeNT.getNodeTypeProp( "Legacy Id" );
        }

        public void Import()
        {
            _CAFResources = CswNbtResourcesFactory.makeCswNbtResources( _NBTResources );
            _CAFResources.AccessId = "caf";

            string sql = "select * from (select * from nbtimportqueue where state = 'N' or state = 'S' order by state asc, priority) where rownum <=" + _NumberToProcess;
            CswArbitrarySelect arbSelect = _CAFResources.makeCswArbitrarySelect( "getImportItems", sql );
            DataTable dt = arbSelect.getTable();
            foreach( DataRow Row in dt.Rows )
            {
                string tableName = Row["tablename"].ToString();
                string itemPK = Row["itempk"].ToString();
                switch( tableName )
                {
                    case "units_of_measure":
                        _importUnitsOfMeasure( itemPK );
                        break;
                    case "materials":
                        _importChemical( itemPK );
                        break;
                    case "vendors":
                        _importVendor( itemPK );
                        break;
                    case "materials_synonyms":
                        _importMaterialSynonym( itemPK );
                        break;
                    case "packdetail":
                        _importSize( itemPK );
                        break;
                    default:
                        //Add more as we extend the importer
                        break;
                }
            }

            CswTableUpdate tu = _CAFResources.makeCswTableUpdate( "update_nbtimportqueue", "nbtimportqueue" );
            string whereClause = _buildWhereClause();
            if( false == string.IsNullOrEmpty( whereClause ) )
            {
                DataTable nbtimportqueue = tu.getTable( "where " + whereClause );
                foreach( DataRow Row in nbtimportqueue.Rows )
                {
                    Row["state"] = "D"; //D for DONT import me
                }
                tu.update( nbtimportqueue );
            }

            //Important!!! Always manually release resources after use
            _CAFResources.finalize();
            _CAFResources.release();
            _CAFResources = null;
        }

        #region private methods

        private void _initMappings()
        {
            _Mappings = new Dictionary<string, CAFMapping>();

            #region Vendor
            CswNbtMetaDataNodeType vendorNT = _NBTResources.MetaData.getNodeType( "Vendor" );
            CswNbtMetaDataNodeTypeProp vendorNameNTP = vendorNT.getNodeTypeProp( "Vendor Name" );
            CswNbtFieldTypeRuleText textFTR = (CswNbtFieldTypeRuleText) vendorNameNTP.getFieldTypeRule();
            _Mappings.Add( vendorNameNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = vendorNameNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "vendorname",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp cityNTP = vendorNT.getNodeTypeProp( "City" );
            _Mappings.Add( cityNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = cityNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "city",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp street1NTP = vendorNT.getNodeTypeProp( "Street1" );
            _Mappings.Add( street1NTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = street1NTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "street1",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp street2NTP = vendorNT.getNodeTypeProp( "Street2" );
            _Mappings.Add( street2NTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = street2NTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "street2",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp zipNTP = vendorNT.getNodeTypeProp( "Zip" );
            _Mappings.Add( zipNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = zipNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "zip",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp stateNTP = vendorNT.getNodeTypeProp( "State" );
            _Mappings.Add( stateNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = stateNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "state",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp faxNTP = vendorNT.getNodeTypeProp( "Fax" );
            _Mappings.Add( faxNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = faxNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "fax",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp phoneNTP = vendorNT.getNodeTypeProp( "Phone" );
            _Mappings.Add( phoneNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = phoneNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "phone",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp contactNTP = vendorNT.getNodeTypeProp( "Contact Name" );
            _Mappings.Add( contactNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = contactNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "phone",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp vendorLegacyIdNTP = vendorNT.getNodeTypeProp( "Legacy Id" );
            CswNbtFieldTypeRuleNumber numberFTR = (CswNbtFieldTypeRuleNumber) vendorLegacyIdNTP.getFieldTypeRule();
            _Mappings.Add( "Vendor_" + vendorLegacyIdNTP.PropName, new CAFMapping
            {
                NodeTypeId = vendorNT.NodeTypeId,
                NodeTypePropId = vendorLegacyIdNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "vendorid",
                        NBTSubfield = numberFTR.ValueSubField
                    }
                }
            } );
            #endregion

            #region Chemical

            CswNbtMetaDataNodeType chemicalNT = _NBTResources.MetaData.getNodeType( "Chemical" );

            CswNbtMetaDataNodeTypeProp tradenameNTP = chemicalNT.getNodeTypeProp( "Tradename" );
            _Mappings.Add( tradenameNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = tradenameNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "materialname",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp casNoNTP = chemicalNT.getNodeTypeProp( "CAS No" );
            _Mappings.Add( casNoNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = casNoNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "casno",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp specGravNTP = chemicalNT.getNodeTypeProp( "Specific Gravity" );
            _Mappings.Add( specGravNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = specGravNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "specific_gravity",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp formulaNTP = chemicalNT.getNodeTypeProp( "Formula" );
            _Mappings.Add( formulaNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = formulaNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "formula",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp structureNTP = chemicalNT.getNodeTypeProp( "Structure" );
            CswNbtFieldTypeRuleMol molFTR = (CswNbtFieldTypeRuleMol) structureNTP.getFieldTypeRule();
            _Mappings.Add( structureNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = structureNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "struct_pict",
                        NBTSubfield = molFTR.MolSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp partNumNTP = chemicalNT.getNodeTypeProp( "Part Number" );
            _Mappings.Add( partNumNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = partNumNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "productno",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp expIntNTP = chemicalNT.getNodeTypeProp( "Expiration Interval" );
            CswNbtFieldTypeRuleQuantity expInt_quantFTR = (CswNbtFieldTypeRuleQuantity) expIntNTP.getFieldTypeRule();
            _Mappings.Add( expIntNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = expIntNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "expireinterval",
                        NBTSubfield = expInt_quantFTR.QuantitySubField
                    },
                    new CAFSubfieldMapping{
                        CAFColName = "expireintervalunits",
                        NBTSubfield = expInt_quantFTR.UnitNameSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp boilingPtNTP = chemicalNT.getNodeTypeProp( "Boiling Point" );
            _Mappings.Add( boilingPtNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = boilingPtNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "boiling_point",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp aquSolNTP = chemicalNT.getNodeTypeProp( "Aqueous Solubility" );
            _Mappings.Add( aquSolNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = aquSolNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "aqueous_solubility",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp meltingPtNTP = chemicalNT.getNodeTypeProp( "Melting Point" );
            _Mappings.Add( meltingPtNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = meltingPtNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "melting_point",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp physStateNTP = chemicalNT.getNodeTypeProp( "Physical State" );
            CswNbtFieldTypeRuleList listFTR = (CswNbtFieldTypeRuleList) physStateNTP.getFieldTypeRule();
            _Mappings.Add( physStateNTP.PropName, new CAFMapping
            {
                MappingDictionaryName = CafTranslator.CafTranslationDictionaryNames.PhysicalState,
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = physStateNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "physical_state",
                        NBTSubfield = listFTR.ValueSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp vaporPressureNTP = chemicalNT.getNodeTypeProp( "Vapor Pressure" );
            _Mappings.Add( vaporPressureNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = vaporPressureNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "vapor_pressure",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp vaporDensityNTP = chemicalNT.getNodeTypeProp( "Vapor Density" );
            _Mappings.Add( vaporDensityNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = vaporDensityNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "vapor_density",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp molWeightNTP = chemicalNT.getNodeTypeProp( "Molecular Weight" );
            _Mappings.Add( molWeightNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = molWeightNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "molecular_weight",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp flashPtNTP = chemicalNT.getNodeTypeProp( "Flash Point" );
            _Mappings.Add( flashPtNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = flashPtNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "flash_point",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp phNTP = chemicalNT.getNodeTypeProp( "pH" );
            _Mappings.Add( phNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = phNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "ph",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp physDescriptNTP = chemicalNT.getNodeTypeProp( "Physical Description" );
            CswNbtFieldTypeRuleMemo memoFTR = (CswNbtFieldTypeRuleMemo) physDescriptNTP.getFieldTypeRule();
            _Mappings.Add( physDescriptNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = physDescriptNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "physical_description",
                        NBTSubfield = memoFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp materialLegacyIdNTP = chemicalNT.getNodeTypeProp( "Legacy Id" );
            _Mappings.Add( "Chemical_" + materialLegacyIdNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = materialLegacyIdNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "packageid", //packageid NOT material id - this is intentional, in CAF we can get to the material from the packageid
                        NBTSubfield = numberFTR.ValueSubField
                    }
                }
            } );
            #endregion

            #region Synonyms

            CswNbtMetaDataNodeType materialSynonymNT = _NBTResources.MetaData.getNodeType( "Material Synonym" );
            CswNbtMetaDataNodeTypeProp nameNTP = materialSynonymNT.getNodeTypeProp( "Name" );
            _Mappings.Add( "Name", new CAFMapping
            {
                NodeTypeId = materialSynonymNT.NodeTypeId,
                NodeTypePropId = nameNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>
                {
                    new CAFSubfieldMapping{
                        CAFColName = "synonymname",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp materialSynLegacyIdNTP = materialSynonymNT.getNodeTypeProp( "Legacy Id" );
            _Mappings.Add( "Material Synonym_" + materialLegacyIdNTP.PropName, new CAFMapping
            {
                NodeTypeId = materialSynonymNT.NodeTypeId,
                NodeTypePropId = materialSynLegacyIdNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "materialsynonymid",
                        NBTSubfield = numberFTR.ValueSubField
                    }
                }
            } );

            #endregion

            #region Sizes

            CswNbtMetaDataNodeType sizeNT = _NBTResources.MetaData.getNodeType( "Size" );

            CswNbtMetaDataNodeTypeProp initQuantNTP = sizeNT.getNodeTypeProp( "Initial Quantity" );
            CswNbtFieldTypeRuleQuantity quantityFTR = (CswNbtFieldTypeRuleQuantity) initQuantNTP.getFieldTypeRule();
            _Mappings.Add( initQuantNTP.PropName, new CAFMapping
            {
                NodeTypeId = sizeNT.NodeTypeId,
                NodeTypePropId = initQuantNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "capacity",
                        NBTSubfield = quantityFTR.QuantitySubField
                    },
                    new CAFSubfieldMapping{ 
                        CAFColName = "unitofmeasurename",
                        NBTSubfield =  quantityFTR.UnitNameSubField
                    },
                    new CAFSubfieldMapping{ 
                        CAFColName = "unitofmeasureid", //legacy id on UoM
                        NBTSubfield =  quantityFTR.UnitIdSubField,
                        ExpectedObjClassId = _NBTResources.MetaData.getObjectClassId(NbtObjectClass.UnitOfMeasureClass)
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp catalogNoNTP = sizeNT.getNodeTypeProp( "Catalog No" );
            CswNbtFieldTypeRuleText catalogNTP_TextFTR = (CswNbtFieldTypeRuleText) catalogNoNTP.getFieldTypeRule();
            _Mappings.Add( catalogNoNTP.PropName, new CAFMapping
            {
                NodeTypeId = sizeNT.NodeTypeId,
                NodeTypePropId = catalogNoNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "catalogno",
                        NBTSubfield = catalogNTP_TextFTR.TextSubField
                    }
                }
            } );

            CswNbtMetaDataNodeTypeProp sizeLegacyIdNTP = sizeNT.getNodeTypeProp( "Legacy Id" );
            _Mappings.Add( "Size_" + sizeLegacyIdNTP.PropName, new CAFMapping
            {
                NodeTypeId = sizeNT.NodeTypeId,
                NodeTypePropId = sizeLegacyIdNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "packdetailid",
                        NBTSubfield = numberFTR.ValueSubField
                    }
                }
            } );

            #endregion

            #region Units of Measures

            #endregion
        }

        private string _buildWhereClause()
        {
            string sql = "";
            Dictionary<string, CswCommaDelimitedString> tables = new Dictionary<string, CswCommaDelimitedString>()
            {
                {"materials", materialIds},
                {"materials_synonyms", matSynIds},
                {"vendors", vendorIds},
                {"packdetail", sizeIds},
                {"units_of_measure", unitIds}
            };

            bool first = true;
            foreach( var item in tables )
            {
                if( false == item.Value.IsEmpty )
                {
                    if( first )
                    {
                        first = false;
                    }
                    else
                    {
                        sql += " or ";
                    }
                    sql += _getClauseForIds( item.Value, item.Key );
                }
            }
            return sql;
        }

        private string _getClauseForIds( CswCommaDelimitedString ids, string TableName )
        {
            string clause = "(tablename = '" + TableName + "'" + " and ( itempk in ( " + ids.ToString() + " )))";
            return clause;
        }

        private void _addNodeTypeProps( CswNbtNode Node, DataRow Row, CswNbtMetaDataNodeType NodeType )
        {
            //CswNbtMetaDataNodeType NodeType = _NBTResources.MetaData.getNodeType( Node.NodeTypeId );
            foreach( CswNbtMetaDataNodeTypeProp ntp in NodeType.getNodeTypeProps() )
            {
                string PropName = ntp.PropName;
                if( PropName.Equals( "Legacy Id" ) )
                {
                    PropName = NodeType.NodeTypeName + "_" + PropName;
                }
                if( null != Node.Properties[ntp] && _Mappings.ContainsKey( PropName ) )
                {
                    CAFMapping mapping = _Mappings[PropName];
                    if( null != Node.Properties[ntp] )
                    {
                        switch( Node.Properties[ntp].getFieldType().FieldType )
                        {
                            case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                                foreach( CAFSubfieldMapping subfield in mapping.Subfields )
                                {
                                    if( false == string.IsNullOrEmpty( Row[subfield.CAFColName].ToString() ) )
                                    {
                                        string molData = System.Text.Encoding.UTF8.GetString( (byte[]) Row[subfield.CAFColName] );
                                        Node.Properties[ntp].SetPropRowValue( subfield.NBTSubfield.Column, molData );
                                    }
                                }
                                break;
                            default:
                                foreach( CAFSubfieldMapping subfield in mapping.Subfields )
                                {
                                    string nbtValue = _CafTranslator.Translate( mapping.MappingDictionaryName, Row[subfield.CAFColName].ToString() );
                                    if( subfield.ExpectedObjClassId != Int32.MinValue ) //indicates we're looking for an FK
                                    {
                                        nbtValue = _getNodeIdFromLegacyId( nbtValue, subfield.ExpectedObjClassId ).ToString();
                                    }
                                    Node.Properties[ntp].SetPropRowValue( subfield.NBTSubfield.Column, nbtValue );
                                }
                                break;
                        }
                        Node.Properties[ntp].SyncGestalt();
                    } //if( null != Node.Properties[ntp] )
                } //if( null != Node.Properties[ntp] && _Mappings.ContainsKey( ntp.PropName ) )
            } //foreach( CswNbtMetaDataNodeTypeProp ntp in nodeType.getNodeTypeProps() )
        }

        private int _getNodeIdFromLegacyId( string LegacyId, int ExpectedOCId )
        {
            int NodeId = Int32.MinValue;

            CswNbtMetaDataObjectClass expectedOC = _NBTResources.MetaData.getObjectClass( ExpectedOCId );

            //it doesn't matter what NT we get the legacy id from
            CswNbtMetaDataNodeTypeProp legacyIdNTP = null;
            foreach( CswNbtMetaDataNodeType uomNT in expectedOC.getNodeTypes() )
            {
                legacyIdNTP = uomNT.getNodeTypeProp( "Legacy Id" );
                break;
            }

            CswNbtView unitsView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = unitsView.AddViewRelationship( expectedOC, false );

            unitsView.AddViewPropertyAndFilter( parent,
                MetaDataProp: legacyIdNTP,
                Value: LegacyId,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree tree = _NBTResources.Trees.getTreeFromView( unitsView, false, false, true );
            int count = tree.getChildNodeCount();
            for( int i = 0; i < count; i++ )
            {
                tree.goToNthChild( i );
                NodeId = tree.getNodeIdForCurrentPosition().PrimaryKey;
                tree.goToParentNode();
            }
            return NodeId;
        }

        #endregion

        #region Unit of Measure Creation

        /// <summary>
        /// Import the Units of Measure if they have not already been imported, returns true if UoMs had to be imported
        /// </summary>
        /// <returns></returns>
        private void _importUnitsOfMeasure( string UnitOfMeasurePK = "" )
        {
            string sql = "select * from units_of_measure uom" + ( String.IsNullOrEmpty( UnitOfMeasurePK ) ?
                " join nbtimportqueue niq on niq.itempk = uom.unitofmeasureid where tablename = 'units_of_measure' and state = 'N'" : ( "where uom.unitofmeasureid = " + UnitOfMeasurePK ) );
            CswArbitrarySelect arbSelect = _CAFResources.makeCswArbitrarySelect( "GetUoMs_28122", sql );
            DataTable cafUoMs = arbSelect.getTable();

            foreach( DataRow Row in cafUoMs.Rows )
            {
                string unitName = Row["unitofmeasurename"].ToString();
                CswNbtObjClassUnitOfMeasure unitOfMeasure = _getExistingUnitOfMeasure( unitName );
                double baseVal = Double.MinValue;
                int expVal = Int32.MinValue;
                if( null == unitOfMeasure )
                {
                    string unitType = Row["unittype"].ToString();
                    CswNbtMetaDataNodeType UoM_NT = null;

                    if( unitType.ToLower().Equals( CswNbtObjClassUnitOfMeasure.UnitTypes.Weight._Name.ToLower() ) )
                    {
                        UoM_NT = UoM_weight_NT;
                        baseVal = CswConvert.ToDouble( Row["converttokgs_base"] );
                        expVal = CswConvert.ToInt32( Row["converttokgs_exp"] );
                    }
                    else if( unitType.ToLower().Equals( CswNbtObjClassUnitOfMeasure.UnitTypes.Each._Name.ToLower() ) )
                    {
                        UoM_NT = UoM_each_NT;
                        baseVal = CswConvert.ToDouble( Row["converttoeaches_base"] );
                        expVal = CswConvert.ToInt32( Row["converttoeaches_exp"] );
                    }
                    else if( unitType.ToLower().Equals( CswNbtObjClassUnitOfMeasure.UnitTypes.Volume._Name.ToLower() ) )
                    {
                        UoM_NT = UoM_vol_NT;
                        baseVal = CswConvert.ToDouble( Row["converttoliters_base"] );
                        expVal = CswConvert.ToInt32( Row["converttoliters_exp"] );
                    }
                    else
                    {
                        //My test set only has Weight, Each and Volume - there's still TIME, and RADIATION
                        //We also might have to handle new Unit of Measure types if there are any
                    }

                    unitOfMeasure = _NBTResources.Nodes.makeNodeFromNodeTypeId( UoM_NT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                    unitOfMeasure.Name.Text = unitName;
                    unitOfMeasure.Fractional.Checked = Tristate.False;
                    unitOfMeasure.ConversionFactor.Base = baseVal;
                    unitOfMeasure.ConversionFactor.Exponent = expVal;
                }
                unitOfMeasure.IsTemp = false;
                CswNbtMetaDataNodeTypeProp legacyIdNTP = unitOfMeasure.NodeType.getNodeTypeProp( "Legacy Id" );
                unitOfMeasure.Node.Properties[legacyIdNTP].AsNumber.Value = CswConvert.ToDouble( Row["unitofmeasureid"] ); //set the legacy ID
                unitIds.Add( Row["unitofmeasureid"].ToString() );
                unitOfMeasure.postChanges( false );
            }
        }

        /// <summary>
        /// Gets the existing Unit of Measure if there is one
        /// </summary>
        /// <returns></returns>
        private CswNbtObjClassUnitOfMeasure _getExistingUnitOfMeasure( string Name )
        {
            CswNbtObjClassUnitOfMeasure Ret = null;

            string nbtName = _CafTranslator.Translate( CafTranslator.CafTranslationDictionaryNames.UnitName, Name );

            CswNbtView unitsView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = unitsView.AddViewRelationship( unitOfMeasureOC, false );
            unitsView.AddViewPropertyAndFilter( parent, nameOCP,
                Value: nbtName,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree unitsTree = _NBTResources.Trees.getTreeFromView( unitsView, false, false, true );
            int childCount = unitsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                unitsTree.goToNthChild( i );
                Ret = unitsTree.getNodeForCurrentPosition(); //we need the node
                unitsTree.goToParentNode();
            }

            return Ret;
        }

        #endregion

        #region Vendor creation

        private void _importVendor( string VendorPK )
        {
            string sql = @"select v.vendorname, v.city, v.state, v.street1, v.street2, v.zip, v.accountno, v.fax, v.phone, v.contactname
                                from vendors v where v.vendorid = " + VendorPK;
            CswArbitrarySelect cswArbSelect = _CAFResources.makeCswArbitrarySelect( "cafimport_selectvendors", sql );
            DataTable cafTbl = cswArbSelect.getTable();

            foreach( DataRow Row in cafTbl.Rows )
            {
                _createVendorNode( vendorNT, Row );
            }
        }

        /// <summary>
        /// Creates a new Vendor node if the Vendor does not already exist, otherwise gets the existing Vendor
        /// </summary>
        /// <param name="VendorNT"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        private CswNbtObjClassVendor _createVendorNode( CswNbtMetaDataNodeType VendorNT, DataRow Row )
        {
            CswNbtObjClassVendor vendorNode = _getExistingVendorNode( Row["vendorname"].ToString(), Row["vendorid"].ToString(), VendorNT );
            if( null == vendorNode )
            {
                vendorNode = _NBTResources.Nodes.makeNodeFromNodeTypeId( VendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
            }
            _addNodeTypeProps( vendorNode.Node, Row, vendorNode.NodeType );
            vendorNode.IsTemp = false;
            vendorNode.postChanges( true );
            vendorIds.Add( Row["vendorid"].ToString() );

            return vendorNode;
        }

        private CswNbtObjClassVendor _getExistingVendorNode( string VendorName, string VendorId, CswNbtMetaDataNodeType VendorNT )
        {
            CswNbtObjClassVendor existingVendor = null;

            CswNbtView vendorsView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = vendorsView.AddViewRelationship( VendorNT, false );
            vendorsView.AddViewPropertyAndFilter( parent, vendorLegacyIdNTP,
                Value: VendorId,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree vendorsTree = _NBTResources.Trees.getTreeFromView( vendorsView, false, false, true );
            int childCount = vendorsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                vendorsTree.goToNthChild( i );
                existingVendor = vendorsTree.getNodeForCurrentPosition();
                vendorsTree.goToParentNode();
            }

            if( null == existingVendor ) //if we couldn't find a matching Vendor by Legacy Id
            {
                vendorsView = new CswNbtView( _NBTResources );
                parent = vendorsView.AddViewRelationship( VendorNT, false );
                vendorsView.AddViewPropertyAndFilter( parent, vendorNameNTP,
                    Value: VendorName,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                vendorsTree = _NBTResources.Trees.getTreeFromView( vendorsView, false, false, true );
                childCount = vendorsTree.getChildNodeCount();
                for( int i = 0; i < childCount; i++ )
                {
                    vendorsTree.goToNthChild( i );
                    existingVendor = vendorsTree.getNodeForCurrentPosition();
                    vendorsTree.goToParentNode();
                }
            }

            return existingVendor;
        }

        #endregion

        #region Chemical Creation

        private void _importChemical( string ChemicalPK )
        {
            _importUnitsOfMeasure(); //always check to import UoMs first

            if( unitIds.Count == 0 ) //if we didn't import UoMs
            {
                string sql = @"select 
                                      m.materialname, v.vendorname, p.productno, m.materialid,
                                      m.nbtuptodate as material_uptodate, v.nbtuptodate as vendor_uptodate, p.nbtuptodate as package_uptodate,
                                      m.casno, m.specific_gravity, m.expireinterval, m.expireintervalunits, m.formula, m.struct_pict,
                                      m.physical_state, m.melting_point, m.aqueous_solubility, m.boiling_point, m.vapor_density, m.vapor_pressure, m.molecular_weight, m.flash_point, m.ph,
                                      m.physical_description,
                                      v.vendorname, v.city, v.state, v.street1, v.street2, v.zip, v.accountno, v.fax, v.phone, v.contactname,
                                      p.packageid, v.vendorid
                               from materials m
                                    left join packages p on p.materialid = m.materialid
                                    left join vendors v on v.vendorid = p.supplierid
                                    left join materials_subclass ms on m.materialsubclassid = ms.materialsubclassid and ms.deleted = '0'
                                    left join materials_class mc on ms.materialclassid = mc.materialclassid and mc.deleted = '0' and mc.classname = 'CHEMICAL'
                               where m.deleted = '0' and m.materialid = " + ChemicalPK;

                CswArbitrarySelect cswArbSelect = _CAFResources.makeCswArbitrarySelect( "cafimport_selectmaterials", sql );
                DataTable cafTbl = cswArbSelect.getTable();

                foreach( DataRow row in cafTbl.Rows )
                {
                    string materialId = row["materialid"].ToString();
                    CswNbtObjClassVendor vendorNode = _createVendorNode( vendorNT, row );
                    CswNbtObjClassMaterial materialNode = _createChemical( chemicalNT, row, vendorNode );

                    if( false == materialIds.Contains( materialId ) )
                    {
                        materialIds.Add( row["materialid"].ToString() );
                    }

                    if( null != materialNode )
                    {
                        _createMaterialSynonym( materialId, materialNode );
                        _createSize( materialId, materialNode );
                    }
                }
            }
        }

        private CswNbtObjClassMaterial _createChemical( CswNbtMetaDataNodeType ChemicalNT, DataRow Row, CswNbtObjClassVendor VendorNode )
        {
            CswNbtObjClassMaterial materialNode = _getExistingChemical( Row, ChemicalNT, VendorNode );
            if( null == materialNode )
            {
                materialNode = _NBTResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
            }
            _addNodeTypeProps( materialNode.Node, Row, materialNode.NodeType );
            materialNode.Supplier.RelatedNodeId = VendorNode.NodeId;
            materialNode.Supplier.RefreshNodeName();
            materialNode.IsTemp = false;
            materialNode.postChanges( true );
            _createdMaterials.Add( materialNode );

            string packageId = Row["packageid"].ToString();
            if( false == packageIds.Contains( packageId ) )
            {
                packageIds.Add( packageId );
            }
            return materialNode;
        }

        private CswNbtObjClassMaterial _getExistingChemical( DataRow row, CswNbtMetaDataNodeType ChemicalNT, CswNbtObjClassVendor VendorNode )
        {
            CswNbtObjClassMaterial ExistingChemical = null;

            CswNbtView materialView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship materialParent = materialView.AddViewRelationship( ChemicalNT, false );
            materialView.AddViewPropertyAndFilter( materialParent,
                MetaDataProp: materialLegacyIdNTP,
                Value: row["packageid"].ToString(),
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            CswNbtViewRelationship vendorParent = materialView.AddViewRelationship( materialParent, NbtViewPropOwnerType.Second, supplierNTP, false );
            materialView.AddViewPropertyAndFilter( vendorParent,
                MetaDataProp: vendorLegacyIdNTP,
                Value: row["vendorid"].ToString(),
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree tree = _NBTResources.Trees.getTreeFromView( materialView, false, false, true );
            int childNodeCount = tree.getChildNodeCount();
            for( int i = 0; i < childNodeCount; i++ )
            {
                tree.goToNthChild( i );
                ExistingChemical = tree.getNodeForCurrentPosition();
                tree.goToParentNode();
            }

            if( null == ExistingChemical )
            {
                CAFMapping tradeNameMapping = _Mappings[tradenameNTP.PropName];
                CAFMapping partNoMapping = _Mappings[partNoNTP.PropName];
                string tradeName = row[tradeNameMapping.Subfields[0].CAFColName].ToString(); //not the best
                string supplierName = VendorNode.VendorName.Text;
                string partNo = row[partNoMapping.Subfields[0].CAFColName].ToString(); //not the best

                materialView = new CswNbtView( _NBTResources );

                CswNbtViewRelationship parent = materialView.AddViewRelationship( ChemicalNT, false );
                materialView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: tradenameNTP,
                    Value: tradeName,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                materialView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: supplierNTP,
                    Value: supplierName,
                    SubFieldName: CswNbtSubField.SubFieldName.Name,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                materialView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: partNoNTP,
                    Value: partNo,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                tree = _NBTResources.Trees.getTreeFromView( materialView, false, false, true );
                childNodeCount = tree.getChildNodeCount();
                for( int i = 0; i < childNodeCount; i++ )
                {
                    tree.goToNthChild( i );
                    ExistingChemical = tree.getNodeForCurrentPosition();
                    tree.goToParentNode();
                }
            }

            return ExistingChemical;
        }

        #endregion

        #region Synonym Creation

        private void _importMaterialSynonym( string MaterialSynonymPK )
        {
            string sql = @"select materialid, materialsynonymid from materials_synonyms where deleted = '0' and materialsynonymid = " + MaterialSynonymPK;
            CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect( "cafselect_materialsyn", sql );
            DataTable tbl = arbSel.getTable();

            foreach( DataRow Row in tbl.Rows )
            {
                _importChemical( Row["materialid"].ToString() ); //trigger the chemical import, which will end up importing this material synonym
            }
        }

        private void _createMaterialSynonym( string MaterialId, CswNbtObjClassMaterial ChemicalNode )
        {
            if( null != materialSynNT )
            {
                string sql = @"select synonymname, materialsynonymid from materials_synonyms where deleted = '0' and materialid = " + MaterialId;
                CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect( "cafselect_materialsyn", sql );
                DataTable tbl = arbSel.getTable();

                foreach( DataRow row in tbl.Rows )
                {
                    CswNbtObjClassMaterialSynonym matSyn = _getExistingSynonym( row["materialsynonymid"].ToString(), ChemicalNode.NodeId );
                    if( null == matSyn )
                    {
                        matSyn = _NBTResources.Nodes.makeNodeFromNodeTypeId( materialSynNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                    }
                    CAFMapping mapping = _Mappings[matSyn.Name.PropName];
                    _addNodeTypeProps( matSyn.Node, row, matSyn.NodeType );
                    matSyn.Material.RelatedNodeId = ChemicalNode.NodeId;
                    matSyn.IsTemp = false;
                    matSyn.postChanges( true );

                    string matSynId = row["materialsynonymid"].ToString();
                    if( false == matSynIds.Contains( matSynId ) )
                    {
                        matSynIds.Add( matSynId );
                    }
                }
            }
        }

        private CswNbtObjClassMaterialSynonym _getExistingSynonym( string SynonymId, CswPrimaryKey ChemicalNodeId )
        {
            CswNbtObjClassMaterialSynonym ExistingSynonym = null;

            CswNbtView synonymsView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = synonymsView.AddViewRelationship( materialSynNT, false );

            synonymsView.AddViewPropertyAndFilter( parent, synonymMaterialNTP,
                Value: ChemicalNodeId.PrimaryKey.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            synonymsView.AddViewPropertyAndFilter( parent, synonymLegacyIdNTP,
                Value: SynonymId,
                SubFieldName: CswNbtSubField.SubFieldName.Value,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree tree = _NBTResources.Trees.getTreeFromView( synonymsView, false, true, true );
            int childNodeCount = tree.getChildNodeCount();
            for( int i = 0; i < childNodeCount; i++ )
            {
                tree.goToNthChild( i );
                ExistingSynonym = tree.getNodeForCurrentPosition();
                tree.goToParentNode();
            }

            return ExistingSynonym;
        }

        #endregion

        #region Size Creation

        private void _importSize( string PackdetailPK )
        {
            string sql = @"select p.materialid from packdetail
                                left join packages p on p.packageid = pd.packageid
                           where pd.packdetailid = " + PackdetailPK;
            CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect( "cafselect_size", sql );
            DataTable tbl = arbSel.getTable();

            foreach( DataRow Row in tbl.Rows )
            {
                _importChemical( Row["materialid"].ToString() ); //trigger the chemical import, which will end up importing this size
            }
        }

        private void _createSize( string MaterialId, CswNbtObjClassMaterial ChemicalNode )
        {
            if( null != sizeNT )
            {
                string sql = @"select pd.capacity, pd.catalogno, uom.unitofmeasurename, uom.unitofmeasureid, uom.unittype, pd.packdetailid from packages p
                                   left join packdetail pd on p.packageid = pd.packageid
                                   left join units_of_measure uom on pd.unitofmeasureid = uom.unitofmeasureid
                               where pd.deleted = '0' and p.materialid = " + MaterialId;
                CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect( "cafselect_materialsize", sql );
                DataTable tbl = arbSel.getTable();

                foreach( DataRow row in tbl.Rows )
                {
                    CswNbtObjClassSize sizeNode = _getExistingSize( sizeNT, row["packdetailid"].ToString(), ChemicalNode.NodeId );
                    if( null == sizeNode )
                    {
                        sizeNode = _NBTResources.Nodes.makeNodeFromNodeTypeId( sizeNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                    }
                    _addNodeTypeProps( sizeNode.Node, row, sizeNode.NodeType );
                    sizeNode.Material.RelatedNodeId = ChemicalNode.NodeId;
                    sizeNode.IsTemp = false;
                    sizeNode.postChanges( true );
                    _createdSizes.Add( sizeNode );
                    string sizeId = row["packdetailid"].ToString();
                    if( false == sizeIds.Contains( sizeId ) )
                    {
                        sizeIds.Add( row["packdetailid"].ToString() );
                    }
                }
            }
        }

        private CswNbtObjClassSize _getExistingSize( CswNbtMetaDataNodeType SizeNT, string SizeId, CswPrimaryKey ChemicalNodeId )
        {
            CswNbtObjClassSize ExistingSizeNode = null;

            CswNbtView sizesView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = sizesView.AddViewRelationship( SizeNT, false );

            sizesView.AddViewPropertyAndFilter( parent, sizeLegacyId,
                Value: SizeId,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            sizesView.AddViewPropertyAndFilter( parent, materialNTP,
                Value: ChemicalNodeId.PrimaryKey.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree sizesTree = _NBTResources.Trees.getTreeFromView( sizesView, false, false, true );
            int childNodeCount = sizesTree.getChildNodeCount();
            for( int i = 0; i < childNodeCount; i++ )
            {
                sizesTree.goToNthChild( i );
                ExistingSizeNode = sizesTree.getNodeForCurrentPosition();
                sizesTree.goToParentNode();
            }

            //if( null == ExistingSizeNode )
            //{
            //    sizesView = new CswNbtView( _NBTResources );
            //    parent = sizesView.AddViewRelationship( SizeNT, false );

            //    sizesView.AddViewPropertyAndFilter( parent, catalogNTP,
            //        Value: PotentialSizeNode.CatalogNo.Text,
            //        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            //    sizesView.AddViewPropertyAndFilter( parent, materialNTP,
            //        Value: PotentialSizeNode.NodeId.PrimaryKey.ToString(),
            //        SubFieldName: CswNbtSubField.SubFieldName.NodeID,
            //        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            //    sizesView.AddViewPropertyAndFilter( parent, quantNTP,
            //        Value: PotentialSizeNode.InitialQuantity.CachedUnitName,
            //        SubFieldName: CswNbtSubField.SubFieldName.Name,
            //        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            //    sizesView.AddViewPropertyAndFilter( parent, quantNTP,
            //        Value: CswConvert.ToString( PotentialSizeNode.InitialQuantity.Quantity ),
            //        SubFieldName: CswNbtSubField.SubFieldName.Value,
            //        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            //    sizesTree = _NBTResources.Trees.getTreeFromView( sizesView, false, false, true );
            //    childNodeCount = sizesTree.getChildNodeCount();
            //    for( int i = 0; i < childNodeCount; i++ )
            //    {
            //        sizesTree.goToNthChild( i );
            //        ExistingSizeNode = sizesTree.getNodeForCurrentPosition();
            //        sizesTree.goToParentNode();
            //    }
            //}

            return ExistingSizeNode;
        }

        #endregion
    }

    public sealed class CAFMapping
    {
        public int NodeTypeId;
        public int NodeTypePropId;
        public Collection<CAFSubfieldMapping> Subfields;
        public string MappingDictionaryName = CafTranslator.CafTranslationDictionaryNames.NONE;

        public CswNbtMetaDataNodeType NBTNodeType( CswNbtResources NbtResources )
        {
            return NbtResources.MetaData.getNodeType( NodeTypeId );
        }
        public CswNbtMetaDataNodeTypeProp NBTProp( CswNbtResources NbtResources )
        {
            return NbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
        }
    }

    public sealed class CAFSubfieldMapping
    {
        public string CAFColName;
        public CswNbtSubField NBTSubfield;
        public int ExpectedObjClassId = Int32.MinValue; //for FKs
    }

}

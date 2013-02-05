using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;

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

        //private Stopwatch RowTimer = new Stopwatch();
        //private Stopwatch SelectTimer = new Stopwatch();
        //private Stopwatch MakeNodeTimer = new Stopwatch();
        //private Stopwatch UpdateTimer = new Stopwatch();

        //private Stopwatch MaterialTimer = new Stopwatch();
        //private Stopwatch VendorTimer = new Stopwatch();
        //private Stopwatch SizeTimer = new Stopwatch();
        //private Stopwatch SynTimer = new Stopwatch();

        private Stopwatch PostChangesTimer = new Stopwatch();

        public CAFImportManager(CswNbtResources NBTResources, Int32 NumberToProcess)
        {
            _NBTResources = NBTResources;
            _NumberToProcess = NumberToProcess;
            _initMappings();
            _CafTranslator = new CafTranslator();
        }

        public void Import()
        {
            _CAFResources = CswNbtResourcesFactory.makeCswNbtResources(_NBTResources);
            _CAFResources.AccessId = "cispro";

            Collection<string> unitIds = _importUnitsOfMeasure(); //always check to import UoMs first
            Collection<string> materialIds = new Collection<string>();
            Collection<string> matSynIds = new Collection<string>();
            Collection<string> sizeIds = new Collection<string>();

            if (unitIds.Count == 0) //if we didn't import UoMs
            {
                string sql = @"with vandp as (
                                  select distinct 
                                      v.vendorname, v.city, v.state, v.street1, v.street2, v.zip, v.accountno, v.fax, v.phone, v.contactname, 
                                      p.productno, p.materialid 
                                  from packages p
                                      left join vendors v on v.vendorid = p.supplierid and v.deleted = '0'
                                  where p.deleted = '0')
                           
                           select 
                             m.nbtuptodate, m.materialid, m.materialname, m.casno, m.specific_gravity, m.expireinterval, m.expireintervalunits, m.formula, m.struct_pict,
                             m.physical_state, m.melting_point, m.aqueous_solubility, m.boiling_point, m.vapor_density, m.vapor_pressure, m.molecular_weight, m.flash_point, m.ph,
                             m.physical_description,
                             mc.classname,
                             vp.*
                           from materials m 
                                left join materials_subclass ms on m.materialsubclassid = ms.materialsubclassid and ms.deleted = '0'
                                left join materials_class mc on ms.materialclassid = mc.materialclassid and mc.deleted = '0'
                                left join vandp vp on vp.materialid = m.materialid
                           where mc.classname = 'CHEMICAL' and m.deleted = '0' and nbtuptodate = '0' and rownum <= " + _NumberToProcess + " order by m.materialid ";

                //SelectTimer.Start();
                CswArbitrarySelect cswArbSelect = _CAFResources.makeCswArbitrarySelect("cafimport_selectmaterials", sql);
                DataTable cafTbl = cswArbSelect.getTable();
                //SelectTimer.Stop();

                CswNbtMetaDataNodeType vendorNT = _NBTResources.MetaData.getNodeType("Vendor");
                CswNbtMetaDataNodeType chemicalNT = _NBTResources.MetaData.getNodeType("Chemical");
                foreach (DataRow row in cafTbl.Rows)
                {
                    //RowTimer.Start();
                    string materialId = row["materialid"].ToString();
                    CswNbtObjClassVendor vendorNode = _createVendorNode(vendorNT, row);
                    CswNbtObjClassMaterial materialNode = _createChemical(chemicalNT, row, vendorNode);
                    materialIds.Add(row["materialid"].ToString());

                    if (null != materialNode)
                    {
                        matSynIds = _createMaterialSynonym(row["materialid"].ToString(), materialNode);
                        sizeIds = _createSize(row["materialid"].ToString(), materialNode);
                    }
                    //RowTimer.Stop();
                }
            }

            //UpdateTimer.Start();
            if (materialIds.Count > 0)
            {
                _updateCAFTable(materialIds, "materials", "materialid");
            }
            if (matSynIds.Count > 0)
            {
                _updateCAFTable(matSynIds, "materials_synonyms", "materialsynonymid");
            }
            if (sizeIds.Count > 0)
            {
                _updateCAFTable(sizeIds, "packdetail", "packdetailid");
            }
            if (unitIds.Count > 0)
            {
                _updateCAFTable(unitIds, "units_of_measure", "unitofmeasureid");
            }
            //UpdateTimer.Stop();

            //Important!!! Always manually release resources after use
            _CAFResources.finalize();
            _CAFResources.release();
            _CAFResources = null;

            //write timer stuff to txt file
            //TextWriter tw = new StreamWriter( @"C:\log\CAFImport.txt" );
            //tw.WriteLine( "Total Row Time: " + RowTimer.ElapsedMilliseconds );
            //tw.WriteLine( "Total Select Time: " + SelectTimer.ElapsedMilliseconds );
            //tw.WriteLine( "Total Make Node Time: " + MakeNodeTimer.ElapsedMilliseconds );
            //tw.WriteLine( "Total Update Time: " + UpdateTimer.ElapsedMilliseconds );
            //tw.WriteLine( "Total Post Changes Time: " + PostChangesTimer.ElapsedMilliseconds );
            //tw.WriteLine( "Total Material Time: " + MaterialTimer.ElapsedMilliseconds );
            //tw.WriteLine( "Total Vendor Time: " + VendorTimer.ElapsedMilliseconds );
            //tw.Close();

        }

        #region private methods

        private void _initMappings()
        {
            _Mappings = new Dictionary<string, CAFMapping>();

            #region Vendor
            CswNbtMetaDataNodeType vendorNT = _NBTResources.MetaData.getNodeType("Vendor");
            CswNbtMetaDataNodeTypeProp vendorNameNTP = vendorNT.getNodeTypeProp("Vendor Name");
            CswNbtFieldTypeRuleText textFTR = (CswNbtFieldTypeRuleText)vendorNameNTP.getFieldTypeRule();
            _Mappings.Add(vendorNameNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp cityNTP = vendorNT.getNodeTypeProp("City");
            _Mappings.Add(cityNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp street1NTP = vendorNT.getNodeTypeProp("Street1");
            _Mappings.Add(street1NTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp street2NTP = vendorNT.getNodeTypeProp("Street2");
            _Mappings.Add(street2NTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp zipNTP = vendorNT.getNodeTypeProp("Zip");
            _Mappings.Add(zipNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp stateNTP = vendorNT.getNodeTypeProp("State");
            _Mappings.Add(stateNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp faxNTP = vendorNT.getNodeTypeProp("Fax");
            _Mappings.Add(faxNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp phoneNTP = vendorNT.getNodeTypeProp("Phone");
            _Mappings.Add(phoneNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp contactNTP = vendorNT.getNodeTypeProp("Contact Name");
            _Mappings.Add(contactNTP.PropName, new CAFMapping
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
            });
            #endregion

            #region Chemical

            CswNbtMetaDataNodeType chemicalNT = _NBTResources.MetaData.getNodeType("Chemical");

            CswNbtMetaDataNodeTypeProp tradenameNTP = chemicalNT.getNodeTypeProp("Tradename");
            _Mappings.Add(tradenameNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp casNoNTP = chemicalNT.getNodeTypeProp("CAS No");
            _Mappings.Add(casNoNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp specGravNTP = chemicalNT.getNodeTypeProp("Specific Gravity");
            _Mappings.Add(specGravNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp formulaNTP = chemicalNT.getNodeTypeProp("Formula");
            _Mappings.Add(formulaNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp structureNTP = chemicalNT.getNodeTypeProp("Structure");
            CswNbtFieldTypeRuleMol molFTR = (CswNbtFieldTypeRuleMol)structureNTP.getFieldTypeRule();
            _Mappings.Add(structureNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp partNumNTP = chemicalNT.getNodeTypeProp("Part Number");
            _Mappings.Add(partNumNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp expIntNTP = chemicalNT.getNodeTypeProp("Expiration Interval");
            CswNbtFieldTypeRuleQuantity expInt_quantFTR = (CswNbtFieldTypeRuleQuantity)expIntNTP.getFieldTypeRule();
            _Mappings.Add(expIntNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp boilingPtNTP = chemicalNT.getNodeTypeProp("Boiling Point");
            _Mappings.Add(boilingPtNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp aquSolNTP = chemicalNT.getNodeTypeProp("Aqueous Solubility");
            _Mappings.Add(aquSolNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp meltingPtNTP = chemicalNT.getNodeTypeProp("Melting Point");
            _Mappings.Add(meltingPtNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp physStateNTP = chemicalNT.getNodeTypeProp("Physical State");
            CswNbtFieldTypeRuleList listFTR = (CswNbtFieldTypeRuleList)physStateNTP.getFieldTypeRule();
            _Mappings.Add(physStateNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp vaporPressureNTP = chemicalNT.getNodeTypeProp("Vapor Pressure");
            _Mappings.Add(vaporPressureNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp vaporDensityNTP = chemicalNT.getNodeTypeProp("Vapor Density");
            _Mappings.Add(vaporDensityNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp molWeightNTP = chemicalNT.getNodeTypeProp("Molecular Weight");
            _Mappings.Add(molWeightNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp flashPtNTP = chemicalNT.getNodeTypeProp("Flash Point");
            _Mappings.Add(flashPtNTP.PropName, new CAFMapping
            {
                NodeTypeId = chemicalNT.NodeTypeId,
                NodeTypePropId = flashPtNTP.PropId,
                Subfields = new Collection<CAFSubfieldMapping>()
                {
                    new CAFSubfieldMapping{ 
                        CAFColName = "molecular_weight",
                        NBTSubfield = textFTR.TextSubField
                    }
                }
            });

            CswNbtMetaDataNodeTypeProp phNTP = chemicalNT.getNodeTypeProp("pH");
            _Mappings.Add(phNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp physDescriptNTP = chemicalNT.getNodeTypeProp("Physical Description");
            CswNbtFieldTypeRuleMemo memoFTR = (CswNbtFieldTypeRuleMemo)physDescriptNTP.getFieldTypeRule();
            _Mappings.Add(physDescriptNTP.PropName, new CAFMapping
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
            });
            #endregion

            #region Synonyms

            CswNbtMetaDataNodeType materialSynonymNT = _NBTResources.MetaData.getNodeType("Material Synonym");
            CswNbtMetaDataNodeTypeProp nameNTP = materialSynonymNT.getNodeTypeProp("Name");
            _Mappings.Add("Name", new CAFMapping
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
            });

            #endregion

            #region Sizes

            CswNbtMetaDataNodeType sizeNT = _NBTResources.MetaData.getNodeType("Size");

            CswNbtMetaDataNodeTypeProp initQuantNTP = sizeNT.getNodeTypeProp("Initial Quantity");
            CswNbtFieldTypeRuleQuantity quantityFTR = (CswNbtFieldTypeRuleQuantity)initQuantNTP.getFieldTypeRule();
            _Mappings.Add(initQuantNTP.PropName, new CAFMapping
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
            });

            CswNbtMetaDataNodeTypeProp catalogNoNTP = sizeNT.getNodeTypeProp("Catalog No");
            CswNbtFieldTypeRuleText catalogNTP_TextFTR = (CswNbtFieldTypeRuleText)catalogNoNTP.getFieldTypeRule();
            _Mappings.Add(catalogNoNTP.PropName, new CAFMapping
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
            });
            #endregion

            #region Units of Measures

            #endregion
        }


        private void _updateCAFTable(Collection<string> ids, string tableName, string colName)
        {
            string where = "where ";
            bool first = true;
            foreach (string id in ids)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    where += " or ";
                }
                where += colName + " = " + id;
            }
            CswTableUpdate tu = _CAFResources.makeCswTableUpdate("cafupdate_nbtuptodate_" + tableName, tableName);
            DataTable tbl = tu.getTable(where);
            foreach (DataRow Row in tbl.Rows)
            {
                Row["nbtuptodate"] = CswConvert.ToDbVal(1);
            }
            tu.update(tbl);

        }

        private void _addNodeTypeProps(CswNbtNode Node, DataRow Row)
        {
            CswNbtMetaDataNodeType nodeType = _NBTResources.MetaData.getNodeType(Node.NodeTypeId);
            foreach (CswNbtMetaDataNodeTypeProp ntp in nodeType.getNodeTypeProps())
            {
                if (null != Node.Properties[ntp] && _Mappings.ContainsKey(ntp.PropName))
                {
                    CAFMapping mapping = _Mappings[ntp.PropName];
                    if (null != Node.Properties[ntp])
                    {
                        switch (Node.Properties[ntp].getFieldType().FieldType)
                        {
                            case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                                foreach (CAFSubfieldMapping subfield in mapping.Subfields)
                                {
                                    if (false == string.IsNullOrEmpty(Row[subfield.CAFColName].ToString()))
                                    {
                                        string molData = System.Text.Encoding.UTF8.GetString((byte[])Row[subfield.CAFColName]);
                                        Node.Properties[ntp].SetPropRowValue(subfield.NBTSubfield.Column, molData);
                                    }
                                }
                                break;
                            default:
                                string gestalt = "";
                                foreach (CAFSubfieldMapping subfield in mapping.Subfields)
                                {
                                    string nbtValue = _CafTranslator.Translate(mapping.MappingDictionaryName, Row[subfield.CAFColName].ToString());
                                    if (subfield.ExpectedObjClassId != Int32.MinValue) //indicates we're looking for an FK
                                    {
                                        nbtValue = _getNodeIdFromLegacyId(nbtValue, subfield.ExpectedObjClassId).ToString();
                                    }
                                    else
                                    {
                                        gestalt += " " + nbtValue;
                                    }
                                    Node.Properties[ntp].SetPropRowValue(subfield.NBTSubfield.Column, nbtValue);
                                }
                                break;
                        }
                        Node.Properties[ntp].SyncGestalt();
                    } //if( null != Node.Properties[ntp] )
                } //if( null != Node.Properties[ntp] && _Mappings.ContainsKey( ntp.PropName ) )
            } //foreach( CswNbtMetaDataNodeTypeProp ntp in nodeType.getNodeTypeProps() )
        }

        #region Unit of Measure Creation

        /// <summary>
        /// Import the Units of Measure if they have not already been imported, returns true if UoMs had to be imported
        /// </summary>
        /// <returns></returns>
        private Collection<string> _importUnitsOfMeasure()
        {
            Collection<string> unitIds = new Collection<string>();
            string sql = "select * from units_of_measure where nbtuptodate = '0'";
            CswArbitrarySelect arbSelect = _CAFResources.makeCswArbitrarySelect("GetUoMs_28122", sql);
            DataTable cafUoMs = arbSelect.getTable();

            if (cafUoMs.Rows.Count > 0)
            {
                foreach (DataRow Row in cafUoMs.Rows)
                {
                    string unitName = Row["unitofmeasurename"].ToString();
                    CswNbtObjClassUnitOfMeasure unitOfMeasure = _getExistingUnitOfMeasure(unitName);
                    double baseVal = Double.MinValue;
                    int expVal = Int32.MinValue;
                    if (null == unitOfMeasure)
                    {
                        string unitType = Row["unittype"].ToString();
                        CswNbtMetaDataNodeType UoM_NT = null;

                        if (unitType.ToLower().Equals(CswNbtObjClassUnitOfMeasure.UnitTypes.Weight._Name.ToLower()))
                        {
                            UoM_NT = _NBTResources.MetaData.getNodeType("Unit (Weight)");
                            baseVal = CswConvert.ToDouble(Row["converttokgs_base"]);
                            expVal = CswConvert.ToInt32(Row["converttokgs_exp"]);
                        }
                        else if (unitType.ToLower().Equals(CswNbtObjClassUnitOfMeasure.UnitTypes.Each._Name.ToLower()))
                        {
                            UoM_NT = _NBTResources.MetaData.getNodeType("Unit (Each)");
                            baseVal = CswConvert.ToDouble(Row["converttoeaches_base"]);
                            expVal = CswConvert.ToInt32(Row["converttoeaches_exp"]);
                        }
                        else if (unitType.ToLower().Equals(CswNbtObjClassUnitOfMeasure.UnitTypes.Volume._Name.ToLower()))
                        {
                            UoM_NT = _NBTResources.MetaData.getNodeType("Unit (Volume)");
                            baseVal = CswConvert.ToDouble(Row["converttoliters_base"]);
                            expVal = CswConvert.ToInt32(Row["converttoliters_exp"]);
                        }
                        else
                        {
                            //My test set only has Weight, Each and Volume - there's still TIME, and RADIATION
                            //We also might have to handle new Unit of Measure types if there are any
                        }

                        unitOfMeasure = _NBTResources.Nodes.makeNodeFromNodeTypeId(UoM_NT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp);
                        unitOfMeasure.Name.Text = unitName;
                        unitOfMeasure.Fractional.Checked = Tristate.False;
                        unitOfMeasure.ConversionFactor.Base = baseVal;
                        unitOfMeasure.ConversionFactor.Exponent = expVal;
                    }
                    unitOfMeasure.IsTemp = false;
                    //unitOfMeasure.LegacyId.Value = CswConvert.ToDouble( Row["unitofmeasureid"] ); //set the legacy ID
                    unitIds.Add(Row["unitofmeasureid"].ToString());
                    unitOfMeasure.postChanges(false);
                }
            }
            return unitIds;
        }

        /// <summary>
        /// Gets the existing Unit of Measure if there is one
        /// </summary>
        /// <returns></returns>
        private CswNbtObjClassUnitOfMeasure _getExistingUnitOfMeasure(string Name)
        {
            CswNbtObjClassUnitOfMeasure Ret = null;

            string nbtName = _CafTranslator.Translate(CafTranslator.CafTranslationDictionaryNames.UnitName, Name);

            CswNbtMetaDataObjectClass unitOfMeasureOC = _NBTResources.MetaData.getObjectClass(NbtObjectClass.UnitOfMeasureClass);
            CswNbtMetaDataObjectClassProp nameOCP = unitOfMeasureOC.getObjectClassProp(CswNbtObjClassUnitOfMeasure.PropertyName.Name);

            CswNbtView unitsView = new CswNbtView(_NBTResources);
            CswNbtViewRelationship parent = unitsView.AddViewRelationship(unitOfMeasureOC, false);
            unitsView.AddViewPropertyAndFilter(parent, nameOCP,
                Value: nbtName,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            ICswNbtTree unitsTree = _NBTResources.Trees.getTreeFromView(unitsView, false, false, true);
            int childCount = unitsTree.getChildNodeCount();
            for (int i = 0; i < childCount; i++)
            {
                unitsTree.goToNthChild(i);
                Ret = unitsTree.getNodeForCurrentPosition(); //we need the node
                unitsTree.goToParentNode();
            }

            return Ret;
        }

        #endregion

        #region Vendor creation

        /// <summary>
        /// Creates a new Vendor node if the Vendor does not already exist, otherwise gets the existing Vendor
        /// </summary>
        /// <param name="VendorNT"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        private CswNbtObjClassVendor _createVendorNode(CswNbtMetaDataNodeType VendorNT, DataRow Row)
        {
            //VendorTimer.Start();
            CswNbtObjClassVendor vendorNode = _getExistingVendorNode(Row["vendorname"].ToString(), VendorNT);
            if (null == vendorNode)
            {
                //MakeNodeTimer.Start();
                vendorNode = _NBTResources.Nodes.makeNodeFromNodeTypeId(VendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp);
                //MakeNodeTimer.Stop();
                _addNodeTypeProps(vendorNode.Node, Row);
                vendorNode.IsTemp = false;
                PostChangesTimer.Start();
                vendorNode.postChanges(true);
                PostChangesTimer.Stop();
            }
            //VendorTimer.Stop();
            return vendorNode;
        }

        private CswNbtObjClassVendor _getExistingVendorNode(string VendorName, CswNbtMetaDataNodeType VendorNT)
        {
            CswNbtObjClassVendor existingVendor = null;

            CswNbtMetaDataNodeTypeProp vendorNameNTP = VendorNT.getNodeTypePropByObjectClassProp(CswNbtObjClassVendor.PropertyName.VendorName);
            CswNbtView vendorsView = new CswNbtView(_NBTResources);
            CswNbtViewRelationship parent = vendorsView.AddViewRelationship(VendorNT, false);
            vendorsView.AddViewPropertyAndFilter(parent, vendorNameNTP,
                Value: VendorName,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);
            ICswNbtTree vendorsTree = _NBTResources.Trees.getTreeFromView(vendorsView, false, false, true);

            int childCount = vendorsTree.getChildNodeCount();
            for (int i = 0; i < childCount; i++)
            {
                vendorsTree.goToNthChild(i);
                existingVendor = vendorsTree.getNodeForCurrentPosition();
                vendorsTree.goToParentNode();
            }

            return existingVendor;
        }

        #endregion

        #region Chemical Creation

        private CswNbtObjClassMaterial _createChemical(CswNbtMetaDataNodeType ChemicalNT, DataRow Row, CswNbtObjClassVendor VendorNode)
        {
            //MaterialTimer.Start();
            CswNbtObjClassMaterial materialNode = null;
            if (false == _doesChemicalExist(Row, ChemicalNT, VendorNode))
            {
                //MakeNodeTimer.Start();
                materialNode = _NBTResources.Nodes.makeNodeFromNodeTypeId(ChemicalNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp);
                //MakeNodeTimer.Stop();
                _addNodeTypeProps(materialNode.Node, Row);
                materialNode.Supplier.RelatedNodeId = VendorNode.NodeId;
                materialNode.Supplier.RefreshNodeName();
                materialNode.IsTemp = false;
                PostChangesTimer.Start();
                materialNode.postChanges(true);
                PostChangesTimer.Stop();
                _createdMaterials.Add(materialNode);
            }
            //MaterialTimer.Stop();
            return materialNode;
        }

        private bool _doesChemicalExist(DataRow row, CswNbtMetaDataNodeType ChemicalNT, CswNbtObjClassVendor VendorNode)
        {
            CswNbtMetaDataNodeTypeProp tradenameNTP = ChemicalNT.getNodeTypePropByObjectClassProp(CswNbtObjClassMaterial.PropertyName.Tradename);
            CswNbtMetaDataNodeTypeProp supplierNTP = ChemicalNT.getNodeTypePropByObjectClassProp(CswNbtObjClassMaterial.PropertyName.Supplier);
            CswNbtMetaDataNodeTypeProp partNoNTP = ChemicalNT.getNodeTypePropByObjectClassProp(CswNbtObjClassMaterial.PropertyName.PartNumber);

            CAFMapping tradeNameMapping = _Mappings[tradenameNTP.PropName];
            CAFMapping partNoMapping = _Mappings[partNoNTP.PropName];
            string tradeName = row[tradeNameMapping.Subfields[0].CAFColName].ToString(); //not the best
            string supplierName = VendorNode.VendorName.Text;
            string partNo = row[partNoMapping.Subfields[0].CAFColName].ToString(); //not the best

            CswNbtView materialView = new CswNbtView(_NBTResources);

            CswNbtViewRelationship parent = materialView.AddViewRelationship(ChemicalNT, false);
            materialView.AddViewPropertyAndFilter(parent,
                MetaDataProp: tradenameNTP,
                Value: tradeName,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            materialView.AddViewPropertyAndFilter(parent,
                MetaDataProp: supplierNTP,
                Value: supplierName,
                SubFieldName: CswNbtSubField.SubFieldName.Name,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            materialView.AddViewPropertyAndFilter(parent,
                MetaDataProp: partNoNTP,
                Value: partNo,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            ICswNbtTree tree = _NBTResources.Trees.getTreeFromView(materialView, false, false, true);
            bool existingChemicalFound = tree.getChildNodeCount() > 0;

            bool recentlyCreatedFound = false;
            foreach (CswNbtObjClassMaterial materialNode in _createdMaterials)
            {
                if (materialNode.TradeName.Text.Equals(tradeName) && materialNode.Supplier.CachedNodeName.Equals(supplierName) && materialNode.PartNumber.Text.Equals(partNo))
                {
                    recentlyCreatedFound = true;
                }
            }

            return existingChemicalFound || recentlyCreatedFound;
        }

        #endregion

        #region Synonym Creation

        private Collection<string> _createMaterialSynonym(string MaterialId, CswNbtObjClassMaterial ChemicalNode)
        {
            CswNbtMetaDataNodeType materialSynNT = _NBTResources.MetaData.getNodeType("Material Synonym");
            Collection<string> materialSynIds = new Collection<string>();

            if (null != materialSynNT)
            {
                string sql = @"select synonymname, materialsynonymid from materials_synonyms where deleted = '0' and nbtuptodate = '0' and materialid = " + MaterialId;
                //SelectTimer.Start();
                CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect("cafselect_materialsyn", sql);
                DataTable tbl = arbSel.getTable();
                //SelectTimer.Stop();

                foreach (DataRow row in tbl.Rows)
                {
                    //MakeNodeTimer.Start();
                    CswNbtObjClassMaterialSynonym matSyn = _NBTResources.Nodes.makeNodeFromNodeTypeId(materialSynNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp);
                    //MakeNodeTimer.Stop();
                    CAFMapping mapping = _Mappings[matSyn.Name.PropName];
                    _addNodeTypeProps(matSyn.Node, row);
                    matSyn.Material.RelatedNodeId = ChemicalNode.NodeId;
                    matSyn.IsTemp = false;
                    PostChangesTimer.Start();
                    matSyn.postChanges(true);
                    PostChangesTimer.Stop();
                    materialSynIds.Add(row["materialsynonymid"].ToString());
                }
            }
            return materialSynIds;
        }

        #endregion

        #region Size Creation

        private Collection<string> _createSize(string MaterialId, CswNbtObjClassMaterial ChemicalNode)
        {
            CswNbtMetaDataNodeType sizeNT = _NBTResources.MetaData.getNodeType("Size");
            Collection<string> sizeIds = new Collection<string>();

            if (null != sizeNT)
            {
                string sql = @"select pd.capacity, pd.catalogno, uom.unitofmeasurename, uom.unitofmeasureid, uom.unittype, pd.packdetailid from packages p
                                   left join packdetail pd on p.packageid = pd.packageid
                                   left join units_of_measure uom on pd.unitofmeasureid = uom.unitofmeasureid
                               where pd.nbtuptodate = '0' and pd.deleted = '0' and p.materialid = " + MaterialId;
                //SelectTimer.Start();
                CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect("cafselect_materialsize", sql);
                DataTable tbl = arbSel.getTable();
                //SelectTimer.Stop();

                foreach (DataRow row in tbl.Rows)
                {
                    //MakeNodeTimer.Start();
                    CswNbtObjClassSize sizeNode = _NBTResources.Nodes.makeNodeFromNodeTypeId(sizeNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp);
                    //MakeNodeTimer.Stop();
                    _addNodeTypeProps(sizeNode.Node, row);
                    sizeNode.Material.RelatedNodeId = ChemicalNode.NodeId;

                    if (false == _doesSizeExist(sizeNT, sizeNode))
                    {
                        sizeNode.IsTemp = false;
                        PostChangesTimer.Start();
                        sizeNode.postChanges(true);
                        PostChangesTimer.Stop();
                        _createdSizes.Add(sizeNode);
                    }
                    sizeIds.Add(row["packdetailid"].ToString());
                }
            }
            return sizeIds;
        }

        private int _getNodeIdFromLegacyId(string LegacyId, int ExpectedOCId)
        {
            int NodeId = Int32.MinValue;

            CswNbtMetaDataObjectClass expectedOC = _NBTResources.MetaData.getObjectClass(ExpectedOCId);
            CswNbtMetaDataObjectClassProp legacyIdOCP = expectedOC.getObjectClassProp("Legacy Id");

            CswNbtView unitsView = new CswNbtView(_NBTResources);
            CswNbtViewRelationship parent = unitsView.AddViewRelationship(expectedOC, false);

            unitsView.AddViewPropertyAndFilter(parent,
                MetaDataProp: legacyIdOCP,
                Value: LegacyId,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            ICswNbtTree tree = _NBTResources.Trees.getTreeFromView(unitsView, false, false, true);
            int count = tree.getChildNodeCount();
            for (int i = 0; i < count; i++)
            {
                tree.goToNthChild(i);
                NodeId = tree.getNodeIdForCurrentPosition().PrimaryKey;
                tree.goToParentNode();
            }
            return NodeId;
        }

        private bool _doesSizeExist(CswNbtMetaDataNodeType SizeNT, CswNbtObjClassSize PotentialSizeNode)
        {
            CswNbtMetaDataNodeTypeProp catalogNTP = SizeNT.getNodeTypePropByObjectClassProp(CswNbtObjClassSize.PropertyName.CatalogNo);
            CswNbtMetaDataNodeTypeProp materialNTP = SizeNT.getNodeTypePropByObjectClassProp(CswNbtObjClassSize.PropertyName.Material);
            CswNbtMetaDataNodeTypeProp quantNTP = SizeNT.getNodeTypePropByObjectClassProp(CswNbtObjClassSize.PropertyName.InitialQuantity);

            CswNbtView sizesView = new CswNbtView(_NBTResources);
            CswNbtViewRelationship parent = sizesView.AddViewRelationship(SizeNT, false);

            sizesView.AddViewPropertyAndFilter(parent, catalogNTP,
                Value: PotentialSizeNode.CatalogNo.Text,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            sizesView.AddViewPropertyAndFilter(parent, materialNTP,
                Value: PotentialSizeNode.NodeId.PrimaryKey.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            sizesView.AddViewPropertyAndFilter(parent, quantNTP,
                Value: PotentialSizeNode.InitialQuantity.CachedUnitName,
                SubFieldName: CswNbtSubField.SubFieldName.Name,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            sizesView.AddViewPropertyAndFilter(parent, quantNTP,
                Value: PotentialSizeNode.InitialQuantity.Quantity.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.Value,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals);

            ICswNbtTree sizesTree = _NBTResources.Trees.getTreeFromView(sizesView, false, false, true);
            bool existingSizeFound = sizesTree.getChildNodeCount() > 0;

            bool createdSizeFound = false;
            foreach (CswNbtObjClassSize sizeNode in _createdSizes)
            {
                if (sizeNode.InitialQuantity.Quantity == PotentialSizeNode.InitialQuantity.Quantity && sizeNode.InitialQuantity.CachedUnitName.Equals(PotentialSizeNode.InitialQuantity.CachedUnitName)
                    && sizeNode.CatalogNo.Text.Equals(PotentialSizeNode.CatalogNo.Text) && sizeNode.Material.RelatedNodeId.Equals(PotentialSizeNode.Material.RelatedNodeId))
                {
                    createdSizeFound = true;
                }
            }

            return (existingSizeFound || createdSizeFound);
        }

        #endregion

        #endregion
    }

    public sealed class CAFMapping
    {
        public int NodeTypeId;
        public int NodeTypePropId;
        public Collection<CAFSubfieldMapping> Subfields;
        public string MappingDictionaryName = CafTranslator.CafTranslationDictionaryNames.NONE;

        public CswNbtMetaDataNodeType NBTNodeType(CswNbtResources NbtResources)
        {
            return NbtResources.MetaData.getNodeType(NodeTypeId);
        }
        public CswNbtMetaDataNodeTypeProp NBTProp(CswNbtResources NbtResources)
        {
            return NbtResources.MetaData.getNodeTypeProp(NodeTypePropId);
        }
    }

    public sealed class CAFSubfieldMapping
    {
        public string CAFColName;
        public CswNbtSubField NBTSubfield;
        public int ExpectedObjClassId = Int32.MinValue; //for FKs
    }

}

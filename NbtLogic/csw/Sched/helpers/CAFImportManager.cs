using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Actions;

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

        //private Stopwatch RowTimer = new Stopwatch();
        //private Stopwatch SelectTimer = new Stopwatch();
        //private Stopwatch MakeNodeTimer = new Stopwatch();
        //private Stopwatch UpdateTimer = new Stopwatch();

        //private Stopwatch MaterialTimer = new Stopwatch();
        //private Stopwatch VendorTimer = new Stopwatch();
        //private Stopwatch SizeTimer = new Stopwatch();
        //private Stopwatch SynTimer = new Stopwatch();

        private Stopwatch PostChangesTimer = new Stopwatch();

        public CAFImportManager( CswNbtResources NBTResources, Int32 NumberToProcess )
        {
            _NBTResources = NBTResources;
            _NumberToProcess = NumberToProcess;
            _initMappings();
            _CafTranslator = new CafTranslator();
        }

        public void Import()
        {
            _CAFResources = CswNbtResourcesFactory.makeCswNbtResources( _NBTResources );
            _CAFResources.AccessId = "cispro";

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
            CswArbitrarySelect cswArbSelect = _CAFResources.makeCswArbitrarySelect( "cafimport_selectmaterials", sql );
            DataTable cafTbl = cswArbSelect.getTable();
            //SelectTimer.Stop();

            Collection<string> materialIds = new Collection<string>();
            Collection<string> matSynIds = new Collection<string>();
            Collection<string> sizeIds = new Collection<string>();
            CswNbtMetaDataNodeType vendorNT = _NBTResources.MetaData.getNodeType( "Vendor" );
            CswNbtMetaDataNodeType chemicalNT = _NBTResources.MetaData.getNodeType( "Chemical" );
            foreach( DataRow row in cafTbl.Rows )
            {
                //RowTimer.Start();
                string materialId = row["materialid"].ToString();
                CswNbtObjClassVendor vendorNode = _createVendorNode( vendorNT, row );
                CswNbtObjClassMaterial materialNode = _createChemical( chemicalNT, row, vendorNode );
                materialIds.Add( row["materialid"].ToString() );

                if( null != materialNode )
                {
                    matSynIds = _createMaterialSynonym( row["materialid"].ToString(), materialNode );
                    sizeIds = _createSize( row["materialid"].ToString(), materialNode );
                }
                //RowTimer.Stop();
            }

            //UpdateTimer.Start();
            if( materialIds.Count > 0 )
            {
                _updateCAFTable( materialIds, "materials", "materialid" );
            }
            if( matSynIds.Count > 0 )
            {
                _updateCAFTable( matSynIds, "materials_synonyms", "materialsynonymid" );
            }
            if( sizeIds.Count > 0 )
            {
                _updateCAFTable( sizeIds, "packdetail", "packdetailid" );
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
            CswNbtMetaDataNodeType vendorNT = _NBTResources.MetaData.getNodeType( "Vendor" );
            _Mappings.Add( "Vendor Name", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "vendorname",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Vendor Name" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "City", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "city",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "City" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Street1", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "street1",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Street1" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Street2", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "street2",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Street2" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Zip", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "zip",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Zip" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "State", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "state",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "State" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Fax", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "fax",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Fax" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Phone", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "phone",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Phone" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Contact Name", new CAFMapping
            {
                NBTNodeTypeId = vendorNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "contactname",
                NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Contact Name" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );
            #endregion

            #region Chemical
            CswNbtMetaDataNodeType chemicalNT = _NBTResources.MetaData.getNodeType( "Chemical" );
            _Mappings.Add( "Tradename", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "materialname",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Tradename" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "CAS No", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "casno",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "CAS No" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Specific Gravity", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "specific_gravity",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Specific Gravity" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Formula", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "formula",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Formula" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Structure", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "struct_pict",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Structure" ).PropId,
                NBTSubFieldPropColName = "ClobData"
            } );

            _Mappings.Add( "Part Number", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "productno",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Part Number" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Expiration Interval", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "expireinterval",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Expiration Interval" ).PropId,
                NBTSubFieldPropColName = "field1_numeric",
                CAFColName2 = "expireintervalunits",
                NBTSubFieldPropColName2 = "field1"
            } );

            _Mappings.Add( "Boiling Point", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "boiling_point",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Boiling Point" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Aqueous Solubility", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "aqueous_solubility",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Aqueous Solubility" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Melting Point", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "melting_point",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Melting Point" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Physical State", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "physical_state",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Physical State" ).PropId,
                NBTSubFieldPropColName = "field1",
                MappingDictionaryName = CafTranslator.CafTranslationDictionaryNames.PhysicalState
            } );

            _Mappings.Add( "Vapor Pressure", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "vapor_pressure",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Vapor Pressure" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Vapor Density", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "vapor_density",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Vapor Density" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Molecular Weight", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "molecular_weight",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Molecular Weight" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Flash Point", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "flash_point",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Flash Point" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "pH", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "ph",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "pH" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            _Mappings.Add( "Physical Description", new CAFMapping
            {
                NBTNodeTypeId = chemicalNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "physical_description",
                NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Physical Description" ).PropId,
                NBTSubFieldPropColName = "gestalt"
            } );

            #endregion

            #region Synonyms

            CswNbtMetaDataNodeType materialSynonymNT = _NBTResources.MetaData.getNodeType( "Material Synonym" );
            _Mappings.Add( "Name", new CAFMapping
            {
                NBTNodeTypeId = materialSynonymNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "synonymname",
                NBTNodeTypePropId = materialSynonymNT.getNodeTypeProp( "Name" ).NodeTypeId,
                NBTSubFieldPropColName = "field1"
            } );

            #endregion

            #region Sizes

            CswNbtMetaDataNodeType sizeNT = _NBTResources.MetaData.getNodeType( "Size" );

            _Mappings.Add( "Initial Quantity", new CAFMapping
            {
                NBTNodeTypeId = sizeNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "capacity",
                NBTNodeTypePropId = sizeNT.getNodeTypeProp( "Initial Quantity" ).PropId,
                NBTSubFieldPropColName = "field1_numeric",
                NBTSubFieldPropColName2 = "field1",
                CAFColName2 = "unitofmeasurename"
            } );

            _Mappings.Add( "Catalog No", new CAFMapping
            {
                NBTNodeTypeId = sizeNT.NodeTypeId,
                CAFTableName = "",
                CAFColName = "catalogno",
                NBTNodeTypePropId = sizeNT.getNodeTypeProp( "Catalog No" ).PropId,
                NBTSubFieldPropColName = "field1"
            } );

            #endregion
        }


        private void _updateCAFTable( Collection<string> ids, string tableName, string colName )
        {
            string where = "where ";
            bool first = true;
            foreach( string id in ids )
            {
                if( first )
                {
                    first = false;
                }
                else
                {
                    where += " or ";
                }
                where += colName + " = " + id;
            }
            CswTableUpdate tu = _CAFResources.makeCswTableUpdate( "cafupdate_nbtuptodate_" + tableName, tableName );
            DataTable tbl = tu.getTable( where );
            foreach( DataRow Row in tbl.Rows )
            {
                Row["nbtuptodate"] = CswConvert.ToDbVal( 1 );
            }
            tu.update( tbl );

        }

        private void _addNodeTypeProps( CswNbtNode Node, DataRow Row )
        {
            CswNbtMetaDataNodeType nodeType = _NBTResources.MetaData.getNodeType( Node.NodeTypeId );
            foreach( CswNbtMetaDataNodeTypeProp ntp in nodeType.getNodeTypeProps() )
            {
                if( null != Node.Properties[ntp] && _Mappings.ContainsKey( ntp.PropName ) )
                {
                    CAFMapping mapping = _Mappings[ntp.PropName];
                    if( null != Node.Properties[ntp] )
                    {
                        switch( Node.Properties[ntp].getFieldType().FieldType )
                        {
                            case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                                CswNbtObjClassUnitOfMeasure unitOfMeasure = _getUnitOfMeasure( Row[mapping.CAFColName2].ToString() );
                                if( null != unitOfMeasure )
                                {
                                    Node.Properties[ntp].SetPropRowValue( (CswNbtSubField.PropColumn) mapping.NBTSubFieldPropColName, Row[mapping.CAFColName].ToString() );
                                    Node.Properties[ntp].SetPropRowValue( (CswNbtSubField.PropColumn) mapping.NBTSubFieldPropColName2, unitOfMeasure.BaseUnit.Text );
                                    Node.Properties[ntp].SetPropRowValue( CswNbtSubField.PropColumn.Field1_FK, unitOfMeasure.NodeId.PrimaryKey );
                                    string sizeGestalt = Row[mapping.CAFColName].ToString() + " " + unitOfMeasure.BaseUnit.Text;
                                    Node.Properties[ntp].SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, sizeGestalt );
                                }
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                                if( false == string.IsNullOrEmpty( Row[mapping.CAFColName].ToString() ) )
                                {
                                    CswNbtSdTabsAndProps tabsPropsSd = new CswNbtSdTabsAndProps( _NBTResources );
                                    string propAttr = new CswPropIdAttr( Node, ntp ).ToString();
                                    string molData = System.Text.Encoding.UTF8.GetString( (byte[]) Row[mapping.CAFColName] );
                                    tabsPropsSd.saveMolProp( molData, propAttr );
                                }
                                break;
                            default:
                                string nbtValue = _CafTranslator.Translate( mapping.MappingDictionaryName, Row[mapping.CAFColName].ToString() );
                                Node.Properties[ntp].SetPropRowValue( (CswNbtSubField.PropColumn) mapping.NBTSubFieldPropColName, nbtValue );
                                Node.Properties[ntp].SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, nbtValue );
                                break;
                        }
                    }
                }
            }
        }

        #region Vendor creation

        /// <summary>
        /// Creates a new Vendor node if the Vendor does not already exist, otherwise gets the existing Vendor
        /// </summary>
        /// <param name="VendorNT"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        private CswNbtObjClassVendor _createVendorNode( CswNbtMetaDataNodeType VendorNT, DataRow Row )
        {
            //VendorTimer.Start();
            CswNbtObjClassVendor vendorNode = _getExistingVendorNode( Row["vendorname"].ToString(), VendorNT );
            if( null == vendorNode )
            {
                //MakeNodeTimer.Start();
                vendorNode = _NBTResources.Nodes.makeNodeFromNodeTypeId( VendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                //MakeNodeTimer.Stop();
                _addNodeTypeProps( vendorNode.Node, Row );
                vendorNode.IsTemp = false;
                PostChangesTimer.Start();
                vendorNode.postChanges( true );
                PostChangesTimer.Stop();
            }
            //VendorTimer.Stop();
            return vendorNode;
        }

        private CswNbtObjClassVendor _getExistingVendorNode( string VendorName, CswNbtMetaDataNodeType VendorNT )
        {
            CswNbtObjClassVendor existingVendor = null;

            CswNbtMetaDataNodeTypeProp vendorNameNTP = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
            CswNbtView vendorsView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = vendorsView.AddViewRelationship( VendorNT, false );
            vendorsView.AddViewPropertyAndFilter( parent, vendorNameNTP,
                Value: VendorName,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
            ICswNbtTree vendorsTree = _NBTResources.Trees.getTreeFromView( vendorsView, false, false, true );

            int childCount = vendorsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                vendorsTree.goToNthChild( i );
                existingVendor = vendorsTree.getNodeForCurrentPosition();
                vendorsTree.goToParentNode();
            }

            return existingVendor;
        }

        #endregion

        #region Chemical Creation

        private CswNbtObjClassMaterial _createChemical( CswNbtMetaDataNodeType ChemicalNT, DataRow Row, CswNbtObjClassVendor VendorNode )
        {
            //MaterialTimer.Start();
            CswNbtObjClassMaterial materialNode = null;
            if( false == _doesChemicalExist( Row, ChemicalNT, VendorNode ) )
            {
                //MakeNodeTimer.Start();
                materialNode = _NBTResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                //MakeNodeTimer.Stop();
                _addNodeTypeProps( materialNode.Node, Row );
                materialNode.Supplier.RelatedNodeId = VendorNode.NodeId;
                materialNode.Supplier.RefreshNodeName();
                materialNode.IsTemp = false;
                PostChangesTimer.Start();
                materialNode.postChanges( true );
                PostChangesTimer.Stop();
                _createdMaterials.Add( materialNode );
            }
            //MaterialTimer.Stop();
            return materialNode;
        }

        private bool _doesChemicalExist( DataRow row, CswNbtMetaDataNodeType ChemicalNT, CswNbtObjClassVendor VendorNode )
        {
            CswNbtMetaDataNodeTypeProp tradenameNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Tradename );
            CswNbtMetaDataNodeTypeProp supplierNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
            CswNbtMetaDataNodeTypeProp partNoNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.PartNumber );

            CAFMapping tradeNameMapping = _Mappings[tradenameNTP.PropName];
            CAFMapping partNoMapping = _Mappings[partNoNTP.PropName];
            string tradeName = row[tradeNameMapping.CAFColName].ToString();
            string supplierName = VendorNode.VendorName.Text;
            string partNo = row[partNoMapping.CAFColName].ToString();

            CswNbtView materialView = new CswNbtView( _NBTResources );

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

            ICswNbtTree tree = _NBTResources.Trees.getTreeFromView( materialView, false, false, true );
            bool existingChemicalFound = tree.getChildNodeCount() > 0;

            //bool recentlyCreatedFound = false;
            //foreach( CswNbtObjClassMaterial materialNode in _createdMaterials )
            //{
            //    if( materialNode.TradeName.Text.Equals( tradeName ) && materialNode.Supplier.CachedNodeName.Equals( supplierName ) && materialNode.PartNumber.Text.Equals( partNo ) )
            //    {
            //        recentlyCreatedFound = true;
            //    }
            //}

            //return ( existingChemicalFound || recentlyCreatedFound );
            return existingChemicalFound;
        }

        #endregion

        #region Synonym Creation

        private Collection<string> _createMaterialSynonym( string MaterialId, CswNbtObjClassMaterial ChemicalNode )
        {
            CswNbtMetaDataNodeType materialSynNT = _NBTResources.MetaData.getNodeType( "Material Synonym" );
            Collection<string> materialSynIds = new Collection<string>();

            if( null != materialSynNT )
            {
                string sql = @"select synonymname, materialsynonymid from materials_synonyms where deleted = '0' and nbtuptodate = '0' and materialid = " + MaterialId;
                //SelectTimer.Start();
                CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect( "cafselect_materialsyn", sql );
                DataTable tbl = arbSel.getTable();
                //SelectTimer.Stop();

                foreach( DataRow row in tbl.Rows )
                {
                    //MakeNodeTimer.Start();
                    CswNbtObjClassMaterialSynonym matSyn = _NBTResources.Nodes.makeNodeFromNodeTypeId( materialSynNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                    //MakeNodeTimer.Stop();
                    CAFMapping mapping = _Mappings[matSyn.Name.PropName];
                    matSyn.Name.Text = row[mapping.CAFColName].ToString();
                    matSyn.Material.RelatedNodeId = ChemicalNode.NodeId;
                    matSyn.IsTemp = false;
                    PostChangesTimer.Start();
                    matSyn.postChanges( true );
                    PostChangesTimer.Stop();
                    materialSynIds.Add( row["materialsynonymid"].ToString() );
                }
            }
            return materialSynIds;
        }

        #endregion

        #region Size Creation

        private Collection<string> _createSize( string MaterialId, CswNbtObjClassMaterial ChemicalNode )
        {
            CswNbtMetaDataNodeType sizeNT = _NBTResources.MetaData.getNodeType( "Size" );
            Collection<string> sizeIds = new Collection<string>();

            if( null != sizeNT )
            {
                string sql = @"select pd.capacity, pd.catalogno, uom.unitofmeasurename, uom.unittype, pd.packdetailid from packages p
                                   left join packdetail pd on p.packageid = pd.packageid
                                   left join units_of_measure uom on pd.unitofmeasureid = uom.unitofmeasureid
                               where pd.nbtuptodate = '0' and pd.deleted = '0' and p.materialid = " + MaterialId;
                //SelectTimer.Start();
                CswArbitrarySelect arbSel = _CAFResources.makeCswArbitrarySelect( "cafselect_materialsize", sql );
                DataTable tbl = arbSel.getTable();
                //SelectTimer.Stop();

                foreach( DataRow row in tbl.Rows )
                {
                    //MakeNodeTimer.Start();
                    CswNbtObjClassSize sizeNode = _NBTResources.Nodes.makeNodeFromNodeTypeId( sizeNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                    //MakeNodeTimer.Stop();
                    _addNodeTypeProps( sizeNode.Node, row );
                    sizeNode.Material.RelatedNodeId = ChemicalNode.NodeId;

                    if( false == _doesSizeExist( sizeNT, sizeNode ) )
                    {
                        sizeNode.IsTemp = false;
                        PostChangesTimer.Start();
                        sizeNode.postChanges( true );
                        PostChangesTimer.Stop();
                        _createdSizes.Add( sizeNode );
                    }
                    sizeIds.Add( row["packdetailid"].ToString() );
                }
            }
            return sizeIds;
        }

        private CswNbtObjClassUnitOfMeasure _getUnitOfMeasure( string unitOfMeasurementName )
        {
            CswNbtObjClassUnitOfMeasure unitOfMeasureNode = null;

            string nbtCafUnitName = _CafTranslator.Translate( CafTranslator.CafTranslationDictionaryNames.UnitName, unitOfMeasurementName );
            CswNbtMetaDataObjectClass unitOfMeasureOC = _NBTResources.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp nameOCP = unitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );

            CswNbtView unitsView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = unitsView.AddViewRelationship( unitOfMeasureOC, false );

            unitsView.AddViewPropertyAndFilter( parent,
                MetaDataProp: nameOCP,
                Value: nbtCafUnitName,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree tree = _NBTResources.Trees.getTreeFromView( unitsView, false, false, true );
            int count = tree.getChildNodeCount();
            for( int i = 0; i < count; i++ )
            {
                tree.goToNthChild( i );
                unitOfMeasureNode = tree.getNodeForCurrentPosition();
                tree.goToParentNode();
            }
            return unitOfMeasureNode;
        }

        private bool _doesSizeExist( CswNbtMetaDataNodeType SizeNT, CswNbtObjClassSize PotentialSizeNode )
        {
            CswNbtMetaDataNodeTypeProp catalogNTP = SizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );
            CswNbtMetaDataNodeTypeProp materialNTP = SizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp quantNTP = SizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );

            CswNbtView sizesView = new CswNbtView( _NBTResources );
            CswNbtViewRelationship parent = sizesView.AddViewRelationship( SizeNT, false );

            sizesView.AddViewPropertyAndFilter( parent, catalogNTP,
                Value: PotentialSizeNode.CatalogNo.Text,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            sizesView.AddViewPropertyAndFilter( parent, materialNTP,
                Value: PotentialSizeNode.NodeId.PrimaryKey.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            sizesView.AddViewPropertyAndFilter( parent, quantNTP,
                Value: PotentialSizeNode.InitialQuantity.CachedUnitName,
                SubFieldName: CswNbtSubField.SubFieldName.Name,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            sizesView.AddViewPropertyAndFilter( parent, quantNTP,
                Value: PotentialSizeNode.InitialQuantity.Quantity.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.Value,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree sizesTree = _NBTResources.Trees.getTreeFromView( sizesView, false, false, true );
            bool existingSizeFound = sizesTree.getChildNodeCount() > 0;

            bool createdSizeFound = false;
            foreach( CswNbtObjClassSize sizeNode in _createdSizes )
            {
                if( sizeNode.InitialQuantity.Quantity == PotentialSizeNode.InitialQuantity.Quantity && sizeNode.InitialQuantity.CachedUnitName.Equals( PotentialSizeNode.InitialQuantity.CachedUnitName )
                    && sizeNode.CatalogNo.Text.Equals( PotentialSizeNode.CatalogNo.Text ) && sizeNode.Material.RelatedNodeId.Equals( PotentialSizeNode.Material.RelatedNodeId ) )
                {
                    createdSizeFound = true;
                }
            }

            return ( existingSizeFound || createdSizeFound );
        }

        #endregion

        #endregion
    }

    public class CAFMapping
    {
        public string CAFTableName;
        public string CAFColName = "";
        public string CAFColName2 = "";
        public int NBTNodeTypeId;
        public int NBTNodeTypePropId;
        public string NBTSubFieldPropColName = "";
        public string NBTSubFieldPropColName2 = "";
        public string MappingDictionaryName = CafTranslator.CafTranslationDictionaryNames.NONE;

        public CswNbtMetaDataNodeType NBTNodeType( CswNbtResources NbtResources )
        {
            return NbtResources.MetaData.getNodeType( NBTNodeTypeId );
        }
        public CswNbtMetaDataNodeTypeProp NBTProp( CswNbtResources NbtResources )
        {
            return NbtResources.MetaData.getNodeTypeProp( NBTNodeTypePropId );
        }
        public CswNbtSubField NBTSubField( CswNbtResources NbtResources )
        {
            CswNbtMetaDataNodeTypeProp ntp = NBTProp( NbtResources );
            return ntp.getFieldTypeRule().SubFields[(CswNbtSubField.PropColumn) NBTSubFieldPropColName];
        }
    }
}

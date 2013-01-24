using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceC3Search
    {
        #region Ctor

        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceC3Search( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Ctor

        #region DataContracts

        [DataContract]
        public class CswNbtC3SearchReturn : CswWebSvcReturn
        {
            public CswNbtC3SearchReturn()
            {
                Data = new C3SearchResponse();
            }

            [DataMember]
            public C3SearchResponse Data;
        }

        [DataContract]
        public class C3SearchResponse
        {
            [DataMember]
            public Collection<DataSource> AvailableDataSources = new Collection<DataSource>();

            [DataMember]
            public List<string> SearchTypes = new List<string>();

            [DataMember]
            public string SearchResults = string.Empty;

            [DataMember]
            public CswC3Product ProductDetails = null;
        }

        [DataContract]
        public class DataSource
        {
            [DataMember]
            public string value = string.Empty;

            [DataMember]
            public string display = string.Empty;
        }

        [DataContract]
        public class CswNbtC3CreateMaterialReturn : CswWebSvcReturn
        {
            public CswNbtC3CreateMaterialReturn()
            {
                Data = new C3CreateMaterialResponse();
            }

            [DataMember]
            public C3CreateMaterialResponse Data;
        }

        [DataContract]
        public class C3CreateMaterialResponse
        {
            [DataMember]
            public string actionname = string.Empty;

            [DataMember]
            public State state = null;

            [DataMember]
            public Collection<Collection<SizeColumnValue>> rows;

            [DataMember]
            public bool success;

            [DataContract]
            public class SizeColumnValue
            {
                [DataMember]
                public string value { get; set; }
            }

            [DataContract]
            public class State
            {
                [DataMember]
                public string materialId = string.Empty;

                [DataMember]
                public string partNo = string.Empty;

                [DataMember]
                public string tradeName = string.Empty;

                [DataMember]
                public bool useExistingTempNode;

                [DataMember]
                public Supplier supplier = null;

                [DataMember]
                public MaterialType materialType = null;

                [DataContract]
                public class MaterialType
                {
                    [DataMember]
                    public string name = string.Empty;

                    [DataMember]
                    //public string val = "";
                    public Int32 val = Int32.MinValue;
                }

                [DataContract]
                public class Supplier
                {
                    [DataMember]
                    public string name = string.Empty;

                    [DataMember]
                    public string val = string.Empty;
                }

            }
        }

        #endregion

        public static void GetAvailableDataSources( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            _setConfigurationVariables( CswC3Params, _CswNbtResources );

            ChemCatCentral.SearchClient C3Search = new ChemCatCentral.SearchClient();
            CswRetObjSearchResults SourcesList = C3Search.getDataSources( CswC3Params );

            //todo: catch error when SourcesList returns null

            Collection<DataSource> AvailableDataSources = new Collection<DataSource>();

            //Create the "All Sources" option
            CswCommaDelimitedString AllSources = new CswCommaDelimitedString();
            AllSources.FromArray( SourcesList.AvailableDataSources );

            DataSource allSourcesDs = new DataSource();
            allSourcesDs.value = AllSources.ToString();
            allSourcesDs.display = "All Sources";
            AvailableDataSources.Add( allSourcesDs );

            //Add available data source options
            foreach( string DataSource in SourcesList.AvailableDataSources )
            {
                DataSource dS = new DataSource();
                dS.value = DataSource;
                dS.display = DataSource;
                AvailableDataSources.Add( dS );
            }

            Return.Data.AvailableDataSources = AvailableDataSources;

        }

        public static void GetSearchTypes( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            _setConfigurationVariables( CswC3Params, _CswNbtResources );

            List<string> newlist = new List<string>();

            foreach( CswC3SearchParams.SearchFieldType SearchType in Enum.GetValues( typeof( CswC3SearchParams.SearchFieldType ) ) )
            {
                newlist.Add( SearchType.ToString() );
            }

            Return.Data.SearchTypes = newlist;

        }

        public static void GetC3ProductDetails( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            _setConfigurationVariables( CswC3SearchParams, _CswNbtResources );

            ChemCatCentral.SearchClient C3Search = new ChemCatCentral.SearchClient();
            CswRetObjSearchResults SearchResults = C3Search.getProductDetails( CswC3SearchParams );
            if( SearchResults.CswC3SearchResults.Length > 0 )
            {
                ChemCatCentral.CswC3Product C3ProductDetails = SearchResults.CswC3SearchResults[0];
                Return.Data.ProductDetails = C3ProductDetails;
            }
        }

        public static void RunChemCatCentralSearch( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            _setConfigurationVariables( CswC3SearchParams, _CswNbtResources );

            JObject ret = new JObject();

            ChemCatCentral.SearchClient C3Search = new ChemCatCentral.SearchClient();
            CswRetObjSearchResults SearchResults = C3Search.search( CswC3SearchParams );

            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, null, Int32.MinValue );
            ret["table"] = wsTable.getTable( SearchResults );
            ret["filters"] = "";
            ret["searchterm"] = CswC3SearchParams.Query;
            ret["filtersapplied"] = "";
            //Search.SaveToCache( true );
            ret["sessiondataid"] = "";
            ret["searchtype"] = "chemcatcentral";

            Return.Data.SearchResults = ret.ToString();

        }

        public static void importC3Product( ICswResources CswResources, CswNbtC3CreateMaterialReturn Return, Int32 ProductId )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            ChemCatCentral.CswC3Product C3ProductDetails = new CswC3Product();

            CswC3SearchParams.Field = "ProductId";
            CswC3SearchParams.Query = CswConvert.ToString( ProductId );

            _setConfigurationVariables( CswC3SearchParams, _CswNbtResources );

            // Perform C3 search to get the product details
            ChemCatCentral.SearchClient C3Search = new ChemCatCentral.SearchClient();
            CswRetObjSearchResults SearchResults = C3Search.getProductDetails( CswC3SearchParams );
            if( SearchResults.CswC3SearchResults.Length > 0 )
            {
                C3ProductDetails = SearchResults.CswC3SearchResults[0];
            }

            // When a product is imported, the nodetype will default to 'Chemical'
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                // Instance the ImportManger
                ImportManager C3Import = new ImportManager( _CswNbtResources, C3ProductDetails );

                // Create the temporary material node
                CswNbtObjClassMaterial C3ProductTempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                // Add props to the tempnode
                C3Import.addNodeTypeProps( C3ProductTempNode.Node );
                C3ProductTempNode.postChanges( false );

                // Get or create a vendor node
                CswNbtObjClassVendor VendorNode = C3Import.createVendorNode( C3ProductDetails.SupplierName );

                // Create size node(s)
                Collection<Collection<C3CreateMaterialResponse.SizeColumnValue>> ProductSizes = C3Import.createSizeNodes( C3ProductTempNode );

                #region Return Object

                Return.Data.success = true;
                Return.Data.actionname = "create material";
                Return.Data.rows = ProductSizes;
                
                C3CreateMaterialResponse.State.Supplier supplier = new C3CreateMaterialResponse.State.Supplier();
                supplier.name = VendorNode.NodeName;
                supplier.val = VendorNode.NodeId.ToString();

                C3CreateMaterialResponse.State.MaterialType materialType = new C3CreateMaterialResponse.State.MaterialType();
                materialType.name = ChemicalNT.NodeTypeName;
                materialType.val = ChemicalNT.NodeTypeId;

                C3CreateMaterialResponse.State state = new C3CreateMaterialResponse.State();
                state.materialId = C3ProductTempNode.NodeId.ToString();
                state.tradeName = C3ProductTempNode.TradeName.Text;
                state.partNo = C3ProductTempNode.PartNumber.Text;
                state.useExistingTempNode = true;
                state.supplier = supplier;
                state.materialType = materialType;

                Return.Data.state = state;

                #endregion Return Object
            }

        }

        #region Import Mananger

        private class ImportManager
        {
            private CswNbtResources _CswNbtResources;
            private Dictionary<string, C3Mapping> _Mappings;
            private CswC3Product _ProductToImport;

            public ImportManager( CswNbtResources CswNbtResources, CswC3Product ProductToImport )
            {
                _CswNbtResources = CswNbtResources;
                _ProductToImport = ProductToImport;
                _initMappings();
            }

            #region Private helper methods

            private void _initMappings()
            {
                _Mappings = new Dictionary<string, C3Mapping>();

                #region Chemical

                CswNbtMetaDataNodeType chemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );

                _Mappings.Add( "Tradename", new C3Mapping
                {
                    NBTNodeTypeId = chemicalNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.TradeName,
                    NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Tradename" ).PropId,
                    NBTSubFieldPropColName = "field1"
                } );

                _Mappings.Add( "CAS No", new C3Mapping
                {
                    NBTNodeTypeId = chemicalNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.CasNo,
                    NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "CAS No" ).PropId,
                    NBTSubFieldPropColName = "field1"
                } );

                _Mappings.Add( "Formula", new C3Mapping
                {
                    NBTNodeTypeId = chemicalNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.Formula,
                    NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Formula" ).PropId,
                    NBTSubFieldPropColName = "field1"
                } );

                _Mappings.Add( "Structure", new C3Mapping
                {
                    NBTNodeTypeId = chemicalNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.MolData,
                    NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Structure" ).PropId,
                    NBTSubFieldPropColName = "ClobData"
                } );

                _Mappings.Add( "Part Number", new C3Mapping
                {
                    NBTNodeTypeId = chemicalNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.PartNo,
                    NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Part Number" ).PropId,
                    NBTSubFieldPropColName = "field1"
                } );

                _Mappings.Add( "Physical Description", new C3Mapping
                {
                    NBTNodeTypeId = chemicalNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.Description,
                    NBTNodeTypePropId = chemicalNT.getNodeTypeProp( "Physical Description" ).PropId,
                    NBTSubFieldPropColName = "gestalt"
                } );

                //todo: Add MSDS, ProductURL, Extension data additional properties

                #endregion

                #region Vendor

                CswNbtMetaDataNodeType vendorNT = _CswNbtResources.MetaData.getNodeType( "Vendor" );
                _Mappings.Add( "Vendor Name", new C3Mapping
                {
                    NBTNodeTypeId = vendorNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.SupplierName,
                    NBTNodeTypePropId = vendorNT.getNodeTypeProp( "Vendor Name" ).PropId,
                    NBTSubFieldPropColName = "field1"
                } );

                #endregion

                #region Sizes

                CswNbtMetaDataNodeType sizeNT = _CswNbtResources.MetaData.getNodeType( "Size" );

                _Mappings.Add( "Initial Quantity", new C3Mapping
                {
                    NBTNodeTypeId = sizeNT.NodeTypeId,
                    NBTNodeTypePropId = sizeNT.getNodeTypeProp( "Initial Quantity" ).PropId,
                    NBTSubFieldPropColName = "field1_numeric",
                    NBTSubFieldPropColName2 = "field1"
                } );

                _Mappings.Add( "Catalog No", new C3Mapping
                {
                    NBTNodeTypeId = sizeNT.NodeTypeId,
                    C3ProductPropertyValue = _ProductToImport.CatalogNo,
                    NBTNodeTypePropId = sizeNT.getNodeTypeProp( "Catalog No" ).PropId,
                    NBTSubFieldPropColName = "field1"
                } );

                #endregion
            }//_initMappings()

            private CswNbtObjClassUnitOfMeasure _getUnitOfMeasure( string unitOfMeasurementName )
            {
                CswNbtObjClassUnitOfMeasure unitOfMeasureNode = null;

                CswNbtMetaDataObjectClass unitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
                CswNbtMetaDataObjectClassProp nameOCP = unitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );

                CswNbtView unitsView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship parent = unitsView.AddViewRelationship( unitOfMeasureOC, false );

                unitsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: nameOCP,
                    Value: unitOfMeasurementName,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( unitsView, false, false, true );
                int count = tree.getChildNodeCount();
                for( int i = 0; i < count; i++ )
                {
                    tree.goToNthChild( i );
                    unitOfMeasureNode = tree.getNodeForCurrentPosition();
                    tree.goToParentNode();
                }

                return unitOfMeasureNode;
            }//_getUnitOfMeasure

            /// <summary>
            /// Class that contains the variables necessary for the mapping.
            /// </summary>
            private class C3Mapping
            {
                public Int32 NBTNodeTypeId = Int32.MinValue;
                public Int32 NBTNodeTypePropId = Int32.MinValue;
                public string C3ProductPropertyValue = string.Empty;
                public string C3ProductPropertyName2 = string.Empty;
                public string NBTSubFieldPropColName = string.Empty;
                public string NBTSubFieldPropColName2 = string.Empty;
            }

            #endregion Private helper methods

            /// <summary>
            /// Creates a new Vendor node if the Vendor does not already exist, otherwise gets the existing Vendor
            /// </summary>
            /// <returns></returns>
            public CswNbtObjClassVendor createVendorNode( string VendorName )
            {
                CswNbtObjClassVendor VendorNode = null;

                CswNbtMetaDataNodeType VendorNT = _CswNbtResources.MetaData.getNodeType( "Vendor" );
                if( null != VendorNT )
                {
                    foreach( CswNbtObjClassVendor CurrentVendorNode in VendorNT.getNodes( false, true ) )
                    {
                        if( CurrentVendorNode.VendorName.Text.Equals( VendorName ) )
                        {
                            VendorNode = CurrentVendorNode;
                        }
                    }
                    if( null == VendorNode )
                    {
                        VendorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( VendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                        addNodeTypeProps( VendorNode.Node );
                        VendorNode.IsTemp = false;
                        VendorNode.postChanges( true );
                    }
                }

                return VendorNode;
            }//createVendorNode()

            public Collection<Collection<C3CreateMaterialResponse.SizeColumnValue>> createSizeNodes( CswNbtObjClassMaterial ChemicalNode )
            {
                // Return object
                Collection<Collection<C3CreateMaterialResponse.SizeColumnValue>> ProductSizes = new Collection<Collection<C3CreateMaterialResponse.SizeColumnValue>>();

                CswNbtMetaDataNodeType SizeNT = _CswNbtResources.MetaData.getNodeType( "Size" );
                if( null != SizeNT )
                {
                    for( int index = 0; index < _ProductToImport.ProductSize.Length; index++ )
                    {
                        CswC3Product.Size CurrentSize = _ProductToImport.ProductSize[index];
                        CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                        addNodeTypeProps( sizeNode.Node, index );
                        sizeNode.Material.RelatedNodeId = ChemicalNode.NodeId;

                        bool duplicateFound = false;
                        foreach( CswNbtObjClassSize existingSizeNode in SizeNT.getNodes( false, false, false, true ) )
                        {
                            if( existingSizeNode.Material.RelatedNodeId == sizeNode.Material.RelatedNodeId &&
                                existingSizeNode.CatalogNo.Text.Equals( sizeNode.CatalogNo.Text ) &&
                                existingSizeNode.InitialQuantity.Quantity.Equals( sizeNode.InitialQuantity.Quantity ) &&
                                existingSizeNode.InitialQuantity.CachedUnitName.Equals( sizeNode.InitialQuantity.CachedUnitName ) )
                            {
                                duplicateFound = true;
                            }
                        }
                        if( false == duplicateFound )
                        {
                            sizeNode.IsTemp = false;
                            sizeNode.postChanges( true );

                            //Set the return object
                            Collection<C3CreateMaterialResponse.SizeColumnValue> Size = new Collection<C3CreateMaterialResponse.SizeColumnValue>();

                            C3CreateMaterialResponse.SizeColumnValue UnitCount = new C3CreateMaterialResponse.SizeColumnValue();
                            UnitCount.value = CswConvert.ToString( sizeNode.UnitCount.Value );
                            Size.Add( UnitCount );

                            C3CreateMaterialResponse.SizeColumnValue InitialQuantity = new C3CreateMaterialResponse.SizeColumnValue();
                            InitialQuantity.value = sizeNode.InitialQuantity.Gestalt;
                            Size.Add( InitialQuantity );

                            C3CreateMaterialResponse.SizeColumnValue CatalogNo = new C3CreateMaterialResponse.SizeColumnValue();
                            CatalogNo.value = sizeNode.CatalogNo.Text;
                            Size.Add( CatalogNo );

                            ProductSizes.Add( Size );
                        }
                    }
                } // if( null != SizeNT )

                return ProductSizes;
            }//createSizeNodes()

            /// <summary>
            /// TODO: explain what currentindex is for
            /// </summary>
            /// <param name="Node"></param>
            /// <param name="CurrentIndex"></param>
            public void addNodeTypeProps( CswNbtNode Node, int CurrentIndex = 0 )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Node.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NTP in NodeType.getNodeTypeProps() )
                {
                    if( null != Node.Properties[NTP] && _Mappings.ContainsKey( NTP.PropName ) )
                    {
                        C3Mapping C3Mapping = _Mappings[NTP.PropName];
                        switch( Node.Properties[NTP].getFieldType().FieldType )
                        {
                            case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                                CswNbtObjClassUnitOfMeasure unitOfMeasure = _getUnitOfMeasure( _ProductToImport.ProductSize[CurrentIndex].pkg_qty_uom );
                                if( null != unitOfMeasure )
                                {
                                    Node.Properties[NTP].SetPropRowValue( (CswNbtSubField.PropColumn) C3Mapping.NBTSubFieldPropColName, _ProductToImport.ProductSize[CurrentIndex].pkg_qty );
                                    Node.Properties[NTP].SetPropRowValue( (CswNbtSubField.PropColumn) C3Mapping.NBTSubFieldPropColName2, unitOfMeasure.BaseUnit.Text );
                                    Node.Properties[NTP].SetPropRowValue( CswNbtSubField.PropColumn.Field1_FK, unitOfMeasure.NodeId.PrimaryKey );
                                    string sizeGestalt = _ProductToImport.ProductSize[CurrentIndex].pkg_qty + " " + unitOfMeasure.BaseUnit.Text;
                                    Node.Properties[NTP].SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, sizeGestalt );
                                }
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                                if( false == string.IsNullOrEmpty( C3Mapping.C3ProductPropertyValue ) )
                                {
                                    CswNbtSdTabsAndProps TabsPropsSd = new CswNbtSdTabsAndProps( _CswNbtResources );
                                    string propAttr = new CswPropIdAttr( Node, NTP ).ToString();
                                    string molData = C3Mapping.C3ProductPropertyValue;
                                    TabsPropsSd.saveMolProp( molData, propAttr );
                                }
                                break;
                            default:
                                Node.Properties[NTP].SetPropRowValue( (CswNbtSubField.PropColumn) C3Mapping.NBTSubFieldPropColName, C3Mapping.C3ProductPropertyValue );
                                Node.Properties[NTP].SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, C3Mapping.C3ProductPropertyValue );
                                break;
                        }
                    }//if( null != Node.Properties[NTP] && _Mappings.ContainsKey( NTP.PropName ) )

                }//foreach( CswNbtMetaDataNodeTypeProp NTP in NodeType.getNodeTypeProps() )

            }//addNodeTypeProps()

        }//class ImportManager

        #endregion Import Mananger

        #region Private helper methods

        /// <summary>
        /// Set the c3 parameter object's CustomerLoginName, LoginPassword, and AccessId
        /// parameters using the values from the configuration_variables table in the db.
        /// </summary>
        /// <param name="CswC3Params"></param>
        private static void _setConfigurationVariables( CswC3Params CswC3Params, CswNbtResources _CswNbtResources )
        {

            CswC3Params.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_Username );
            CswC3Params.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_Password );
            CswC3Params.AccessId = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_AccessId );

        }

        /// <summary>
        /// Set the c3 search parameter object's CustomerLoginName, LoginPassword, and AccessId
        /// parameters using the values from the configuration_variables table in the db.
        /// </summary>
        /// <param name="CswC3SearchParams"></param>
        private static void _setConfigurationVariables( CswC3SearchParams CswC3SearchParams, CswNbtResources _CswNbtResources )
        {
            CswC3SearchParams.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_Username );
            CswC3SearchParams.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_Password );
            CswC3SearchParams.AccessId = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_AccessId );
            CswC3SearchParams.MaxRows = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "treeview_resultlimit" ) );
        }

        #endregion
    }

}
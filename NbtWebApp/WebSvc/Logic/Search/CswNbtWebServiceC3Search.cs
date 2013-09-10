using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.StructureSearch;
using NbtWebApp.WebSvc.Returns;
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
        public class CswNbtC3Import
        {
            [DataContract]
            public class Request
            {
                [DataMember]
                public string C3ProductId = string.Empty;

                [DataMember]
                public string NodeTypeName = string.Empty;

                [DataMember]
                public string NodeTypeId = string.Empty;

                [DataMember]
                public string ObjectClass = string.Empty;
            }
        }

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

            [DataMember]
            public Collection<NodeType> ImportableNodeTypes = new Collection<NodeType>();
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
        public class NodeType
        {
            [DataMember]
            public string nodetypename = string.Empty;

            [DataMember]
            public string nodetypeid = string.Empty;

            [DataMember]
            public string iconfilename = string.Empty;

            [DataMember]
            public string objclass = string.Empty;
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
            public bool success;

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
                public Collection<SizeRecord> sizes;

                [DataMember]
                public Supplier supplier = null;

                [DataMember]
                public bool addNewC3Supplier = false;

                [DataMember]
                public bool showOriginalUoM = true;

                [DataMember]
                public MaterialType materialType = null;

                [DataMember]
                public string sdsDocId = string.Empty;

                [DataContract]
                public class MaterialType
                {
                    [DataMember]
                    public string name = string.Empty;

                    [DataMember]
                    public Int32 val = Int32.MinValue;

                    [DataMember]
                    public bool readOnly = true;
                }

                [DataContract]
                public class Supplier
                {
                    [DataMember]
                    public string name = string.Empty;

                    [DataMember]
                    public string val = string.Empty;
                }

                [DataContract]
                public class SizeRecord
                {
                    [DataMember]
                    public SizeData nodeId = null;

                    [DataMember]
                    public SizeData unitCount = null;

                    [DataMember]
                    public SizeData quantity = null;

                    [DataMember]
                    public SizeData uom = null;

                    [DataMember]
                    public SizeData origUom = null;

                    [DataMember]
                    public SizeData catalogNo = null;

                    [DataMember]
                    public SizeData quantityEditable = null;

                    [DataMember]
                    public SizeData dispensible = null;

                    [DataContract]
                    public class SizeData
                    {
                        [DataMember]
                        public string value = string.Empty;

                        [DataMember]
                        public bool readOnly = false;

                        [DataMember]
                        public bool hidden = false;

                    }
                }

            }
        }

        #endregion

        public static JObject getImportBtnItems( CswNbtResources CswNbtResources )
        {
            JObject ImportableNodeTypes = new JObject();

            Collection<CswEnumNbtObjectClass> MaterialPropSetMembers = CswNbtPropertySetMaterial.Members();
            foreach( CswEnumNbtObjectClass ObjectClassName in MaterialPropSetMembers )
            {
                CswNbtMetaDataObjectClass ObjectClass = CswNbtResources.MetaData.getObjectClass( ObjectClassName );
                foreach( CswNbtMetaDataNodeType CurrentNT in ObjectClass.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp IsConstituentNTP = CurrentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.IsConstituent );
                    if( CswEnumTristate.False == IsConstituentNTP.DefaultValue.AsLogical.Checked )
                    {
                        JObject NodeType = new JObject();
                        ImportableNodeTypes[CurrentNT.NodeTypeName] = NodeType;
                        NodeType["nodetypename"] = CurrentNT.NodeTypeName;
                        NodeType["nodetypeid"] = CurrentNT.NodeTypeId.ToString();
                        NodeType["iconfilename"] = CswNbtMetaDataObjectClass.IconPrefix16 + CurrentNT.IconFileName;
                        NodeType["objclass"] = ObjectClassName.ToString();
                    }
                }
            }

            return ImportableNodeTypes;
        }

        public static void GetAvailableDataSources( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            CswRetObjSearchResults SourcesList = C3SearchClient.getDataSources( CswC3Params );

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

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

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
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            CswRetObjSearchResults SearchResults = C3SearchClient.getProductDetails( CswC3SearchParams );
            if( SearchResults.CswC3SearchResults.Length > 0 )
            {
                ChemCatCentral.CswC3Product C3ProductDetails = SearchResults.CswC3SearchResults[0];
                Return.Data.ProductDetails = C3ProductDetails;
            }
        }

        public static void RunChemCatCentralSearch( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            JObject ret = new JObject();

            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            CswRetObjSearchResults SearchResults = null;

            try
            {
                SearchResults = C3SearchClient.search( CswC3SearchParams );
            }
            catch( TimeoutException TimeoutException )
            {
                const string WarningMessage = "The search has timed out. Please use more specific search terms.";
                throw ( new CswDniException( CswEnumErrorType.Warning, WarningMessage, WarningMessage, TimeoutException ) );
            }

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

        public static void importC3Product( ICswResources CswResources, CswNbtC3CreateMaterialReturn Return, CswNbtC3Import.Request Request )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            ChemCatCentral.CswC3Product C3ProductDetails = new CswC3Product();

            CswC3SearchParams.Field = "ProductId";
            CswC3SearchParams.Query = CswConvert.ToString( Request.C3ProductId );

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            // Perform C3 search to get the product details
            CswRetObjSearchResults SearchResults = C3SearchClient.getProductDetails( CswC3SearchParams );
            if( SearchResults.CswC3SearchResults.Length > 0 )
            {
                C3ProductDetails = SearchResults.CswC3SearchResults[0];
            }

            string NodeTypeName = Request.NodeTypeName;
            if( false == string.IsNullOrEmpty( NodeTypeName ) )
            {
                CswNbtMetaDataNodeType NodeTypeToBeImported = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
                if( null != NodeTypeToBeImported )
                {
                    // Instance the ImportManger
                    ImportManager C3Import = new ImportManager( _CswNbtResources, C3ProductDetails );

                    // Create the temporary material node
                    CswNbtPropertySetMaterial C3ProductTempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeToBeImported.NodeTypeId, IsTemp: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                        {
                            //Set the c3productid property
                            ( (CswNbtPropertySetMaterial) NewNode ).C3ProductId.Text = C3ProductDetails.ProductId.ToString();
                            // Add props to the tempnode
                            C3Import.addNodeTypeProps( NewNode );

                            // Sync Hazard Classes and PCID data if C3ProductTempNode is of type Chemical
                            if( NewNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                            {
                                CswNbtObjClassChemical ChemicalNode = NewNode;
                                ChemicalNode.syncFireDbData();
                                ChemicalNode.syncPCIDData();
                            }
                            //C3ProductTempNode.postChanges( false );
                        } );

                    // Get or create a vendor node
                    C3CreateMaterialResponse.State.Supplier Supplier = C3Import.createVendorNode( C3ProductDetails.SupplierName );

                    // Create size node(s)
                    Collection<C3CreateMaterialResponse.State.SizeRecord> ProductSizes = C3Import.createSizeNodes( C3ProductTempNode );

                    // Create synonyms node(s)
                    C3Import.createMaterialSynonyms( C3ProductTempNode );

                    // Create a document node if C3ProductTempNode is of type Chemical
                    CswPrimaryKey SDSDocumentNodeId = new CswPrimaryKey();
                    if( C3ProductTempNode.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                    {
                        SDSDocumentNodeId = C3Import.createMaterialDocument( C3ProductTempNode );
                    }

                    #region Return Object

                    Return.Data.success = true;
                    Return.Data.actionname = "create material";

                    C3CreateMaterialResponse.State.MaterialType MaterialType = new C3CreateMaterialResponse.State.MaterialType();
                    MaterialType.name = Request.NodeTypeName;
                    MaterialType.val = CswConvert.ToInt32( Request.NodeTypeId );

                    C3CreateMaterialResponse.State State = new C3CreateMaterialResponse.State();
                    State.materialId = C3ProductTempNode.NodeId.ToString();
                    State.tradeName = C3ProductTempNode.TradeName.Text;
                    State.partNo = C3ProductTempNode.PartNumber.Text;
                    //State.useExistingTempNode = true;
                    State.supplier = Supplier;
                    if( string.IsNullOrEmpty( State.supplier.val ) )
                    {
                        State.addNewC3Supplier = true;
                    }
                    State.materialType = MaterialType;
                    State.sizes = ProductSizes;
                    if( null != SDSDocumentNodeId )
                    {
                        State.sdsDocId = SDSDocumentNodeId.ToString();
                    }

                    Return.Data.state = State;

                    #endregion Return Object
                }
            }
        }

        #region Import Mananger

        private class ImportManager
        {
            private CswNbtResources _CswNbtResources;
            private Dictionary<string, C3Mapping> _Mappings;
            private CswC3Product _ProductToImport;
            private List<CswC3Product.Size> _SizesToImport;

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

                #region Material

                const string Tradename = CswNbtPropertySetMaterial.PropertyName.TradeName;
                _Mappings.Add( Tradename, new C3Mapping
                {
                    NBTNodeTypeId = Int32.MinValue,
                    C3ProductPropertyValue = _ProductToImport.TradeName,
                    NBTNodeTypePropId = Int32.MinValue,
                    NBTSubFieldPropColName = "field1"
                } );

                const string PartNumber = CswNbtPropertySetMaterial.PropertyName.PartNumber;
                _Mappings.Add( PartNumber, new C3Mapping
                {
                    NBTNodeTypeId = Int32.MinValue,
                    C3ProductPropertyValue = _ProductToImport.PartNo,
                    NBTNodeTypePropId = Int32.MinValue,
                    NBTSubFieldPropColName = "field1"
                } );

                // Add any additional properties
                foreach( CswC3Product.TemplateSlctdExtData NameValuePair in _ProductToImport.TemplateSelectedExtensionData )
                {
                    string PropertyName = NameValuePair.attribute;
                    _Mappings.Add( PropertyName, new C3Mapping
                    {
                        //NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = NameValuePair.value,
                        //NBTNodeTypePropId = ChemicalNTP.PropId,
                        NBTSubFieldPropColName = "field1"
                    } );
                }

                #endregion

                #region Biological

                CswNbtMetaDataNodeType BiologicalNT = _CswNbtResources.MetaData.getNodeType( "Biological" );
                if( null != BiologicalNT )
                {
                    //todo: Add Type, Species, Reference #, Picture, and Reference Type Properties

                    CswNbtMetaDataNodeTypeProp BiologicalName = BiologicalNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.TradeName );
                    if( null != BiologicalName )
                    {
                        _Mappings.Add( BiologicalName.PropName, new C3Mapping
                        {
                            NBTNodeTypeId = BiologicalNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.TradeName,
                            NBTNodeTypePropId = BiologicalName.PropId,
                            NBTSubFieldPropColName = "field1"
                        } );
                    }

                    //CswNbtMetaDataNodeTypeProp Picture = BiologicalNT.getNodeTypeProp( "Picture" );
                    //if( null != Picture )
                    //{
                    //    _Mappings.Add( "Picture", new C3Mapping
                    //    {
                    //        NBTNodeTypeId = BiologicalNT.NodeTypeId,
                    //        //C3ProductPropertyValue = _ProductToImport.Picture,
                    //        NBTNodeTypePropId = Picture.PropId,
                    //        NBTSubFieldPropColName = "field1"
                    //    } );
                    //}
                }

                #endregion

                #region Supply

                CswNbtMetaDataNodeType SupplyNT = _CswNbtResources.MetaData.getNodeType( "Supply" );
                if( null != SupplyNT )
                {
                    //todo: Finish Picture Property
                    CswNbtMetaDataNodeTypeProp Description = SupplyNT.getNodeTypeProp( "Description" );
                    if( null != Description )
                    {
                        _Mappings.Add( "Description", new C3Mapping
                        {
                            NBTNodeTypeId = SupplyNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.Description,
                            NBTNodeTypePropId = Description.PropId,
                            NBTSubFieldPropColName = "field1"
                        } );
                    }

                    //CswNbtMetaDataNodeTypeProp Picture = SupplyNT.getNodeTypeProp( "Picture" );
                    //if( null != Picture )
                    //{
                    //    _Mappings.Add( "Picture", new C3Mapping
                    //    {
                    //        NBTNodeTypeId = SupplyNT.NodeTypeId,
                    //        //C3ProductPropertyValue = _ProductToImport.Picture,
                    //        NBTNodeTypePropId = Picture.PropId,
                    //        NBTSubFieldPropColName = "field1"
                    //    } );
                    //}
                }

                #endregion

                #region Chemical

                CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );
                if( null != ChemicalNT )
                {
                    const string CasNo = CswNbtObjClassChemical.PropertyName.CasNo;
                    _Mappings.Add( CasNo, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.CasNo,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( CasNo ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    // Defaults to liquid
                    const string PhysicalState = CswNbtObjClassChemical.PropertyName.PhysicalState;
                    _Mappings.Add( PhysicalState, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( PhysicalState ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    CswNbtMetaDataNodeTypeProp Formula = ChemicalNT.getNodeTypeProp( "Formula" );
                    if( null != Formula )
                    {
                        _Mappings.Add( "Formula", new C3Mapping
                        {
                            NBTNodeTypeId = ChemicalNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.Formula,
                            NBTNodeTypePropId = Formula.PropId,
                            NBTSubFieldPropColName = "field1"
                        } );
                    }

                    CswNbtMetaDataNodeTypeProp Structure = ChemicalNT.getNodeTypeProp( "Structure" );
                    if( null != Structure )
                    {
                        _Mappings.Add( "Structure", new C3Mapping
                        {
                            NBTNodeTypeId = ChemicalNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.MolData,
                            NBTNodeTypePropId = Structure.PropId,
                            NBTSubFieldPropColName = "ClobData"
                        } );
                    }


                    CswNbtMetaDataNodeTypeProp PhysicalDescription = ChemicalNT.getNodeTypeProp( "Physical Description" );
                    if( null != PhysicalDescription )
                    {
                        _Mappings.Add( "Physical Description", new C3Mapping
                        {
                            NBTNodeTypeId = ChemicalNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.Description,
                            NBTNodeTypePropId = PhysicalDescription.PropId,
                            NBTSubFieldPropColName = "gestalt"
                        } );
                    }
                }

                #endregion

                #region Vendor

                CswNbtMetaDataNodeType VendorNT = _CswNbtResources.MetaData.getNodeType( "Vendor" );
                if( null != VendorNT )
                {
                    const string VendorName = CswNbtObjClassVendor.PropertyName.VendorName;
                    _Mappings.Add( VendorName, new C3Mapping
                    {
                        NBTNodeTypeId = VendorNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.SupplierName,
                        NBTNodeTypePropId = VendorNT.getNodeTypePropIdByObjectClassProp( VendorName ),
                        NBTSubFieldPropColName = "field1"
                    } );
                }

                #endregion

                #region Sizes

                CswNbtMetaDataNodeType SizeNT = _CswNbtResources.MetaData.getNodeType( "Size" );
                if( null != SizeNT )
                {
                    const string InitialQuantity = CswNbtObjClassSize.PropertyName.InitialQuantity;
                    _Mappings.Add( InitialQuantity, new C3Mapping
                    {
                        NBTNodeTypeId = SizeNT.NodeTypeId,
                        NBTNodeTypePropId = SizeNT.getNodeTypePropIdByObjectClassProp( InitialQuantity ),
                        NBTSubFieldPropColName = "field1_numeric",
                        NBTSubFieldPropColName2 = "field1"
                    } );
                }

                #endregion

            }//_initMappings()

            /// <summary>
            /// Class that contains the variables necessary for the mapping.
            /// </summary>
            private class C3Mapping
            {
                public Int32 NBTNodeTypeId = Int32.MinValue;
                public Int32 NBTNodeTypePropId = Int32.MinValue;
                public string C3ProductPropertyValue = string.Empty;
                public string NBTSubFieldPropColName = string.Empty;
                public string NBTSubFieldPropColName2 = string.Empty;
            }

            private Tuple<int, string> _getUnitOfMeasure( string mappedUnitOfMeasure, string origUnitOfMeasure )
            {
                Tuple<int, string> UnitOfMeasureNodeInfo = null;

                //if( false == string.IsNullOrEmpty( mappedUnitOfMeasure ) )
                //{

                // Create the view
                CswNbtMetaDataObjectClass UnitsOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
                CswNbtView MatchingUOMsView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship ParentRelationship = MatchingUOMsView.AddViewRelationship( UnitsOfMeasureOC, false );

                CswNbtMetaDataObjectClassProp NameOCP = UnitsOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
                CswNbtMetaDataObjectClassProp AliasesOCP = UnitsOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Aliases );

                CswNbtViewPropertyFilter ViewPropertyFilter_Name = MatchingUOMsView.AddViewPropertyAndFilter( ParentRelationship,
                                                               MetaDataProp: NameOCP,
                                                           Value: mappedUnitOfMeasure,
                                                               SubFieldName: CswEnumNbtSubFieldName.Text,
                                                               FilterMode: CswEnumNbtFilterMode.Equals );


                CswNbtViewPropertyFilter ViewPropertyFilter_Aliases = MatchingUOMsView.AddViewPropertyAndFilter( ParentRelationship,
                                                               MetaDataProp: AliasesOCP,
                                                           Value: mappedUnitOfMeasure,
                                                               SubFieldName: CswEnumNbtSubFieldName.Text,
                                                               FilterMode: CswEnumNbtFilterMode.Contains,
                                                               Conjunction: CswEnumNbtFilterConjunction.Or );

                // Get and iterate the Tree
                ICswNbtTree MatchingUOMsTree = _CswNbtResources.Trees.getTreeFromView( MatchingUOMsView, false, false, true );
                //int Count = MatchingUOMsTree.getChildNodeCount();
                if( MatchingUOMsTree.getChildNodeCount() == 0 )
                {
                    // Run view again using the origUnitOfMeasure
                    ViewPropertyFilter_Name.Value = origUnitOfMeasure;
                    ViewPropertyFilter_Aliases.Value = origUnitOfMeasure;

                    MatchingUOMsTree = _CswNbtResources.Trees.getTreeFromView( MatchingUOMsView, false, false, true );
                    if( MatchingUOMsTree.getChildNodeCount() > 0 )
                    {
                        for( int i = 0; i < MatchingUOMsTree.getChildNodeCount(); i++ )
                        {
                            MatchingUOMsTree.goToNthChild( i );
                            UnitOfMeasureNodeInfo =
                                new Tuple<int, string>( MatchingUOMsTree.getNodeIdForCurrentPosition().PrimaryKey,
                                                       MatchingUOMsTree.getNodeNameForCurrentPosition() );
                            MatchingUOMsTree.goToParentNode();
                        }
                    }
                }
                else
                {
                    for( int i = 0; i < MatchingUOMsTree.getChildNodeCount(); i++ )
                    {
                        MatchingUOMsTree.goToNthChild( i );
                        UnitOfMeasureNodeInfo = new Tuple<int, string>( MatchingUOMsTree.getNodeIdForCurrentPosition().PrimaryKey,
                                                   MatchingUOMsTree.getNodeNameForCurrentPosition() );
                        MatchingUOMsTree.goToParentNode();
                    }
                }


                //}//if( false == string.IsNullOrEmpty( unitOfMeasurementName ) )

                return UnitOfMeasureNodeInfo;

            }//_getUnitOfMeasure()

            private void _removeDuplicateSizes()
            {
                // Then loop through both and check for duplicates
                for( int i = 0; i < _ProductToImport.ProductSize.Length; i++ )
                {
                    CswC3Product.Size CurrentSize = _ProductToImport.ProductSize[i];
                    for( int j = ( _SizesToImport.Count - 1 ); j >= 0; j-- )
                    {
                        if( j != i )
                        {
                            CswC3Product.Size SizeToCompare = _SizesToImport[j];
                            if( SizeToCompare.pkg_qty == CurrentSize.pkg_qty
                                && SizeToCompare.catalog_no == CurrentSize.catalog_no )
                            {
                                _SizesToImport.RemoveAt( j );
                            }
                        }
                    }
                }
            }//_removeDuplicateSizes()

            #endregion Private helper methods

            public CswPrimaryKey createMaterialDocument( CswNbtPropertySetMaterial MaterialNode )
            {
                CswPrimaryKey NewSDSDocumentNodeId = null;

                string MsdsUrl = _ProductToImport.MsdsUrl;
                if( false == string.IsNullOrEmpty( MsdsUrl ) && _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )
                {
                    CswNbtMetaDataObjectClass SDSDocClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
                    CswNbtMetaDataNodeType SDSDocumentNT = SDSDocClass.FirstNodeType;
                    if( null != SDSDocumentNT )
                    {
                        CswNbtObjClassSDSDocument NewDoc = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( SDSDocumentNT.NodeTypeId, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                            {
                                // This needs to be CswNbtObjClassSDSDocument NOT CswNbtObjClassDocument!
                                CswNbtObjClassSDSDocument NewSDSDocumentNode = NewNode;
                                NewSDSDocumentNode.Title.Text = "SDS: " + MaterialNode.TradeName.Text;
                                NewSDSDocumentNode.FileType.Value = CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link;
                                NewSDSDocumentNode.Link.Href = MsdsUrl;
                                NewSDSDocumentNode.Link.Text = MsdsUrl;
                                NewSDSDocumentNode.Owner.RelatedNodeId = MaterialNode.NodeId;
                                //NewSDSDocumentNode.IsTemp = false;
                                //NewSDSDocumentNode.postChanges( true );
                            } );

                        // Set the return object
                        NewSDSDocumentNodeId = NewDoc.NodeId;
                    }
                }

                return NewSDSDocumentNodeId;
            }

            /// <summary>
            /// Creates a new Vendor node if the vendor doesn't already exist otherwise uses the pre-existing Vendor node.
            /// </summary>
            /// <param name="VendorName"></param>
            /// <returns>A C3CreateMaterialResponse.State.Supplier object with the Vendor name and Vendor nodeid set.</returns>
            public C3CreateMaterialResponse.State.Supplier createVendorNode( string VendorName )
            {
                C3CreateMaterialResponse.State.Supplier Supplier = new C3CreateMaterialResponse.State.Supplier();

                CswNbtView VendorView = new CswNbtView( _CswNbtResources );
                VendorView.ViewName = "VendorWithNameEquals";

                CswNbtMetaDataObjectClass VendorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
                CswNbtViewRelationship Parent = VendorView.AddViewRelationship( VendorOC, true );

                CswNbtMetaDataObjectClassProp VendorOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );

                CswNbtViewProperty ViewProperty1 = VendorView.AddViewProperty( Parent, VendorOCP );

                CswNbtViewPropertyFilter Filter1 = VendorView.AddViewPropertyFilter( ViewProperty1,
                                                          CswEnumNbtFilterConjunction.And,
                                                          CswEnumNbtFilterResultMode.Hide,
                                                          CswEnumNbtSubFieldName.Text,
                                                          CswEnumNbtFilterMode.Equals,
                                                          VendorName,
                                                          false,
                                                          false );

                ICswNbtTree VendorsTree = _CswNbtResources.Trees.getTreeFromView( VendorView, false, true, true );
                if( VendorsTree.getChildNodeCount() > 0 )
                {
                    VendorsTree.goToNthChild( 0 );

                    // Add to the return object
                    Supplier.name = VendorsTree.getNodeNameForCurrentPosition();
                    Supplier.val = VendorsTree.getNodeIdForCurrentPosition().ToString();
                }
                else
                {
                    // Don't create a new node just return an empty value in the return object - Case 28687
                    Supplier.name = VendorName;
                    Supplier.val = string.Empty;
                }

                return Supplier;
            }//createVendorNode()

            public Collection<C3CreateMaterialResponse.State.SizeRecord> createSizeNodes( CswNbtPropertySetMaterial MaterialNode )
            {
                // Return object
                Collection<C3CreateMaterialResponse.State.SizeRecord> ProductSizes = new Collection<C3CreateMaterialResponse.State.SizeRecord>();

                CswNbtMetaDataNodeType SizeNT = _CswNbtResources.MetaData.getNodeType( "Size" );
                if( null != SizeNT )
                {
                    // First set the sizes to import to the original set of sizes
                    _SizesToImport = _ProductToImport.ProductSize.ToList();

                    // If we have more than 1 size there is a possibility we will have duplicates
                    if( _SizesToImport.Count > 1 )
                    {
                        _removeDuplicateSizes();
                    }
                    for( int index = 0; index < _SizesToImport.Count; index++ )
                    {
                        CswC3Product.Size CurrentSize = _SizesToImport[index];
                        CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            // Don't forget to send in the index so that the correct values get added to the NTPs
                            addNodeTypeProps( NewNode, index );
                            ( (CswNbtObjClassSize) NewNode ).Material.RelatedNodeId = MaterialNode.NodeId;
                            //sizeNode.IsTemp = false;
                            //sizeNode.postChanges( true );
                        } );

                        //Set the return object
                        C3CreateMaterialResponse.State.SizeRecord Size = new C3CreateMaterialResponse.State.SizeRecord();

                        //sizeNodeId
                        C3CreateMaterialResponse.State.SizeRecord.SizeData SizeNodeId = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        SizeNodeId.value = sizeNode.NodeId.ToString();
                        SizeNodeId.readOnly = true;
                        SizeNodeId.hidden = true;
                        Size.nodeId = SizeNodeId;

                        //unitCount
                        C3CreateMaterialResponse.State.SizeRecord.SizeData UnitCount = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        UnitCount.value = CswConvert.ToString( sizeNode.UnitCount.Value );
                        UnitCount.readOnly = true;
                        UnitCount.hidden = false;
                        Size.unitCount = UnitCount;

                        //initialQuantity
                        C3CreateMaterialResponse.State.SizeRecord.SizeData InitialQuantity = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        InitialQuantity.value = CswConvert.ToString( sizeNode.InitialQuantity.Quantity );
                        InitialQuantity.readOnly = true;
                        InitialQuantity.hidden = false;
                        Size.quantity = InitialQuantity;

                        //newUoM
                        C3CreateMaterialResponse.State.SizeRecord.SizeData NewUoM = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        NewUoM.value = sizeNode.InitialQuantity.CachedUnitName;
                        if( string.IsNullOrEmpty( NewUoM.value ) )
                        {
                            NewUoM.readOnly = false;
                        }
                        else
                        {
                            NewUoM.readOnly = true;
                        }
                        NewUoM.hidden = false;
                        Size.uom = NewUoM;

                        //originalUoM
                        C3CreateMaterialResponse.State.SizeRecord.SizeData OriginalUoM = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        OriginalUoM.value = CurrentSize.c3_uom;
                        OriginalUoM.readOnly = true;
                        OriginalUoM.hidden = false;
                        Size.origUom = OriginalUoM;

                        //catalogNo
                        C3CreateMaterialResponse.State.SizeRecord.SizeData CatalogNo = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        CatalogNo.value = sizeNode.CatalogNo.Text;
                        CatalogNo.readOnly = true;
                        CatalogNo.hidden = false;
                        Size.catalogNo = CatalogNo;

                        //quantityEditable
                        C3CreateMaterialResponse.State.SizeRecord.SizeData QuantityEditable = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        QuantityEditable.value = "checked";
                        Size.quantityEditable = QuantityEditable;

                        //dispensible
                        C3CreateMaterialResponse.State.SizeRecord.SizeData Dispensible = new C3CreateMaterialResponse.State.SizeRecord.SizeData();
                        Dispensible.value = "checked";
                        Size.dispensible = Dispensible;

                        ProductSizes.Add( Size );
                    }
                } // if( null != SizeNT )

                return ProductSizes;
            }//createSizeNodes()

            public void createMaterialSynonyms( CswNbtPropertySetMaterial MaterialNode )
            {
                CswNbtMetaDataNodeType MaterialSynonymNT = _CswNbtResources.MetaData.getNodeType( "Material Synonym" );
                if( null != MaterialSynonymNT )
                {
                    for( int index = 0; index < _ProductToImport.Synonyms.Length; index++ )
                    {
                        _CswNbtResources.Nodes.makeNodeFromNodeTypeId( MaterialSynonymNT.NodeTypeId, delegate( CswNbtNode NewNode )
                            {
                                CswNbtObjClassMaterialSynonym MaterialSynonymOC = NewNode;
                                MaterialSynonymOC.Name.Text = _ProductToImport.Synonyms[index];
                                MaterialSynonymOC.Material.RelatedNodeId = MaterialNode.NodeId;
                                //MaterialSynonymOC.IsTemp = false;
                                //MaterialSynonymOC.postChanges( true );
                            } );
                    }
                }
            }

            /// <summary>
            /// Add values to the NodeType Properties of a Node.
            /// </summary>
            /// <param name="Node">The Node whose properties are being filled in.</param>
            /// <param name="CurrentIndex">The current index in the ProductSize array in a CswC3Product object. This is ONLY needed for Size Nodes.</param>
            public void addNodeTypeProps( CswNbtNode Node, int CurrentIndex = 0 )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Node.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NTP in NodeType.getNodeTypeProps() )
                {
                    if( null != Node.Properties[NTP] && _Mappings.ContainsKey( NTP.PropName ) )
                    {
                        C3Mapping C3Mapping = _Mappings[NTP.PropName];
                        switch( Node.Properties[NTP].getFieldTypeValue() )
                        {
                            case CswEnumNbtFieldType.Quantity:
                                string sizeGestalt = string.Empty;
                                string MappedUoM = _SizesToImport[CurrentIndex].pkg_qty_uom;
                                string OriginalUoM = _SizesToImport[CurrentIndex].c3_uom;

                                Tuple<int, string> UnitOfMeasureInfo = _getUnitOfMeasure( MappedUoM, OriginalUoM );
                                if( null != UnitOfMeasureInfo )
                                {
                                    //Node.Properties[NTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName2, UnitOfMeasureInfo.Item2 );
                                    //Node.Properties[NTP].SetPropRowValue( CswEnumNbtPropColumn.Field1_FK, UnitOfMeasureInfo.Item1 );
                                    //sizeGestalt = _SizesToImport[CurrentIndex].pkg_qty + " " + UnitOfMeasureInfo.Item2;
                                    //Node.Properties[NTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, sizeGestalt );
                                    Node.Properties[NTP].AsQuantity.UnitId = UnitOfMeasureInfo.Item1;
                                    Node.Properties[NTP].AsQuantity.CachedUnitName = UnitOfMeasureInfo.Item2;
                                }
                                //else
                                //{
                                //    sizeGestalt = _SizesToImport[CurrentIndex].pkg_qty;
                                //    Node.Properties[NTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, sizeGestalt );
                                //}
                                //Node.Properties[NTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName, _SizesToImport[CurrentIndex].pkg_qty );
                                Node.Properties[NTP].AsQuantity.Quantity = CswConvert.ToDouble( _SizesToImport[CurrentIndex].pkg_qty );

                                // Assumption: We are working with a node that is of NodeType Size
                                if( NodeType.NodeTypeName == "Size" )
                                {
                                    // Set the Unit Count
                                    //CswNbtMetaDataNodeTypeProp UnitCountNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount );
                                    //Node.Properties[UnitCountNTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName, _SizesToImport[CurrentIndex].case_qty );
                                    //Node.Properties[UnitCountNTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, _SizesToImport[CurrentIndex].case_qty );
                                    ( (CswNbtObjClassSize) Node ).UnitCount.Value = CswConvert.ToDouble( _SizesToImport[CurrentIndex].case_qty );
                                    // Set the Catalog No
                                    // This needs to be here because each size has a unique catalogno
                                    //CswNbtMetaDataNodeTypeProp CatalogNoNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );
                                    //Node.Properties[CatalogNoNTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName2, _SizesToImport[CurrentIndex].catalog_no );
                                    //Node.Properties[CatalogNoNTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, _SizesToImport[CurrentIndex].catalog_no );
                                    ( (CswNbtObjClassSize) Node ).CatalogNo.Text = _SizesToImport[CurrentIndex].catalog_no;
                                }

                                break;
                            case CswEnumNbtFieldType.MOL:
                                if( false == string.IsNullOrEmpty( C3Mapping.C3ProductPropertyValue ) )
                                {
                                    string propAttr = new CswPropIdAttr( Node, NTP ).ToString();
                                    string molData = C3Mapping.C3ProductPropertyValue;

                                    string Href;
                                    string FormattedMolString;
                                    CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( _CswNbtResources );

                                    MolecularGraph Mol = MoleculeBuilder.CreateMolFromString( molData );
                                    if( false == Mol.ContainsInvalidAtom() )
                                    {
                                        SdBlobData.saveMol( molData, propAttr, out Href, out FormattedMolString );
                                    }
                                    else
                                    {
                                        _CswNbtResources.logMessage( "Failed to save the MOL file for product with ProductId " + _ProductToImport.ProductId + " during the C3 import process because it contained an invalid atom." );
                                    }
                                }
                                break;
                            default:
                                //Node.Properties[NTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName, C3Mapping.C3ProductPropertyValue );
                                //Node.Properties[NTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, C3Mapping.C3ProductPropertyValue );
                                CswNbtSubField sf = NTP.getFieldTypeRule().SubFields[(CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName];
                                if( null != sf )
                                {
                                    Node.Properties[NTP].SetSubFieldValue( sf, C3Mapping.C3ProductPropertyValue );
                                }
                                break;
                        }
                    }//if( null != Node.Properties[NTP] && _Mappings.ContainsKey( NTP.PropName ) )

                }//foreach( CswNbtMetaDataNodeTypeProp NTP in NodeType.getNodeTypeProps() )

            }//addNodeTypeProps()

        }//class ImportManager

        #endregion Import Mananger

    }

}
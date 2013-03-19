using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;
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
                public bool useExistingTempNode;

                [DataMember]
                public Collection<Collection<SizeColumnValue>> sizes;

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

                [DataContract]
                public class SizeColumnValue
                {
                    [DataMember]
                    public string value { get; set; }

                    [DataMember]
                    public bool hidden { get; set; }
                }

            }
        }

        #endregion

        public static void GetAvailableDataSources( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            _setConfigurationVariables( CswC3Params, _CswNbtResources );

            //Instance a new C3 search and dynamically set the endpoint address
            ChemCatCentral.SearchClient C3SearchClient = new ChemCatCentral.SearchClient();
            _setEndpointAddress( _CswNbtResources, C3SearchClient );

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

            //Instance a new C3 search and dynamically set the endpoint address
            ChemCatCentral.SearchClient C3SearchClient = new ChemCatCentral.SearchClient();
            _setEndpointAddress( _CswNbtResources, C3SearchClient );

            CswRetObjSearchResults SearchResults = C3SearchClient.getProductDetails( CswC3SearchParams );
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

            //Instance a new C3 search and dynamically set the endpoint address
            ChemCatCentral.SearchClient C3SearchClient = new ChemCatCentral.SearchClient();
            _setEndpointAddress( _CswNbtResources, C3SearchClient );

            CswRetObjSearchResults SearchResults = C3SearchClient.search( CswC3SearchParams );

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

            //Instance a new C3 search and dynamically set the endpoint address
            ChemCatCentral.SearchClient C3SearchClient = new ChemCatCentral.SearchClient();
            _setEndpointAddress( _CswNbtResources, C3SearchClient );

            // Perform C3 search to get the product details
            CswRetObjSearchResults SearchResults = C3SearchClient.getProductDetails( CswC3SearchParams );
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
                C3CreateMaterialResponse.State.Supplier Supplier = C3Import.createVendorNode( C3ProductDetails.SupplierName );

                // Create size node(s)
                Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>> ProductSizes = C3Import.createSizeNodes( C3ProductTempNode );

                // Create synonyms node(s)
                C3Import.createMaterialSynonyms( C3ProductTempNode );

                #region Return Object

                Return.Data.success = true;
                Return.Data.actionname = "create material";

                C3CreateMaterialResponse.State.MaterialType MaterialType = new C3CreateMaterialResponse.State.MaterialType();
                MaterialType.name = ChemicalNT.NodeTypeName;
                MaterialType.val = ChemicalNT.NodeTypeId;

                C3CreateMaterialResponse.State State = new C3CreateMaterialResponse.State();
                State.materialId = C3ProductTempNode.NodeId.ToString();
                State.tradeName = C3ProductTempNode.TradeName.Text;
                State.partNo = C3ProductTempNode.PartNumber.Text;
                State.useExistingTempNode = true;
                State.supplier = Supplier;
                State.materialType = MaterialType;
                State.sizes = ProductSizes;

                Return.Data.state = State;

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

                CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );
                if( null != ChemicalNT )
                {
                    #region Object Class Properties

                    const string Tradename = CswNbtObjClassMaterial.PropertyName.Tradename;
                    _Mappings.Add( Tradename, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.TradeName,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( Tradename ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    const string CasNo = CswNbtObjClassMaterial.PropertyName.CasNo;
                    _Mappings.Add( CasNo, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.CasNo,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( CasNo ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    const string PartNumber = CswNbtObjClassMaterial.PropertyName.PartNumber;
                    _Mappings.Add( PartNumber, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.PartNo,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( PartNumber ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    // THIS IS DEFAULTING TO SOLID FOR NOW
                    //todo: write a method that attempts to figure out the physical state by looking at the incoming UOM
                    const string PhysicalState = CswNbtObjClassMaterial.PropertyName.PhysicalState;
                    _Mappings.Add( PhysicalState, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = CswNbtObjClassMaterial.PhysicalStates.Liquid,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( PhysicalState ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    #endregion Object Class Properties

                    #region NodeType Properties

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

                    // Add any additional properties
                    foreach( CswC3Product.TemplateSlctdExtData NameValuePair in _ProductToImport.TemplateSelectedExtensionData )
                    {
                        string PropertyName = NameValuePair.attribute;
                        CswNbtMetaDataNodeTypeProp ChemicalNTP = ChemicalNT.getNodeTypeProp( PropertyName );
                        if( null != ChemicalNTP )
                        {
                            _Mappings.Add( PropertyName, new C3Mapping
                            {
                                NBTNodeTypeId = ChemicalNT.NodeTypeId,
                                C3ProductPropertyValue = NameValuePair.value,
                                NBTNodeTypePropId = ChemicalNTP.PropId,
                                NBTSubFieldPropColName = "field1"
                            } );
                        }
                    }

                    #endregion
                }

                //TODO: In the future, add the MSDS link if it exists

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

                    //const string CatalogNo = CswNbtObjClassSize.PropertyName.CatalogNo;
                    //_Mappings.Add( CatalogNo, new C3Mapping
                    //{
                    //    NBTNodeTypeId = SizeNT.NodeTypeId,
                    //    C3ProductPropertyValue = _ProductToImport.CatalogNo,
                    //    NBTNodeTypePropId = SizeNT.getNodeTypePropIdByObjectClassProp( CatalogNo ),
                    //    NBTSubFieldPropColName = "field1"
                    //} );
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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="unitOfMeasurementName"></param>
            /// <returns></returns>
            private CswNbtObjClassUnitOfMeasure _getUnitOfMeasure( string unitOfMeasurementName )
            {
                CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = null;

                if( false == string.IsNullOrEmpty( unitOfMeasurementName ) )
                {
                    //Translate the name if necessary
                    //string TranslatedUnitOfMeasure = _uomTranslator( unitOfMeasurementName );

                    CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
                    CswNbtMetaDataObjectClassProp NameOCP = UnitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );

                    CswNbtView UnitsView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship Parent = UnitsView.AddViewRelationship( UnitOfMeasureOC, false );

                    UnitsView.AddViewPropertyAndFilter( Parent,
                                                       MetaDataProp: NameOCP,
                                                       Value: unitOfMeasurementName,
                                                       FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( UnitsView, false, false, true );
                    int Count = Tree.getChildNodeCount();
                    for( int i = 0; i < Count; i++ )
                    {
                        Tree.goToNthChild( i );
                        UnitOfMeasureNode = Tree.getNodeForCurrentPosition();
                        Tree.goToParentNode();
                    }
                }

                return UnitOfMeasureNode;

            }//_getUnitOfMeasure

            #endregion Private helper methods

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

                CswNbtMetaDataObjectClass VendorOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.VendorClass );
                CswNbtViewRelationship Parent = VendorView.AddViewRelationship( VendorOC, true );

                CswNbtMetaDataObjectClassProp VendorOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );

                CswNbtViewProperty ViewProperty1 = VendorView.AddViewProperty( Parent, VendorOCP );

                CswNbtViewPropertyFilter Filter1 = VendorView.AddViewPropertyFilter( ViewProperty1,
                                                          CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                          CswNbtPropFilterSql.FilterResultMode.Hide,
                                                          CswNbtSubField.SubFieldName.Text,
                                                          CswNbtPropFilterSql.PropertyFilterMode.Equals,
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
                    CswNbtMetaDataNodeType VendorNT = _CswNbtResources.MetaData.getNodeType( "Vendor" );
                    if( null != VendorNT )
                    {
                        CswNbtObjClassVendor NewVendorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( VendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                        addNodeTypeProps( NewVendorNode.Node );

                        NewVendorNode.IsTemp = false;
                        NewVendorNode.postChanges( true );

                        // Add to the return object
                        Supplier.name = NewVendorNode.NodeName;
                        Supplier.val = NewVendorNode.NodeId.ToString();
                    }
                }

                return Supplier;
            }//createVendorNode()

            public Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>> createSizeNodes( CswNbtObjClassMaterial ChemicalNode )
            {
                // Return object
                Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>> ProductSizes = new Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>>();

                CswNbtMetaDataNodeType SizeNT = _CswNbtResources.MetaData.getNodeType( "Size" );
                if( null != SizeNT )
                {
                    for( int index = 0; index < _ProductToImport.ProductSize.Length; index++ )
                    {
                        CswC3Product.Size CurrentSize = _ProductToImport.ProductSize[index];
                        CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                        // Don't forget to send in the index so that the correct values get added to the NTPs
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
                            Collection<C3CreateMaterialResponse.State.SizeColumnValue> Size = new Collection<C3CreateMaterialResponse.State.SizeColumnValue>();

                            C3CreateMaterialResponse.State.SizeColumnValue UnitCount = new C3CreateMaterialResponse.State.SizeColumnValue();
                            UnitCount.value = CswConvert.ToString( sizeNode.UnitCount.Value );
                            UnitCount.hidden = false;
                            Size.Add( UnitCount );

                            C3CreateMaterialResponse.State.SizeColumnValue InitialQuantity = new C3CreateMaterialResponse.State.SizeColumnValue();
                            InitialQuantity.value = sizeNode.InitialQuantity.Gestalt;
                            InitialQuantity.hidden = false;
                            Size.Add( InitialQuantity );

                            C3CreateMaterialResponse.State.SizeColumnValue CatalogNo = new C3CreateMaterialResponse.State.SizeColumnValue();
                            CatalogNo.value = sizeNode.CatalogNo.Text;
                            CatalogNo.hidden = false;
                            Size.Add( CatalogNo );

                            C3CreateMaterialResponse.State.SizeColumnValue NodeId = new C3CreateMaterialResponse.State.SizeColumnValue();
                            NodeId.value = sizeNode.NodeId.ToString();
                            NodeId.hidden = true;
                            Size.Add( NodeId );

                            ProductSizes.Add( Size );
                        }
                    }
                } // if( null != SizeNT )

                return ProductSizes;
            }//createSizeNodes()

            public void createMaterialSynonyms( CswNbtObjClassMaterial ChemicalNode )
            {
                CswNbtMetaDataNodeType MaterialSynonymNT = _CswNbtResources.MetaData.getNodeType( "Material Synonym" );
                if( null != MaterialSynonymNT )
                {
                    for( int index = 0; index < _ProductToImport.Synonyms.Length; index++ )
                    {
                        CswNbtObjClassMaterialSynonym MaterialSynonymOC = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( MaterialSynonymNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                        MaterialSynonymOC.Name.Text = _ProductToImport.Synonyms[index];
                        MaterialSynonymOC.Material.RelatedNodeId = ChemicalNode.NodeId;
                        MaterialSynonymOC.IsTemp = false;
                        MaterialSynonymOC.postChanges( true );
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
                            case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                                CswNbtObjClassUnitOfMeasure unitOfMeasure = _getUnitOfMeasure( _ProductToImport.ProductSize[CurrentIndex].pkg_qty_uom );
                                if( null != unitOfMeasure )
                                {
                                    Node.Properties[NTP].SetPropRowValue( (CswNbtSubField.PropColumn) C3Mapping.NBTSubFieldPropColName, _ProductToImport.ProductSize[CurrentIndex].pkg_qty );
                                    Node.Properties[NTP].SetPropRowValue( (CswNbtSubField.PropColumn) C3Mapping.NBTSubFieldPropColName2, unitOfMeasure.Name.Text );
                                    Node.Properties[NTP].SetPropRowValue( CswNbtSubField.PropColumn.Field1_FK, unitOfMeasure.NodeId.PrimaryKey );
                                    string sizeGestalt = _ProductToImport.ProductSize[CurrentIndex].pkg_qty + " " + unitOfMeasure.Name.Text;
                                    Node.Properties[NTP].SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, sizeGestalt );

                                    // Note: This is a hackadoodle for now since importer is getting changed... soon...
                                    // Assumption: We are working with a node that is of NodeType Size
                                    if( NodeType.NodeTypeName == "Size" )
                                    {
                                        // Set the Unit Count
                                        CswNbtMetaDataNodeTypeProp UnitCountNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount );
                                        Node.Properties[UnitCountNTP].SetPropRowValue( (CswNbtSubField.PropColumn) C3Mapping.NBTSubFieldPropColName, _ProductToImport.ProductSize[CurrentIndex].case_qty );
                                        Node.Properties[UnitCountNTP].SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, _ProductToImport.ProductSize[CurrentIndex].case_qty );

                                        // Set the Catalog No
                                        // This needs to be here because each size has a unique catalogno
                                        CswNbtMetaDataNodeTypeProp CatalogNoNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );
                                        Node.Properties[CatalogNoNTP].SetPropRowValue( (CswNbtSubField.PropColumn) C3Mapping.NBTSubFieldPropColName2, _ProductToImport.ProductSize[CurrentIndex].catalog_no );
                                        Node.Properties[CatalogNoNTP].SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, _ProductToImport.ProductSize[CurrentIndex].catalog_no );
                                    }
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

        /// <summary>
        /// Dynamically set the endpoint address for a ChemCatCentral SearchClient.
        /// </summary>
        /// <param name="CswNbtResources"></param>
        /// <param name="C3SearchClient"></param>
        private static void _setEndpointAddress( CswNbtResources CswNbtResources, ChemCatCentral.SearchClient C3SearchClient )
        {
            if( null != C3SearchClient )
            {
                string C3_UrlStem = CswNbtResources.SetupVbls[CswSetupVariableNames.C3UrlStem];
                EndpointAddress URI = new EndpointAddress( C3_UrlStem );
                C3SearchClient.Endpoint.Address = URI;
            }
        }

        #endregion
    }

}
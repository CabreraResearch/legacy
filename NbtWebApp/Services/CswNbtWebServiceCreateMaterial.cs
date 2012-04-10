using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceCreateMaterial
    {
        #region ctor

        private CswNbtMetaDataNodeType _getMaterialNt( Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType Ret = null;
            if( Int32.MinValue != NodeTypeId )
            {
                Ret = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( null == Ret ||
                    Ret.getObjectClass().ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "The provided material type was not a valid material.",
                                               "Attempted to call _MaterialNt with a NodeType that was not valid." );
                }
            }
            return Ret;
        }
        private CswNbtMetaDataNodeType _MaterialNt;

        private CswNbtView _getMaterialNodeView( CswNbtNode MaterialNode )
        {
            CswNbtView Ret = null;
            if( MaterialNode != null )
            {
                Ret = MaterialNode.getViewOfNode();
                CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
                Ret.AddViewRelationship( Ret.Root.ChildRelationships[0], NbtViewPropOwnerType.Second, MaterialOcp, false );
                Ret.ViewName = "Create Material: " + MaterialNode.NodeName;
            }
            return Ret;
        }

        private CswNbtView _getMaterialNodeView( string Tradename, string Supplier, string PartNo = "" )
        {
            CswNbtView Ret = null;
            if( _MaterialNt != null )
            {
                if( string.IsNullOrEmpty( Supplier ) ||
                    string.IsNullOrEmpty( Tradename ) )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Cannot get a material without a supplier and a tradename.",
                                               "Attempted to call _getMaterialNodeView with invalid or empty parameters." );
                }
                Ret = new CswNbtView( _CswNbtResources );
                Ret.ViewMode = NbtViewRenderingMode.Tree;
                Ret.Visibility = NbtViewVisibility.User;
                Ret.VisibilityUserId = _CswNbtResources.CurrentNbtUser.UserId;
                CswNbtViewRelationship MaterialRel = Ret.AddViewRelationship( _MaterialNt, false );
                CswNbtMetaDataNodeTypeProp TradeNameNtp = _MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.TradenamePropName );
                CswNbtMetaDataNodeTypeProp SupplierNtp = _MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.SupplierPropertyName );
                CswNbtMetaDataNodeTypeProp PartNoNtp = _MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PartNumberPropertyName );

                Ret.AddViewPropertyAndFilter( MaterialRel, TradeNameNtp, Tradename );
                Ret.AddViewPropertyAndFilter( MaterialRel, SupplierNtp, Supplier );
                Ret.AddViewPropertyAndFilter( MaterialRel, PartNoNtp, PartNo );

                CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
                Ret.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, MaterialOcp, false );

                Ret.ViewName = "Create Material: " + Tradename;
            }
            return Ret;
        }
        private CswNbtView _MaterialNodeView;

        private CswNbtResources _CswNbtResources;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 NodeTypeId, string Supplier, string Tradename, string PartNo )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            if( Int32.MinValue == NodeTypeId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a type.",
                                           "Attempted to call doesMaterialExist with invalid or empty parameters." );
            }
            _MaterialNt = _getMaterialNt( NodeTypeId );
            _MaterialNodeView = _getMaterialNodeView( Tradename, Supplier, PartNo );
        }

        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources,
            CswNbtStatisticsEvents CswNbtStatisticsEvents,
            string MaterialDefinition,
            out JObject MaterialObj,
            out CswNbtNode MaterialNode )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            MaterialObj = CswConvert.ToJObject( MaterialDefinition, true, "material" );
            MaterialNode = null;
            bool UseExistingMaterial = CswConvert.ToBoolean( MaterialObj["useexistingmaterial"] );

            if( UseExistingMaterial )
            {
                string NodeId = CswConvert.ToString( MaterialObj["materialnodeid"] );
                CswPrimaryKey NodePk = new CswPrimaryKey();
                NodePk.FromString( NodeId );
                MaterialNode = _CswNbtResources.Nodes.GetNode( NodePk );
                if( null == MaterialNode )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "The provided material definition does not match an existing material.",
                                               "Attempted to call CswNbtWebServiceCreateMaterial without tradename and supplier properties." );
                }
                _MaterialNodeView = _getMaterialNodeView( MaterialNode );
                _MaterialNt = MaterialNode.getNodeType();
            }
            else
            {
                string TradeName = CswConvert.ToString( MaterialObj["tradename"] );
                string SupplierName = CswConvert.ToString( MaterialObj["suppliername"] );

                if( string.IsNullOrEmpty( TradeName ) || string.IsNullOrEmpty( SupplierName ) )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Cannot create a new material without a Tradename and a Supplier.",
                                               "Attempted to call CswNbtWebServiceCreateMaterial without tradename and supplier properties." );
                }

                string PartNo = CswConvert.ToString( MaterialObj["partno"] );
                _MaterialNodeView = _getMaterialNodeView( TradeName, SupplierName, PartNo );
                Int32 NodeTypeId = CswConvert.ToInt32( MaterialObj["materialnodetypeid"] );
                if( Int32.MinValue == NodeTypeId )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Cannot get a material without a type.",
                                               "Attempted to call CswNbtWebServiceCreateMaterial with invalid or empty parameters." );
                }
                _MaterialNt = _getMaterialNt( NodeTypeId );
            }
        }

        #endregion ctor

        #region Public

        public bool doesMaterialExist( JObject MaterialObj = null )
        {
            return _doesMaterialExist( false, MaterialObj );
        }

        private bool _doesMaterialExist( bool ThrowIfExists = true, JObject MaterialObj = null )
        {
            bool RetExists = false;

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _MaterialNodeView, false );
            if( Tree.getChildNodeCount() > 0 )
            {
                RetExists = true;
                if( null != MaterialObj )
                {
                    Tree.goToNthChild( 0 );
                    CswNbtNode Node = Tree.getNodeForCurrentPosition();
                    CswNbtObjClassMaterial NodeAsMaterial = CswNbtNodeCaster.AsMaterial( Node );

                    string TradeName = NodeAsMaterial.TradeName.Text;
                    string SupplierName = NodeAsMaterial.Supplier.Gestalt;
                    string PartNo = NodeAsMaterial.PartNumber.Text;

                    if( ThrowIfExists )
                    {

                        throw new CswDniException( ErrorType.Error,
                                                   "A material already exists with the provided Tradename, Supplier and Part Number.",
                                                   "Attempted to call createMaterial with tradename: " + TradeName + ", supplier: " + SupplierName + " and partno: " + PartNo + " properties." );

                    }

                    MaterialObj["tradename"] = TradeName;
                    MaterialObj["partno"] = PartNo;
                    MaterialObj["supplier"] = SupplierName;
                    MaterialObj["nodeid"] = Node.NodeId.ToString();
                }
            }

            return RetExists;
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode )
        {
            JObject SizeObj = CswConvert.ToJObject( SizeDefinition, true, "size" );
            CswNbtNode SizeNode;
            return getSizeNodeProps( CswNbtResources, CswNbtStatisticsEvents, SizeNodeTypeId, SizeObj, WriteNode, out SizeNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode, out CswNbtNode SizeNode )
        {
            JObject SizeObj = CswConvert.ToJObject( SizeDefinition, true, "size" );
            return getSizeNodeProps( CswNbtResources, CswNbtStatisticsEvents, SizeNodeTypeId, SizeObj, WriteNode, out SizeNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 SizeNodeTypeId, JObject SizeObj, bool WriteNode, out CswNbtNode SizeNode )
        {
            JObject Ret = new JObject();

            SizeNode = CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing, true );
            CswNbtWebServiceNode NodeWs = new CswNbtWebServiceNode( CswNbtResources, CswNbtStatisticsEvents );
            NodeWs.addNodeProps( SizeNode, SizeObj, null );
            CswNbtObjClassSize NodeAsSize = CswNbtNodeCaster.AsSize( SizeNode );
            JArray Row = new JArray();
            Ret["row"] = Row;

            Row.Add( "(New Size)" );
            Row.Add( NodeAsSize.Capacity.Gestalt );
            Row.Add( NodeAsSize.Dispensable.Gestalt );
            Row.Add( NodeAsSize.QuantityEditable.Gestalt );

            if( WriteNode )
            {
                SizeNode.postChanges( true );
            }
            return Ret;
        }

        public static JObject getMaterialSizes( CswNbtResources CswNbtResources, CswPrimaryKey MaterialId )
        {
            JObject Ret = new JObject();

            if( null == MaterialId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get material's sizes without a valid materialid.",
                                           "Attempted to call getMaterialSizes with invalid or empty parameters." );
            }

            CswNbtNode MaterialNode = CswNbtResources.Nodes.GetNode( MaterialId );

            if( null == MaterialNode ||
                 MaterialNode.getObjectClass().ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided node was not a valid material.",
                                           "Attempted to call getMaterialSizes with a node that was not valid." );
            }

            CswNbtView SizesView = new CswNbtView( CswNbtResources );  //MaterialNode.getNodeType().CreateDefaultView();
            SizesView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtMetaDataObjectClass SizeOc = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
            CswNbtViewRelationship SizeRel = SizesView.AddViewRelationship( SizeOc, false );

            SizesView.AddViewPropertyAndFilter( SizeRel, SizeMaterialOcp, MaterialId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName ) );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName ) );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName ) );

            CswNbtWebServiceGrid wsG = new CswNbtWebServiceGrid( CswNbtResources, SizesView );
            Ret["rows"] = wsG.getThinGridRows( Int32.MinValue, true );

            return Ret;
        }



        public JObject createMaterial( JObject MaterialObj, CswNbtNode MaterialNode = null )
        {
            JObject RetObj = new JObject();
            JArray SizesArray = (JArray) MaterialObj["sizes"];

            if( null == MaterialNode )
            {
                /* 1. Validate the new material and get its properties and sizes */
                _doesMaterialExist( true );
                JObject MaterialProperties = (JObject) MaterialObj["properties"];

                /* 2. Create the node */
                CswNbtWebServiceTabsAndProps wsTap = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                CswNbtNodeKey MaterialNodeKey;
                CswNbtMetaDataNodeType MaterialNt = _MaterialNt;
                wsTap.addNode( MaterialNt, out MaterialNode, MaterialProperties, out MaterialNodeKey );
                RetObj["createdmaterial"] = true;
            }

            /* 3. Add the sizes */
            _addMaterialSizes( SizesArray, MaterialNode );
            RetObj["sizescount"] = SizesArray.Count;

            /* 4. Add possible secondary actions 
             * Recieve Material and Request Material workflows don't exist yet.
             * For now, return a view of the new Node.             
             */
            _MaterialNodeView.SaveToCache( false );
            RetObj["nextoptions"] = new JObject();
            RetObj["nextoptions"]["nodeview"] = _MaterialNodeView.SessionViewId.ToString();

            return RetObj;
        }

        /// <summary>
        /// Make nodes for defined sizes, else remove undefinable sizes from the JArray
        /// </summary>
        private void _addMaterialSizes( JArray SizesArray, CswNbtNode MaterialNode )
        {
            foreach( JObject SizeObj in SizesArray )
            {
                if( SizeObj.HasValues )
                {
                    CswNbtNode SizeNode;
                    JObject SizeDef = (JObject) SizeObj["sizedef"];
                    Int32 SizeNtId = CswConvert.ToInt32( SizeObj["nodetypeid"] );
                    if( Int32.MinValue != SizeNtId )
                    {
                        getSizeNodeProps( _CswNbtResources, _CswNbtStatisticsEvents, SizeNtId, SizeDef, true, out SizeNode );
                        if( null != SizeNode )
                        {
                            CswNbtObjClassSize NodeAsSize = CswNbtNodeCaster.AsSize( SizeNode );
                            NodeAsSize.Material.RelatedNodeId = MaterialNode.NodeId;
                            SizeNode.postChanges( true );
                        }
                        else
                        {
                            SizesArray.Remove( SizeObj );
                        }
                    }
                    else
                    {
                        SizesArray.Remove( SizeObj );
                    }
                    //Int32 CapacityQty = CswConvert.ToInt32( SizeObj["capacityqty"] );
                    //string CapacityUnit = SizeObj["capacityunit"].ToString();
                    //if( Int32.MinValue != CapacityQty && false == string.IsNullOrEmpty( CapacityUnit ) )
                    //{
                    //    Tristate Dispensable = CswConvert.ToTristate( SizeObj["dispensable"] );
                    //    Tristate QuantityEditable = CswConvert.ToTristate( SizeObj["quantityeditable"] );
                    //    CswNbtMetaDataNodeType MaterialNt = _MaterialNt;
                    //    CswNbtNode SizeNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( MaterialNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    //    CswNbtObjClassSize NodeAsSize = CswNbtNodeCaster.AsSize( SizeNode );
                    //    NodeAsSize.Material.RelatedNodeId = MaterialNode.NodeId;
                    //    NodeAsSize.Capacity.Quantity = CapacityQty;
                    //    NodeAsSize.Capacity.Units = CapacityUnit;
                    //    NodeAsSize.Dispensable.Checked = Dispensable;
                    //    NodeAsSize.QuantityEditable.Checked = QuantityEditable;
                    //    SizeNode.postChanges( true );
                    //}
                    //else
                    //{
                    //    SizesArray.Remove( SizeObj );
                    //}
                }
                else
                {
                    SizesArray.Remove( SizeObj );
                }
            }
        }

        #endregion Public

    }
}
using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceCreateMaterial
    {
        #region ctor

        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor

        private CswNbtMetaDataNodeType __MaterialNt;
        private CswNbtMetaDataNodeType _MaterialNt( Int32 NodeTypeId = Int32.MinValue )
        {
            if( null == __MaterialNt || ( Int32.MinValue != NodeTypeId && __MaterialNt.NodeTypeId != NodeTypeId ) )
            {
                __MaterialNt = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( null == __MaterialNt ||
                    __MaterialNt.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "The provided material type was not a valid material.",
                                               "Attempted to call _MaterialNt with a NodeType that was not valid." );
                }
            }
            return __MaterialNt;
        }

        private CswNbtView __MaterialNodeView;
        private CswNbtView _MaterialNodeView( CswNbtMetaDataNodeType MaterialNt = null, string Tradename = "", string Supplier = "", string PartNo = "" )
        {
            if( null == __MaterialNodeView && MaterialNt != null )
            {
                if( string.IsNullOrEmpty( Supplier ) ||
                    string.IsNullOrEmpty( Tradename ) )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Cannot get a material without a supplier and a tradename.",
                                               "Attempted to call doesMaterialExist with invalid or empty parameters." );
                }
                __MaterialNodeView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship MaterialRel = __MaterialNodeView.AddViewRelationship( MaterialNt, false );
                CswNbtMetaDataNodeTypeProp TradeNameNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.TradenamePropName );
                CswNbtMetaDataNodeTypeProp SupplierNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.SupplierPropertyName );
                CswNbtMetaDataNodeTypeProp PartNoNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PartNumberPropertyName );

                __MaterialNodeView.AddViewPropertyAndFilter( MaterialRel, TradeNameNtp, Tradename );
                __MaterialNodeView.AddViewPropertyAndFilter( MaterialRel, SupplierNtp, Supplier );
                __MaterialNodeView.AddViewPropertyAndFilter( MaterialRel, PartNoNtp, PartNo );
            }
            return __MaterialNodeView;
        }

        #region Public

        public bool doesMaterialExist( Int32 NodeTypeId, string Supplier, string Tradename, string PartNo, JObject MaterialObj = null )
        {
            if( Int32.MinValue == NodeTypeId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a type.",
                                           "Attempted to call doesMaterialExist with invalid or empty parameters." );
            }
            CswNbtMetaDataNodeType MaterialNt = _MaterialNt( NodeTypeId );

            CswNbtView MaterialNodeView = _MaterialNodeView( MaterialNt, Tradename, Supplier, PartNo );
            return _doesMaterialExist( MaterialNodeView, MaterialObj );
        }

        private bool _doesMaterialExist( CswNbtView MaterialNodeView, JObject MaterialObj = null )
        {
            bool RetExists = false;

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( MaterialNodeView, false );
            if( Tree.getChildNodeCount() > 0 )
            {
                RetExists = true;
                if( null != MaterialObj )
                {
                    Tree.goToNthChild( 0 );
                    CswNbtNode Node = Tree.getNodeForCurrentPosition();
                    CswNbtObjClassMaterial NodeAsMaterial = CswNbtNodeCaster.AsMaterial( Node );
                    MaterialObj["tradename"] = NodeAsMaterial.TradeName.Text;
                    MaterialObj["partno"] = NodeAsMaterial.PartNumber.Text;
                    MaterialObj["supplier"] = NodeAsMaterial.Supplier.Gestalt;
                    MaterialObj["nodeid"] = Node.NodeId.ToString();
                }
            }

            return RetExists;
        }

        public CswNbtView getMaterialSizes( CswPrimaryKey MaterialId )
        {
            CswNbtView RetView;

            if( null == MaterialId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get material's sizes without a valid materialid.",
                                           "Attempted to call getMaterialSizes with invalid or empty parameters." );
            }

            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( MaterialId );

            if( null == MaterialNode ||
                 MaterialNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided node was not a valid material.",
                                           "Attempted to call getMaterialSizes with a node that was not valid." );
            }

            RetView = new CswNbtView( _CswNbtResources );  //MaterialNode.getNodeType().CreateDefaultView();
            RetView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
            CswNbtViewRelationship SizeRel = RetView.AddViewRelationship( SizeOc, false );

            RetView.AddViewPropertyAndFilter( SizeRel, SizeMaterialOcp, MaterialId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            RetView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName ) );
            RetView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName ) );
            RetView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName ) );

            return RetView;
        }

        public JObject createMaterial( string MaterialDefinition )
        {
            JObject RetObj = new JObject();

            /* 1. Validate the new material and get its properties and sizes */
            JArray SizesArray;
            JObject MaterialProperties;
            _getMaterialPropsAndSizes( MaterialDefinition, out SizesArray, out MaterialProperties );

            /* 2. Create the node */
            CswNbtWebServiceTabsAndProps wsTap = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
            CswNbtNode MaterialNode;
            CswNbtNodeKey MaterialNodeKey;
            CswNbtMetaDataNodeType MaterialNt = _MaterialNt();
            wsTap.addNode( MaterialNt, out MaterialNode, MaterialProperties, out MaterialNodeKey );
            RetObj["createdmaterial"] = true;

            /* 3. Add the sizes */
            _addMaterialSizes( SizesArray, MaterialNode );
            RetObj["sizescount"] = SizesArray.Count;

            /* 4. Add possible secondary actions 
             * Recieve Material and Request Material workflows don't exist yet.
             * For now, return a view of the new Node.             
             */
            CswNbtView MaterialNodeView = _MaterialNodeView();
            MaterialNodeView.SaveToCache( false );
            RetObj["nextoptions"] = new JObject();
            RetObj["nextoptions"]["nodeview"] = MaterialNodeView.SessionViewId.get();

            return RetObj;
        }

        /// <summary>
        /// Validate the new material node, out the material node properties and sizes, and cache the material nodetype and material node view
        /// </summary>
        private void _getMaterialPropsAndSizes( string MaterialDefinition, out JArray SizesObj, out JObject PropertiesObj )
        {
            if( string.IsNullOrEmpty( MaterialDefinition ) )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot create a new material without a definition.",
                                           "Attempted to call createMaterial with a node definition that was not valid." );
            }
            JObject MaterialObj = JObject.Parse( MaterialDefinition );

            if( null == MaterialObj || false == MaterialObj.HasValues )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The material definition could not be parsed into an object.",
                                           "Attempted to call createMaterial with a JSON object that was not valid." );
            }

            string TradeName = MaterialObj["tradename"].ToString();
            string SupplierName = MaterialObj["suppliername"].ToString();

            if( string.IsNullOrEmpty( TradeName ) || string.IsNullOrEmpty( SupplierName ) )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot create a new material without a Tradename and a Supplier.",
                                           "Attempted to call createMaterial without tradename and supplier properties." );
            }

            string PartNo = MaterialObj["partno"].ToString();
            Int32 NodeTypeId = CswConvert.ToInt32( MaterialObj["nodetypeid"] );

            CswNbtMetaDataNodeType MaterialNt = _MaterialNt( NodeTypeId );
            CswNbtView MaterialNodeView = _MaterialNodeView( MaterialNt, TradeName, SupplierName, PartNo );

            bool MaterialExists = _doesMaterialExist( MaterialNodeView );
            if( MaterialExists )
            {
                throw new CswDniException( ErrorType.Error,
                                           "A material already exists with the provided Tradename, Supplier and Part Number.",
                                           "Attempted to call createMaterial with tradename: " + TradeName + ", supplier: " + SupplierName + " and partno: " + PartNo + " properties." );
            }

            PropertiesObj = (JObject) MaterialObj["properties"];
            SizesObj = (JArray) MaterialObj["sizes"];
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
                    Int32 CapacityQty = CswConvert.ToInt32( SizeObj["capacityqty"] );
                    string CapacityUnit = SizeObj["capacityunit"].ToString();
                    if( Int32.MinValue != CapacityQty && false == string.IsNullOrEmpty( CapacityUnit ) )
                    {
                        Tristate Dispensable = CswConvert.ToTristate( SizeObj["dispensable"] );
                        Tristate QuantityEditable = CswConvert.ToTristate( SizeObj["quantityeditable"] );
                        CswNbtMetaDataNodeType MaterialNt = _MaterialNt();
                        CswNbtNode SizeNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( MaterialNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                        CswNbtObjClassSize NodeAsSize = CswNbtNodeCaster.AsSize( SizeNode );
                        NodeAsSize.Material.RelatedNodeId = MaterialNode.NodeId;
                        NodeAsSize.Capacity.Quantity = CapacityQty;
                        NodeAsSize.Capacity.Units = CapacityUnit;
                        NodeAsSize.Dispensable.Checked = Dispensable;
                        NodeAsSize.QuantityEditable.Checked = QuantityEditable;
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
            }
        }

        #endregion Public

    }
}
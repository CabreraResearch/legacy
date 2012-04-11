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

        private CswNbtMetaDataNodeType __MaterialNt( Int32 NodeTypeId )
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

        private CswNbtView __MaterialNodeView( string Tradename, string Supplier, string PartNo = "" )
        {
            CswNbtView Ret = null;
            if( _MaterialNt != null )
            {
                if( string.IsNullOrEmpty( Supplier ) ||
                    string.IsNullOrEmpty( Tradename ) )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Cannot get a material without a supplier and a tradename.",
                                               "Attempted to call doesMaterialExist with invalid or empty parameters." );
                }
                Ret = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship MaterialRel = Ret.AddViewRelationship( _MaterialNt, false );
                CswNbtMetaDataNodeTypeProp TradeNameNtp = _MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.TradenamePropName );
                CswNbtMetaDataNodeTypeProp SupplierNtp = _MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.SupplierPropertyName );
                CswNbtMetaDataNodeTypeProp PartNoNtp = _MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PartNumberPropertyName );

                _MaterialNodeView.AddViewPropertyAndFilter( MaterialRel, TradeNameNtp, Tradename );
                _MaterialNodeView.AddViewPropertyAndFilter( MaterialRel, SupplierNtp, Supplier );
                _MaterialNodeView.AddViewPropertyAndFilter( MaterialRel, PartNoNtp, PartNo );
            }
            return Ret;
        }
        private CswNbtView _MaterialNodeView;

        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources, Int32 NodeTypeId, string Supplier, string Tradename, string PartNo )
        {
            _CswNbtResources = CswNbtResources;

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
            _MaterialNt = __MaterialNt( NodeTypeId );
            _MaterialNodeView = __MaterialNodeView( Tradename, Supplier, PartNo );
        }

        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources, string MaterialDefinition, out JObject MaterialObj )
        {
            _CswNbtResources = CswNbtResources;

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            if( string.IsNullOrEmpty( MaterialDefinition ) )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot create a new material without a definition.",
                                           "Attempted to call createMaterial with a node definition that was not valid." );
            }
            MaterialObj = JObject.Parse( MaterialDefinition );

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
            if( Int32.MinValue == NodeTypeId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a type.",
                                           "Attempted to call doesMaterialExist with invalid or empty parameters." );
            }
            _MaterialNt = __MaterialNt( NodeTypeId );
            _MaterialNodeView = __MaterialNodeView( TradeName, SupplierName, PartNo );
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

        public static CswNbtView getMaterialSizes( CswNbtResources CswNbtResources, CswPrimaryKey MaterialId )
        {
            CswNbtView RetView;

            if( null == MaterialId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get material's sizes without a valid materialid.",
                                           "Attempted to call getMaterialSizes with invalid or empty parameters." );
            }

            CswNbtNode MaterialNode = CswNbtResources.Nodes.GetNode( MaterialId );

            if( null == MaterialNode ||
                 MaterialNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided node was not a valid material.",
                                           "Attempted to call getMaterialSizes with a node that was not valid." );
            }

            RetView = new CswNbtView( CswNbtResources );  //MaterialNode.getNodeType().CreateDefaultView();
            RetView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtMetaDataObjectClass SizeOc = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
            CswNbtViewRelationship SizeRel = RetView.AddViewRelationship( SizeOc, false );

            RetView.AddViewPropertyAndFilter( SizeRel, SizeMaterialOcp, MaterialId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            RetView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName ) );
            RetView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName ) );
            RetView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName ) );

            return RetView;
        }

        public JObject createMaterial( JObject MaterialObj )
        {
            JObject RetObj = new JObject();

            /* 1. Validate the new material and get its properties and sizes */
            JArray SizesArray;
            JObject MaterialProperties;
            _getMaterialPropsAndSizes( MaterialObj, out SizesArray, out MaterialProperties );

            /* 2. Create the node */
            CswNbtWebServiceTabsAndProps wsTap = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
            CswNbtNode MaterialNode;
            CswNbtNodeKey MaterialNodeKey;
            CswNbtMetaDataNodeType MaterialNt = _MaterialNt;
            wsTap.addNode( MaterialNt, out MaterialNode, MaterialProperties, out MaterialNodeKey );
            RetObj["createdmaterial"] = true;

            /* 3. Add the sizes */
            _addMaterialSizes( SizesArray, MaterialNode );
            RetObj["sizescount"] = SizesArray.Count;

            /* 4. Add possible secondary actions 
             * Recieve Material and Request Material workflows don't exist yet.
             * For now, return a view of the new Node.             
             */
            _MaterialNodeView.SaveToCache( false );
            RetObj["nextoptions"] = new JObject();
            RetObj["nextoptions"]["nodeview"] = _MaterialNodeView.SessionViewId.get();

            return RetObj;
        }

        /// <summary>
        /// Validate the new material node, out the material node properties and sizes, and cache the material nodetype and material node view
        /// </summary>
        private void _getMaterialPropsAndSizes( JObject MaterialObj, out JArray SizesObj, out JObject PropertiesObj )
        {
            _doesMaterialExist( true );
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
                        CswNbtMetaDataNodeType MaterialNt = _MaterialNt;
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
using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceCreateMaterial
    {
        #region private

        private CswNbtMetaDataNodeType _getMaterialNt( object Id )
        {
            Int32 NodeTypeId = CswConvert.ToInt32( Id );
            if( Int32.MinValue == NodeTypeId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a type.",
                                           "Attempted to call CswNbtWebServiceCreateMaterial with invalid or empty parameters." );
            }

            CswNbtMetaDataNodeType Ret = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            if( null == Ret ||
                Ret.getObjectClass().ObjectClass != CswNbtMetaDataObjectClassName.NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided material type was not a valid material.",
                                           "Attempted to call _MaterialNt with a NodeType that was not valid." );
            }

            return Ret;
        }

        private CswNbtView _getMaterialNodeView( CswNbtNode MaterialNode )
        {
            CswNbtView Ret = null;
            if( MaterialNode != null )
            {
                Ret = MaterialNode.getViewOfNode();
                CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                Ret.AddViewRelationship( Ret.Root.ChildRelationships[0], NbtViewPropOwnerType.Second, MaterialOcp, false );
                Ret.ViewName = "New Material: " + MaterialNode.NodeName;
            }
            return Ret;
        }

        private CswNbtView _getMaterialNodeView( Int32 NodeTypeId, string Tradename, CswPrimaryKey SupplierId, string PartNo = "" )
        {

            if( false == CswTools.IsPrimaryKey( SupplierId ) ||
                    string.IsNullOrEmpty( Tradename ) )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a supplier and a tradename.",
                                           "Attempted to call _getMaterialNodeView with invalid or empty parameters." );
            }

            CswNbtView Ret = new CswNbtView( _CswNbtResources );
            Ret.ViewMode = NbtViewRenderingMode.Tree;
            Ret.Visibility = NbtViewVisibility.User;
            Ret.VisibilityUserId = _CswNbtResources.CurrentNbtUser.UserId;
            CswNbtMetaDataNodeType MaterialNt = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtViewRelationship MaterialRel = Ret.AddViewRelationship( MaterialNt, false );
            CswNbtMetaDataNodeTypeProp TradeNameNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Tradename );
            CswNbtMetaDataNodeTypeProp SupplierNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
            CswNbtMetaDataNodeTypeProp PartNoNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.PartNumber );

            Ret.AddViewPropertyAndFilter( MaterialRel, TradeNameNtp, Tradename );
            Ret.AddViewPropertyAndFilter( MaterialRel, SupplierNtp, SupplierId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            Ret.AddViewPropertyAndFilter( MaterialRel, PartNoNtp, PartNo );

            CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            Ret.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, MaterialOcp, false );

            Ret.ViewName = "New Material: " + Tradename;

            return Ret;
        }

        private void _getMaterialPropsFromObject( JObject Obj, out string Tradename, out string Supplier, out string PartNo )
        {
            Tradename = CswConvert.ToString( Obj["tradename"] );
            Supplier = CswConvert.ToString( Obj["suppliername"] );
            PartNo = CswConvert.ToString( Obj["partno"] );
            if( string.IsNullOrEmpty( Tradename ) || string.IsNullOrEmpty( Supplier ) )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot create a new material without a Tradename and a Supplier.",
                                           "Attempted to call _getMaterialPropsFromObject without tradename and supplier properties." );
            }
        }

        private void _getMaterialPropsFromObject( JObject Obj, out string Tradename, out CswPrimaryKey SupplierId, out string PartNo )
        {
            Tradename = CswConvert.ToString( Obj["tradename"] );
            SupplierId = new CswPrimaryKey();
            string SupplierPk = CswConvert.ToString( Obj["supplierid"] );
            SupplierId.FromString( SupplierPk );
            PartNo = CswConvert.ToString( Obj["partno"] );
            if( string.IsNullOrEmpty( Tradename ) || Int32.MinValue == SupplierId.PrimaryKey )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot create a new material without a Tradename and a Supplier.",
                                           "Attempted to call _getMaterialPropsFromObject without tradename and supplier properties." );
            }
        }

        private CswNbtObjClassMaterial _getExistingMaterial( Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo, bool ThrowIfExists = true )
        {
            CswNbtObjClassMaterial Ret = null;

            CswNbtView MaterialNodeView = _getMaterialNodeView( MaterialNodeTypeId, TradeName, SupplierId, PartNo );
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( MaterialNodeView, false );
            bool MaterialExists = Tree.getChildNodeCount() > 0;

            if( MaterialExists )
            {
                Tree.goToNthChild( 0 );
                //if( ThrowIfExists )
                //{
                //    string SupplierName = "";
                //    CswNbtNode SupplierNode = _CswNbtResources.Nodes[SupplierId];
                //    if( null != SupplierNode )
                //    {
                //        SupplierName = SupplierNode.NodeName;
                //    }
                //    throw new CswDniException( ErrorType.Error,
                //                              "A material already exists with the with tradename: " + TradeName + ", supplier: " + SupplierName + " and partno: " + PartNo + " properties.",
                //                              "Attempted to call createMaterial with tradename: " + TradeName + ", supplier: " + SupplierName + " and partno: " + PartNo + " properties." );
                //}
                Ret = Tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        private JObject _tryCreateMaterial( Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo, bool ThrowIfExists = true )
        {
            JObject Ret = new JObject();

            CswNbtObjClassMaterial NodeAsMaterial = _getExistingMaterial( MaterialNodeTypeId, SupplierId, TradeName, PartNo, ThrowIfExists );
            bool MaterialExists = ( null != NodeAsMaterial );

            if( false == MaterialExists && Int32.MinValue != MaterialNodeTypeId )
            {
                NodeAsMaterial = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( MaterialNodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp, OverrideUniqueValidation: false );
                if( null != NodeAsMaterial )
                {
                    NodeAsMaterial.TradeName.Text = TradeName;
                    NodeAsMaterial.PartNumber.Text = PartNo;
                    NodeAsMaterial.Supplier.RelatedNodeId = SupplierId;
                    NodeAsMaterial.postChanges( ForceUpdate: false );
                }
            }

            if( null != NodeAsMaterial )
            {
                Ret["materialexists"] = MaterialExists;
                if( false == MaterialExists )
                {
                    Ret["materialid"] = NodeAsMaterial.NodeId.ToString();
                    Ret["tradename"] = NodeAsMaterial.TradeName.Text;
                    Ret["partno"] = NodeAsMaterial.PartNumber.Text;
                    Ret["supplier"] = NodeAsMaterial.Supplier.CachedNodeName;
                    Ret["nodetypeid"] = NodeAsMaterial.NodeTypeId;
                    _CswNbtResources.EditMode = NodeEditMode.Temp;
                    CswNbtSdTabsAndProps SdProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                    Ret["properties"] = SdProps.getProps( NodeAsMaterial.Node, string.Empty, null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
                    Int32 DocumentNodeTypeId = CswNbtActReceiving.getMaterialDocumentNodeTypeId( _CswNbtResources, NodeAsMaterial );
                    if( Int32.MinValue != DocumentNodeTypeId )
                    {
                        Ret["documenttypeid"] = DocumentNodeTypeId;
                    }
                }
                else
                {
                    Ret["noderef"] = NodeAsMaterial.Node.NodeLink; //for the link
                }
            }

            return Ret;
        }

        #endregion private

        #region ctor

        private CswNbtResources _CswNbtResources;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

        /// <summary>
        /// Base Constructor
        /// </summary>
        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor

        #region Public

        /// <summary>
        /// Creates a new material, if one does not already exist, and returns the material nodeid
        /// </summary>
        public JObject createMaterial( Int32 NodeTypeId, string SupplierId, string Tradename, string PartNo )
        {
            return _tryCreateMaterial( NodeTypeId, CswConvert.ToPrimaryKey( SupplierId ), Tradename, PartNo, false );
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

            SizeNode = CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
            CswPrimaryKey UnitIdPK = CswConvert.ToPrimaryKey( SizeObj["unitid"].ToString() );
            if( null != UnitIdPK )
            {
                CswNbtObjClassSize NodeAsSize = (CswNbtObjClassSize) SizeNode;
                NodeAsSize.InitialQuantity.Quantity = CswConvert.ToDouble( SizeObj["quantity"] );
                NodeAsSize.InitialQuantity.UnitId = UnitIdPK;
                NodeAsSize.CatalogNo.Text = SizeObj["catalogNo"].ToString();
                NodeAsSize.QuantityEditable.Checked = CswConvert.ToTristate( SizeObj["quantEditableChecked"] );
                NodeAsSize.Dispensable.Checked = CswConvert.ToTristate( SizeObj["dispensibleChecked"] );
                NodeAsSize.UnitCount.Value = CswConvert.ToInt32( SizeObj["unitCount"] );

                JArray Row = new JArray();
                Ret["row"] = Row;

                Row.Add( "(New Size)" );
                Row.Add( NodeAsSize.InitialQuantity.Gestalt );
                Row.Add( NodeAsSize.Dispensable.Gestalt );
                Row.Add( NodeAsSize.QuantityEditable.Gestalt );
                Row.Add( NodeAsSize.CatalogNo.Gestalt );
                Row.Add( NodeAsSize.UnitCount.Gestalt );

                if( Tristate.False == NodeAsSize.QuantityEditable.Checked && false == CswTools.IsDouble( NodeAsSize.InitialQuantity.Quantity ) )
                {
                    SizeNode = null;//Case 27665 - instead of throwing a serverside warning, just throw out the size
                }
                else if( WriteNode )
                {
                    SizeNode.postChanges( true );
                }
            }
            else
            {
                SizeNode = null;
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
                 MaterialNode.getObjectClass().ObjectClass != CswNbtMetaDataObjectClassName.NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided node was not a valid material.",
                                           "Attempted to call getMaterialSizes with a node that was not valid." );
            }

            CswNbtView SizesView = new CswNbtView( CswNbtResources );  //MaterialNode.getNodeType().CreateDefaultView();
            SizesView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtMetaDataObjectClass SizeOc = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            CswNbtViewRelationship SizeRel = SizesView.AddViewRelationship( SizeOc, false );

            SizesView.AddViewPropertyAndFilter( SizeRel, SizeMaterialOcp, MaterialId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity ) );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable ) );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.QuantityEditable ) );

            CswNbtWebServiceGrid wsG = new CswNbtWebServiceGrid( CswNbtResources, SizesView, false );
            Ret["rows"] = wsG.getThinGridRows( Int32.MinValue, true );

            return Ret;
        }

        private CswNbtNode _commitMaterialNode( JObject MaterialObj )
        {
            CswNbtNode Ret = null;

            JObject MaterialProperties = (JObject) MaterialObj["properties"];
            CswNbtWebServiceTabsAndProps wsTap = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );

            Int32 MaterialNodeTypeId = CswConvert.ToInt32( MaterialObj["materialnodetypeid"] );
            if( Int32.MinValue != MaterialNodeTypeId )
            {
                CswNbtMetaDataNodeType MaterialNt = _CswNbtResources.MetaData.getNodeType( MaterialNodeTypeId );
                if( null != MaterialNt )
                {
                    _CswNbtResources.EditMode = NodeEditMode.Edit;
                    Ret = _CswNbtResources.Nodes[CswConvert.ToString( MaterialObj["materialId"] )];
                    if( null != Ret )
                    {
                        Ret.IsTemp = false;
                        wsTap.saveProps( Ret.NodeId, Int32.MinValue, MaterialProperties.ToString(), Ret.NodeTypeId, null );
                        string Tradename;
                        CswPrimaryKey SupplierId;
                        string PartNo;
                        _getMaterialPropsFromObject( MaterialObj, out Tradename, out SupplierId, out PartNo );

                        CswNbtObjClassMaterial NodeAsMaterial = Ret;
                        NodeAsMaterial.TradeName.Text = Tradename;
                        NodeAsMaterial.Supplier.RelatedNodeId = SupplierId;
                        NodeAsMaterial.PartNumber.Text = PartNo;
                        Ret.postChanges( true );

                        CswNbtActReceiving.commitDocumentNode( _CswNbtResources, NodeAsMaterial, MaterialObj );
                    }
                }

                //if( null == Ret )
                //{
                //    Ret = wsTap.addNode( MaterialNt, MaterialProperties, out MaterialNodeKey );
                //}
                if( null == Ret )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Failed to create new material.",
                                               "Attempted to call _commitMaterialNode failed." );
                }
            }
            return Ret;
        }

        /// <summary>
        /// Finalize the new Material
        /// </summary>
        public JObject commitMaterial( string MaterialDefinition )
        {
            JObject RetObj = new JObject();

            JObject MaterialObj = CswConvert.ToJObject( MaterialDefinition );
            if( MaterialObj.HasValues )
            {
                JArray SizesArray = CswConvert.ToJArray( MaterialObj["sizeNodes"] );
                CswPrimaryKey MaterialId = new CswPrimaryKey();
                MaterialId.FromString( CswConvert.ToString( MaterialObj["materialId"] ) );
                if( CswTools.IsPrimaryKey( MaterialId ) )
                {
                    CswNbtNode MaterialNode = _CswNbtResources.Nodes[MaterialId];
                    if( null != MaterialNode )
                    {
                        CswNbtView MaterialNodeView = _getMaterialNodeView( MaterialNode );

                        /* 1. Validate the new material and get its properties and sizes */

                        MaterialNode = _commitMaterialNode( MaterialObj );
                        RetObj["createdmaterial"] = true;

                        /* 2. Add the sizes */
                        SizesArray = _removeDuplicateSizes( SizesArray );
                        _addMaterialSizes( SizesArray, MaterialNode );
                        RetObj["sizescount"] = SizesArray.Count;

                        /* 3. Add possible secondary actions 
                         * Recieve Material and Request Material workflows don't exist yet.
                         * For now, return a view of the new Node.             
                         */
                        MaterialNodeView.SaveToCache( false );
                        RetObj["nextoptions"] = new JObject();
                        RetObj["nextoptions"]["nodeview"] = MaterialNodeView.SessionViewId.ToString();
                    }
                }
            }
            return RetObj;
        }

        private JArray _removeDuplicateSizes( JArray SizesArray )
        {
            JArray UniqueSizesArray = new JArray();
            foreach( JObject SizeObj in SizesArray )
            {
                bool addSizeToCompare = true;
                if( SizeObj.HasValues )
                {
                    foreach( JObject UniqueSizeObj in UniqueSizesArray )
                    {
                        if( UniqueSizeObj["unitid"].ToString() == SizeObj["unitid"].ToString() &&
                            UniqueSizeObj["quantity"].ToString() == SizeObj["quantity"].ToString() &&
                            UniqueSizeObj["catalogNo"].ToString() == SizeObj["catalogNo"].ToString() )
                        {
                            addSizeToCompare = false;
                        }
                    }
                    if( addSizeToCompare )
                    {
                        UniqueSizesArray.Add( SizeObj );
                    }
                }
            }
            return UniqueSizesArray;
        }

        /// <summary>
        /// Make nodes for defined sizes, else remove undefinable sizes from the JArray
        /// </summary>
        private void _addMaterialSizes( JArray SizesArray, CswNbtNode MaterialNode )
        {
            JArray ArrayToIterate = (JArray) SizesArray.DeepClone();
            foreach( JObject SizeObj in ArrayToIterate )
            {
                if( SizeObj.HasValues )
                {
                    CswNbtNode SizeNode;
                    Int32 SizeNtId = CswConvert.ToInt32( SizeObj["nodetypeid"] );
                    if( Int32.MinValue != SizeNtId )
                    {
                        getSizeNodeProps( _CswNbtResources, _CswNbtStatisticsEvents, SizeNtId, SizeObj, false, out SizeNode );
                        if( null != SizeNode )
                        {
                            CswNbtObjClassSize NodeAsSize = (CswNbtObjClassSize) SizeNode;
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
                }
                else
                {
                    SizesArray.Remove( SizeObj );
                }
            }
        }

        public static JObject getMaterialUnitsOfMeasure( string MaterialId, CswNbtResources CswNbtResources )
        {
            JObject ret = new JObject();
            string PhysicalState = CswNbtObjClassMaterial.PhysicalStates.Solid;
            CswNbtObjClassMaterial Material = CswNbtResources.Nodes[MaterialId];
            if( null != Material &&
                false == string.IsNullOrEmpty( Material.PhysicalState.Value ) )
            {
                PhysicalState = Material.PhysicalState.Value;
            }

            CswNbtUnitViewBuilder unitViewBuilder = new CswNbtUnitViewBuilder( CswNbtResources );

            CswNbtView unitsView = unitViewBuilder.getQuantityUnitOfMeasureView( PhysicalState );

            Collection<CswNbtNode> _UnitNodes = new Collection<CswNbtNode>();
            ICswNbtTree UnitsTree = CswNbtResources.Trees.getTreeFromView( unitsView, false, true, false, false );
            UnitsTree.goToRoot();
            for( int i = 0; i < UnitsTree.getChildNodeCount(); i++ )
            {
                UnitsTree.goToNthChild( i );
                _UnitNodes.Add( UnitsTree.getNodeForCurrentPosition() );
                UnitsTree.goToParentNode();
            }

            foreach( CswNbtNode unitNode in _UnitNodes )
            {
                CswNbtObjClassUnitOfMeasure nodeAsUnitOfMeasure = (CswNbtObjClassUnitOfMeasure) unitNode;
                ret[nodeAsUnitOfMeasure.NodeId.ToString()] = nodeAsUnitOfMeasure.Name.Gestalt;
            }

            return ret;
        }

        public JObject getSizeLogicalsVisibility( int SizeNodeTypeId )
        {
            JObject ret = new JObject();
            ret["showQuantityEditable"] = "false";
            ret["showDispensable"] = "false";
            CswNbtMetaDataNodeType SizeNt = _CswNbtResources.MetaData.getNodeType( SizeNodeTypeId );
            if( null != SizeNt )
            {
                CswNbtMetaDataNodeTypeProp QuantityEditable = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.QuantityEditable );
                if( null != QuantityEditable.AddLayout )
                {
                    ret["showQuantityEditable"] = "true";
                }
                CswNbtMetaDataNodeTypeProp Dispensable = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable );
                if( null != Dispensable.AddLayout )
                {
                    ret["showDispensable"] = "true";
                }
            }
            return ret;
        }

        #endregion Public

    }
}
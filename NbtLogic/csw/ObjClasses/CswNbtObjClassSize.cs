using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSize : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string InitialQuantity = "Initial Quantity";
            public const string QuantityEditable = "Quantity Editable";
            public const string Dispensable = "Dispensable";
            public const string CatalogNo = "Catalog No";
            public const string UnitCount = "Unit Count";
            public const string ContainerType = "Container Type";
            public const string Supplier = "Supplier";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassSize( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassSize
        /// </summary>
        public static implicit operator CswNbtObjClassSize( CswNbtNode Node )
        {
            CswNbtObjClassSize ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.SizeClass ) )
            {
                ret = (CswNbtObjClassSize) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( null != _CswNbtResources.CurrentNbtUser.Cookies && null == Material.RelatedNodeId && _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentnodeid" ) )
            {
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( _CswNbtResources.CurrentNbtUser.Cookies["csw_currentnodeid"] );
                if( null != pk && _isMaterialID( pk ) ) //only assign the id if we got a real nodeid from cookies and it's indeed a material id
                {
                    Material.RelatedNodeId = pk;
                }
            }
            if( CswEnumTristate.False == this.QuantityEditable.Checked && false == CswTools.IsDouble( this.InitialQuantity.Quantity ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot have a null Initial Quantity if Quantity Editable is unchecked.", "Cannot have a null Initial Quantity if Quantity Editable is unchecked." );
            }

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            //case 25759 - set initialQuantity unittype view based on related material physical state
            //Case 29579 - this needs to be done whenever the Size node is referenced (to overwrite the view of the last Size node)
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                Material.setReadOnly( value: true, SaveToDb: true );
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                Vb.setQuantityUnitOfMeasureView( MaterialNode, InitialQuantity );
            }
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropQuantity InitialQuantity { get { return _CswNbtNode.Properties[PropertyName.InitialQuantity]; } }
        public CswNbtNodePropLogical QuantityEditable { get { return _CswNbtNode.Properties[PropertyName.QuantityEditable]; } }
        public CswNbtNodePropLogical Dispensable { get { return _CswNbtNode.Properties[PropertyName.Dispensable]; } }
        public CswNbtNodePropText CatalogNo { get { return _CswNbtNode.Properties[PropertyName.CatalogNo]; } }
        public CswNbtNodePropNumber UnitCount { get { return _CswNbtNode.Properties[PropertyName.UnitCount]; } }
        public CswNbtNodePropList ContainerType { get { return _CswNbtNode.Properties[PropertyName.ContainerType]; } }
        public CswNbtNodePropPropertyReference Supplier { get { return _CswNbtNode.Properties[PropertyName.Supplier]; } }

        #endregion

        #region Custom logic

        private bool _isMaterialID( CswPrimaryKey nodeid )
        {
            bool isMaterialID = false;
            CswNbtNode node = _CswNbtResources.Nodes.GetNode( nodeid );
            if( null != node )
            {
                CswNbtMetaDataPropertySet NodePS = node.getObjectClass().getPropertySet();
                if( null != NodePS )
                {
                    CswNbtMetaDataPropertySet MaterialPS = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
                    isMaterialID = ( MaterialPS.PropertySetId == NodePS.PropertySetId );
                }
            }
            
            return isMaterialID;
        }

        #endregion

    }//CswNbtObjClassSize

}//namespace ChemSW.Nbt.ObjClasses

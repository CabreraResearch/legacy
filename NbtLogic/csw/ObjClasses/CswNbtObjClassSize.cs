using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSize : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Material = "Material";
            public const string InitialQuantity = "Initial Quantity";
            public const string QuantityEditable = "Quantity Editable";
            public const string Dispensable = "Dispensable";
            public const string CatalogNo = "Catalog No";
            public const string UnitCount = "Unit Count";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassSize( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassSize
        /// </summary>
        public static implicit operator CswNbtObjClassSize( CswNbtNode Node )
        {
            CswNbtObjClassSize ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ) )
            {
                ret = (CswNbtObjClassSize) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( null == Material.RelatedNodeId && _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentnodeid" ) )
            {
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( _CswNbtResources.CurrentNbtUser.Cookies["csw_currentnodeid"] );
                if( null != pk && _isMaterialID( pk ) ) //only assign the id if we got a real nodeid from cookies and it's indeed a material id
                {
                    Material.RelatedNodeId = pk;
                }
            }
            if( Tristate.False == this.QuantityEditable.Checked && false == CswTools.IsDouble( this.InitialQuantity.Quantity ) )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot have a null Initial Quantity if Quantity Editable is unchecked.", "Cannot have a null Initial Quantity if Quantity Editable is unchecked." );
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

        public override void afterPopulateProps()
        {
            Material.SetOnPropChange( OnMaterialChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        private void OnMaterialChange( CswNbtNodeProp Prop )
        {
            //case 25759 - set capacity unittype view based on related material physical state
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                Material.setReadOnly( value: true, SaveToDb: true );
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                Vb.setQuantityUnitOfMeasureView( MaterialNode, InitialQuantity );
            }
        }
        public CswNbtNodePropQuantity InitialQuantity { get { return _CswNbtNode.Properties[PropertyName.InitialQuantity]; } }
        public CswNbtNodePropLogical QuantityEditable { get { return _CswNbtNode.Properties[PropertyName.QuantityEditable]; } }
        public CswNbtNodePropLogical Dispensable { get { return _CswNbtNode.Properties[PropertyName.Dispensable]; } }
        public CswNbtNodePropText CatalogNo { get { return _CswNbtNode.Properties[PropertyName.CatalogNo]; } }
        public CswNbtNodePropNumber UnitCount { get { return _CswNbtNode.Properties[PropertyName.UnitCount]; } }

        #endregion

        #region Custom logic

        private bool _isMaterialID( CswPrimaryKey nodeid )
        {
            bool isMaterialID = true;
            CswNbtNode node = _CswNbtResources.Nodes.GetNode( nodeid );
            isMaterialID = ( null != node && CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass == node.getObjectClass().ObjectClass );
            return isMaterialID;
        }

        #endregion

    }//CswNbtObjClassSize

}//namespace ChemSW.Nbt.ObjClasses

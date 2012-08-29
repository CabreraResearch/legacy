using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSize : CswNbtObjClass
    {
        public const string MaterialPropertyName = "Material";
        public const string InitialQuantityPropertyName = "Initial Quantity";
        public const string QuantityEditablePropertyName = "Quantity Editable";
        public const string DispensablePropertyName = "Dispensable";
        public const string CatalogNoPropertyName = "Catalog No";

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
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( null == Material.RelatedNodeId )
            {
                CswPrimaryKey pk = new CswPrimaryKey();
                bool succeeded = true;
                try
                {
                    pk.FromString( _CswNbtResources.CurrentNbtUser.Cookies["csw_currentnodeid"] );
                }
                catch( Exception e )
                {
                    _CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Warning, e ) );
                    succeeded = false;
                }
                if( succeeded && _isMaterialID( pk ) ) //only assign the id if we got a real nodeid from cookies and it's indeed a material id
                {
                    Material.RelatedNodeId = pk;
                }
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

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[MaterialPropertyName]; } }
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
        public CswNbtNodePropQuantity InitialQuantity { get { return _CswNbtNode.Properties[InitialQuantityPropertyName]; } }
        public CswNbtNodePropLogical QuantityEditable { get { return _CswNbtNode.Properties[QuantityEditablePropertyName]; } }
        public CswNbtNodePropLogical Dispensable { get { return _CswNbtNode.Properties[DispensablePropertyName]; } }
        public CswNbtNodePropText CatalogNo { get { return _CswNbtNode.Properties[CatalogNoPropertyName]; } }

        #endregion

        #region Custom logic

        private bool _isMaterialID( CswPrimaryKey nodeid )
        {
            CswNbtNode node = _CswNbtResources.Nodes.GetNode( nodeid );
            return _Validate( node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
        }

        #endregion

    }//CswNbtObjClassSize

}//namespace ChemSW.Nbt.ObjClasses

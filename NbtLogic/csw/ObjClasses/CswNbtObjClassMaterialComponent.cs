using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Batch;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterialComponent : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMaterialComponent( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass ); }
        }

        public const string PercentagePropertyName = "Percentage";
        public const string MixturePropertyName = "Mixture";
        public const string ConstituentPropertyName = "Constituent";

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterialComponent
        /// </summary>
        public static implicit operator CswNbtObjClassMaterialComponent( CswNbtNode Node )
        {
            CswNbtObjClassMaterialComponent ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass ) )
            {
                ret = (CswNbtObjClassMaterialComponent) Node.ObjClass;
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
            Core.CswPrimaryKey pk = new Core.CswPrimaryKey();
            pk.FromString( _CswNbtResources.CurrentUser.Cookies["csw_currentnodeid"] );
            if( null != Constituent.RelatedNodeId && false == IsTemp )
            {
                if( null == Mixture.RelatedNodeId )
                {
                    Mixture.RelatedNodeId = pk;
                }
                if( Constituent.RelatedNodeId.Equals( Mixture.RelatedNodeId ) && false == IsTemp )
                {
                    throw new CswDniException( ErrorType.Warning, "Constituent cannot be the same as Mixture", "" );
                }
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            if( Mixture.WasModified || Constituent.WasModified )
            {
                _recalculateRegListMembership();
            }
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _recalculateRegListMembership();
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            Mixture.SetOnPropChange( OnMixturePropChange );
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

        public CswNbtNodePropNumber Percentage { get { return ( _CswNbtNode.Properties[PercentagePropertyName] ); } }

        public CswNbtNodePropRelationship Mixture { get { return ( _CswNbtNode.Properties[MixturePropertyName] ); } }
        private void OnMixturePropChange( CswNbtNodeProp Prop )
        {
            if( null != Mixture.RelatedNodeId && null != Constituent.RelatedNodeId )
            {
                if( Mixture.RelatedNodeId.Equals( Constituent.RelatedNodeId ) )
                {
                    Mixture.RelatedNodeId = null;
                    Constituent.RelatedNodeId = null;
                }
            }
        }

        public CswNbtNodePropRelationship Constituent { get { return ( _CswNbtNode.Properties[ConstituentPropertyName] ); } }

        #endregion

        #region Custom logic

        /*
         * When a material component is changed in any way, we have to assume this changes the playing field for regulatory list membership
         * This means we have to re-calculate the regulatory list membership for each material and reg list
         */
        private void _recalculateRegListMembership()
        {
            if( false == IsTemp )
            {
                CswCommaDelimitedString parents = new CswCommaDelimitedString();
                CswNbtObjClassMaterial constituentNode = _CswNbtResources.Nodes.GetNode( Constituent.RelatedNodeId );
                constituentNode.getParentMaterials( ref parents );
                parents.Add( Mixture.RelatedNodeId.ToString() );
                CswNbtBatchOpUpdateRegulatoryListsForMaterials BatchOp = new CswNbtBatchOpUpdateRegulatoryListsForMaterials( _CswNbtResources );
                BatchOp.makeBatchOp( parents );
            }
        }

        #endregion

    }//CswNbtObjClassMaterialComponent

}//namespace ChemSW.Nbt.ObjClasses

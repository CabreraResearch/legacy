using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

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
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass ); }
        }

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Percentage = "Percentage";
            public const string Mixture = "Mixture";
            public const string Constituent = "Constituent";
            public const string Active = "Active";
            public const string HazardousReporting = "Hazardous Reporting";
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterialComponent
        /// </summary>
        public static implicit operator CswNbtObjClassMaterialComponent( CswNbtNode Node )
        {
            CswNbtObjClassMaterialComponent ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MaterialComponentClass ) )
            {
                ret = (CswNbtObjClassMaterialComponent) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            if( null != Mixture.RelatedNodeId )
            {
                if( Mixture.RelatedNodeId.Equals( Constituent.RelatedNodeId ) && false == IsTemp )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Constituent cannot be the same as Mixture", "" );
                }
            }
            else if( false == IsTemp )
            {
                throw new CswDniException( CswEnumErrorType.Warning,
                    "Material Components must be added from a Chemical.",
                    "Mixture is a server managed property and in this context no material can be discerned to set as the Mixture." );
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            if( Mixture.WasModified || Constituent.WasModified )
            {
                _recalculateRegListMembership();
            }
            _CswNbtObjClassDefault.afterWriteNode( Creating );
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

        protected override void afterPopulateProps()
        {
            Mixture.SetOnPropChange( OnMixturePropChange );
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

        public CswNbtNodePropNumber Percentage { get { return ( _CswNbtNode.Properties[PropertyName.Percentage] ); } }

        public CswNbtNodePropRelationship Mixture { get { return ( _CswNbtNode.Properties[PropertyName.Mixture] ); } }
        private void OnMixturePropChange( CswNbtNodeProp Prop )
        {
            if( null != Mixture.RelatedNodeId )
            {
                Mixture.setReadOnly( true, true );
                if( null != Constituent.RelatedNodeId )
                {
                    if( Mixture.RelatedNodeId.Equals( Constituent.RelatedNodeId ) )
                    {
                        Constituent.RelatedNodeId = null;
                    }
                }
            }
        }

        public CswNbtNodePropRelationship Constituent { get { return ( _CswNbtNode.Properties[PropertyName.Constituent] ); } }
        public CswNbtNodePropLogical Active { get { return ( _CswNbtNode.Properties[PropertyName.Active] ); } }
        public CswNbtNodePropLogical HazardousReporting { get { return ( _CswNbtNode.Properties[PropertyName.HazardousReporting] ); } }

        #endregion

        #region Custom logic

        /*
         * When a material component is changed in any way, we have to assume this changes the playing field for regulatory list membership
         * This means we have to re-calculate the regulatory list membership for the mixture material
         */

        private void _recalculateRegListMembership()
        {
            if( false == IsTemp && null != Mixture.RelatedNodeId )
            {
                CswNbtObjClassChemical mixtureNode = _CswNbtResources.Nodes.GetNode( Mixture.RelatedNodeId );
                mixtureNode.RefreshRegulatoryListMembers();
            }
        }

        #endregion

    }//CswNbtObjClassMaterialComponent

}//namespace ChemSW.Nbt.ObjClasses

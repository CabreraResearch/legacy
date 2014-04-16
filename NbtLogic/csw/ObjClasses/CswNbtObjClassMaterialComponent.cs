using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterialComponent : CswNbtObjClass
    {
        public CswNbtObjClassMaterialComponent( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass ); }
        }

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string PercentageRange = "Percentage Range";
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

        protected override void beforePromoteNodeLogic( bool OverrideUniqueValidation = false )
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
        }

        protected override void afterWriteNodeLogic()
        {
            if( Mixture.wasAnySubFieldModified() || Constituent.wasAnySubFieldModified() )
            {
                _recalculateRegListMembership();
            }
        }

        protected override void afterDeleteNodeLogic()
        {
            _recalculateRegListMembership();
        }

        protected override void afterPopulateProps()
        {
            Mixture.SetOnPropChange( OnMixturePropChange );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Mixture { get { return ( _CswNbtNode.Properties[PropertyName.Mixture] ); } }
        private void OnMixturePropChange( CswNbtNodeProp Prop, bool Creating )
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
        
        public CswNbtNodePropNumericRange PercentageRange { get { return ( _CswNbtNode.Properties[PropertyName.PercentageRange] ); } }
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

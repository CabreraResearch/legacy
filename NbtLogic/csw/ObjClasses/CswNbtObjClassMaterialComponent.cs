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
            public const string Percentage = "Percentage";
            public const string LowPercentageValue = "Low % Value";
            public const string TargetPercentageValue = "Target % Value";
            public const string HighPercentageValue = "High % Value";
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
            _setPercentage();
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
            LowPercentageValue.SetOnPropChange( _onLowPercentageValuePropChange );
            TargetPercentageValue.SetOnPropChange( _onTargetPercentageValuePropChange );
            HighPercentageValue.SetOnPropChange( _onHighPercentageValuePropChange );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropNumber Percentage { get { return ( _CswNbtNode.Properties[PropertyName.Percentage] ); } }
        public CswNbtNodePropNumber LowPercentageValue { get { return ( _CswNbtNode.Properties[PropertyName.LowPercentageValue] ); } }
        private void _onLowPercentageValuePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( CswTools.IsDouble( TargetPercentageValue.Value ) && LowPercentageValue.Value > TargetPercentageValue.Value )
            {
                throw new CswDniException( CswEnumErrorType.Warning, PropertyName.LowPercentageValue + " cannot be higher than " + PropertyName.TargetPercentageValue, "" );
            }
            if( CswTools.IsDouble( HighPercentageValue.Value ) && LowPercentageValue.Value > HighPercentageValue.Value )
            {
                throw new CswDniException( CswEnumErrorType.Warning, PropertyName.LowPercentageValue + " cannot be higher than " + PropertyName.HighPercentageValue, "" );
            }
            _setPercentage();
        }
        public CswNbtNodePropNumber TargetPercentageValue { get { return ( _CswNbtNode.Properties[PropertyName.TargetPercentageValue] ); } }
        private void _onTargetPercentageValuePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( CswTools.IsDouble( HighPercentageValue.Value ) && TargetPercentageValue.Value > HighPercentageValue.Value )
            {
                throw new CswDniException( CswEnumErrorType.Warning, PropertyName.TargetPercentageValue + " cannot be higher than " + PropertyName.HighPercentageValue, "" );
            }
            if( CswTools.IsDouble( LowPercentageValue.Value ) && TargetPercentageValue.Value < LowPercentageValue.Value )
            {
                throw new CswDniException( CswEnumErrorType.Warning, PropertyName.TargetPercentageValue + " cannot be lower than " + PropertyName.LowPercentageValue, "" );
            }
            _setPercentage();
        }
        public CswNbtNodePropNumber HighPercentageValue { get { return ( _CswNbtNode.Properties[PropertyName.HighPercentageValue] ); } }
        private void _onHighPercentageValuePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( CswTools.IsDouble( LowPercentageValue.Value ) && HighPercentageValue.Value < LowPercentageValue.Value )
            {
                throw new CswDniException( CswEnumErrorType.Warning, PropertyName.HighPercentageValue + " cannot be lower than " + PropertyName.LowPercentageValue, "" );
            }
            if( CswTools.IsDouble( TargetPercentageValue.Value ) && HighPercentageValue.Value < TargetPercentageValue.Value )
            {
                throw new CswDniException( CswEnumErrorType.Warning, PropertyName.HighPercentageValue + " cannot be lower than " + PropertyName.TargetPercentageValue, "" );
            }
            _setPercentage();
        }
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

        private void _setPercentage()
        {
            double Percent = 0;
            Percent = CswTools.IsDouble( LowPercentageValue.Value ) ? LowPercentageValue.Value : Percent;
            Percent = CswTools.IsDouble( TargetPercentageValue.Value ) ? TargetPercentageValue.Value : Percent;
            Percent = CswTools.IsDouble( HighPercentageValue.Value ) ? HighPercentageValue.Value : Percent;
            Percentage.Value = Percent;
        }

        #endregion

    }//CswNbtObjClassMaterialComponent

}//namespace ChemSW.Nbt.ObjClasses

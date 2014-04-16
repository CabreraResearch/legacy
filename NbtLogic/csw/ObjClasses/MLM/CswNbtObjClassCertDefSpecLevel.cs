using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCertDefSpecLevel: CswNbtObjClass
    {
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Level = "Level";
            public const string CertDefSpec = "Cert Def Spec";
            public const string AllowedDataSources = "Allowed Data Sources";
            public const string RequiredForApproval = "Required For Approval";
            public const string InitialSampleRegime = "Initial Sample Regime";
            public const string RetestSampleRegime = "Retest Sample Regime";
            public const string SampleSize = "Sample Size";
            public const string Frequency = "Frequency";
            public const string ApprovalPeriod = "Approval Period";
            public const string SampleSizeNumber = "Sample Size Number";
        }

        public CswNbtObjClassCertDefSpecLevel( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecLevelClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassLevel
        /// </summary>
        public static implicit operator CswNbtObjClassCertDefSpecLevel( CswNbtNode Node )
        {
            CswNbtObjClassCertDefSpecLevel ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CertDefSpecLevelClass ) )
            {
                ret = (CswNbtObjClassCertDefSpecLevel) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void afterPopulateProps()
        {
            RetestSampleRegime.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    CswNbtObjClassLevel LevelNode = _CswNbtResources.Nodes.GetNode( Level.RelatedNodeId );
                    if( null != LevelNode && LevelNode.LabUseOnly.Checked == CswEnumTristate.True ) //On Add, LevelNode will be null
                    {
                        Prop.setHidden( true, false );
                    }
                } );
        }

        protected override void afterPromoteNodeLogic()
        {
            CswNbtObjClassCertDefSpec RelatedCertDefSpec = _CswNbtResources.Nodes.GetNode( CertDefSpec.RelatedNodeId );
            CswNbtObjClassCertificateDefinition RelatedCertDef = _CswNbtResources.Nodes.GetNode( RelatedCertDefSpec.CertDef.RelatedNodeId );
            CswNbtNode RelatedPart = _CswNbtResources.Nodes.GetNode( RelatedCertDef.Material.RelatedNodeId );

            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp NameOCP = UnitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
            CswNbtView MonthsUnitView = new CswNbtView( _CswNbtResources );
            MonthsUnitView.saveNew( "UnitsViewForCertDefSpecApprovvalPeriodProp", CswEnumNbtViewVisibility.Property );
            CswNbtViewRelationship parent = MonthsUnitView.AddViewRelationship( UnitOfMeasureOC, true );
            if( CswEnumNbtObjectClass.ChemicalClass == RelatedPart.getObjectClass().ObjectClass )
            {
                MonthsUnitView.AddViewPropertyAndFilter( parent, NameOCP, "g" );
            }
            else if( CswEnumNbtObjectClass.EnterprisePartClass == RelatedPart.getObjectClass().ObjectClass )
            {
                MonthsUnitView.AddViewPropertyAndFilter( parent, NameOCP, "Each" );
            }
            MonthsUnitView.save();

            ICswNbtTree unitsTree = _CswNbtResources.Trees.getTreeFromView( MonthsUnitView, false, false, false );
            if( false == SampleSizeNumber.Empty && unitsTree.getChildNodeCount() > 0 )
            {
                unitsTree.goToNthChild( 0 );
                SampleSize.Quantity = SampleSizeNumber.Value;
                SampleSize.UnitId = unitsTree.getNodeIdForCurrentPosition();
                SampleSize.View = MonthsUnitView;
            }

            SampleSizeNumber.setHidden( true, true );

            postChanges( false );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Level { get { return _CswNbtNode.Properties[PropertyName.Level]; } }
        public CswNbtNodePropRelationship CertDefSpec { get { return _CswNbtNode.Properties[PropertyName.CertDefSpec]; } }
        public CswNbtNodePropList AllowedDataSources { get { return _CswNbtNode.Properties[PropertyName.AllowedDataSources]; } }
        public CswNbtNodePropLogical RequiredForApproval { get { return _CswNbtNode.Properties[PropertyName.RequiredForApproval]; } }
        public CswNbtNodePropList InitialSampleRegime { get { return _CswNbtNode.Properties[PropertyName.InitialSampleRegime]; } }
        public CswNbtNodePropList RetestSampleRegime { get { return _CswNbtNode.Properties[PropertyName.RetestSampleRegime]; } }
        public CswNbtNodePropQuantity SampleSize { get { return _CswNbtNode.Properties[PropertyName.SampleSize]; } }
        public CswNbtNodePropList Frequency { get { return _CswNbtNode.Properties[PropertyName.Frequency]; } }
        public CswNbtNodePropQuantity ApprovalPeriod { get { return _CswNbtNode.Properties[PropertyName.ApprovalPeriod]; } }
        public CswNbtNodePropNumber SampleSizeNumber { get { return _CswNbtNode.Properties[PropertyName.SampleSizeNumber]; } }

        #endregion


    }//CswNbtObjClassCertDefSpecLevel

}//namespace ChemSW.Nbt.ObjClasses

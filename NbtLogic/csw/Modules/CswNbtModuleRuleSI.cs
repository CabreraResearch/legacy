using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the SI Module
    /// </summary>
    public class CswNbtModuleRuleSI : CswNbtModuleRule
    {
        public CswNbtModuleRuleSI( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }

        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.SI; } }

        protected override void OnEnable()
        {
            //case 26717 - show the following...
            //   Inspector Manager role and user
            //   Inspector role and user
            //   All views in categories Lab Safety, Lab Safety (Demo) and Inspections
            _CswNbtResources.Modules.ToggleRoleNodes( false, "inspector" );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "inspection manager" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "inspector" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "inspectmgr" );
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Lab Safety (demo)", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Inspections", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Lab Safety", CswEnumNbtViewVisibility.Global );

            CswNbtMetaDataObjectClass reportOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp reportNameOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.ReportName );
            CswNbtNode deficientInspectionsNode = _findNode( "Deficient Inspections", reportOC, reportNameOCP );
            if( null != deficientInspectionsNode )
            {
                deficientInspectionsNode.Hidden = false;
                deficientInspectionsNode.postChanges( false );
            }

            //Case 28117 - show Future Scheduling
            _CswNbtResources.Modules.ToggleAction( true, CswEnumNbtActionName.Future_Scheduling );

            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.UpdtInspection, Disabled: false );

            //Mail Reports
            _CswNbtResources.Modules.ToggleNode( false, "Lab 1 Deficiencies Mail Report", CswEnumNbtObjectClass.MailReportClass );
            _CswNbtResources.Modules.ToggleNode( false, "Lab Safety Checklist (demo) Action Required Notification", CswEnumNbtObjectClass.MailReportClass );
        }

        protected override void OnDisable()
        {
            //case 26717 - hide the following...
            //   Inspector Manager role and user
            //   Inspector role and user
            //   All views in categories Lab Safety, Lab Safety (Demo) and Inspections
            _CswNbtResources.Modules.ToggleRoleNodes( true, "inspector" );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "inspection manager" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "inspector" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "inspectmgr" );
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Lab Safety (demo)", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Inspections", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Lab Safety", CswEnumNbtViewVisibility.Global );


            CswNbtMetaDataObjectClass reportOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp reportNameOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.ReportName );
            CswNbtNode deficientInspectionsNode = _findNode( "Deficient Inspections", reportOC, reportNameOCP );
            if( null != deficientInspectionsNode )
            {
                deficientInspectionsNode.Hidden = true;
                deficientInspectionsNode.postChanges( false );
            }

            //Case 28117 - hide Future Scheduling
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SI ) && false   == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
            {
                _CswNbtResources.Modules.ToggleAction( false, CswEnumNbtActionName.Future_Scheduling );
            }

            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.UpdtInspection, Disabled: true );

            //Mail Reports
            _CswNbtResources.Modules.ToggleNode( true, "Lab 1 Deficiencies Mail Report", CswEnumNbtObjectClass.MailReportClass );
            _CswNbtResources.Modules.ToggleNode( true, "Lab Safety Checklist (demo) Action Required Notification", CswEnumNbtObjectClass.MailReportClass );
        }

        private CswNbtNode _findNode( string value, CswNbtMetaDataObjectClass objClass, CswNbtMetaDataObjectClassProp objClassProp )
        {
            CswNbtView view = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = view.AddViewRelationship( objClass, false );
            view.AddViewPropertyAndFilter( parent,
                MetaDataProp: objClassProp,
                Value: value,
                SubFieldName: CswNbtFieldTypeRuleText.SubFieldName.Text,
                FilterMode: CswEnumNbtFilterMode.Equals );

            ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( view, false, false, true );
            CswNbtNode node = null;
            for( int i = 0; i < tree.getChildNodeCount(); i++ )
            {
                tree.goToNthChild( i );
                node = tree.getNodeForCurrentPosition();
                tree.goToParentNode();
            }
            return node;
        }

    } // class CswNbtModuleSI
}// namespace ChemSW.Nbt

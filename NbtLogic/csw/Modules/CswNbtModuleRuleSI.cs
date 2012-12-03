using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;

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

        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.SI; } }

        public override void OnEnable()
        {
            //case 26717 - show the following...
            //   Inspector Manager role and user
            //   Inspector role and user
            //   All views in categories Lab Safety, Lab Safety (Demo) and Inspections
            _CswNbtResources.Modules.ToggleRoleNodes( false, "inspector" );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "inspection manager" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "inspector" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "inspectmgr" );
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Lab Safety (demo)", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Inspections", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Lab Safety", NbtViewVisibility.Global );

            CswNbtMetaDataObjectClass reportOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp reportNameOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.ReportName );
            CswNbtNode deficientInspectionsNode = _findNode( "Deficient Inspections", reportOC, reportNameOCP );
            if( null != deficientInspectionsNode )
            {
                deficientInspectionsNode.Hidden = false;
                deficientInspectionsNode.postChanges( false );
            }

            //Case 28117 - show Future Scheduling
            _CswNbtResources.Modules.ToggleAction( true, CswNbtActionName.Future_Scheduling );
        }

        public override void OnDisable()
        {
            //case 26717 - hide the following...
            //   Inspector Manager role and user
            //   Inspector role and user
            //   All views in categories Lab Safety, Lab Safety (Demo) and Inspections
            _CswNbtResources.Modules.ToggleRoleNodes( true, "inspector" );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "inspection manager" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "inspector" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "inspectmgr" );
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Lab Safety (demo)", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Inspections", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Lab Safety", NbtViewVisibility.Global );


            CswNbtMetaDataObjectClass reportOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp reportNameOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.ReportName );
            CswNbtNode deficientInspectionsNode = _findNode( "Deficient Inspections", reportOC, reportNameOCP );
            if( null != deficientInspectionsNode )
            {
                deficientInspectionsNode.Hidden = true;
                deficientInspectionsNode.postChanges( false );
            }

            //Case 28117 - hide Future Scheduling
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SI ) && false   == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
            {
                _CswNbtResources.Modules.ToggleAction( false, CswNbtActionName.Future_Scheduling );
            }
        }

        private CswNbtNode _findNode( string value, CswNbtMetaDataObjectClass objClass, CswNbtMetaDataObjectClassProp objClassProp )
        {
            CswNbtView view = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = view.AddViewRelationship( objClass, false );
            view.AddViewPropertyAndFilter( parent,
                MetaDataProp: objClassProp,
                Value: value,
                SubFieldName: CswNbtSubField.SubFieldName.Text,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

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

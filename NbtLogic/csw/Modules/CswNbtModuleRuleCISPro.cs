using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Data;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CISPro Module
    /// </summary>
    public class CswNbtModuleRuleCISPro : CswNbtModuleRule
    {
        public CswNbtModuleRuleCISPro( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.CISPro; } }
        public override void OnEnable()
        {
            // When CISPro is enabled, display the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   User.WorkUnit
            //   User.Jurisdiction
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatability );
                //LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp LocationInvLevelsNTP = LocationNT.getNodeTypeProp( "Inventory Levels" );
                if( null != LocationInvLevelsNTP )
                {
                    CswNbtMetaDataNodeTypeTab LocationInvLevelsTab = LocationNT.getNodeTypeTab( "Inventory Levels" );
                    if( LocationInvLevelsTab == null )
                    {
                        LocationInvLevelsTab = _CswNbtResources.MetaData.makeNewTab( LocationNT, "Inventory Levels", 100 );
                    }
                    LocationInvLevelsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationInvLevelsTab.TabId );
                }
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserWorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, UserNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                UserJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                UserJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, UserNT.getFirstNodeTypeTab().TabId );
            }

            //Case 27862 - show...
            //   All CISPro roles and users
            //   Unit of measure and work units views
            _toggleCISPRoUsers( false );
            _toggleCISPRoRoles( false );
            _toggleView( false, "Work Units" );
            _toggleView( false, "Units of Measurement" );

        }

        public override void OnDisable()
        {
            // case 26717 - When CISPro is disabled, hide the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   User.WorkUnit
            //   Location.Inventory Levels
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                LocationInvGrpNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatability );
                LocationStorCompatNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp LocationInvLevelsNTP = LocationNT.getNodeTypeProp( "Inventory Levels" );
                if( null != LocationInvLevelsNTP )
                {
                    LocationInvLevelsNTP.removeFromAllLayouts();
                    CswNbtMetaDataNodeTypeTab LocationInvLevelsTab = LocationNT.getNodeTypeTab( "Inventory Levels" );
                    if( LocationInvLevelsTab != null )
                    {
                        _CswNbtResources.MetaData.DeleteNodeTypeTab( LocationInvLevelsTab );
                    }
                }
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserWorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                UserWorkUnitNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                UserJurisdictionNTP.removeFromAllLayouts();
            }

            //Case 27862 - hide...
            //   All CISPro roles and users
            //   Unit of measure and work units views
            _toggleCISPRoUsers( true );
            _toggleCISPRoRoles( true );
            _toggleView( true, "Work Units" );
            _toggleView( true, "Units of Measurement" );


        } // OnDisable()

        private void _toggleCISPRoUsers( bool hidden )
        {
            CswNbtMetaDataObjectClass userOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp usernameOCP = userOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Username );
            CswNbtView cispro_usersView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = cispro_usersView.AddViewRelationship( userOC, false );
            cispro_usersView.AddViewPropertyAndFilter( parent,
                MetaDataProp: usernameOCP,
                Value: "cispro",
                SubFieldName: CswNbtSubField.SubFieldName.Text,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

            ICswNbtTree cisproUsersTree = _CswNbtResources.Trees.getTreeFromView( cispro_usersView, false, true, true );
            int count = cisproUsersTree.getChildNodeCount();
            for( int i = 0; i < count; i++ )
            {
                cisproUsersTree.goToNthChild( i );
                CswNbtNode userNode = cisproUsersTree.getNodeForCurrentPosition();
                userNode.Hidden = hidden;
                userNode.postChanges( false );
                cisproUsersTree.goToParentNode();
            }
        }

        private void _toggleCISPRoRoles( bool hidden )
        {
            CswNbtMetaDataObjectClass roleOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp nameOCP = roleOC.getObjectClassProp( CswNbtObjClassRole.PropertyName.Name );
            CswNbtView cispro_rolesView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = cispro_rolesView.AddViewRelationship( roleOC, false );
            cispro_rolesView.AddViewPropertyAndFilter( parent,
                MetaDataProp: nameOCP,
                Value: "cispro",
                SubFieldName: CswNbtSubField.SubFieldName.Text,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

            ICswNbtTree cisproUsersTree = _CswNbtResources.Trees.getTreeFromView( cispro_rolesView, false, true, true );
            int count = cisproUsersTree.getChildNodeCount();
            for( int i = 0; i < count; i++ )
            {
                cisproUsersTree.goToNthChild( i );
                CswNbtNode userNode = cisproUsersTree.getNodeForCurrentPosition();
                userNode.Hidden = hidden;
                userNode.postChanges( false );
                cisproUsersTree.goToParentNode();
            }
        }

        private void _toggleView( bool hidden, string viewName )
        {
            DataTable viewDT = _CswNbtResources.ViewSelect.getView( viewName, NbtViewVisibility.Global, null, null );
            if( viewDT.Rows.Count == 1 )
            {
                CswNbtView view = _CswNbtResources.ViewSelect.restoreView( viewDT.Rows[0]["viewxml"].ToString() );
                if( null != view )
                {
                    view.SetVisibility( NbtViewVisibility.Hidden, null, null );
                    view.save();
                }
            }
        }

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt

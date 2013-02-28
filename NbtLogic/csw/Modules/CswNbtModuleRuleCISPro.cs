using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

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
            //   Location.Containers Grid
            //   User.WorkUnit
            //   User.Jurisdiction
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp LocationControlZoneNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.ControlZone );
                LocationControlZoneNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationControlZoneNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatibility );
                //LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );
                LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, false );

                CswNbtMetaDataNodeTypeProp LocationContainersGridNTP = LocationNT.getNodeTypeProp( "Containers" );
                if( null != LocationContainersGridNTP )
                {
                    CswNbtMetaDataNodeTypeTab LocationContainersTab = LocationNT.getNodeTypeTab( "Containers" ) ?? _CswNbtResources.MetaData.makeNewTab( LocationNT, "Containers", 99 );
                    LocationContainersGridNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationContainersTab.TabId );
                }

                CswNbtMetaDataNodeTypeProp LocationInvLevelsNTP = LocationNT.getNodeTypeProp( "Inventory Levels" );
                if( null != LocationInvLevelsNTP )
                {
                    CswNbtMetaDataNodeTypeTab LocationInvLevelsTab = LocationNT.getNodeTypeTab( "Inventory Levels" ) ?? _CswNbtResources.MetaData.makeNewTab( LocationNT, "Inventory Levels", 100 );
                    LocationInvLevelsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationInvLevelsTab.TabId );
                }
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserWorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                if( null != UserWorkUnitNTP )
                {
                    UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                    UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, UserNT.getFirstNodeTypeTab().TabId );
                }

                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                if( null != UserJurisdictionNTP )
                {
                    UserJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                    UserJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, UserNT.getFirstNodeTypeTab().TabId );
                }
            }

            //Case 27862 - show...
            //   All CISPro roles and users
            //   Unit of measure and work units views
            //_CswNbtResources.Modules.ToggleRoleNodes()
            _CswNbtResources.Modules.ToggleRoleNodes( false, "cispro" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "cispro" );
            _CswNbtResources.Modules.ToggleView( false, "Units of Measurement", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Work Units", NbtViewVisibility.Global );

            //Case 28117 - show views...
            //   Expiring Containers
            //   Missing Containers
            //   Above Maximum Inventory
            //   Below Minimum Inventory
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Containers", NbtViewVisibility.Global );

            // Case 28930 - Enable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( NbtScheduleRuleNames.GenRequest, Disabled: false );

            //Case 28933 show report nodes
            _CswNbtResources.Modules.ToggleReportNodes( "Containers", false );
        }

        public override void OnDisable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.MLM );
            }

            // The C3 module can only be enabled if the CISPro module is enabled.
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.C3 ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.C3 );
            }

            // case 26717 - When CISPro is disabled, hide the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   Location.Containers Grid
            //   User.WorkUnit
            //   Location.Inventory Levels
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                LocationInvGrpNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp LocationControlZoneNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.ControlZone );
                LocationControlZoneNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatibility );
                LocationStorCompatNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp LocationContainersGridNTP = LocationNT.getNodeTypeProp( "Containers" );
                if( null != LocationContainersGridNTP )
                {
                    LocationContainersGridNTP.removeFromAllLayouts();
                    CswNbtMetaDataNodeTypeTab LocationContainersTab = LocationNT.getNodeTypeTab( "Containers" );
                    if( LocationContainersTab != null )
                    {
                        _CswNbtResources.MetaData.DeleteNodeTypeTab( LocationContainersTab );
                    }
                }

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
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
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
            _CswNbtResources.Modules.ToggleRoleNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleView( true, "Units of Measurement", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Work Units", NbtViewVisibility.Global );

            //Case 28117 - hide views...
            //   Expiring Containers
            //   Missing Containers
            //   Above Maximum Inventory
            //   Below Minimum Inventory
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Containers", NbtViewVisibility.Global );

            // Case 28930 - Enable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( NbtScheduleRuleNames.GenRequest, Disabled: true );

            //Case 28933 hide report nodes
            _CswNbtResources.Modules.ToggleReportNodes( "Containers", true );
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt

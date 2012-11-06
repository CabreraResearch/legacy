using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
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
            _CswNbtResources.Modules.ToggleView( false, "Units of Measurement" );
            _CswNbtResources.Modules.ToggleView( false, "Work Units" );
        }

        public override void OnDisable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.MLM );
            }
            // case 26717 - When CISPro is disabled, hide the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   User.WorkUnit
            //   Location.Inventory Levels
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
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
            _CswNbtResources.Modules.ToggleView( true, "Units of Measurement" );
            _CswNbtResources.Modules.ToggleView( true, "Work Units" );

        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt

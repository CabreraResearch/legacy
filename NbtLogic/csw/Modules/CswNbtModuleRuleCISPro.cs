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
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
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

            // Case 28930 - Enable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( NbtScheduleRuleNames.GenRequest, Disabled: false );

        }

        public override void OnDisable()
        {
            //Disable dependent modules
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.Containers );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.C3 ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.C3 );
            }

            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                UserJurisdictionNTP.removeFromAllLayouts();
            }

            //Case 27862 - hide...
            //   All CISPro roles and users
            //   Unit of measure and work units views
            _CswNbtResources.Modules.ToggleRoleNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleView( true, "Units of Measurement", NbtViewVisibility.Global );
            // Case 28930 - Enable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( NbtScheduleRuleNames.GenRequest, Disabled: true );

        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt

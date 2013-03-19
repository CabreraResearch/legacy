using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the IMCS Module
    /// </summary>
    public class CswNbtModuleRuleIMCS : CswNbtModuleRule
    {
        public CswNbtModuleRuleIMCS( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }

        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.IMCS; } }

        public override void OnEnable()
        {
            //Case 27862 show the following...
            //   Equipment* views
            //   Equipment manager, user and techician roles and users
            _CswNbtResources.Modules.ToggleView( false, "All Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Equipment By Assembly", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Equipment By Location", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Equipment List", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Find Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "My Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Retired Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "technician" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "equipmgr" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "equipuser" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "technician" );

            //Case 28117 - show Future Scheduling
            _CswNbtResources.Modules.ToggleAction( true, CswNbtActionName.Future_Scheduling );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( true, CswNbtActionName.KioskMode );
            _CswNbtResources.Actions[CswNbtActionName.KioskMode].SetCategory( "Equipment" );
        }

        public override void OnDisable()
        {
            //Case 27862 hide the following...
            //   Equipment* views
            //   Equipment manager, user and techician roles and users
            _CswNbtResources.Modules.ToggleView( true, "All Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Equipment By Assembly", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Equipment By Location", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Equipment List", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Find Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "My Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Retired Equipment", NbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "technician" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "equipmgr" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "equipuser" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "technician" );

            //Case 28117 - hide Future Scheduling
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SI ) && false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
            {
                _CswNbtResources.Modules.ToggleAction( false, CswNbtActionName.Future_Scheduling );
            }

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                _CswNbtResources.Modules.ToggleAction( false, CswNbtActionName.KioskMode );
                _CswNbtResources.Actions[CswNbtActionName.KioskMode].SetCategory( "Containers" );
            }
        }

    } // class CswNbtModuleIMCS
}// namespace ChemSW.Nbt

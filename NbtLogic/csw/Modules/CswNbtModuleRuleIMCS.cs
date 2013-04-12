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

        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.IMCS; } }

        public override void OnEnable()
        {
            //Case 27862 show the following...
            //   Equipment* views
            //   Equipment manager, user and techician roles and users
            _CswNbtResources.Modules.ToggleView( false, "All Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Equipment By Assembly", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Equipment By Location", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Equipment List", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Find Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "My Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( false, "Retired Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "technician" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "equipmgr" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "equipuser" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "technician" );

            //Case 28117 - show Future Scheduling
            _CswNbtResources.Modules.ToggleAction( true, CswEnumNbtActionName.Future_Scheduling );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( true, CswEnumNbtActionName.Kiosk_Mode );
            _CswNbtResources.Actions[CswEnumNbtActionName.Kiosk_Mode].SetCategory( "Equipment" );
        }

        public override void OnDisable()
        {
            //Case 27862 hide the following...
            //   Equipment* views
            //   Equipment manager, user and techician roles and users
            _CswNbtResources.Modules.ToggleView( true, "All Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Equipment By Assembly", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Equipment By Location", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Equipment List", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Find Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "My Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleView( true, "Retired Equipment", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "technician" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "equipmgr" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "equipuser" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "technician" );

            //Case 28117 - hide Future Scheduling
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SI ) && false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
            {
                _CswNbtResources.Modules.ToggleAction( false, CswEnumNbtActionName.Future_Scheduling );
            }

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                _CswNbtResources.Modules.ToggleAction( false, CswEnumNbtActionName.Kiosk_Mode );
            }
            else
            {
                _CswNbtResources.Actions[CswEnumNbtActionName.Kiosk_Mode].SetCategory( "Containers" );
            }
        }

    } // class CswNbtModuleIMCS
}// namespace ChemSW.Nbt

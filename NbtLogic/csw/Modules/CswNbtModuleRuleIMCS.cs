
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
            _CswNbtResources.Modules.ToggleView( false, "All Equipment" );
            _CswNbtResources.Modules.ToggleView( false, "Equipment By Assembly" );
            _CswNbtResources.Modules.ToggleView( false, "Equipment By Location" );
            _CswNbtResources.Modules.ToggleView( false, "Equipment List" );
            _CswNbtResources.Modules.ToggleView( false, "Find Equipment" );
            _CswNbtResources.Modules.ToggleView( false, "My Equipment" );
            _CswNbtResources.Modules.ToggleView( false, "Retired Equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( false, "technician" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "equipmgr" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "equipuser" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "technician" );
        }

        public override void OnDisable()
        {
            //Case 27862 hide the following...
            //   Equipment* views
            //   Equipment manager, user and techician roles and users
            _CswNbtResources.Modules.ToggleView( true, "All Equipment" );
            _CswNbtResources.Modules.ToggleView( true, "Equipment By Assembly" );
            _CswNbtResources.Modules.ToggleView( true, "Equipment By Location" );
            _CswNbtResources.Modules.ToggleView( true, "Equipment List" );
            _CswNbtResources.Modules.ToggleView( true, "Find Equipment" );
            _CswNbtResources.Modules.ToggleView( true, "My Equipment" );
            _CswNbtResources.Modules.ToggleView( true, "Retired Equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "equipment" );
            _CswNbtResources.Modules.ToggleRoleNodes( true, "technician" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "equipmgr" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "equipuser" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "technician" );



        }

    } // class CswNbtModuleIMCS
}// namespace ChemSW.Nbt

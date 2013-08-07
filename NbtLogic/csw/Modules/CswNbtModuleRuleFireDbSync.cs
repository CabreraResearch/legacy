
using ChemSW.Config;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the FireDb Sync Module
    /// </summary>
    public class CswNbtModuleRuleFireDbSync : CswNbtModuleRule
    {
        public CswNbtModuleRuleFireDbSync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.FireDbSync; } }
        protected override void OnEnable()
        {

            // Clear the C3SyncDate configuration variable
            _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConvert.ToString( CswEnumConfigurationVariableNames.C3SyncDate ), string.Empty );

        }// OnEnabled

        protected override void OnDisable()
        {
        } // OnDisable()

    } // class CswNbtModuleFireDbSync
}// namespace ChemSW.Nbt

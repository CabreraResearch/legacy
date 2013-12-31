using ChemSW.Config;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Ariel Sync Module
    /// </summary>
    public class CswNbtModuleRuleArielSync : CswNbtModuleRule
    {
        public CswNbtModuleRuleArielSync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.ArielSync; } }
        protected override void OnEnable()
        {
            // Clear the C3SyncDate configuration variable
            _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConvert.ToString( CswEnumConfigurationVariableNames.C3SyncDate ), string.Empty );

        }// OnEnable()

        protected override void OnDisable()
        {
        } // OnDisable()

    } // class CswNbtModuleArielSync
}// namespace ChemSW.Nbt

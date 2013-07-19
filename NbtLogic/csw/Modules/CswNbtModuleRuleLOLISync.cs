using ChemSW.Config;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the LOLI Sync Module
    /// </summary>
    public class CswNbtModuleRuleLOLISync : CswNbtModuleRule
    {
        public CswNbtModuleRuleLOLISync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.LOLISync; } }
        protected override void OnEnable()
        {
            // Clear the C3SyncDate configuration variable
            _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConvert.ToString( CswEnumConfigurationVariableNames.C3SyncDate ), string.Empty );

        }// OnEnable()

        protected override void OnDisable()
        {
        } // OnDisable()

    } // class CswNbtModuleLOLISync
}// namespace ChemSW.Nbt

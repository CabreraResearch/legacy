
using ChemSW.Config;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the PCID Sync Module
    /// </summary>
    public class CswNbtModuleRulePCIDSync : CswNbtModuleRule
    {
        public CswNbtModuleRulePCIDSync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.PCIDSync; } }
        protected override void OnEnable()
        {
            // Clear the C3SyncDate configuration variable
            _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConvert.ToString( CswEnumConfigurationVariableNames.C3SyncDate ), string.Empty );

        }// OnEnabled

        protected override void OnDisable()
        {

        } // OnDisable()

    } // class CswNbtModulePCIDSync
}// namespace ChemSW.Nbt


using ChemSW.Config;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Dev Module
    /// </summary>
    public class CswNbtModuleRuleDev : CswNbtModuleRule
    {
        public CswNbtModuleRuleDev( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.Dev; } }
        public override void OnEnable()
        {
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level.ToString(), "Info" );
            }
        }
        public override void OnDisable()
        {
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level.ToString(), "None" );
            }
        }

    } // class CswNbtModuleRuleDev
}// namespace ChemSW.Nbt

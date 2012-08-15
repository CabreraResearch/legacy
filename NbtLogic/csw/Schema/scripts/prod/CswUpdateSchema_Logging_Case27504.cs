
using ChemSW.Config;
using ChemSW.Log;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27504
    /// </summary>
    public class CswUpdateSchema_Logging_Case27504 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level, "Log level verbosity. Possible values: Info, Performance, Warn, Error, None.", LogLevels.None, IsSystem: false );
        }//Update()

    }//class CswUpdateSchema_Logging_Case27504

}//namespace ChemSW.Nbt.Schema
using ChemSW.Config;
using System.Collections;

namespace ChemSW.Nbt.Config
{

    public class CswSetupVblsNbt : ICswSetupVbls
    {

        private CswSetupVariables _cswSetupVariables = null;
        public CswSetupVblsNbt( CswEnumSetupMode SetupMode )
        {
            _cswSetupVariables = new CswSetupVariables( SetupMode );

            //_cswOracleSetupVbls.addVblDef( "LogFileLocation", "c:\\", "Location of logfile in local filesystem.", false );
            //_cswOracleSetupVbls.addVblDef( "LogOutputToFlatText", "0", "If 1, log data will be sent to a flat txt file in the LogFileLocation.", false );
            //_cswOracleSetupVbls.addVblDef( "LogOutputToXml", "1", "If 1, log data will be sent to an xml file in the LogFileLocation.", false );
            //_cswOracleSetupVbls.addVblDef( "LogOutputToDb", "0", "If 1, log data will be sent to the database.", false );
            //_cswOracleSetupVbls.addVblDef( "LogErrorMessages", "1", "If 1, Error messages should be included in the log.", false );
            //_cswOracleSetupVbls.addVblDef( "LogApplicationState", "0", "If 1, Application State messages should be included in the log.", false );
            //_cswOracleSetupVbls.addVblDef( "LogTraceMessages", "0", "If 1, Trace messages should be included in the log.", false );
            //_cswOracleSetupVbls.addVblDef( "SchedulerPollMinutes", "1", "Interval in minutes for the Scheduler to run.", false );
            //_cswOracleSetupVbls.addVblDef( "ChooseAccessIDFromDropDown", "0", "If 1, allow users to select their Customer ID from a drop down list.\nIf 0, the user must enter the correct Customer ID in a text box.", false );
            //_cswOracleSetupVbls.addVblDef( "ForceGcCollectInScheduler", "0", "If 1, the scheduler service will force a garbage collection on each cycle.\nIf 0, the schedule service will not interfere with garbage collection.", false );
            //_cswOracleSetupVbls.addVblDef( "SmtpServer", "", "Domain name or ip address of SMTP server", false );
            //_cswOracleSetupVbls.addVblDef( "SmtpPort", "", "Port number of SMTP server", false );
            //_cswOracleSetupVbls.addVblDef( "SmtpSender", "", "Email address for origination of outgoing email", false );
            //_cswOracleSetupVbls.addVblDef( "SmtpSenderDisplayName", "", "Name to display in 'from' field of outgoing email", false );
            //_cswOracleSetupVbls.addVblDef( "SmtpType", "", "'Plain' for non-authenticated SMTP; 'Authenticated' for Authenticated SMTP", false );
            //_cswOracleSetupVbls.addVblDef( "SmtpUserId", "", "For 'Authenticated' SmtpType, specifies the logon id", true );
            //_cswOracleSetupVbls.addVblDef( "SmtpPassword", "", "For 'Authenticated' SmtpType, specifies the password to go with the SmtpLogOnId", true );

        }//ctor

        public CswEnumSetupMode SetupMode { get { return ( _cswSetupVariables.SetupMode ); } }
        public ICswSetupVblsContract SetupVblsContract { get; set; }

        public bool doesSettingExist( string VblName )
        {
            return ( _cswSetupVariables.doesSettingExist( VblName ) );
        }//

        public string readSetting( string VblName )
        {
            return ( _cswSetupVariables.readSetting( VblName ) );

        }//ReadSetting()

        public void writeSetting( string VblName, string VblValue )
        {
            _cswSetupVariables.writeSetting( VblName, VblValue );

        }//writeSetting()

        public void removeSetting( string VblName )
        {
            _cswSetupVariables.removeSetting( VblName );

        }//removeSetting() 

        public string getDescription( string VariableName )
        {
            return ( _cswSetupVariables.getDescription( VariableName ) );
        }

        public IEnumerable Settings
        {
            get { return ( _cswSetupVariables.Settings ); }
        }
       
        public string this[string VariableName]
        {
            set { _cswSetupVariables[VariableName] = value; }
            get { return ( _cswSetupVariables[VariableName] ); }
        }

    }//CswOracleSetupVbls
}//namespace ChemSW.Nbt.Config



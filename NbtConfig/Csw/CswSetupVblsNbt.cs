using System;
using System.Collections;
using System.Data;
using ChemSW.Core;
using ChemSW.Config;

namespace ChemSW.Nbt.Config
{

    public class CswSetupVblsNbt : ICswSetupVbls
    {

        private CswSetupVbls _CswSetupVbls = null;
        public CswSetupVblsNbt( SetupMode SetupMode )
        {
            _CswSetupVbls = new CswSetupVbls( SetupMode, CswTools.getConfigurationFilePath( SetupMode ) );

            //_CswSetupVbls.addVblDef( "LogFileLocation", "c:\\", "Location of logfile in local filesystem.", false );
            //_CswSetupVbls.addVblDef( "LogOutputToFlatText", "0", "If 1, log data will be sent to a flat txt file in the LogFileLocation.", false );
            //_CswSetupVbls.addVblDef( "LogOutputToXml", "1", "If 1, log data will be sent to an xml file in the LogFileLocation.", false );
            //_CswSetupVbls.addVblDef( "LogOutputToDb", "0", "If 1, log data will be sent to the database.", false );
            //_CswSetupVbls.addVblDef( "LogErrorMessages", "1", "If 1, Error messages should be included in the log.", false );
            //_CswSetupVbls.addVblDef( "LogApplicationState", "0", "If 1, Application State messages should be included in the log.", false );
            //_CswSetupVbls.addVblDef( "LogTraceMessages", "0", "If 1, Trace messages should be included in the log.", false );
            //_CswSetupVbls.addVblDef( "SchedulerPollMinutes", "1", "Interval in minutes for the Scheduler to run.", false );
            //_CswSetupVbls.addVblDef( "ChooseAccessIDFromDropDown", "0", "If 1, allow users to select their Customer ID from a drop down list.\nIf 0, the user must enter the correct Customer ID in a text box.", false );
            //_CswSetupVbls.addVblDef( "ForceGcCollectInScheduler", "0", "If 1, the scheduler service will force a garbage collection on each cycle.\nIf 0, the schedule service will not interfere with garbage collection.", false );
            //_CswSetupVbls.addVblDef( "SmtpServer", "", "Domain name or ip address of SMTP server", false );
            //_CswSetupVbls.addVblDef( "SmtpPort", "", "Port number of SMTP server", false );
            //_CswSetupVbls.addVblDef( "SmtpSender", "", "Email address for origination of outgoing email", false );
            //_CswSetupVbls.addVblDef( "SmtpSenderDisplayName", "", "Name to display in 'from' field of outgoing email", false );
            //_CswSetupVbls.addVblDef( "SmtpType", "", "'Plain' for non-authenticated SMTP; 'Authenticated' for Authenticated SMTP", false );
            //_CswSetupVbls.addVblDef( "SmtpUserId", "", "For 'Authenticated' SmtpType, specifies the logon id", true );
            //_CswSetupVbls.addVblDef( "SmtpPassword", "", "For 'Authenticated' SmtpType, specifies the password to go with the SmtpLogOnId", true );

        }//ctor


        public SetupMode SetupMode { get { return ( _CswSetupVbls.SetupMode ); } }

        public void setSetupMode( SetupMode SetupMode, string SetupFilePath )
        {
            _CswSetupVbls.setSetupMode( SetupMode, SetupFilePath );
        }//SetSetupMode()


        public bool doesSettingExist( string VblName )
        {
            return ( _CswSetupVbls.doesSettingExist( VblName ) );
        }//

        public string readSetting( string VblName )
        {
            return ( _CswSetupVbls.readSetting( VblName ) );

        }//ReadSetting()

        public void writeSetting( string VblName, string VblValue )
        {
            _CswSetupVbls.writeSetting( VblName, VblValue );

        }//writeSetting()

        public void removeSetting( string VblName )
        {
            _CswSetupVbls.removeSetting( VblName );

        }//removeSetting() 

        public string getDescription( string VariableName )
        {
            return ( _CswSetupVbls.getDescription( VariableName ) );
        }

        public IEnumerable Settings
        {
            get { return ( _CswSetupVbls.Settings ); }
        }

        public DataTable SettingsTable
        {
            get { return _CswSetupVbls.SettingsTable; }
        }

        public string this[string VariableName]
        {
            set { _CswSetupVbls[VariableName] = value; }
            get { return ( _CswSetupVbls[VariableName] ); }
        }

    }//CswSetupVbls
}//namespace ChemSW.Nbt.Config



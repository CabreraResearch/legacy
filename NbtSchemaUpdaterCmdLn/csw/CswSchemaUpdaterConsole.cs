using System;
using System.Data;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt;
using ChemSW.DB;
using ChemSW.Nbt.Security;


namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaUpdaterConsole
    {


        private string _VersionInfo
        {
            get
            {
                string ReturnVal = string.Empty;

                ReturnVal = _Separator_NuLine + "ChemSW NBT SchemaUpdater version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if( ( null != _CswSchemaUpdater ) && ( string.Empty != _CswNbtResources.AccessId ) )
                {
                    ReturnVal += _Separator_NuLine + _Separator_NuLine + "Minimum schema version: " + _CswSchemaUpdater.MinimumVersion;
                    ReturnVal += _Separator_NuLine + "Latest schema version: " + _CswSchemaUpdater.LatestVersion;
                }

                return ( ReturnVal );
            }
        }

        private string _Help
        {
            get
            {

                string ReturnVal = string.Empty;

                ReturnVal = _VersionInfo;




                ReturnVal += _Separator_NuLine + _Separator_NuLine + "Usage: " +
                                            _Separator_NuLine + "NbtSchemaUpdate " + _Separator_Arg + _ArgKey_Help + _Separator_OrArgs + _Separator_Arg + _ArgKey_All + _Separator_OrArgs + _Separator_Arg + _ArgKey_AccessId + " <AccessId>" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey_All + ": update all schemata specified CswDbConfig.xml" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey_AccessId + " <AccessId>: The AccessId, as per CswDbConfig.xml, of the schema to be updated" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey_Mode + " prod | test: perform schema update, or auto-test the schema update infrastructure" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey_Describe + " writes descriptions of all current scripts"+
                                            _Separator_NuLine + _Separator_Arg + _ArgKey_StartAtTestCase + "  test case to begin at";
                return ( ReturnVal );
            }
        }

        private CswDbCfgInfoNbt _CswDbCfgInfoNbt = null;
        private CswSchemaUpdater _CswSchemaUpdater = null;
        private CswNbtResources _CswNbtResources = null;
        private CswSchemaUpdateThread _CswSchemaUpdateThread = null;
        private CswConsoleOutput _CswConsoleOutput = null;



        public CswSchemaUpdaterConsole()
        {
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.Executable );
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, new CswSetupVblsNbt( SetupMode.Executable ), _CswDbCfgInfoNbt, CswTools.getConfigurationFilePath( SetupMode.Executable ), false, false );
            _CswNbtResources.CurrentUser = new CswNbtSystemUser( _CswNbtResources, "_SchemaUpdaterUser" );
            _CswConsoleOutput = new CswConsoleOutput( _CswNbtResources.CswLogger );


        }//ctor


        private const string _Separator_OrArgs = " | ";
        private const string _Separator_NuLine = "\r\n";
        private const string _Separator_Arg = "-";
        private const string _ArgKey_Help = "help";
        private const string _ArgKey_AccessId = "accessid";
        private const string _ArgKey_All = "all";
        private const string _ArgKey_Mode = "mode";
        private const string _ArgVal_Test = "test";
        private const string _ArgKey_Describe = "describe";
        private const string _ArgKey_StartAtTestCase = "start";



        private Dictionary<string, string> _UserArgs = new Dictionary<string, string>();
        private void _convertArgsToDictionary( string[] args )
        {
            bool ArgsAreValid = true;
            int idx = 0;
            while( ( idx < args.Length ) && ArgsAreValid )
            {
                string CurrentArg = args[idx];
                if( Convert.ToChar( _Separator_Arg ) == CurrentArg[0] )
                {
                    string CurrentArgContent = CurrentArg.Substring( 1 ).Trim().ToLower();
                    if( _ArgKey_AccessId == CurrentArgContent || _ArgKey_Mode == CurrentArgContent || _ArgKey_StartAtTestCase == CurrentArgContent )
                    {
                        _UserArgs.Add( CurrentArgContent, args[idx + 1].Trim() );
                        idx += 2;
                    }
                    else
                    {
                        _UserArgs.Add( CurrentArgContent, string.Empty );
                        idx += 1;
                    }
                }
                else
                {
                    ArgsAreValid = false;
                    _UserArgs.Clear();
                }
            }

        }//_convertArgsToDictionary() 


        ICswSchemaScripts _CswSchemaScripts = null;
        public void process( string[] args )
        {

            try
            {

                _convertArgsToDictionary( args );

                Int32 StartAtTestCase = 0;
                if( _UserArgs.ContainsKey( _ArgKey_StartAtTestCase ) )
                {
                    StartAtTestCase = CswConvert.ToInt32( _UserArgs[_ArgKey_StartAtTestCase] ); 
                }

                if( _UserArgs.ContainsKey( _ArgKey_Mode ) && _ArgVal_Test == _UserArgs[_ArgKey_Mode] )
                {
                    _CswSchemaScripts = new CswSchemaScriptsTest( _CswNbtResources, StartAtTestCase );
                }
                else
                {
                    _CswSchemaScripts = new CswSchemaScriptsProd( _CswNbtResources );
                }



                _CswSchemaUpdater = new CswSchemaUpdater( _CswNbtResources, _CswSchemaScripts );
                _CswSchemaUpdateThread = new CswSchemaUpdateThread( _CswSchemaUpdater );


                if( false == _UserArgs.ContainsKey( _ArgKey_Help ) )
                {
                    string AccessId = string.Empty;
                    if( _UserArgs.ContainsKey( _ArgKey_AccessId ) )
                    {
                        AccessId = _UserArgs[_ArgKey_AccessId];
                        if( _CswDbCfgInfoNbt.AccessIds.Contains( AccessId ) )
                        {
                            _CswNbtResources.AccessId = AccessId;

                            if( false == _UserArgs.ContainsKey( _ArgKey_Describe ) )
                            {
                                _updateAccessId();
                            }
                            else
                            {
                                if( _UserArgs.ContainsKey( _ArgKey_Mode ) && _ArgVal_Test == _UserArgs[_ArgKey_Mode] )
                                {
                                    _describe();
                                }
                                else
                                {
                                    _CswConsoleOutput.write( "Ach. The iteration model for production scripts so woefully different than for test scripts that verily do I say unto the brother, uh, yay: it is easier for a camel to pass through the eye of a needle than is to give an inventory of production test scripts. Of course, since the production test scripts don't deploy the Description property of CswRequestDriver in as a rich a way as do the test scripts, it probably doesn't matter. See case 21739" );
                                }
                            }//if-else a 
                        }
                        else
                        {
                            _CswConsoleOutput.write( "AccessId " + AccessId + " unknown" );
                        }
                    }//if user said to work with a specific accessid
                    else if( _UserArgs.ContainsKey( _ArgKey_All ) )
                    {
                        string[] AccessIds = new string[_CswDbCfgInfoNbt.AccessIds.Count];
                        _CswDbCfgInfoNbt.AccessIds.CopyTo( AccessIds );
                        for( int idx = 0; idx < AccessIds.Length; idx++ )
                        {

                            string CurrentAccessId = AccessIds[idx];
                            _CswConsoleOutput.write( _Separator_NuLine + "Applying schema operation to AccessId " + CurrentAccessId + "=========================" + _Separator_NuLine );
                            _CswNbtResources.AccessId = CurrentAccessId;
                            _updateAccessId();
                            _CswConsoleOutput.write( _Separator_NuLine );
                        }
                    }//if user said to update all accessids
                    else
                    {
                        _CswConsoleOutput.write( _Help );
                    }//if the user's input does not fit our semantic space, he needs help (as do we all)
                }
                else
                {
                    _CswConsoleOutput.write( _Help );
                }//if-else the "help" flag was not specified

            }//try

            catch( Exception Exception )
            {
                _CswConsoleOutput.write( "Update failed: " + Exception.Message );
            }

        }//process() 

        private void _updateAccessId()
        {
            string AccessId = _CswNbtResources.AccessId;
            CswSchemaVersion CurrentVersion = _CswSchemaUpdater.CurrentVersion;
            if( _CswSchemaUpdater.LatestVersion != CurrentVersion )
            {

                //_CswConsoleOutput.write( "Updating AccessId " + AccessId + " from to schema version " + _CswSchemaUpdater.TargetVersion.ToString() + " to schema version " + _CswSchemaUpdater.LatestVersion.ToString() + _Separator_NuLine );
                _CswConsoleOutput.write( _Separator_NuLine + _Separator_NuLine + "AccessId " + AccessId + ": schema version " + _CswSchemaUpdater.CurrentVersion.ToString() + " to schema version " + _CswSchemaUpdater.LatestVersion.ToString() + _Separator_NuLine + _Separator_NuLine );
                bool UpdateSucceeded = true;
                while( UpdateSucceeded && CurrentVersion != _CswSchemaUpdater.LatestVersion )
                {
                    CswSchemaVersion UpdateFromVersion = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration );
                    CswSchemaVersion UpdateToVersion = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration + 1 );
                    string UpdateDescription = _CswSchemaUpdater.getDriver( UpdateToVersion ).Description;
                    //                    _CswConsoleOutput.write( "Updating AccessId " + AccessId + " to schema version " + UpdateToVersion.ToString() );
                    _CswConsoleOutput.write( "AccessId " + AccessId + ": applying schema operation -- " + UpdateDescription );
                    _CswSchemaUpdateThread.start();
                    while( UpdateState.Running == _CswSchemaUpdateThread.UpdateState )
                    {
                        _CswConsoleOutput.write( " ." );
                        Thread.Sleep( 1000 );
                    }

                    UpdateSucceeded = ( UpdateState.Succeeded == _CswSchemaUpdateThread.UpdateState );
                    string MessageStem = "AccessId " + AccessId + ": ";
                    //" from schema version " + UpdateFromVersion.ToString() + " to schema version " + UpdateToVersion.ToString();
                    if( UpdateSucceeded )
                    {

                        //                        _CswConsoleOutput.write( _Separator_NuLine + MessageStem + " succeeded." + _Separator_NuLine + _Separator_NuLine );
                        _CswConsoleOutput.write( " succeeded." + _Separator_NuLine + _Separator_NuLine );
                    }
                    else
                    {
                        //                        _CswConsoleOutput.write( _Separator_NuLine + MessageStem + " failed: " + _CswSchemaUpdateThread.Message + _Separator_NuLine + _Separator_NuLine );
                        _CswConsoleOutput.write( " failed: " + _CswSchemaUpdateThread.Message + _Separator_NuLine + _Separator_NuLine );
                    }

                    CurrentVersion = _CswSchemaUpdater.CurrentVersion;

                }//iterate updates

            }
            else
            {
                _CswConsoleOutput.write( _Separator_NuLine + "AccessId " + AccessId + ": Schema version -- " + _CswSchemaUpdater.LatestVersion.ToString() + "-- is current" );
            }


        }//_updateAccessId() 


        private void _describe()
        {
            List<string> Descriptions = new List<string>();
            while( _CswSchemaUpdater.Next() )
            {
                string CurrentDescription = _CswSchemaUpdater.CurrentVersion.ToString() + ": " + _CswSchemaUpdater.getDriver( _CswSchemaUpdater.CurrentVersion ).Description;
                if( false == Descriptions.Contains( CurrentDescription ) )
                {
                    Descriptions.Add( CurrentDescription );
                }

            }

            _CswConsoleOutput.write( _VersionInfo );
            _CswConsoleOutput.write( _Separator_NuLine + _Separator_NuLine + "Script inventory:" );

            foreach( string CurrentDescription in Descriptions )
            {
                _CswConsoleOutput.write( _Separator_NuLine + CurrentDescription );
            }


        }//_describe

    }//CswSchemaUpdaterConsole

}//ChemSW.Nbt.Schema.CmdLn

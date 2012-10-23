using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaUpdaterConsole
    {

        private const string _Separator_OrArgs = " | ";
        private const string _Separator_NuLine = "\r\n";
        private const string _Separator_Arg = "-";
        private const string _ArgVal_Test = "test";
        private sealed class _ArgKey
        {
            public const string Help = "help";
            public const string AccessId = "accessid";
            public const string All = "all";
            public const string Mode = "mode";
            public const string Version = "version";
            public const string Describe = "describe";
            public const string StartAtTestCase = "start";
            public const string EndAtTestCase = "end";
            public const string IgnoreTestCasesCsv = "ignore";
        }

        public CswSchemaUpdaterConsole()
        {
        }//ctor

        private string _VersionInfo
        {
            get
            {
                return _Separator_NuLine + "ChemSW NBT SchemaUpdater version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private string _Help
        {
            get
            {
                string ReturnVal = string.Empty;
                ReturnVal = _VersionInfo;

                ReturnVal += _Separator_NuLine + _Separator_NuLine + "Usage: " +
                                            _Separator_NuLine + "NbtSchemaUpdate " + _Separator_Arg + _ArgKey.Help + _Separator_OrArgs + _Separator_Arg + _ArgKey.All + _Separator_OrArgs + _Separator_Arg + _ArgKey.AccessId + " <AccessId>" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.All + ": update all schemata specified CswDbConfig.xml" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.AccessId + " <AccessId>: The AccessId, as per CswDbConfig.xml, of the schema to be updated" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.Mode + " prod | test: perform schema update, or auto-test the schema update infrastructure" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.Version + " writes schema version of all active AccessIds" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.Describe + " writes descriptions of all current scripts" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.StartAtTestCase + "  test case to begin at" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.EndAtTestCase + "  test case to end at" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.IgnoreTestCasesCsv + "  csv of test cases to ignore";
                return ( ReturnVal );
            }
        }

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
                    if( _ArgKey.AccessId == CurrentArgContent ||
                        _ArgKey.Mode == CurrentArgContent ||
                        _ArgKey.StartAtTestCase == CurrentArgContent ||
                        _ArgKey.EndAtTestCase == CurrentArgContent ||
                        _ArgKey.IgnoreTestCasesCsv == CurrentArgContent )
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

            }//iterate commandline args

        }//_convertArgsToDictionary() 

        private CswNbtResources _makeResources( string AccessId )
        {
            CswNbtResources ret = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, false, false, null, ChemSW.RscAdo.PooledConnectionState.Closed );
            ret.InitCurrentUser = InitUser;
            if( AccessId != string.Empty )
            {
                ret.AccessId = AccessId;
            }
            return ret;
        }//_makeResources()

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, SystemUserNames.SysUsr_SchemaUpdt );
        }

        private void _doUpdateOp( string CurrentAccessId, CswNbtResources CswNbtResources, CswConsoleOutput CswConsoleOutput )
        {
            // Do the update on the current accessid
            ICswSchemaScripts CswSchemaScripts = null;
            if( _UserArgs.ContainsKey( _ArgKey.Mode ) && _ArgVal_Test == _UserArgs[_ArgKey.Mode] )
            {
                // Use test cases
                Int32 StartAtTestCase = 0;
                if( _UserArgs.ContainsKey( _ArgKey.StartAtTestCase ) )
                {
                    StartAtTestCase = CswConvert.ToInt32( _UserArgs[_ArgKey.StartAtTestCase] );
                }

                Int32 EndAtTestCase = 0;
                if( _UserArgs.ContainsKey( _ArgKey.EndAtTestCase ) )
                {
                    EndAtTestCase = CswConvert.ToInt32( _UserArgs[_ArgKey.EndAtTestCase] );
                }

                List<string> TestCasesToIgnore = new List<string>();
                if( _UserArgs.ContainsKey( _ArgKey.IgnoreTestCasesCsv ) )
                {
                    CswCommaDelimitedString CswCommaDelimitedString = new CswCommaDelimitedString();
                    CswCommaDelimitedString.FromString( _UserArgs[_ArgKey.IgnoreTestCasesCsv] );
                    TestCasesToIgnore = CswCommaDelimitedString.ToList<string>();
                }
                CswSchemaScripts = new CswSchemaScriptsTest( StartAtTestCase, EndAtTestCase, TestCasesToIgnore );
            }
            else
            {
                // Use production scripts
                CswSchemaScripts = new CswSchemaScriptsProd();
            }

            CswSchemaUpdater CswSchemaUpdater = new CswSchemaUpdater( CurrentAccessId, _makeResources, CswSchemaScripts );

            CswConsoleOutput.write( _Separator_NuLine + "Applying schema operation to AccessId " + CurrentAccessId + "=========================" + _Separator_NuLine );
            if( false == _UserArgs.ContainsKey( _ArgKey.Describe ) )
            {
                _updateAccessId( CurrentAccessId, CswNbtResources, CswSchemaUpdater, CswConsoleOutput );
            }
            else if( _UserArgs.ContainsKey( _ArgKey.Mode ) && _ArgVal_Test == _UserArgs[_ArgKey.Mode] )
            {
                CswConsoleOutput.write( "Ach. The iteration model for production scripts so woefully different than for test scripts that verily do I say unto the brother, uh, yay: it is easier for a camel to pass through the eye of a needle than is to give an inventory of production test scripts. Of course, since the production test scripts don't deploy the Description property of CswRequestDriver in as a rich a way as do the test scripts, it probably doesn't matter. See case 21739" );
            }
            else
            {
                _describe( CswNbtResources, CswSchemaUpdater, CswConsoleOutput );
            }

            CswConsoleOutput.write( _Separator_NuLine );
        }

        private void _doVersionOp( string CurrentAccessId, CswNbtResources CswNbtResources, CswConsoleOutput CswConsoleOutput )
        {
            CswSchemaUpdater CswSchemaUpdater = new CswSchemaUpdater( CurrentAccessId, _makeResources, new CswSchemaScriptsProd() );
            CswConsoleOutput.write( CurrentAccessId + ", " + CswSchemaUpdater.CurrentVersion( CswNbtResources ) );
        }

        public void process( string[] args )
        {
            CswNbtResources CswNbtResources = _makeResources( string.Empty );
            CswConsoleOutput CswConsoleOutput = new CswConsoleOutput( CswNbtResources.CswLogger );

            try
            {
                _convertArgsToDictionary( args );

                if( _UserArgs.ContainsKey( _ArgKey.Help ) )
                {
                    CswConsoleOutput.write( _Help );
                }
                else
                {
                    StringCollection AccessIdsToUpdate = new StringCollection();

                    if( _UserArgs.ContainsKey( _ArgKey.AccessId ) )
                    {
                        AccessIdsToUpdate.Add( _UserArgs[_ArgKey.AccessId] );
                    }
                    else if( _UserArgs.ContainsKey( _ArgKey.All ) || _UserArgs.ContainsKey( _ArgKey.Version ) )
                    {
                        foreach( string AccessId in CswNbtResources.CswDbCfgInfo.ActiveAccessIds )
                        {
                            AccessIdsToUpdate.Add( AccessId );
                        }
                    }
                    else
                    {
                        //if the user's input does not fit our semantic space, he needs help (as do we all)
                        CswConsoleOutput.write( _Help );
                    }

                    if( _UserArgs.ContainsKey( _ArgKey.Version ) )
                    {
                        CswConsoleOutput.write( "AccessId, Version" );
                        CswConsoleOutput.write( _Separator_NuLine );
                    }

                    foreach( string CurrentAccessId in AccessIdsToUpdate )
                    {
                        if( false == CswNbtResources.CswDbCfgInfo.AccessIds.Contains( CurrentAccessId ) )
                        {
                            CswConsoleOutput.write( "AccessId " + CurrentAccessId + " unknown" );
                        }
                        else
                        {
                            CswNbtResources.AccessId = CurrentAccessId;

                            if( _UserArgs.ContainsKey( _ArgKey.Version ) )
                            {
                                _doVersionOp( CurrentAccessId, CswNbtResources, CswConsoleOutput );
                            }
                            else
                            {
                                _doUpdateOp( CurrentAccessId, CswNbtResources, CswConsoleOutput );
                            }

                            CswConsoleOutput.write( _Separator_NuLine );

                        } // if-else( false == CswNbtResources.CswDbCfgInfo.AccessIds.Contains( CurrentAccessId ) )

                    } // foreach( string CurrentAccessId in AccessIdsToUpdate )

                } // if-else( _UserArgs.ContainsKey( Help ) )
            }//try

            catch( Exception Exception )
            {
                CswConsoleOutput.write( "Update failed: " + Exception.Message );
            }

        }//process() 


        private bool _runNonVersionScripts( CswSchemaUpdater CswSchemaUpdater, List<CswSchemaUpdateDriver> ScriptCollection, CswConsoleOutput CswConsoleOutput )
        {
            bool ReturnVal = true;

            for( int idx = 0; ReturnVal && ( idx < ScriptCollection.Count ); idx++ )
            {
                CswSchemaUpdateDriver CurrentUpdateDriver = ScriptCollection[idx];

                string ScriptDescription = CurrentUpdateDriver.SchemaVersion.ToString() + ": " + CurrentUpdateDriver.Description + ": ";
                ReturnVal = CswSchemaUpdater.runArbitraryScript( CurrentUpdateDriver );
                if( ReturnVal )
                {
                    CswConsoleOutput.write( ScriptDescription + "succeeded" + _Separator_NuLine );
                }
                else
                {
                    CswConsoleOutput.write( ScriptDescription + "failed: " + CurrentUpdateDriver.Message + _Separator_NuLine );
                }

            }

            return ( ReturnVal );
        }

        private void _updateAccessId( string AccessId, CswNbtResources CswNbtResources, CswSchemaUpdater CswSchemaUpdater, CswConsoleOutput CswConsoleOutput )
        {
            CswSchemaUpdateThread CswSchemaUpdateThread = new CswSchemaUpdateThread( CswSchemaUpdater );

            //string AccessId = _CswNbtResources.AccessId;
            CswSchemaVersion CurrentVersion = CswSchemaUpdater.CurrentVersion( CswNbtResources );
            //if( CswSchemaUpdater.LatestVersion != CurrentVersion )
            //{
            CswConsoleOutput.write( "From " + CurrentVersion.ToString() + " to " + CswSchemaUpdater.LatestVersion.ToString() + _Separator_NuLine + _Separator_NuLine );


            CswSchemaScriptsProd CswSchemaScriptsProd = new CswSchemaScriptsProd();




            bool UpdateSucceeded = _runNonVersionScripts( CswSchemaUpdater, CswSchemaScriptsProd.RunBeforeScripts, CswConsoleOutput );

            if( UpdateSucceeded )
            {

                // refresh current version in case it was altered
                CswNbtResources.ConfigVbls.refresh();
                CurrentVersion = CswSchemaUpdater.CurrentVersion( CswNbtResources );

                while( UpdateSucceeded && CurrentVersion != CswSchemaUpdater.LatestVersion )
                {
                    CswSchemaVersion UpdateFromVersion = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration );

                    if( CurrentVersion < CswSchemaUpdater.MinimumVersion )
                    {
                        UpdateSucceeded = false;
                        CswConsoleOutput.write( "AccessId " + AccessId + ": " );
                        CswConsoleOutput.write( " failed: Schema version (" + CurrentVersion.ToString() + ") is below minimum version (" + CswSchemaUpdater.MinimumVersion.ToString() + ")" + _Separator_NuLine );
                    }
                    else
                    {
                        CswSchemaVersion UpdateToVersion = null;
                        if( CurrentVersion == CswSchemaUpdater.MinimumVersion )
                        {
                            UpdateToVersion = new CswSchemaVersion( CswSchemaUpdater.LatestVersion.CycleIteration, CswSchemaUpdater.LatestVersion.ReleaseIdentifier, 1 );
                        }
                        else
                        {
                            UpdateToVersion = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration + 1 );
                        }

                        string UpdateDescription = CswSchemaUpdater.getDriver( UpdateToVersion ).Description;

                        CswConsoleOutput.write( UpdateDescription + ": " );
                        CswSchemaUpdateThread.start();
                        while( UpdateState.Running == CswSchemaUpdateThread.UpdateState )
                        {
                            CswConsoleOutput.write( "." );
                            Thread.Sleep( 1000 );
                        }

                        UpdateSucceeded = ( UpdateState.Succeeded == CswSchemaUpdateThread.UpdateState );

                        if( UpdateSucceeded )
                        {

                            CswConsoleOutput.write( " succeeded" + _Separator_NuLine );
                        }
                        else
                        {
                            CswConsoleOutput.write( " failed: " + CswSchemaUpdateThread.Message + _Separator_NuLine );
                        }

                        CswNbtResources.ClearCache();

                        CurrentVersion = CswSchemaUpdater.CurrentVersion( CswNbtResources );

                    } // if( CurrentVersion < CswSchemaUpdater.MinimumVersion )

                }// while( UpdateSucceeded && CurrentVersion != CswSchemaUpdater.LatestVersion )

            }//if pre-scripts succeded

            if( UpdateSucceeded )
            {
                UpdateSucceeded = _runNonVersionScripts( CswSchemaUpdater, CswSchemaScriptsProd.RunAfterScripts, CswConsoleOutput );
            }

            //} // if( CswSchemaUpdater.LatestVersion != CurrentVersion )
            //else
            //{
            //    CswConsoleOutput.write( _Separator_NuLine + "Schema version -- " + CswSchemaUpdater.LatestVersion.ToString() + "-- is current" );
            //}

        }//_updateAccessId() 


        private void _describe( CswNbtResources CswNbtResources, CswSchemaUpdater CswSchemaUpdater, CswConsoleOutput CswConsoleOutput )
        {
            List<string> Descriptions = new List<string>();
            foreach( CswSchemaUpdateDriver CurrentDriver in CswSchemaUpdater.UpdateDrivers.Values )
            {
                string CurrentDescription = CurrentDriver.SchemaVersion.ToString() + ": " + CurrentDriver.Description;
                if( false == Descriptions.Contains( CurrentDescription ) )
                {
                    Descriptions.Add( CurrentDescription );
                }
            }

            CswConsoleOutput.write( _VersionInfo );

            if( ( null != CswSchemaUpdater ) && ( string.Empty != CswNbtResources.AccessId ) )
            {
                CswConsoleOutput.write( _Separator_NuLine + _Separator_NuLine + "Minimum schema version: " + CswSchemaUpdater.MinimumVersion );
                CswConsoleOutput.write( _Separator_NuLine + "Latest schema version: " + CswSchemaUpdater.LatestVersion );
            }

            CswConsoleOutput.write( _Separator_NuLine + _Separator_NuLine + "Script inventory:" );

            foreach( string CurrentDescription in Descriptions )
            {
                CswConsoleOutput.write( _Separator_NuLine + CurrentDescription );
            }


        }//_describe

    }//CswSchemaUpdaterConsole

}//ChemSW.Nbt.Schema.CmdLn

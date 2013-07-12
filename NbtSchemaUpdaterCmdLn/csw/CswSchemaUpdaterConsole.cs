using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading;

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
            public const string MaxConcurrentSchemata = "maxconcurrentschemata";
            public const string DisplayConcurrentProgress = "displayconcurrentprogress";
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
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.EndAtTestCase + "  test case to end fat" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.IgnoreTestCasesCsv + "  csv of test cases to ignore" +
                                            _Separator_NuLine + _Separator_Arg + _ArgKey.MaxConcurrentSchemata + "  total simultaneous schemata to update (0 or unspecfiied means do not do concurrent updates)";
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
                        _ArgKey.IgnoreTestCasesCsv == CurrentArgContent ||
                        _ArgKey.MaxConcurrentSchemata == CurrentArgContent
                        )
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


        private object ResourceAllocationLock = new object();
        private void _makeResources( string AccessId, ref CswNbtResources CswNbtResourcesOut )
        {
            lock( ResourceAllocationLock )
            {
                CswNbtResourcesOut = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, CswEnumSetupMode.NbtExe, false, false, null, ChemSW.RscAdo.CswEnumPooledConnectionState.Closed );
                CswNbtResourcesOut.InitCurrentUser = InitUser;
                if( AccessId != string.Empty )
                {
                    CswNbtResourcesOut.AccessId = AccessId;
                }
            }
        }//_makeResources()

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswEnumSystemUserNames.SysUsr_SchemaUpdt );
        }


        public class CswNbtSchemaUpdateThreadParams
        {
            public string CurrentAccessId;
            public CswNbtResources CswNbtResources;
            public CswSchemaUpdater.ResourcesInitHandler ResourcesInitHandler;
            public CswConsoleOutput CswConsoleOutput;
//            public bool DisplayConcurrentProgressMessages;
        }

        private void _doUpdateOpThreadWrapper( Object doUpdateParamsObj )
        {
            CswNbtSchemaUpdateThreadParams ThreadParams = (CswNbtSchemaUpdateThreadParams) doUpdateParamsObj;
            try
            {
                _doUpdateOp( ThreadParams.CurrentAccessId, ThreadParams.CswNbtResources, ThreadParams.ResourcesInitHandler, ThreadParams.CswConsoleOutput, true );
            }

            catch( Exception Exception )
            {
                ThreadParams.CswConsoleOutput.write( _Separator_NuLine );
                ThreadParams.CswConsoleOutput.write( "Error updating schema " + ThreadParams.CurrentAccessId + ": " + Exception.Message + ": " + Exception.StackTrace, ForceWrite: true );
            }
        }//_doUpdateOpThreadWrapper()


        /// <summary>
        /// CAUTION: in concurrent mode ( -maxconcurrentschemata > 0 ), this method will be multi-threaded
        /// </summary>
        /// <param name="CurrentAccessId"></param>
        /// <param name="CswNbtResources"></param>
        /// <param name="ResourcesInitHandler"></param>
        /// <param name="CswConsoleOutput"></param>
        private void _doUpdateOp( string CurrentAccessId, CswNbtResources CswNbtResources, CswSchemaUpdater.ResourcesInitHandler ResourcesInitHandler, CswConsoleOutput CswConsoleOutput, bool SuppressRealTimeProgressTics )
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

            CswSchemaUpdater CswSchemaUpdater = new CswSchemaUpdater( CurrentAccessId, ResourcesInitHandler, CswSchemaScripts );

            if( false == _UserArgs.ContainsKey( _ArgKey.Describe ) )
            {
                _updateAccessId( CurrentAccessId, CswNbtResources, CswSchemaUpdater, CswConsoleOutput, SuppressRealTimeProgressTics );
            }
            else if( _UserArgs.ContainsKey( _ArgKey.Mode ) && _ArgVal_Test == _UserArgs[_ArgKey.Mode] )
            {
                CswConsoleOutput.write( "Ach. The iteration model for production scripts so woefully different than for test scripts that verily do I say unto the brother, uh, yay: it is easier for a camel to pass through the eye of a needle than is to give an inventory of production test scripts. Of course, since the production test scripts don't deploy the Description property of CswRequestDriver in as a rich a way as do the test scripts, it probably doesn't matter. See case 21739" );
            }
            else
            {
                _describe( CswNbtResources, CswSchemaUpdater, CswConsoleOutput );
            }

            
        }

        private void _doVersionOp( string CurrentAccessId, CswNbtResources CswNbtResources, CswConsoleOutput CswConsoleOutput )
        {
            CswSchemaUpdater CswSchemaUpdater = new CswSchemaUpdater( CurrentAccessId, _makeResources, new CswSchemaScriptsProd() );
            CswConsoleOutput.write( CurrentAccessId + ", " + CswSchemaUpdater.CurrentVersion( CswNbtResources ) );
        }

        public void process( string[] args )
        {
            CswNbtResources CswNbtResources = null;
            _makeResources( string.Empty, ref CswNbtResources );
            CswConsoleOutput CswConsoleOutput = new CswConsoleOutput( CswNbtResources.CswLogger, CswNbtResources.AccessId );

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
                        foreach( string AccessId in CswNbtResources.CswDbCfgInfo.AccessIds )
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


                    //Detect missing accessIDs
                    StringCollection AccessIdsToRemove = new StringCollection();
                    foreach( string CurrentAccessId in AccessIdsToUpdate )
                    {
                        if( false == CswNbtResources.CswDbCfgInfo.AccessIds.Contains( CurrentAccessId ) )
                        {
                            AccessIdsToRemove.Add( CurrentAccessId );
                        }
                    }//iterate accessids for missing ones

                    //Remove misisng accessIds
                    foreach( string CurrentAccessId in AccessIdsToRemove )
                    {
                        AccessIdsToUpdate.Remove( CurrentAccessId );
                        CswConsoleOutput.write( "AccessId " + CurrentAccessId + " could not be found; no action will be taken on it", false );
                        CswConsoleOutput.write( _Separator_NuLine );


                    }//iterate access ids to remove missing ones

                    //if we're just reporting versions, we're not going to do concurrent-updating
                    if( _UserArgs.ContainsKey( _ArgKey.Version ) )
                    {
                        foreach( string CurrentAccessId in AccessIdsToUpdate )
                        {
                            CswNbtResources.AccessId = CurrentAccessId;
                            _doVersionOp( CurrentAccessId, CswNbtResources, CswConsoleOutput );
                            CswConsoleOutput.write( _Separator_NuLine );
                        }//iterate accessids to report version
                    }
                    else if( _UserArgs.ContainsKey( _ArgKey.MaxConcurrentSchemata ) )
                    {
                        Int32 MaxConcurrentSchema = CswConvert.ToInt32( _UserArgs[_ArgKey.MaxConcurrentSchemata] );
                        _runConcurrentSchemaUpdates( AccessIdsToUpdate, MaxConcurrentSchema );
                    }
                    else //non-concurrent schema update op
                    {
                        foreach( string CurrentAccessId in AccessIdsToUpdate )
                        {
                            CswConsoleOutput.write( _Separator_NuLine );
                            CswNbtResources.AccessId = CurrentAccessId;
                            CswConsoleOutput.write( _Separator_NuLine + "Applying schema operation to AccessId " + CurrentAccessId + "=========================" + _Separator_NuLine, false );
                            _doUpdateOp( CurrentAccessId, CswNbtResources, _makeResources, CswConsoleOutput, false );
                            CswConsoleOutput.write( _Separator_NuLine );
                        }//

                    }//if-else on type of operations to do

                } // if-else( _UserArgs.ContainsKey( Help ) )
            }//try

            catch( Exception Exception )
            {
                CswConsoleOutput.write( "Update failed: " + Exception.Message );
            }

        }//process() 

        private void _runConcurrentSchemaUpdates( StringCollection AccessIds, Int32 MaxConcurrentSchemata )
        {

            List<Thread> RunningThreads = new List<Thread>();
            Queue<string> AccessIdQueue = new Queue<string>();
            Dictionary<int, CswNbtSchemaUpdateThreadParams> ThreadIdsToParams = new Dictionary<int, CswNbtSchemaUpdateThreadParams>();
            //Queue<string>

            foreach( string CurrentAccessId in AccessIds )
            {
                AccessIdQueue.Enqueue( CurrentAccessId );
            } //populate stack of Access Id's to process

            do
            {
                if( ( RunningThreads.Count < MaxConcurrentSchemata ) && ( AccessIdQueue.Count > 0 ) )
                {
                    string AccessIdToRunConcurrently = AccessIdQueue.Dequeue();
                    //        private void _doUpdateOp( string CurrentAccessId, CswNbtResources CswNbtResources, CswConsoleOutput CswConsoleOutput )

                    CswNbtSchemaUpdateThreadParams ThreadParams = new CswNbtSchemaUpdateThreadParams();
                    _makeResources( AccessIdToRunConcurrently, ref ThreadParams.CswNbtResources );
                    ThreadParams.CurrentAccessId = AccessIdToRunConcurrently;
                    ThreadParams.ResourcesInitHandler = _makeResources;
                    ThreadParams.CswConsoleOutput = new CswConsoleOutput( ThreadParams.CswNbtResources.CswLogger, ThreadParams.CswNbtResources.AccessId );
                    ThreadParams.CswConsoleOutput.CollectStatusMessages = true;
                    ThreadParams.CswConsoleOutput.ReportAccessIds = false;
//                    ThreadParams.DisplayConcurrentProgressMessages = false;



                    if( _UserArgs.ContainsKey( _ArgKey.DisplayConcurrentProgress ) )
                    {
                        ThreadParams.CswConsoleOutput.CollectStatusMessages = false;
                        ThreadParams.CswConsoleOutput.ReportAccessIds = true;

                    }
                    else
                    {
                        ThreadParams.CswConsoleOutput.CollectStatusMessages = true;
                        ThreadParams.CswConsoleOutput.ReportAccessIds = false;
                    }


                    Thread CurrentThread = new Thread( new ParameterizedThreadStart( _doUpdateOpThreadWrapper ) );
                    RunningThreads.Add( CurrentThread );



                    ThreadParams.CswConsoleOutput.write( DateTime.Now.ToString() + ": Initiating concurrent schema update on AccessId " + AccessIdToRunConcurrently + _Separator_NuLine, ForceWrite: true );

                    ThreadIdsToParams.Add( CurrentThread.ManagedThreadId, ThreadParams );
                    CurrentThread.Start( ThreadParams );
                }
                else
                {
                    bool ThreadCompleted = false;
                    Int32 CurrentThreadIdx = 0;
                    while( ( false == ThreadCompleted ) && ( CurrentThreadIdx < RunningThreads.Count ) )
                    {
                        Thread.Sleep( 500 );
                        Thread CurrentThread = RunningThreads[CurrentThreadIdx];
                        if( false == CurrentThread.IsAlive )
                        {
                            if( ThreadIdsToParams.ContainsKey( CurrentThread.ManagedThreadId ) )
                            {
                                CswNbtSchemaUpdateThreadParams RemovedThreadParams = ThreadIdsToParams[CurrentThread.ManagedThreadId];
                                RemovedThreadParams.CswConsoleOutput.write( DateTime.Now.ToString() + ": Completed schema update on AccessId " + RemovedThreadParams.CurrentAccessId + _Separator_NuLine, ForceWrite: true );

                            }

                            ThreadIdsToParams.Remove( CurrentThread.ManagedThreadId );
                            RunningThreads.Remove( CurrentThread );
                            ThreadCompleted = true;


                        }

                        CurrentThreadIdx++;
                    }

                } //if else there are more accessids to process and we're under the concurrency limit

            } while( ( AccessIdQueue.Count > 0 ) && ( RunningThreads.Count > 0 ) );


        }//_runConcurrentSchemaUpdates()


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

        }//_runNonVersionScripts()

        private void _updateAccessId( string AccessId, CswNbtResources CswNbtResources, CswSchemaUpdater CswSchemaUpdater, CswConsoleOutput CswConsoleOutput, bool SuppressRealTimeProgressTics )
        {
            CswSchemaUpdateThread CswSchemaUpdateThread = new CswSchemaUpdateThread( CswSchemaUpdater );

            //string AccessId = _CswNbtResources.AccessId;
            CswSchemaVersion CurrentVersion = CswSchemaUpdater.CurrentVersion( CswNbtResources );
            //if( CswSchemaUpdater.LatestVersion != CurrentVersion )
            //{
            CswConsoleOutput.write( "Updating from " + CurrentVersion.ToString() + " to " + CswSchemaUpdater.LatestVersion.ToString() + _Separator_NuLine + _Separator_NuLine );


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

                        CswSchemaUpdateThread.start();

                        if( false == SuppressRealTimeProgressTics )
                        {
                            CswConsoleOutput.write( UpdateDescription + ": " );
                        }

                        while( CswEnumSchemaUpdateState.Running == CswSchemaUpdateThread.UpdateState )
                        {
                            if( false == SuppressRealTimeProgressTics )
                            {
                                CswConsoleOutput.write( ". " );
                            }

                            Thread.Sleep( 1000 );

                        }

                        UpdateSucceeded = ( CswEnumSchemaUpdateState.Succeeded == CswSchemaUpdateThread.UpdateState );

                        if( UpdateSucceeded )
                        {
                            string Message = string.Empty;
                            if( false == SuppressRealTimeProgressTics )
                            {
                                Message = " succeeded" + _Separator_NuLine;
                            }
                            else
                            {
                                Message = UpdateDescription + " succeeded" + _Separator_NuLine;
                            }

                            CswConsoleOutput.write( Message );
                        }
                        else
                        {
                            string Message = string.Empty;
                            if( true == SuppressRealTimeProgressTics )
                            {
                                Message = " failed: " + CswSchemaUpdateThread.Message + _Separator_NuLine;
                            }
                            else
                            {
                                Message = UpdateDescription + " failed: " + CswSchemaUpdateThread.Message + _Separator_NuLine;
                            }

                            CswConsoleOutput.write( Message, ForceWrite: true );
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

using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Threading;
using ChemSW.Nbt;
using ChemSW.Nbt.TreeEvents;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Log;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.Config;
using ChemSW.DB;

namespace ChemSW.Nbt.Sched
{

    public struct ScheduleNodePair
    {
        public ScheduleNodePair( CswNbtSchdItem CswNbtSchdItemParam, CswNbtNode CswNbtNodeParam )
        {
            _CswNbtSchdItem = CswNbtSchdItemParam;
            _CswNbtNode = CswNbtNodeParam;
        }//
        private CswNbtSchdItem _CswNbtSchdItem;
        public CswNbtSchdItem CswNbtSchdItem
        {
            get
            {
                return ( null );
            }//
        }//
        private CswNbtNode _CswNbtNode;

    }//struct ScheduleNodePair





    public class CswNbtSchdItemRunner
    {

        private int _MainLoopSleepCycleMs = 4000;
        private ICswLogger _CswLogger = null;

        private Thread _SchdThread = null;
        private bool _ThreadContinue = true;
        private bool _ThreadResourcesCleanedUp = false;




        //        private string _CurrentActionsView = "<TreeView viewname=\"ScheduleByObjectClass\" iconfilename=\"view/view.gif\" nodeidstofilterout=\"\"><Relationship secondname=\"ScheduleClass\" secondtype=\"ObjectClassId\" secondid=\"13\" secondiconfilename=\"icons/clock.gif\" selectable=\"True\" arbitraryid=\"13\" /></TreeView>";
        private CswNbtResources _CswNbtResources = null;
        //        private CswNbtNodeWriter _CswNbtNodeWriter = null;
        private CswNbtSchdItemFactory _CswNbtSchdItemFactory = null;
        //private CswDbResourcesColl _CswDbResourcesColl = null;

        private ArrayList _ScheduleItems = new ArrayList();
        private ArrayList _AlwaysRunItems = new ArrayList();


        public CswNbtSchdItemRunner()
        {
        }//ctor()

        /*The client (i.e., the windows service app) wants to know that he can instantiate
         this class (i.e., call its constructor) without any risk of an exception occurring, 
         so that it's easier for him to have an effective exception handling mechanism. 
         Hence we put everything that would normally go into the constructor here and call
         this method when start() is called.*/
        private string _Path = "";
        private bool _InitSucceeded = false;
        private ArrayList _SessionAccessIds = new ArrayList();
        CswDbCfgInfoNbt _CswDbCfgInfoNbt = null;
        CswSetupVblsNbt _CswSetupVblsNbt = null;


        public bool logServiceRunException( Exception IncomingException, ref Exception OutGoingException )
        {
            bool ReturnVal = false;

            try
            {

                if( null != _CswNbtResources )
                {
                    _CswNbtResources.CswLogger.reportError( IncomingException );
                    ReturnVal = true;
                }

            }// try

            catch( Exception Exception )
            {
                OutGoingException = Exception;
            } //catch


            return ( ReturnVal );

        }//logServiceRunError()


        void GlobalExceptionHandler( object sender, UnhandledExceptionEventArgs args )
        {
            Exception Exception = (Exception) args.ExceptionObject;

            if( null != _CswNbtResources )
            {
                _CswNbtResources.CswLogger.reportError( Exception );
                throw ( new Exception( "Error running NBT scheduler; consult the NBT scheduler log file located in " + _CswNbtResources.SetupVbls.readSetting( "LogFileLocation" ) ) );
            }
            else
            {
                throw ( new Exception( "The following exception was thrown before NBT logging resources could be initialized; it will not appear in any NBT log files: " + Exception.Message + Exception.StackTrace ) );
            }
        }//catch    

        private void _initSessionResources()
        {

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler( GlobalExceptionHandler );
            //*** Main resource objects
            _Path = Application.StartupPath + "\\..\\etc";
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.Executable );
            _CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode.Executable  );

            _SessionAccessIds.Clear();
            foreach( string CurrentAccessId in _CswDbCfgInfoNbt.AccessIds )
            {
                _SessionAccessIds.Add( CurrentAccessId );
            }//iterate access ids


            //***Loop timing configuration
            if( !_CswSetupVblsNbt.doesSettingExist( "SchedulerTestModeMsecs" ) )
            {
                decimal LoopDelay = decimal.MinValue;
                try
                {
                    LoopDelay = Convert.ToDecimal( _CswSetupVblsNbt["SchedulerPollMSecs"] );
                }
                catch( Exception ex )
                {
					throw new CswDniException( ErrorType.Error, "Configuration Error", "SchedulerPollMSecs is non-numeric", ex );
                }

                if( LoopDelay < 1 )
                {
                    LoopDelay = 1;
                }
                else
                {
                    LoopDelay = Math.Round( LoopDelay, 0 );
                }

                _MainLoopSleepCycleMs = CswConvert.ToInt32( LoopDelay ); //bz # 8255
                //                _MainLoopSleepCycleMs = CswConvert.ToInt32( LoopDelay ) * 60000;
            }
            else
            {
                Int32 CandidateValue = CswConvert.ToInt32( _CswSetupVblsNbt["SchedulerTestModeMsecs"] );
                if( CandidateValue > 0 )
                {
                    _MainLoopSleepCycleMs = CswConvert.ToInt32( _CswSetupVblsNbt["SchedulerTestModeMsecs"] );
                }//
            }//

            //**** Thread configuration
            _SchdThread = new Thread( new ThreadStart( _mainThreadEntry ) );
            _SchdThread.Name = "ReportRunnerThread";


            _setNbtResources();


            //bz # 5150 -- the implementations of these classes need to be filled in
            //_AlwaysRunItems.Add(new CswNbtSchdItemProblemNotification(_CswNbtResources));
            //_AlwaysRunItems.Add(new CswNbtSchdItemTaskNotification(_CswNbtResources));


            _InitSucceeded = true;



        }//_initSessionResources()


        private void _setNbtResources()
        {
            _AlwaysRunItems.Clear();


            //_CswNbtObjClassFactory = new CswNbtObjClassFactory();
            //_CswNbtResources = new CswNbtResources( AppType.Sched, _CswSetupVblsNbt, _CswDbCfgInfoNbt, true, false );
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Sched, _CswSetupVblsNbt, _CswDbCfgInfoNbt , CswTools.getConfigurationFilePath( SetupMode.Executable ), true, false ); 

            _CswNbtResources.SetDbResources( new CswNbtTreeFactory( _Path ) );

            string CloseSchedulerDbConnectionsVariableName = "CloseSchedulerDbConnections";
            if( _CswSetupVblsNbt.doesSettingExist( CloseSchedulerDbConnectionsVariableName ) && "1" == _CswSetupVblsNbt[CloseSchedulerDbConnectionsVariableName] )
            {
                _CswNbtResources.PooledConnectionState = ChemSW.RscAdo.PooledConnectionState.Closed;
            }
            else
            {
                _CswNbtResources.PooledConnectionState = ChemSW.RscAdo.PooledConnectionState.Open;
            }


            //_CswNbtResources.CswTblFactory = new CswNbtTblFactory( _CswNbtResources );
            //_CswNbtResources.CswTableCaddyFactory = new CswTableCaddyFactoryNbt( _CswNbtResources );

            //            _CswNbtResources.InitDbResources();

            _CswLogger = _CswNbtResources.CswLogger;
            //            _CswLogger.setConfigurationMode( LogConfigMode.AppConfig );
            //END: Formerlly init cycle resources


            //********************************************
            //Add "run always" schedule items
            _AlwaysRunItems.Add( new CswNbtSchdItemUpdatePropertyValues( _CswNbtResources ) );
            _AlwaysRunItems.Add( new CswNbtSchdItemUpdateMTBF( _CswNbtResources ) );

            string ConfigVarName_EmulateCatastrophicError = "EmulateCatastrophicError";
            if( _CswSetupVblsNbt.doesSettingExist( ConfigVarName_EmulateCatastrophicError ) )
            {

                CswNbtEmulateCatastrophicError.CatastrophicErrorType CatastrophicErrorType;

                //in .NET 4 we'll be to replace this try/catch with the more useful Enum.TryParse() method :-(
                try
                {
                    CatastrophicErrorType = (CswNbtEmulateCatastrophicError.CatastrophicErrorType) Enum.Parse( typeof( CswNbtEmulateCatastrophicError.CatastrophicErrorType ), _CswSetupVblsNbt[ConfigVarName_EmulateCatastrophicError], true );
                    _AlwaysRunItems.Add( new CswNbtEmulateCatastrophicError( _CswNbtResources, CatastrophicErrorType ) );
                }
                catch( Exception Exception )
                {
                    _CswNbtResources.logError( new CswDniException( "Error parsing configuration variable " + ConfigVarName_EmulateCatastrophicError + ":" + Exception.Message ) );
                }


            }

        }//_setNbtResources()

        //CswNbtObjClassFactory _CswNbtObjClassFactory = null;
        private void _initCycleResources( string AccessId )
        {
            //_CswNbtResources.CswResources.clearUpdates();
            //_CswNbtResources.release(); //bz # 10124: force new database connections to be opened on every cycle

            _setNbtResources();


            if( ( _CswSetupVblsNbt.doesSettingExist( "ForceGcCollectInScheduler" ) ) &&
                ( "1" == _CswSetupVblsNbt.readSetting( "ForceGcCollectInScheduler" ) ) )
            {
                GC.Collect();
            }


            _CswNbtResources.AccessId = AccessId;
            _CswNbtResources.refresh();
            //_CswNbtResources.CurrentUser = CswNbtNodeCaster.AsUser( _CswNbtResources.Nodes.makeUserNodeFromUsername( "ScheduleRunner" ) );
            //_CswNbtResources.CurrentUser = new CswNbtSystemUser( _CswNbtResources, "_SchedItemRunnerUser" );
			_CswNbtResources.InitCurrentUser = InitUser;

			_CswNbtResources.MetaData.refreshAll();

            //**** objects configuration
            _CswNbtSchdItemFactory = new CswNbtSchdItemFactory( _CswNbtResources );

            //_CswLogger.reportAppState( "Resources initialzied for AccessId " + AccessId );
            //***Logger configuration


        }//_initCycleResources()

		public ICswUser InitUser( ICswResources Resources )
		{
			return new CswNbtSystemUser( _CswNbtResources, "_SchedItemRunnerUser" );
		}

        public void start()
        {
            try
            {
                _initSessionResources();

                if( _InitSucceeded )
                {
                    _ThreadContinue = true;
                    _SchdThread.Start();
                }
            }

            catch( Exception ex )
            {
                if( null != _CswNbtResources )
                {
                    _CswNbtResources.CswLogger.reportError( ex );
					throw ( new CswDniException( ErrorType.Error, "Error initializing NBT scheduler", "Error initializing NBT scheduler; consult the NBT scheduler log file located in " + _CswNbtResources.SetupVbls.readSetting( "LogFileLocation" ), ex ) );
                }
                else
                {
                    throw ( ex );
                }
            }//

        }//start()


        /*It's possible we're being stopped because the client tried to start us
         but then something went wrong. So we n eed to check the extent to which 
         we are actually running before we try to do any of our shutdown procedure.*/
        public void stop()
        {
            try
            {

                if( _InitSucceeded )
                {
                    _CswLogger.reportAppState( "NBTScheduleService, set termination flag; waiting for resource cleanup" );
                    _ThreadContinue = false;
                    int WaitCount = 0;
                    while( !_ThreadResourcesCleanedUp && WaitCount < 5 )
                    {

                        Thread.Sleep( 2000 );
                        WaitCount++;
                    }

                    if( _ThreadResourcesCleanedUp )
                    {
                        _CswLogger.reportAppState( "NBTScheduleService, resources cleaned up gracefully" );
                    }
                    else
                    {
                        _CswLogger.reportAppState( "NBTScheduleService, resources not cleaned up gracefully" );
                    }
                }

                if( null != _SchdThread && _SchdThread.IsAlive )
                {
                    _SchdThread.Abort();
                    _SchdThread.Join();
                    _CswLogger.reportAppState( "NBTScheduleService, thread terminated" );
                }//

            } //try


            catch( Exception Exception )
            {
                if( null != _CswNbtResources )
                {
                    _CswNbtResources.CswLogger.reportError( Exception );
                    throw ( new Exception( "Error stopping NBT scheduler; consult the NBT scheduler log file located in " + _CswNbtResources.SetupVbls.readSetting( "LogFileLocation" ) ) );
                }
                else
                {
                    throw ( Exception );
                }

            }//catch

        }//stop()


        private void _loadSchedules()
        {

            _ScheduleItems.Clear();

            //********************************************
            //Add data-driven schedule items
            //

            //// BZ 7843
            //// These are ICswNbtPropertySetScheduler classes.
            //// Eventually we'll need to make this set of object classes data-driven, but for now this'll do:
            //Collection<CswNbtMetaDataObjectClass.NbtObjectClass> ObjectClasses = new Collection<CswNbtMetaDataObjectClass.NbtObjectClass>();
            //ObjectClasses.Add( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            //ObjectClasses.Add( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            //string ConfigVarName_EmulateBogusScheduleError = "EmulateBogusScheduleClass";
            //if( _CswSetupVblsNbt.doesSettingExist( ConfigVarName_EmulateBogusScheduleError ) && "1" == _CswSetupVblsNbt[ConfigVarName_EmulateBogusScheduleError] )
            //{
            //    ObjectClasses.Add( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );
            //}

            //foreach( CswNbtMetaDataObjectClass.NbtObjectClass NbtObjectClass in ObjectClasses )
            //{
            //    ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromObjectClass( NbtObjectClass );
            //    int TotalChildren = CswNbtTree.getChildNodeCount();
            //    for( Int32 CurrentChildIdx = 0; CurrentChildIdx < TotalChildren && _ThreadContinue; CurrentChildIdx++ )
            //    {

            //        CswNbtTree.goToNthChild( CurrentChildIdx );
            //        CswNbtNode CurrentNode = CswNbtTree.getNodeForCurrentPosition();

            //        try
            //        {
            //            ICswNbtPropertySetScheduler CurrentNodeAsScheduler = CswNbtNodeCaster.AsPropertySetScheduler( CurrentNode );
            //            if( CurrentNodeAsScheduler.Enabled.Checked == Tristate.True )  // BZ 7845
            //            {
            //                CswNbtSchdItem CurrentScheduleItem = _CswNbtSchdItemFactory.makeSchdItem( CurrentNode );
            //                _ScheduleItems.Add( CurrentScheduleItem );
            //            }
            //        }

            //        catch( Exception Exception )
            //        {
            //            _CswLogger.reportAppState( "NbtScheduleService, error making schedule class from node " + CurrentNode.NodeName + " of nodetype " + CurrentNode.NodeType.NodeTypeName + ": " + Exception.Message );
            //        }


            //        CswNbtTree.goToParentNode();


            //    }//iterate schedule items

            //}// iterate object classes

            // BZ 10350 - Use an S4 to find due generators
            CswStaticSelect ScheduleItemsDueSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtSchdItemRunner.loadSchedules()_select", "ScheduleItemsDue" );
            DataTable ScheduleItemsDueTable = ScheduleItemsDueSelect.getTable( false, false, 0, 25 );

            // BZ 10350 - Pick just one row at random
            if( ScheduleItemsDueTable.Rows.Count > 0 )
            {
                System.Random r = new Random();
                Int32 RowNum = r.Next( 0, ScheduleItemsDueTable.Rows.Count );
                Int32 ScheduleItemNodeId = CswConvert.ToInt32( ScheduleItemsDueTable.Rows[RowNum]["nodeid"] );
                CswNbtNode ScheduleItemNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", ScheduleItemNodeId )];
                CswNbtSchdItem CurrentScheduleItem = _CswNbtSchdItemFactory.makeSchdItem( ScheduleItemNode );
                _ScheduleItems.Add( CurrentScheduleItem );
            }

        }//_loadSchedules()

        private void _runSchedItem( ArrayList ItemsToRun1, ArrayList ItemsToRun2, string CurrentAccessId )
        {

            //foreach( CswNbtSchdItem CurrentItem in ItemsToRun )
            //{
            // BZ 10157 - Pick an arbitrary item to run
            CswNbtSchdItem CurrentItem = null;
            System.Random r = new Random();
            if( ( ItemsToRun2.Count == 0 || r.Next( 1, 3 ) == 1 ) && ItemsToRun1.Count > 0 )
            {
                Int32 ItemIndex = r.Next( 0, ItemsToRun1.Count );
                CurrentItem = (CswNbtSchdItem) ItemsToRun1[ItemIndex];
                _CswLogger.reportAppState( "Picked Item List 1, Item: " + CurrentItem.SchedItemName + " (#" + ItemIndex.ToString() + " of " + ItemsToRun1.Count.ToString() + " total pending items)" );
            }
            else if( ItemsToRun2.Count > 0 )
            {
                Int32 ItemIndex = r.Next( 0, ItemsToRun2.Count );
                CurrentItem = (CswNbtSchdItem) ItemsToRun2[ItemIndex];
                _CswLogger.reportAppState( "Picked Item List 2, Item: " + CurrentItem.SchedItemName + " (#" + ItemIndex.ToString() + " of " + ItemsToRun2.Count.ToString() + " total pending items)" );
            }


            if( CurrentItem != null )
            {
                //bz # 9787: additional try/catch
                try
                {
                    _CswLogger.reportAppState( "Determining whether to run scheduled item " + CurrentItem.Name + " on AccessId " + CurrentAccessId );

                    if( CurrentItem.doesItemRunNow() )
                    {
                        _CswLogger.reportAppState( "NBTScheduleService, running scheduled item " + CurrentItem.Name + " on Access Id " + CurrentAccessId );

                        CurrentItem.run();

                        if( CurrentItem.Succeeded )
                        {
                            // update last run date
                            CswTableUpdate SchedItemTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtSchdItemRunner_SchedItemTable_Update", "schedule_items" );
                            DataTable SchedItemTable = SchedItemTableUpdate.getTable( "where itemname = '" + CurrentItem.SchedItemName + "'" );
                            DataRow ItemRow = null;
                            if( SchedItemTable.Rows.Count > 0 )
                            {
                                ItemRow = SchedItemTable.Rows[0];
                            }
                            else
                            {
                                ItemRow = SchedItemTable.NewRow();
                                ItemRow["itemname"] = CurrentItem.SchedItemName;
                                SchedItemTable.Rows.Add( ItemRow );
                            }
                            ItemRow["lastrun"] = CswConvert.ToDbVal( DateTime.Now );
                            SchedItemTableUpdate.update( SchedItemTable );
                        } // if( CurrentItem.Succeeded )
                        else
                        {
                            _CswLogger.reportError( new CswDniException( CurrentItem.StatusMessage ) );
                        }
                        CurrentItem.reset();

                    }//if item runs now
                }
                catch( Exception Exception )
                {
                    _CswNbtResources.logError( new CswDniException( "Scheduled-task iteration loop encountered the following exception: " + Exception.Message ) );
                }
            }

            //}//iterate items array

        }//_runSchedItems()


        private Int32 _TotalIterations = 0;
        private DateTime _DateTimeStarted = DateTime.Now;
        private string _Iteration
        {
            get
            {
                return ( _DateTimeStarted.ToString() + ": " + _TotalIterations.ToString() );
            }
        }
        private void _mainThreadEntry()
        {
            //try
            //{
            while( _ThreadContinue )
            {

                //bz # 9787: additional try/catch
                try
                {
                    _TotalIterations++;
                    _CswLogger.reportAppState( "****NBTScheduleService, beginning iteration:  " + _Iteration );

                    Int32 TotalAccessIds = _SessionAccessIds.Count;
                    for( int AccessIdIdx = 0; AccessIdIdx < TotalAccessIds && _ThreadContinue; AccessIdIdx++ )
                    {
                        string CurrentAccessId = _SessionAccessIds[AccessIdIdx].ToString();
                        _CswLogger.reportAppState( "NBTScheduleService, entering loop to evaluate scheduled items for processing on Access Id " + CurrentAccessId );



                        try
                        {
                            _CswDbCfgInfoNbt.makeConfigurationCurrent( CurrentAccessId );
                            if( false == _CswDbCfgInfoNbt.CurrentDeactivated )
                            {

                                _initCycleResources( CurrentAccessId );


                                //_runSchedItems( _AlwaysRunItems, CurrentAccessId );

                                _loadSchedules();

                                //_runSchedItems( _ScheduleItems, CurrentAccessId );
                                _runSchedItem( _AlwaysRunItems, _ScheduleItems, CurrentAccessId );

                                _CswNbtResources.finalize();
                            }
                            else
                            {
                                _CswLogger.reportAppState( "NBTScheduleService, not processing scheduled items for  Acesss Id " + CurrentAccessId + " because it is deactivated" );
                            }
                        }

                        catch( Exception Exception )
                        {
                            _CswLogger.reportError( new CswDniException( "AccessId iteration loop for AccessId " + CurrentAccessId + " encountered the following error : " + Exception.Message ) );
                        }


                    }//iterate accessids

                    _ThreadResourcesCleanedUp = true;
                }

                catch( Exception Exception )
                {
                    _CswNbtResources.logError( new CswDniException( "The top-level scheduler thread encountered the following exception: " + Exception.Message ) );
                }


                Thread.Sleep( _MainLoopSleepCycleMs );


            }//while()




        }//_runSchdThread()

    }//CswNbtSchdItemRunner

}//namespace ChemSW.Nbt.Sched

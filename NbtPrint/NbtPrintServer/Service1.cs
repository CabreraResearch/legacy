using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using System.Xml.Serialization;
using NbtPrintLib;

namespace NbtPrintServer
{
    public partial class Service1 : ServiceBase
    {
        private CswPrintJobServiceThread _svcThread;
        private bool timerEnabled = false;
        private int timerInterval = 10000;
        private System.IO.TextWriter logger = null;
        private Timer timer1 = null;
        private int logLineCount = 0;
        private string logFilePath = string.Empty;

        public Service1()
        {
            InitializeComponent();

            _svcThread = new CswPrintJobServiceThread();
            _svcThread.OnNextJob += new CswPrintJobServiceThread.NextJobEventHandler( _InitNextJobUI );

            string path = Assembly.GetExecutingAssembly().Location;
            FileInfo fileInfo = new FileInfo( path );
            logFilePath = fileInfo.DirectoryName + "\\print_server_log.txt";

            timer1 = new Timer();
            timer1.Interval = timerInterval;
            timer1.Enabled = false;
            timer1.AutoReset = false;
            timer1.Elapsed += timer1_Elapsed;
        }

        private void timer1_Elapsed( object sender, EventArgs e )
        {
            ProcessRequests( true ); //cause timer to restart itself for next cycle after this one finishes
        }
        #region WebService Thread Plumbing

        public void OnDebug()
        {
            Log( "NbtPrintServer built and running in debug mode." );
            ProcessRequests( false );
        }

        private void _InitNextJobUI( CswPrintJobServiceThread.NextJobEventArgs e )
        {
            if( e.Succeeded )
            {

                string errMsg = string.Empty;
                string statusInfo = "Job#" + e.Job.JobNo + " for " + e.Job.JobOwner + " " + e.Job.LabelCount.ToString() + " of " + e.Job.LabelName;
                bool success = NbtPrintUtil.PrintLabel( e.printer.PrinterName, e.Job.LabelData, ref statusInfo, ref errMsg );

                if( e.Job.LabelCount > 0 )
                {
                    _svcThread.updateJob( e.auth, e.Job.JobKey, success, errMsg );
                }

                if( e.Job.RemainingJobCount > 0 )
                {
                    timerInterval = 1000; //more jobs, fire soon
                }
                else
                {
                    timerInterval = 10000; //no jobs, use std polling interval of 10 sec
                }

            }
            else
            {
                Log( e.Message );
            }
            timerEnabled = true;
        } // _InitNextJobUI()

        #endregion

        protected override void OnStart( string[] args )
        {

            Log( "NbtPrintServer is starting." );
            try
            {
                ProcessRequests( true );
            }
            catch( Exception e )
            {
                Log( e.Message );
            }
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Log( "NbtPrintServer is stopping." );
        }

        private void ProcessRequests( bool restartTimer )
        {
            string path = Assembly.GetExecutingAssembly().Location;
            FileInfo fileInfo = new FileInfo( path );
            string FilePath = fileInfo.DirectoryName + "\\printersetup.config";
            XmlSerializer reader = new XmlSerializer( typeof( NbtPrintClientConfig ) );
            NbtPrintLib.NbtPrintClientConfig config = null;
            try
            {
                using( FileStream file = File.OpenRead( FilePath ) )
                {
                    config = reader.Deserialize( file ) as NbtPrintClientConfig;
                    file.Close();
                    if( config.serviceMode != true )
                    {
                        Log( "Service started but LPC configured for Client mode." );
                    }
                    else
                    {
                        //clear any working flag settings from the client side
                        for( int i = 0; i < config.printers.Count; ++i )
                        {
                            config.printers[i].working = false;
                        }
                        CheckForPrintJob( config );
                    }
                }
            }
            catch( Exception e )
            {
                Log( "Error loading config: " + e.Message );
            }
            if( restartTimer )
            {
                timer1.Interval = timerInterval;
                timer1.Enabled = true;
                timer1.Start();
            }
        }

        private void Log( string msg )
        {
            //we only hold 1000 lines and we throw it all away when you get that many
            if( logger != null && logLineCount > 1000 )
            {
                logger.Close();
                logger = null;
            }
            try
            {
                if( logger == null )
                {
                    logger = new StreamWriter( logFilePath, false );
                }

                logger.WriteLine( DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + msg );
                logger.Flush();
                ++logLineCount;
            }
            catch( Exception e )
            {

            }
        }

        private CswPrintJobServiceThread.NbtAuth _getAuth( NbtPrintClientConfig config )
        {
            return new CswPrintJobServiceThread.NbtAuth()
            {
                AccessId = config.accessid,
                UserId = config.logon,
                Password = config.password,
                baseURL = config.url,
                useSSL = ( config.url.ToLower().IndexOf( "https:" ) > -1 )
            };
        }

        private void CheckForPrintJob( NbtPrintClientConfig config )
        {
            int cnt = 0;
            foreach( PrinterSetupData aprinter in config.printers )
            {
                if( aprinter.Enabled )
                {
                    ++cnt;
                    _svcThread.NextJob( _getAuth( config ), aprinter );
                }
            }
            if( config.printers.Count < 1 )
            {
                Log( "No printers have been setup." );
            }
            else if( cnt < 1 )
            {
                Log( "No enabled printers." );
            }
        }


    }
}

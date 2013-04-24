using System.ServiceProcess;
using Microsoft.Win32;
using NbtPrintLib;

namespace NbtPrintServer
{
    public partial class Service1 : ServiceBase
    {
        private CswPrintJobServiceThread _svcThread;

        public Service1()
        {
            InitializeComponent();

            _svcThread = new CswPrintJobServiceThread();
            _svcThread.OnNextJob += new CswPrintJobServiceThread.NextJobEventHandler( _InitNextJobUI );

        }

        #region WebService Thread Plumbing


        private void _InitNextJobUI( CswPrintJobServiceThread.NextJobEventArgs e )
        {
            if( e.Succeeded )
            {

                string errMsg = string.Empty;
                string statusInfo = "Job#" + e.Job.JobNo + " for " + e.Job.JobOwner + " " + e.Job.LabelCount.ToString() + " of " + e.Job.LabelName;
                bool success = _printLabel( e.printer.PrinterName, e.Job.LabelData, statusInfo, "Labels printed: " + statusInfo, ref errMsg );

                if( e.Job.LabelCount > 0 )
                {
                    _svcThread.updateJob( e.auth, e.Job.JobKey, success, errMsg );
                }
                if( e.Job.RemainingJobCount > 0 )
                {
                    timer1.Interval = 500; //more jobs, fire soon
                }
                else
                {
                    timer1.Interval = 10000; //no jobs, use std polling interval of 10 sec
                }
            }
            else
            {
                Log( e.Message );
            }
            timer1.Enabled = true;
        } // _InitNextJobUI()


        private bool _printLabel( string aPrinterName, string LabelData, string statusInfo, string LogOnSuccess, ref string errMsg )
        {
            bool Ret = true;
            errMsg = string.Empty;

            if( LabelData != string.Empty )
            {
                if( !RawPrinterHelper.SendStringToPrinter( aPrinterName, LabelData ) )
                {
                    Ret = false;
                    errMsg = "Label printing error on client.";
                }
            }

            return Ret;
        } // _printLabel()

        #endregion

        protected override void OnStart( string[] args )
        {
            timer1.Enabled = true;
        }

        protected override void OnStop()
        {
        }

        private void timer1_Tick( object sender, System.EventArgs e )
        {
            timer1.Enabled = false;

            //read the common config info
            string subkeyname = @"SOFTWARE\ChemSW\NbtPrintClient";
            RegistryKey areg = Registry.LocalMachine.OpenSubKey( subkeyname, true );
            if( areg != null )
            {
                NbtPrintLib.NbtPrintClientConfig config = new NbtPrintClientConfig();

                config.LoadSettings( areg );
                //if we are enabled
                if( config.enabled == true )
                {
                    //for each printer
                    //if it is enabled
                    //request printjobs
                }
            }

            timer1.Enabled = true;
        }

        private void Log( string msg )
        {
            //send the message to the windows event log
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

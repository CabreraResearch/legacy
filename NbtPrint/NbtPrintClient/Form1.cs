using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Win32;
using NbtPrintLib;

namespace NbtPrintClient
{
    public partial class Form1 : Form
    {
        private string _printerKey = string.Empty;
        private CswPrintJobServiceThread _svcThread;
        //private PrinterSetupDataCollection printers = null;
        private NbtPrintClientConfig config = null;
        // private RegistryKey myRootKey = null;

        public Form1()
        {
            InitializeComponent();
            _svcThread = new CswPrintJobServiceThread();
            //_svcThread.OnRegisterLpc += new ServiceThread.RegisterEventHandler( _ServiceThread_Register );
            _svcThread.OnNextJob += new CswPrintJobServiceThread.NextJobEventHandler( _ServiceThread_NextJob );
            _svcThread.OnLabelById += new CswPrintJobServiceThread.LabelByIdEventHandler( _ServiceThread_LabelById );
            // printers = new PrinterSetupDataCollection();
            config = new NbtPrintClientConfig();
        }

        #region CAN NOT TOUCH UI
        //must not touch UI components directly!
        /*
        void _ServiceThread_Register( ServiceThread.RegisterEventArgs e )
        {
            this.BeginInvoke( new InitRegisterHandler( _InitRegisterUI ), new object[] { e } );
        } */
        void _ServiceThread_NextJob( CswPrintJobServiceThread.NextJobEventArgs e )
        {
            this.BeginInvoke( new InitNextJobHandler( _InitNextJobUI ), new object[] { e } );
        }
        void _ServiceThread_LabelById( CswPrintJobServiceThread.LabelByIdEventArgs e )
        {
            this.BeginInvoke( new InitLabelByIdHandler( _InitLabelByIdUI ), new object[] { e } );
        }

        #endregion

        private CswPrintJobServiceThread.NbtAuth _getAuth()
        {
            return new CswPrintJobServiceThread.NbtAuth()
            {
                AccessId = tbAccessId.Text,
                UserId = tbUsername.Text,
                Password = tbPassword.Text,
                baseURL = tbURL.Text,
                useSSL = ( tbURL.Text.ToLower().IndexOf( "https:" ) > -1 )
            };
        }

        private delegate void InitNextJobHandler( CswPrintJobServiceThread.NextJobEventArgs e );
        private void _InitNextJobUI( CswPrintJobServiceThread.NextJobEventArgs e )
        {
            if( e.Succeeded )
            {
                string errMsg = string.Empty;
                string statusInfo = "Job#" + e.Job.JobNo + " for " + e.Job.JobOwner + " " + e.Job.LabelCount.ToString() + " of " + e.Job.LabelName;
                if( e.Job.LabelCount < 1 )
                {
                    statusInfo = "No label jobs to print at " + DateTime.Now.ToString();
                    lblStatus.Text = statusInfo;
                    Update();
                }
                else
                {


                    bool success = NbtPrintUtil.PrintLabel( e.printer.PrinterName, e.Job.LabelData, ref statusInfo, ref errMsg );
                    if( success )
                    {
                        lblStatus.Text = statusInfo;
                    }
                    else
                    {
                        Log( errMsg );
                    }

                    if( e.Job.LabelCount > 0 )
                    {
                        CswPrintJobServiceThread.UpdateJobInvoker lblInvoke = new CswPrintJobServiceThread.UpdateJobInvoker( _svcThread.updateJob );
                        lblInvoke.BeginInvoke( _getAuth(), e.Job.JobKey, success, errMsg, null, null );
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
            }
            else
            {
                Log( e.Message );
            }
            //find this printer and clear its working flag
            for( int i = 0; i < config.printers.Count; ++i )
            {
                if( config.printers[i].LPCname == e.printer.LPCname )
                {
                    config.printers[i].working = false;
                    i = config.printers.Count + 1;
                }
            }

        } // _InitNextJobUI()

        private delegate void InitLabelByIdHandler( CswPrintJobServiceThread.LabelByIdEventArgs e );
        private void _InitLabelByIdUI( CswPrintJobServiceThread.LabelByIdEventArgs e )
        {
            if( e.Succeeded )
            {
                string errMsg = string.Empty;
                string statusInfo = "Test Label printed OK.";
                if( !NbtPrintUtil.PrintLabel( e.printer.PrinterName, e.LabelData, ref statusInfo, ref errMsg ) )
                {
                    Log( errMsg );
                    lblTestStatus.Text = errMsg;
                }
                else
                {
                    lblTestStatus.Text = statusInfo;
                }
            }
            else
            {
                lblTestStatus.Text = e.Message;
                Log( e.Message );
            }
            btnTestPrintSvc.Enabled = true;
        } // _InitNextJobUI()


        private void btnPrintEPL_Click( object sender, EventArgs e )
        {
            if( lbPrinterList.SelectedIndex > -1 )
            {
                if( RawPrinterHelper.SendStringToPrinter( config.printers[lbPrinterList.SelectedIndex].PrinterName, textBox1.Text ) != true )
                {
                    MessageBox.Show( "Print failed!" );
                    lblTestStatus.Text = "Print failed!";
                }
            }
            else
            {
                MessageBox.Show( "Please select a Printer from the list on the Setup tab for testing.", "No Selected Printer", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
        }

        protected void Status( string msg )
        {
            lblStatus.Text = msg;
        }

        protected void Log( string msg )
        {
            if( tbLog.Lines.Length > 100 )
            {
                tbLog.Text = tbLog.Text.Substring( 0, tbLog.Text.LastIndexOf( "\r\n" ) );
            }
            tbLog.Text = DateTime.Now.ToString() + " " + msg + "\r\n" + tbLog.Text;
        }



        private void btnTestPrintService_Click( object sender, EventArgs e )
        {
            //FIX THIS for selected printer
            if( lbPrinterList.SelectedIndex > -1 )
            {
                btnTestPrintSvc.Enabled = false;
                lblTestStatus.Text = "Contacting server for label data...";
                CswPrintJobServiceThread.LabelByIdInvoker lblInvoke = new CswPrintJobServiceThread.LabelByIdInvoker( _svcThread.LabelById );
                lblInvoke.BeginInvoke( _getAuth(), tbPrintLabelId.Text, tbTargetId.Text, config.printers[lbPrinterList.SelectedIndex], null, null );
            }
            else
            {
                MessageBox.Show( "Please select a Printer from the list on the Setup tab for tesing.", "No Selected Printer", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }

        }

        private void Form1_Load( object sender, EventArgs e )
        {
            //let's being our setup
            Log( "Starting up..." );
            LoadSettings();
            RefreshPrinterList();
            Log( "Ready" );
            if( cbServiceMode.Checked != true )
            {
                tabControl1.TabPages[0].Enabled = true;
                tabControl1.SelectTab( 0 );
                lblStatus.Text = "Waiting for print job.";
                CheckForPrintJob();
            }
            else
            {
                tabControl1.TabPages[0].Enabled = false;
                tabControl1.SelectTab( 1 );
            }
        }

        private void SaveSettings()
        {
            config.accessid = tbAccessId.Text;
            config.logon = tbUsername.Text;
            config.serviceMode = cbServiceMode.Checked;
            config.password = tbPassword.Text;
            config.url = tbURL.Text;
            config.SaveToReg( Application.UserAppDataRegistry );
            try
            {
                /*
                                string subkeyname = @"SOFTWARE\ChemSW\NbtPrintClient";
                                RegistryKey areg = Registry.LocalMachine.OpenSubKey( subkeyname, true );
                                if( areg == null )
                                {
                                    areg = Registry.LocalMachine.CreateSubKey( subkeyname );
                                }
                                config.SaveToReg( areg );
                */
                string path = Assembly.GetExecutingAssembly().Location;
                FileInfo fileInfo = new FileInfo( path );
                string FilePath = fileInfo.DirectoryName + "\\printersetup.config";
                XmlSerializer writer = new XmlSerializer( typeof( NbtPrintClientConfig ) );
                using( FileStream file = File.OpenWrite( FilePath ) )
                {
                    writer.Serialize( file, config );
                    file.Flush();
                    file.Close();
                }

            }
            catch( Exception )
            {
                //we can't do anything about this. the user does not have admin rights to the registry
            }
        }

        private void LoadSettings()
        {
            config.LoadFromReg( Application.UserAppDataRegistry );
            try
            {
                string subkeyname = @"SOFTWARE\ChemSW\NbtPrintClient";
                RegistryKey areg = Registry.LocalMachine.OpenSubKey( subkeyname, true );
                if( areg != null && config.url.Length > 0 )
                {
                    //if we can read it (admin) and its non-blank:
                    //force the user data to match the common data
                    config.SaveToReg( Application.UserAppDataRegistry );
                }
            }
            catch( Exception )
            {
                //oh well, no admin rights to registry
            }
            tbAccessId.Text = config.accessid;
            tbUsername.Text = config.logon;
            cbServiceMode.Checked = config.serviceMode;
            tbPassword.Text = config.password;
            tbURL.Text = config.url;
        }

        private void Form1_FormClosed( object sender, System.Windows.Forms.FormClosedEventArgs e )
        {
            SaveSettings();
        }

        private void CheckForPrintJob()
        {
            int cnt = 0;
            foreach( PrinterSetupData aprinter in config.printers )
            {
                if( aprinter.Enabled )
                {
                    ++cnt;
                    if( !aprinter.working )
                    {
                        aprinter.working = true;
                        CswPrintJobServiceThread.NextJobInvoker jobInvoke = new CswPrintJobServiceThread.NextJobInvoker( _svcThread.NextJob );
                        jobInvoke.BeginInvoke( _getAuth(), aprinter, null, null );
                    }
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

        private void timer1_Tick( object sender, EventArgs e )
        {
            //we are polling the service
            CheckForPrintJob();
        }

        private void CheckClientMode()
        {
            timer1.Enabled = false;

            if( cbServiceMode.Checked == false )
            {
                Status( "Waiting for print job." );
                CheckForPrintJob();
                timer1.Enabled = true;
            }
            else
            {
                Status( "Using service mode, this program is only for configuring printers." );
            }

        }

        private void cbEnabled_Click( object sender, EventArgs e )
        {
            CheckClientMode();
        }

        private void btnSave_Click( object sender, EventArgs e )
        {
            SaveSettings();
        }

        private void RefreshPrinterList()
        {
            lbPrinterList.Items.Clear();

            foreach( PrinterSetupData prn in config.printers )
            {
                string aname = prn.PrinterName + " as " + prn.LPCname;
                if( prn.Enabled != true )
                {
                    aname += " (disabled)";
                }
                lbPrinterList.Items.Add( aname );
            }
            if( lbPrinterList.Items.Count > 0 )
            {
                lbPrinterList.SelectedIndex = 0;
            }
            CheckClientMode();
        }

        private void button1_Click( object sender, EventArgs e )
        {

            PrinterSetup psd = new PrinterSetup();
            PrinterSetupData newPrinter = new PrinterSetupData();
            if( psd.AddPrinter( newPrinter, config.printers, _getAuth() ) )
            {
                config.printers.Add( newPrinter );
                SaveSettings();
            }
            RefreshPrinterList();
        }

        private void lbPrinterList_DoubleClick( object sender, EventArgs e )
        {
            if( lbPrinterList.SelectedIndex > -1 )
            {
                PrinterSetup psd = new PrinterSetup();
                if( psd.EditPrinter( config.printers[lbPrinterList.SelectedIndex], config.printers ) )
                {
                    SaveSettings();
                }
                RefreshPrinterList();
            }
        }


    }
}

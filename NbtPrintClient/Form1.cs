using System;
using System.Windows.Forms;
using ChemSW.Encryption;
using Microsoft.Win32;

namespace NbtPrintClient
{
    public partial class Form1 : Form
    {
        private string _printerKey = string.Empty;
        private ServiceThread _svcThread;
        private PrinterSetupDataCollection printers = null;
        public Form1()
        {
            InitializeComponent();
            _svcThread = new ServiceThread();
            //_svcThread.OnRegisterLpc += new ServiceThread.RegisterEventHandler( _ServiceThread_Register );
            _svcThread.OnNextJob += new ServiceThread.NextJobEventHandler( _ServiceThread_NextJob );
            _svcThread.OnLabelById += new ServiceThread.LabelByIdEventHandler( _ServiceThread_LabelById );
            printers = new PrinterSetupDataCollection();
        }

        #region CAN NOT TOUCH UI
        //must not touch UI components directly!
        /*
        void _ServiceThread_Register( ServiceThread.RegisterEventArgs e )
        {
            this.BeginInvoke( new InitRegisterHandler( _InitRegisterUI ), new object[] { e } );
        } */
        void _ServiceThread_NextJob( ServiceThread.NextJobEventArgs e )
        {
            this.BeginInvoke( new InitNextJobHandler( _InitNextJobUI ), new object[] { e } );
        }
        void _ServiceThread_LabelById( ServiceThread.LabelByIdEventArgs e )
        {
            this.BeginInvoke( new InitLabelByIdHandler( _InitLabelByIdUI ), new object[] { e } );
        }

        #endregion

        private ServiceThread.NbtAuth _getAuth()
        {
            return new ServiceThread.NbtAuth()
            {
                AccessId = tbAccessId.Text,
                UserId = tbUsername.Text,
                Password = tbPassword.Text,
                baseURL = tbURL.Text,
                useSSL = ( tbURL.Text.ToLower().IndexOf( "https:" ) > -1 )
            };
        }

        private delegate void InitNextJobHandler( ServiceThread.NextJobEventArgs e );
        private void _InitNextJobUI( ServiceThread.NextJobEventArgs e )
        {
            if( e.Succeeded )
            {
                string errMsg = string.Empty;
                string statusInfo = "Job#" + e.Job.JobNo + " for " + e.Job.JobOwner + " " + e.Job.LabelCount.ToString() + " of " + e.Job.LabelName;
                bool success = _printLabel( e.printer.PrinterName, e.Job.LabelData, statusInfo, "Labels printed: " + statusInfo, ref errMsg );

                if( e.Job.LabelCount > 0 )
                {
                    ServiceThread.UpdateJobInvoker lblInvoke = new ServiceThread.UpdateJobInvoker( _svcThread.updateJob );
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
            else
            {
                Log( e.Message );
            }
            timer1.Enabled = true;
        } // _InitNextJobUI()

        private delegate void InitLabelByIdHandler( ServiceThread.LabelByIdEventArgs e );
        private void _InitLabelByIdUI( ServiceThread.LabelByIdEventArgs e )
        {
            if( e.Succeeded )
            {
                string errMsg = string.Empty;
                if( !_printLabel( e.printer.PrinterName, e.LabelData, "Test Label Printed OK.", "Test label printed.", ref errMsg ) )
                {
                    Log( errMsg );
                    lblStatus.Text = errMsg;
                }
            }
            else
            {
                lblStatus.Text = e.Message;
                Log( e.Message );
            }
            btnTestPrintSvc.Enabled = true;
        } // _InitNextJobUI()

        private bool _printLabel( string aPrinterName, string LabelData, string statusInfo, string LogOnSuccess, ref string errMsg )
        {
            bool Ret = true;
            errMsg = string.Empty;

            if( LabelData != string.Empty )
            {
                if( RawPrinterHelper.SendStringToPrinter( aPrinterName, LabelData ) )
                {
                    lblStatus.Text = "Printed " + statusInfo;
                    Log( LogOnSuccess );
                }
                else
                {
                    Ret = false;
                    errMsg = "Label printing error on client.";
                    lblStatus.Text = "Error printing " + statusInfo;
                }

            }
            else
            {
                lblStatus.Text = "No label jobs to print at " + DateTime.Now.ToString();
            }

            return Ret;
        } // _printLabel()

        private void btnPrintEPL_Click( object sender, EventArgs e )
        {
            if( lbPrinterList.SelectedIndex > -1 )
            {
                if( RawPrinterHelper.SendStringToPrinter( printers[lbPrinterList.SelectedIndex].PrinterName, textBox1.Text ) != true )
                {
                    MessageBox.Show( "Print failed!" );
                }
            }
            else
            {
                MessageBox.Show( "Please select a Printer from the list on the Setup tab for tesing.", "No Selected Printer", MessageBoxButtons.OK, MessageBoxIcon.Information );
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
                lblStatus.Text = "Contacting server for label data...";
                ServiceThread.LabelByIdInvoker lblInvoke = new ServiceThread.LabelByIdInvoker( _svcThread.LabelById );
                lblInvoke.BeginInvoke( _getAuth(), tbPrintLabelId.Text, tbTargetId.Text, printers[lbPrinterList.SelectedIndex], null, null );
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

            if( cbEnabled.Checked )
            {
                CheckForPrintJob();
            }
        }

        private void SaveSettings()
        {
            CswEncryption _CswEncryption = new CswEncryption( string.Empty );
            _CswEncryption.Method = EncryptionMethod.TypeZero;

            RegistryKey akey = Application.UserAppDataRegistry.OpenSubKey( "printers", true );
            if( akey == null )
            {
                akey = Application.UserAppDataRegistry.CreateSubKey( "printers" );
            }
            printers.SaveToReg( printers, akey );

            Application.UserAppDataRegistry.SetValue( "accessid", tbAccessId.Text );
            Application.UserAppDataRegistry.SetValue( "logon", tbUsername.Text );
            Application.UserAppDataRegistry.SetValue( "enabled", cbEnabled.Checked.ToString() );
            String pwd = tbPassword.Text;
            if( pwd.Length > 0 )
            {
                pwd = _CswEncryption.encrypt( pwd );
            }
            Application.UserAppDataRegistry.SetValue( "password", pwd, Microsoft.Win32.RegistryValueKind.String );
            if( tbURL.Modified && tbURL.Text == string.Empty )
            {
                tbURL.Text = "https://imcslive.chemswlive.com/Services/"; //the default server
            }
            Application.UserAppDataRegistry.SetValue( "serverurl", tbURL.Text );
        }

        private void LoadSettings()
        {
            CswEncryption _CswEncryption = new CswEncryption( string.Empty );
            _CswEncryption.Method = EncryptionMethod.TypeZero;

            try
            {

                tbAccessId.Text = Application.UserAppDataRegistry.GetValue( "accessid" ).ToString();
                tbUsername.Text = Application.UserAppDataRegistry.GetValue( "logon" ).ToString();
                String pwd = Application.UserAppDataRegistry.GetValue( "password" ).ToString();
                pwd = pwd.Replace( "\0", string.Empty );
                try
                {
                    tbPassword.Text = _CswEncryption.decrypt( pwd );
                }
                catch( Exception e )
                {
                    tbPassword.Text = "";
                }
                tbURL.Text = Application.UserAppDataRegistry.GetValue( "serverurl" ).ToString();
                if( tbURL.Text == string.Empty )
                {
                    tbURL.Text = "https://imcslive.chemswlive.com/Services/"; //the default server
                }

                Log( "Loaded settings." );
                cbEnabled.Checked = ( Application.UserAppDataRegistry.GetValue( "Enabled" ).ToString().ToLower() == "true" );
                if( true != cbEnabled.Checked )
                {
                    lblStatus.Text = "Print jobs are not enabled, see Setup tab.";
                }
                else
                {
                    timer1.Enabled = true;
                    lblStatus.Text = "Waiting...";
                }
            }
            catch( Exception )
            {
                Log( "No configuration data found." );
                lblStatus.Text = "Use Setup tab.";
            }
            try
            {
                RegistryKey akey = Application.UserAppDataRegistry.OpenSubKey( "printers", true );
                if( akey == null )
                {
                    akey = Application.UserAppDataRegistry.CreateSubKey( "printers" );
                }
                printers.LoadFromReg( printers, akey );
            }
            catch( Exception e )
            {
                Log( "Missing or invalid printer configuration(s). " + e.Message );
            }

        }

        private void Form1_FormClosed( object sender, System.Windows.Forms.FormClosedEventArgs e )
        {
            SaveSettings();
        }

        private void CheckForPrintJob()
        {
            int cnt = 0;
            foreach( PrinterSetupData aprinter in printers )
            {
                if( aprinter.Enabled )
                {
                    ++cnt;
                    ServiceThread.NextJobInvoker jobInvoke = new ServiceThread.NextJobInvoker( _svcThread.NextJob );
                    jobInvoke.BeginInvoke( _getAuth(), aprinter, null, null );
                }
            }
            if( printers.Count < 1 )
            {
                Log( "No printers have been setup." );
                timer1.Enabled = cbEnabled.Checked;
            }
            else if( cnt < 1 )
            {
                Log( "No enabled printers." );
                timer1.Enabled = cbEnabled.Checked;
            }
        }

        private void timer1_Tick( object sender, EventArgs e )
        {
            //we are polling the service
            timer1.Enabled = false;
            if( cbEnabled.Checked )
            {
                CheckForPrintJob();
            }
        }

        private void cbEnabled_Click( object sender, EventArgs e )
        {
            if( cbEnabled.Checked == true )
            {
                Status( "Waiting for print job." );
                CheckForPrintJob();
            }
            else
            {
                Status( "Print jobs are disabled, see Setup tab." );
            }
            timer1.Enabled = cbEnabled.Checked;
        }

        private void btnSave_Click( object sender, EventArgs e )
        {
            SaveSettings();
        }

        private void RefreshPrinterList()
        {
            lbPrinterList.Items.Clear();

            foreach( PrinterSetupData prn in printers )
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
        }

        private void button1_Click( object sender, EventArgs e )
        {

            PrinterSetup psd = new PrinterSetup();
            PrinterSetupData newPrinter = new PrinterSetupData();
            if( psd.AddPrinter( newPrinter, printers, _getAuth() ) )
            {
                printers.Add( newPrinter );
                SaveSettings();
            }
            RefreshPrinterList();
        }

        private void lbPrinterList_DoubleClick( object sender, EventArgs e )
        {
            if( lbPrinterList.SelectedIndex > -1 )
            {
                PrinterSetup psd = new PrinterSetup();
                if( psd.EditPrinter( printers[lbPrinterList.SelectedIndex], printers ) )
                {
                    SaveSettings();
                }
                RefreshPrinterList();
            }
        }


    }
}

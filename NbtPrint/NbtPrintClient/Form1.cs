using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ChemSW.Core;
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
        private RegistryKey myRootKey = null;

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
                bool success = _printLabel( e.printer.PrinterName, e.Job.LabelData, statusInfo, "Labels printed: " + statusInfo, ref errMsg );

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
            else
            {
                Log( e.Message );
            }
            timer1.Enabled = true;
        } // _InitNextJobUI()

        private delegate void InitLabelByIdHandler( CswPrintJobServiceThread.LabelByIdEventArgs e );
        private void _InitLabelByIdUI( CswPrintJobServiceThread.LabelByIdEventArgs e )
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
                string HexStarter = "<HEX>";
                string HexEnder = "</HEX>";
                if( LabelData.Contains( HexStarter ) )
                {
                    // We have to print it as byte[], not string

                    // Convert to a set of byte[]'s
                    Collection<byte[]> PartsOfLabel = new Collection<byte[]>();
                    string currentLabelData = LabelData;

                    while( currentLabelData.Contains( HexStarter ) )
                    {
                        Int32 hexstart = currentLabelData.IndexOf( HexStarter );
                        Int32 hexend = currentLabelData.IndexOf( HexEnder );
                        string prestr = currentLabelData.Substring( 0, hexstart );
                        string hexstr = currentLabelData.Substring( hexstart + HexStarter.Length, hexend - hexstart - HexEnder.Length + 1 );
                        PartsOfLabel.Add( CswTools.StringToByteArray( prestr ) );
                        PartsOfLabel.Add( Convert.FromBase64String( hexstr ) );

                        currentLabelData = currentLabelData.Substring( hexend + HexEnder.Length + 1 );
                    }
                    PartsOfLabel.Add( CswTools.StringToByteArray( currentLabelData ) );

                    // Concatenate all parts into a single byte[]
                    Int32 newLen = 0;
                    foreach( byte[] part in PartsOfLabel )
                    {
                        newLen += part.Length;
                    }
                    byte[] entireLabel = new byte[newLen];
                    Int32 currentOffset = 0;
                    foreach( byte[] part in PartsOfLabel )
                    {
                        System.Buffer.BlockCopy( part, 0, entireLabel, currentOffset, part.Length );
                        currentOffset += part.Length;
                    }

                    //unmanaged code pointer required for the function call
                    IntPtr unmanagedPointer = Marshal.AllocHGlobal( entireLabel.Length );
                    try
                    {
                        Marshal.Copy( entireLabel, 0, unmanagedPointer, entireLabel.Length );
                        // Call unmanaged code
                        if( RawPrinterHelper.SendBytesToPrinter( aPrinterName, unmanagedPointer, entireLabel.Length ) )
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
                    finally
                    {
                        //unmanaged pointer must be explicitly released to prevent memory leak
                        Marshal.FreeHGlobal( unmanagedPointer );
                    }
                }
                else
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
            } // if( LabelData != string.Empty )
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
                if( RawPrinterHelper.SendStringToPrinter( config.printers[lbPrinterList.SelectedIndex].PrinterName, textBox1.Text ) != true )
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

            if( cbEnabled.Checked )
            {
                CheckForPrintJob();
            }
        }

        private void SaveSettings()
        {
            config.accessid = tbAccessId.Text;
            config.logon = tbUsername.Text;
            config.enabled = cbEnabled.Checked;
            config.password = tbPassword.Text;
            config.url = tbURL.Text;
            config.SaveSettings( Application.UserAppDataRegistry );
            try
            {
                string subkeyname = @"SOFTWARE\ChemSW\NbtPrintClient";
                RegistryKey areg = Registry.LocalMachine.OpenSubKey( subkeyname, true );
                if( areg == null )
                {
                    areg = Registry.LocalMachine.CreateSubKey( subkeyname );
                }
                config.SaveSettings( areg );
            }
            catch( Exception e )
            {
                //we can't do anything about this. the user cdoe snot have admin rights to the registry
            }
        }

        private void LoadSettings()
        {
            config.LoadSettings( Application.UserAppDataRegistry );
            try
            {
                string subkeyname = @"SOFTWARE\ChemSW\NbtPrintClient";
                RegistryKey areg = Registry.LocalMachine.OpenSubKey( subkeyname, true );
                if( areg != null && config.url.Length > 0 )
                {
                    //if we can read it (admin) and its non-blank:
                    //force the user data to match the common data
                    config.SaveSettings( Application.UserAppDataRegistry );
                }
            }
            catch( Exception e )
            {
                //oh well, no admin rights to registry
            }
            tbAccessId.Text = config.accessid;
            tbUsername.Text = config.logon;
            cbEnabled.Checked = config.enabled;
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
                    CswPrintJobServiceThread.NextJobInvoker jobInvoke = new CswPrintJobServiceThread.NextJobInvoker( _svcThread.NextJob );
                    jobInvoke.BeginInvoke( _getAuth(), aprinter, null, null );
                }
            }
            if( config.printers.Count < 1 )
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

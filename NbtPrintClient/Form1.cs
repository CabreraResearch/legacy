using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ChemSW.Core;

namespace CswPrintClient1
{
    public partial class Form1 : Form
    {
        private string _printerKey = string.Empty;
        private ServiceThread _svcThread;

        public Form1()
        {
            InitializeComponent();
            _svcThread = new ServiceThread();
            _svcThread.OnRegisterLpc += new ServiceThread.RegisterEventHandler( _ServiceThread_Register );
            _svcThread.OnNextJob += new ServiceThread.NextJobEventHandler( _ServiceThread_NextJob );
            _svcThread.OnLabelById += new ServiceThread.LabelByIdEventHandler( _ServiceThread_LabelById );

        }

        #region CAN NOT TOUCH UI
        //must not touch UI components directly!
        void _ServiceThread_Register( ServiceThread.RegisterEventArgs e )
        {
            this.BeginInvoke( new InitRegisterHandler( _InitRegisterUI ), new object[] { e } );
        }
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

        private delegate void InitRegisterHandler( ServiceThread.RegisterEventArgs e );
        private void _InitRegisterUI( ServiceThread.RegisterEventArgs e )
        {
            if( e.Succeeded )
            {
                _printerKey = e.PrinterKey;
                if( e.PrinterKey != string.Empty )
                {
                    setBtnRegisterState( "" );
                    SaveSettings();
                }
                else
                {
                    setBtnRegisterState( "No PrinterKey returned, try again." );
                }

            }
            else
            {
                _printerKey = string.Empty;
                setBtnRegisterState( e.Message );
            }
        }

        private delegate void InitNextJobHandler( ServiceThread.NextJobEventArgs e );
        private void _InitNextJobUI( ServiceThread.NextJobEventArgs e )
        {
            if( e.Succeeded )
            {
                string errMsg = string.Empty;
                string statusInfo = "Job#" + e.Job.JobNo + " for " + e.Job.JobOwner + " " + e.Job.LabelCount.ToString() + " of " + e.Job.LabelName;
                bool success = _printLabel( e.Job.LabelData, statusInfo, "Labels printed: " + statusInfo, ref errMsg );

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
                if( !_printLabel( e.LabelData, "Test Label Printed OK.", "Test label printed.", ref errMsg ) )
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

        private bool _printLabel( string LabelData, string statusInfo, string LogOnSuccess, ref string errMsg )
        {
            bool Ret = true;
            errMsg = string.Empty;

            if( LabelData != string.Empty )
            {
                if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, LabelData ) )
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


        private Int32 FourBytesToInt32( byte[] src, int start )
        {
            int headerLen = CswConvert.ToInt32( src[start] );
            headerLen += CswConvert.ToInt32( src[start + 1] ) * 64;
            headerLen += CswConvert.ToInt32( src[start + 2] ) * 64 * 64;
            headerLen += CswConvert.ToInt32( src[start + 3] ) * 64 * 64 * 64;
            return headerLen;
        }

        private void btnPrintEPL_Click( object sender, EventArgs e )
        {
            if( false ) //this is debug code for printing images
            {
                //use the EPL command GWx_orig,y_orig,width_bytes,height_bits,[byte array data]
                byte[] fsArr = File.ReadAllBytes( "skull64.bmp" );
                //getting the pixel size from the DIB header
                int height = FourBytesToInt32( fsArr, 18 );
                int width = FourBytesToInt32( fsArr, 22 );
                //build the epl data and append the width (bytes) and height (pixels). template has the leading "GWn,n,"  before width
                byte[] prefix = CswTools.StringToByteArray( textBox1.Text + CswConvert.ToInt32( width / 8 ).ToString() + "," + height.ToString() + "," );
                byte[] suffix = { 0x50, 0x0D, 0x0A, 0x0D, 0x0A }; //says print now
                byte[] rv = new byte[prefix.Length + fsArr.Length + suffix.Length];

                //getting header offset from bitmap hader
                int headerLen = FourBytesToInt32( fsArr, 10 );
                int newLen = fsArr.Length - headerLen; //BMP format has a variable length header block
                //concatenate the bytes together
                System.Buffer.BlockCopy( prefix, 0, rv, 0, prefix.Length );
                System.Buffer.BlockCopy( fsArr, headerLen, rv, prefix.Length, newLen );
                System.Buffer.BlockCopy( suffix, 0, rv, prefix.Length + fsArr.Length, suffix.Length );

                //unmanaged code pointer required for the function call
                IntPtr unmanagedPointer = Marshal.AllocHGlobal( rv.Length );
                try
                {
                    Marshal.Copy( rv, 0, unmanagedPointer, rv.Length );
                    // Call unmanaged code
                    //if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, CswTools.ByteArrayToString( rv ) ) != true )
                    if( RawPrinterHelper.SendBytesToPrinter( tbPrinter.Text, unmanagedPointer, rv.Length ) != true )
                    {
                        MessageBox.Show( "Print failed!" );
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


                if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, textBox1.Text ) != true )
                {
                    MessageBox.Show( "Print failed!" );
                }

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
            btnTestPrintSvc.Enabled = false;
            lblStatus.Text = "Contacting server for label data...";
            ServiceThread.LabelByIdInvoker lblInvoke = new ServiceThread.LabelByIdInvoker( _svcThread.LabelById );
            lblInvoke.BeginInvoke( _getAuth(), tbPrintLabelId.Text, tbTargetId.Text, null, null );
        }



        private void btnSelPrn_Click( object sender, EventArgs e )
        {
            if( printDialog1.ShowDialog() == DialogResult.OK )
            {
                tbPrinter.Text = printDialog1.PrinterSettings.PrinterName;
                setEnablePrintJobsStates();
                SaveSettings();
            }
        }

        private void setEnablePrintJobsStates()
        {
            if( btnRegister.Enabled || tbPrinter.Text == string.Empty )
            {
                cbEnabled.Checked = false;
                cbEnabled.Enabled = false;
                lblStatus.Text = "Print jobs are disabled, see Setup tab.";
            }
            else
            {
                cbEnabled.Enabled = true;
            }
            btnSelPrn.Enabled = !cbEnabled.Checked;
        }

        private void setBtnRegisterState( string errorStatus )
        {
            if( _printerKey != string.Empty )
            {
                btnRegister.Enabled = false;
                lblRegisterStatus.Text = "Success! Your printer is registered (" + _printerKey + ").";
            }
            else
            {
                btnRegister.Enabled = true;
                lblRegisterStatus.Text = errorStatus;
                setEnablePrintJobsStates();
            }
            tbDescript.Enabled = btnRegister.Enabled;
            tbLPCname.Enabled = btnRegister.Enabled;
            setEnablePrintJobsStates();
        }


        private void btnRegister_Click( object sender, EventArgs e )
        {

            btnTestPrintSvc.Enabled = false;
            lblRegisterStatus.Text = "Contacting server...";
            ServiceThread.RegisterInvoker regInvoke = new ServiceThread.RegisterInvoker( _svcThread.Register );
            regInvoke.BeginInvoke( _getAuth(), tbLPCname.Text, tbDescript.Text, null, null );
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            //let's being our setup
            Log( "Starting up..." );
            LoadSettings();
            setEnablePrintJobsStates();
        }

        private void SaveSettings()
        {
            CswEncryption _CswEncryption = new CswEncryption( string.Empty );
            _CswEncryption.Method = EncryptionMethod.TypeZero;

            Application.UserAppDataRegistry.SetValue( "LPCname", tbLPCname.Text );
            Application.UserAppDataRegistry.SetValue( "Enabled", cbEnabled.Checked.ToString() );
            Application.UserAppDataRegistry.SetValue( "printer", tbPrinter.Text );
            Application.UserAppDataRegistry.SetValue( "printerkey", _printerKey );
            Application.UserAppDataRegistry.SetValue( "description", tbDescript.Text );
            Application.UserAppDataRegistry.SetValue( "accessid", tbAccessId.Text );
            Application.UserAppDataRegistry.SetValue( "logon", tbUsername.Text );
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
                tbLPCname.Text = Application.UserAppDataRegistry.GetValue( "LPCname" ).ToString();
                cbEnabled.Checked = ( Application.UserAppDataRegistry.GetValue( "Enabled" ).ToString().ToLower() == "true" );
                tbPrinter.Text = Application.UserAppDataRegistry.GetValue( "printer" ).ToString();
                _printerKey = Application.UserAppDataRegistry.GetValue( "printerkey" ).ToString();
                tbDescript.Text = Application.UserAppDataRegistry.GetValue( "description" ).ToString();
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
                setBtnRegisterState( "" );
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
        }

        private void Form1_FormClosed( object sender, System.Windows.Forms.FormClosedEventArgs e )
        {
            SaveSettings();
        }

        private void CheckForPrintJob()
        {
            //Log( "CheckForPrintJob() not implemented." );

            //            Status( "Waiting for print job..." );

            ServiceThread.NextJobInvoker jobInvoke = new ServiceThread.NextJobInvoker( _svcThread.NextJob );
            jobInvoke.BeginInvoke( _getAuth(), _printerKey, null, null );

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
            setEnablePrintJobsStates();
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

        private void btnClearReg_Click( object sender, EventArgs e )
        {
            ConfirmDialog confirm = new ConfirmDialog();
            confirm.Text = "Are you sure you want to clear the printer?\r\nThis will permanently and irrevocably disconnect this printer from the existing print queue!";
            confirm.StartPosition = FormStartPosition.CenterParent;
            confirm.onOk = clearReg;

            confirm.ShowDialog();
        }

        private void clearReg()
        {
            _printerKey = string.Empty;
            setBtnRegisterState( "" );
            SaveSettings();
        }


    }
}

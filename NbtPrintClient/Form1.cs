using System;
using System.Windows.Forms;

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

        private void btnPrintEPL_Click( object sender, EventArgs e )
        {
            if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, textBox1.Text ) != true )
            {
                MessageBox.Show( "Print failed!" );
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
                lblRegisterStatus.Text = "Print jobs are disabled, see Setup tab.";
            }
            else
            {
                cbEnabled.Enabled = true;
            }
        }

        private void setBtnRegisterState( string errorStatus )
        {
            if( _printerKey != string.Empty )
            {
                btnRegister.Enabled = false;
                lblRegisterStatus.Text = "Success! Registered PrinterKey is " + _printerKey;
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
            Application.CommonAppDataRegistry.SetValue( "LPCname", tbLPCname.Text );
            Application.CommonAppDataRegistry.SetValue( "Enabled", cbEnabled.Checked.ToString() );
            Application.CommonAppDataRegistry.SetValue( "printer", tbPrinter.Text );
            Application.CommonAppDataRegistry.SetValue( "printerkey", _printerKey );
            Application.CommonAppDataRegistry.SetValue( "description", tbDescript.Text );
            Application.CommonAppDataRegistry.SetValue( "accessid", tbAccessId.Text );
            Application.CommonAppDataRegistry.SetValue( "logon", tbUsername.Text );
            Application.CommonAppDataRegistry.SetValue( "code", tbPassword.Text );
            if( tbURL.Modified && tbURL.Text == string.Empty )
            {
                tbURL.Text = "https://imcslive.chemswlive.com/Services/"; //the default server
            }
            Application.CommonAppDataRegistry.SetValue( "serverurl", tbURL.Text );
        }

        private void LoadSettings()
        {
            try
            {
                tbLPCname.Text = Application.CommonAppDataRegistry.GetValue( "LPCname" ).ToString();
                cbEnabled.Checked = ( Application.CommonAppDataRegistry.GetValue( "Enabled" ).ToString().ToLower() == "true" );
                tbPrinter.Text = Application.CommonAppDataRegistry.GetValue( "printer" ).ToString();
                _printerKey = Application.CommonAppDataRegistry.GetValue( "printerkey" ).ToString();
                tbDescript.Text = Application.CommonAppDataRegistry.GetValue( "description" ).ToString();
                tbAccessId.Text = Application.CommonAppDataRegistry.GetValue( "accessid" ).ToString();
                tbUsername.Text = Application.CommonAppDataRegistry.GetValue( "logon" ).ToString();
                tbPassword.Text = Application.CommonAppDataRegistry.GetValue( "code" ).ToString();
                tbURL.Text = Application.CommonAppDataRegistry.GetValue( "serverurl" ).ToString();
                if( tbURL.Text == string.Empty )
                {
                    tbURL.Text = "https://imcslive.chemswlive.com/Services/"; //the default server
                }

                Log( "Loaded settings." );
                setBtnRegisterState( "" );
            }
            catch( Exception e )
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

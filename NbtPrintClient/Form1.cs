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
                if( e.PrinterData != string.Empty )
                {
                    if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, e.PrinterData ) == true )
                    {

                        lblStatus.Text += "\nPrinting Done!";
                        Log( "Labels printed." );
                    }
                    else
                    {
                        lblStatus.Text = "Error printing!";
                    }

                }
            }
            else
            {
                Log( e.Message );
            }
            timer1.Enabled = true;
        }

        private delegate void InitLabelByIdHandler( ServiceThread.LabelByIdEventArgs e );
        private void _InitLabelByIdUI( ServiceThread.LabelByIdEventArgs e )
        {
            if( e.Succeeded )
            {
                if( e.LabelData != string.Empty )
                {
                    if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, e.LabelData ) == true )
                    {

                        lblStatus.Text += "\nPrinting Done!";
                        Log( "Test label printed." );
                    }
                    else
                    {
                        lblStatus.Text = "Error printing!";
                    }

                }
                else
                {
                    lblStatus.Text = "No label returned.";
                }
            }
            else
            {
                lblStatus.Text = e.Message;
                Log( e.Message );
            }
            btnTestPrintSvc.Enabled = true;
        }


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
                tbLog.Text = tbLog.Text.Substring( 0, tbLog.Text.LastIndexOf( "\r\n" ) ); // Skip( tbLog.Lines.Length - 100 ).ToArray();
            }
            tbLog.Text = DateTime.Now.ToString() + " " + msg + "\r\n" + tbLog.Text;
            //tbLog.AppendText( DateTime.Now.ToString() + " " + msg + "\n" );
        }



        private void btnTestPrintService_Click( object sender, EventArgs e )
        {
            btnTestPrintSvc.Enabled = false;
            lblStatus.Text = "Contacting server for label data...";
            ServiceThread.LabelByIdInvoker lblInvoke = new ServiceThread.LabelByIdInvoker( _svcThread.LabelById );
            lblInvoke.BeginInvoke( tbAccessId.Text, tbUsername.Text, tbPassword.Text, tbPrintLabelId.Text, tbTargetId.Text, null, null );
        }



        private void btnSelPrn_Click( object sender, EventArgs e )
        {
            if( printDialog1.ShowDialog() == DialogResult.OK )
            {
                tbPrinter.Text = printDialog1.PrinterSettings.PrinterName;
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
                cbEnabled.Checked = false;
            }
            tbDescript.Enabled = btnRegister.Enabled;
            tbLPCname.Enabled = btnRegister.Enabled;
            cbEnabled.Enabled = !( btnRegister.Enabled );
        }


        private void btnRegister_Click( object sender, EventArgs e )
        {

            btnTestPrintSvc.Enabled = false;
            lblRegisterStatus.Text = "Contacting server...";
            ServiceThread.RegisterInvoker regInvoke = new ServiceThread.RegisterInvoker( _svcThread.Register );
            regInvoke.BeginInvoke( tbAccessId.Text, tbUsername.Text, tbPassword.Text, tbLPCname.Text, tbDescript.Text, null, null );
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            //let's being our setup
            Log( "Starting up..." );
            LoadSettings();
            cbEnabled_Click( sender, e );
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
        }

        private void LoadSettings()
        {
            tbLPCname.Text = Application.CommonAppDataRegistry.GetValue( "LPCname" ).ToString();
            cbEnabled.Checked = ( Application.CommonAppDataRegistry.GetValue( "Enabled" ).ToString().ToLower() == "true" );
            tbPrinter.Text = Application.CommonAppDataRegistry.GetValue( "printer" ).ToString();
            _printerKey = Application.CommonAppDataRegistry.GetValue( "printerkey" ).ToString();
            tbDescript.Text = Application.CommonAppDataRegistry.GetValue( "description" ).ToString();
            tbAccessId.Text = Application.CommonAppDataRegistry.GetValue( "accessid" ).ToString();
            tbUsername.Text = Application.CommonAppDataRegistry.GetValue( "logon" ).ToString();
            tbPassword.Text = Application.CommonAppDataRegistry.GetValue( "code" ).ToString();

            Log( "Loaded settings." );
            setBtnRegisterState( "" );
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
            jobInvoke.BeginInvoke( tbAccessId.Text, tbUsername.Text, tbPassword.Text, _printerKey, null, null );

        }

        private void timer1_Tick( object sender, EventArgs e )
        {
            //we are polling the service
            timer1.Enabled = false;
            CheckForPrintJob();
            //            timer1.Enabled = true;
        }

        private void cbEnabled_Click( object sender, EventArgs e )
        {
            if( cbEnabled.Checked == true )
            {
                Status( "Waiting for print job." );

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
            confirm.ShowDialog();

            confirm.onOk += clearReg;
        }

        private void clearReg()
        {
            _printerKey = string.Empty;
            setBtnRegisterState( "" );
            SaveSettings();
        }

    }
}

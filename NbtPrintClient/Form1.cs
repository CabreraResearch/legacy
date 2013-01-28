using System;
using System.Linq;
using System.Windows.Forms;
using ChemSW;
using CswPrintClient1.NbtLabels;
using CswPrintClient1.NbtSession;

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
            //try login
            CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();
            SessionClient mySession = new SessionClient();
            mySession.Endpoint.Behaviors.Add( cookieBehavior );

            NbtSession.CswWebSvcReturn ret = mySession.Init( new NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest()
            {
                CustomerId = tbAccessId.Text,
                UserName = tbUsername.Text,
                Password = tbPassword.Text,
                IsMobile = false
            } );

            if( ret.Authentication.AuthenticationStatus == "Authenticated" )
            {
                //logged in
                Labels2Client l = new NbtLabels.Labels2Client();
                l.Endpoint.Behaviors.Add( cookieBehavior );

                NbtPrintLabelRequestGet nbtLabelget = new NbtPrintLabelRequestGet();
                nbtLabelget.LabelId = tbPrintLabelId.Text;
                nbtLabelget.TargetId = tbTargetId.Text;
                CswNbtLabelEpl epl = l.getLabel( nbtLabelget );
                if( epl.Data.Labels.Count() < 1 )
                {
                    lblStatus.Text = "No labels returned.";
                }
                else
                {
                    foreach( PrintLabel p in epl.Data.Labels )
                    {
                        lblStatus.Text = "Printing...";
                        if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, p.EplText ) == true )
                        {

                            lblStatus.Text += "\nDone!";
                        }
                        else
                        {
                            lblStatus.Text = "Error printing!";
                        }
                    }
                }
                l.Close();
                //LOGOUT
                mySession.End();
            }
            else
            {
                Status( "Authentication error: " + ret.Authentication.AuthenticationStatus );
            }
            mySession.Close();
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
                lblRegisterStatus.Text = "Registered PrinterKey=" + _printerKey;
            }
            else
            {
                btnRegister.Enabled = true;
                lblRegisterStatus.Text = errorStatus;
                cbEnabled.Checked = false;
            }
            tbLPCname.Enabled = btnRegister.Enabled;
            cbEnabled.Enabled = !( btnRegister.Enabled );
        }


        private void btnRegister_Click( object sender, EventArgs e )
        {

            btnRegister.Enabled = false;
            lblRegisterStatus.Text = "Contacting server...";
            ServiceThread.RegisterInvoker regInvoke = new ServiceThread.RegisterInvoker( _svcThread.Register );
            regInvoke.BeginInvoke( tbAccessId.Text, tbUsername.Text, tbPassword.Text, tbLPCname.Text, null, null );
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
            // Application.CommonAppDataRegistry.SetValue( "URL", tbURL.Text );
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
            //tbURL.Text = Application.CommonAppDataRegistry.GetValue( "URL" ).ToString();
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
            _printerKey = string.Empty;
            setBtnRegisterState( "" );
            SaveSettings();
        }

    }
}

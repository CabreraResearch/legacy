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

        public Form1()
        {
            InitializeComponent();
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

        private bool doRegister( string accessid, string userid, string pwd, ref string status )
        {
            bool myReturn = false;
            //try login
            CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();
            SessionClient mySession = new SessionClient();
            mySession.Endpoint.Behaviors.Add( cookieBehavior );

            NbtSession.CswWebSvcReturn ret = mySession.Init( new NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest()
            {
                CustomerId = accessid,
                UserName = userid,
                Password = pwd,
                IsMobile = false
            } );

            status = "";
            if( ret.Authentication.AuthenticationStatus == "Authenticated" )
            {
                //logged in
                Labels2Client l = new NbtLabels.Labels2Client();
                l.Endpoint.Behaviors.Add( cookieBehavior );
                LabelPrinter lblPrn = new LabelPrinter();
                lblPrn.LpcName = tbLPCname.Text;
                lblPrn.Description = "some description of printer here";
                CswPrintClient1.NbtLabels.CswNbtLabelPrinterReg Ret = l.registerLpc( lblPrn );
                l.Close();
                if( Ret.Status.Success == true )
                {
                    status = "Printer " + lblPrn.LpcName + " registered successfully.";
                    status += " PrinterKey=" + Ret.PrinterKey;
                    myReturn = true;
                }
                else
                {
                    status = "Printer " + lblPrn.LpcName + " registration failed. ";
                    if( Ret.Status.Errors.Count() > 0 )
                    {
                        status += Ret.Status.Errors[0].Message.ToString();
                    }
                }
                //LOGOUT
                mySession.End();
            }
            else
            {
                status = "Authentication error: " + ret.Authentication.AuthenticationStatus;
            }
            mySession.Close();
            return myReturn;
        }


        private void btnRegister_Click( object sender, EventArgs e )
        {
            /*
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

                        string status = "";
                        if( ret.Authentication.AuthenticationStatus == "Authenticated" )
                        {
                            //logged in
                            Labels2Client l = new NbtLabels.Labels2Client();
                            l.Endpoint.Behaviors.Add( cookieBehavior );
                            LabelPrinter lblPrn = new LabelPrinter();
                            lblPrn.LpcName = tbLPCname.Text;
                            lblPrn.Description = "some description of printer here";
                            CswPrintClient1.NbtLabels.CswNbtLabelPrinterReg Ret = l.registerLpc( lblPrn );
                            l.Close();
                            if( Ret.Status.Success == true )
                            {
                                status = "Printer " + lblPrn.LpcName + " registered successfully.";
                                status += " PrinterKey=" + Ret.PrinterKey;
                            }
                            else
                            {
                                status = "Printer " + lblPrn.LpcName + " registration failed. ";
                                if( Ret.Status.Errors.Count() > 0 )
                                {
                                    status += Ret.Status.Errors[0].Message.ToString();
                                }
                            }
                            //LOGOUT
                            mySession.End();
                        }
                        else
                        {
                            status = "Authentication error: " + ret.Authentication.AuthenticationStatus;
                        }
                        mySession.Close();
            */
            string status = "";
            //btnRegister.Enabled = (!
            doRegister( tbAccessId.Text, tbUsername.Text, tbPassword.Text, ref status );
            //);

            Log( status );
            lblRegisterStatus.Text = status;
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
            Application.CommonAppDataRegistry.SetValue( "URL", tbURL.Text );
            Application.CommonAppDataRegistry.SetValue( "accessid", tbAccessId.Text );
            Application.CommonAppDataRegistry.SetValue( "logon", tbUsername.Text );
            Application.CommonAppDataRegistry.SetValue( "code", tbPassword.Text );
        }

        private void LoadSettings()
        {
            tbLPCname.Text = Application.CommonAppDataRegistry.GetValue( "LPCname" ).ToString();
            cbEnabled.Checked = ( Application.CommonAppDataRegistry.GetValue( "Enabled" ).ToString().ToLower() == "true" );
            tbPrinter.Text = Application.CommonAppDataRegistry.GetValue( "printer" ).ToString();
            tbURL.Text = Application.CommonAppDataRegistry.GetValue( "URL" ).ToString();
            tbAccessId.Text = Application.CommonAppDataRegistry.GetValue( "accessid" ).ToString();
            tbUsername.Text = Application.CommonAppDataRegistry.GetValue( "logon" ).ToString();
            tbPassword.Text = Application.CommonAppDataRegistry.GetValue( "code" ).ToString();

            Log( "Loaded settings." );
        }

        private void Form1_FormClosed( object sender, System.Windows.Forms.FormClosedEventArgs e )
        {
            SaveSettings();
        }

        private void CheckForPrintJob()
        {
            Log( "CheckForPrintJob() not implemented." );

            Status( "Waiting for print job..." );
        }

        private void timer1_Tick( object sender, EventArgs e )
        {
            //we are polling the service
            timer1.Enabled = false;
            CheckForPrintJob();
            timer1.Enabled = true;
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

    }
}

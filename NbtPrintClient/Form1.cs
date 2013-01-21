using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
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

        private void button1_Click( object sender, EventArgs e )
        {
            if( RawPrinterHelper.SendStringToPrinter( tbPrinter.Text, textBox1.Text ) == true )
            {

            }
            else
            {

            }
        }

        protected void Status( string msg )
        {
            lblStatus.Text = msg;
        }

        protected void Log( string msg )
        {
            Status( msg );
            if( tbLog.Lines.Length > 10 )
            {
                tbLog.Lines = tbLog.Lines.Skip( tbLog.Lines.Length - 10 ).ToArray();
            }
            tbLog.AppendText( DateTime.Now.ToString() + " " + msg + "\n" );
        }

        private void button2_Click( object sender, EventArgs e )
        {
            CookieManagerBehavior cookieBehavior = new CookieManagerBehavior();

            SessionClient s = new SessionClient();
            s.Endpoint.Behaviors.Add( cookieBehavior );
            NbtSession.CswWebSvcReturn ret = s.Init( new NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest()
            {
                CustomerId = tbAccessId.Text,
                UserName = tbUsername.Text,
                Password = tbPassword.Text,
                IsMobile = false
            } );
            if( ret.Authentication.AuthenticationStatus == "Authenticated" )
            {
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
                s.End();
                s.Close();
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {
            if( printDialog1.ShowDialog() == DialogResult.OK )
            {
                tbPrinter.Text = printDialog1.PrinterSettings.PrinterName;
            }
        }

        private void button4_Click( object sender, EventArgs e )
        {
            Log( " register printer tried" );
            lblRegisterStatus.Text = "not implemented: registerLpc() now!";
            Log( " register printer failed" );
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

        private void button5_Click( object sender, EventArgs e )
        {
            SaveSettings();
        }


    }
}

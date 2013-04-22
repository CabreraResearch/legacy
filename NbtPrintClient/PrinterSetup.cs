using System.Windows.Forms;

namespace NbtPrintClient
{
    public partial class PrinterSetup : Form
    {

        private PrinterSetupData myPrinter = null;
        private PrinterSetupDataCollection printers = null;
        private ServiceThread _svcThread;
        private ServiceThread.NbtAuth _Auth = null;

        public PrinterSetup()
        {
            InitializeComponent();
            _svcThread = new ServiceThread();
            _svcThread.OnRegisterLpc += new ServiceThread.RegisterEventHandler( _ServiceThread_Register );

        }


        void _ServiceThread_Register( ServiceThread.RegisterEventArgs e )
        {
            this.BeginInvoke( new InitRegisterHandler( _InitRegisterUI ), new object[] { e } );
        }

        private delegate void InitRegisterHandler( ServiceThread.RegisterEventArgs e );
        private void _InitRegisterUI( ServiceThread.RegisterEventArgs e )
        {
            button1.Enabled = true;

            myPrinter.Succeeded = e.printer.Succeeded;
            myPrinter.Message = e.printer.Message;
            myPrinter.PrinterKey = string.Empty;
            if( e.printer.Succeeded )
            {
                myPrinter.PrinterKey = e.printer.PrinterKey;
                if( e.printer.PrinterKey == string.Empty )
                {
                    myPrinter.Message = "No PrinterKey returned, try again.";
                    myPrinter.Succeeded = false;
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }

            }
            else
            {
                myPrinter.PrinterKey = string.Empty;
            }
            if( e.printer.Succeeded != true )
            {
                if( string.IsNullOrEmpty( myPrinter.Message ) )
                {
                    myPrinter.Message = "Printer registration service failed without an error from the server.";
                }
                MessageBox.Show( myPrinter.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
        }

        private void SetupForm( PrinterSetupData aprinter, PrinterSetupDataCollection thePrinters, bool editing )
        {
            myPrinter = aprinter;
            printers = thePrinters;
            tbPrinter.Text = myPrinter.PrinterName;
            tbLPCname.Text = myPrinter.LPCname;
            tbDescript.Text = myPrinter.Description;
            cbEnabled.Checked = aprinter.Enabled;
            btnClearReg.Enabled = editing;
        }
        public bool AddPrinter( PrinterSetupData aprinter, PrinterSetupDataCollection thePrinters, ServiceThread.NbtAuth _getAuth )
        {
            _Auth = _getAuth;

            SetupForm( aprinter, thePrinters, false );
            return ( this.ShowDialog() == DialogResult.OK );
        }
        public bool EditPrinter( PrinterSetupData aprinter, PrinterSetupDataCollection thePrinters )
        {
            SetupForm( aprinter, thePrinters, true );
            return ( this.ShowDialog() == DialogResult.OK );
        }

        private void btnSelPrn_Click( object sender, System.EventArgs e )
        {
            if( printDialog1.ShowDialog() == DialogResult.OK )
            {
                myPrinter.PrinterName = printDialog1.PrinterSettings.PrinterName;
                tbPrinter.Text = myPrinter.PrinterName;
            }
        }

        private void button1_Click( object sender, System.EventArgs e )
        {
            if( tbPrinter.Text.Length > 0 && tbLPCname.Text.Length > 0 )
            {
                myPrinter.PrinterName = tbPrinter.Text;
                myPrinter.LPCname = tbLPCname.Text;
                myPrinter.Description = tbDescript.Text;
                myPrinter.Enabled = cbEnabled.Checked;

                if( myPrinter.isRegistered() != true && _Auth != null )
                {
                    //verify printername not already assigned
                    bool duplicate = false;
                    foreach( PrinterSetupData sd in printers )
                    {
                        if( sd.PrinterName == myPrinter.PrinterName )
                        {
                            duplicate = true;
                        }
                    }
                    if( !duplicate )
                    {
                        button1.Enabled = false;
                        button1.Text = "Registering...";
                        ServiceThread.RegisterInvoker regInvoke = new ServiceThread.RegisterInvoker( _svcThread.Register );
                        regInvoke.BeginInvoke( _Auth, myPrinter, null, null );
                        //this.DialogResult = DialogResult.OK; //DEBUG ONLY
                    }
                    else
                    {
                        MessageBox.Show( "This label printer is already assigned to an NBT printer queue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    }
                }
                else
                {
                    //update enabled
                    this.DialogResult = DialogResult.OK;
                }
            }
            else
            {
                MessageBox.Show( "Missing Required Fields", "You must select a Label Printer and provide a unique ChemSW Printer Name.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
        }

        private void clearReg()
        {
            //remove this printer
            int idx = -1;
            foreach( PrinterSetupData psd in printers )
            {
                if( myPrinter.PrinterKey == psd.PrinterKey )
                {
                    ++idx;
                }
            }
            if( idx > -1 )
            {
                printers.RemoveAt( idx );
            }
            //close the dialog
            this.DialogResult = DialogResult.OK;
        }


        private void btnClearReg_Click( object sender, System.EventArgs e )
        {
            ConfirmDialog confirm = new ConfirmDialog();
            confirm.Text = "Are you sure you want to clear the printer?\r\nThis will permanently and irrevocably disconnect this printer from the existing print queue!";
            confirm.StartPosition = FormStartPosition.CenterParent;
            confirm.onOk = clearReg;

            confirm.ShowDialog();

        }

        /*
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
    */
    }
}

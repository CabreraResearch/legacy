using System.Windows.Forms;
using NbtPrintLib;

namespace NbtPrintClient
{
    public partial class PrinterSetup : Form
    {

        private PrinterSetupData myPrinter = null;
        private PrinterSetupDataCollection printers = null;
        private CswPrintJobServiceThread _svcThread;
        private CswPrintJobServiceThread.NbtAuth _Auth = null;

        public PrinterSetup()
        {
            InitializeComponent();
            _svcThread = new CswPrintJobServiceThread();
            _svcThread.OnRegisterLpc += new CswPrintJobServiceThread.RegisterEventHandler( _ServiceThread_Register );

        }


        void _ServiceThread_Register( CswPrintJobServiceThread.RegisterEventArgs e )
        {
            this.BeginInvoke( new InitRegisterHandler( _InitRegisterUI ), new object[] { e } );
        }

        private delegate void InitRegisterHandler( CswPrintJobServiceThread.RegisterEventArgs e );
        private void _InitRegisterUI( CswPrintJobServiceThread.RegisterEventArgs e )
        {
            button1.Enabled = true;
            button1.Text = "OK";

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
                if( false == string.IsNullOrEmpty( myPrinter.Message ) ) {
                    //prefer an error specific to the printer registration if one exists
                    MessageBox.Show( myPrinter.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }
                else if( false == string.IsNullOrEmpty( e.Message ) ) {
                    //if there was no printer error, check for a general authentication error
                    MessageBox.Show( e.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }
                else {
                    MessageBox.Show( "Printer registration service failed without an error from the server.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }

            }//if e.printer.Succeeded != true

        }//private void _InitRegisterUI

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
        public bool AddPrinter( PrinterSetupData aprinter, PrinterSetupDataCollection thePrinters, CswPrintJobServiceThread.NbtAuth _getAuth )
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
                        CswPrintJobServiceThread.RegisterInvoker regInvoke = new CswPrintJobServiceThread.RegisterInvoker( _svcThread.Register );
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

using System;
using System.Windows.Forms;
using ChemSW.Core;

namespace Nbt2DImporterTester
{
    public partial class Form1 : Form
    {
        private readonly WorkerThread _WorkerThread;

        public Form1()
        {
            InitializeComponent();

            _WorkerThread = new WorkerThread();
            _WorkerThread.OnError = handleMessage;
            _WorkerThread.OnMessage = handleMessage;
            _WorkerThread.OnFinish = finish;
            _WorkerThread.OnStoreDataFinish = StoreDataFinished;
        }

        #region NON-UI THREAD 

        private void handleMessage( string Msg )
        {
            txtLog.BeginInvoke( new logHandler( log ), new object[] {Msg} );
        }

        private void finish()
        {
            txtLog.BeginInvoke( new logHandler( log ), new object[] { "Finished." } );
            button1.BeginInvoke( new setButtonsEnabledHandler( setButtonsEnabled ), new object[] { true } );
        }

        private void StoreDataFinished( string ImportDataTableName )
        {
            txtImportDataTableName.BeginInvoke( new setImportDataTableNameHandler( setImportDataTableName ), new object[] { ImportDataTableName } );
        }

        #endregion NON-UI THREAD

        public delegate void logHandler( string Msg );
        public void log( string Msg )
        {
            txtLog.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + Msg + "\r\n" + txtLog.Text;
        }

        private delegate void setButtonsEnabledHandler( bool enabled );
        private void setButtonsEnabled( bool enabled )
        {
            button1.Enabled = enabled;
            button2.Enabled = enabled;
            button3.Enabled = enabled;
        }

        private delegate void setImportDataTableNameHandler( string ImportDataTableName );
        private void setImportDataTableName( string ImportDataTableName )
        {
            txtImportDataTableName.Text = ImportDataTableName;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            setButtonsEnabled( false );
            log( "Storing data..." );
            ( (WorkerThread.storeDataHandler) _WorkerThread.storeData ).BeginInvoke( txtDataFilePath.Text, null, null );
        }

        private void button2_Click( object sender, EventArgs e )
        {
            setButtonsEnabled( false );
            log( "Importing rows..." );
            Int32 rows = CswConvert.ToInt32( txtRows.Text );
            if( rows < 0 )
            {
                rows = 1;
            }
            ( (WorkerThread.importRowsHandler) _WorkerThread.importRows ).BeginInvoke( txtImportDataTableName.Text, rows, null, null );
        }

        private void button3_Click( object sender, EventArgs e )
        {
            setButtonsEnabled( false );
            log( "Reading bindings..." );
            ( (WorkerThread.readBindingsHandler) _WorkerThread.readBindings).BeginInvoke( txtBindingsFilePath.Text, null, null );
        }
    }
}

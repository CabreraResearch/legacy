using System;
using System.Collections.Specialized;
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
            _WorkerThread.OnError = handleError;
            _WorkerThread.OnMessage = handleMessage;
            _WorkerThread.OnFinish = finish;
            _WorkerThread.OnStoreDataFinish = StoreDataFinished;
            _WorkerThread.OnGetCountsFinish = GetCountsFinished;
        }

        #region NON-UI THREAD 

        private void handleMessage( string Msg )
        {
            txtLog.BeginInvoke( new logHandler( log ), new object[] { Msg } );
        }
        private void handleError( string Msg )
        {
            txtLog.BeginInvoke( new logHandler( log ), new object[] { "ERROR: " + Msg } );
        }

        private void finish()
        {
            txtLog.BeginInvoke( new logHandler( log ), new object[] { "Finished." } );
            button1.BeginInvoke( new setButtonsEnabledHandler( setButtonsEnabled ), new object[] { true } );
        }

        private void StoreDataFinished( StringCollection ImportDataTableNames )
        {
            cbxImportDataTableName.BeginInvoke( new setImportDataTableNameHandler( setImportDataTableName ), new object[] { ImportDataTableNames } );
        }
        
        private void GetCountsFinished( Int32 PendingCount, Int32 ErrorCount )
        {
            lblPending.BeginInvoke( new setCountsHandler( setCounts ), new object[] {PendingCount, ErrorCount} );
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

        private delegate void setImportDataTableNameHandler( StringCollection ImportDataTableNames );
        private void setImportDataTableName( StringCollection ImportDataTableNames )
        {
            cbxImportDataTableName.Items.Clear();
            foreach( string ImportDataTableName in ImportDataTableNames )
            {
                cbxImportDataTableName.Items.Add( ImportDataTableName );
            }
            cbxImportDataTableName.SelectedIndex = 0;
        }

        private delegate void setCountsHandler( Int32 PendingCount, Int32 ErrorCount );
        private void setCounts( Int32 PendingCount, Int32 ErrorCount )
        {
            if( PendingCount != Int32.MinValue )
            {
                lblPending.Text = PendingCount.ToString();
            }
            else
            {
                lblPending.Text = "???";
            }
            if( ErrorCount != Int32.MinValue )
            {
                lblError.Text = ErrorCount.ToString();
            }
            else
            {
                lblError.Text = "???";
            }
        } // setCounts()

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
            ( (WorkerThread.importRowsHandler) _WorkerThread.importRows ).BeginInvoke( cbxImportDataTableName.Text, rows, null, null );
        }

        private void button3_Click( object sender, EventArgs e )
        {
            setButtonsEnabled( false );
            log( "Reading bindings..." );
            ( (WorkerThread.readBindingsHandler) _WorkerThread.readBindings).BeginInvoke( txtBindingsFilePath.Text, null, null );
        }

        private void cbxImportDataTableName_TextChanged( object sender, EventArgs e )
        {
            ( (WorkerThread.getCountsHandler) _WorkerThread.getCounts ).BeginInvoke( cbxImportDataTableName.Text, null, null );
        }
    }
}

using System;
using System.Data;
using System.Windows.Forms;
using ChemSW.Exceptions;
using ChemSW.Nbt.ImportExport;
using Microsoft.VisualBasic.FileIO;

namespace ChemSW.Nbt.Schema
{
    public partial class ImporterForm : Form
    {
        private DataTable _DbInstances = null;
        //private DataTable _DataFilesTable = null;
        private string _ConfigurationPath;
        private string _DataFilePath = string.Empty;
        private WorkerThread _WorkerThread;

        private CswNbtImportStatus _CswNbtImportStatus = null;


        public ImporterForm()
        {
            InitializeComponent();

            _ConfigurationPath = Application.StartupPath + "\\..\\etc";
            if( !FileSystem.DirectoryExists( _ConfigurationPath ) )
            {
                FileSystem.CreateDirectory( _ConfigurationPath );
            }

            _WorkerThread = new WorkerThread( _ConfigurationPath );


            _WorkerThread.OnStatusChange += new WorkerThread.StatusMessageHandler( _WorkerThread_OnStatusChange );
            _WorkerThread.OnImportPhaseChange += new WorkerThread.ImportPhaseMessageHandler( _WorkerThread_OnImportPhaseChange );

            _DbInstances = _WorkerThread.getDbInstances();
            _WorkerThread.AccessId = _DbInstances.Rows[0][WorkerThread.ColName_AccessId].ToString();

            InitSchemaSelectBox.DataSource = _DbInstances;
            InitSchemaSelectBox.ValueMember = WorkerThread.ColName_AccessId;
            InitSchemaSelectBox.DisplayMember = WorkerThread.ColName_Display;
            InitSchemaSelectBox.SelectedIndexChanged += new EventHandler( SchemaSelectBox_SelectedIndexChanged );

            ErrorLabel.Text = string.Empty;

            //_DataFilesTable = _WorkerThread.getDataFilesTable();
            //DataFileSelectBox.DataSource = _DataFilesTable;
            //DataFileSelectBox.ValueMember = WorkerThread.ColName_DataFileFullName;
            //DataFileSelectBox.DisplayMember = WorkerThread.ColName_DataFileName;

            //ConfirmCheckbox.CheckedChanged += new EventHandler( ConfirmCheckbox_CheckedChanged );

            _initModeComboBox();


            _CswNbtImportStatus = _WorkerThread.getThreadSafeImportStatus();

            _refreshStatus();


        }//ctor


        private void _refreshStatus( bool End = false )
        {

            PhaseTextBox.Text = string.Empty;
            PhaseTextBox.AppendText( "Last Status: " + _CswNbtImportStatus.CompletedPhaseDescription );

            if( false == End )
            {

                if( ImportProcessPhase.NothingDoneYet == _CswNbtImportStatus.CompletedProcessPhase )
                {
                    ImportButton.Text = ImportButtonState.Start.ToString();
                    ImportButton.Enabled = true;

                }
                else if( ImportProcessPhase.NothingDoneYet < _CswNbtImportStatus.CompletedProcessPhase &&
                         ImportProcessPhase.PopulatingNbtNodes > _CswNbtImportStatus.CompletedProcessPhase )
                {
                    ImportButton.Text = ImportButtonState.Stop.ToString();
                    ImportButton.Enabled = false;
                }
                else if( ImportProcessPhase.Completed == _CswNbtImportStatus.CompletedProcessPhase )
                {
                    ImportButton.Text = ImportButtonState.Start.ToString();
                    ImportButton.Enabled = true;

                }
            }
            else
            {
                ImportButton.Text = ImportButtonState.Start.ToString();
                ImportButton.Enabled = true;
            }//if-else the import process completed



            /*
            if( _CswNbtImportStatus.CompletedProcessPhase < ImportProcessPhase.PostProcessingNbtNodes )
            {
                ImportButton.Text = ImportButtonState.Stop.ToString();
                ImportButton.Enabled = false;

            }
            else
            {

                if( _CswNbtImportStatus.CompletedProcessPhase != ImportProcessPhase.Completed )
                {
                    ImportButton.Text = ImportButtonState.Stop.ToString();
                    ImportButton.Enabled = true;

                }
                else
                {
                    ImportButton.Text = ImportButtonState.Resume.ToString();
                    ImportButton.Enabled = false;
                }
            }
             */

        }//_refreshStatus() 




        void _WorkerThread_OnImportPhaseChange( CswNbtImportStatus CswNbtImportStatus )
        {
            if( null != CswNbtImportStatus )
            {
                PhaseTextBox.BeginInvoke( new AddImportStatusHandler( _AddImportStatus ), new object[] { CswNbtImportStatus } );

            }
        }//_WorkerThread_OnImportPhaseChange


        private ImportProcessPhase _LastProcessPhase = ImportProcessPhase.NothingDoneYet;
        private delegate void AddImportStatusHandler( CswNbtImportStatus CswNbtImportStatus );


        System.Threading.Timer _ProgressTimer = null;
        private void _addTicTimerCallBack( object state )
        {
            PhaseTextBox.BeginInvoke( new AddTimerTicHandler( _addTimerTicToPhaseStatus ) );
        }//_addTicTimerCallBack()


        private delegate void AddTimerTicHandler();
        private void _addTimerTicToPhaseStatus()
        {
            //PhaseTextBox.Text += " .";
            if( PhaseTextBox.Lines.Length >= 2 )
            {
                PhaseTextBox.Lines[1] += " ."; ;
            }
        }//_addTimerTicToPhaseStatus() 

        private void _AddImportStatus( CswNbtImportStatus CswNbtImportStatus )
        {
            if( _LastProcessPhase != CswNbtImportStatus.TargetProcessPhase )
            {
                _WorkerThread_OnStatusChange( PhaseTextBox.Text );
                _LastProcessPhase = CswNbtImportStatus.TargetProcessPhase;
            }


            if( PhaseTypes.Monolithic == CswNbtImportStatus.PhaseType )
            {
                if( null == _ProgressTimer )
                {
                    _ProgressTimer = new System.Threading.Timer( _addTicTimerCallBack, null, 1000, 1000 );
                }
            }
            else
            {
                if( null != _ProgressTimer )
                {
                    _ProgressTimer = null;
                }
            }

            PhaseTextBox.Clear();
            PhaseTextBox.AppendText( "Current Phase " + ": " + CswNbtImportStatus.TargetPhaseDescription + "\r\n" );
            PhaseTextBox.AppendText( "Status " + ": " + CswNbtImportStatus.PhaseStatus + "\r\n" );

        }



        void _WorkerThread_OnStatusChange( string Msg )
        {
            if( string.Empty != Msg )
            {
                ResultsTextBox.BeginInvoke( new AddStatusMsgHandler( _AddStatusMsg ), new object[] { Msg } );
            }
        }

        private delegate void AddStatusMsgHandler( string Msg );
        private void _AddStatusMsg( string Msg )
        {
            ResultsTextBox.AppendText( DateTime.Now.ToString() + ": " + Msg + "\r\n" );
        }

        void SchemaSelectBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            string AccessId = ( (ComboBox) sender ).SelectedValue.ToString();
            _WorkerThread.AccessId = AccessId;
            ErrorLabel.Text = string.Empty;

            InitSchemaSelectBox.SelectedValue = AccessId;

            _refreshStatus();
        } // InitializerForm()





        void ConfirmCheckbox_CheckedChanged( object sender, EventArgs e )
        {
            //ImportButton.Enabled = ConfirmCheckbox.Checked;
        } // ConfirmCheckbox_CheckedChanged()



        //TODO: Add "nuke current state" button to start over from scratch
        //      Take away checkbox and add confirm diaglogue button.

        private enum ImportButtonState { Start, Stop, Resume }
        private void ImportButton_Click( object sender, EventArgs e )
        {

            if( ImportButtonState.Start.ToString() == ImportButton.Text )
            {

                if( MessageBox.Show( "Do not proceed unless you have made a viable backup of schema  " + InitSchemaSelectBox.Text + "; this action is irreversable. Proceed? ", "Do Import", MessageBoxButtons.YesNo ) == DialogResult.Yes )
                {

                    if( _DataFilePath != string.Empty )
                    {
                        ImportButton.Text = ImportButtonState.Stop.ToString();
                        ImportMode Mode = (ImportMode) Enum.Parse( typeof( ImportMode ), ModeComboBox.SelectedItem.ToString() );

                        ImportTablePopulationMode ImportTablePopulationMode = ImportTablePopulationMode.Unknown;

                        //Post-Ripper style XML
                        //Rapid-Loader Style XLS
                        if( "Rapid-Loader Style XLS" == FileTypeSelectBox.SelectedItem.ToString() )
                        {
                            ImportTablePopulationMode = ImportTablePopulationMode.FromRapidLoaderXls;
                        }
                        else if( "Post-Ripper style XML" == FileTypeSelectBox.SelectedItem.ToString() )
                        {

                            ImportTablePopulationMode = ImportTablePopulationMode.FromXml;
                        }
                        else
                        {
                            throw ( new CswDniException( "Unknown file type: " + FileTypeSelectBox.SelectedItem.ToString() ) );
                        }

                        WorkerThread.ImportHandler ImportHandler = new WorkerThread.ImportHandler( _WorkerThread.DoImport );
                        ImportHandler.BeginInvoke( _DataFilePath, Mode, ImportTablePopulationMode, new AsyncCallback( ImportButton_Callback ), null );
                    }
                    else
                    {
                        _AddStatusMsg( "Error: You must choose a file to import" );
                    }
                }//if the user took responsibility for his actions
            }
            else if( ImportButtonState.Stop.ToString() == ImportButton.Text )
            {
                if( _CswNbtImportStatus.CompletedProcessPhase < ImportProcessPhase.PopulatingImportTableProps )
                {
                    MessageBox.Show( "You cannot halt the import process until after temporary tables are created", "Do Import", MessageBoxButtons.OK );
                }
                else
                {
                    _WorkerThread.stopImport();
                }
            }//if-else-if on importbutton state



        } // ImportButton_Click()

        private void ImportButton_Callback( IAsyncResult Result )
        {
            ImportButton.BeginInvoke( new MethodInvoker( _EndImport ), null );
        }
        private void _EndImport()
        {
            _refreshStatus( true );
            //ImportInProgressLabel.Visible = false;
            //ImportInProgressLabel.Refresh();
            //ImportCompleteLabel.Visible = true;
            //ImportCompleteLabel.Refresh();
            //ConfirmCheckbox.Checked = false;
        } // ImportButton_Callback


        //private void ClearExistingCheckBox_CheckedChanged( object sender, EventArgs e )
        //{
        //    if( ClearExistingCheckBox.Checked )
        //        ClearExistingCheckBox.ForeColor = Color.Red;
        //    else
        //        ClearExistingCheckBox.ForeColor = Color.Black;
        //} // ClearExistingCheckBox_CheckedChanged()



        private void DataFileLink_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            openFileDialog1.FileName = string.Empty;
            if( openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                _DataFilePath = openFileDialog1.FileName;
                DataFileLink.Text = _DataFilePath;
            }
            else
            {
                _DataFilePath = string.Empty;
                DataFileLink.Text = "Choose...";
            }
        }

        private void FileTypeSelectBox_OnChange( object sender, EventArgs e )
        {
            _initModeComboBox();
        }

        private void _initModeComboBox()
        {
            ModeComboBox.Items.Clear();

            //All methods should support these options now
            ModeComboBox.Items.Add( ImportMode.Duplicate.ToString() );
            ModeComboBox.Items.Add( ImportMode.Overwrite.ToString() );
            ModeComboBox.SelectedItem = ImportMode.Duplicate.ToString();

            //switch( FileTypeSelectBox.SelectedItem.ToString() )
            //{
            //    case "IMCS Desktop Export XML File":
            //        ModeComboBox.Items.Add( ImportMode.Duplicate.ToString() );
            //        ModeComboBox.Items.Add( ImportMode.Overwrite.ToString() );
            //        ModeComboBox.SelectedItem = ImportMode.Duplicate.ToString();
            //        break;

            //    default:
            //        ModeComboBox.Items.Add( ImportMode.Duplicate.ToString() );
            //        ModeComboBox.Items.Add( ImportMode.Overwrite.ToString() );
            //        ModeComboBox.Items.Add( ImportMode.Update.ToString() );
            //        ModeComboBox.SelectedItem = ImportMode.Duplicate.ToString();
            //        break;
            //}
        }

        //private void btn_ResetSchema_Click( object sender, EventArgs e )
        //{
        //    if( MessageBox.Show( "Remove temporary import tables and reset the import status on schema " + InitSchemaSelectBox.Text + "; Proceed? ", "Reset", MessageBoxButtons.YesNo ) == DialogResult.Yes )
        //    {
        //        _WorkerThread.reset();
        //        _refreshStatus();
        //    }

        //}

        private void btn_Types_Click( object sender, EventArgs e )
        {
            _WorkerThread.writeNodeTypesAsXml();
        } // FileTypeSelectBox_OnChange


    } // class InitializerForm
} // namespace ChemSW.Nbt.Schema

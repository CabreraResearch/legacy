using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.Config;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.Config;
using ChemSW.Security;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.TreeEvents;

namespace ChemSW.Nbt.Schema
{
    public partial class MainForm : Form
    {
        private string _TargetAccessId
        {
            get { return schemacombobox.SelectedValue.ToString(); }
        }

        private UpdateThread _UpdateThread;

        public MainForm()
        {
            InitializeComponent();
            updatebutton.Visible = false;
            MyCancelButton.Visible = false;

            _UpdateThread = new UpdateThread();
            _UpdateThread.OnFetchSchemata += new UpdateThread.FetchSchemataEventHandler( _UpdateThread_OnFetchSchemata );
            _UpdateThread.OnGetSchemaInfo += new UpdateThread.GetSchemaInfoEventHandler( _UpdateThread_OnGetSchemaInfo );
            _UpdateThread.OnStatusChange += new UpdateThread.StatusChangeEventHandler( _UpdateThread_OnStatusChange );
            _UpdateThread.OnUpdateDone += new UpdateThread.UpdateDoneEventHandler( _UpdateThread_OnUpdateDone );
            _UpdateThread.OnUpdateFailed += new UpdateThread.UpdateFailedEventHandler( _UpdateThread_OnUpdateFailed );
        }

        private void FetchButton_Click( object sender, EventArgs e )
        {
            FetchButton.Enabled = false;
            UpdateThread.FetchInvoker fi = new UpdateThread.FetchInvoker( _UpdateThread.FetchSchemata );
            fi.BeginInvoke( new AsyncCallback( ThreadCallback ), null );
        }

        private delegate void InitSchemaComboBoxHandler( UpdateThread.FetchSchemataEventArgs e );
        private void _InitSchemaComboBox( UpdateThread.FetchSchemataEventArgs e )
        {
            if( e.Succeeded )
            {
                schemacombobox.DataSource = e.DbInstances;
                schemacombobox.ValueMember = UpdateThread._ColName_AccessId;
                schemacombobox.DisplayMember = UpdateThread._ColName_Display;
                schemacombobox.SelectedValueChanged += new EventHandler( schemacombobox_SelectedValueChanged );
                _setSelected();
            }
            else
            {
                MessageBox.Show( e.Message );
            }
        }

        void schemacombobox_SelectedValueChanged( object sender, EventArgs e )
        {
            _setSelected();
        }

        private void _setSelected()
        {
            UpdateThread.GetAccessIdInfoInvoker gaiii = new UpdateThread.GetAccessIdInfoInvoker( _UpdateThread.GetAccessIdInfo );
            gaiii.BeginInvoke( _TargetAccessId, new AsyncCallback( ThreadCallback ), null );
        }

        private delegate void InitSchemaInfoHandler( UpdateThread.SchemaInfoEventArgs e );
        private void _InitSchemaInfo( UpdateThread.SchemaInfoEventArgs e )
        {
            minimumversion.Text = e.MinimumSchemaVersion.ToString();
            currentschemaversion.Text = e.CurrentSchemaVersion.ToString();
            updatetoschemaversion.Text = e.LatestSchemaVersion.ToString();

            _SetUpdateButtonVisible( e );

            historygrid.DataSource = e.UpdateHistoryTable;
            historygrid.Columns["version"].DisplayIndex = 0;
            historygrid.Columns["version"].Width = 50;
            historygrid.Columns["updatedate"].DisplayIndex = 1;
            historygrid.Columns["updatedate"].Width = 120;
            historygrid.Columns["log"].DisplayIndex = 2;
            historygrid.Columns["log"].Width = 200;
        }

        private void updatebutton_Click( object sender, EventArgs e )
        {
            updatebutton.Enabled = false;
            updatebutton.Refresh();
            MyCancelButton.Visible = true;
            MyCancelButton.Enabled = true;
            schemacombobox.Enabled = false;

            UpdateThread.DoUpdateInvoker dui = new UpdateThread.DoUpdateInvoker( _UpdateThread.DoUpdate );
            dui.BeginInvoke( _TargetAccessId, new AsyncCallback( ThreadCallback ), null );
        }

        private delegate void ReadyForUpdateHandler( UpdateThread.SchemaInfoEventArgs e );
        private void _ReadyForUpdate( UpdateThread.SchemaInfoEventArgs e )
        {
            _UpdateThread.Cancel = false;
            MyCancelButton.Enabled = true;
            MyCancelButton.Visible = false;
            updatebutton.Enabled = true;
            schemacombobox.Enabled = true;

            _SetUpdateButtonVisible( e );
        }

        private delegate void ShowClipboardLinkHandler();
        private void _ShowClipboardLink()
        {
            ClipBoardLink.Visible = true;
        }

        private void _SetUpdateButtonVisible( UpdateThread.SchemaInfoEventArgs e )
        {
            if( e.CurrentSchemaVersion == e.MinimumSchemaVersion ||
                ( e.LatestSchemaVersion.CycleIteration == e.CurrentSchemaVersion.CycleIteration &&
                  e.LatestSchemaVersion.ReleaseIdentifier == e.CurrentSchemaVersion.ReleaseIdentifier &&
                  e.LatestSchemaVersion.ReleaseIteration > e.CurrentSchemaVersion.ReleaseIteration ) )
            {
                updatebutton.Visible = true;
            }
            else
            {
                updatebutton.Visible = false;
            }
        }

        private delegate void SetStatusHandler( string Message );
        private void _SetStatus( string Message )
        {
            StatusTextBox.Text = DateTime.Now.ToString() + ": " + Message + "\r\n" + StatusTextBox.Text;
        }


        #region Non-UI Thread

        void _UpdateThread_OnFetchSchemata( UpdateThread.FetchSchemataEventArgs e )
        {
            schemacombobox.BeginInvoke( new InitSchemaComboBoxHandler( _InitSchemaComboBox ), new object[] { e } );
        }

        void _UpdateThread_OnGetSchemaInfo( UpdateThread.SchemaInfoEventArgs e )
        {
            historygrid.BeginInvoke( new InitSchemaInfoHandler( _InitSchemaInfo ), new object[] { e } );
        }

        void _UpdateThread_OnStatusChange( UpdateThread.StatusChangeEventArgs e )
        {
            StatusTextBox.BeginInvoke( new SetStatusHandler( _SetStatus ), new object[] { e.Message } );
        }

        void _UpdateThread_OnUpdateDone( UpdateThread.SchemaInfoEventArgs e )
        {
            updatebutton.BeginInvoke( new ReadyForUpdateHandler( _ReadyForUpdate ), new object[] { e } );
        }

        void _UpdateThread_OnUpdateFailed()
        {
            ClipBoardLink.BeginInvoke( new ShowClipboardLinkHandler( _ShowClipboardLink ), null );
        }

        private void ThreadCallback( IAsyncResult Result )
        {
            _UpdateThread.Cancel = false;
        }

        #endregion Non-UI Thread

        private void CancelButton_Click( object sender, EventArgs e )
        {
            MyCancelButton.Enabled = false;
            _UpdateThread.Cancel = true;
        }

        private void ClipBoardLink_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            // Set the clipboard to include the first row's 'log' column
            Clipboard.SetText( historygrid.Rows[0].Cells["log"].Value.ToString() );
        }

    } // class Form1
} // namespace ChemSW.Nbt.Schema

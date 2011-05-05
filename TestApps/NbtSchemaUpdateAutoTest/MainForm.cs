using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChemSW.Nbt.Schema
{
    public partial class MainForm : Form
    {
        CswScmUpdtAutoTest _CswSchemaUpdaterAutoTest = null;
        public MainForm()
        {
            InitializeComponent();

            _CswSchemaUpdaterAutoTest = new CswScmUpdtAutoTest();

            _CswSchemaUpdaterAutoTest.WriteMessage += new CswScmUpdtAutoTest.UpdateTestWriteMessage( _reportMessage );

            foreach ( string CurrentName in _CswSchemaUpdaterAutoTest.Names )
            {
                cbxl_TestToRun.Items.Add( CurrentName, true );
            }

        }//ctor

        private void _reportMessage( string Message )
        {
            tbxMessages.AppendText( Message + "\r\n" );
        }

        private void btn_Test_Click( object sender, EventArgs e )
        {
            List<string> NamesOfTestCasesToRun = new List<string>();


            for ( int idx = 0; idx < cbxl_TestToRun.Items.Count; idx++ )
            {
                if ( cbxl_TestToRun.GetItemChecked( idx ) )
                {
                    NamesOfTestCasesToRun.Add( cbxl_TestToRun.GetItemText( cbxl_TestToRun.Items[ idx ] ) );
                }
            }

            _CswSchemaUpdaterAutoTest.runTests( NamesOfTestCasesToRun );
        }

        private void btn_CheckAll_Click( object sender, EventArgs e )
        {

            for ( int idx = 0; idx < cbxl_TestToRun.Items.Count; idx++ )
            {
                cbxl_TestToRun.SetItemChecked( idx, true );
            }

        }

        private void btn_UncheckAll_Click( object sender, EventArgs e )
        {
            foreach ( int CurrentIdx in cbxl_TestToRun.CheckedIndices )
            {
                cbxl_TestToRun.SetItemChecked( CurrentIdx, false );
            }
        }//_reportMessage()

    }
}

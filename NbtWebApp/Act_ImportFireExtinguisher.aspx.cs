using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using FarPoint.Web.Spread;
using ChemSW.CswWebControls;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_ImportFireExtinguisher : System.Web.UI.Page
    {
        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( ImportSpread, Master.ErrorBox );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        private CswWizard _Wizard;
        protected override void CreateChildControls()
        {
            _Wizard = new CswWizard();
            _Wizard.ID = "ImportFEDataWizard";
            _Wizard.WizardTitle = "Import Fire Extinguisher Data";
            _Wizard.onCancel += new CswWizard.CswWizardEventHandler( _Wizard_onCancel );
            _Wizard.OnError += new CswErrorHandler( Master.HandleError );
            _Wizard.onPageChange += new CswWizard.CswWizardEventHandler( _Wizard_onPageChange );
            _Wizard.onFinish += new CswWizard.CswWizardEventHandler( _Wizard_onFinish );

            LayoutImportSpreadsheet();

            ph.Controls.Add( _Wizard );
        }

        private FpSpread ImportSpread;
        private SheetView Sheet1;
        private CheckBox AssignNextCheckBox;

        private enum ImportColumns
        {
            Building,
            Floor,
            Room,
            Mount_Point_Description,
            Mount_Point_Group,
            Mount_Point_Barcode,
            Type,
            Last_Inspection_Date,
            Last_Inspection_Status,
            Fire_Extinguisher_Description,
            Fire_Extinguisher_Barcode,
            Fire_Extinguisher_Manufacturer,
            Fire_Extinguisher_Model,
            Fire_Extinguisher_Size,
            Fire_Extinguisher_Size_Unit
        }
        private static string ImportColumnsToDisplayString( ImportColumns Column )
        {
            return Column.ToString().Replace( '_', ' ' );
        }
        private static ImportColumns ImportColumnsFromDisplayString( string Column )
        {
            return (ImportColumns) Enum.Parse( typeof( ImportColumns ), Column.Replace( ' ', '_' ), true );
        }

        private Int32 numColumns = 15;
        
        private void LayoutImportSpreadsheet()
        {
            CswWizardStep ImportDataStep = new CswWizardStep();
            ImportDataStep.ID = "ImportFEDataWizard_ImportDataStep";
            ImportDataStep.Step = 2;
            ImportDataStep.Title = "Locations, Mount Points and Fire Extinguishers";
            _Wizard.WizardSteps.Add( ImportDataStep );

            CswAutoTable ImportDataStepTable = new CswAutoTable();
            ImportDataStepTable.ID = "ImportDataStepTable";
            ImportDataStep.Controls.Add( ImportDataStepTable );

            ImportSpread = new FpSpread();
            ImportSpread.ID = "ImportSpread";
            ImportSpread.Height = 340;
            ImportSpread.Width = 600;
            ImportSpread.EditModeReplace = true;
            ImportSpread.ActiveSheetViewIndex = 0;
            ImportSpread.EnableAjaxCall = true;
            ImportSpread.UpdateCommand += new SpreadCommandEventHandler( QuestionSpread_UpdateCommand );
            ImportDataStepTable.addControl( 0, 0, ImportSpread );

            Sheet1 = new SheetView();
            Sheet1.SheetName = "Sheet1";
            Sheet1.AllowInsert = true;
            Sheet1.AllowDelete = true;
            Sheet1.AutoGenerateColumns = false;
            ImportSpread.Sheets.Add( Sheet1 );

            // clear the sheet
            Sheet1.RemoveRows( 0, Sheet1.Rows.Count );
            Sheet1.RemoveColumns( 0, Sheet1.Columns.Count );

            Sheet1.SelectionBackColor = System.Drawing.Color.FromArgb( 255, 255, 0 );
            Sheet1.LockBackColor = System.Drawing.Color.FromArgb( 200, 200, 200 );

            Sheet1.Columns.Add( 0, numColumns );
            Sheet1.Rows.Count = 200;
            Sheet1.PageSize = 500;

            Sheet1.Columns[(Int32) ImportColumns.Building].Label = ImportColumnsToDisplayString( ImportColumns.Building );
            Sheet1.Columns[(Int32) ImportColumns.Floor].Label = ImportColumnsToDisplayString( ImportColumns.Floor );
            Sheet1.Columns[(Int32) ImportColumns.Room].Label = ImportColumnsToDisplayString( ImportColumns.Room );
            Sheet1.Columns[(Int32) ImportColumns.Mount_Point_Description].Label = ImportColumnsToDisplayString( ImportColumns.Mount_Point_Description );
            Sheet1.Columns[(Int32) ImportColumns.Mount_Point_Group].Label = ImportColumnsToDisplayString( ImportColumns.Mount_Point_Group );
            Sheet1.Columns[(Int32) ImportColumns.Mount_Point_Barcode].Label = ImportColumnsToDisplayString( ImportColumns.Mount_Point_Barcode );
            Sheet1.Columns[(Int32) ImportColumns.Type].Label = ImportColumnsToDisplayString( ImportColumns.Type );
            Sheet1.Columns[(Int32) ImportColumns.Last_Inspection_Date].Label = ImportColumnsToDisplayString( ImportColumns.Last_Inspection_Date );
            Sheet1.Columns[(Int32) ImportColumns.Last_Inspection_Status].Label = ImportColumnsToDisplayString( ImportColumns.Last_Inspection_Status );
            Sheet1.Columns[(Int32) ImportColumns.Fire_Extinguisher_Description].Label = ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Description );
            Sheet1.Columns[(Int32) ImportColumns.Fire_Extinguisher_Barcode].Label = ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Barcode );
            Sheet1.Columns[(Int32) ImportColumns.Fire_Extinguisher_Manufacturer].Label = ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Manufacturer );
            Sheet1.Columns[(Int32) ImportColumns.Fire_Extinguisher_Model].Label = ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Model );
            Sheet1.Columns[(Int32) ImportColumns.Fire_Extinguisher_Size].Label = ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Size );
            Sheet1.Columns[(Int32) ImportColumns.Fire_Extinguisher_Size_Unit].Label = ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Size_Unit );

        }

        protected override void OnPreRender( EventArgs e )
        {
            if( _Wizard.CurrentStep == 2 )
                _Wizard.NextButton.OnClientClick = "return Spread_Update_Click('" + ImportSpread.ClientID + "_Update');";

            base.OnPreRender( e );
        }

        #endregion Page Lifecycle

        #region Events

        protected void QuestionSpread_UpdateCommand( object sender, SpreadCommandEventArgs e )
        {
            try
            {
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void _Wizard_onPageChange( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                if( CswWizardEventArgs.NewPage == 2 )
                {
                    if( NewNameBox.Text == string.Empty || Master.CswNbtResources.MetaData.getNodeType( NewNameBox.Text ) != null )
                    {
                        Label WarningLabel = new Label();
                        WarningLabel.CssClass = "ErrorContent";
                        WarningLabel.Text = "You must supply a unique name for the inspection design";
                        InspectionDetailsStep.Controls.Add( WarningLabel );
                        _Wizard.CurrentStep = 1;
                    }
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

        }

        void _Wizard_onCancel( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                Master.Redirect( "Main.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }



        void _Wizard_onFinish( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
         
            }
            catch( Exception ex )
            {
                // rollback
                Master.CswNbtResources.Rollback();
                _Wizard.CurrentStep = 2;
                Master.HandleError( ex );
            }
        }

        #endregion Events

    } // class Act_ImportFireExtinguisher
} // namespace ChemSW.Nbt.WebPages

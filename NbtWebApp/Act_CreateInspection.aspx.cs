using System;
using System.Collections;
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
    public partial class Act_CreateInspection : System.Web.UI.Page
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
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( QuestionSpread, Master.ErrorBox );
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
            _Wizard.ID = "EditInspectionWizard";
            _Wizard.WizardTitle = "Create Inspection";
            _Wizard.onCancel += new CswWizard.CswWizardEventHandler( _Wizard_onCancel );
            _Wizard.OnError += new CswErrorHandler( Master.HandleError );
            _Wizard.onPageChange += new CswWizard.CswWizardEventHandler( _Wizard_onPageChange );
            _Wizard.onFinish += new CswWizard.CswWizardEventHandler( _Wizard_onFinish );

            CreateInspectionDetailsStep();
            CreateSectionsAndQuestionsStep();

            ph.Controls.Add( _Wizard );
        }

        private TextBox NewNameBox;
        private DropDownList FilteredTargetDropDown;
        private DropDownList AllTargetDropDown;
        private CheckBox ShowAllCheckBox;
        private CheckBox NameTemplateDueDateCheckBox;
        private CheckBox NameTemplateRouteCheckBox;
        private CheckBox NameTemplateTargetCheckBox;
        private CheckBox NameTemplateNameCheckBox;
        private CswWizardStep InspectionDetailsStep;

        private void CreateInspectionDetailsStep()
        {
            InspectionDetailsStep = new CswWizardStep();
            InspectionDetailsStep.ID = "EditInspectionWizard_InspectionDetailsStep";
            InspectionDetailsStep.Step = 1;
            InspectionDetailsStep.Title = "Inspection Details";
            _Wizard.WizardSteps.Add( InspectionDetailsStep );

            CswAutoTable InspectionDetailsStepTable = new CswAutoTable();
            InspectionDetailsStepTable.ID = "InspectionDetailsStepTable";
            InspectionDetailsStepTable.OddCellRightAlign = true;
            InspectionDetailsStep.Controls.Add( InspectionDetailsStepTable );

            // Name
            Literal NameLiteral = new Literal();
            NameLiteral.Text = "New Design Name:";
            InspectionDetailsStepTable.addControl( 0, 0, NameLiteral );

            NewNameBox = new TextBox();
            NewNameBox.ID = "NewInspectionName";
            NewNameBox.Width = Unit.Parse( "150px" );
            NewNameBox.CssClass = "textinput";
            InspectionDetailsStepTable.addControl( 0, 1, NewNameBox );

            // Target
            Literal TargetLiteral = new Literal();
            TargetLiteral.Text = "Target:";
            InspectionDetailsStepTable.addControl( 1, 0, TargetLiteral );

            CswAutoTable TargetTable = new CswAutoTable();
            TargetTable.ID = "TargetTable";
            InspectionDetailsStepTable.addControl( 1, 1, TargetTable );

            FilteredTargetDropDown = new DropDownList();
            FilteredTargetDropDown.ID = "TargetDropDown";
            FilteredTargetDropDown.CssClass = "selectinput";
            AllTargetDropDown = new DropDownList();
            AllTargetDropDown.ID = "AllTargetDropDown";
            AllTargetDropDown.CssClass = "selectinput";
            AllTargetDropDown.Style.Add( HtmlTextWriterStyle.Display, "none" );
            
            //When implementing case 20051 or in general, consider adding property sets into metadata to improve the quality of this code
            foreach( CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                AllTargetDropDown.Items.Add( new System.Web.UI.WebControls.ListItem( NodeType.NodeTypeName, "nt_" + NodeType.FirstVersionNodeTypeId.ToString() ) );
                if( NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass ||
                    NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass )
                {
                    FilteredTargetDropDown.Items.Add( new System.Web.UI.WebControls.ListItem( NodeType.NodeTypeName, "nt_" + NodeType.FirstVersionNodeTypeId.ToString() ) );
                }
            }
            foreach( CswNbtMetaDataObjectClass ObjectClass in Master.CswNbtResources.MetaData.ObjectClasses )
            {
                AllTargetDropDown.Items.Add( new System.Web.UI.WebControls.ListItem( "Any " + ObjectClass.ObjectClass, "oc_" + ObjectClass.ObjectClassId.ToString() ) );
                if( ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass ||
                    ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass )
                {
                    FilteredTargetDropDown.Items.Add( new System.Web.UI.WebControls.ListItem( "Any " + ObjectClass.ObjectClass, "oc_" + ObjectClass.ObjectClassId.ToString() ) );
                }
            }
            TargetTable.addControl( 2, 1, FilteredTargetDropDown );
            TargetTable.addControl( 2, 1, AllTargetDropDown );

            ShowAllCheckBox = new CheckBox();
            ShowAllCheckBox.ID = "ShowAllCheckBox";
            ShowAllCheckBox.Text = "Show All";
            TargetTable.addControl( 2, 2, ShowAllCheckBox );

            // Name Template
            Literal NameTemplateLiteral = new Literal();
            NameTemplateLiteral.Text = "Inspection Name Template:";
            InspectionDetailsStepTable.addControl( 2, 0, NameTemplateLiteral );

            CswAutoTable NameTemplateTable = new CswAutoTable();
            NameTemplateTable.ID = "NameTemplateTable";
            InspectionDetailsStepTable.addControl( 2, 1, NameTemplateTable );

            NameTemplateNameCheckBox = new CheckBox();
            NameTemplateNameCheckBox.ID = "NameTemplateNameCheckBox";
            NameTemplateNameCheckBox.Text = "Design Name";
            NameTemplateNameCheckBox.Checked = true;
            NameTemplateTable.addControl( 0, 0, NameTemplateNameCheckBox );

            NameTemplateTargetCheckBox = new CheckBox();
            NameTemplateTargetCheckBox.ID = "NameTemplateTargetCheckBox";
            NameTemplateTargetCheckBox.Text = "Target";
            NameTemplateTable.addControl( 1, 0, NameTemplateTargetCheckBox );

            NameTemplateDueDateCheckBox = new CheckBox();
            NameTemplateDueDateCheckBox.ID = "NameTemplateDueDateCheckBox";
            NameTemplateDueDateCheckBox.Text = "Due Date";
            NameTemplateDueDateCheckBox.Checked = true;
            NameTemplateTable.addControl( 2, 0, NameTemplateDueDateCheckBox );

            NameTemplateRouteCheckBox = new CheckBox();
            NameTemplateRouteCheckBox.ID = "NameTemplateRouteCheckBox";
            NameTemplateRouteCheckBox.Text = "Route Name";
            NameTemplateTable.addControl( 3, 0, NameTemplateRouteCheckBox );
        }

        private FpSpread QuestionSpread;
        private SheetView Sheet1;
        private CheckBox AssignNextCheckBox;

        private Int32 SectionNameColumn = 0;
        private Int32 QuestionColumn = 1;
        private Int32 FieldTypeColumn = 2;
        private Int32 OptionsColumn = 3;
        private Int32 numColumns = 4;

        private void CreateSectionsAndQuestionsStep()
        {
            CswWizardStep SectionsAndQuestionsStep = new CswWizardStep();
            SectionsAndQuestionsStep.ID = "EditInspectionWizard_SectionsAndQuestionsStep";
            SectionsAndQuestionsStep.Step = 2;
            SectionsAndQuestionsStep.Title = "Sections and Questions";
            _Wizard.WizardSteps.Add( SectionsAndQuestionsStep );

            CswAutoTable SectionsAndQuestionsStepTable = new CswAutoTable();
            SectionsAndQuestionsStepTable.ID = "SectionsAndQuestionsStepTable";
            SectionsAndQuestionsStep.Controls.Add( SectionsAndQuestionsStepTable );

            QuestionSpread = new FpSpread();
            QuestionSpread.ID = "QuestionSpread";
            QuestionSpread.Height = 340;
            QuestionSpread.Width = 600;
            QuestionSpread.EditModeReplace = true;
            QuestionSpread.ActiveSheetViewIndex = 0;
            QuestionSpread.EnableAjaxCall = true;
            QuestionSpread.UpdateCommand += new SpreadCommandEventHandler( QuestionSpread_UpdateCommand );
            SectionsAndQuestionsStepTable.addControl( 0, 0, QuestionSpread );

            Sheet1 = new SheetView();
            Sheet1.SheetName = "Sheet1";
            Sheet1.AllowInsert = true;
            Sheet1.AllowDelete = true;
            Sheet1.AutoGenerateColumns = false;
            QuestionSpread.Sheets.Add( Sheet1 );

            // clear the sheet
            Sheet1.RemoveRows( 0, Sheet1.Rows.Count );
            Sheet1.RemoveColumns( 0, Sheet1.Columns.Count );

            Sheet1.SelectionBackColor = System.Drawing.Color.FromArgb( 255, 255, 0 );
            Sheet1.LockBackColor = System.Drawing.Color.FromArgb( 200, 200, 200 );

            Sheet1.Columns.Add( 0, numColumns );
            Sheet1.Rows.Count = 200;
            Sheet1.PageSize = 500;

            Sheet1.Columns[SectionNameColumn].Label = "Section Name";
            Sheet1.Columns[QuestionColumn].Label = "Question";
            Sheet1.Columns[QuestionColumn].Width = 350;
            Sheet1.Columns[FieldTypeColumn].Label = "Field Type";
            Sheet1.Columns[OptionsColumn].Label = "List Options";
            Sheet1.Columns[OptionsColumn].Width = 150;

            string FieldTypes = string.Empty;
            foreach( CswNbtMetaDataFieldType FieldType in Master.CswNbtResources.MetaData.FieldTypes )
            {
                if( FieldType.IsSimpleType() && FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Static )
                {
                    if( FieldTypes != string.Empty ) FieldTypes += ",";
                    FieldTypes += FieldType.FieldType.ToString();
                }
            }
            string[] FieldTypesStringArray = CswTools.SplitAndTrim( FieldTypes, ',' );
            FarPoint.Web.Spread.ComboBoxCellType FieldTypeCell = new FarPoint.Web.Spread.ComboBoxCellType();
            FieldTypeCell.Items = FieldTypesStringArray;
            FieldTypeCell.Values = FieldTypesStringArray;
            Sheet1.Columns[FieldTypeColumn].CellType = FieldTypeCell;
            
            AssignNextCheckBox = new CheckBox();
            AssignNextCheckBox.ID = "AssignNextCheckBox";
            AssignNextCheckBox.Text = "Assign Inspection Next";
            AssignNextCheckBox.Checked = true;
            SectionsAndQuestionsStepTable.addControl( 1, 0, AssignNextCheckBox );
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( _Wizard.CurrentStep == 2 )
                _Wizard.NextButton.OnClientClick = "return Spread_Update_Click('" + QuestionSpread.ClientID + "_Update');";

            ShowAllCheckBox.Attributes.Add( "onclick", "CreateInspection_ShowAll_Click(this, '" + FilteredTargetDropDown.ClientID + "','" + AllTargetDropDown.ClientID + "')" );

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
                Master.GoMain();
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
                // Save the new Inspection
                CswNbtMetaDataObjectClass InspectionObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                CswNbtMetaDataNodeType NewInspectionNodeType = Master.CswNbtResources.MetaData.makeNewNodeType( InspectionObjectClass.ObjectClassId, NewNameBox.Text, string.Empty );

                // Get rid of the automatically created Section in this case
                Master.CswNbtResources.MetaData.DeleteNodeTypeTab( NewInspectionNodeType.getNodeTypeTab( "Section 1" ) );

                // Override the automatically created name template
                string NewNameTemplateText = string.Empty;
                if( NameTemplateNameCheckBox.Checked )
                {
                    if( NewNameTemplateText != string.Empty )
                        NewNameTemplateText += " ";
                    NewNameTemplateText += CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInspectionDesign.NamePropertyName );
                }
                if( NameTemplateTargetCheckBox.Checked )
                {
                    if( NewNameTemplateText != string.Empty )
                        NewNameTemplateText += " ";
                    NewNameTemplateText += CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInspectionDesign.TargetPropertyName );
                }
                if( NameTemplateDueDateCheckBox.Checked )
                {
                    if( NewNameTemplateText != string.Empty )
                        NewNameTemplateText += " ";
                    NewNameTemplateText += CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInspectionDesign.DatePropertyName );
                }
                if( NameTemplateRouteCheckBox.Checked )
                {
                    if( NewNameTemplateText != string.Empty )
                        NewNameTemplateText += " ";
                    NewNameTemplateText += CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInspectionDesign.RoutePropertyName );
                }
                NewInspectionNodeType.NameTemplateText = NewNameTemplateText;

                // Set the target nodeType of the Target relationship property
                string NewFKType = CswNbtViewRelationship.RelatedIdType.Unknown.ToString();
                Int32 NewFKValue = Int32.MinValue;
                
                DropDownList TargetDropDown = null;
                if( ShowAllCheckBox.Checked )
                    TargetDropDown = AllTargetDropDown;
                else
                    TargetDropDown = FilteredTargetDropDown;

                if( TargetDropDown.SelectedValue.Substring( 0, "nt_".Length ) == "nt_" )
                {
                    NewFKType = CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString();
                    NewFKValue = CswConvert.ToInt32( TargetDropDown.SelectedValue.Substring( "nt_".Length ) );
                }
                else
                {
                    NewFKType = CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString();
                    NewFKValue = CswConvert.ToInt32( TargetDropDown.SelectedValue.Substring( "oc_".Length ) );
                }
                CswNbtMetaDataNodeTypeProp TargetProperty = NewInspectionNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                TargetProperty.SetFK(NewFKType, NewFKValue,string.Empty, Int32.MinValue);

                // Setup Sections
                if(Sheet1.Cells[0, SectionNameColumn].Text == string.Empty)
                    throw new CswDniException( "The first row must have a Section Name", "User must supply a Section Name in the first row" );

                Int32 CurrentTabOrder = 1;
                for( Int32 r = 0; r < Sheet1.Rows.Count; r++ )
                {
                    string ThisTabName = Sheet1.Cells[r, SectionNameColumn].Text;
                    if( ThisTabName != string.Empty )
                    {
                        CswNbtMetaDataNodeTypeTab CurrentTab = NewInspectionNodeType.getNodeTypeTab( ThisTabName );
                        if( CurrentTab == null )
                        {
                            CurrentTab = Master.CswNbtResources.MetaData.makeNewTab( NewInspectionNodeType, ThisTabName, CurrentTabOrder );
                            CurrentTabOrder++;
                        }
                    }
                }
                    
                // Setup Questions
                string PriorTabName = string.Empty;
                for( Int32 r = 0; r < Sheet1.Rows.Count; r++ )
                {
                    // Gather data
                    string ThisTabName = Sheet1.Cells[r, SectionNameColumn].Text;
                    if( ThisTabName == string.Empty )
                        ThisTabName = PriorTabName;
                    if( ThisTabName != PriorTabName )
                        PriorTabName = ThisTabName;

                    string ThisQuestion = Sheet1.Cells[r, QuestionColumn].Text;
                    string ThisFieldTypeString = Sheet1.Cells[r, FieldTypeColumn].Text;
                    string ThisListOptions = Sheet1.Cells[r, OptionsColumn].Text;

                    if( ThisQuestion != string.Empty )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab = NewInspectionNodeType.getNodeTypeTab( ThisTabName );
                        CswNbtMetaDataFieldType ThisFieldType = Master.CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Text );
                        if( ThisFieldTypeString != string.Empty )
                            ThisFieldType = Master.CswNbtResources.MetaData.getFieldType( (CswNbtMetaDataFieldType.NbtFieldType) Enum.Parse( typeof( CswNbtMetaDataFieldType.NbtFieldType ), ThisFieldTypeString ) );

                        // Make the new property
                        // Display row/column are set here too
                        CswNbtMetaDataNodeTypeProp ThisProp = Master.CswNbtResources.MetaData.makeNewProp( NewInspectionNodeType, ThisFieldType.FieldTypeId, ThisQuestion, ThisTab.TabId );

                        // Set list options
                        ThisProp.ListOptions = ThisListOptions;
                    }
                }

                if( AssignNextCheckBox.Checked )
                {
                    // Go directly to the Assign Inspection workflow
                    Session["AssignInspection_SelectedInspectionId"] = NewInspectionNodeType.NodeTypeId;
                    Master.Redirect( Master.CswNbtResources.Actions[CswNbtActionName.Assign_Inspection].Url );
                }
                else
                {
                    // Go to the Inspection Designer for the new Inspection
                    Session["Design_SelectedType"] = CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType.ToString();
                    Session["Design_SelectedValue"] = NewInspectionNodeType.NodeTypeId;
                    Master.Redirect( "Design.aspx?mode=i" );
                }

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

    } // class Act_CreateInspection
} // namespace ChemSW.Nbt.WebPages

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using Telerik.Web.UI;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class EditView : System.Web.UI.Page
    {
        private CswViewEditorWizard _ViewEditorWizard = null;

        public void setViewId( Int32 ViewId, bool ForceReload )
        {
            Master.setViewId( ViewId, ForceReload );
        }

        private string ReturnTo = string.Empty;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                if( Request.QueryString["viewid"] != null && Request.QueryString["viewid"].ToString() != string.Empty )
                {
                    if( CswConvert.ToInt32( Request.QueryString["viewid"] ) > 0 )
                    {
						CswNbtView View = Master.CswNbtResources.ViewSelect.restoreView( CswConvert.ToInt32( Request.QueryString["viewid"].ToString() ) );
                        _ViewEditorWizard = new CswViewEditorWizard( Master.CswNbtResources, View, null, Master.AjaxManager );
                    }
                }
                if( _ViewEditorWizard == null )
                    _ViewEditorWizard = new CswViewEditorWizard( Master.CswNbtResources, Master.AjaxManager );

                _ViewEditorWizard.ID = "ViewEditorWizard";
                _ViewEditorWizard.OnError += new CswErrorHandler( Master.HandleError );
                _ViewEditorWizard.onCancel += new CswViewEditorWizard.ViewEditorEvent( HandleCancel );
                _ViewEditorWizard.onFinish += new CswViewEditorWizard.ViewEditorEvent( HandleFinish );
                ViewEditorManagerPlaceholder.Controls.Add( _ViewEditorWizard );

                if( Request.QueryString["sa"] != null && Request.QueryString["sa"].ToString() == "1" )
                    _ViewEditorWizard.IncludeAllCheckBox.Checked = true;

                if( Request.QueryString["step"] != null && CswTools.IsInteger( Request.QueryString["step"].ToString() ) )
                {
                    _ViewEditorWizard.MinimumStep = CswConvert.ToInt32( Request.QueryString["step"].ToString() );
                    _ViewEditorWizard.CurrentStep = CswConvert.ToInt32( Request.QueryString["step"].ToString() );
                }

                if( Request.QueryString["return"] != null && Request.QueryString["return"].ToString() != string.Empty )
                {
                    ReturnTo = CswTools.QueryStringParamToUrl( Request.QueryString["return"].ToString() );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        protected void HandleCancel()
        {
            try
            {
                if( ReturnTo != string.Empty )
                    Master.Redirect( ReturnTo );
                else
                    Master.GoMain();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }
        protected void HandleFinish()
        {
            try
            {
                Master.HandleViewEditorFinish( _ViewEditorWizard.SelectedView );

                _ViewEditorWizard.SelectedView.save();
                if( _ViewEditorWizard.SelectedView.Visibility != NbtViewVisibility.Property )
                    setViewId( _ViewEditorWizard.SelectedView.ViewId, true );

                if( ReturnTo != string.Empty )
                    Master.Redirect( ReturnTo );
                else
                    Master.GoMain();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

    }//EditView
}//ChemSW.Nbt.WebPages



namespace ChemSW.NbtWebControls
{
    public class CswViewEditorWizard : CompositeControl, IPostBackDataHandler
    {
        #region Variables and Properties


        public Int32 PreviewPageSize = 5;


        public bool AllowSetName = true;
        public bool AllowSetMode = true;
        public bool AllowSetAddChildren = true;
        public bool AllowSetAllowDelete = true;
        public bool AllowEdit = true;
        public bool AllowCreate = true;
        public bool AllowDelete = true;
        public bool AllowLoad = true;

        public bool Debug = false;


        private CswNbtResources _CswNbtResources;
        private RadAjaxManager _AjaxMgr;

        private CswWizard _Wizard;
        private CswNbtView _View;
        public CswNbtView SelectedView { get { return _View; } set { _View = value; } }

        private HiddenField _Hidden_CurrentViewXml;
        private Button _HiddenRemoveButton;
        private HiddenField _HiddenNodeToRemoveField;
        private RadTreeView _PreviewTree;
        private CswNodesGrid _PreviewGrid;
        private CswNodesList _PreviewList;
        private Label _PreviewLabel;
        private TableCell _PreviewCell;

        // SelectViewStep
        private CswWizardStep _SelectViewStep;
        private CswAutoTable _SelectViewStepTable;
        private DropDownList _LoadViewList;
        private Label _LoadViewLabel;
        private Button _DeleteViewButton;
        private HtmlGenericControl _HiddenDiv;
        private Button _RealDeleteViewButton;
        private Button _CreateNewViewButton;
        private CheckBox _IncludeAllCheckBox;
        public CheckBox IncludeAllCheckBox { get { EnsureChildControls(); return _IncludeAllCheckBox; } }

        // ViewAttributesStep
        private CswWizardStep _ViewAttributesStep;
        private CswAutoTable _ViewAttributesStepTable;
        private Label _ViewNameLabel;
        private TextBox _ViewNameTextBox;
        private Label _ModeLabel;
        private Label _ModeValueLabel;
        //private DropDownList _ModeDropDown;
        private Label _GridWidthLabel;
        private TextBox _GridWidthBox;
        //private Label _GridEditModeLabel;
        //private DropDownList _GridEditModeDropDown;
        private Label _ViewCategoryLabel;
        private TextBox _ViewCategoryTextBox;
        private Label _ViewVisibilityLabel;
        private CswViewVisibilityEditor _ViewVisibilityEditor;
        //private Literal _IncludeInQuickLaunchLiteral;
        //private CheckBox _IncludeInQuickLaunch;
        private CheckBox _ForMobileCheckBox;
        private Label _ForMobileLabel;
        
        // SetRelationshipsStep
        private CswWizardStep _SetRelationshipsStep;
        private CswAutoTable _SetRelationshipsStepTable;
        private CswAutoTable _SetRelationshipsStepSubTable;
        private CswViewStructureTree _RelationshipsViewTree;
        private Label _NextOptionsLabel;
        private DropDownList _NextOptions;
        private Button _AddButton;
        //private Label _AllowAddLabel;
        //private DropDownList _AllowAddDropDown;
        private Label _AllowDeleteLabel;
        private CheckBox _AllowDeleteCheckBox;
        private Label _EditSectionLabel;

        // SelectPropertiesStep 
        private CswWizardStep _SelectPropertiesStep;
        private CswAutoTable _SelectPropertiesStepTable;
        private CswAutoTable _SelectPropertiesStepSubTable;
        private CswViewStructureTree _PropertiesViewTree;
        private Label _PropLabel;
        //private ListBox _PropList;
        private CswCheckBoxArray _PropCheckBoxArray;
        private DataTable _PropDataTable;
        private Button _AddPropButton;
        private Label _SortByLabel;
        private CheckBox _SortByCheckBox;
        private DropDownList _SortByDropDown;
        private Label _GridOrderLabel;
        private TextBox _GridOrderBox;
        private Label _GridColumnWidthLabel;
        private TextBox _GridColumnWidthBox;
        private Button _ApplyPropButton;
        private Label _ShowInTreeLabel;
        private CheckBox _ShowInTreeCheck;

        // SelectFiltersStep
        private CswWizardStep _SelectFiltersStep;
        private CswAutoTable _SelectFiltersStepTable;
        private CswAutoTable _SelectFiltersStepSubTable;
        private CswViewStructureTree _FiltersViewTree;
        private Button _AddFilterButton;
        private CswPropertyFilter _Filter;
        private Label _FilterLabel;
        private Label _CaseSensitiveLabel;
        private CheckBox _CaseSensitiveCheck;
        private Button _ApplyFilterButton;


        // Welcome Information
        //private CswWizardStep _WelcomeInfoStep;
        //private CswAutoTable _WelcomeInfoStepTable;
        //private CswAutoTable _WelcomeInfoStepSubTable;
        //private CswViewStructureTree _WelcomeViewTree;
        //private Literal _WelcomeTextLiteral;
        //private TextBox _WelcomeText;
        //private Literal _RelatedViewsLiteral;
        //private CswCheckBoxArray _RelatedViewsCBA;
        //private Button _SaveWelcomeInfoButton;

        public Int32 CurrentStep
        {
            get { EnsureChildControls(); return _Wizard.CurrentStep; }
            set { EnsureChildControls(); _Wizard.CurrentStep = value; _setView( true ); }
        }

        public Int32 MinimumStep
        {
            get { EnsureChildControls(); return _Wizard.MinimumStep; }
            set { EnsureChildControls(); _Wizard.MinimumStep = value; }
        }

        #endregion Variables and Properties

        #region Lifecycle


        public CswViewEditorWizard( CswNbtResources Resources, CswNbtView View, Selectable DefaultSelectable, RadAjaxManager AjaxMgr )
        {
            try
            {
                if( DefaultSelectable != null )
                {
                    RestrictSelectable = true;
                    addSelectable( DefaultSelectable );
                }
                FinishConstructor( Resources, AjaxMgr );
                _View = View;
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public CswViewEditorWizard( CswNbtResources Resources, RadAjaxManager AjaxMgr )
        {
            FinishConstructor( Resources, AjaxMgr );
        }

        //private CswTableCaddy _UserViewsCaddy;
        //private DataTable _UserViewsTable;
        private void FinishConstructor( CswNbtResources Resources, RadAjaxManager AjaxMgr )
        {
            _CswNbtResources = Resources;
            _AjaxMgr = AjaxMgr;

            //_UserViewsCaddy = _CswNbtResources.makeCswTableCaddy( "jct_users_views" );

            EnsureChildControls();
        }

        private TableCell _WizardCell;

        protected override void CreateChildControls()
        {
            CswAutoTable OuterTable = new CswAutoTable();
            OuterTable.ID = "outertable";
            OuterTable.CssClass = "ViewEditorOuterTable";
            this.Controls.Add( OuterTable );

            _WizardCell = OuterTable.getCell( 0, 0 );
            _WizardCell.CssClass = "ViewEditorWizardCell";

            _Wizard = new CswWizard();
            _Wizard.ID = "wizard";
            _Wizard.WizardTitle = "Edit View";
            _Wizard.onPageChange += new CswWizard.CswWizardEventHandler( _Wizard_OnPageChange );
            _Wizard.onFinish += new CswWizard.CswWizardEventHandler( _Wizard_OnFinish );
            _Wizard.onCancel += new CswWizard.CswWizardEventHandler( _Wizard_OnCancel );
            _Wizard.FinishButtonText = "Save and Finish";
            _Wizard.PreviousClientClickFunctionName = "CswViewWizard_Previous";
            _WizardCell.Controls.Add( _Wizard );

            _SelectViewStep = new CswWizardStep();
            _SelectViewStep.ID = "SelectViewStep";
            _SelectViewStep.Title = "Choose a View";
            _SelectViewStep.Step = 1;
            _SelectViewStep.OnStepInit += new CswWizardStep.StepEventHandler( _SelectViewStep_OnStepInit );
            _SelectViewStep.OnStepLoad += new CswWizardStep.StepEventHandler( _SelectViewStep_OnStepLoad );
            _Wizard.WizardSteps.Add( _SelectViewStep );

            _ViewAttributesStep = new CswWizardStep();
            _ViewAttributesStep.ID = "ViewAttributesStep";
            _ViewAttributesStep.Title = "Edit View Attributes";
            _ViewAttributesStep.Step = 2;
            _ViewAttributesStep.ShowFinish = true;
            _ViewAttributesStep.OnStepInit += new CswWizardStep.StepEventHandler( _ViewAttributesStep_OnStepInit );
            _Wizard.WizardSteps.Add( _ViewAttributesStep );

            _SetRelationshipsStep = new CswWizardStep();
            _SetRelationshipsStep.ID = "SetRelationshipsStep";
            _SetRelationshipsStep.Title = "Add Relationships";
            _SetRelationshipsStep.Step = 3;
            _SetRelationshipsStep.ShowFinish = true;
            _SetRelationshipsStep.OnStepInit += new CswWizardStep.StepEventHandler( _SetRelationshipsStep_OnStepInit );
            _Wizard.WizardSteps.Add( _SetRelationshipsStep );

            _SelectPropertiesStep = new CswWizardStep();
            _SelectPropertiesStep.ID = "SelectPropertiesStep";
            _SelectPropertiesStep.Title = "Select Properties";
            _SelectPropertiesStep.Step = 4;
            _SelectPropertiesStep.ShowFinish = true;
            _SelectPropertiesStep.OnStepInit += new CswWizardStep.StepEventHandler( _SelectPropertiesStep_OnStepInit );
            _Wizard.WizardSteps.Add( _SelectPropertiesStep );

            _SelectFiltersStep = new CswWizardStep();
            _SelectFiltersStep.ID = "SelectFiltersStep";
            _SelectFiltersStep.Title = "Set Filters";
            _SelectFiltersStep.Step = 5;
            _SelectFiltersStep.ShowFinish = true;
            _SelectFiltersStep.OnStepInit += new CswWizardStep.StepEventHandler( _SelectFiltersStep_OnStepInit );
            _Wizard.WizardSteps.Add( _SelectFiltersStep );

            //_WelcomeInfoStep = new CswWizardStep();
            //_WelcomeInfoStep.ID = "WelcomeInfoStep ";
            //_WelcomeInfoStep.Title = "Setup Welcome Information";
            //_WelcomeInfoStep.Step = 6;
            //_WelcomeInfoStep.ShowFinish = true;
            //_WelcomeInfoStep.OnStepInit += new CswWizardStep.StepEventHandler( _WelcomeInfoStep_OnStepInit );
            //_Wizard.WizardSteps.Add( _WelcomeInfoStep );


            _PreviewCell = OuterTable.getCell( 1, 0 );
            _PreviewCell.CssClass = "ViewEditorPreviewCell";

            _Hidden_CurrentViewXml = new HiddenField();
            _Hidden_CurrentViewXml.ID = "currentxml";
            _PreviewCell.Controls.Add( _Hidden_CurrentViewXml );

            _HiddenRemoveButton = new Button();
            _HiddenRemoveButton.CssClass = "Button";
            _HiddenRemoveButton.ID = "hiddenremovebutton";
            _HiddenRemoveButton.Style.Add( HtmlTextWriterStyle.Display, "none" );
            _HiddenRemoveButton.Click += new EventHandler( _HiddenRemoveButton_Click );
            _PreviewCell.Controls.Add( _HiddenRemoveButton );

            _HiddenNodeToRemoveField = new HiddenField();
            _HiddenNodeToRemoveField.ID = "HiddenNodeToRemoveField";
            _PreviewCell.Controls.Add( _HiddenNodeToRemoveField );

            _PreviewLabel = new Label();
            _PreviewLabel.ID = "PreviewLabel";
            _PreviewLabel.Text = "View Preview";
            _PreviewLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            _PreviewCell.Controls.Add( _PreviewLabel );

            _PreviewTree = new RadTreeView();
            _PreviewTree.ID = "PreviewTree";
            _PreviewTree.Height = 300;
            _PreviewTree.Width = 300;
            _PreviewTree.EnableEmbeddedSkins = false;
            _PreviewTree.Skin = "ChemSW";
            _PreviewTree.Visible = false;
            _PreviewCell.Controls.Add( _PreviewTree );

            HtmlGenericControl _PreviewDiv = new HtmlGenericControl( "div" );
            _PreviewDiv.Attributes.Add( "class", "GridDiv" );
            _PreviewCell.Controls.Add( _PreviewDiv );

            _PreviewGrid = new CswNodesGrid( _CswNbtResources );
            _PreviewGrid.OnError += new CswErrorHandler( HandleError );
            _PreviewGrid.Grid.ClientSettings.Selecting.AllowRowSelect = false;
            _PreviewGrid.Visible = false;
            _PreviewDiv.Controls.Add( _PreviewGrid );

            _PreviewList = new CswNodesList( _CswNbtResources );
            _PreviewList.OnError += new CswErrorHandler( HandleError );
            _PreviewList.EnableLinks = false;
            _PreviewList.Visible = false;
            _PreviewDiv.Controls.Add( _PreviewList );

            base.CreateChildControls();
        }

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
        }

        // IPostBackDataHandler
        public bool LoadPostData( String postDataKey, NameValueCollection values )
        {
            try
            {
                // The values collection isn't always correct.  I don't know why.
                //if( values[_Hidden_CurrentViewXml.UniqueID] != null && values[_Hidden_CurrentViewXml.UniqueID].ToString() != string.Empty )
                //{
                //    _View = new CswNbtView( _CswNbtResources );
                //    _View.LoadXml( values[_Hidden_CurrentViewXml.UniqueID].ToString() );
                //}
                if( Page.IsPostBack && _Hidden_CurrentViewXml.Value != string.Empty )
                {
                    _View = new CswNbtView( _CswNbtResources );
                    _View.LoadXml( _Hidden_CurrentViewXml.Value );
                }


                // We have to do this to create dynamic controls in time, even though
                // it causes form values not to be restored
                _setView( false );

                // Now override view values from form

                // ViewAttributesStep
                if( values[_ViewNameTextBox.UniqueID] != null )
                    _ViewNameTextBox.Text = values[_ViewNameTextBox.UniqueID].ToString();
                //if( values[_ModeDropDown.UniqueID] != null )
                //{
                //    foreach( ListItem Item in _ModeDropDown.Items )
                //        Item.Selected = false;
                //    _ModeDropDown.Items.FindByValue( values[_ModeDropDown.UniqueID].ToString() ).Selected = true;
                //}
                if( values[_GridWidthBox.UniqueID] != null )
                    _GridWidthBox.Text = values[_GridWidthBox.UniqueID].ToString();
                if( values[_ForMobileCheckBox.UniqueID] != null )
                    _ForMobileCheckBox.Checked = CswConvert.ToBoolean( values[_ForMobileCheckBox.UniqueID] );
                //if( values[_GridEditModeDropDown.UniqueID] != null )
                //{
                //    foreach( ListItem Item in _GridEditModeDropDown.Items )
                //        Item.Selected = false;
                //    _GridEditModeDropDown.Items.FindByValue( values[_GridEditModeDropDown.UniqueID].ToString() ).Selected = true;
                //}
                if( values[_ViewCategoryTextBox.UniqueID] != null )
                    _ViewCategoryTextBox.Text = values[_ViewCategoryTextBox.UniqueID].ToString();
                if( values[_ViewVisibilityEditor._VisibilityDropDown.UniqueID] != null )
                    _ViewVisibilityEditor.SelectedVisibility = (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), values[_ViewVisibilityEditor._VisibilityDropDown.UniqueID].ToString() );
                if( values[_ViewVisibilityEditor._VisibilityRoleDropDown.UniqueID] != null )
                    _ViewVisibilityEditor.SelectedRoleId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( values[_ViewVisibilityEditor._VisibilityRoleDropDown.UniqueID].ToString() ) );
                if( values[_ViewVisibilityEditor._VisibilityUserDropDown.UniqueID] != null )
                    _ViewVisibilityEditor.SelectedUserId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( values[_ViewVisibilityEditor._VisibilityUserDropDown.UniqueID].ToString() ) );

                //if ( values[ _IncludeInQuickLaunch.UniqueID ] != null )
                //    _IncludeInQuickLaunch.Checked = ( values[ _IncludeInQuickLaunch.UniqueID ] == "on" );



                // SetRelationshipsStep
                if( values[_NextOptions.UniqueID] != null )
                {
                    ListItem NextItem = _NextOptions.Items.FindByValue( values[_NextOptions.UniqueID].ToString() );
                    if( NextItem != null )
                        _NextOptions.SelectedValue = NextItem.Value;
                }
                //if( values[_AllowAddDropDown.UniqueID] != null )
                //{
                //    foreach( ListItem Item in _AllowAddDropDown.Items )
                //        Item.Selected = false;
                //    _AllowAddDropDown.Items.FindByValue( values[_AllowAddDropDown.UniqueID].ToString() ).Selected = true;
                //}
                if( values[_AllowDeleteCheckBox.UniqueID] != null )
                    _AllowDeleteCheckBox.Checked = ( values[_AllowDeleteCheckBox.UniqueID].ToString() == "on" );

                if( values[_GroupByDropDown.UniqueID] != null ) // && values[_GroupByDropDown.UniqueID].ToString() != string.Empty )  // BZ 10187
                {
                    foreach( ListItem Item in _GroupByDropDown.Items )
                        Item.Selected = false;
                    _GroupByDropDown.Items.FindByValue( values[_GroupByDropDown.UniqueID].ToString() ).Selected = true;
                }
                if( values[_ShowInTreeCheck.UniqueID] != null )
                    _ShowInTreeCheck.Checked = ( values[_ShowInTreeCheck.UniqueID].ToString() == "on" );


                // SelectPropertiesStep 
                if( values[_SortByCheckBox.UniqueID] != null )
                    _SortByCheckBox.Checked = ( values[_SortByCheckBox.UniqueID].ToString() == "on" );
                if( values[_SortByDropDown.UniqueID] != null )
                {
                    foreach( ListItem Item in _SortByDropDown.Items )
                        Item.Selected = false;
                    _SortByDropDown.Items.FindByValue( values[_SortByDropDown.UniqueID].ToString() ).Selected = true;
                }
                if( values[_GridOrderBox.UniqueID] != null )
                    _GridOrderBox.Text = values[_GridOrderBox.UniqueID].ToString();
                if( values[_GridColumnWidthBox.UniqueID] != null )
                    _GridColumnWidthBox.Text = values[_GridColumnWidthBox.UniqueID].ToString();

                // SelectFiltersStep
                if( values[_CaseSensitiveCheck.UniqueID] != null )
                    _CaseSensitiveCheck.Checked = ( values[_CaseSensitiveCheck.UniqueID].ToString() == "on" );

                // WelcomeInfoStep
                //if( values[_WelcomeText.UniqueID] != null )
                //    _WelcomeText.Text = values[_WelcomeText.UniqueID].ToString();

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            return false;
        }
        public void RaisePostDataChangedEvent()
        {
        }


        private void _setView( bool bValidateView )
        {
            if( _View != null && ( !bValidateView || _View.IsFullyEnabled() ) )
            {
                //_UserViewsCaddy.WhereClause = "where userid = '" + _CswNbtResources.CurrentUser.UserId.ToString() + "' and nodeviewid = '" + _View.ViewId.ToString() + "'";
                //_UserViewsTable = _UserViewsCaddy.Table;

                //                if ( _Wizard.CurrentStep > 1 )
                _ViewAttributesStep_SetValuesFromView();
                //                if ( _Wizard.CurrentStep > 2 )
                _SetRelationshipsStep_SetValuesFromView();
                //                if ( _Wizard.CurrentStep > 3 )
                _SelectPropertiesStep_SetValuesFromView();
                //                if( _Wizard.CurrentStep > 4 )
                _SelectFiltersStep_SetValuesFromView();
                //                if( _Wizard.CurrentStep > 5 )
                //_WelcomeInfoStep_SetValuesFromView();
            }
            else
            {
                _Wizard.CurrentStep = 1;
                Literal WarningLiteral = new Literal();
                WarningLiteral.Text = "The selected view is disabled.  Select another view.";
                _SelectViewStep.Controls.Add( WarningLiteral );
            }
        }


        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                Page.RegisterRequiresPostBack( this );

                if( !AllowLoad )
                {
                    _IncludeAllCheckBox.Visible = false;
                    _LoadViewLabel.Visible = false;
                    _LoadViewList.Visible = false;
                }

                if( !AllowDelete )
                    _DeleteViewButton.Visible = false;

                if( !AllowCreate )
                    _CreateNewViewButton.Visible = false;

                if( _Wizard.CurrentStep > 2 )
                    _PreviewCell.Visible = true;
                else
                    _PreviewCell.Visible = false;

                if( !_CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    _IncludeAllCheckBox.Checked = false;
                    _IncludeAllCheckBox.Visible = false;
                }
                if( _View != null )
                    _CreateNewViewButton.OnClientClick = "openNewViewPopup('" + _View.ViewId + "'); return false;";
                else
                    _CreateNewViewButton.OnClientClick = "openNewViewPopup(''); return false;";
                _DeleteViewButton.OnClientClick = "openDeleteViewPopup(document.getElementById('" + _LoadViewList.ClientID + "').value, '" + _RealDeleteViewButton.ClientID + "'); return false;";

                if( _ModeValueLabel.Text != "Grid" )
                {
                    _GridWidthLabel.Visible = false;
                    _GridWidthBox.Visible = false;
                    _ForMobileLabel.Visible = true;
                    _ForMobileCheckBox.Visible = true;
                }
                else
                {
                    _GridWidthLabel.Visible = true;
                    _GridWidthBox.Visible = true;
                    _ForMobileLabel.Visible = false;
                    _ForMobileCheckBox.Visible = false;
                    _ForMobileCheckBox.Checked = false;
                }

                if( _View != null )
                    _Hidden_CurrentViewXml.Value = _View.ToString();

                if( Debug )
                {
                    TextBox tb = new TextBox();
                    tb.ID = "temp";
                    tb.TextMode = TextBoxMode.MultiLine;
                    tb.Text = _View.ToString();
                    tb.Rows = 12;
                    tb.Columns = 120;
                    this.Controls.Add( tb );
                }

                // BZ 7076
                if( _LoadViewList.Visible == false && _Wizard.CurrentStep == 1 )
                    _Wizard.NextButton.Visible = false;

                _initPreview();

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }
        #endregion Lifecycle

        #region XML

        //private void _initTreeFromViewRecursive( CswNbtViewNode ViewNode, RadTreeNodeCollection Nodes, Type SelectType )
        //{
        //    RadTreeNode newNode = null;
        //    newNode = CreateNode( ViewNode, SelectType );
        //    Nodes.Add( newNode );

        //    // Recurse
        //    if( ViewNode is CswNbtViewRoot )
        //    {
        //        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
        //            _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
        //    }
        //    else if( ViewNode is CswNbtViewRelationship )
        //    {
        //        foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
        //            _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
        //        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
        //            _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
        //    }
        //    else if( ViewNode is CswNbtViewProperty )
        //    {
        //        foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
        //            _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
        //    }
        //}

        //public string getViewXml()
        //{
        //    if ( AllowSetName )
        //    {
        //        _View.SetViewName( _ViewNameTextBox.Text );
        //        _View.SetCategory( _ViewCategoryTextBox.Text );
        //        _View.SetVisibility( _ViewVisibilityEditor.SelectedVisibility, _ViewVisibilityEditor.SelectedRoleId, _ViewVisibilityEditor.SelectedUserId );
        //    }
        //    if ( AllowSetMode )
        //        _View.SetViewMode( ( NbtViewRenderingMode )Enum.Parse( typeof( NbtViewRenderingMode ), _ModeDropDown.SelectedValue ) );
        //    if ( _View.ViewMode == NbtViewRenderingMode.Grid )
        //    {
        //        if ( CswTools.IsInteger( _GridWidthBox.Text ) )
        //            _View.Width = CswConvert.ToInt32( _GridWidthBox.Text );
        //        else
        //            _View.Width = Int32.MinValue;
        //        _View.EditMode = ( ChemSW.Nbt.GridEditMode )Enum.Parse( typeof( ChemSW.Nbt.GridEditMode ), _GridEditModeDropDown.SelectedValue );
        //    }
        //    return _View.ToString();
        //}

        #endregion XML


        #region Form Initialization

        protected void _SelectViewStep_OnStepInit()
        {
            _SelectViewStepTable = new CswAutoTable();
            _SelectViewStepTable.FirstCellRightAlign = true;
            _SelectViewStepTable.ID = "SelectViewStepTable";
            _SelectViewStep.Controls.Add( _SelectViewStepTable );

            _CreateNewViewButton = new Button();
            _CreateNewViewButton.ID = "createnew";
            _CreateNewViewButton.CssClass = "Button";
            _CreateNewViewButton.Text = "Create New View";

            _LoadViewLabel = new Label();
            _LoadViewLabel.ID = "LoadViewLabel";
            _LoadViewLabel.Text = "Select a View to Edit:";

            _LoadViewList = new DropDownList();
            _LoadViewList.ID = "viewlist";
            _LoadViewList.CssClass = "selectinput";

            _DeleteViewButton = new Button();
            _DeleteViewButton.ID = "DeleteView";
            _DeleteViewButton.CssClass = "Button";
            _DeleteViewButton.Text = "Delete";
            _DeleteViewButton.UseSubmitBehavior = false;

            _HiddenDiv = new HtmlGenericControl( "div" );
            _HiddenDiv.ID = "hiddendiv";
            _HiddenDiv.Style.Add( "display", "none" );

            _RealDeleteViewButton = new Button();
            _RealDeleteViewButton.ID = "RealDeleteView";
            _RealDeleteViewButton.CssClass = "Button";
            _RealDeleteViewButton.Text = "Delete";
            _RealDeleteViewButton.UseSubmitBehavior = true;
            _RealDeleteViewButton.Click += new EventHandler( _RealDeleteButton_Click );
            _HiddenDiv.Controls.Add( _RealDeleteViewButton );

            _IncludeAllCheckBox = new CheckBox();
            _IncludeAllCheckBox.ID = "includeall";
            _IncludeAllCheckBox.Text = "Show All";
            _IncludeAllCheckBox.AutoPostBack = true;
            _IncludeAllCheckBox.Checked = false;

            _SelectViewStepTable.addControl( 0, 0, _LoadViewLabel );
            _SelectViewStepTable.addControl( 0, 1, _LoadViewList );
            _SelectViewStepTable.addControl( 0, 1, _DeleteViewButton );
            _SelectViewStepTable.addControl( 0, 1, _HiddenDiv );
            _SelectViewStepTable.addControl( 1, 1, _IncludeAllCheckBox );
            _SelectViewStepTable.addControl( 2, 1, _CreateNewViewButton );
        }


        protected void _SelectViewStep_OnStepLoad()
        {
            //LoadViewList DropDown
            string LoadViewListPriorSelectedValue = _LoadViewList.SelectedValue;
            _LoadViewList.Items.Clear();
            DataTable Views = new DataTable();
            //if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            //{
            //    if( _IncludeAllCheckBox.Checked )
            //        Views = _CswNbtResources.ViewSelect.getAllViews();
            //    else
            //        Views = _CswNbtResources.ViewSelect.getVisibleViews( true );
            //}
            //else
            //{
            //    Views = _CswNbtResources.ViewSelect.getUserViews();
            //}

            if( Views.Rows.Count > 0 )
            {
                foreach( DataRow Row in Views.Rows )
                {
                    string ViewListItemName = Row["viewname"].ToString();
                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        switch( (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), Row["visibility"].ToString() ) )
                        {
                            case NbtViewVisibility.User:
                                ViewListItemName += " (User: " + Row["username"].ToString() + ")";
                                break;
                            case NbtViewVisibility.Role:
                                ViewListItemName += " (Role: " + Row["rolename"].ToString() + ")";
                                break;
                            case NbtViewVisibility.Global:
                                ViewListItemName += " (Global)";
                                break;
                        }
                    }
                    _LoadViewList.Items.Add( new ListItem( ViewListItemName, Row["nodeviewid"].ToString() ) );
                }

                if( LoadViewListPriorSelectedValue != string.Empty && null != _LoadViewList.Items.FindByValue( LoadViewListPriorSelectedValue ) )
                    _LoadViewList.SelectedValue = LoadViewListPriorSelectedValue;
                else if( null != _View && _View.ViewId > 0 && null != _LoadViewList.Items.FindByValue( _View.ViewId.ToString() ) )
                    _LoadViewList.SelectedValue = _View.ViewId.ToString();
            }
            else
            {
                _LoadViewList.Visible = false;
                _LoadViewLabel.Visible = false;
                _DeleteViewButton.Visible = false;
            }
        }

        protected void _ViewAttributesStep_OnStepInit()
        {
            _ViewAttributesStepTable = new CswAutoTable();
            _ViewAttributesStepTable.FirstCellRightAlign = true;
            _ViewAttributesStepTable.ID = "ViewAttributesStepTable";
            _ViewAttributesStep.Controls.Add( _ViewAttributesStepTable );

            _ModeLabel = new Label();
            _ModeLabel.Text = "Display Mode:";

            _ModeValueLabel = new Label();

            _ForMobileLabel = new Label();
            _ForMobileLabel.Text = "For Mobile:";

            _ForMobileCheckBox = new CheckBox();

            //_ModeDropDown = new DropDownList();
            //_ModeDropDown.ID = "mode";
            //_ModeDropDown.CssClass = "selectinput";
            //_ModeDropDown.Items.Add( new ListItem( "Tree", NbtViewRenderingMode.Tree.ToString() ) );
            //_ModeDropDown.Items.Add( new ListItem( "Grid", NbtViewRenderingMode.Grid.ToString() ) );

            _ViewNameLabel = new Label();
            _ViewNameLabel.Text = "Set View Name:";

            _ViewNameTextBox = new TextBox();
            _ViewNameTextBox.ID = "name";
            _ViewNameTextBox.CssClass = "textinput";

            _ViewCategoryLabel = new Label();
            _ViewCategoryLabel.Text = "Category:";

            _ViewCategoryTextBox = new TextBox();
            _ViewCategoryTextBox.ID = "category";
            _ViewCategoryTextBox.CssClass = "textinput";

            _ViewVisibilityLabel = new Label();
            _ViewVisibilityLabel.Text = "View Visibility:";

            _ViewVisibilityEditor = new CswViewVisibilityEditor( _CswNbtResources );
            _ViewVisibilityEditor.ID = "viewvis";

            _GridWidthLabel = new Label();
            _GridWidthLabel.Text = "Grid Width (in characters):";

            _GridWidthBox = new TextBox();
            _GridWidthBox.ID = "width";
            _GridWidthBox.CssClass = "textinput";

            //_GridEditModeLabel = new Label();
            //_GridEditModeLabel.Text = "Grid Edit Mode:";

            //_GridEditModeDropDown = new DropDownList();
            //_GridEditModeDropDown.ID = "editmode";
            //_GridEditModeDropDown.CssClass = "selectinput";
            //_GridEditModeDropDown.Items.Add( new ListItem( "Full Editing", ChemSW.Nbt.GridEditMode.Full.ToString() ) );
            //_GridEditModeDropDown.Items.Add( new ListItem( "Quick Editing", ChemSW.Nbt.GridEditMode.Quick.ToString() ) );
            //_GridEditModeDropDown.Items.Add( new ListItem( "No Editing", ChemSW.Nbt.GridEditMode.None.ToString() ) );

            //_IncludeInQuickLaunchLiteral = new Literal();
            //_IncludeInQuickLaunchLiteral.Text = "Include in my Quick Launch:";

            //_IncludeInQuickLaunch = new CheckBox();
            //_IncludeInQuickLaunch.ID = "IncludeInQuickLaunch";

            _ViewAttributesStepTable.addControl( 0, 0, _ViewNameLabel );
            _ViewAttributesStepTable.addControl( 0, 1, _ViewNameTextBox );
            _ViewAttributesStepTable.addControl( 1, 0, _ViewCategoryLabel );
            _ViewAttributesStepTable.addControl( 1, 1, _ViewCategoryTextBox );
            _ViewAttributesStepTable.addControl( 2, 0, _ViewVisibilityLabel );
            _ViewAttributesStepTable.addControl( 2, 1, _ViewVisibilityEditor );
            _ViewAttributesStepTable.addControl( 3, 0, _ModeLabel );
            //_ViewAttributesStepTable.addControl( 3, 1, _ModeDropDown );
            _ViewAttributesStepTable.addControl( 3, 1, _ModeValueLabel );
            _ViewAttributesStepTable.addControl( 4, 0, _GridWidthLabel );
            _ViewAttributesStepTable.addControl( 4, 1, _GridWidthBox );
            //_ViewAttributesStepTable.addControl( 5, 0, _GridEditModeLabel );
            //_ViewAttributesStepTable.addControl( 5, 1, _GridEditModeDropDown );
            //_ViewAttributesStepTable.addControl( 6, 0, _IncludeInQuickLaunchLiteral );
            //_ViewAttributesStepTable.addControl( 6, 1, _IncludeInQuickLaunch );
            _ViewAttributesStepTable.addControl( 4, 0, _ForMobileLabel );
            _ViewAttributesStepTable.addControl( 4, 1, _ForMobileCheckBox );
        }

        protected void _ViewAttributesStep_SetValuesFromView()
        {
            if( _View != null )
            {
                _ViewNameTextBox.Text = _View.ViewName;
                _ViewCategoryTextBox.Text = _View.Category;
                _ViewVisibilityEditor.SelectedVisibility = _View.Visibility;
                _ViewVisibilityEditor.SelectedRoleId = _View.VisibilityRoleId;
                _ViewVisibilityEditor.SelectedUserId = _View.VisibilityUserId;

                if( !_CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    _ViewVisibilityLabel.Visible = false;
                    _ViewVisibilityEditor.Style.Add( HtmlTextWriterStyle.Display, "none" );
                }

                //_ModeDropDown.SelectedValue = _View.ViewMode.ToString();
                _ModeValueLabel.Text = _View.ViewMode.ToString();
                //if( _View.Visibility == NbtViewVisibility.Property )
                //    _ModeDropDown.Enabled = false;

                if( _View.Width > 0 )
                    _GridWidthBox.Text = _View.Width.ToString();
                else
                    _GridWidthBox.Text = string.Empty;
                //_GridEditModeDropDown.SelectedValue = _View.EditMode.ToString();

                //if ( _UserViewsTable.Rows.Count > 0 )
                //{
                //    _IncludeInQuickLaunch.Checked = CswConvert.ToBoolean( _UserViewsTable.Rows[ 0 ][ "quicklaunch" ] );
                //}
                //else
                //{
                //    _IncludeInQuickLaunch.Checked = false;
                //}

                _ForMobileCheckBox.Checked = _View.ForMobile;
            }
        }

        private Button _SaveRelationshipAttributesButton;
        private Label _GroupByLabel;
        private DropDownList _GroupByDropDown;

        protected void _SetRelationshipsStep_OnStepInit()
        {
            _SetRelationshipsStepTable = new CswAutoTable();
            _SetRelationshipsStepTable.ID = "SetRelationshipsStepTable";

            _SetRelationshipsStepSubTable = new CswAutoTable();
            _SetRelationshipsStepSubTable.FirstCellRightAlign = true;
            _SetRelationshipsStepSubTable.CssClass = "ViewEditorSubTable";
            _SetRelationshipsStepSubTable.ID = "SetRelationshipsStepSubTable";

            //_RelationshipsViewTree = new RadTreeView();
            //_RelationshipsViewTree.EnableEmbeddedSkins = false;
            //_RelationshipsViewTree.Skin = "ChemSW";
            //_RelationshipsViewTree.ID = "RelationshipsViewTree";
            //_RelationshipsViewTree.CssClass = "Tree";
            //_RelationshipsViewTree.NodeClick += new RadTreeViewEventHandler( _RelationshipsViewTree_NodeClick );

            _RelationshipsViewTree = new CswViewStructureTree( _CswNbtResources );
            _RelationshipsViewTree.ID = "RelationshipsViewTree";
            _RelationshipsViewTree.OnClick += new RadTreeViewEventHandler( _RelationshipsViewTree_NodeClick );
            _RelationshipsViewTree.OnError += new CswErrorHandler( HandleError );

            _NextOptionsLabel = new Label();

            _NextOptions = new DropDownList();
            _NextOptions.ID = "next";
            _NextOptions.CssClass = "selectinput";

            _AddButton = new Button();
            _AddButton.CssClass = "Button";
            _AddButton.Text = "Add";
            _AddButton.ID = "add";
            _AddButton.Click += new EventHandler( _AddButton_Click );

            //_AllowAddLabel = new Label();
            //_AllowAddLabel.Text = "Adding Children:";

            //_AllowAddDropDown = new DropDownList();
            //_AllowAddDropDown.CssClass = "selectinput";
            //_AllowAddDropDown.ID = "allowadd";

            _AllowDeleteLabel = new Label();
            _AllowDeleteLabel.Text = "Allow Deleting:";

            _AllowDeleteCheckBox = new CheckBox();
            _AllowDeleteCheckBox.ID = "allowdelete";

            _GroupByLabel = new Label();
            _GroupByLabel.Text = "Group By:";

            _GroupByDropDown = new DropDownList();
            _GroupByDropDown.ID = "groupby";
            _GroupByDropDown.CssClass = "selectinput";

            _EditSectionLabel = new Label();
            _EditSectionLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );

            _SaveRelationshipAttributesButton = new Button();
            _SaveRelationshipAttributesButton.ID = "SaveRelationshipAttributesButton";
            _SaveRelationshipAttributesButton.Text = "Apply";
            _SaveRelationshipAttributesButton.CssClass = "Button";
            _SaveRelationshipAttributesButton.Click += new EventHandler( _SaveRelationshipAttributesButton_Click );

            _ShowInTreeCheck = new CheckBox();
            _ShowInTreeCheck.ID = "showincheck";

            _ShowInTreeLabel = new Label();
            _ShowInTreeLabel.Text = "Show In Tree";

            _SetRelationshipsStep.Controls.Add( _SetRelationshipsStepTable );
            _SetRelationshipsStepTable.addControl( 0, 0, _RelationshipsViewTree );
            _SetRelationshipsStepTable.addControl( 0, 1, _SetRelationshipsStepSubTable );
            _SetRelationshipsStepSubTable.addControl( 0, 0, _NextOptionsLabel );
            TableCell NextOptionsCell = _SetRelationshipsStepSubTable.getCell( 1, 0 );
            NextOptionsCell.ColumnSpan = 2;
            NextOptionsCell.Controls.Add( _NextOptions );
            NextOptionsCell.Controls.Add( _AddButton );
            _SetRelationshipsStepSubTable.addControl( 2, 0, new CswLiteralNbsp() );
            _SetRelationshipsStepSubTable.addControl( 3, 0, _EditSectionLabel );
            //_SetRelationshipsStepSubTable.addControl( 4, 0, _AllowAddLabel );
            //_SetRelationshipsStepSubTable.addControl( 4, 1, _AllowAddDropDown );
            _SetRelationshipsStepSubTable.addControl( 5, 0, _AllowDeleteLabel );
            _SetRelationshipsStepSubTable.addControl( 5, 1, _AllowDeleteCheckBox );
            _SetRelationshipsStepSubTable.addControl( 6, 0, _GroupByLabel );
            _SetRelationshipsStepSubTable.addControl( 6, 1, _GroupByDropDown );
            _SetRelationshipsStepSubTable.addControl( 7, 0, _ShowInTreeLabel );
            _SetRelationshipsStepSubTable.addControl( 7, 1, _ShowInTreeCheck );
            _SetRelationshipsStepSubTable.addControl( 8, 1, _SaveRelationshipAttributesButton );

        }



        protected void _SetRelationshipsStep_SetValuesFromView()
        {
            if( _View != null )
            {
                // BZ 9203
                if( _View.Visibility == NbtViewVisibility.Property && _View.ViewMode == NbtViewRenderingMode.Grid )
                {
                    _RelationshipsViewTree.IsRootSelectable = false;
                    _RelationshipsViewTree.IsFirstLevelRemovable = false;
                }

                _RelationshipsViewTree.SelectableNodeTextPrefix = @"&nbsp;<img src=""Images\buttons\smallclear.gif"" style=""border: 0px;"" onclick=""ViewTree_ClearButton_Click('";
                _RelationshipsViewTree.SelectableNodeTextSuffix = @"', '" + _HiddenNodeToRemoveField.ClientID + @"', '" + _HiddenRemoveButton.ClientID + @"');"" />";
                _RelationshipsViewTree.reinitTreeFromView( _View, null, _View.Root, CswViewStructureTree.ViewTreeSelectType.Relationship );

                // Setup form                        
                _ShowInTreeCheck.Visible = false;
                _ShowInTreeLabel.Visible = false;

                CswNbtViewNode RelationshipsSelectedViewNode = _View.FindViewNodeByArbitraryId( _RelationshipsViewTree.SelectedNode.Value );
                if( _View.Visibility == NbtViewVisibility.Property && _View.ViewMode == NbtViewRenderingMode.Grid )
                {
                    if( RelationshipsSelectedViewNode is CswNbtViewRoot )
                    {
                        // BZ 9203 - don't select root for grid properties
                        RelationshipsSelectedViewNode = ( (CswNbtViewRoot) RelationshipsSelectedViewNode ).ChildRelationships[0];
                        _RelationshipsViewTree.setSelectedNode( RelationshipsSelectedViewNode.ArbitraryId );
                    }
                }

                if( _RelationshipsViewTree.SelectedNode != null )
                {
                    if( _View.ViewMode == NbtViewRenderingMode.List && _View.Root.ChildRelationships.Count > 0 )
                    {
                        // BZ 9935 - Only one relationship in a list
                        _NextOptions.Visible = false;
                        _NextOptionsLabel.Visible = false;
                        _AddButton.Visible = false;
                        _NextOptions.SelectedValue = null;   // BZ 6920
                        _GroupByDropDown.Visible = false;
                        _GroupByLabel.Visible = false;
                        //_AllowAddDropDown.Visible = false;
                        //_AllowAddLabel.Visible = false;
                        _AllowDeleteCheckBox.Visible = false;
                        _AllowDeleteLabel.Visible = false;
                        _SaveRelationshipAttributesButton.Visible = false;

                        if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRelationship )
                        {
                            _SaveRelationshipAttributesButton.Visible = true;
                            CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) RelationshipsSelectedViewNode;

                            _AllowDeleteCheckBox.Visible = true;
                            _AllowDeleteLabel.Visible = true;

                            _AllowDeleteCheckBox.Checked = CurrentRelationship.AllowDelete;

                            _EditSectionLabel.Text = "For '" + RelationshipsSelectedViewNode.TextLabel + "' ";

                            _ShowInTreeLabel.Visible = false;
                            _ShowInTreeCheck.Visible = false;

                        } // if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRelationship )
                        else if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRoot )
                        {
                            _SaveRelationshipAttributesButton.Visible = true;

                            //_AllowAddDropDown.Visible = true;
                            //_AllowAddLabel.Visible = true;
                            _EditSectionLabel.Visible = true;

                            //_AllowAddDropDown.Items.Clear();
                            //_AllowAddDropDown.Items.Add( new ListItem( "None", NbtViewAddChildrenSetting.None.ToString() ) );
                            //_AllowAddDropDown.Items.Add( new ListItem( "In View Only", NbtViewAddChildrenSetting.InView.ToString() ) );
                            ////_AllowAddDropDown.Items.Add( new ListItem( "All", NbtViewAddChildrenSetting.All.ToString() ) );
                            //_AllowAddDropDown.SelectedValue = ( (CswNbtViewRoot) RelationshipsSelectedViewNode ).AddChildren.ToString();

                            // BZ 8292
                            if( RelationshipsSelectedViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ).Count == 0 )
                            {
                                //_AllowAddLabel.Visible = false;
                                //_AllowAddDropDown.Visible = false;
                                _SaveRelationshipAttributesButton.Visible = false;
                                _EditSectionLabel.Visible = false;
                            }

                            _EditSectionLabel.Text = "For top level";

                        } // else if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRoot )
                    }
                    else
                    {
                        _NextOptions.Visible = true;
                        _NextOptionsLabel.Visible = true;
                        _AddButton.Visible = true;

                        string PriorRelationshipSelected = _NextOptions.SelectedValue;
                        _NextOptions.Items.Clear();

                        if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRelationship )
                        {
                            _SaveRelationshipAttributesButton.Visible = true;
                            CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) RelationshipsSelectedViewNode;
                            Int32 CurrentLevel = 0;
                            CswNbtViewNode Parent = CurrentRelationship;
                            while( !( Parent is CswNbtViewRoot ) )
                            {
                                CurrentLevel++;
                                Parent = Parent.Parent;
                            }


                            //_AllowAddDropDown.Visible = true;
                            //_AllowAddLabel.Visible = true;
                            _AllowDeleteCheckBox.Visible = true;
                            _AllowDeleteLabel.Visible = true;
                            _GroupByDropDown.Visible = true;
                            _GroupByLabel.Visible = true;

                            //_AllowAddDropDown.Items.Clear();
                            //_AllowAddDropDown.Items.Add( new ListItem( "None", NbtViewAddChildrenSetting.None.ToString() ) );
                            //_AllowAddDropDown.Items.Add( new ListItem( "In View Only", NbtViewAddChildrenSetting.InView.ToString() ) );
                            ////_AllowAddDropDown.Items.Add(new ListItem("All", _View.AddChildrenSetting.All.ToString()));
                            //_AllowAddDropDown.SelectedValue = CurrentRelationship.AddChildren.ToString();
                            // BZ 8292
                            //if( RelationshipsSelectedViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ).Count == 0 )
                            //{
                            //    _AllowAddLabel.Visible = false;
                            //    _AllowAddDropDown.Visible = false;
                            //}

                            _AllowDeleteCheckBox.Checked = CurrentRelationship.AllowDelete;

                            _NextOptionsLabel.Text = "Add below '" + RelationshipsSelectedViewNode.TextLabel + "':";
                            _EditSectionLabel.Text = "For '" + RelationshipsSelectedViewNode.TextLabel + "' ";


                            if( _View.ViewMode == NbtViewRenderingMode.Tree )
                            {
                                _ShowInTreeLabel.Visible = true;
                                _ShowInTreeCheck.Visible = true;
                                _ShowInTreeCheck.Checked = CurrentRelationship.ShowInTree;
                            }
                            //else
                            //{
                            //    _ShowInTreeLabel.Visible = false;
                            //    _ShowInTreeCheck.Visible = false;
                            //    //_ShowInTreeCheck.Checked = CurrentRelationship.ShowInGrid;
                            //    //_ShowInTreeLabel.Text = "Show In Grid";
                            //}

                            // Set _NextOptions to be all relations to this nodetype
                            _NextOptionsLabel.Visible = false;
                            _NextOptions.Visible = false;
                            _AddButton.Visible = false;

                            Int32 CurrentId = CurrentRelationship.SecondId;

                            _GroupByDropDown.Items.Clear();
                            _GroupByDropDown.Items.Add( new ListItem( "[none]", string.Empty ) );
                            ArrayList Relationships = null;
                            if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
                            {
                                Relationships = getObjectClassRelatedNodeTypesAndObjectClasses( CurrentId, _View, CurrentLevel );
                                CswNbtMetaDataObjectClass ThisObjectClass = _CswNbtResources.MetaData.getObjectClass( CurrentId );
                                foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in ThisObjectClass.ObjectClassProps )
                                {
                                    _GroupByDropDown.Items.Add( new ListItem( ObjectClassProp.PropName, ObjectClassProp.PropId.ToString() ) );
                                }
                                if( CurrentRelationship.GroupByPropId != Int32.MinValue )
                                    _GroupByDropDown.SelectedValue = CurrentRelationship.GroupByPropId.ToString();
                            }
                            else if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                            {
                                Relationships = getNodeTypeRelatedNodeTypesAndObjectClasses( CurrentId, _View, CurrentLevel );
                                CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( CurrentId );
                                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in ThisNodeType.NodeTypeProps )
                                {
                                    _GroupByDropDown.Items.Add( new ListItem( NodeTypeProp.PropName, NodeTypeProp.PropId.ToString() ) );
                                }
                                if( CurrentRelationship.GroupByPropId != Int32.MinValue )
                                    _GroupByDropDown.SelectedValue = CurrentRelationship.GroupByPropId.ToString();
                            }
                            else
                                throw new CswDniException( "A Data Misconfiguration has occurred", "CswViewEditor2._initNextOptions() has a selected node which is neither a NodeTypeNode nor an ObjectClassNode" );

                            foreach( CswNbtViewRelationship R in Relationships )
                            {
                                //R.ArbitraryId = ( (CswNbtViewRelationship) RelationshipsSelectedViewNode ).ArbitraryId.ToString() + "_" + R.SecondId;
                                if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                                {
                                    string Label = String.Empty;

                                    if( R.PropOwner == CswNbtViewRelationship.PropOwnerType.First )
                                    {
                                        Label = R.SecondName + " (by " + R.PropName + ")";
                                    }
                                    else if( R.PropOwner == CswNbtViewRelationship.PropOwnerType.Second )
                                    {
                                        Label = R.SecondName + " (by " + R.SecondName + "'s " + R.PropName + ")";
                                    }

                                    if( isSelectable( R.SecondType, R.SecondId ) )
                                        R.Selectable = true;
                                    else
                                        R.Selectable = false;

                                    ListItem Item = new ListItem( Label, R.ToString() );
                                    _NextOptions.Items.Add( Item );

                                    _NextOptionsLabel.Visible = true;
                                    _NextOptions.Visible = true;
                                    _AddButton.Visible = true;
                                } //  if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                            } // foreach( CswNbtViewRelationship R in Relationships )
                        } // if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRelationship )
                        else if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRoot )
                        {
                            _NextOptionsLabel.Visible = true;
                            _NextOptions.Visible = true;
                            _AddButton.Visible = true;
                            //_AllowAddDropDown.Visible = true;
                            //_AllowAddLabel.Visible = true;
                            _SaveRelationshipAttributesButton.Visible = true;
                            _EditSectionLabel.Visible = true;
                            _AllowDeleteCheckBox.Visible = false;
                            _AllowDeleteLabel.Visible = false;
                            _GroupByDropDown.Visible = false;
                            _GroupByLabel.Visible = false;

                            //_AllowAddDropDown.Items.Clear();
                            //_AllowAddDropDown.Items.Add( new ListItem( "None", NbtViewAddChildrenSetting.None.ToString() ) );
                            //_AllowAddDropDown.Items.Add( new ListItem( "In View Only", NbtViewAddChildrenSetting.InView.ToString() ) );
                            ////_AllowAddDropDown.Items.Add( new ListItem( "All", NbtViewAddChildrenSetting.All.ToString() ) );
                            //_AllowAddDropDown.SelectedValue = ( (CswNbtViewRoot) RelationshipsSelectedViewNode ).AddChildren.ToString();
                            // BZ 8292
                            if( RelationshipsSelectedViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ).Count == 0 )
                            {
                                //_AllowAddLabel.Visible = false;
                                //_AllowAddDropDown.Visible = false;
                                _SaveRelationshipAttributesButton.Visible = false;
                                _EditSectionLabel.Visible = false;
                            }

                            // Set NextOptions to be all viewable nodetypes and objectclasses
                            _NextOptionsLabel.Text = "Add to top level:";
                            _EditSectionLabel.Text = "For top level";

                            foreach( CswNbtMetaDataNodeType LatestNodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
                            {
                                if( _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, LatestNodeType.NodeTypeId, null, null ) )
                                {
                                    // This is purposefully not the typical way of creating CswNbtViewRelationships.
                                    CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, _View, LatestNodeType.FirstVersionNodeType, false );
                                    //R.ArbitraryId = "NT_" + R.SecondId.ToString();

                                    if( isSelectable( R.SecondType, R.SecondId ) )
                                        R.Selectable = true;
                                    else
                                        R.Selectable = false;

                                    bool IsChildAlready = false;
                                    foreach( CswNbtViewRelationship ChildRel in ( (CswNbtViewRoot) RelationshipsSelectedViewNode ).ChildRelationships )
                                    {
                                        if( ChildRel.SecondType == R.SecondType && ChildRel.SecondId == R.SecondId )
                                            IsChildAlready = true;
                                    }

                                    if( !IsChildAlready )
                                    {
                                        ListItem Item = new ListItem( LatestNodeType.NodeTypeName, R.ToString() );
                                        _NextOptions.Items.Add( Item );
                                    }
                                }
                            }

                            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
                            {
                                // This is purposefully not the typical way of creating CswNbtViewRelationships.
                                CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, _View, ObjectClass, false );
                                //R.ArbitraryId = "OC_" + R.SecondId.ToString();

                                if( isSelectable( R.SecondType, R.SecondId ) )
                                    R.Selectable = true;
                                else
                                    R.Selectable = false;

                                if( !( (CswNbtViewRoot) RelationshipsSelectedViewNode ).ChildRelationships.Contains( R ) )
                                {
                                    ListItem Item = new ListItem( "Any " + ObjectClass.ObjectClass, R.ToString() );
                                    _NextOptions.Items.Add( Item );
                                }
                            }
                        } // else if( AllowEdit && RelationshipsSelectedViewNode is CswNbtViewRoot )
                        else
                        {
                            _NextOptions.SelectedValue = null;   // BZ 6920
                            _NextOptionsLabel.Visible = false;
                            _NextOptions.Visible = false;
                            _AddButton.Visible = false;
                            //_AllowAddDropDown.Visible = false;
                            //_AllowAddLabel.Visible = false;
                            _AllowDeleteCheckBox.Visible = false;
                            _AllowDeleteLabel.Visible = false;
                            _GroupByLabel.Visible = false;
                            _GroupByDropDown.Visible = false;
                        }

                        if( AllowEdit && ( RelationshipsSelectedViewNode is CswNbtViewRelationship || RelationshipsSelectedViewNode is CswNbtViewRoot ) )
                        {
                            if( _NextOptions.Items.FindByValue( PriorRelationshipSelected ) != null )
                                _NextOptions.SelectedValue = PriorRelationshipSelected;
                            else if( _NextOptions.Items.Count > 0 )
                                _NextOptions.SelectedValue = _NextOptions.Items[0].Value;
                        }

                    } // if-else( _View.ViewMode == NbtViewRenderingMode.List && _View.Root.ChildRelationships.Count > 0 )
                } // if( _RelationshipsViewTree.SelectedNode != null )
            } // if( _View != null )
        } // _SetRelationshipsStep_SetValuesFromView()

        protected void _SelectPropertiesStep_OnStepInit()
        {
            _SelectPropertiesStepTable = new CswAutoTable();
            _SelectPropertiesStepTable.ID = "SelectPropertiesStepTable";

            _SelectPropertiesStepSubTable = new CswAutoTable();
            _SelectPropertiesStepSubTable.FirstCellRightAlign = true;
            _SelectPropertiesStepSubTable.CssClass = "ViewEditorSubTable";
            _SelectPropertiesStepSubTable.ID = "SelectPropertiesStepSubTable";

            //_PropertiesViewTree = new RadTreeView();
            //_PropertiesViewTree.EnableEmbeddedSkins = false;
            //_PropertiesViewTree.Skin = "ChemSW";
            //_PropertiesViewTree.ID = "PropertiesViewTree";
            //_PropertiesViewTree.CssClass = "Tree";
            //_PropertiesViewTree.NodeClick += new RadTreeViewEventHandler( _PropertiesViewTree_NodeClick );

            _PropertiesViewTree = new CswViewStructureTree( _CswNbtResources );
            _PropertiesViewTree.ID = "PropertiesViewTree";
            _PropertiesViewTree.OnClick += new RadTreeViewEventHandler( _PropertiesViewTree_NodeClick );
            _PropertiesViewTree.OnError += new CswErrorHandler( HandleError );

            _PropLabel = new Label();

            _PropCheckBoxArray = new CswCheckBoxArray( _CswNbtResources );
            _PropCheckBoxArray.ID = "PropCheckBoxArray";
            _PropCheckBoxArray.CheckboxesOnLeft = true;

            _AddPropButton = new Button();
            _AddPropButton.CssClass = "Button";
            _AddPropButton.Text = "Apply";
            _AddPropButton.ID = "addprop";
            _AddPropButton.Click += new EventHandler( _AddPropButton_Click );

            _SortByLabel = new Label();
            _SortByLabel.Text = "Sort By This Property:";

            _SortByCheckBox = new CheckBox();
            _SortByCheckBox.ID = "sortby";

            _SortByDropDown = new DropDownList();
            _SortByDropDown.ID = "sortmethod";
            _SortByDropDown.CssClass = "selectinput";
            foreach( string method in Enum.GetNames( typeof( CswNbtViewProperty.PropertySortMethod ) ) )
            {
                _SortByDropDown.Items.Add( new ListItem( method, method ) );
            }

            _GridOrderLabel = new Label();
            _GridOrderLabel.Text = "Grid Column Order: ";

            _GridOrderBox = new TextBox();
            _GridOrderBox.ID = "order";
            _GridOrderBox.CssClass = "textinput";

            _GridColumnWidthLabel = new Label();
            _GridColumnWidthLabel.Text = "Grid Column Width (in characters): ";

            _GridColumnWidthBox = new TextBox();
            _GridColumnWidthBox.ID = "colwidth";
            _GridColumnWidthBox.CssClass = "textinput";

            _ApplyPropButton = new Button();
            _ApplyPropButton.CssClass = "Button";
            _ApplyPropButton.Text = "Apply";
            _ApplyPropButton.ID = "ApplyPropButton";
            _ApplyPropButton.Click += new EventHandler( _ApplyPropButton_Click );

            _SelectPropertiesStep.Controls.Add( _SelectPropertiesStepTable );
            _SelectPropertiesStepTable.addControl( 0, 0, _PropertiesViewTree );
            _SelectPropertiesStepTable.addControl( 0, 1, _SelectPropertiesStepSubTable );
            _SelectPropertiesStepSubTable.addControl( 0, 1, _PropLabel );
            _SelectPropertiesStepSubTable.addControl( 1, 1, _PropCheckBoxArray );
            _SelectPropertiesStepSubTable.addControl( 3, 1, _AddPropButton );
            _SelectPropertiesStepSubTable.addControl( 5, 0, _SortByLabel );
            _SelectPropertiesStepSubTable.addControl( 5, 1, _SortByCheckBox );
            _SelectPropertiesStepSubTable.addControl( 5, 1, _SortByDropDown );
            _SelectPropertiesStepSubTable.addControl( 6, 0, _GridOrderLabel );
            _SelectPropertiesStepSubTable.addControl( 6, 1, _GridOrderBox );
            _SelectPropertiesStepSubTable.addControl( 7, 0, _GridColumnWidthLabel );
            _SelectPropertiesStepSubTable.addControl( 7, 1, _GridColumnWidthBox );
            _SelectPropertiesStepSubTable.addControl( 8, 1, _ApplyPropButton );
        }

        protected void _SelectPropertiesStep_SetValuesFromView()
        {
            if( _View != null )
            {
                CswNbtViewNode DefaultNodeToSelect = _View.Root;
                if( _View.Root.ChildRelationships.Count > 0 )
                    DefaultNodeToSelect = ( (CswNbtViewRelationship) _View.Root.ChildRelationships[0] );

                _PropertiesViewTree.SelectableNodeTextPrefix = @"&nbsp;<img src=""Images\buttons\smallclear.gif"" style=""border: 0px;"" onclick=""ViewTree_ClearButton_Click('";
                _PropertiesViewTree.SelectableNodeTextSuffix = @"', '" + _HiddenNodeToRemoveField.ClientID + @"', '" + _HiddenRemoveButton.ClientID + @"');"" />";
                _PropertiesViewTree.reinitTreeFromView( _View, null, DefaultNodeToSelect, CswViewStructureTree.ViewTreeSelectType.Property );

                // Setup form
                if( _PropertiesViewTree.SelectedNode != null )
                {
                    CswNbtViewNode PropertiesSelectedViewNode = _View.FindViewNodeByArbitraryId( _PropertiesViewTree.SelectedNode.Value );

                    //string PriorPropSelected = _PropList.SelectedValue;
                    if( AllowEdit && PropertiesSelectedViewNode is CswNbtViewRelationship )
                    {
                        CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) PropertiesSelectedViewNode;
                        initPropDataTable();

                        _PropLabel.Visible = true;
                        _PropCheckBoxArray.Visible = true;

                        _PropLabel.Text = "Select properties of " + PropertiesSelectedViewNode.TextLabel + ":";
                        _AddPropButton.Visible = true;
                    }
                    else
                    {
                        _PropLabel.Visible = false;
                        _AddPropButton.Visible = false;
                        _PropCheckBoxArray.Visible = false;
                    }

                    if( _View.ViewMode == NbtViewRenderingMode.Grid && PropertiesSelectedViewNode is CswNbtViewProperty )
                    {
                        _SortByLabel.Visible = true;
                        _SortByCheckBox.Visible = true;
                        _SortByDropDown.Visible = true;

                        _SortByCheckBox.Checked = ( (CswNbtViewProperty) PropertiesSelectedViewNode ).SortBy;
                        _SortByDropDown.SelectedValue = ( (CswNbtViewProperty) PropertiesSelectedViewNode ).SortMethod.ToString();

                        _GridOrderLabel.Visible = true;
                        _GridOrderBox.Visible = true;
                        _GridColumnWidthLabel.Visible = true;
                        _GridColumnWidthBox.Visible = true;
                        _ApplyPropButton.Visible = true;
                        if( ( (CswNbtViewProperty) PropertiesSelectedViewNode ).Order != Int32.MinValue )
                            _GridOrderBox.Text = ( (CswNbtViewProperty) PropertiesSelectedViewNode ).Order.ToString();
                        else
                            _GridOrderBox.Text = string.Empty;
                        if( ( (CswNbtViewProperty) PropertiesSelectedViewNode ).Width != Int32.MinValue )
                            _GridColumnWidthBox.Text = ( (CswNbtViewProperty) PropertiesSelectedViewNode ).Width.ToString();
                        else
                            _GridColumnWidthBox.Text = string.Empty;
                    }
                    else
                    {
                        _SortByLabel.Visible = false;
                        _SortByCheckBox.Visible = false;
                        _SortByDropDown.Visible = false;

                        _GridOrderLabel.Visible = false;
                        _GridOrderBox.Visible = false;

                        _GridColumnWidthLabel.Visible = false;
                        _GridColumnWidthBox.Visible = false;

                        _ApplyPropButton.Visible = false;
                    }

                } // if( _PropertiesViewTree.SelectedNode != null )
            } // if( _View != null )
        } // _SelectPropertiesStep_SetValuesFromView()

        protected void _SelectFiltersStep_OnStepInit()
        {
            _SelectFiltersStepTable = new CswAutoTable();
            _SelectFiltersStepTable.ID = "SelectFiltersStepTable";

            _SelectFiltersStepSubTable = new CswAutoTable();
            //_SelectFiltersStepSubTable.FirstCellRightAlign = true;
            _SelectFiltersStepSubTable.CssClass = "ViewEditorSubTable";
            _SelectFiltersStepSubTable.ID = "SelectFiltersStepSubTable";

            _FiltersViewTree = new CswViewStructureTree( _CswNbtResources );
            _FiltersViewTree.ID = "FiltersViewTree";
            _FiltersViewTree.OnClick += new RadTreeViewEventHandler( _FiltersViewTree_NodeClick );
            _FiltersViewTree.OnError += new CswErrorHandler( HandleError );

            _FilterLabel = new Label();

            CswAutoTable FilterTable = new CswAutoTable();
            _Filter = new CswPropertyFilter( _CswNbtResources, _AjaxMgr, null, false, false, false, false );
            _Filter.ShowSubFieldAndMode = true;
            _Filter.OnError += new CswErrorHandler( HandleError );
            _Filter.ID = "viewfiltercontrol";
            FilterTable.Rows.Add( _Filter );

            _AddFilterButton = new Button();
            _AddFilterButton.CssClass = "Button";
            _AddFilterButton.Text = "Add";
            _AddFilterButton.ID = "addfilter";
            _AddFilterButton.Click += new EventHandler( _AddFilterButton_Click );

            _CaseSensitiveLabel = new Label();
            _CaseSensitiveLabel.Text = "Case Sensitive:";

            _CaseSensitiveCheck = new CheckBox();
            _CaseSensitiveCheck.ID = "casesensitive";

            _ApplyFilterButton = new Button();
            _ApplyFilterButton.CssClass = "Button";
            _ApplyFilterButton.Text = "Apply";
            _ApplyFilterButton.ID = "ApplyFilterButton";
            _ApplyFilterButton.Click += new EventHandler( _ApplyFilterButton_Click );

            _NoPropsLiteral = new Literal();
            _NoPropsLiteral.Text = "No Properties Selected.  Select them in the previous step.";
            _NoPropsLiteral.Visible = false;

            _SelectFiltersStep.Controls.Add( _SelectFiltersStepTable );
            _SelectFiltersStepTable.addControl( 0, 0, _FiltersViewTree );
            _SelectFiltersStepTable.addControl( 0, 1, _SelectFiltersStepSubTable );
            _SelectFiltersStepSubTable.addControl( 0, 0, _FilterLabel );
            _SelectFiltersStepSubTable.addControl( 1, 0, FilterTable );
            _SelectFiltersStepSubTable.addControl( 1, 1, _AddFilterButton );
            _SelectFiltersStepSubTable.addControl( 3, 0, _CaseSensitiveLabel );
            _SelectFiltersStepSubTable.addControl( 3, 1, _CaseSensitiveCheck );
            _SelectFiltersStepSubTable.addControl( 4, 1, _ApplyFilterButton );
            _SelectFiltersStepSubTable.addControl( 5, 0, _NoPropsLiteral );
        }

        private Literal _NoPropsLiteral;

        protected void _SelectFiltersStep_SetValuesFromView()
        {
            if( _View != null )
            {
                CswNbtViewNode DefaultFilterNode = _View.findFirstProperty();
                _FiltersViewTree.SelectableNodeTextPrefix = @"&nbsp;<img src=""Images\buttons\smallclear.gif"" style=""border: 0px;"" onclick=""ViewTree_ClearButton_Click('";
                _FiltersViewTree.SelectableNodeTextSuffix = @"', '" + _HiddenNodeToRemoveField.ClientID + @"', '" + _HiddenRemoveButton.ClientID + @"');"" />";
                _FiltersViewTree.reinitTreeFromView( _View, null, DefaultFilterNode, CswViewStructureTree.ViewTreeSelectType.Filter );

                if( DefaultFilterNode == null )
                    _NoPropsLiteral.Visible = true;
                else
                    _NoPropsLiteral.Visible = false;

                // Setup form
                if( _FiltersViewTree.SelectedNode != null )
                {
                    CswNbtViewNode FiltersSelectedViewNode = _View.FindViewNodeByArbitraryId( _FiltersViewTree.SelectedNode.Value );

                    if( AllowEdit && FiltersSelectedViewNode is CswNbtViewProperty )
                    {
                        _Filter.Visible = true;
                        _AddFilterButton.Visible = true;
                        _FilterLabel.Visible = true;
                        _FilterLabel.Text = "Filter on " + FiltersSelectedViewNode.TextLabel + ":";
                        _Filter.SetFromView( (CswNbtViewProperty) FiltersSelectedViewNode );
                    }
                    else
                    {
                        _AddFilterButton.Visible = false;
                        _Filter.Visible = false;
                        _FilterLabel.Visible = false;
                    }

                    if( FiltersSelectedViewNode is CswNbtViewPropertyFilter )
                    {
                        _CaseSensitiveCheck.Visible = true;
                        _ApplyFilterButton.Visible = true;
                        _CaseSensitiveLabel.Visible = true;
                        _CaseSensitiveCheck.Checked = ( (CswNbtViewPropertyFilter) FiltersSelectedViewNode ).CaseSensitive;
                    }
                    else
                    {
                        _ApplyFilterButton.Visible = false;
                        _CaseSensitiveCheck.Visible = false;
                        _CaseSensitiveLabel.Visible = false;
                    }
                }//if( _FiltersViewTree.SelectedNode != null )
            } //if( _View != null )
        } //_SelectFiltersStep_SetValuesFromView()




        private void _initPreview()
        {
            if( _View != null )
            {
                if( _View.ViewMode == NbtViewRenderingMode.Tree )
                {
                    _PreviewTree.Visible = true;
                    _PreviewGrid.Visible = false;
                    _PreviewList.Visible = false;
                    CswNbtNodeKey ParentKey = null;
                    ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( _View, true, ref ParentKey, null, PreviewPageSize, true, false, null, false );
                    string XmlStr = CswNbtTree.getTreeAsXml();
                    _PreviewTree.LoadXml( XmlStr );
                    _PreviewTree.ExpandAllNodes();
                }
                else if( _View.ViewMode == NbtViewRenderingMode.List )
                {
                    _PreviewList.Visible = true;
                    _PreviewTree.Visible = false;
                    _PreviewGrid.Visible = false;

                    _PreviewList.View = _View;
                    _PreviewList.DataBind();
                }
                else
                {
                    _PreviewTree.Visible = false;
                    _PreviewGrid.Visible = true;
                    _PreviewGrid.View = _View;
                    _PreviewGrid.ResultsLimit = PreviewPageSize;
                    _PreviewGrid.DataBind();
                }
            }
        }

        //protected void _WelcomeInfoStep_OnStepInit()
        //{
        //    _WelcomeInfoStepTable = new CswAutoTable();
        //    _WelcomeInfoStepTable.ID = "WelcomeInfoStepTable";

        //    _WelcomeInfoStepSubTable = new CswAutoTable();
        //    _WelcomeInfoStepSubTable.CssClass = "ViewEditorSubTable";
        //    _WelcomeInfoStepSubTable.ID = "WelcomeInfoStepSubTable";

        //    _WelcomeViewTree = new CswViewStructureTree( _CswNbtResources );
        //    _WelcomeViewTree.ID = "WelcomeViewTree";
        //    _WelcomeViewTree.OnError += new CswErrorHandler( HandleError );

        //    _WelcomeTextLiteral = new Literal();
        //    _WelcomeTextLiteral.Text = "Welcome Text:";

        //    _WelcomeText = new TextBox();
        //    _WelcomeText.ID = "welcometext";
        //    _WelcomeText.CssClass = "textinput";
        //    _WelcomeText.TextMode = TextBoxMode.MultiLine;
        //    _WelcomeText.Rows = 8;
        //    _WelcomeText.Columns = 60;

        //    _RelatedViewsLiteral = new Literal();
        //    _RelatedViewsLiteral.Text = "Related Views:";

        //    _RelatedViewsCBA = new CswCheckBoxArray( _CswNbtResources );
        //    _RelatedViewsCBA.ID = "RelatedViewsCBA";

        //    _WelcomeInfoStep.Controls.Add( _WelcomeInfoStepTable );
        //    _WelcomeInfoStepTable.addControl( 0, 0, _WelcomeViewTree );
        //    _WelcomeInfoStepTable.addControl( 0, 1, _WelcomeInfoStepSubTable );
        //    _WelcomeInfoStepSubTable.addControl( 0, 0, _WelcomeTextLiteral );
        //    _WelcomeInfoStepSubTable.addControl( 0, 1, _WelcomeText );
        //    _WelcomeInfoStepSubTable.addControl( 1, 0, _RelatedViewsLiteral );
        //    _WelcomeInfoStepSubTable.addControl( 1, 1, _RelatedViewsCBA );

        //}

        //protected void _WelcomeInfoStep_SetValuesFromView()
        //{
        //    if( _View != null )
        //    {
        //        _WelcomeViewTree.reinitTreeFromView( _View, null, _View.Root, CswViewStructureTree.ViewTreeSelectType.Relationship );
        //        _WelcomeText.Text = _View.WelcomeText;

        //        DataTable ViewsTable = null;
        //        //if( _CswNbtResources.CurrentUser.IsAdministrator() )
        //        ViewsTable = _CswNbtResources.ViewSelect.getVisibleViews( false );
        //        //else
        //        //    ViewsTable = CswNbtView.getUserViews( _CswNbtResources );

        //        DataTable ModifiedViewsTable = new CswDataTable( "relatedviewscheckboxdatatable", "" );
        //        ModifiedViewsTable.Columns.Add( "View Name", typeof( string ) );
        //        ModifiedViewsTable.Columns.Add( "nodeviewid", typeof( Int32 ) );
        //        ModifiedViewsTable.Columns.Add( "Include", typeof( Boolean ) );
        //        foreach( DataRow OrigRow in ViewsTable.Rows )
        //        {
        //            if( CswConvert.ToInt32( OrigRow["nodeviewid"] ) != _View.ViewId )
        //            {
        //                DataRow NewRow = ModifiedViewsTable.NewRow();
        //                NewRow["View Name"] = OrigRow["viewname"];
        //                NewRow["nodeviewid"] = OrigRow["nodeviewid"];
        //                NewRow["Include"] = ( ( "," + _View.RelatedViewIds + "," ).Contains( "," + OrigRow["nodeviewid"].ToString() + "," ) );
        //                ModifiedViewsTable.Rows.Add( NewRow );
        //            }
        //        }
        //        _RelatedViewsCBA.CreateCheckBoxes( ModifiedViewsTable, "View Name", "nodeviewid" );
        //    }
        //}


        #endregion Form Initialization

        #region Events


        void _RelationshipsViewTree_NodeClick( object sender, RadTreeNodeEventArgs e )
        {
            _setView( false );
        }
        void _PropertiesViewTree_NodeClick( object sender, RadTreeNodeEventArgs e )
        {
            _setView( false );
        }
        void _FiltersViewTree_NodeClick( object sender, RadTreeNodeEventArgs e )
        {
            _setView( false );
        }


        protected void _Wizard_OnPageChange( object sender, CswWizardEventArgs e )
        {
            SaveChanges( e.PreviousPage );
            // BZ 8661 - Prevent going to 'Welcome' step for Grid views
            //if( e.NewPage >= 3 && _View.ViewMode == NbtViewRenderingMode.Grid )
            //{
            //    _Wizard.MaximumStep = 5;
            //}
        }

        private void SaveChanges( Int32 Page )
        {
            try
            {
                switch( Page )
                {
                    case 1:
                        // Set values from SelectViewStep
						_View = _CswNbtResources.ViewSelect.restoreView( CswConvert.ToInt32( _LoadViewList.SelectedValue ) );
                        break;

                    case 2:
                        // Set values from ViewAttributesStep POST
                        _View.ViewName = _ViewNameTextBox.Text;
                        _View.Root.Category = _ViewCategoryTextBox.Text;
                        _View.Visibility = _ViewVisibilityEditor.SelectedVisibility;
                        _View.VisibilityRoleId = _ViewVisibilityEditor.SelectedRoleId;
                        _View.VisibilityUserId = _ViewVisibilityEditor.SelectedUserId;
                        _View.Root.ViewMode = (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), _ModeValueLabel.Text ); //_ModeDropDown.SelectedValue );

                        //// Handle the Include in Quick Launch checkbox
                        //if ( _IncludeInQuickLaunch.Checked && _UserViewsTable.Rows.Count == 0 )
                        //{
                        //    DataRow UserViewRow = _UserViewsTable.NewRow();
                        //    UserViewRow[ "userid" ] = _CswNbtResources.CurrentUser.UserId.ToString();
                        //    UserViewRow[ "nodeviewid" ] = _View.ViewId.ToString();
                        //    _UserViewsTable.Rows.Add( UserViewRow );
                        //}
                        //if ( _UserViewsTable.Rows.Count > 0 )
                        //{
                        //    _UserViewsTable.Rows[ 0 ][ "quicklaunch" ] = CswConvert.ToDbVal( _IncludeInQuickLaunch.Checked );
                        //    _UserViewsCaddy.update( _UserViewsTable );
                        //}

                        if( _View.ViewMode == NbtViewRenderingMode.Grid )
                        {
                            if( _View.Width > 0 )
                            {
                                if( CswTools.IsInteger( _GridWidthBox.Text ) )
                                    _View.Width = CswConvert.ToInt32( _GridWidthBox.Text );
                            }
                            _View.ForMobile = false;
                            //_View.EditMode = (ChemSW.Nbt.GridEditMode) Enum.Parse( typeof( ChemSW.Nbt.GridEditMode ), _GridEditModeDropDown.SelectedValue.ToString() );
                        }
                        else
                        {
                            _View.ForMobile = _ForMobileCheckBox.Checked;
                        }

                        break;

                    case 3:
                        // Set values from SaveRelationshipsStep
                        if( _RelationshipsViewTree.SelectedNode != null )
                        {
                            CswNbtViewNode RelationshipSelectedViewNode = _View.FindViewNodeByArbitraryId( _RelationshipsViewTree.SelectedNode.Value );
                            if( RelationshipSelectedViewNode is CswNbtViewRelationship )
                            {
                                CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) RelationshipSelectedViewNode;
                                CurrentRelationship.AllowDelete = _AllowDeleteCheckBox.Checked;
                                //CurrentRelationship.AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), _AllowAddDropDown.SelectedValue );
                                if( _GroupByDropDown.SelectedValue != string.Empty )
                                {
                                    if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                                    {
                                        CswNbtMetaDataNodeType SecondNodeType = _CswNbtResources.MetaData.getNodeType( CurrentRelationship.SecondId );
                                        CurrentRelationship.setGroupByProp( SecondNodeType.getNodeTypeProp( CswConvert.ToInt32( _GroupByDropDown.SelectedValue ) ) );
                                    }
                                    else
                                    {
                                        CswNbtMetaDataObjectClass SecondObjectClass = _CswNbtResources.MetaData.getObjectClass( CurrentRelationship.SecondId );
                                        CurrentRelationship.setGroupByProp( SecondObjectClass.getObjectClassProp( CswConvert.ToInt32( _GroupByDropDown.SelectedValue ) ) );
                                    }
                                }
                                else
                                {
                                    CurrentRelationship.clearGroupBy();
                                }
                                if( _View.ViewMode == NbtViewRenderingMode.Tree )   
                                    CurrentRelationship.ShowInTree = _ShowInTreeCheck.Checked;

                            }
                            else
                            {
                                //( (CswNbtViewRoot) RelationshipSelectedViewNode ).AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), _AllowAddDropDown.SelectedValue );
                            }
                        }
                        _SetRelationshipsStep_SetValuesFromView();
                        break;
                    case 4:
                        // Properties Step
                        if( _PropertiesViewTree.SelectedNode != null )
                        {
                            CswNbtViewNode PropertiesSelectedViewNode = _View.FindViewNodeByArbitraryId( _PropertiesViewTree.SelectedNode.Value );
                            if( PropertiesSelectedViewNode is CswNbtViewProperty )
                            {
                                CswNbtViewProperty PropViewNode = (CswNbtViewProperty) PropertiesSelectedViewNode;
                                PropViewNode.SortBy = _SortByCheckBox.Checked;
                                PropViewNode.SortMethod = (CswNbtViewProperty.PropertySortMethod) Enum.Parse( typeof( CswNbtViewProperty.PropertySortMethod ), _SortByDropDown.SelectedValue );
                                if( CswTools.IsInteger( _GridOrderBox.Text ) )
                                    PropViewNode.Order = CswConvert.ToInt32( _GridOrderBox.Text );
                                if( CswTools.IsInteger( _GridColumnWidthBox.Text ) )
                                    PropViewNode.Width = CswConvert.ToInt32( _GridColumnWidthBox.Text );
                            }
                            else if( PropertiesSelectedViewNode is CswNbtViewRelationship )
                            {
                                CswNbtViewRelationship RelationshipViewNode = (CswNbtViewRelationship) PropertiesSelectedViewNode;
                                foreach( DataRow PropRow in _PropDataTable.Rows )
                                {
                                    CswDelimitedString ViewPropString = new CswDelimitedString( CswNbtView.delimiter );
                                    ViewPropString.FromString( PropRow["ViewProp"].ToString() );

                                    CswNbtViewProperty NewProp = new CswNbtViewProperty( _CswNbtResources, _View, ViewPropString );
                                    bool Checked = _PropCheckBoxArray.GetValue( PropRow["Value"].ToString(), "Include" );
                                    bool Contains = RelationshipViewNode.Properties.Contains( NewProp );
                                    if( Checked && !Contains )
                                        RelationshipViewNode.addProperty( NewProp );
                                    else if( !Checked && Contains )
                                        RelationshipViewNode.removeProperty( NewProp );
                                }

                            }
                        }
                        _setView( false );
                        break;
                    case 5:
                        // Filters Step
                        if( _FiltersViewTree.SelectedNode != null )
                        {
                            CswNbtViewNode FilterSelectedViewNode = _View.FindViewNodeByArbitraryId( _FiltersViewTree.SelectedNode.Value );
                            if( FilterSelectedViewNode is CswNbtViewPropertyFilter )
                                ( (CswNbtViewPropertyFilter) FilterSelectedViewNode ).CaseSensitive = _CaseSensitiveCheck.Checked;
                        }
                        break;
                    //case 6:
                    //    // Welcome and Related step
                    //    _View.WelcomeText = _WelcomeText.Text;
                    //    _View.RelatedViewIds = _RelatedViewsCBA.GetCheckedValues( "Include" );
                    //    break;
                }
                _setView( true );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        } // SaveChanges()


        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else
                throw ex;
        }

        void _SaveRelationshipAttributesButton_Click( object sender, EventArgs e )
        {
            try
            {
                SaveChanges( CurrentStep );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected void _RealDeleteButton_Click( object sender, EventArgs e )
        {
            try
            {
                Int32 ViewId = CswConvert.ToInt32( _LoadViewList.SelectedValue );
				_View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                _View.Delete();

                _SelectViewStep_OnStepLoad();

                if( CswTools.IsInteger( _LoadViewList.SelectedValue ) )
					_View = _CswNbtResources.ViewSelect.restoreView( CswConvert.ToInt32( _LoadViewList.SelectedValue ) );

                _setView( true );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected void _AddButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( _NextOptions.SelectedValue != String.Empty &&
                     _RelationshipsViewTree.SelectedNode != null )
                {
                    CswDelimitedString NextOptsSelVal = new CswDelimitedString( CswNbtView.delimiter );
                    NextOptsSelVal.FromString( _NextOptions.SelectedValue );

                    CswNbtViewRelationship SourceRelationship = new CswNbtViewRelationship( _CswNbtResources, _View, NextOptsSelVal );
                    CswNbtViewNode RelationshipsSelectedViewNode = _View.FindViewNodeByArbitraryId( _RelationshipsViewTree.SelectedNode.Value.ToString() );
                    CswNbtViewRelationship NewRelationship = _View.CopyViewRelationship( RelationshipsSelectedViewNode, SourceRelationship, true );
                    _RelationshipsViewTree.reinitTreeFromView( _View, NewRelationship, _View.Root, CswViewStructureTree.ViewTreeSelectType.Relationship );
                    _setView( false );
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected void _AddFilterButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( _FiltersViewTree.SelectedNode != null )
                {
                    CswNbtViewNode FilterSelectedViewNode = _View.FindViewNodeByArbitraryId( _FiltersViewTree.SelectedNode.Value );
                    CswNbtViewPropertyFilter NewFilter = _View.AddViewPropertyFilter( (CswNbtViewProperty) FilterSelectedViewNode, _Filter.SelectedSubField.Name, _Filter.SelectedFilterMode, _Filter.FilterValue.ToString(), false );
                    //NewFilter.SubfieldName = _Filter.SelectedSubField.Name;
                    //NewFilter.FilterMode = _Filter.SelectedFilterMode;
                    //NewFilter.Value = _Filter.FilterValue.ToString();
                    //NewFilter.ArbitraryId = FilterSelectedViewNode.ArbitraryId + NewFilter.FilterMode.ToString() + NewFilter.Value.ToString();
                    //FilterSelectedViewNode.AddChild( NewFilter );
                    _FiltersViewTree.reinitTreeFromView( _View, NewFilter, _View.Root, CswViewStructureTree.ViewTreeSelectType.Filter );
                    _setView( false );
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected void _ApplyFilterButton_Click( object sender, EventArgs e )
        {
            SaveChanges( CurrentStep );
        }

        protected void _AddPropButton_Click( object sender, EventArgs e )
        {
            SaveChanges( CurrentStep );
        }

        protected void _ApplyPropButton_Click( object sender, EventArgs e )
        {
            SaveChanges( CurrentStep );
        }

        protected void _HiddenRemoveButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( _View != null )
                {
                    CswNbtViewNode NodeToRemove = _View.FindViewNodeByArbitraryId( _HiddenNodeToRemoveField.Value );
                    if( NodeToRemove != null )
                    {
                        CswNbtViewNode ParentNode = NodeToRemove.Parent;
                        ParentNode.RemoveChild( NodeToRemove );

                        _RelationshipsViewTree.reinitTreeFromView( _View, ParentNode, _View.Root, CswViewStructureTree.ViewTreeSelectType.Relationship );
                        _PropertiesViewTree.reinitTreeFromView( _View, ParentNode, _View.Root, CswViewStructureTree.ViewTreeSelectType.Property );
                        _FiltersViewTree.reinitTreeFromView( _View, ParentNode, _View.Root, CswViewStructureTree.ViewTreeSelectType.Filter );
                        _setView( false );
                    }
                    else
                    {
                        throw new CswDniException( "Could not find view node to remove", "_HiddenRemoveButton_Click could not find match for: " + _HiddenNodeToRemoveField.Value + " in the current view." );
                    }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public delegate void ViewEditorEvent();
        public ViewEditorEvent onCancel = null;
        public ViewEditorEvent onFinish = null;

        protected void _Wizard_OnCancel( object sender, CswWizardEventArgs CswWizardEventArgs )
        {
            if( onCancel != null )
                onCancel();
        }

        protected void _Wizard_OnFinish( object sender, CswWizardEventArgs CswWizardEventArgs )
        {
            SaveChanges( CurrentStep );
            Finish();
        }

        private void Finish()
        {
            if( onFinish != null )
                onFinish();
        }



        #endregion Events

        #region Helper Functions

        private ArrayList getNodeTypeRelatedNodeTypesAndObjectClasses( Int32 FirstVersionId, CswNbtView View, Int32 Level )
        {
            ArrayList Relationships = new ArrayList();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid ) &&
                            ( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataNodeType FirstVersionNodeType = _CswNbtResources.MetaData.getNodeType( FirstVersionId );
            CswNbtMetaDataNodeType LatestVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( FirstVersionNodeType );
            CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.ObjectClass;

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForNodeTypeId_select", "getRelationsForNodeTypeId" );
            RelationshipPropsSelect.S4Parameters.Add( "getnodetypeid", FirstVersionNodeType.NodeTypeId );
            //RelationshipPropsQueryCaddy.S4Parameters.Add("getroleid", _CswNbtResources.CurrentUser.RoleId);
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                    PropRow["fkvalue"].ToString() != String.Empty )
                {
                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );

                    if( ( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString() &&
                          PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                          PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) )
                    {
                        if( _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, FirstVersionNodeType.NodeTypeId, null, null ) )
                        {
                            // Special case -- relationship to my own type
                            // We need to create two relationships from this

                            CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
                            //CswNbtViewRelationship R1 = View.MakeEmptyViewRelationship();
                            //R1.setProp( CswNbtViewRelationship.PropOwnerType.First, ThisProp );
                            //R1.setFirst( FirstVersionNodeType );
                            //R1.setSecond( FirstVersionNodeType );
                            Relationships.Add( R1 );

                            if( !Restrict )
                            {
                                // Copy it
                                //CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
                                //R2.setProp( CswNbtViewRelationship.PropOwnerType.Second, ThisProp );
                                CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
                                Relationships.Add( R2 );
                            }
                        }
                    }
                    else if( ( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString() &&
                               PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                             ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                               PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                    {
                        // Special case -- relationship to my own class
                        // We need to create two relationships from this

                        CswNbtViewRelationship R1 = _View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
                        //R1.setProp( CswNbtViewRelationship.PropOwnerType.First, ThisProp );
                        R1.overrideFirst( FirstVersionNodeType );
                        R1.overrideSecond( ObjectClass );
                        Relationships.Add( R1 );

                        if( !Restrict )
                        {
                            // Copy it
                            //CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
                            //R2.setProp( CswNbtViewRelationship.PropOwnerType.Second, ThisProp );
                            CswNbtViewRelationship R2 = _View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( FirstVersionNodeType );
                            R2.overrideSecond( ObjectClass );
                            Relationships.Add( R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString() &&
                            PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
                            if( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
                                R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            else
                                R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );

                            if( R.SecondType != CswNbtViewRelationship.RelatedIdType.NodeTypeId ||
                                _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, R.SecondId, null, null ) )
                            {
                                Relationships.Add( R );
                            }
                        }
                        else if( ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                                   PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me or my object class
                                R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() )
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                else
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );

                                if( R.SecondType != CswNbtViewRelationship.RelatedIdType.NodeTypeId ||
                                    _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, R.SecondId, null, null ) )
                                {
                                    Relationships.Add( R );
                                }
                            }
                        }
                        else
                        {
                            throw new CswDniException( "An unexpected data condition has occurred", "CswDataSourceNodeType.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original nodetypeid" );
                        }
                        if( R != null )
                            R.overrideFirst( FirstVersionNodeType );

                    }
                }
            }

            return Relationships;
        }

        private ArrayList getObjectClassRelatedNodeTypesAndObjectClasses( Int32 ObjectClassId, CswNbtView View, Int32 Level )
        {
            ArrayList Relationships = new ArrayList();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid ) &&
                            ( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForObjectClassId_select", "getRelationsForObjectClassId" );
            RelationshipPropsSelect.S4Parameters.Add( "getobjectclassid", ObjectClassId );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    if( ( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() &&
                          PropRow["typeid"].ToString() == ObjectClassId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                          PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) )
                    {
                        CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );

                        // Special case -- relationship to my own class
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( ObjectClass );
                        R1.overrideSecond( ObjectClass );
                        Relationships.Add( R1 );

                        if( !Restrict )
                        {
                            // Copy it
                            //CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
                            //R2.setProp( CswNbtViewRelationship.PropOwnerType.Second, ThisProp );
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( ObjectClass );
                            R2.overrideSecond( ObjectClass );
                            Relationships.Add( R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() &&
                            PropRow["typeid"].ToString() == ObjectClassId.ToString() )
                        {
                            // my relation to something else
                            CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                            R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
                            if( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
                                R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            else
                                R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideFirst( ObjectClass );
                            Relationships.Add( R );
                        }
                        else if( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() && PropRow["fkvalue"].ToString() == ObjectClassId.ToString() )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() )
                                {
                                    CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                    R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                    R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( ObjectClass );
                                Relationships.Add( R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( "An unexpected data condition has occurred", "CswDataSourceObjectClass.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original objectclassid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private void initPropDataTable()
        {
            CswNbtViewNode PropertiesSelectedViewNode = _View.FindViewNodeByArbitraryId( _PropertiesViewTree.SelectedNode.Value );

            if( AllowEdit && PropertiesSelectedViewNode is CswNbtViewRelationship )
            {
                _PropDataTable = new CswDataTable( "propdatatable", "PropDataTable" );
                _PropDataTable.Columns.Add( "Property Name" );
                _PropDataTable.Columns.Add( "Include", typeof( bool ) );
                _PropDataTable.Columns.Add( "Value" );
                _PropDataTable.Columns.Add( "ViewProp" );

                CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) PropertiesSelectedViewNode;
                ICollection PropsCollection = null;
                if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
                {
                    PropsCollection = _getObjectClassPropsCollection( CurrentRelationship.SecondId );
                    //PropsCollection = _CswNbtResources.MetaData.getObjectClass( CurrentRelationship.SecondId ).ObjectClassProps;
                }
                else if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                {
                    PropsCollection = _getNodeTypePropsCollection( CurrentRelationship.SecondId );
                }
                else
                {
                    throw new CswDniException( "A Data Misconfiguration has occurred", "CswViewEditor.initPropDataTable() has a selected node which is neither a NodeTypeNode nor an ObjectClassNode" );
                }

                foreach( CswNbtMetaDataNodeTypeProp ThisProp in PropsCollection )
                {
                    // BZs 7085, 6651, 6644, 7092
                    if( ThisProp.FieldTypeRule.SearchAllowed )
                    {
                        CswNbtViewProperty ViewProp = _View.AddViewProperty( null, (CswNbtMetaDataNodeTypeProp) ThisProp );
                        DataRow PropRow = _PropDataTable.NewRow();
                        ////if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                        ////{
                        //    ViewProp.Type = CswNbtViewProperty.CswNbtPropType.NodeTypePropId;
                        //    //                            ViewProp.NodeTypePropId = ThisProp.PropId;
                        //    //bz #8016: Not all versions of props were coming out in exported xml
                        //    ViewProp.NodeTypePropId = ( (CswNbtMetaDataNodeTypeProp) ThisProp ).FirstPropVersionId;
                        //    ViewProp.Name = ThisProp.PropNameWithQuestionNo;
                        //ViewProp.ArbitraryId = PropertiesSelectedViewNode.ArbitraryId + "_prop" + ThisProp.PropId.ToString();
                        PropRow["Value"] = ViewProp.Name.ToLower() + "_" + ViewProp.NodeTypePropId.ToString();
                        ////}
                        ////else
                        ////{
                        ////    ViewProp.Type = CswNbtViewProperty.CswNbtPropType.ObjectClassPropId;
                        ////    ViewProp.ObjectClassPropId = ThisProp.PropId;
                        ////    ViewProp.Name = ThisProp.PropNameWithQuestionNo;
                        ////    ViewProp.ArbitraryId = PropertiesSelectedViewNode.ArbitraryId + "_prop" + ThisProp.PropId.ToString();
                        ////    PropRow["Value"] = ViewProp.ObjectClassPropId.ToString();
                        ////}
                        //ViewProp.FieldType = ThisProp.FieldType;
                        PropRow["Property Name"] = ViewProp.Name;
                        if( !ThisProp.NodeType.IsLatestVersion )
                            PropRow["Property Name"] += "&nbsp;(v" + ThisProp.NodeType.VersionNo + ")";
                        PropRow["ViewProp"] = ViewProp.ToString();
                        PropRow["Include"] = ( CurrentRelationship.Properties.Contains( ViewProp ) );
                        _PropDataTable.Rows.Add( PropRow );
                    } // if( ThisProp.FieldTypeRule.SearchAllowed )

                } // foreach (DataRow Row in Props.Rows)
                _PropCheckBoxArray.CreateCheckBoxes( _PropDataTable, "Property Name", "Value" );
            }
        }

        private ICollection _getNodeTypePropsCollection( Int32 NodeTypeId )
        {
            // Need to generate a set of all Props, including latest version props and
            // all historical ones from previous versions that are no longer included in the latest.
            SortedList PropsByName = new SortedList();
            SortedList PropsById = new SortedList();

            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( NodeType );
            while( ThisVersionNodeType != null )
            {
                foreach( CswNbtMetaDataNodeTypeProp ThisProp in ThisVersionNodeType.NodeTypeProps )
                {
                    //string ThisKey = ThisProp.PropName.ToLower(); //+ "_" + ThisProp.FirstPropVersionId.ToString();
                    if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                        !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                    }
                }
                ThisVersionNodeType = ThisVersionNodeType.PriorVersionNodeType;
            }
            return PropsByName.Values;
        }

        private ICollection _getObjectClassPropsCollection( Int32 ObjectClassId )
        {
            // Need to generate all properties on all nodetypes of this object class
            SortedList AllProps = new SortedList();
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
            {
                ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeProps )
                {
                    string ThisKey = NodeTypeProp.PropName.ToLower(); //+ "_" + NodeTypeProp.FirstPropVersionId.ToString();
                    if( !AllProps.ContainsKey( ThisKey ) )
                        AllProps.Add( ThisKey, NodeTypeProp );
                }
            }
            return AllProps.Values;
        }

        #endregion Helper Functions

        #region Selectable

        // Manage which nodes are selectable in this view
        public bool RestrictSelectable = false;
        private ArrayList _Selectables = new ArrayList();

        public void addSelectableId( CswNbtViewRelationship.RelatedIdType Type, Int32 Value )
        {
            addSelectable( new Selectable( Type, Value ) );
        }
        public void addSelectable( Selectable S )
        {
            _Selectables.Add( S );
        }
        public bool isSelectable( CswNbtViewRelationship.RelatedIdType Type, Int32 Value )
        {
            bool ret = true;
            if( RestrictSelectable )
            {
                ret = false;
                Selectable S = new Selectable( Type, Value );
                if( _Selectables.Contains( S ) )
                    ret = true;
            }
            return ret;
        }

        public class Selectable : IEquatable<Selectable>
        {
            public Selectable( CswNbtViewRelationship.RelatedIdType T, Int32 V )
            {
                Type = T;
                Value = V;
            }
            CswNbtViewRelationship.RelatedIdType Type;
            Int32 Value;


            // IEquatable
            public static bool operator ==( Selectable s1, Selectable s2 )
            {
                // If both are null, or both are same instance, return true.
                if( System.Object.ReferenceEquals( s1, s2 ) )
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if( ( (object) s1 == null ) || ( (object) s2 == null ) )
                {
                    return false;
                }

                // Now we know neither are null.  Compare values.
                if( ( s1.Type == s2.Type ) && ( s1.Value == s2.Value ) )
                    return true;
                else
                    return false;

            }

            public static bool operator !=( Selectable s1, Selectable s2 )
            {
                return !( s1 == s2 );
            }

            public override bool Equals( object obj )
            {
                if( !( obj is Selectable ) )
                    return false;
                return this == (Selectable) obj;
            }

            public bool Equals( Selectable obj )
            {
                return this == (Selectable) obj;
            }

            public override int GetHashCode()
            {
                return this.Value;
            }
        }

        #endregion Selectable

    }
}

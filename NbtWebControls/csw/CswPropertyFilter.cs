using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.NbtWebControls.FieldTypes;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    /// <summary>
    /// UI for editing a single filter on a single property in a View.
    /// Implemented as a TableRow, so that multiple filters can be arranged 
    /// in a table (e.g. CswViewFilterEditor) and their form elements will line up.
    /// </summary>
    public class CswPropertyFilter : TableRow, INamingContainer, IPostBackDataHandler
    {
        private CswNbtResources _CswNbtResources;
        private RadAjaxManager _AjaxMgr;
        private bool _AllowSelectNodeType = true;
        private bool _AllowSelectNodeTypeProp = true;

        /// <summary>
        /// Display the Node Type
        /// </summary>
        public bool ShowNodeType = true;
        /// <summary>
        /// Display the Node Type Property
        /// </summary>
        public bool ShowNodeTypeProp = true;
        /// <summary>
        /// Display the SubField and Mode select boxes
        /// </summary>
        public bool ShowSubFieldAndMode = false;
        /// <summary>
        /// Only include properties on a given tab
        /// </summary>
        public Int32 FilterPropertiesToTabId = Int32.MinValue;
        /// <summary>
        /// Do not include this property
        /// </summary>
        public Int32 FilterOutPropertyId = Int32.MinValue;
        /// <summary>
        /// Include blank options in select boxes ('Select...')
        /// </summary>
        public bool ShowBlankOptions = false;
        /// <summary>
        /// Don't include properties conditional on other properties
        /// </summary>
        public bool FilterOutConditionalProperties = false;
        /// <summary>
        /// Restrict the set of properties to include by field type
        /// </summary>
        public Collection<CswNbtMetaDataFieldType.NbtFieldType> AllowedFieldTypes = new Collection<CswNbtMetaDataFieldType.NbtFieldType>();
        /// <summary>
        /// Restrict the set of filter modes to include (further restricted by subfield of field type of selected property)
        /// </summary>
        public ArrayList AllowedFilterModes = new ArrayList();
        /// <summary>
        /// If true, only include properties marked as quick search
        /// </summary>
        public bool _IsQuickSearch = false;
        /// <summary>
        /// If true, use check changes javascript validation
        /// </summary>
        public bool UseCheckChanges = false;

        private bool _ShowLogic = false;
        private string _DefaultNodeTypeId = string.Empty;
        private string _DefaultNodeTypePropName = string.Empty;
        private string _DefaultSubFieldColumn = string.Empty;
        private CswNbtPropFilterSql.PropertyFilterMode _DefaultFilterMode = CswNbtPropFilterSql.PropertyFilterMode.Unknown;
        private string _DefaultFilterValue = string.Empty;
        private CswNbtViewNode _DefaultViewNode = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtResources">Resources</param>
        /// <param name="AjaxMgr">RAD Ajax Manager</param>
        /// <param name="DefaultViewNode">View node to use to construct default search</param>
        /// <param name="AllowSelectNodeType">If true, allow the user to pick the nodetype</param>
        /// <param name="AllowSelectNodeTypeProp">If true, allow the user to pick the property</param>
        /// <param name="IsQuickSearch">If true, only include properties marked as quick search</param>
        /// <param name="ShowLogic">Whether to show 'and/or' logic select box</param>
        public CswPropertyFilter( CswNbtResources CswNbtResources, RadAjaxManager AjaxMgr, CswNbtViewNode DefaultViewNode,
                                  bool AllowSelectNodeType, bool AllowSelectNodeTypeProp, bool IsQuickSearch, bool ShowLogic )
        {
            _Construct( CswNbtResources, AjaxMgr, AllowSelectNodeType, AllowSelectNodeTypeProp, IsQuickSearch, ShowLogic );

            _DefaultViewNode = DefaultViewNode;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtResources">Resources</param>
        /// <param name="AjaxMgr">RAD Ajax Manager</param>
        /// <param name="DefaultNodeTypeId">Nodetype to use in default search</param>
        /// <param name="DefaultNodeTypePropName">Property to use in default search</param>
        /// <param name="DefaultSubFieldColumn">Subfield to use in default search</param>
        /// <param name="DefaultFilterMode">Filter mode to use in default search</param>
        /// <param name="DefaultFilterValue">Filter value to use in default search</param>
        /// <param name="AllowSelectNodeType">If true, allow the user to pick the nodetype</param>
        /// <param name="AllowSelectNodeTypeProp">If true, allow the user to pick the property</param>
        /// <param name="IsQuickSearch">If true, only include properties marked as quick search</param>
        /// <param name="ShowLogic">Whether to show 'and/or' logic select box</param>
        public CswPropertyFilter( CswNbtResources CswNbtResources, RadAjaxManager AjaxMgr,
                                  Int32 DefaultNodeTypeId, string DefaultNodeTypePropName, string DefaultSubFieldColumn,
                                  CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode, string DefaultFilterValue,
                                  bool AllowSelectNodeType, bool AllowSelectNodeTypeProp, bool IsQuickSearch, bool ShowLogic )
        {
            _Construct( CswNbtResources, AjaxMgr, AllowSelectNodeType, AllowSelectNodeTypeProp, IsQuickSearch, ShowLogic );

            _DefaultNodeTypeId = _NodeTypePrefix + DefaultNodeTypeId.ToString();
            _DefaultNodeTypePropName = DefaultNodeTypePropName;
            _DefaultSubFieldColumn = DefaultSubFieldColumn;
            _DefaultFilterMode = DefaultFilterMode;
            _DefaultFilterValue = DefaultFilterValue;

            EnsureChildControls();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtResources">Resources</param>
        /// <param name="AjaxMgr">RAD Ajax Manager</param>
        /// <param name="DefaultNodeTypeId">Nodetype to use in default search</param>
        /// <param name="DefaultNodeTypePropName">Property to use in default search</param>
        /// <param name="AllowSelectNodeType">If true, allow the user to pick the nodetype</param>
        /// <param name="AllowSelectNodeTypeProp">If true, allow the user to pick the property</param>
        /// <param name="IsQuickSearch">If true, only include properties marked as quick search</param>
        /// <param name="ShowLogic">Whether to show 'and/or' logic select box</param>
        public CswPropertyFilter( CswNbtResources CswNbtResources, RadAjaxManager AjaxMgr, Int32 DefaultNodeTypeId, string DefaultNodeTypePropName,
                                  bool AllowSelectNodeType, bool AllowSelectNodeTypeProp, bool IsQuickSearch, bool ShowLogic )
        {
            _Construct( CswNbtResources, AjaxMgr, AllowSelectNodeType, AllowSelectNodeTypeProp, IsQuickSearch, ShowLogic );

            _DefaultNodeTypeId = _NodeTypePrefix + DefaultNodeTypeId.ToString();
            _DefaultNodeTypePropName = DefaultNodeTypePropName;

            EnsureChildControls();
        }


        private void _Construct( CswNbtResources CswNbtResources, RadAjaxManager AjaxMgr, bool AllowSelectNodeType,
                                 bool AllowSelectNodeTypeProp, bool IsQuickSearch, bool ShowLogic )
        {
            this.EnableViewState = false;
            _CswNbtResources = CswNbtResources;
            _AjaxMgr = AjaxMgr;
            _AllowSelectNodeType = AllowSelectNodeType;
            _AllowSelectNodeTypeProp = AllowSelectNodeTypeProp;
            _IsQuickSearch = IsQuickSearch;
            _ShowLogic = ShowLogic;
        }




        private static string _ObjectClassPrefix = "oc_";
        private static string _NodeTypePrefix = "nt_";

        /// <summary>
        /// Init
        /// </summary>
        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();

                if( _CswNbtResources.MetaData != null )   // for example, the Login page
                {
                    PrimarySelectBox.IncludeBlank = ShowBlankOptions;
                    PrimarySelectBox.DataBind();

                    if( _DefaultViewNode != null )
                    {
                        SetFromView( _DefaultViewNode );
                    }
                    else if( _DefaultNodeTypeId != string.Empty )
                        init_NodeTypeSelectBox( _DefaultNodeTypeId, _DefaultNodeTypePropName, _DefaultSubFieldColumn, _DefaultFilterMode, _DefaultFilterValue );
                    else
                        init_NodeTypeSelectBox( PrimarySelectBox.Items[0].Value, string.Empty, string.Empty, CswNbtPropFilterSql.PropertyFilterMode.Unknown, string.Empty );
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnInit( e );
        }

        private DropDownList _PropSelectBox;
        /// <summary>
        /// Property select box
        /// </summary>
        public DropDownList PropSelectBox { get { return _PropSelectBox; } }

        private CswNodeTypeDropDown PrimarySelectBox;
        private Literal NodeTypeLiteral;
        private Literal NodeTypePropLiteral;
        private DropDownList SubFieldSelectBox;
        private DropDownList FilterModeSelectBox;
        private Literal FilterModeLiteral;
        private DropDownList FilterValueSelectBox;
        private TextBox FilterValueTextBox;
        private CswDatePicker FilterValueDatePicker;
        private CswTriStateCheckBox FilterValueCheckBox;
        private HiddenField ArbitraryIdField;

        // I use hidden fields to record what was selected, to know whether something changed
        // I had to do it this way because:
        //   1. we're not using viewstate
        //   2. the onchange events were misfiring for some reason
        private HiddenField OldPrimaryField;
        private HiddenField OldPropField;
        private HiddenField OldSubFieldField;
        private HiddenField OldFilterModeField;

        private Literal _LogicLiteral;

        /// <summary>
        /// CreateChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Cells.Add( new TableCell() );

            _LogicLiteral = new Literal();
            _LogicLiteral.Text = "&nbsp;and&nbsp;";
            this.Cells[this.Cells.Count - 1].Controls.Add( _LogicLiteral );

            this.Cells.Add( new TableCell() );

            ArbitraryIdField = new HiddenField();
            ArbitraryIdField.ID = "arbitraryid";
            this.Cells[this.Cells.Count - 1].Controls.Add( ArbitraryIdField );

            PrimarySelectBox = new CswNodeTypeDropDown( _CswNbtResources );
            PrimarySelectBox.ID = "NodeTypeSelectBox";
            PrimarySelectBox.CssClass = "selectinput";
            PrimarySelectBox.AutoPostBack = true;
            PrimarySelectBox.SelectedNodeTypeChanged += new CswNodeTypeDropDown.SelectedNodeTypeChangedHandler( PrimarySelectBox_SelectedNodeTypeChanged );
            this.Cells[this.Cells.Count - 1].Controls.Add( PrimarySelectBox );

            NodeTypeLiteral = new Literal();
            NodeTypeLiteral.ID = "NodeTypeLiteral";
            this.Cells[this.Cells.Count - 1].Controls.Add( NodeTypeLiteral );

            this.Cells.Add( new TableCell() );

            _PropSelectBox = new DropDownList();
            _PropSelectBox.ID = "PropSelectBox";
            _PropSelectBox.CssClass = "selectinput";
            _PropSelectBox.AutoPostBack = true;
            _PropSelectBox.SelectedIndexChanged += new EventHandler( _PropSelectBox_SelectedIndexChanged );
            this.Cells[this.Cells.Count - 1].Controls.Add( _PropSelectBox );

            NodeTypePropLiteral = new Literal();
            NodeTypePropLiteral.ID = "NodeTypePropLiteral";
            this.Cells[this.Cells.Count - 1].Controls.Add( NodeTypePropLiteral );

            this.Cells.Add( new TableCell() );

            SubFieldSelectBox = new DropDownList();
            SubFieldSelectBox.ID = "SubFieldSelectBox";
            SubFieldSelectBox.CssClass = "selectinput";
            SubFieldSelectBox.AutoPostBack = true;
            SubFieldSelectBox.SelectedIndexChanged += new EventHandler( SubFieldSelectBox_SelectedIndexChanged );
            this.Cells[this.Cells.Count - 1].Controls.Add( SubFieldSelectBox );

            this.Cells.Add( new TableCell() );

            FilterModeSelectBox = new DropDownList();
            FilterModeSelectBox.ID = "FilterModeSelectBox";
            FilterModeSelectBox.CssClass = "selectinput";
            FilterModeSelectBox.AutoPostBack = true;
            FilterModeSelectBox.SelectedIndexChanged += new EventHandler( FilterModeSelectBox_SelectedIndexChanged );
            this.Cells[this.Cells.Count - 1].Controls.Add( FilterModeSelectBox );

            this.Cells.Add( new TableCell() );

            FilterModeLiteral = new Literal();
            FilterModeLiteral.ID = "FilterModeLiteral";
            this.Cells[this.Cells.Count - 1].Controls.Add( FilterModeLiteral );

            this.Cells.Add( new TableCell() );

            CswAutoTable ValueTable = new CswAutoTable();
            ValueTable.ID = "ValueTable";
            this.Cells[this.Cells.Count - 1].Controls.Add( ValueTable );

            FilterValueTextBox = new TextBox();
            FilterValueTextBox.ID = "FilterValueTextBox";
            FilterValueTextBox.CssClass = "textinput";
            ValueTable.addControl( 0, 0, FilterValueTextBox );

            FilterValueSelectBox = new DropDownList();
            FilterValueSelectBox.ID = "FilterValueSelectBox";
            FilterValueSelectBox.CssClass = "selectinput";
            ValueTable.addControl( 0, 1, FilterValueSelectBox );

            FilterValueCheckBox = new CswTriStateCheckBox( false );
            FilterValueCheckBox.ID = "FilterValueCheckBox";
            ValueTable.addControl( 0, 2, FilterValueCheckBox );

            FilterValueDatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, UseCheckChanges );
            FilterValueDatePicker.ID = "FilterValueDatePicker";
            FilterValueDatePicker.AllowToday = true;
            FilterValueDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            ValueTable.addControl( 0, 3, FilterValueDatePicker );

            this.Cells.Add( new TableCell() );

            OldPrimaryField = new HiddenField();
            OldPrimaryField.ID = "OldPrimaryField";
            this.Cells[this.Cells.Count - 1].Controls.Add( OldPrimaryField );

            OldPropField = new HiddenField();
            OldPropField.ID = "OldPropField";
            this.Cells[this.Cells.Count - 1].Controls.Add( OldPropField );

            OldSubFieldField = new HiddenField();
            OldSubFieldField.ID = "OldSubFieldField";
            this.Cells[this.Cells.Count - 1].Controls.Add( OldSubFieldField );

            OldFilterModeField = new HiddenField();
            OldFilterModeField.ID = "OldFilterModeField";
            this.Cells[this.Cells.Count - 1].Controls.Add( OldFilterModeField );

            base.CreateChildControls();
        }

        void FilterModeSelectBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( OnChange != null )
                OnChange();
        }

        void SubFieldSelectBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( OnChange != null )
                OnChange();
        }

        void _PropSelectBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( OnChange != null )
                OnChange();
        }

        void PrimarySelectBox_SelectedNodeTypeChanged( NbtViewRelatedIdType SelectedType, Int32 SelectedValue )
        {
            if( OnChange != null )
                OnChange();
        }

        /// <summary>
        /// Set the nodetype, property, and filter from this ViewNode (Property or PropertyFilter)
        /// </summary>
        public void SetFromView( CswNbtViewNode ViewNode )
        {
            if( ViewNode is CswNbtViewProperty )
                SetFromView( (CswNbtViewProperty) ViewNode );
            else if( ViewNode is CswNbtViewPropertyFilter )
                SetFromView( (CswNbtViewPropertyFilter) ViewNode );
            else
				throw new CswDniException( ErrorType.Error, "Invalid ViewNode", "CswPropertyFilter.SetFromView got an invalid ViewNode" );
        }

        /// <summary>
        /// Set the nodetype, property, and filter from this ViewPropertyFilter
        /// </summary>
        public void SetFromView( CswNbtViewPropertyFilter CswNbtViewPropertyFilter )
        {
            CswNbtViewProperty CswNbtViewProperty = (CswNbtViewProperty) CswNbtViewPropertyFilter.Parent;
            CswNbtViewRelationship CswNbtViewRelationship = (CswNbtViewRelationship) CswNbtViewProperty.Parent;

            string PrimaryId = string.Empty;
            Int32 PropId = Int32.MinValue;
            string SubFieldColumn = string.Empty;
            CswNbtPropFilterSql.PropertyFilterMode FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Unknown;
            string FilterValue = string.Empty;

            if( CswNbtViewRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId )
            {
                PrimaryId = _NodeTypePrefix + CswNbtViewRelationship.SecondId;
            }
            else
            {
                PrimaryId = _ObjectClassPrefix + CswNbtViewRelationship.SecondId;
            }
            PropId = CswNbtViewProperty.NodeTypePropId;
            if( PropId > 0 )
            {
                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId );
                if( MetaDataProp != null )
                    SubFieldColumn = MetaDataProp.getFieldTypeRule().SubFields[CswNbtViewPropertyFilter.SubfieldName].Column.ToString();
            }
            FilterMode = CswNbtViewPropertyFilter.FilterMode;
            FilterValue = CswNbtViewPropertyFilter.Value;

            init_NodeTypeSelectBox( PrimaryId, CswNbtViewProperty.Name, SubFieldColumn, FilterMode, FilterValue );

            this.ArbitraryId = CswNbtViewPropertyFilter.ArbitraryId;
        }

        /// <summary>
        /// Set the nodetype, property, and filter from this ViewProperty
        /// </summary>
        public void SetFromView( CswNbtViewProperty CswNbtViewProperty )
        {
            CswNbtViewRelationship CswNbtViewRelationship = (CswNbtViewRelationship) CswNbtViewProperty.Parent;

            string PrimaryId = string.Empty;
            string SubFieldColumn = string.Empty;
            CswNbtPropFilterSql.PropertyFilterMode FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Unknown;
            string FilterValue = string.Empty;

            if( CswNbtViewRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId )
            {
                PrimaryId = _NodeTypePrefix + CswNbtViewRelationship.SecondId;
            }
            else
            {
                PrimaryId = _ObjectClassPrefix + CswNbtViewRelationship.SecondId;
            }

            // Case 20636
            string PropName = string.Empty;
            if( null != CswNbtViewProperty.NodeTypeProp )
                PropName = CswNbtViewProperty.NodeTypeProp.PropName;
            else if( null != CswNbtViewProperty.ObjectClassProp )
                PropName = CswNbtViewProperty.ObjectClassProp.PropName;

            init_NodeTypeSelectBox( PrimaryId, PropName, SubFieldColumn, FilterMode, FilterValue );

            this.ArbitraryId = CswNbtViewProperty.ArbitraryId;
        }

        /// <summary>
        /// Set to use this nodetype and property
        /// </summary>
        public void Set( Int32 NodeTypeId, string PropName )
        {
            Set( NodeTypeId, PropName, string.Empty, CswNbtPropFilterSql.PropertyFilterMode.Unknown, string.Empty );
        }
        /// <summary>
        /// Set to use this nodetype, property, and filter
        /// </summary>
        public void Set( Int32 NodeTypeId, string PropName, string SubFieldColumn, CswNbtPropFilterSql.PropertyFilterMode FilterMode, string FilterValue )
        {
            init_NodeTypeSelectBox( _NodeTypePrefix + NodeTypeId.ToString(), PropName, SubFieldColumn, FilterMode, FilterValue );
        }



        // NodeTypeSelectBox
        private void init_NodeTypeSelectBox( string IdToSelect, string PropNameToSelect, string SubFieldColumnToSelect, CswNbtPropFilterSql.PropertyFilterMode FilterModeToSelect, string Value )
        {
            CswTimer Timer = new CswTimer();
            PrimarySelectBox.SelectedValue = IdToSelect;
            init_PropSelectBox( PropNameToSelect, SubFieldColumnToSelect, FilterModeToSelect, Value );
            _CswNbtResources.logTimerResult( "CswPropertyFilter.init_NodeTypeSelectBox() finished", Timer.ElapsedDurationInSecondsAsString );
        }


        private ICollection _getNodeTypePropsCollection( Int32 NodeTypeId )
        {
            // Need to generate a set of all Props, including latest version props and
            // all historical ones from previous versions that are no longer included in the latest.
            SortedList PropsByName = new SortedList();
            SortedList PropsById = new SortedList();

            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeType );
            while( ThisVersionNodeType != null )
            {
                foreach( CswNbtMetaDataNodeTypeProp ThisProp in ThisVersionNodeType.getNodeTypeProps() )
                {
                    if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                        !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                    }
                }
                ThisVersionNodeType = ThisVersionNodeType.getPriorVersionNodeType();
            }
            return PropsByName.Values;
        }

        private ICollection _getObjectClassPropsCollection( Int32 ObjectClassId )
        {
            // Need to generate all properties on all nodetypes of this object class
            SortedList AllProps = new SortedList();
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
            {
                ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeProps )
                {
                    string ThisKey = NodeTypeProp.PropName.ToLower();
                    if( !AllProps.ContainsKey( ThisKey ) )
                        AllProps.Add( ThisKey, NodeTypeProp );
                }
            }
            return AllProps.Values;
        }




        // NodeTypePropSelectBox
        private void init_PropSelectBox( string PropNameToSelect, string SubFieldColumnToSelect, CswNbtPropFilterSql.PropertyFilterMode FilterModeToSelect, string Value )
        {
            PropSelectBox.Items.Clear();

            ICollection NodeTypeProps = null;
            if( SelectedNodeTypeLatestVersion != null )
            {
                NodeTypeProps = _getNodeTypePropsCollection( SelectedNodeTypeLatestVersion.NodeTypeId );
            }
            else if( SelectedObjectClass != null )
            {
                NodeTypeProps = _getObjectClassPropsCollection( SelectedObjectClass.ObjectClassId );
            }
            if( ShowBlankOptions )
                PropSelectBox.Items.Add( new ListItem( "Select...", "" ) );

            foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeProps )
            {
                if( NodeTypeProp.getFieldTypeRule().SearchAllowed &&
                    ( !_IsQuickSearch || NodeTypeProp.IsQuickSearch ) &&
                    ( FilterPropertiesToTabId == Int32.MinValue || ( NodeTypeProp.FirstEditLayout != null && NodeTypeProp.FirstEditLayout.TabId == FilterPropertiesToTabId ) ) &&
                    ( FilterOutPropertyId == Int32.MinValue || NodeTypeProp.PropId != FilterOutPropertyId ) &&
                    ( AllowedFieldTypes.Count == 0 || AllowedFieldTypes.Contains( NodeTypeProp.getFieldType().FieldType ) ) &&
                    ( !FilterOutConditionalProperties || !NodeTypeProp.hasFilter() ) )
                {
                    PropSelectBox.Items.Add( new ListItem( NodeTypeProp.PropName, NodeTypeProp.FirstPropVersionId.ToString() ) );
                    if( NodeTypeProp.PropName == PropNameToSelect )
                        PropSelectBox.SelectedValue = NodeTypeProp.FirstPropVersionId.ToString();
                }
            }
            init_SubFieldSelectBox( SubFieldColumnToSelect, FilterModeToSelect, Value );
        }

        // SubFieldSelectBox
        private void init_SubFieldSelectBox( string SubFieldColumnToSelect, CswNbtPropFilterSql.PropertyFilterMode FilterModeToSelect, string Value )
        {
            string DefaultSubFieldColumn = string.Empty;
            SubFieldSelectBox.Items.Clear();
            if( SelectedPropLatestVersion != null )
            {
                DefaultSubFieldColumn = SelectedPropLatestVersion.getFieldTypeRule().SubFields.Default.Column.ToString();

                foreach( CswNbtSubField SubField in SelectedPropLatestVersion.getFieldTypeRule().SubFields )
                {
                    SubFieldSelectBox.Items.Add( new ListItem( SubField.Name.ToString(), SubField.Column.ToString() ) );
                    if( SubFieldColumnToSelect != string.Empty )
                    {
                        if( SubField.Column.ToString() == SubFieldColumnToSelect )
                            SubFieldSelectBox.SelectedValue = SubFieldColumnToSelect;
                    }
                    else if( SelectedPropLatestVersion != null )
                    {
                        if( SubField.Column.ToString() == DefaultSubFieldColumn )
                            SubFieldSelectBox.SelectedValue = DefaultSubFieldColumn;
                    }
                }
            }
            init_FilterModeSelectBox( FilterModeToSelect, Value );
        }

        // FilterModeSelectBox
        private void init_FilterModeSelectBox( CswNbtPropFilterSql.PropertyFilterMode FilterModeToSelect, string Value )
        {
            FilterModeSelectBox.Items.Clear();
            FilterModeSelectBox.SelectedValue = string.Empty;
            if( SelectedSubField != null )
            {
                foreach( CswNbtPropFilterSql.PropertyFilterMode FilterModeOpt in SelectedSubField.SupportedFilterModes )
                {
                    if( AllowedFilterModes.Count == 0 || AllowedFilterModes.Contains( FilterModeOpt ) )
                    {
                        FilterModeSelectBox.Items.Add( new ListItem( SelectedPropLatestVersion.getFieldTypeRule().FilterModeToString( SelectedSubField, FilterModeOpt ), FilterModeOpt.ToString() ) );

                        if( FilterModeToSelect == CswNbtPropFilterSql.PropertyFilterMode.Unknown )
                        {
                            if( FilterModeOpt == SelectedSubField.DefaultFilterMode )
                                FilterModeSelectBox.SelectedValue = SelectedSubField.DefaultFilterMode.ToString();
                        }
                        else
                        {
                            if( FilterModeOpt == FilterModeToSelect )
                                FilterModeSelectBox.SelectedValue = FilterModeToSelect.ToString();
                        }
                    }
                }
            }
            init_Value( Value );
        }

        // Filter Value
        private void init_Value( string Value )
        {
            FilterValueSelectBox.Style[HtmlTextWriterStyle.Display] = "none";
            FilterValueTextBox.Style[HtmlTextWriterStyle.Display] = "none";
            FilterValueDatePicker.Style[HtmlTextWriterStyle.Display] = "none";
            FilterValueCheckBox.Style[HtmlTextWriterStyle.Display] = "none";

            if( SelectedPropLatestVersion != null )
            {
                if( SelectedFilterMode != CswNbtPropFilterSql.PropertyFilterMode.Null &&
                    SelectedFilterMode != CswNbtPropFilterSql.PropertyFilterMode.NotNull )
                {
                    switch( SelectedPropLatestVersion.getFieldType().FieldType )
                    {
                        case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                            FilterValueDatePicker.Style[HtmlTextWriterStyle.Display] = "";
                            if( Value != string.Empty && Value != null )
                                if( Value.Substring( 0, "today".Length ) == "today" )
                                {
                                    FilterValueDatePicker.Today = true;
                                    FilterValueDatePicker.TodayPlusDays = CswConvert.ToInt32( Value.Substring( "today+".Length ) );
                                }
                                else
                                    FilterValueDatePicker.SelectedDate = Convert.ToDateTime( Value );
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.List:
                            FilterValueSelectBox.Style[HtmlTextWriterStyle.Display] = "";
                            FilterValueSelectBox.Items.Clear();
                            ChemSW.Nbt.PropTypes.CswNbtNodeTypePropListOptions _CswNbtNodeTypePropListOptions = new ChemSW.Nbt.PropTypes.CswNbtNodeTypePropListOptions( _CswNbtResources, SelectedPropLatestVersion.PropId );
                            foreach( ChemSW.Nbt.PropTypes.CswNbtNodeTypePropListOption Option in _CswNbtNodeTypePropListOptions.Options )
                            {
                                FilterValueSelectBox.Items.Add( new ListItem( Option.Text, Option.Value ) );
                            }
                            if( FilterValueSelectBox.Items.FindByValue( Value ) != null )
                                FilterValueSelectBox.SelectedValue = Value;
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                            FilterValueCheckBox.Style[HtmlTextWriterStyle.Display] = "";
                            if( Value != string.Empty && Value != null )
                            {
                                if( Value.ToString().ToLower() == "1" || Value.ToString().ToLower() == "true" )
                                    FilterValueCheckBox.Checked = Tristate.True;
                                else if( Value.ToString().ToLower() == "0" || Value.ToString().ToLower() == "false" )
                                    FilterValueCheckBox.Checked = Tristate.False;
                                else
                                    FilterValueCheckBox.Checked = Tristate.Null;
                            }
                            break;
                        default:
                            FilterValueTextBox.Style[HtmlTextWriterStyle.Display] = "";
                            FilterValueTextBox.Text = Value;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// LoadPostData for IPostBackDataHandler
        /// </summary>
        public bool LoadPostData( String postDataKey, NameValueCollection values )
        {
            try
            {
                EnsureChildControls();

                string NewId = string.Empty;
                Int32 NewPropId = Int32.MinValue;
                string NewSubFieldColumn = string.Empty;
                CswNbtPropFilterSql.PropertyFilterMode NewFilterMode = CswNbtPropFilterSql.PropertyFilterMode.Unknown;
                string NewFilterValue = string.Empty;
                string NewPropName = string.Empty;

                if( values[PrimarySelectBox.UniqueID] != null )
                {
                    NewId = values[PrimarySelectBox.UniqueID].ToString();
                    if( values[OldPrimaryField.UniqueID] == null || NewId == values[OldPrimaryField.UniqueID].ToString() )
                    {
                        if( values[PropSelectBox.UniqueID] != null && CswTools.IsInteger( values[PropSelectBox.UniqueID].ToString() ) )
                        {
                            NewPropId = CswConvert.ToInt32( values[PropSelectBox.UniqueID].ToString() );
                            ICswNbtMetaDataProp MetaDataProp = null;
                            //if( NewId.Substring( 0, _NodeTypePrefix.Length ) == _NodeTypePrefix )
                            MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NewPropId );
                            //else
                            //    MetaDataProp = _CswNbtResources.MetaData.getObjectClassProp( NewPropId );
                            NewPropName = MetaDataProp.PropName;

                            if( values[OldPropField.UniqueID] == null || NewPropId.ToString() == values[OldPropField.UniqueID].ToString() )
                            {
                                if( values[SubFieldSelectBox.UniqueID] != null )
                                    NewSubFieldColumn = values[SubFieldSelectBox.UniqueID].ToString();
                                if( values[OldSubFieldField.UniqueID] == null || NewSubFieldColumn == values[OldSubFieldField.UniqueID].ToString() )
                                {
                                    if( values[FilterModeSelectBox.UniqueID] != null )
                                    {
                                        //NewFilterMode = (CswNbtPropFilterSql.PropertyFilterMode) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterMode ), values[FilterModeSelectBox.UniqueID].ToString() );
                                        NewFilterMode = (CswNbtPropFilterSql.PropertyFilterMode) values[FilterModeSelectBox.UniqueID].ToString();
                                    }
                                    if( values[OldFilterModeField.UniqueID] == null || NewFilterMode.ToString() == values[OldFilterModeField.UniqueID].ToString() )
                                    {
                                        switch( MetaDataProp.getFieldType().FieldType )
                                        {
                                            case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                                                if( FilterValueDatePicker.Today )
                                                {
                                                    NewFilterValue = "today+" + FilterValueDatePicker.TodayPlusDays.ToString();
                                                }
                                                else
                                                    NewFilterValue = values[FilterValueDatePicker.UniqueID];
                                                break;
                                            case CswNbtMetaDataFieldType.NbtFieldType.List:
                                                NewFilterValue = values[FilterValueSelectBox.UniqueID];
                                                break;
                                            case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                                                //NewFilterValue = ( values[FilterValueCheckBox.UniqueID] == "on" ).ToString().ToLower();
                                                break;
                                            default:
                                                NewFilterValue = values[FilterValueTextBox.UniqueID];
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                init_NodeTypeSelectBox( NewId,
                                        NewPropName,
                                        NewSubFieldColumn,
                                        NewFilterMode,
                                        NewFilterValue );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            return false;
        }

        /// <summary>
        /// RaisePostDataChangedEvent for IPostBackDataHandler
        /// </summary>
        public void RaisePostDataChangedEvent()
        {
        }

        /// <summary>
        /// Assigned error handler
        /// </summary>
        public event CswErrorHandler OnError;

        /// <summary>
        /// Internal error handler
        /// </summary>
        /// <param name="ex"></param>
        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else
                throw ex;
        }

        /// <summary>
        /// Change handler delegate
        /// </summary>
        public delegate void ChangeHandler();
        /// <summary>
        /// Change event
        /// </summary>
        public event ChangeHandler OnChange;

        /// <summary>
        /// Load
        /// </summary>
        protected override void OnLoad( EventArgs e )
        {
            try
            {
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, PropSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, SubFieldSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, FilterModeSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, FilterModeLiteral );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, FilterValueSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, FilterValueTextBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, FilterValueDatePicker );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, FilterValueCheckBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, OldPrimaryField );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, OldPropField );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, OldSubFieldField );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PrimarySelectBox, OldFilterModeField );

                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, SubFieldSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, FilterModeSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, FilterModeLiteral );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, FilterValueSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, FilterValueTextBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, FilterValueDatePicker );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, FilterValueCheckBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, OldPropField );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, OldSubFieldField );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( PropSelectBox, OldFilterModeField );

                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, FilterModeSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, FilterModeLiteral );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, FilterValueSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, FilterValueTextBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, FilterValueDatePicker );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, FilterValueCheckBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, OldSubFieldField );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( SubFieldSelectBox, OldFilterModeField );

                _AjaxMgr.AjaxSettings.AddAjaxSetting( FilterModeSelectBox, FilterValueSelectBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( FilterModeSelectBox, FilterValueTextBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( FilterModeSelectBox, FilterValueDatePicker );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( FilterModeSelectBox, FilterValueCheckBox );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( FilterModeSelectBox, OldFilterModeField );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnLoad( e );
        }

        /// <summary>
        /// Selected object class, if one is selected
        /// </summary>
        public CswNbtMetaDataObjectClass SelectedObjectClass
        {
            get
            {
                CswNbtMetaDataObjectClass ret = null;
                Int32 SOCId = SelectedObjectClassId;
                if( SOCId != Int32.MinValue )
                    ret = _CswNbtResources.MetaData.getObjectClass( SOCId );
                return ret;
            }
        }
        /// <summary>
        /// Selected objectclassid, if one is selected
        /// </summary>
        public Int32 SelectedObjectClassId
        {
            get
            {
                EnsureChildControls();
                return PrimarySelectBox.SelectedObjectClassId;
            }
        }
        /// <summary>
        /// Latest version of selected nodetype, if one is selected
        /// </summary>
        public CswNbtMetaDataNodeType SelectedNodeTypeLatestVersion
        {
            get
            {
                EnsureChildControls();
                return PrimarySelectBox.SelectedNodeTypeLatestVersion;
            }
        }
        /// <summary>
        /// First version id of selected nodetype, if one is selected
        /// </summary>
        public Int32 SelectedNodeTypeFirstVersionId
        {
            get
            {
                EnsureChildControls();
                return PrimarySelectBox.SelectedNodeTypeFirstVersionId;
            }
        }
        /// <summary>
        /// Latest version of selected property
        /// </summary>
        public ICswNbtMetaDataProp SelectedPropLatestVersion
        {
            get
            {
                ICswNbtMetaDataProp ret = null;
                Int32 PropId = SelectedNodeTypePropFirstVersionId;
                if( PropId != Int32.MinValue )
                {
                    ret = _CswNbtResources.MetaData.getNodeTypePropLatestVersion( PropId );
                }
                return ret;
            }
        }
        /// <summary>
        /// First version id of selected property
        /// </summary>
        public Int32 SelectedNodeTypePropFirstVersionId
        {
            get
            {
                EnsureChildControls();
                Int32 ret = Int32.MinValue;
                if( PropSelectBox.SelectedValue != string.Empty )
                    ret = CswConvert.ToInt32( PropSelectBox.SelectedValue );
                return ret;
            }
        }
        /// <summary>
        /// Selected subfield of selected property
        /// </summary>
        public CswNbtSubField SelectedSubField
        {
            get
            {
                EnsureChildControls();
                CswNbtSubField ret = null;
                if( SubFieldSelectBox.SelectedValue != string.Empty )
                {
                    //CswNbtSubField.PropColumn Column = (CswNbtSubField.PropColumn) Enum.Parse( typeof( CswNbtSubField.PropColumn ), SubFieldSelectBox.SelectedValue );
                    CswNbtSubField.PropColumn Column = (CswNbtSubField.PropColumn) SubFieldSelectBox.SelectedValue;
                    ret = SelectedPropLatestVersion.getFieldTypeRule().SubFields[Column];
                }
                return ret;
            }
        }

        /// <summary>
        /// Selected filter mode
        /// </summary>
        public CswNbtPropFilterSql.PropertyFilterMode SelectedFilterMode
        {
            get
            {
                EnsureChildControls();

                CswNbtPropFilterSql.PropertyFilterMode ret = CswNbtPropFilterSql.PropertyFilterMode.Unknown;
                if( FilterModeSelectBox.SelectedValue != string.Empty )
                {
                    //ret = (CswNbtPropFilterSql.PropertyFilterMode) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterMode ), FilterModeSelectBox.SelectedValue );
                    ret = (CswNbtPropFilterSql.PropertyFilterMode) FilterModeSelectBox.SelectedValue;
                }
                return ret;
            }
        }

        /// <summary>
        /// Entered Filter Value
        /// </summary>
        public object FilterValue
        {
            get
            {
                EnsureChildControls();
                object ret = null;
                switch( SelectedPropLatestVersion.getFieldType().FieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                        if( FilterValueDatePicker.Today )
                        {
                            ret = "today+" + FilterValueDatePicker.TodayPlusDays.ToString();
                        }
                        else
                            ret = FilterValueDatePicker.SelectedDate;
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.List:
                        ret = FilterValueSelectBox.SelectedValue;
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                        ret = FilterValueCheckBox.Checked;
                        break;
                    default:
                        ret = FilterValueTextBox.Text;
                        break;
                }
                return ret;
            }
        }

        /// <summary>
        /// Arbitrarily unique identifier for this filter
        /// </summary>
        public string ArbitraryId
        {
            get
            {
                return ArbitraryIdField.Value;
            }
            set
            {
                ArbitraryIdField.Value = value;
            }
        }

        /// <summary>
        /// OnPreRender
        /// </summary>
        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                Page.RegisterRequiresPostBack( this );

                if( _ShowLogic )
                    _LogicLiteral.Visible = true;
                else
                    _LogicLiteral.Visible = false;

                if( !_AllowSelectNodeType )
                {
                    PrimarySelectBox.Style[HtmlTextWriterStyle.Display] = "none";
                    if( ShowNodeType )
                    {
                        if( SelectedNodeTypeLatestVersion != null )
                            NodeTypeLiteral.Text = SelectedNodeTypeLatestVersion.NodeTypeName + ":&nbsp;";
                        else
                            NodeTypeLiteral.Text = SelectedObjectClass.ObjectClass.ToString() + ":&nbsp;";
                    }
                }

                if( !_AllowSelectNodeTypeProp )
                {
                    PropSelectBox.Style[HtmlTextWriterStyle.Display] = "none";
                    if( ShowNodeTypeProp )
                        NodeTypePropLiteral.Text = SelectedPropLatestVersion.PropName + "&nbsp;";
                }

                if( ShowSubFieldAndMode )
                {
                    SubFieldSelectBox.Style[HtmlTextWriterStyle.Display] = "";
                    FilterModeSelectBox.Style[HtmlTextWriterStyle.Display] = "";
                    FilterModeLiteral.Text = "";
                }
                else
                {
                    SubFieldSelectBox.Style[HtmlTextWriterStyle.Display] = "none";
                    FilterModeSelectBox.Style[HtmlTextWriterStyle.Display] = "none";
                    FilterModeLiteral.Text = SelectedFilterMode.ToString() + "&nbsp;";
                }

                OldPrimaryField.Value = PrimarySelectBox.SelectedValue;
                OldPropField.Value = PropSelectBox.SelectedValue;
                OldSubFieldField.Value = SubFieldSelectBox.SelectedValue.ToString();
                OldFilterModeField.Value = FilterModeSelectBox.SelectedValue;
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }


        /// <summary>
        /// True if the filter is valid
        /// </summary>
        /// <remarks>
        /// TODO: This should be refactored into the CswNbtSubField
        /// </remarks>
        public bool HasValidFilter
        {
            get
            {
                return ( SelectedFilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull ||
                         SelectedFilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null ||
                         ( FilterValue.ToString() != string.Empty &&
                           ( SelectedPropLatestVersion.getFieldType().FieldType != CswNbtMetaDataFieldType.NbtFieldType.DateTime ||
                             FilterValue.ToString().Substring( 0, "today".Length ) == "today" ||
                             CswConvert.ToDateTime( FilterValue ) != DateTime.MinValue ) ) );
            }
        }

        /// <summary>
        /// Make a view from the given filter
        /// </summary>
        /// <returns></returns>
        public CswNbtView MakeView()
        {
            CswNbtView FilterView = new CswNbtView( _CswNbtResources );

            CswNbtViewRelationship FilterRelationship;
            if( SelectedNodeTypeLatestVersion != null )
            {
                FilterView.ViewName = SelectedNodeTypeLatestVersion.NodeTypeName + " Search";
                FilterRelationship = FilterView.AddViewRelationship( SelectedNodeTypeLatestVersion, true );
            }
            else
            {
                FilterView.ViewName = "All " + SelectedObjectClass.ObjectClass.ToString() + " Search";
                FilterRelationship = FilterView.AddViewRelationship( SelectedObjectClass, true );
            }

            CswNbtViewProperty FilterProperty;
            if( SelectedNodeTypePropFirstVersionId != Int32.MinValue )
            {
                FilterProperty = FilterView.AddViewProperty( FilterRelationship, _CswNbtResources.MetaData.getNodeTypeProp( SelectedPropLatestVersion.PropId ) );
                CswNbtViewPropertyFilter Filter = FilterView.AddViewPropertyFilter( FilterProperty, SelectedSubField.Name, SelectedFilterMode, FilterValue.ToString(), false );
            }

            return FilterView;
        }
    }
}

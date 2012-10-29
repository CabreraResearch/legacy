using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    /// <summary>
    /// Represents a user-configurable table of components
    /// </summary>
    public class CswLayoutTable : CompositeControl
    {
        /// <summary>
        /// Defines a single component
        /// </summary>
        public class LayoutComponent
        {
            public Int32 LayoutComponentId;
            public Int32 DisplayRow;
            public Int32 DisplayColumn;
            public WebControl LabelControl;
            public WebControl ValueControl;
            public bool AllowDelete;

            public LayoutComponent( Int32 inLayoutComponentId, Int32 inDisplayRow, Int32 inDisplayColumn, WebControl inLabelControl, WebControl inValueControl, bool inAllowDelete )
            {
                // This prevents IIS from dying when very small or very large numbers are used.
                if( inDisplayRow < 0 || inDisplayRow > 200 )
                    throw new CswDniException( ErrorType.Error, "Invalid Row", "LayoutComponent got an invalid row:" + inDisplayRow.ToString() );
                if( inDisplayColumn < 0 || inDisplayColumn > 200 )
                    throw new CswDniException( ErrorType.Error, "Invalid Column", "LayoutComponent got an invalid column:" + inDisplayColumn.ToString() );

                LayoutComponentId = inLayoutComponentId;
                DisplayRow = inDisplayRow;
                DisplayColumn = inDisplayColumn;
                LabelControl = inLabelControl;
                ValueControl = inValueControl;
                AllowDelete = inAllowDelete;
            }

        }

        /// <summary>
        /// Collection of components to display
        /// </summary>
        public Dictionary<Int32, LayoutComponent> Components = new Dictionary<Int32, LayoutComponent>();

        /// <summary>
        /// If true, allow reordering of components in the page
        /// </summary>
        public bool EditMode
        {
            get { EnsureChildControls(); return _EditModeHiddenField.Value.ToLower() == "true"; }
            set { EnsureChildControls(); _EditModeHiddenField.Value = value.ToString().ToLower(); }
        }

        /// <summary>
        /// Whether Labels should be right-aligned
        /// </summary>
        public bool LabelCellRightAlign = true;

        public string CssClassLabelCell = string.Empty;
        public string CssClassValueCell = "ComponentValueCell";
        public string CssClassValueCellEditMode = "ComponentValueCellEditMode";
        public string CssClassEmptyCell = "ComponentEmptyValueCell";
        public string CssClassEmptyCellEditMode = "ComponentEmptyValueCellEditMode";
        public string TableTextAlign = "center";

        private CswNbtResources _CswNbtResources;
        private RadAjaxManager _AjaxManager;
        public CswLayoutTable( CswNbtResources CswNbtResources, RadAjaxManager AjaxManager )
        {
            _CswNbtResources = CswNbtResources;
            _AjaxManager = AjaxManager;
        }

        protected override void OnInit( EventArgs e )
        {
            EnsureChildControls();

            base.OnInit( e );
        }

        private PlaceHolder _ComponentPlaceHolder;
        private HiddenField _EditModeHiddenField;

        protected override void CreateChildControls()
        {
            _ComponentPlaceHolder = new PlaceHolder();
            this.Controls.Add( _ComponentPlaceHolder );

            _EditModeHiddenField = new HiddenField();
            _EditModeHiddenField.ID = "EditModeHiddenField";
            _EditModeHiddenField.Value = "false";
            this.Controls.Add( _EditModeHiddenField );

            base.CreateChildControls();
        }

        protected override void OnLoad( EventArgs e )
        {
            //ReinitComponents();
            base.OnLoad( e );
        }

        public void Clear()
        {
            EnsureChildControls();
            Components.Clear();
            _ComponentPlaceHolder.Controls.Clear(); // BEWARE
        }

        public void ReinitComponents()
        {
            _ComponentPlaceHolder.Controls.Clear(); // BEWARE

            CswAutoTable ComponentTable = new CswAutoTable();
            ComponentTable.ID = "ComponentTable";
            ComponentTable.CssClass = "ComponentTable";
            //ComponentTable.Style.Add( HtmlTextWriterStyle.TextAlign, TableTextAlign );
            ComponentTable.Attributes.Add( "align", TableTextAlign );
            _ComponentPlaceHolder.Controls.Add( ComponentTable );

            Int32 CellIncrement = 3;
            foreach( LayoutComponent Component in Components.Values )
            {
                TableCell LabelCell = ComponentTable.getCell( Component.DisplayRow - 1, ( Component.DisplayColumn - 1 ) * CellIncrement );
                TableCell ValueCell = ComponentTable.getCell( Component.DisplayRow - 1, ( Component.DisplayColumn - 1 ) * CellIncrement + 1 );
                TableCell ConfigCell = ComponentTable.getCell( Component.DisplayRow - 1, ( Component.DisplayColumn - 1 ) * CellIncrement + 2 );

                LabelCell.CssClass = CssClassLabelCell;
                if( EditMode )
                    ValueCell.CssClass = CssClassValueCellEditMode;
                else
                    ValueCell.CssClass = CssClassValueCell;

                if( Component.LabelControl != null )
                    LabelCell.Controls.Add( Component.LabelControl );
                if( Component.ValueControl != null )
                    ValueCell.Controls.Add( Component.ValueControl );

                if( LabelCellRightAlign )
                    LabelCell.Style.Add( HtmlTextWriterStyle.TextAlign, "right" );

                if( EditMode )
                {
                    CswAutoTable ButtonTable = new CswAutoTable();
                    ConfigCell.Controls.Add( ButtonTable );

                    if( Component.DisplayRow > 1 )
                    {
                        CswImageButton NorthButton = new CswImageButton( CswImageButton.ButtonType.ArrowNorth );
                        NorthButton.ID = "North_" + Component.LayoutComponentId;
                        NorthButton.Click += new EventHandler( MoveButton_Click );
                        ButtonTable.addControl( 0, 1, NorthButton );
                        _AjaxManager.AjaxSettings.AddAjaxSetting( NorthButton, ComponentTable );
                    }

                    CswImageButton EastButton = new CswImageButton( CswImageButton.ButtonType.ArrowEast );
                    EastButton.ID = "East_" + Component.LayoutComponentId;
                    EastButton.Click += new EventHandler( MoveButton_Click );
                    ButtonTable.addControl( 1, 2, EastButton );
                    _AjaxManager.AjaxSettings.AddAjaxSetting( EastButton, ComponentTable );

                    CswImageButton SouthButton = new CswImageButton( CswImageButton.ButtonType.ArrowSouth );
                    SouthButton.ID = "South_" + Component.LayoutComponentId;
                    SouthButton.Click += new EventHandler( MoveButton_Click );
                    ButtonTable.addControl( 2, 1, SouthButton );
                    _AjaxManager.AjaxSettings.AddAjaxSetting( SouthButton, ComponentTable );

                    if( Component.DisplayColumn > 1 )
                    {
                        CswImageButton WestButton = new CswImageButton( CswImageButton.ButtonType.ArrowWest );
                        WestButton.ID = "West_" + Component.LayoutComponentId;
                        WestButton.Click += new EventHandler( MoveButton_Click );
                        ButtonTable.addControl( 1, 0, WestButton );
                        _AjaxManager.AjaxSettings.AddAjaxSetting( WestButton, ComponentTable );
                    }

                    if( Component.AllowDelete )
                    {
                        CswImageButton DeleteButton = new CswImageButton( CswImageButton.ButtonType.Delete );
                        DeleteButton.ID = "Delete_" + Component.LayoutComponentId;
                        DeleteButton.Click += new EventHandler( DeleteButton_Click );
                        ButtonTable.addControl( 1, 1, DeleteButton );
                        _AjaxManager.AjaxSettings.AddAjaxSetting( DeleteButton, ComponentTable );
                    }

                } // if( EditMode )
            } // foreach( LayoutComponent Component in Components.Values )

            for( Int32 r = 0; r < ComponentTable.Rows.Count; r++ )
            {
                for( Int32 c = 0; c < ComponentTable.MaxCells; c++ )
                {
                    TableCell Cell = ComponentTable.getCell( r, c );
                    if( Cell.Controls.Count == 0 )
                    {
                        Cell.Controls.Add( new CswLiteralNbsp() );
                        if( c % CellIncrement == 1 )  // this is a Value Cell
                        {
                            if( EditMode )
                                Cell.CssClass = CssClassEmptyCellEditMode;
                            else
                                Cell.CssClass = CssClassEmptyCell;
                        }
                    }
                } // for( Int32 c = 0; c < ComponentTable.MaxCells; c++ )
            } // for( Int32 r = 0; r < ComponentTable.Rows.Count; r++ )
        } // ReinitComponents()

        public delegate void DeleteComponentEventHandler( Int32 LayoutComponentId );
        public event DeleteComponentEventHandler OnDeleteComponent = null;

        void DeleteButton_Click( object sender, EventArgs e )
        {
            string[] SplitID = ( (CswImageButton) sender ).ID.Split( '_' );
            Int32 LayoutComponentId = Convert.ToInt32( SplitID[1] );
            Components.Remove( LayoutComponentId );

            if( OnDeleteComponent != null )
                OnDeleteComponent( LayoutComponentId );

            ReinitComponents();
        }

        public delegate void MoveComponentEventHandler( Int32 LayoutComponentId, Int32 NewDisplayRow, Int32 NewDisplayColumn );
        public event MoveComponentEventHandler OnMoveComponent = null;

        void MoveButton_Click( object sender, EventArgs e )
        {
            string[] SplitID = ( (CswImageButton) sender ).ID.Split( '_' );
            string Direction = SplitID[0];
            Int32 LayoutComponentId = Convert.ToInt32( SplitID[1] );
            LayoutComponent Component = Components[LayoutComponentId];
            Int32 NewDisplayRow = Component.DisplayRow;
            Int32 NewDisplayColumn = Component.DisplayColumn;
            switch( Direction )
            {
                case "North": NewDisplayRow--; break;
                case "East": NewDisplayColumn++; break;
                case "South": NewDisplayRow++; break;
                case "West": NewDisplayColumn--; break;
            }
            // Find existing component in that slot
            if( NewDisplayRow > 0 && NewDisplayColumn > 0 )
            {
                LayoutComponent SwapComponent = null;
                foreach( LayoutComponent OtherComponent in Components.Values )
                {
                    if( OtherComponent.DisplayRow == NewDisplayRow && OtherComponent.DisplayColumn == NewDisplayColumn )
                    {
                        SwapComponent = OtherComponent;
                        break;
                    }
                }
                if( SwapComponent != null )
                {
                    SwapComponent.DisplayRow = Component.DisplayRow;
                    SwapComponent.DisplayColumn = Component.DisplayColumn;
                }
                Component.DisplayRow = NewDisplayRow;
                Component.DisplayColumn = NewDisplayColumn;

                if( OnMoveComponent != null )
                {
                    OnMoveComponent( Component.LayoutComponentId, Component.DisplayRow, Component.DisplayColumn );
                    if( SwapComponent != null )
                        OnMoveComponent( SwapComponent.LayoutComponentId, SwapComponent.DisplayRow, SwapComponent.DisplayColumn );
                }

                ReinitComponents();

            } // if( NewDisplayRow > 0 && NewDisplayColumn > 0 )
        } // MoveButton_Click()


        // Error handling
        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }

    }
}

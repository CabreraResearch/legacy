using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using ChemSW.CswWebControls;
using ChemSW.DB;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_WelcomeAdd : System.Web.UI.Page
    {
        #region Private Variables

        //public enum WelcomeComponentType
        //{
        //    Link,
        //    Search
        //}
        //public enum WelcomeForType
        //{
        //    User,
        //    Role,
        //    Global
        //}


        #endregion Private Variables


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
        } // OnInit()


        private DropDownList _ComponentTypeList;
        //private DropDownList _ViewList;
        private DropDownList _NodeTypeList;
        //private DropDownList _ForList;
        private TextBox _Text;
        private Literal _TypeLiteral;
        private Literal _ViewLiteral;
        private Literal _NodeTypeLiteral;
        private Literal _TextLiteral;
        //private Literal _ForLiteral;
        private Literal _ButtonIconLiteral;
        private DropDownList _ButtonIconList;
        private Image _ButtonIconImage;
        private CswViewListTree _ViewTree;

        protected override void CreateChildControls()
        {
            CswAutoTable FormTable = new CswAutoTable();
            ph.Controls.Add( FormTable );
            Int32 formrow = 0;

            _TypeLiteral = new Literal();
            _TypeLiteral.Text = "Type:&nbsp;";
            FormTable.addControl( formrow, 0, _TypeLiteral );

            _ComponentTypeList = new DropDownList();
            _ComponentTypeList.ID = "ComponentTypeList";
            foreach( CswWelcomeTable.WelcomeComponentType ComponentType in Enum.GetValues( typeof( CswWelcomeTable.WelcomeComponentType ) ) )
                _ComponentTypeList.Items.Add( ComponentType.ToString() );
            _ComponentTypeList.AutoPostBack = true;
            FormTable.addControl( formrow++, 1, _ComponentTypeList );

            _ViewLiteral = new Literal();
            _ViewLiteral.Text = "View:&nbsp;";
            FormTable.addControl( formrow, 0, _ViewLiteral );

            //_ViewList = new DropDownList();
            //_ViewList.ID = "ViewList";
            //_initViewList( _ViewList );
            //FormTable.addControl( formrow++, 1, _ViewList );

            _ViewTree = new CswViewListTree( Master.CswNbtResources, false );
            _ViewTree.ID = "viewtree";
            //_ViewTree.ViewSelected += new CswViewListTree.ViewSelectedEventHandler( _ViewTree_ViewSelected );
            _ViewTree.OnError += new CswErrorHandler( Master.HandleError );
            //if( Master.CswNbtView != null )
            //    _ViewTree.SelectedViewId = Master.CswNbtView.ViewId;
            //else if( Master.ActionId > 0 )
            //    _ViewTree.SelectedActionId = Master.ActionId;
            FormTable.addControl( formrow++, 1, _ViewTree );

            _NodeTypeLiteral = new Literal();
            _NodeTypeLiteral.Text = "Add new:&nbsp;";
            FormTable.addControl( formrow, 0, _NodeTypeLiteral );

            _NodeTypeList = new DropDownList();
            _NodeTypeList.ID = "NodeTypeList";
            _initNodeTypeList( _NodeTypeList );
            FormTable.addControl( formrow++, 1, _NodeTypeList );

            _TextLiteral = new Literal();
            _TextLiteral.Text = "Text:&nbsp;";
            FormTable.addControl( formrow, 0, _TextLiteral );

            _Text = new TextBox();
            _Text.ID = "Text";
            FormTable.addControl( formrow++, 1, _Text );

            //_ForLiteral = new Literal();
            //_ForLiteral.Text = "For:&nbsp;";
            //FormTable.addControl( formrow, 0, _ForLiteral );

            //_ForList = new DropDownList();
            //_ForList.ID = "ForList";
            //_ForList.Items.Add( new ListItem( "Me", CswWelcomeTable.WelcomeForType.User.ToString() ) );
            //_ForList.Items.Add( new ListItem( "My Role", CswWelcomeTable.WelcomeForType.Role.ToString() ) );
            //if( Master.CswNbtResources.CurrentNbtUser.IsAdministrator() )
            //    _ForList.Items.Add( new ListItem( "Everyone", CswWelcomeTable.WelcomeForType.Global.ToString() ) );
            //FormTable.addControl( formrow++, 1, _ForList );


            //Literal RowLiteral = new Literal();
            //RowLiteral.Text = "Row:&nbsp;";
            //FormTable.addControl( formrow, 0, RowLiteral );

            //_ComponentRow = new TextBox();
            //_ComponentRow.ID = "ComponentRow";
            //_ComponentRow.Width = Unit.Parse( "30px" );
            //FormTable.addControl( formrow++, 1, _ComponentRow );

            //Literal ColumnLiteral = new Literal();
            //ColumnLiteral.Text = "Column:&nbsp;";
            //FormTable.addControl( formrow, 0, ColumnLiteral );

            //_ComponentCol = new TextBox();
            //_ComponentCol.ID = "ComponentCol";
            //_ComponentCol.Width = Unit.Parse( "30px" );
            //FormTable.addControl( formrow++, 1, _ComponentCol );

            _ButtonIconLiteral = new Literal();
            _ButtonIconLiteral.Text = "Use Button:&nbsp;";
            FormTable.addControl( formrow, 0, _ButtonIconLiteral );

            _ButtonIconList = new DropDownList();
            _ButtonIconList.ID = "ButtonIconList";
            FormTable.addControl( formrow++, 1, _ButtonIconList );

            _ButtonIconImage = new Image();
            _ButtonIconImage.ID = "ButtonIconImage";
            FormTable.addControl( formrow++, 1, _ButtonIconImage );

            _initButtonIconList( _ButtonIconList, _ButtonIconImage );

            Button AddButton = new Button();
            AddButton.ID = "AddButton";
            AddButton.CssClass = "Button";
            AddButton.Text = "Add";
            AddButton.Click += new EventHandler( AddButton_Click );
            FormTable.addControl( formrow++, 1, AddButton );

            base.CreateChildControls();
        } // CreateChildControls()

        protected override void OnLoad( EventArgs e )
        {
            if( _SelectedComponentType == CswWelcomeTable.WelcomeComponentType.Link )
            {
                _ViewTree.IncludeActions = true;
                _ViewTree.IncludeReports = true;
                _ViewTree.SearchableViewsOnly = false;
            }
            else if( _SelectedComponentType == CswWelcomeTable.WelcomeComponentType.Search )
            {
                _ViewTree.IncludeActions = false;
                _ViewTree.IncludeReports = false;
                _ViewTree.SearchableViewsOnly = true;
            }
            _ViewTree.DataBind();

            base.OnLoad( e );
        }

        private CswWelcomeTable.WelcomeComponentType _SelectedComponentType
        {
            get
            {
                return (CswWelcomeTable.WelcomeComponentType) Enum.Parse( typeof( CswWelcomeTable.WelcomeComponentType ), _ComponentTypeList.SelectedValue );
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            switch( _SelectedComponentType )
            {
                case CswWelcomeTable.WelcomeComponentType.Add:
                    //_ViewList.Visible = false;
                    _ViewTree.Visible = false;
                    _ViewLiteral.Visible = false;
                    _NodeTypeList.Visible = true;
                    _NodeTypeLiteral.Visible = true;
                    _Text.Visible = true;
                    _TextLiteral.Visible = true;
                    _ButtonIconLiteral.Visible = true;
                    _ButtonIconList.Visible = true;
                    _ButtonIconImage.Visible = true;
                    break;
                case CswWelcomeTable.WelcomeComponentType.Link:
                    //_ViewList.Visible = true;
                    _ViewTree.Visible = true;
                    _ViewLiteral.Visible = true;
                    _NodeTypeList.Visible = false;
                    _NodeTypeLiteral.Visible = false;
                    _Text.Visible = true;
                    _TextLiteral.Visible = true;
                    _ButtonIconLiteral.Visible = true;
                    _ButtonIconList.Visible = true;
                    _ButtonIconImage.Visible = true;
                    break;
                case CswWelcomeTable.WelcomeComponentType.Search:
                    //_ViewList.Visible = true;
                    _ViewTree.Visible = true;
                    _ViewLiteral.Visible = true;
                    _NodeTypeList.Visible = false;
                    _NodeTypeLiteral.Visible = false;
                    _Text.Visible = false;
                    _TextLiteral.Visible = false;
                    _ButtonIconLiteral.Visible = false;
                    _ButtonIconList.Visible = false;
                    _ButtonIconImage.Visible = false;
                    break;
                case CswWelcomeTable.WelcomeComponentType.Text:
                    _ViewTree.Visible = false;
                    //_ViewList.Visible = false;
                    _ViewLiteral.Visible = false;
                    _NodeTypeList.Visible = false;
                    _NodeTypeLiteral.Visible = false;
                    _Text.Visible = true;
                    _TextLiteral.Visible = true;
                    _ButtonIconLiteral.Visible = false;
                    _ButtonIconList.Visible = false;
                    _ButtonIconImage.Visible = false;
                    break;
            }

            _ButtonIconImage.ImageUrl = CswWelcomeTable.IconImageRoot + "/" + "blank.gif";
            _ButtonIconList.Attributes.Add( "onchange", _ButtonIconImage.ClientID + ".src = '" + CswWelcomeTable.IconImageRoot + "/'+ this.value;" );

            base.OnPreRender( e );
        }

        #endregion Page Lifecycle


        #region Events

        void AddButton_Click( object sender, EventArgs e )
        {
            try
            {
                //                string SqlText = @"select max(display_row) maxcol
                //                                     from welcome
                //                                    where display_col = 1
                //                                      and (userid is null or userid = " + Master.CswNbtResources.CurrentNbtUser.UserId.PrimaryKey.ToString() + @")
                //                                      and (roleid is null or roleid = " + Master.CswNbtResources.CurrentNbtUser.RoleId.PrimaryKey.ToString() + @")";
                //                CswArbitrarySelect WelcomeSelect = Master.CswNbtResources.makeCswArbitrarySelect( "AddButton_Click_WelcomeSelect", SqlText );
                //                DataTable WelcomeSelectTable = WelcomeSelect.getTable();
                //                Int32 MaxRow = 0;
                //                if( WelcomeSelectTable.Rows.Count > 0 )
                //                    MaxRow = CswConvert.ToInt32( WelcomeSelectTable.Rows[0]["maxcol"] );

                CswTableUpdate WelcomeUpdate = Master.CswNbtResources.makeCswTableUpdate( "AddButton_Click_WelcomeUpdate", "welcome" );
                DataTable WelcomeTable = WelcomeUpdate.getEmptyTable();
                //CswWelcomeTable.WelcomeComponentType SelectedComponentType = (CswWelcomeTable.WelcomeComponentType) Enum.Parse( typeof( CswWelcomeTable.WelcomeComponentType ), _ComponentTypeList.SelectedValue );
                //CswWelcomeTable.WelcomeForType SelectedForType = (CswWelcomeTable.WelcomeForType) Enum.Parse( typeof( CswWelcomeTable.WelcomeForType ), _ForList.SelectedValue );
                string SelectedButtonIcon = _ButtonIconList.SelectedValue;

                CswWelcomeTable.AddWelcomeComponent( Master.CswNbtResources, WelcomeTable, _SelectedComponentType,
                                                     _ViewTree.SelectedType, _ViewTree.SelectedValue,
                                                     Convert.ToInt32( _NodeTypeList.SelectedValue ), _Text.Text,
                                                     Int32.MinValue, Int32.MinValue, SelectedButtonIcon,
                                                     Master.CswNbtResources.CurrentNbtUser.RoleId.PrimaryKey );

                //DataRow NewWelcomeRow = WelcomeTable.NewRow();
                //if( SelectedButtonIcon == "blank.gif" )
                //    SelectedButtonIcon = string.Empty;

                //NewWelcomeRow["componenttype"] = SelectedComponentType.ToString();
                //NewWelcomeRow["display_row"] = CswConvert.ToDbVal( Convert.ToInt32( _ComponentRow.Text ) );
                //NewWelcomeRow["display_col"] = CswConvert.ToDbVal( Convert.ToInt32( _ComponentCol.Text ) );
                //NewWelcomeRow["display_col"] = "1";
                //NewWelcomeRow["display_row"] = MaxRow + 1;
                //switch( SelectedComponentType )
                //{
                //    case CswWelcomeTable.WelcomeComponentType.Add:
                //        NewWelcomeRow["nodetypeid"] = CswConvert.ToDbVal( Convert.ToInt32( _NodeTypeList.SelectedValue ) );
                //        NewWelcomeRow["buttonicon"] = SelectedButtonIcon;
                //        break;
                //    case CswWelcomeTable.WelcomeComponentType.Link:
                //        //NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( Convert.ToInt32( _ViewList.SelectedValue ) );
                //        switch( _ViewTree.SelectedType )
                //        {
                //            case CswViewListTree.ViewType.View:
                //                NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( _ViewTree.SelectedValue );
                //                break;
                //            case CswViewListTree.ViewType.Action:
                //                NewWelcomeRow["actionid"] = CswConvert.ToDbVal( _ViewTree.SelectedValue );
                //                break;
                //            case CswViewListTree.ViewType.Report:
                //                NewWelcomeRow["reportid"] = CswConvert.ToDbVal( _ViewTree.SelectedValue );
                //                break;
                //            default:
                //                throw new CswDniException( "You must select a view", "No view was selected for new Welcome Page Component" );
                //        }
                //        NewWelcomeRow["buttonicon"] = SelectedButtonIcon;
                //        break;
                //    case CswWelcomeTable.WelcomeComponentType.Search:
                //        //NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( Convert.ToInt32( _ViewList.SelectedValue ) );
                //        if( _ViewTree.SelectedType == CswViewListTree.ViewType.View )
                //            NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( _ViewTree.SelectedValue );
                //        else
                //            throw new CswDniException( "You must select a view", "No view was selected for new Welcome Page Component" );
                //        break;
                //    case CswWelcomeTable.WelcomeComponentType.Text:
                //        NewWelcomeRow["displaytext"] = _Text.Text;
                //        break;
                //}
                //switch( SelectedForType )
                //{
                //    case CswWelcomeTable.WelcomeForType.User:
                //        NewWelcomeRow["userid"] = Master.CswNbtResources.CurrentNbtUser.UserId.PrimaryKey.ToString();
                //        break;
                //    case CswWelcomeTable.WelcomeForType.Role:
                //        NewWelcomeRow["roleid"] = Master.CswNbtResources.CurrentNbtUser.RoleId.PrimaryKey.ToString();
                //        break;
                //}
                //WelcomeTable.Rows.Add( NewWelcomeRow );
                WelcomeUpdate.update( WelcomeTable );

                string JS = @"<script language=""Javascript"">Popup_OK_Clicked();</script>";
                ScriptManager.RegisterClientScriptBlock( this, this.GetType(), this.UniqueID + "_JS", JS, false );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // AddButton_Click()


        #endregion Events

        private void _initNodeTypeList( DropDownList NodeTypeList )
        {
            _NodeTypeList.Items.Clear();
            foreach( CswNbtMetaDataNodeType Nodetype in Master.CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                _NodeTypeList.Items.Add( new ListItem( Nodetype.NodeTypeName, Nodetype.FirstVersionNodeTypeId.ToString() ) );
            }
        } // _initNodeTypeList()

        private void _initViewList( DropDownList ViewList )
        {
            //bool ret = false;
            ViewList.Items.Clear();
            DataTable Views = null;
            //if( Master.CswNbtResources.CurrentUser.IsAdministrator() )
            Views = Master.CswNbtResources.ViewSelect.getVisibleViews( false );
            //else
            //Views = CswNbtView.getUserViews( Master.CswNbtResources );

            if( Views.Rows.Count > 0 )
            {
                //ret = true;
                foreach( DataRow Row in Views.Rows )
                {
                    ViewList.Items.Add( new ListItem( Row["viewname"].ToString(), Row["nodeviewid"].ToString() ) );
                }
            }
            //return ret;
        } // _initViewList()

        private void _initButtonIconList( DropDownList IconList, Image IconImage )
        {
            // Icon select box
            IconList.Items.Clear();
            ListItem BlankIconItem = new ListItem( "", "blank.gif" );
            IconList.Items.Add( BlankIconItem );
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo( HttpContext.Current.Request.PhysicalApplicationPath + CswWelcomeTable.IconImageRoot );
            System.IO.FileInfo[] IconFiles = d.GetFiles();
            foreach( System.IO.FileInfo IconFile in IconFiles )
            {
                if( IconFile.Name != "blank.gif" &&
                    ( IconFile.Name.EndsWith( ".gif" ) || IconFile.Name.EndsWith( ".jpg" ) ) )
                {
                    ListItem IconItem = new ListItem( IconFile.Name, IconFile.Name );
                    IconList.Items.Add( IconItem );
                }
            }
        } // _initButtonIconList()

    } // class Popup_WelcomeAdd
} // namespace ChemSW.Nbt.WebPages

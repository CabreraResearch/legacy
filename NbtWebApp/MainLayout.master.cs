using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;
using ChemSW.NbtWebControls;
using ChemSW.Security;
using ChemSW.Session;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class MainLayout : System.Web.UI.MasterPage
    {
        public CswPropertyFilter _CswPropertyFilter;
        //public Button SearchButton;
        //private CswAutoTable SearchTable;
        //private Literal SearchLiteral;
        private CswQuickLaunch _QuickLaunch;
        //private LinkButton SearchLink;
        //private HiddenField SearchLinkStatusHF;
        public CswMessageBox ErrorBox { get { return this._ErrorBox; } }
        private RadMenuItem HomeMenuItem;
        private RadMenuItem AdminMenuItem;
        private RadMenuItem UserListMenuItem;
        private RadMenuItem StatisticsMenuItem;
        private RadMenuItem LogMenuItem;
        private RadMenuItem ConfigVarsMenuItem;
        private RadMenuItem PrefsMenuItem;
        private RadMenuItem HelpTopMenuItem;
        private RadMenuItem HelpMenuItem;
        private RadMenuItem AboutMenuItem;
        private RadMenuItem LogoutMenuItem;
        private RadMenuItem RemoveDemoDataItem;
        private Button HiddenChangeViewButton;
        private HiddenField HiddenChangeViewId;
        public CswMainMenu MainMenu;
        //public CswImageButton StatusImage;

        public bool HideContent = false;

        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
                CswTimer Timer = new CswTimer();
                LogTimerResult( "MainLayout.OnInit() started", Timer.ElapsedDurationInSecondsAsString );

                Master.OnError += new CswErrorHandler( HandleError );

                //StatusImage = new CswImageButton( CswImageButton.ButtonType.SaveStatus );
                //StatusImage.ID = "StatusImage";
                //StatusImagePH.Controls.Add( StatusImage );

                //SearchLink = new LinkButton();
                //SearchLink.Text = "Search";
                //SearchLink.Click += new EventHandler( SearchLink_Click );
                //SearchPlaceHolder.Controls.Add( SearchLink );

                //SearchLinkStatusHF = new HiddenField();
                //SearchLinkStatusHF.ID = "SearchLinkStatusHF";
                //SearchLinkStatusHF.Value = "0";
                //SearchPlaceHolder.Controls.Add( SearchLinkStatusHF );

                //SearchLiteral = new CswLiteralNbsp();
                //SearchPlaceHolder.Controls.Add( SearchLiteral );

                MainMenu = new CswMainMenu( Master.CswNbtResources );
                MainMenu.ID = "mainmenu";
                MainMenu.OnError += new CswErrorHandler( Master.HandleError );
                MainMenu.AllowBatch = false;
                MainMenu.AllowMobile = false;
                MainMenu.AllowEditView = false;
                MainMenu.AllowChangeView = true;
                MenuPlaceHolder.Controls.Add( MainMenu );

                HiddenChangeViewButton = new Button();
                HiddenChangeViewButton.ID = "HiddenChangeViewButton";
                HiddenChangeViewButton.Style.Add( HtmlTextWriterStyle.Display, "none" );
                HiddenChangeViewButton.Click += new EventHandler( HiddenChangeViewButton_Click );
                MenuPlaceHolder.Controls.Add( HiddenChangeViewButton );

                HiddenChangeViewId = new HiddenField();
                HiddenChangeViewId.ID = "HiddenChangeViewId";
                MenuPlaceHolder.Controls.Add( HiddenChangeViewId );

                if( Master.CswNbtResources.CurrentUser != null )
                {
                    _QuickLaunch = new CswQuickLaunch( Master.CswNbtResources );
                    _QuickLaunch.OnError += new CswErrorHandler( Master.HandleError );
                    _QuickLaunch.OnActionLinkClick += new CswQuickLaunch.ActionLinkClickEvent( _QuickLaunch_OnActionLinkClick );
                    _QuickLaunch.OnSessionViewLinkClick += new CswQuickLaunch.SessionViewLinkClickEvent( _QuickLaunch_OnSessionViewLinkClick );
                    _QuickLaunch.OnViewLinkClick += new CswQuickLaunch.ViewLinkClickEvent( _QuickLaunch_OnViewLinkClick );
                    quicklaunchplaceholder.Controls.Add( _QuickLaunch );
                }

                if( Master.CswNbtResources.ConfigVbls.getConfigVariableValue( "showloadbox" ) != "1" )
                {
                    ProgressDiv.Visible = false;
                }
                else
                {
                    ProgressDiv.Visible = true;
                }
                LogTimerResult( "MainLayout.OnInit() finished", Timer.ElapsedDurationInSecondsAsString );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnInit( e );
        }

        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                //AjaxManager.AjaxSettings.AddAjaxSetting( SearchLink, SearchPlaceHolder );

                HomeMenuItem = RightHeaderMenu.FindItemByValue( "HomeMenuItem" );
                AdminMenuItem = RightHeaderMenu.FindItemByValue( "AdminMenuItem" );
                UserListMenuItem = RightHeaderMenu.FindItemByValue( "UserListMenuItem" );
                LogMenuItem = RightHeaderMenu.FindItemByValue( "LogMenuItem" );
                ConfigVarsMenuItem = RightHeaderMenu.FindItemByValue( "ConfigVarsMenuItem" );
                PrefsMenuItem = RightHeaderMenu.FindItemByValue( "PrefsMenuItem" );
                HelpTopMenuItem = RightHeaderMenu.FindItemByValue( "HelpTopMenuItem" );
                HelpMenuItem = RightHeaderMenu.FindItemByValue( "HelpMenuItem" );
                AboutMenuItem = RightHeaderMenu.FindItemByValue( "AboutMenuItem" );
                LogoutMenuItem = RightHeaderMenu.FindItemByValue( "LogoutMenuItem" );
                StatisticsMenuItem = RightHeaderMenu.FindItemByValue( "StatisticsMenuItem" );
                RemoveDemoDataItem = RightHeaderMenu.FindItemByValue( "RemoveDemoDataItem" );

                if( !HideContent && Master.IsAuthenticated() && Page.AppRelativeVirtualPath != @"~/License.aspx" )
                {
                    //if( SearchLinkStatusHF.Value == "1" )
                    //    _makeSearchForm();

                    MainMenu.Visible = true;

                    ////_QuickLaunch.UserId = Master.CswNbtResources.CurrentUser.UserId;
                    //_QuickLaunch.PreviousView1 = PreviousView1;
                    //_QuickLaunch.PreviousView2 = PreviousView2;
                    //_QuickLaunch.PreviousView3 = PreviousView3;
                    //_QuickLaunch.PreviousView4 = PreviousView4;
                    //_QuickLaunch.PreviousView5 = PreviousView5;
                    //_QuickLaunch.DataBind();
                    ResetQuickLaunch();

                    CswNbtResources.CswEventLinker.Subscribe( "MainLayoutQuickLaunch_AfterDeleteView", CswNbtView.AfterDeleteViewEventName, (CswNbtView.AfterDeleteViewEventHandler) ResetQuickLaunch );
                    CswNbtResources.CswEventLinker.Subscribe( "CswViewListTree_AfterViewDelete", CswNbtView.AfterDeleteViewEventName, (CswNbtView.AfterDeleteViewEventHandler) _AfterDeleteView );
                    CswNbtResources.CswEventLinker.Subscribe( "CswViewListTree_AfterModifyReport", CswNbtObjClassReport.AfterModifyReportEventName, (CswNbtObjClassReport.AfterModifyReportEventHandler) _AfterModifyReport );

                    UserLabel.Text = Master.CswNbtResources.CurrentUser.Username;

                    AdminMenuItem.Visible = false;
                    //if( Master.CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    //{
                    //    AdminMenuItem.Visible = true;
                    //}

                    //BrandTitle.Visible = false;
                    //BrandTitle2.Visible = false;
                    //BrandIcon.Visible = false;
                    //TitleSpacer.Visible = false;
                    //TitleSpacer2.Visible = false;

                    //string BrandPageTitle = Master.CswNbtResources.ConfigVbls.getConfigVariableValue( "brand_pagetitle" );
                    //if( BrandPageTitle != string.Empty )
                    //{
                    //    BrandTitle.Text = BrandPageTitle;
                    //    BrandTitle.Visible = true;
                    //    BrandTitle2.Text = BrandPageTitle;
                    //    BrandTitle2.Visible = true;
                    //    TitleSpacer.Visible = true;
                    //    TitleSpacer2.Visible = true;
                    //}
                    //string BrandPageIcon = Master.CswNbtResources.ConfigVbls.getConfigVariableValue( "brand_pageicon" );
                    //if( BrandPageIcon != string.Empty )
                    //{
                    //    BrandIcon.ImageUrl = BrandPageIcon;
                    //    BrandIcon.Visible = true;
                    //}

                    HelpMenuItem.CssClass = "SubMenuGroup";
                    AboutMenuItem.CssClass = "SubMenuGroup";
                    LogMenuItem.CssClass = "SubMenuGroup";
                    UserListMenuItem.CssClass = "SubMenuGroup";
                    StatisticsMenuItem.CssClass = "SubMenuGroup";
                    ConfigVarsMenuItem.CssClass = "SubMenuGroup";
                    RemoveDemoDataItem.CssClass = "SubMenuGroup";

                    RemoveDemoDataItem.Visible = false;
                    if( "1" == Master.CswNbtResources.ConfigVbls.getConfigVariableValue( "is_demo".ToLower() ) )
                    {
                        RemoveDemoDataItem.Visible = true;
                    }
                    //SearchLiteral.Visible = false;

                    // Set dashboard module lights
                    DashboardTable.Visible = true;

                    dash_imcs.Visible = false;
                    dash_si.Visible = false;
                    dash_fe.Visible = false;
                    dash_ccpro.Visible = false;
                    dash_cispro.Visible = false;
                    dash_hh.Visible = false;
                    dash_stis.Visible = false;
                    dash_biosafety.Visible = false;
                    dash_nbtmgr.Visible = false;

                    dash_si_off.Visible = true;
                    dash_fe_off.Visible = true;
                    dash_ccpro_off.Visible = true;
                    dash_cispro_off.Visible = true;
                    dash_imcs_off.Visible = true;
                    dash_hh_off.Visible = true;
                    dash_stis_off.Visible = true;
                    dash_biosafety_off.Visible = true;
                    dash_nbtmgr_off.Visible = true;

                    if( Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
                    {
                        dash_imcs.Visible = true;
                        dash_imcs_off.Visible = false;
                    }
                    if( Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SI ) )
                    {
                        dash_si.Visible = true;
                        dash_si_off.Visible = false;
                    }
                    if( Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CCPro ) )
                    {
                        dash_ccpro.Visible = true;
                        dash_ccpro_off.Visible = false;
                    }
                    if( Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                    {
                        dash_cispro.Visible = true;
                        dash_cispro_off.Visible = false;
                    }
                    if( Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.STIS ) )
                    {
                        dash_stis.Visible = true;
                        dash_stis_off.Visible = false;
                    }
                    if( Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.BioSafety ) )
                    {
                        dash_biosafety.Visible = true;
                        dash_biosafety_off.Visible = false;
                    }
                    if( Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.NBTManager ) )
                    {
                        dash_nbtmgr.Visible = true;
                        dash_nbtmgr_off.Visible = false;
                    }
                }
                else
                {
                    UserLabel.Visible = false;
                    PrefsMenuItem.Visible = false;
                    LogoutMenuItem.Visible = false;
                    HomeMenuItem.Visible = false;
                    HelpTopMenuItem.Visible = false;
                    HelpMenuItem.Visible = false;
                    AboutMenuItem.Visible = false;
                    AdminMenuItem.Visible = false;
                    //SearchLiteral.Visible = true;
                    //SearchLink.Visible = false;
                    DashboardTable.Visible = false;
                    //StatusImage.Visible = false;
                    MainMenu.Visible = false;
                }
                MainMenu.DataBind();
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }


        }

        #endregion Page Lifecycle

        #region Events

        private void _AfterDeleteView()
        {
            CswViewListTree.AfterDeleteView( Page.Session );
        }
        private void _AfterModifyReport()
        {
            CswViewListTree.AfterModifyReport( Page.Session );
        }

        protected void RightHeaderMenu_OnItemSelectedHandler( object sender, RadMenuEventArgs e )
        {
            try
            {
                switch( e.Item.Value )
                {
                    case "HomeMenuItem":
                        {
                            //clearView();
                            //setViewId( CswNbtResources.CurrentNbtUser.DefaultViewId, true );
                            //Master.GoMain();
                            GoHome();
                            break;
                        }
                    case "UserListMenuItem":
                        {
                            Redirect( "UserList.aspx" );
                            break;
                        }
                    case "StatisticsMenuItem":
                        {
                            Redirect( "Statistics.aspx" );
                            break;
                        }
                    case "HelpMenuItem":
                        {
                            // Client-side
                            break;
                        }
                    case "AboutMenuItem":
                        {
                            // Client-side
                            break;
                        }
                    case "ProfileMenuItem":
                        {
                            CswNbtMetaDataObjectClass UserObjectClass =
                                Master.CswNbtResources.MetaData.getObjectClass(
                                    NbtObjectClass.UserClass );

                            CswNbtView UserView = new CswNbtView( Master.CswNbtResources );
                            UserView.ViewName = "Preferences";
                            CswNbtViewRelationship UserRelationship = UserView.AddViewRelationship( UserObjectClass,
                                                                                                    true );
                            UserRelationship.NodeIdsToFilterIn.Add( Master.CswNbtResources.CurrentUser.UserId );

                            ICswNbtTree PrefsTree = Master.CswNbtResources.Trees.getTreeFromView( Master.CswNbtResources.CurrentNbtUser, UserView, true, false, false );
                            PrefsTree.goToNthChild( 0 );
                            Session["Main_SelectedNodeKey"] = PrefsTree.getNodeKeyForCurrentPosition().ToString();

                            Master.setViewXml( UserView.ToString() );
                            Master.GoMain();
                            break;
                        }
                    case "SubscriptionsMenuItem":
                        {
                            Master.Redirect( "Subscriptions.aspx" );
                            break;
                        }
                    case "LogoutMenuItem":
                        {
                            Logout();
                            break;
                        }
                    case "LogMenuItem":
                        {
                            Master.Redirect( "DisplayLog.aspx" );
                            break;
                        }
                    case "ConfigVarsMenuItem":
                        {
                            Master.Redirect( "ConfigVars.aspx" );
                            break;
                        }
                    case "RemoveDemoDataItem":
                        {
                            CswDemoDataManager DemoDataManager = new CswDemoDataManager( Master.CswNbtResources );
                            DemoDataManager.RemoveDemoData();
                            break;
                        }
                    default:
                        {
                            Master.Redirect( "Welcome.aspx" );
                            break;
                        }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        //void SearchLink_Click( object sender, EventArgs e )
        //{
        //    _makeSearchForm();
        //}

        public void ResetQuickLaunch()
        {
            _QuickLaunch.PreviousView1 = PreviousView1;
            _QuickLaunch.PreviousView2 = PreviousView2;
            _QuickLaunch.PreviousView3 = PreviousView3;
            _QuickLaunch.PreviousView4 = PreviousView4;
            _QuickLaunch.PreviousView5 = PreviousView5;
            _QuickLaunch.DataBind();
        }


        //protected void SearchButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Make a view with the search parameters
        //        CswNbtView SearchView = new CswNbtView( CswNbtResources );

        //        CswNbtViewRelationship SearchRelationship;
        //        if( _CswPropertyFilter.SelectedNodeTypeLatestVersion != null )
        //        {
        //            SearchView.ViewName = _CswPropertyFilter.SelectedNodeTypeLatestVersion.NodeTypeName + " Search";
        //            SearchRelationship = SearchView.AddViewRelationship( _CswPropertyFilter.SelectedNodeTypeLatestVersion );
        //        }
        //        else
        //        {
        //            SearchView.ViewName = "All " + _CswPropertyFilter.SelectedObjectClass.ObjectClass.ToString() + " Search";
        //            SearchRelationship = SearchView.AddViewRelationship( _CswPropertyFilter.SelectedObjectClass );
        //        }

        //        CswNbtViewProperty SearchProperty;
        //        if( _CswPropertyFilter.SelectedNodeTypePropFirstVersionId != Int32.MinValue )
        //        {
        //            SearchProperty = SearchView.AddViewProperty( SearchRelationship, CswNbtResources.MetaData.getNodeTypeProp( _CswPropertyFilter.SelectedPropLatestVersion.PropId ) );
        //            //SearchProperty.NodeTypePropId = _CswPropertyFilter.SelectedPropLatestVersion.PropId;
        //            //SearchProperty.Type = CswNbtPropType.NodeTypePropId;
        //        }
        //        else
        //        {
        //            SearchProperty = SearchView.AddViewProperty( SearchRelationship, CswNbtResources.MetaData.getObjectClassProp( _CswPropertyFilter.SelectedObjectClassPropId ) );
        //            //SearchProperty.ObjectClassPropId = _CswPropertyFilter.SelectedObjectClassPropId;
        //            //SearchProperty.Type = CswNbtPropType.ObjectClassPropId;
        //        }
        //        //SearchRelationship.addProperty( SearchProperty );

        //        CswNbtViewPropertyFilter SearchPropFilter = SearchView.AddViewPropertyFilter( SearchProperty, _CswPropertyFilter.SelectedSubField.Name, _CswPropertyFilter.SelectedFilterMode, _CswPropertyFilter.FilterValue.ToString(), false );
        //        //SearchPropFilter.SubfieldName = _CswPropertyFilter.SelectedSubField.Name;
        //        //SearchPropFilter.FilterMode = _CswPropertyFilter.SelectedFilterMode;
        //        //SearchPropFilter.Value = _CswPropertyFilter.FilterValue.ToString();
        //        //SearchProperty.addFilter(SearchPropFilter);

        //        Master.setViewXml(SearchView.ToString());

        //        Master.HandleSearch( SearchProperty );

        //        Master.Redirect("Main.aspx");
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleError(ex);
        //    }
        //}

        public void HandleError( Exception ex )
        {
            // See BZ 6662
            if( !( ex is System.Threading.ThreadAbortException ) )
            {
                if( CswNbtResources != null )
                {
                    CswNbtResources.AnErrorOccurred = true;
                    CswNbtResources.logError( ex );

                    // Display the error in the ErrorBox
                    if( CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" )
                    {
                        string Title;
                        string Message;
                        if( ex is CswDniException )
                        {
                            Title = "Error: " + ( (CswDniException) ex ).MsgFriendly;
                            Message = ( (CswDniException) ex ).MsgEscoteric;
                        }
                        else
                        {
                            Title = "Error: An internal error occurred.";
                            Message = ex.Message;
                        }
                        Message += "<br/>Time: " + DateTime.Now.ToString();
                        Message += "<br/>Stack Trace: <br/>" + ex.StackTrace.Replace( "\n", "<br/>\n" ) + "<br/><br/>";

                        //Title = DateTime.Now.ToString() + ": " + Title;

                        ErrorBox.addMessage( Title, Message );
                    }
                }

                //if (Session["errors"] == null)
                //{
                //    Session.Add("errors", ex.Message);
                //}
                //else
                //{
                //    Session["errors"] = Session["errors"].ToString() + "|" + ex.Message;
                //}

                if( ex.InnerException != null )
                {
                    HandleError( ex.InnerException );
                }
            }
            else
            {
                throw ex;
            }
            Master.HandleAfterError( ex );
        }

        protected void HiddenChangeViewButton_Click( object sender, EventArgs e )
        {
            try
            {
                string SelectedNodeValue = ( (HiddenField) HiddenChangeViewId ).Value;
                string[] SplitValue = SelectedNodeValue.Split( '_' );
                CswViewListTree.ViewType SelectedViewType = (CswViewListTree.ViewType) Enum.Parse( typeof( CswViewListTree.ViewType ), SplitValue[0] );
                ChangeMainView( SelectedViewType, CswConvert.ToInt32( SplitValue[1] ) );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // _ViewTree_ViewSelected()

        public void ChangeMainView( CswViewListTree.ViewType ViewType, Int32 Pk )
        {
            switch( ViewType )
            {
                case CswViewListTree.ViewType.View:
                    Master.setViewId( new CswNbtViewId( Pk ) );
                    Master.GoMain();
                    break;
                case CswViewListTree.ViewType.RecentView:
                    CswNbtView View = Master.CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( Pk ) );
                    Master.setViewXml( View.ToXml().InnerXml.ToString() );
                    Master.GoMain();
                    break;
                case CswViewListTree.ViewType.Action:
                    CswNbtAction Action = Master.CswNbtResources.Actions[Pk];
                    Master.setAction( Action.ActionId );
                    Master.Redirect( Action.Url );
                    break;
                case CswViewListTree.ViewType.Report:
                    Master.HandleLoadReport( new CswPrimaryKey( "nodes", Pk ) );
                    Master.Redirect( "Report.aspx?reportid=" + Pk.ToString() );
                    break;
                default:
                    throw new CswDniException( ErrorType.Error, "Invalid Selection", "MainLayout.master.cs::ChangeMainView() got an invalid ViewType: " + ViewType.ToString() );
            } // switch( SelectedViewType )
        } // ChangeMainView()



        protected void _QuickLaunch_OnViewLinkClick( CswNbtViewId ViewId )
        {
            Master.setViewId( ViewId );
            Master.GoMain();
        }

        protected void _QuickLaunch_OnSessionViewLinkClick( CswNbtSessionDataId SessionViewId )
        {
            Master.setSessionViewId( SessionViewId );
            Master.GoMain();
        }

        protected void _QuickLaunch_OnActionLinkClick( Int32 ActionId )
        {
            Master.setAction( ActionId );
            Master.Redirect( Master.CswNbtResources.Actions[ActionId].Url );
        }


        #endregion Events


        //private void _makeSearchForm()
        //{
        //    SearchTable = new CswAutoTable();
        //    SearchTable.ID = "SearchTable";
        //    SearchTable.CssClass = "SearchTable";
        //    SearchPlaceHolder.Controls.Add( SearchTable );

        //    // Default search for IMCS
        //    if( CswNbtResources.MetaData != null ) // login page
        //    {
        //        CswNbtMetaDataNodeType EquipmentNodeType = CswNbtResources.MetaData.getNodeType( "Equipment" );
        //        if( EquipmentNodeType != null && EquipmentNodeType.BarcodeProperty != null )
        //            _CswPropertyFilter = new CswPropertyFilter( Master.CswNbtResources, AjaxManager, EquipmentNodeType.NodeTypeId, EquipmentNodeType.BarcodeProperty.PropId, true, true, true );
        //    }
        //    else
        //    {
        //        _CswPropertyFilter = new CswPropertyFilter( Master.CswNbtResources, AjaxManager, null, true, true, true );
        //    }
        //    _CswPropertyFilter.OnError += new CswErrorHandler( HandleError );
        //    _CswPropertyFilter.ID = "SearchPropFilter";
        //    _CswPropertyFilter.UseCheckChanges = false;
        //    SearchTable.addControl( 0, 1, _CswPropertyFilter );

        //    TableCell ButtonCell = SearchTable.getCell( 0, 2 );
        //    ButtonCell.VerticalAlign = VerticalAlign.Top;

        //    SearchButton = new Button();
        //    SearchButton.Text = "Search";
        //    SearchButton.CssClass = "Button";
        //    SearchButton.Click += new EventHandler( SearchButton_Click );
        //    ButtonCell.Controls.Add( SearchButton );

        //    SearchLink.Visible = false;
        //    SearchLinkStatusHF.Value = "1";
        //}

        #region Master pass-thru

        public string LogoutPath { get { return Master.LogoutPath; } set { Master.LogoutPath = value; } }
        public object PreviousView1 { get { return Master.PreviousView1; } }
        public object PreviousView2 { get { return Master.PreviousView2; } }
        public object PreviousView3 { get { return Master.PreviousView3; } }
        public object PreviousView4 { get { return Master.PreviousView4; } }
        public object PreviousView5 { get { return Master.PreviousView5; } }
        //        public CswNbtSession CswSession { get { return Master.CswSession; } }
        public RadAjaxManager AjaxManager { get { return Master.AjaxManager; } }
        public CswNbtView CswNbtView { get { return Master.CswNbtView; } }
        public RadWindow DesignDeleteDialogWindow { get { return Master.DesignDeleteDialogWindow; } }
        public CswSessions SessionList { get { return ( Master.SessionList ); } }
        public void setAction( Int32 ActionId ) { Master.setAction( ActionId ); }
        public void setViewId( CswNbtViewId ViewId ) { Master.setViewId( ViewId ); }
        public void setViewId( CswNbtViewId ViewId, bool ForceReload ) { Master.setViewId( ViewId, ForceReload ); }
        public void setSessionViewId( CswNbtSessionDataId ViewId, bool ForceReload ) { Master.setSessionViewId( ViewId, ForceReload ); }
        public void setSessionViewId( CswNbtSessionDataId ViewId ) { Master.setSessionViewId( ViewId ); }
        public void setViewXml( string ViewXml ) { Master.setViewXml( ViewXml ); }
        public void setViewXml( string ViewXml, bool ForceReload ) { Master.setViewXml( ViewXml, ForceReload ); }
        public void clearView() { Master.clearView(); }
        public string AccessID { get { return Master.AccessId; } set { Master.AccessId = value; } }
        public void ReleaseAll() { Master.ReleaseAll(); }
        public void Redirect( string url ) { Master.Redirect( url ); }
        //public void GoHome() { Master.GoHome(); }
        public void GoHome() { Master.Redirect( "Main.html?clear=y" ); }
        public void GoMain() { Master.GoMain(); }
        public void LogMessage( string Message ) { Master.LogMessage( Message ); }
        public void LogTimerResult( string Message, string TimerResult ) { Master.LogTimerResult( Message, TimerResult ); }
        public CswNbtResources CswNbtResources { get { return Master.CswNbtResources; } }
        public AuthenticationStatus Authenticate( string username, string password )
        {
            return ( Master.Authenticate( username, password ) );
        }
        public void Logout() { Master.Logout(); }
        public void HandleAddNode( CswNbtNode Node ) { Master.HandleAddNode( Node ); }
        public void HandleCopyNode( CswNbtNode OldNode, CswNbtNode NewNode ) { Master.HandleCopyNode( OldNode, NewNode ); }
        public void HandleMultiModeEnabled( CswNbtView View ) { Master.HandleMultiModeEnabled( View ); }
        public void HandleViewEditorFinish( CswNbtView View ) { Master.HandleViewEditorFinish( View ); }
        public void EndSession( string SessionId ) { Master.EndSession( SessionId ); }
        public void HandleModifyViewFilters( CswNbtView OldView, CswNbtView NewView ) { Master.HandleModifyViewFilters( OldView, NewView ); }
        public void HandleSearch( CswNbtViewProperty ViewProperty ) { Master.HandleSearch( ViewProperty ); }

        #endregion Master pass-thru


    } // class MainLayout
} // namespace ChemSW.Nbt.WebPages

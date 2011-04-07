using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.Config;
using ChemSW.Security;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Session;
using Telerik.Web.UI;
using ChemSW.Nbt.Security;
using ChemSW.Config;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{

    public partial class Standard : System.Web.UI.MasterPage
    {
        //        public CswSessionResourcesNbt CswInitialization { get { return Master.CswInitialization; } }
        public CswNbtResources CswNbtResources { get { return Master.CswNbtResources; } }
        //        public CswNbtSession CswSession { get { return Master.CswSession; } }

        public string LogoutPath { get { return Master.LogoutPath; } set { Master.LogoutPath = value; } }
        public string AccessId { get { return Master.AccessId; } set { Master.AccessId = value; } }

        public bool IsAuthenticated() { return Master.IsAuthenticated(); }
        public AuthenticationStatus Authenticate( string username, string password )
        {
            return ( Master.Authenticate( username, password ) );
        }
        public CswSessions SessionList
        {
            get { return ( Master.SessionList ); }
        }

        #region Page Lifecycle

        public Standard()
        {
        }

        public RadAjaxManager AjaxManager
        {
            get { return StandardAjaxManager; }
        }

        public RadWindow DesignDeleteDialogWindow
        {
            get { return DesignDeleteDialog; }
        }

        #region Events

        public void HandleAddNode( CswNbtNode Node )
        {
            Master.HandleAddNode( Node );
        }
        public void HandleCopyNode( CswNbtNode OldNode, CswNbtNode NewNode )
        {
            Master.HandleCopyNode( OldNode, NewNode );
        }
        public void HandleMultiModeEnabled( CswNbtView View )
        {
            Master.HandleMultiModeEnabled( View );
        }
        public void HandleViewEditorFinish( CswNbtView View )
        {
            Master.HandleViewEditorFinish( View );
        }
        public void EndSession( string SessionId )
        {
            Master.EndSession( SessionId );
        }
        public void HandleModifyViewFilters( CswNbtView OldView, CswNbtView NewView )
        {
            Master.HandleModifyViewFilters( OldView, NewView );
        }
        public void HandleSearch( CswNbtViewProperty ViewProperty )
        {
            Master.HandleSearch( ViewProperty );
        }
        public void HandleLoadReport( CswPrimaryKey ReportId )
        {
            Master.HandleLoadReport( ReportId );
        }
        public void HandleAfterError( Exception ex )
        {
            Master.HandleAfterError( ex );
        }

        #endregion Events

        public HtmlGenericControl TimeReportDiv
        {
            get { return TimeReportOuter; }
        }


        private CswTimer Timer = new CswTimer();
        public CswImageButton TimeReportImageButton;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                LogTimerResult( "Standard.OnInit() started", Timer.ElapsedDurationInSecondsAsString );

                TimeReportImageButton = new CswImageButton( CswImageButton.ButtonType.ClockGrey );
                TimeReportImageButton.ID = "TimeReportImageButton";
                TimeReportImageButton.OnClientClick = "TimeReportImageButton_Click(); return false;";
                TimeReportImageButtonPH.Controls.Add( TimeReportImageButton );

                //Master.CswNbtResources.OnBeforeDeleteView += new CswNbtResources.BeforeDeleteViewEventHandler( OnBeforeDeleteView );
                Master.CswNbtResources.CswEventLinker.Subscribe( "StandardMasterInit_BeforeDeleteView", CswNbtView.BeforeDeleteViewEventName, (CswNbtView.BeforeDeleteViewEventHandler) OnBeforeDeleteView );
                //Master.CswNbtResources.CswEventLinker.Subscribe( "StandardMasterInit_AfterNewView", CswNbtView.AfterNewViewEventName, (CswNbtView.AfterNewViewEventHandler) OnAfterNewView );
                Master.CswNbtResources.CswEventLinker.Subscribe( "StandardMasterInit_AfterEditView", CswNbtView.AfterEditViewEventName, (CswNbtView.AfterEditViewEventHandler) OnAfterEditView );

                base.OnInit( e );

                LogTimerResult( "Standard.OnInit() finished", Timer.ElapsedDurationInSecondsAsString );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }//OnInit()

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                LogLifeCycle( "Standard.OnLoad()" );
                base.OnLoad( e );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                LogLifeCycle( "Standard.OnPreRender()" );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }

        protected override void Render( HtmlTextWriter writer )
        {
            try
            {
                LogLifeCycle( "Standard.Render()" );
                TimeReportLiteral.Text = CswNbtResources.Timer.ElapsedDurationInSecondsAsString + "&nbsp;server&nbsp;\n";
                base.Render( writer );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected override void OnUnload( EventArgs e )
        {
            try
            {
                base.OnUnload( e );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public void ReleaseAll()
        {
            Master.ReleaseAll();
        }

        public void Redirect( string url )
        {
            Master.Redirect( url );
        }
        public void GoHome() { Master.GoHome(); }
        public void GoMain() { Master.GoMain(); }

        #endregion Page Lifecycle






        #region Error Management

        public new CswErrorHandler OnError { get { return Master.OnError; } set { Master.OnError = value; } }
        public void HandleError( Exception ex )
        {
            Master.HandleError( ex );
        }


        #endregion Error Management

        #region Selected View/Search/Action Management

        private CswNbtView _CswNbtView;
        public CswNbtView CswNbtView
        {
            get
            {
                initSelectedView( false );
                return _CswNbtView;
            }
        }

        //private CswNbtSearch _CswNbtSearch;
        //public CswNbtSearch CswNbtSearch
        //{
        //    get
        //    {
        //        initSelectedView( false );
        //        return _CswNbtSearch;
        //    }
        //}

        private Int32 _ActionId = Int32.MinValue;
        public Int32 ActionId
        {
            get
            {
                initSelectedView( false );
                return _ActionId;
            }
        }

        //public void OnAfterNewView( CswNbtView NewView )
        //{
        //    CswViewListTree.ClearCache( Session );
        //}

        public void OnAfterEditView( CswNbtView NewView )
        {
            if( NewView.Visibility != NbtViewVisibility.Property )
                CswViewListTree.ClearCache( Session );
			Master.CswNbtResources.ViewCache.clearFromCache( NewView );
        }

        public void OnBeforeDeleteView( CswNbtView ViewToDelete )
        {
            if( ViewToDelete != null )
            {
                while( PreviousView1 is CswNbtView && (CswNbtView) PreviousView1 == ViewToDelete )
                {
                    PreviousView1 = PreviousView2;
                    PreviousView2 = PreviousView3;
                    PreviousView3 = PreviousView4;
                    PreviousView4 = PreviousView5;
                    PreviousView5 = null;
                }
                while( PreviousView2 is CswNbtView && (CswNbtView) PreviousView2 == ViewToDelete )
                {
                    PreviousView2 = PreviousView3;
                    PreviousView3 = PreviousView4;
                    PreviousView4 = PreviousView5;
                    PreviousView5 = null;
                }
                while( PreviousView3 is CswNbtView && (CswNbtView) PreviousView3 == ViewToDelete )
                {
                    PreviousView3 = PreviousView4;
                    PreviousView4 = PreviousView5;
                    PreviousView5 = null;
                }
                while( PreviousView4 is CswNbtView && (CswNbtView) PreviousView4 == ViewToDelete )
                {
                    PreviousView4 = PreviousView5;
                    PreviousView5 = null;
                }
                if( PreviousView5 is CswNbtView && (CswNbtView) PreviousView5 == ViewToDelete )
                {
                    PreviousView5 = null;
                }
            }
            CswViewListTree.ClearCache( Session );
        }

        //cf. bz # 10266 and 9932
        public object PreviousView1 { get { return Session["PreviousView1"]; } private set { Session["PreviousView1"] = value; } }
        public object PreviousView2 { get { return Session["PreviousView2"]; } private set { Session["PreviousView2"] = value; } }
        public object PreviousView3 { get { return Session["PreviousView3"]; } private set { Session["PreviousView3"] = value; } }
        public object PreviousView4 { get { return Session["PreviousView4"]; } private set { Session["PreviousView4"] = value; } }
        public object PreviousView5 { get { return Session["PreviousView5"]; } private set { Session["PreviousView5"] = value; } }

        private void initSelectedView( bool ForceReload )
        {
            //bz # 5150 init from query param
            string ViewIdFromQueryParam = string.Empty;
            bool ViewLoaded = false;
            if( _CswNbtView == null || ForceReload )
            {
                if( Request.QueryString["viewid"] != null && Request.QueryString["viewid"] != string.Empty )
                {
                    ViewIdFromQueryParam = CswTools.QueryStringParamToUrl( Request.QueryString["viewid"].ToString() );
                    Int32 TargetViewId = CswConvert.ToInt32( ViewIdFromQueryParam );
                    if( Session["ViewId"] != null && Session["ViewId"].ToString() != TargetViewId.ToString() )  // BZ 10125
                    {
                        ViewLoaded = ( null != ( _CswNbtView = (CswNbtView) CswNbtViewFactory.restoreView( CswNbtResources, TargetViewId ) ) );
                    }
                }

                if( !ViewLoaded )
                {
                    if( Session["ActionId"] != null )
                    {
                        // Action
                        _ActionId = CswConvert.ToInt32( Session["ActionId"].ToString() );
                        // BZ 9934 - No need for 'default view' anymore
                        //// this is for when we come back
                        //_CswNbtView = (CswNbtView) CswNbtViewFactory.restoreView( CswNbtResources, CswNbtResources.CurrentNbtUser.DefaultViewId );

                        if( ForceReload )
                            Master.HandleLoadAction( _ActionId );
                    }
                    else
                    {
                        // View
                        _CswNbtView = new CswNbtView( CswNbtResources );

                        if( Session["SessionViewId"] != null )
                            ViewLoaded = ( null != ( _CswNbtView = (CswNbtView) CswNbtViewFactory.restoreView( CswNbtResources, CswNbtResources.ViewCache.getView( CswConvert.ToInt32( Session["SessionViewId"] ) ).ToString() ) ) );

                        if( Session["ViewId"] != null )
                            ViewLoaded = ( null != ( _CswNbtView = (CswNbtView) CswNbtViewFactory.restoreView( CswNbtResources, CswConvert.ToInt32( Session["ViewId"] ) ) ) );

                        if( !ViewLoaded && Session["ViewXml"] != null )
                            ViewLoaded = ( null != ( _CswNbtView = (CswNbtView) CswNbtViewFactory.restoreView( CswNbtResources, Session["ViewXml"].ToString() ) ) );

                        // BZ 9934 - No need for 'default view' anymore
                        //if( !ViewLoaded && CswNbtResources.CurrentUser != null && CswNbtResources.CurrentNbtUser.DefaultViewId > 0 )
                        //    ViewLoaded = ( null != ( _CswNbtView = (CswNbtView) CswNbtViewFactory.restoreView( CswNbtResources, CswNbtResources.CurrentNbtUser.DefaultViewId ) ) );

                        if( ForceReload )
                            Master.HandleLoadView( _CswNbtView );
                    }
                }

                if( _CswNbtView == null )
                {
                    // BZ 8321 - If all else fails, use an empty view
                    _CswNbtView = new CswNbtView( CswNbtResources );
                }
            } // if( _CswNbtView == null || ForceReload )
        } // initSelectedView()

        public void setViewId( Int32 ViewId )
        {
            setViewId( ViewId, false );
        }
        public void setViewId( Int32 ViewId, bool ForceReload )
        {
            if( Session["ViewId"] == null || ViewId.ToString() != Session["ViewId"].ToString() || ForceReload )
            {
                clearView();
                Session.Add( "ViewId", ViewId.ToString() );
                initSelectedView( true );
            }
        }

        public void setSessionViewId( Int32 SessionViewId )
        {
            setSessionViewId( SessionViewId, false );
        }//setSessionViewId()

        public void setSessionViewId( Int32 SessionViewId, bool ForceReload )
        {
            if( Session["SessionViewId"] == null || SessionViewId.ToString() != Session["SessionViewId"].ToString() || ForceReload )
            {
                clearView();
                Session.Add( "SessionViewId", SessionViewId.ToString() );
                initSelectedView( true );
            }
        }//setSessionViewId()


        public void setViewXml( string ViewXml )
        {
            setViewXml( ViewXml, false );
        }
        public void setViewXml( string ViewXml, bool ForceReload )
        {
            if( Session["ViewXml"] == null || ViewXml != Session["ViewXml"].ToString() || ForceReload )
            {
                clearView();
                Session.Add( "ViewXml", ViewXml.ToString() );
                initSelectedView( true );
            }
        }
        public void setAction( Int32 ActionId )
        {
            clearView();
            Session.Add( "ActionId", ActionId );
            initSelectedView( true );
        }
        //public void setSearch( CswNbtSearch NbtSearch )
        //{
        //    clearView();
        //    Session.Add( "Search", NbtSearch.ToString() );
        //    initSelectedView( true );
        //}

        public void clearView()
        {
            // if the view isn't already on the list of recents, add it
            object ViewToKeep = null;

            //if ( _CswNbtSearch != null )
            //{
            //    if ( ( !( Session[ "PreviousView1" ] is CswNbtSearch ) || _CswNbtSearch != ( CswNbtSearch )Session[ "PreviousView1" ] ) &&
            //         ( !( Session[ "PreviousView2" ] is CswNbtSearch ) || _CswNbtSearch != ( CswNbtSearch )Session[ "PreviousView2" ] ) &&
            //         ( !( Session[ "PreviousView3" ] is CswNbtSearch ) || _CswNbtSearch != ( CswNbtSearch )Session[ "PreviousView3" ] ) &&
            //         ( !( Session[ "PreviousView4" ] is CswNbtSearch ) || _CswNbtSearch != ( CswNbtSearch )Session[ "PreviousView4" ] ) &&
            //         ( !( Session[ "PreviousView5" ] is CswNbtSearch ) || _CswNbtSearch != ( CswNbtSearch )Session[ "PreviousView5" ] ) )
            //    {
            //        ViewToKeep = _CswNbtSearch;
            //    }
            //}
            //else 
            if( ActionId > 0 )
            {
                if( ( !( PreviousView1 is Int32 ) || PreviousView1 == null || ActionId != CswConvert.ToInt32( PreviousView1.ToString() ) ) &&
                    ( !( PreviousView2 is Int32 ) || PreviousView2 == null || ActionId != CswConvert.ToInt32( PreviousView2.ToString() ) ) &&
                    ( !( PreviousView3 is Int32 ) || PreviousView3 == null || ActionId != CswConvert.ToInt32( PreviousView3.ToString() ) ) &&
                    ( !( PreviousView4 is Int32 ) || PreviousView4 == null || ActionId != CswConvert.ToInt32( PreviousView4.ToString() ) ) &&
                    ( !( PreviousView5 is Int32 ) || PreviousView5 == null || ActionId != CswConvert.ToInt32( PreviousView5.ToString() ) ) )
                {
                    ViewToKeep = ActionId;
                }
            }
            else if( CswNbtView != null )  // this needs to be last, because it's never null
            {
                if( CswNbtView.Visibility != NbtViewVisibility.Property )   // BZ 7934
                {
                    if( ( !( PreviousView1 is CswNbtView ) || CswNbtView != (CswNbtView) PreviousView1 ) &&
                        ( !( PreviousView2 is CswNbtView ) || CswNbtView != (CswNbtView) PreviousView2 ) &&
                        ( !( PreviousView3 is CswNbtView ) || CswNbtView != (CswNbtView) PreviousView3 ) &&
                        ( !( PreviousView4 is CswNbtView ) || CswNbtView != (CswNbtView) PreviousView4 ) &&
                        ( !( PreviousView5 is CswNbtView ) || CswNbtView != (CswNbtView) PreviousView5 ) )
                    {
                        ViewToKeep = CswNbtView;
                    }
                }
            }


            if( ViewToKeep != null )
            {
                PreviousView5 = PreviousView4;
                PreviousView4 = PreviousView3;
                PreviousView3 = PreviousView2;
                PreviousView2 = PreviousView1;
                PreviousView1 = ViewToKeep;
            }

            Session.Remove( "ViewId" );
            Session.Remove( "SessionViewId" );
            Session.Remove( "ViewXml" );
            Session.Remove( "Search" );
            Session.Remove( "ActionId" );
            //_CswNbtSearch = null;
            _ActionId = Int32.MinValue;
            _CswNbtView = new CswNbtView( CswNbtResources );
        }
        #endregion Selected View/Search/Action Management

        #region Logging

        private void LogLifeCycle( string EventName )
        {
            Master.LogLifeCycle( EventName );
        }

        public void LogMessage( string Message )
        {
            Master.LogMessage( Message );
        }

        public void LogTimerResult( string Message, string TimerResult )
        {
            Master.LogTimerResult( Message, TimerResult );
        }

        #endregion Logging


        public void Logout()
        {
            Master.Logout();
        }
    }
}

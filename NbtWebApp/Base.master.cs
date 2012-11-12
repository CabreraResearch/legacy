using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using ChemSW.Config;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Statistics;
using ChemSW.Security;
using ChemSW.Session;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.WebPages
{
    public partial class Base : System.Web.UI.MasterPage
    {
        public CswSessionResourcesNbt CswSessionResourcesNbt = null;
        public CswNbtResources CswNbtResources = null;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents = null;

        private CswSessionManager _CswSessionManager;
        public CswSessionManager CswSessionManager { get { return _CswSessionManager; } }

        #region Page Lifecycle

        public string LogoutPath
        {
            get
            {
                if( Session["logoutpath"] != null && Session["logoutpath"].ToString() != string.Empty )
                    return Session["logoutpath"].ToString();
                else
                    return "Main.html";
            }
            set
            {
                Session["logoutpath"] = value;
            }
        }
        public string AccessId
        {
            get
            {
                return ( CswSessionManager.AccessId );
            }
            set
            {
                CswSessionManager.setAccessId( value.ToLower() );
            }
        }
        public AuthenticationStatus Authenticate( string username, string password )
        {
            return ( CswSessionManager.beginSession( username, password, CswWebSvcCommonMethods.getIpAddress(), false ) );
        }//Authenticate()


        public bool IsAuthenticated()
        {
            bool ReturnVal = false;
            if( CswSessionManager.IsAuthenticated() )
            {
                ReturnVal = true;
                CswSessionManager.updateLastAccess( false );
            }

            return ( ReturnVal );

        }//IsAuthenticated()

        public CswSessions SessionList
        {
            get
            {
                return ( CswSessionManager.SessionsList );
            }
        }

        #region Events

        public void HandleMultiModeEnabled( CswNbtView View )
        {
            _CswNbtStatisticsEvents.OnMultiModeEnabled( View );
        }
        public void HandleLoadReport( CswPrimaryKey ReportId )
        {
            _CswNbtStatisticsEvents.OnLoadReport( ReportId );
        }

        public void HandleAddNode( CswNbtNode Node )
        {
            _CswNbtStatisticsEvents.OnAddNode( Node );
        }
        public void HandleCopyNode( CswNbtNode OldNode, CswNbtNode NewNode )
        {
            _CswNbtStatisticsEvents.OnCopyNode( OldNode, NewNode );
        }
        public void HandleViewEditorFinish( CswNbtView View )
        {
            _CswNbtStatisticsEvents.OnFinishEditingView( View );
        }
        public void HandleSearch( CswNbtViewProperty ViewProp )
        {
            _CswNbtStatisticsEvents.OnLoadSearch( ViewProp );
        }
        public void HandleModifyViewFilters( CswNbtView OldView, CswNbtView NewView )
        {
            _CswNbtStatisticsEvents.OnModifyViewFilters( OldView, NewView );
        }
        public void HandleLoadAction( Int32 ActionId )
        {
            _CswNbtStatisticsEvents.OnLoadAction( ActionId );
        }
        public void HandleLoadView( CswNbtView View )
        {
            _CswNbtStatisticsEvents.OnLoadView( View );
        }
        public void EndSession( string SessionId )
        {
            //Sergei: I suspect that EndSession() was never actually called, 
            //unless it is supposed to be called when a person actively logs out. 
            //If its just the end of the request cycle, this would have resulted in 
            //the user's session getting nuked. So I'm commenting it out now, for 
            //further investigation later. 
            //--Dimitri
            //            _CswSessionList.remove();
        }
        public void HandleAfterError( Exception ex )
        {
            _CswNbtStatisticsEvents.OnError( ex );
        }

        #endregion Events

        private CswTimer Timer = new CswTimer();

        protected override void OnInit( EventArgs e )
        {
            try
            {

                //bz # 9278


                CswSessionResourcesNbt = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, Context, string.Empty, SetupMode.NbtWeb );


                CswNbtResources = CswSessionResourcesNbt.CswNbtResources;


                _CswSessionManager = CswSessionResourcesNbt.CswSessionManager;
                _CswNbtStatisticsEvents = CswSessionResourcesNbt.CswNbtStatisticsEvents;
                CswNbtResources.beginTransaction();

                // Setup statistics events
                OnEndOfPageLifeCycle += new EndOfPageLifeCycleHandler( _CswNbtStatisticsEvents.OnEndOfPageLifeCycle );

                CswNbtResources.CswNbtNodeFactory.OnWriteNode += new CswNbtNode.OnRequestWriteNodeHandler( _CswNbtStatisticsEvents.OnWriteNode );
                CswNbtResources.CswNbtNodeFactory.OnDeleteNode += new CswNbtNode.OnRequestDeleteNodeHandler( _CswNbtStatisticsEvents.OnDeleteNode );





                LogTimerResult( "Base.OnInit() started", Timer.ElapsedDurationInSecondsAsString );

                //bz # 9278
                if( _CswSessionManager.IsAuthenticated() && _CswSessionManager.TimedOut )
                {
                    string LP = LogoutPath;
                    if( Page.Request.IsAuthenticated )
                    {
                        FormsAuthentication.SignOut();
                    }
                    //ReleaseAll();
                    //CswSessionResourcesNbt.DeAuthenticate(); //bz # 6163
                    Redirect( LP );
                }

                if( !CswSessionManager.IsAuthenticated() )
                {
                    string ScriptName = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];
                    if( ScriptName.Substring( ScriptName.LastIndexOf( '/' ) ) != "/Main.html" )
                    {
                        //ReleaseAll();
                        Redirect( LogoutPath );
                    }
                }

                base.OnInit( e );

                LogTimerResult( "Base.OnInit() finished", Timer.ElapsedDurationInSecondsAsString );
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
                LogLifeCycle( "Base.OnLoad()" );
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
                LogLifeCycle( "Base.OnPreRender()" );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }

        public delegate void EndOfPageLifeCycleHandler( CswTimer Timer );
        public event EndOfPageLifeCycleHandler OnEndOfPageLifeCycle = null;

        protected override void Render( HtmlTextWriter writer )
        {
            try
            {
                LogLifeCycle( "Base.Render()" );
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
                LogMessage( "END OF PAGE LIFECYCLE" );

                if( OnEndOfPageLifeCycle != null )
                    OnEndOfPageLifeCycle( CswNbtResources.Timer );

                ReleaseAll();
                //if( CswSessionResourcesNbt != null )
                //    CswSessionResourcesNbt.setCache();

                base.OnUnload( e );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public void ReleaseAll()
        {
            if( CswNbtResources != null )
            {
                CswNbtResources.finalize();
                //The implementation in the original nbt session class was commented out
                //_CswSessionManager.finalize();
                CswNbtResources.release();

                _CswSessionManager.release();
            }
        }

        public void Redirect( string url )
        {
            // Don't mess with this without reading BZ 6662 first
            LogMessage( "Redirecting To " + url );
            ReleaseAll();
            Response.Redirect( url );
        }

        public void GoHome()
        {
            Redirect( "Main.html" );
        }
        public void GoMain()
        {
            Redirect( "Main.html" );
        }

        #endregion Page Lifecycle






        #region Error Management

        public new CswErrorHandler OnError = null;

        public void HandleError( Exception ex )
        {
            // See BZ 6662
            if( !( ex is System.Threading.ThreadAbortException ) )
            {
                if( CswNbtResources != null )
                    CswNbtResources.AnErrorOccurred = true;

                if( OnError != null )
                    OnError( ex );
                else
                    throw ex;
            }
            else
                throw ex;
        }


        #endregion Error Management

        #region Logging

        public void LogLifeCycle( string EventName )
        {
            LogMessage( "Page Lifecycle (" + Page.AppRelativeVirtualPath + "): " + EventName + " called" );
        }

        public void LogMessage( string Message )
        {
            if( CswNbtResources != null )
                CswNbtResources.logMessage( Message );
        }

        public void LogTimerResult( string Message, string TimerResult )
        {
            if( CswNbtResources != null )
                CswNbtResources.logTimerResult( Message, TimerResult );
        }

        #endregion Logging


        public void Logout()
        {
            string CachedLogoutPath = LogoutPath;
            CswSessionManager.clearSession();
            Redirect( CachedLogoutPath );
        }
    }
}

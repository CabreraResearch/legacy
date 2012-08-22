using System;
using System.Web;
using System.Web.UI;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Log;
using ChemSW.Nbt.Config;

namespace ChemSW.HTTPModules
{
    public class TimingModule : IHttpModule
    {
        private CswTimer _ServerStartTimer;
        private CswTimer _RequestTimer;
        //private StreamWriter _LogFileStream;

        private ICswLogger _CswLogger = null;
        private ICswSetupVbls _CswSetupVbls = null;


        public TimingModule()
        {
            _ServerStartTimer = new CswTimer();
//            _LogFileStream = File.CreateText( @"D:/vault/dn/logs/httplog.txt" );

            _CswSetupVbls = new CswSetupVblsNbt( SetupMode.NbtWeb );
            _CswLogger = new CswAppStatusReporter( null, _CswSetupVbls, AppType.Nbt );

        }//ctor()

        public void _logEvent( HttpRequest Request, string EventName )
        {
            string Url = string.Empty;
            if( Request != null )
                Url = Request.Url.AbsoluteUri;

            //_LogFileStream.WriteLine();
            //_LogFileStream.Write( DateTime.Now.ToString() + "\t" +
            //                      _ServerStartTimer.ElapsedDurationInSecondsAsString + "\t" +
            //                      _RequestTimer.ElapsedDurationInSecondsAsString + "\t" +
            //                      Url + "\t" +
            //                      EventName );

            _CswLogger.reportHttpStat( _ServerStartTimer.ElapsedDurationInSecondsAsString, _RequestTimer.ElapsedDurationInSecondsAsString, Url, EventName );
        }

        public void _endCycle()
        {
            //_LogFileStream.WriteLine();
            //_LogFileStream.Flush();
            _CswLogger.release();
        }

        void IHttpModule.Dispose()
        {
            //_LogFileStream.Close();
        }

        void IHttpModule.Init( HttpApplication context )
        {
            context.BeginRequest += new EventHandler( context_BeginRequest );
            context.AuthenticateRequest += new EventHandler( context_AuthenticateRequest );
            context.PostAuthenticateRequest += new EventHandler( context_PostAuthenticateRequest );
            context.AuthorizeRequest += new EventHandler( context_AuthorizeRequest );
            context.PostAuthorizeRequest += new EventHandler( context_PostAuthorizeRequest );
            context.ResolveRequestCache += new EventHandler( context_ResolveRequestCache );
            context.PostResolveRequestCache += new EventHandler( context_PostResolveRequestCache );
            //"This operation requires IIS integrated pipeline mode."
            //context.MapRequestHandler += new EventHandler( context_MapRequestHandler );
            //context.PostMapRequestHandler += new EventHandler( context_PostMapRequestHandler );
            context.AcquireRequestState += new EventHandler( context_AcquireRequestState );
            context.PostAcquireRequestState += new EventHandler( context_PostAcquireRequestState );
            context.PreRequestHandlerExecute += new EventHandler( context_PreRequestHandlerExecute );
            context.PostRequestHandlerExecute += new EventHandler( context_PostRequestHandlerExecute );
            context.ReleaseRequestState += new EventHandler( context_ReleaseRequestState );
            context.PostReleaseRequestState += new EventHandler( context_PostReleaseRequestState );
            context.UpdateRequestCache += new EventHandler( context_UpdateRequestCache );
            context.PostUpdateRequestCache += new EventHandler( context_PostUpdateRequestCache );
            context.EndRequest += new EventHandler( context_EndRequest );
            context.PreSendRequestHeaders += new EventHandler( context_PreSendRequestHeaders );
            context.PreSendRequestContent += new EventHandler( context_PreSendRequestContent );

            //"This operation requires IIS integrated pipeline mode."
            //context.LogRequest += new EventHandler( context_LogRequest );
            //context.PostLogRequest += new EventHandler( context_PostLogRequest );
            context.Error += new EventHandler( context_Error );
            context.Disposed += new EventHandler( context_Disposed );
        }

        void context_BeginRequest( object sender, EventArgs e )
        {
            _RequestTimer = new CswTimer();
            _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_BeginRequest" );
        }
        void context_AuthenticateRequest( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_AuthenticateRequest" ); }
        void context_PostAuthenticateRequest( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostAuthenticateRequest" ); }
        void context_AuthorizeRequest( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_AuthorizeRequest" ); }
        void context_PostAuthorizeRequest( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostAuthorizeRequest" ); }
        void context_ResolveRequestCache( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_ResolveRequestCache" ); }
        void context_PostResolveRequestCache( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostResolveRequestCache" ); }
        void context_MapRequestHandler( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_MapRequestHandler" ); }
        void context_PostMapRequestHandler( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostMapRequestHandler" ); }
        void context_AcquireRequestState( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_AcquireRequestState" ); }
        void context_PostAcquireRequestState( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostAcquireRequestState" ); }
        void context_PreRequestHandlerExecute( object sender, EventArgs e )
        {
            _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PreRequestHandlerExecute" );
            IHttpHandler handler = ( (System.Web.HttpApplication) ( sender ) ).Context.CurrentHandler as IHttpHandler;
            if( handler != null && handler is Page )
            {
                Page page = handler as Page;
                page.DataBinding += new EventHandler( page_DataBinding );
                page.Disposed += new EventHandler( page_Disposed );
                page.Error += new EventHandler( page_Error );
                page.Init += new EventHandler( page_Init );
                page.InitComplete += new EventHandler( page_InitComplete );
                page.Load += new EventHandler( page_Load );
                page.LoadComplete += new EventHandler( page_LoadComplete );
                page.PreInit += new EventHandler( page_PreInit );
                page.PreLoad += new EventHandler( page_PreLoad );
                page.PreRender += new EventHandler( page_PreRender );
                page.PreRenderComplete += new EventHandler( page_PreRenderComplete );
                page.SaveStateComplete += new EventHandler( page_SaveStateComplete );
                page.Unload += new EventHandler( page_Unload );
            }
        }

        void page_PreInit( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_PreInit" ); }
        void page_Init( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_Init" ); }
        void page_InitComplete( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_InitComplete" ); }
        void page_PreLoad( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_PreLoad" ); }
        void page_Load( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_Load" ); }
        void page_LoadComplete( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_LoadComplete" ); }
        void page_DataBinding( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_DataBinding" ); }
        void page_PreRender( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_PreRender" ); }
        void page_PreRenderComplete( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_PreRenderComplete" ); }
        void page_SaveStateComplete( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_SaveStateComplete" ); }
        //void page_Unload( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_Unload" ); }
        void page_Unload( object sender, EventArgs e ) { _logEvent( null, "Page_Unload" ); }
        void page_Error( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_Error" ); }
        void page_Disposed( object sender, EventArgs e ) { _logEvent( ( (Page) sender ).Request, "Page_Disposed" ); }

        void context_PostRequestHandlerExecute( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostRequestHandlerExecute" ); }
        void context_ReleaseRequestState( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_ReleaseRequestState" ); }
        void context_PostReleaseRequestState( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostReleaseRequestState" ); }
        void context_UpdateRequestCache( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_UpdateRequestCache" ); }
        void context_PostUpdateRequestCache( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostUpdateRequestCache" ); }
        void context_EndRequest( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_EndRequest" ); }
        void context_PreSendRequestHeaders( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PreSendRequestHeaders" ); }
        void context_PreSendRequestContent( object sender, EventArgs e )
        {
            _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PreSendRequestContent" );
            _endCycle();
        }

        void context_LogRequest( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_LogRequest" ); }
        void context_PostLogRequest( object sender, EventArgs e ) { _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_PostLogRequest" ); }
        void context_Error( object sender, EventArgs e )
        {
            _logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_Error" );
            _endCycle();
        }

        void context_Disposed( object sender, EventArgs e )
        {
            //_logEvent( ( (System.Web.HttpApplication) ( sender ) ).Request, "HTTP_Disposed" );
            _logEvent( null, "HTTP_Disposed" );
            _endCycle();
        }

    }
}
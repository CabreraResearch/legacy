using System;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Security;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public class CswNbtWebServiceResponse : ICswNbtWebServiceResponse
    {
        private CswTimer _Timer;
        private double _ServerInitTime;
        private HttpContext _Context;

        public CswNbtWebServiceResponse( HttpContext Context )
        {
            _ServerInitTime = 0;
            _Timer = new CswTimer();
            _Context = Context;
            SessionAuthenticationStatus = new CswNbtSessionAuthenticationStatus();
            SessionAuthenticationStatus.AuthenticationStatus = AuthenticationStatus.Unknown.ToString();
            try
            {
                CswNbtSessionResources = CswNbtSessionResources.initResources( _Context );
                AuthenticationStatus Auth = CswNbtSessionResources.attemptRefresh( true );
                SessionAuthenticationStatus.AuthenticationStatus = Auth.ToString();
            }
            catch( Exception ex )
            {
                addError( ex );
            }
        }

        public CswNbtSessionAuthenticationStatus SessionAuthenticationStatus { get; set; }

        public CswNbtWebServiceStatus Status { get; set; }

        public CswNbtWebServicePerformance Performance { get; set; }

        public Object Data { get; set; }

        public CswNbtSessionResources CswNbtSessionResources { get; set; }

        public void finalizeResponse( object DataObj, CswNbtResources OtherResources = null )
        {
            try
            {
                if( null != CswNbtSessionResources &&
                     null != CswNbtSessionResources.CswSessionManager )
                {
                    CswDateTime CswTimeout = new CswDateTime( CswNbtSessionResources.CswNbtResources, CswNbtSessionResources.CswSessionManager.TimeoutDate );
                    SessionAuthenticationStatus.TimeOut = CswTimeout.ToClientAsJavascriptString();
                }
                Performance = new CswNbtWebServicePerformance
                {
                    ServerInit = _ServerInitTime
                };
                if( null != CswNbtSessionResources &&
                     null != CswNbtSessionResources.CswNbtResources )
                {
                    Performance.DbInit = CswNbtSessionResources.CswNbtResources.CswLogger.DbInitTime;
                    Performance.DbQuery = CswNbtSessionResources.CswNbtResources.CswLogger.DbQueryTime;
                    Performance.DbCommit = CswNbtSessionResources.CswNbtResources.CswLogger.DbCommitTime;
                    Performance.DbDeinit = CswNbtSessionResources.CswNbtResources.CswLogger.DbDeInitTime;
                    Performance.TreeLoaderSql = CswNbtSessionResources.CswNbtResources.CswLogger.TreeLoaderSQLTime;
                }
                Performance.ServerTotal = _Timer.ElapsedDurationInMilliseconds;
                Data = "Response Complete";
                if( null != CswNbtSessionResources )
                {
                    CswNbtSessionResources.deInitResources( OtherResources );
                }
            }
            catch( Exception Ex )
            {
                addError( Ex );
            }
        }

        public void addError( Exception Exception )
        {
            CswNbtWebServiceError Error = new CswNbtWebServiceError( CswNbtSessionResources );
            Status = Error.getErrorStatus( Exception );
            Status.Success = false;
        }
    }
}
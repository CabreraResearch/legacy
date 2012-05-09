using System;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Security;
using NbtWebAppServices.Core;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWebServiceResponseBase : ICswNbtWebServiceResponse
    {
        private CswTimer _Timer;
        private double _ServerInitTime;
        private HttpContext _Context;

        public CswNbtWebServiceResponseBase( HttpContext Context )
        {
            _ServerInitTime = 0;
            _Timer = new CswTimer();
            _Context = Context;
            SessionAuthenticationStatus = new CswNbtSessionAuthenticationStatus();
            SessionAuthenticationStatus.AuthenticationStatus = "Unknown";
            Status = new CswNbtWebServiceStatus();

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

        [DataMember]
        public CswNbtSessionAuthenticationStatus SessionAuthenticationStatus { get; set; }

        [DataMember]
        public CswNbtWebServiceStatus Status { get; set; }

        [DataMember]
        public CswNbtWebServicePerformance Performance { get; set; }

        public CswNbtSessionResources CswNbtSessionResources { get; set; }

        public void finalizeResponse( CswNbtResources OtherResources = null )
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
            Status.Errors.Add( Error.getErrorStatus( Exception ) );
            Status.Success = false;
        }
    }
}
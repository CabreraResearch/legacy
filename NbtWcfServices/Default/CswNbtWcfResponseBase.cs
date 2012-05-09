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
    public class CswNbtWcfResponseBase : ICswNbtWcfResponse
    {
        private CswTimer _Timer;
        private double _ServerInitTime;
        private HttpContext _Context;

        public CswNbtWcfResponseBase( HttpContext Context, bool AttemptRefresh = true )
        {
            _ServerInitTime = 0;
            _Timer = new CswTimer();
            _Context = Context;
            SessionAuthenticationStatus = new CswNbtSessionAuthenticationStatus();
            SessionAuthenticationStatus.AuthenticationStatus = "Unknown";
            Status = new CswNbtWebServiceStatus();

            try
            {
                CswNbtWcfSessionResources = CswNbtWcfSessionResources.initResources( _Context );
                if( AttemptRefresh )
                {
                    AuthenticationStatus Auth = CswNbtWcfSessionResources.attemptRefresh( true );
                    SessionAuthenticationStatus.AuthenticationStatus = Auth.ToString();
                }
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

        public CswNbtWcfSessionResources CswNbtWcfSessionResources { get; set; }

        public void finalizeResponse( CswNbtResources OtherResources = null )
        {
            try
            {
                if( null != CswNbtWcfSessionResources &&
                     null != CswNbtWcfSessionResources.CswSessionManager )
                {
                    CswDateTime CswTimeout = new CswDateTime( CswNbtWcfSessionResources.CswNbtResources, CswNbtWcfSessionResources.CswSessionManager.TimeoutDate );
                    SessionAuthenticationStatus.TimeOut = CswTimeout.ToClientAsJavascriptString();
                }
                Performance = new CswNbtWebServicePerformance
                                  {
                                      ServerInit = _ServerInitTime
                                  };
                if( null != CswNbtWcfSessionResources &&
                     null != CswNbtWcfSessionResources.CswNbtResources )
                {
                    Performance.DbInit = CswNbtWcfSessionResources.CswNbtResources.CswLogger.DbInitTime;
                    Performance.DbQuery = CswNbtWcfSessionResources.CswNbtResources.CswLogger.DbQueryTime;
                    Performance.DbCommit = CswNbtWcfSessionResources.CswNbtResources.CswLogger.DbCommitTime;
                    Performance.DbDeinit = CswNbtWcfSessionResources.CswNbtResources.CswLogger.DbDeInitTime;
                    Performance.TreeLoaderSql = CswNbtWcfSessionResources.CswNbtResources.CswLogger.TreeLoaderSQLTime;
                }
                Performance.ServerTotal = _Timer.ElapsedDurationInMilliseconds;
                if( null != CswNbtWcfSessionResources )
                {
                    CswNbtWcfSessionResources.deInitResources( OtherResources );
                }
            }
            catch( Exception Ex )
            {
                addError( Ex );
            }
        }

        public void addError( Exception Exception )
        {
            CswNbtWcfError Error = new CswNbtWcfError( CswNbtWcfSessionResources );
            Status.Errors.Add( Error.getErrorStatus( Exception ) );
            Status.Success = false;
        }
    }
}
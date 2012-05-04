using System;
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
        public CswNbtWebServiceResponse()
        {
            _ServerInitTime = 0;
            _Timer = new CswTimer();
        }

        public CswNbtSessionAuthenticationStatus AuthenticationStatus { get; set; }

        public CswNbtWebServiceStatus Status { get; set; }

        public CswNbtWebServicePerformance Performance { get; set; }

        public Object Data { get; set; }

        public void finalizeResponse( AuthenticationStatus AuthenticationStatusIn, CswNbtSessionResources CswNbtSessionResources, object Data )
        {

            CswNbtWebServiceResponse Response = new CswNbtWebServiceResponse();
            Response.AuthenticationStatus = new CswNbtSessionAuthenticationStatus
                                                              {
                                                                  AuthenticationStatus = AuthenticationStatusIn.ToString()
                                                              };

            if( null != CswNbtSessionResources &&
                null != CswNbtSessionResources.CswSessionManager )
            {
                CswDateTime CswTimeout = new CswDateTime( CswNbtSessionResources.CswNbtResources, CswNbtSessionResources.CswSessionManager.TimeoutDate );
                Response.AuthenticationStatus.TimeOut = CswTimeout.ToClientAsJavascriptString();
            }
            Response.Performance = new CswNbtWebServicePerformance
                                              {
                                                  ServerInit = _ServerInitTime
                                              };
            if( null != CswNbtSessionResources &&
                null != CswNbtSessionResources.CswNbtResources )
            {
                Response.Performance.DbInit = CswNbtSessionResources.CswNbtResources.CswLogger.DbInitTime;
                Response.Performance.DbQuery = CswNbtSessionResources.CswNbtResources.CswLogger.DbQueryTime;
                Response.Performance.DbCommit = CswNbtSessionResources.CswNbtResources.CswLogger.DbCommitTime;
                Response.Performance.DbDeinit = CswNbtSessionResources.CswNbtResources.CswLogger.DbDeInitTime;
                Response.Performance.TreeLoaderSql = CswNbtSessionResources.CswNbtResources.CswLogger.TreeLoaderSQLTime;
            }
            Response.Performance.ServerTotal = _Timer.ElapsedDurationInMilliseconds;
            Response.Data = Data;
        }

        public void addError( Exception Exception, CswNbtSessionResources CswNbtSessionResources )
        {
            CswNbtWebServiceError Error = new CswNbtWebServiceError( CswNbtSessionResources );
            Status = Error.getErrorStatus( Exception );
            Status.Success = false;
        }
    }
}
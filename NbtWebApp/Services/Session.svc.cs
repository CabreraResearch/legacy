using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Session operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Session
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// Initiate a new session
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Initiate a new session" )]
        public CswNbtWebServiceSession.CswNbtAuthReturn Init( CswWebSvcSessionAuthenticateData.Authentication.Request Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceSession.CswNbtAuthReturn Ret = new CswNbtWebServiceSession.CswNbtAuthReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtWebServiceSession.CswNbtAuthReturn, CswWebSvcSessionAuthenticateData.Authentication.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, Request ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSession.getDefaults,
                ParamObj: Request
                );

            InitDriverType.run();
            return ( Ret );

        }

        /// <summary>
        /// Terminate the current session
        /// </summary>
        [OperationContract]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Terminate the current session" )]
        public bool End()
        {
            CswWebSvcResourceInitializerNbt Resource = new CswWebSvcResourceInitializerNbt( _Context, null );
            Resource.initResources();
            Resource.deauthenticate();
            Resource.deInitResources();
            return true;
        }

        /// <summary>
        /// Reset a user's password
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Reset a user's password" )]
        public CswNbtWebServiceSession.CswNbtSessionReturn ResetPassword( CswWebSvcSessionAuthenticateData.Authentication.Response.Expired Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceSession.CswNbtSessionReturn Ret = new CswNbtWebServiceSession.CswNbtSessionReturn();
            CswWebSvcResourceInitializerNbt ResourceInitializerNbt = new CswWebSvcResourceInitializerNbt( _Context, null );
            var InitDriverType = new CswWebSvcDriver<CswNbtWebServiceSession.CswNbtSessionReturn, CswWebSvcSessionAuthenticateData.Authentication.Response.Expired>(
                CswWebSvcResourceInitializer: ResourceInitializerNbt,
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSession.resetPassword,
                ParamObj: Request
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all login data for the current accessid in the given timeframe" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceSession.LoginDataReturn getLoginData( LoginData.LoginDataRequest Request )
        {
            CswNbtWebServiceSession.LoginDataReturn Ret = new CswNbtWebServiceSession.LoginDataReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceSession.LoginDataReturn, LoginData.LoginDataRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSession.getLoginData,
                ParamObj: Request
                );
            SvcDriver.run();
            return ( Ret );
        }
    }
}

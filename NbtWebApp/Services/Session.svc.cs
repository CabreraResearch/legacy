﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.WebServices;
using ChemSW.Security;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

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
        /// Initiate a new session
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Gets the current user's default settings" )]
        public CswNbtWebServiceSession.CswNbtAuthReturn GetUserDefaults()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceSession.CswNbtAuthReturn Ret = new CswNbtWebServiceSession.CswNbtAuthReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtWebServiceSession.CswNbtAuthReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSession.getUserDefaults,
                ParamObj: null
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
        /// Terminate the current session, version 2.0: Include AuthenticationStatus
        /// </summary>
        [OperationContract( Name = "EndWithAuth" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Terminate the current session" )]
        public CswNbtWebServiceSession.CswNbtAuthReturn End( bool Deauthenticate )
        {
            CswWebSvcResourceInitializerNbt Resource = new CswWebSvcResourceInitializerNbt( _Context, null );
            Resource.initResources();
            Resource.deauthenticate();
            Resource.deInitResources();

            CswNbtWebServiceSession.CswNbtAuthReturn Ret = new CswNbtWebServiceSession.CswNbtAuthReturn();
            Ret.Authentication.AuthenticationStatus = CswEnumAuthenticationStatus.Deauthenticated;
            ( (ICswWebSvcRetObj) Ret ).finalize( Resource.CswNbtResources, _Context, Ret.Authentication.AuthenticationStatus );
            return Ret;
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

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "End all active sessions for the current user" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn endCurrentUserSessions()
        {
            CswWebSvcReturn Ret = new CswWebSvcReturn();

            var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSession.endCurrentUserSessions,
                ParamObj: null
            );
            SvcDriver.run();
            return Ret;

        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceHeader.HeaderUsername getHeaderUsername()
        {
            CswNbtWebServiceHeader.HeaderUsername Ret = new CswNbtWebServiceHeader.HeaderUsername();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceHeader.HeaderUsername, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceHeader.getHeaderUsername,
                ParamObj: null
            );
            SvcDriver.run();
            return Ret;
        }
    }
}

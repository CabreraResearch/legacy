using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Session;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Session
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtSessionUriMethods
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Initiate a new session" )]
        public CswWebSvcReturn Init( CswNbtSessionAuthenticateData.Authentication.Request Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswWebSvcReturn Ret = new CswWebSvcReturn();
            var InitDriverType = new CswWebSvcDriver<CswWebSvcReturn, CswNbtSessionAuthenticateData.Authentication.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, Request ),
                ReturnObj: Ret,
                WebSvcMethodPtr: null,
                ParamObj: null
                );

            InitDriverType.run();
            return ( Ret );

        }
    }
}
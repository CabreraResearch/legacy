using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using ChemSW;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Services
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Testing
    {
        private HttpContext _Context = HttpContext.Current;

        private void throwException( ICswResources Resources, CswWebSvcReturn Output, object Input )
        {
            throw new CswDniException("Generated an exception for web service endpoint Testing.");
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Generate an Exception (with Authenticated Session)" )]
        public CswWebSvcReturn exception()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswWebSvcReturn Ret = new CswWebSvcReturn();
            var GetViewDriverType = new CswWebSvcDriver<CswWebSvcReturn, object>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : throwException,
                ParamObj : null
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Verify the web server is alive (no authentication)" )]
        public bool ping()
        {
            return true;
        }

    }
}

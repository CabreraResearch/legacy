using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Request operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Requests
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Get " )]
        public CswNbtRequestReturn findMaterialCreate()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestReturn Ret = new CswNbtRequestReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.getRequestMaterialCreate,
                ParamObj: null
                );

            InitDriverType.run();
            return ( Ret );
        }

        // Add more operations here and mark them with [OperationContract]
    }
}

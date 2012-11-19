using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.CISPro;

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
        [WebInvoke( Method = "GET" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Get the appropriate Request type for Material Creation" )]
        public CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn findMaterialCreate()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn Ret = new CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.getRequestMaterialCreate,
                ParamObj: null
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Fulfill a Request Item" )]
        public CswNbtRequestDataModel.CswNbtRequestMaterialDispenseReturn fulfill( CswNbtRequestDataModel.RequestFulfill Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswNbtRequestMaterialDispenseReturn Ret = new CswNbtRequestDataModel.CswNbtRequestMaterialDispenseReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswNbtRequestMaterialDispenseReturn, CswNbtRequestDataModel.RequestFulfill>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.fulfillRequest,
                ParamObj: Request
                );

            InitDriverType.run();
            return ( Ret );
        }

        // Add more operations here and mark them with [OperationContract]
    }
}

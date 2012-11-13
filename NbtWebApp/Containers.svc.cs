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
    /// WCF Web Methods for Landing Page operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Containers
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all Container barcodes and their most recent ContainerLocation Status for the given Location and timeframe" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceContainer.ContainerDataReturn getContainerData( ContainerData.ReconciliationRequest Request )
        {
            CswNbtWebServiceContainer.ContainerDataReturn Ret = new CswNbtWebServiceContainer.ContainerDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceContainer.ContainerDataReturn, ContainerData.ReconciliationRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceContainer.getContainerData,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all of the ContainerLocation Statuses along with their Container count and scan percentage for the given Location and timeframe" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceContainer.ContainerDataReturn getContainerStatistics( ContainerData.ReconciliationRequest Request )
        {
            CswNbtWebServiceContainer.ContainerDataReturn Ret = new CswNbtWebServiceContainer.ContainerDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceContainer.ContainerDataReturn, ContainerData.ReconciliationRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceContainer.getContainerStatistics,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all Container barcodes and their most recent ContainerLocation Status for the given Location and timeframe" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceContainer.ContainerDataReturn getContainerStatuses( ContainerData.ReconciliationRequest Request )
        {
            CswNbtWebServiceContainer.ContainerDataReturn Ret = new CswNbtWebServiceContainer.ContainerDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceContainer.ContainerDataReturn, ContainerData.ReconciliationRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceContainer.getContainerStatuses,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}

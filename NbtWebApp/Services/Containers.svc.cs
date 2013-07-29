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
    /// WCF Web Methods for Container operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Containers
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all Container Reconciliation data (including both Statistics and Statuses)" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceContainer.ContainerDataReturn getReconciliationData( ContainerData.ReconciliationRequest Request )
        {
            CswNbtWebServiceContainer.ContainerDataReturn Ret = new CswNbtWebServiceContainer.ContainerDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceContainer.ContainerDataReturn, ContainerData.ReconciliationRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceContainer.getReconciliationData,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get the number of ContainerLocation records whose Action has been set, but not applied, for the given Location" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceContainer.ContainerDataReturn getOutstandingActionsCount( ContainerData.ReconciliationRequest Request )
        {
            CswNbtWebServiceContainer.ContainerDataReturn Ret = new CswNbtWebServiceContainer.ContainerDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceContainer.ContainerDataReturn, ContainerData.ReconciliationRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceContainer.getOutstandingActionsCount,
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

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Save all actions that have changed on selected Containers' most recent ContainerLocation" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceContainer.ContainerDataReturn saveContainerActions( ContainerData.ReconciliationRequest Request )
        {
            CswNbtWebServiceContainer.ContainerDataReturn Ret = new CswNbtWebServiceContainer.ContainerDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceContainer.ContainerDataReturn, ContainerData.ReconciliationRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceContainer.saveContainerActions,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        #region Receive Material Wizard

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "updateExpirationDate" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Updates the default Expiration Date on containers to receive based on Receipt Lot's Manufactured Date" )]
        public CswNbtWebServiceContainer.ReceivingDataReturn updateExpirationDate( ContainerData.ReceiptLotRequest Request )
        {
            CswNbtWebServiceContainer.ReceivingDataReturn Ret = new CswNbtWebServiceContainer.ReceivingDataReturn();
            var GetViewDriverType = new CswWebSvcDriver<CswNbtWebServiceContainer.ReceivingDataReturn, ContainerData.ReceiptLotRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceContainer.updateExpirationDate,
                ParamObj: Request
                );
            GetViewDriverType.run();
            return ( Ret );
        }

        #endregion Receive Material Wizard
    }
}

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
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class DemoData
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary> 
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "getDemoDataGrid" )]
        [Description( "Get all demo data as a grid" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtDemoDataReturn getDemoDataGrid()
        {
            CswNbtDemoDataReturn Ret = new CswNbtDemoDataReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtDemoDataReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtDemoDataManager.getDemoDataGrid,
                ParamObj: null
                );

            SvcDriver.run();

            return ( Ret );

        }//getScheduledRulesGrid

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "getDemoDataNodesAsGrid" )]
        [Description( "Get specific demo data nodes as a grid" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtDemoDataReturn getDemoDataNodesAsGrid(CswNbtDemoDataRequests.CswDemoNodesGridRequest Request )
        {
            CswNbtDemoDataReturn Ret = new CswNbtDemoDataReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtDemoDataReturn, CswNbtDemoDataRequests.CswDemoNodesGridRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtDemoDataManager.getDemoDataNodesAsGrid,
                ParamObj: Request
                );

            SvcDriver.run();

            return ( Ret );

        }//getScheduledRulesGrid


        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "updateDemoData" )]
        [Description( "Update demo data" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtDemoDataReturn updateDemoData( CswNbtDemoDataRequests.CswUpdateDemoNodesRequest Request )
        {
            CswNbtDemoDataReturn Ret = new CswNbtDemoDataReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtDemoDataReturn, CswNbtDemoDataRequests.CswUpdateDemoNodesRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtDemoDataManager.updateDemoData,
                ParamObj: Request
                );

            SvcDriver.run();

            return ( Ret );

        }//updateDemoData


    }//DemoData 
}

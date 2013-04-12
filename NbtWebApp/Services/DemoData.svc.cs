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
        [Description( "Get all scheduled rules as a Grid" )]
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

    }//DemoData 
}

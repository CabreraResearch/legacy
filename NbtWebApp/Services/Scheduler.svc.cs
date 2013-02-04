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
    public class Scheduler
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary> 
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "get" )]
        [Description( "Get all scheduled rules as a Grid" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtScheduledRulesReturn getScheduledRulesGrid( string AccessId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtScheduledRulesReturn Ret = new CswNbtScheduledRulesReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtScheduledRulesReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNbtManager.getScheduledRulesGrid,
                ParamObj: AccessId
                );

            SvcDriver.run();

            return ( Ret );

        }//getScheduledRulesGrid

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "save" )]
        [Description( "Save changes to scheduled rules" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtScheduledRulesReturn.Ret updateAllScheduledRules( CswNbtScheduledRulesReturn.Ret Request  )
        {
            //delegate has to be static because you can't creat e an instance yet: you don't have resources until the delegate is actually called
            CswNbtScheduledRulesReturn.Ret Ret = new CswNbtScheduledRulesReturn.Ret();
            var SvcDriver = new CswWebSvcDriver<CswNbtScheduledRulesReturn.Ret, CswNbtScheduledRulesReturn.Ret>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNbtManager.updateAllScheduledRules,
                ParamObj: Request
                );

            SvcDriver.run();

            return ( Ret );

        }//updateAllScheduledRules

    }//Scheduler 
}

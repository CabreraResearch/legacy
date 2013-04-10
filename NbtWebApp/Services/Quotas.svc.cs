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
    /// WCF Web Methods for Quota operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Quotas
    {
        private HttpContext _Context = HttpContext.Current;
        
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Check the Quota for this NodeType" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceQuotas.CswNbtQuotaResponse check( CswNbtWebServiceQuotas.QuotaRequest QuotaReq )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceQuotas.CswNbtQuotaResponse Ret = new CswNbtWebServiceQuotas.CswNbtQuotaResponse();
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceQuotas.CswNbtQuotaResponse, CswNbtWebServiceQuotas.QuotaRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceQuotas.getQuota,
                ParamObj: QuotaReq
                );

            SvcDriver.run();
            return ( Ret );
        }
    }//
}

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Reports;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Mail Report operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Reports
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebGet( ResponseFormat = WebMessageFormat.Json )]
        [Description( "Generate subscription information for the current user" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceMailReports.MailReportSubscriptionsReturn getSubscriptions()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceMailReports.MailReportSubscriptionsReturn Ret = new CswNbtWebServiceMailReports.MailReportSubscriptionsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMailReports.MailReportSubscriptionsReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMailReports.getSubscriptions,
                ParamObj: null
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( ResponseFormat = WebMessageFormat.Json )]
        [Description( "Save subscription information for the current user" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn saveSubscriptions( CswNbtWebServiceMailReports.MailReportSubscriptions Subscriptions )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswWebSvcReturn Ret = new CswWebSvcReturn();

            var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, CswNbtWebServiceMailReports.MailReportSubscriptions>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMailReports.saveSubscriptions,
                ParamObj: Subscriptions
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}

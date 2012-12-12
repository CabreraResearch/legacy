using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Reports;
using NbtWebApp.WebSvc.Returns;
using ChemSW.Nbt.WebServices;
using System.IO;
using System.Data;
using System.Text;
using ChemSW;
using System.ServiceModel.Channels;

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
        [WebGet()]
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
        [WebInvoke()]
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

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Generate a report" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceReport.ReportReturn report( CswNbtWebServiceReport.ReportData Request )
        {
            CswNbtWebServiceReport.ReportReturn Ret = new CswNbtWebServiceReport.ReportReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceReport.ReportReturn, CswNbtWebServiceReport.ReportData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceReport.runReport,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Generate a csv report file" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream reportCSV( CswNbtWebServiceReport.ReportData Request )
        {
            CswNbtWebServiceReport.ReportReturn Ret = new CswNbtWebServiceReport.ReportReturn();
            //CswNbtWebServiceReport.ReportData Request = new CswNbtWebServiceReport.ReportData();
            //Request.reportFormat = reportFormat;
            //Request.nodeId = nodeId;
            ////Request.reportParams = reportParams;
            //Request.gridJSON = gridJSON;

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceReport.ReportReturn, CswNbtWebServiceReport.ReportData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceReport.runReportCSV,
                ParamObj: Request
                );
            SvcDriver.run();

            WebOperationContext.Current.OutgoingResponse.Headers.Set( "Content-Disposition", "attachment; filename=export.csv;" );
            //WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";

            return Request.stream;
        }

        [OperationContract, WebGet]
        public Stream GetValue()
        {
            string result = "Hello world";
            byte[] resultBytes = Encoding.UTF8.GetBytes( result );
            //WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            WebOperationContext.Current.OutgoingResponse.Headers.Set( "Content-Disposition", "attachment; filename=export.csv;" );
            return new MemoryStream( resultBytes );
        }

    }
}

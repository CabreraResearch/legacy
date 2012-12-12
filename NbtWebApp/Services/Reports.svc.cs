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
using System.Collections.Specialized;

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
        [WebInvoke( BodyStyle = WebMessageBodyStyle.Bare, Method = "POST" )]
        [Description( "Generate a csv report file" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream reportCSV( Stream dataStream )
        {

            string body = new StreamReader( dataStream ).ReadToEnd();
            NameValueCollection formData = HttpUtility.ParseQueryString( body );

            CswNbtWebServiceReport.ReportReturn Ret = new CswNbtWebServiceReport.ReportReturn();
            CswNbtWebServiceReport.ReportData Request = new CswNbtWebServiceReport.ReportData();
            Request.nodeId = formData["reportid"];
            Request.reportFormat = formData["reportFormat"];
            formData.Remove( "reportid" );
            formData.Remove( "reportFormat" );
            Request.reportParams = CswNbtWebServiceReport.FormReportParamsToCollection( formData );

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceReport.ReportReturn, CswNbtWebServiceReport.ReportData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceReport.runReportCSV,
                ParamObj: Request
                );
            SvcDriver.run();

            WebOperationContext.Current.OutgoingResponse.Headers.Set( "Content-Disposition", "attachment; filename=export.csv;" );

            return Request.stream;
        }
    }
}

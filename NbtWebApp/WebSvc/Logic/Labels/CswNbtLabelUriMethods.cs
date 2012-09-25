using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Labels;

namespace NbtWebApp.WebSvc.Session
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtLabelUriMethods
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebGet( UriTemplate = "type/{TargetTypeId}" )]

        [Description( "Get all Print labels matching this Target Type" )]
        public CswNbtLabelList list( string TargetTypeId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelList Ret = new CswNbtLabelList();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelList, NbtPrintLabel.Request.List>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.getLabels,
                ParamObj: new NbtPrintLabel.Request.List { TargetTypeId = CswConvert.ToInt32( TargetTypeId ) }
                );

            SvcDriver.run();
            return ( Ret );

        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebGet( UriTemplate = "label/{PrintLabelId}/target/{TargetId}" )]

        [Description( "Get a collection of EPL texts for the selected Targets" )]
        public CswNbtLabelEpl get( string PrintLabelId, string TargetId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelEpl Ret = new CswNbtLabelEpl();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelEpl, NbtPrintLabel.Request.Get>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.getEPLText,
                ParamObj: new NbtPrintLabel.Request.Get { LabelId = PrintLabelId, TargetId = TargetId }
                );

            SvcDriver.run();
            return ( Ret );
        }

    }
}
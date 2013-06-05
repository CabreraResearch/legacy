using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.ViewEditor;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for the View Editor
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class ViewEditor
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetStepData" )]
        [Description( "Get the data for a particular View Editor step" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse GetStepData( CswNbtViewEditorData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.GetStepData,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "HandleAction" )]
        [Description( "Perform an action on a particular View Editor step" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse HandleStep( CswNbtViewEditorData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.HandleAction,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetPreview" )]
        [Description( "Get a preview of a view" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse GetPreview( CswNbtViewEditorData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.GetPreview,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Finalize" )]
        [Description( "Save a view" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse Finalize( CswNbtViewEditorData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.Finalize,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }

    [DataContract]
    public class CswNbtViewEditorResponse: CswWebSvcReturn
    {
        public CswNbtViewEditorResponse()
        {
            Data = new CswNbtViewEditorData();
        }

        [DataMember]
        public CswNbtViewEditorData Data = new CswNbtViewEditorData();
    }
}
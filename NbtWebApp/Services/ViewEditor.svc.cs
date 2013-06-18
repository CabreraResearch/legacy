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
        [WebInvoke( Method = "POST", UriTemplate = "GetPreviewGrid" )]
        [Description( "Get a preview of a view" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse GetPreview( CswNbtViewEditorData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.GetPreviewGrid,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetPreviewTree" )]
        [Description( "Get a preview of a view" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtSdTrees.Contract.Response GetPreviewTree( CswNbtViewEditorData Request )
        {
            CswNbtSdTrees.Contract.Response Ret = new CswNbtSdTrees.Contract.Response();

            var SvcDriver = new CswWebSvcDriver<CswNbtSdTrees.Contract.Response, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.GetPreviewTree,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "InitializeVisibilitySelect" )]
        [Description( "Fetch the current users role and user info for a View Visibility Select" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewVisibilityResponse InitializeVisibilitySelect()
        {
            CswNbtViewVisibilityResponse Ret = new CswNbtViewVisibilityResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewVisibilityResponse, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.InitializeVisibilitySelect,
                ParamObj : ""
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

    [DataContract]
    public class CswNbtViewVisibilityResponse: CswWebSvcReturn
    {
        public CswNbtViewVisibilityResponse()
        {
            Data = new CswNbtViewVisibilityInitData();
        }

        [DataMember]
        public CswNbtViewVisibilityInitData Data = new CswNbtViewVisibilityInitData();
    }

    [DataContract]
    public class CswNbtViewVisibilityInitData
    {
        [DataMember]
        public string RoleId = string.Empty;

        [DataMember] 
        public string RoleName = string.Empty;

        [DataMember] 
        public string UserId = string.Empty;

        [DataMember] 
        public string Username = string.Empty;
    }
}
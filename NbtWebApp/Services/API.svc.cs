using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.API;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.Services
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class API
    {
        private HttpContext _Context = HttpContext.Current;

        #region Generic

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceREAD.VERB, UriTemplate = "/v1/{metadataname}/{id}", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get a single entity by id" )]
        public CswNbtResource GetResource( string metadataname, string id )
        {
            CswNbtAPIGenericRequest Req = new CswNbtAPIGenericRequest( metadataname, id );
            CswNbtResource Ret = new CswNbtResource();

            var SvcDriver = new CswWebSvcDriver<CswNbtResource, CswNbtAPIGenericRequest>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceREAD.GetResource,
                    ParamObj : Req
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.OK )
            {
                Ret = null;
            }

            return Ret;
        }

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceREAD.VERB, UriTemplate = "/v1/{metadataname}" )]
        [Description( "Get a collection of entities by NodeType" )]
        public CswNbtResourceCollection GetCollection( string metadataname )
        {
            CswNbtAPIGenericRequest Req = new CswNbtAPIGenericRequest( metadataname, string.Empty );
            CswNbtResourceCollection Ret = new CswNbtResourceCollection();

            Req.PropertyFilters = _Context.Request.QueryString;

            var SvcDriver = new CswWebSvcDriver<CswNbtResourceCollection, CswNbtAPIGenericRequest>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceREAD.GetCollection,
                    ParamObj : Req
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.OK )
            {
                Ret = null;
            }

            return Ret;
        }

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceCREATE.VERB, UriTemplate = "/v1/{metadataname}" )]
        [Description( "Create a new entity of the specified type" )]
        public CswNbtResource Create( string metadataname )
        {
            CswNbtAPIGenericRequest Req = new CswNbtAPIGenericRequest( metadataname, string.Empty );
            CswNbtResource Ret = new CswNbtResource();

            var SvcDriver = new CswWebSvcDriver<CswNbtResource, CswNbtAPIGenericRequest>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceCREATE.Create,
                    ParamObj : Req
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.Created )
            {
                Ret = null;
            }

            return Ret;
        }

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceUPDATE.VERB, UriTemplate = "/v1/{metadataname}/{id}",
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare
             )]
        [Description( "Update an entity " )]
        public void Update( CswNbtAPIGenericRequest Req, string metadataname, string id )
        {
            CswNbtAPIReturn Ret = new CswNbtAPIReturn();
            CswNbtAPIGenericRequest Req2 = new CswNbtAPIGenericRequest( metadataname, id );
            Req2.PropData = Req.PropData;

            var SvcDriver = new CswWebSvcDriver<CswNbtAPIReturn, CswNbtAPIGenericRequest>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceUPDATE.Edit,
                    ParamObj : Req2
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.OK )
            {
                Ret = null;
            }
        }

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceDELETE.VERB, UriTemplate = "/v1/{metadataname}/{id}", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Delete an entity" )]
        public void DeleteResource( string metadataname, string id )
        {
            CswNbtAPIGenericRequest Req = new CswNbtAPIGenericRequest( metadataname, id );
            CswNbtResource Ret = new CswNbtResource();

            var SvcDriver = new CswWebSvcDriver<CswNbtResource, CswNbtAPIGenericRequest>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceDELETE.Delete,
                    ParamObj : Req
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.OK )
            {
                Ret = null;
            }
        }

        #endregion

        #region Search

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceApiSearch.VERB, UriTemplate = "/v1/search?searchtype={searchtype}&query={query}", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Run a universal search" )]
        public CswNbtResourceCollection Search( string searchtype, string query )
        {
            CswNbtApiSearchRequest Req = new CswNbtApiSearchRequest( query, searchtype );
            CswNbtResourceCollection Ret = new CswNbtResourceCollection();

            //Look for optional params
            Req.NodeType = _Context.Request.Params["nodetype"] ?? string.Empty;

            var SvcDriver = new CswWebSvcDriver<CswNbtResourceCollection, CswNbtApiSearchRequest>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceApiSearch.Search,
                    ParamObj : Req
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.OK )
            {
                Ret = null;
            }

            return Ret;
        }

        #endregion

        #region Views

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceApiViews.VERB, UriTemplate = "/v1/grid/{viewid}", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Fetch a grid view by Id" )]
        public CswNbtAPIGrid RunGrid( string viewid )
        {
            CswNbtAPIGrid Ret = new CswNbtAPIGrid();

            var SvcDriver = new CswWebSvcDriver<CswNbtAPIGrid, int>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceApiViews.RunGrid,
                    ParamObj : CswConvert.ToInt32( viewid )
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.OK )
            {
                Ret = null;
            }

            return Ret;
        }

        [OperationContract]
        [WebInvoke( Method = CswNbtWebServiceApiViews.VERB, UriTemplate = "/v1/tree/{viewid}", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Fetch a grid view by Id" )]
        public CswNbtAPITree RunTree( string viewid )
        {
            CswNbtAPITree Ret = new CswNbtAPITree();

            var SvcDriver = new CswWebSvcDriver<CswNbtAPITree, int>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : Ret,
                    WebSvcMethodPtr : CswNbtWebServiceApiViews.RunTree,
                    ParamObj : CswConvert.ToInt32( viewid )
                    );

            SvcDriver.run();

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = Ret.Status;

            if( ctx.OutgoingResponse.StatusCode != HttpStatusCode.OK )
            {
                Ret = null;
            }

            return Ret;
        }

        #endregion

    }
}

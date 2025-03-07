﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
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
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceREAD.GetResource,
                    ParamObj: Req
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
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceREAD.GetCollection,
                    ParamObj: Req
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
        public CswNbtResource Create( Collection<CswNbtWcfProperty> Properties, string metadataname )
        {
            CswNbtAPIGenericRequest Req = new CswNbtAPIGenericRequest( metadataname, string.Empty );
            CswNbtResource Ret = new CswNbtResource();
            if( null != Properties )
            {
                Req.Properties = Properties;
            }

            var SvcDriver = new CswWebSvcDriver<CswNbtResource, CswNbtAPIGenericRequest>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceCREATE.Create,
                    ParamObj: Req
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
        public void Update( CswNbtResource ResourceToUpdate, string metadataname, string id )
        {
            WebOperationContext ctx = WebOperationContext.Current;

            CswNbtAPIReturn Ret = new CswNbtAPIReturn();
            CswNbtAPIGenericRequest Req = new CswNbtAPIGenericRequest( metadataname, id );

            bool idsMatch = true;
            if( null != ResourceToUpdate )
            {
                Req.ResourceToUpdate = ResourceToUpdate;
                if( ResourceToUpdate.NodeId.PrimaryKey != Req.NodeId.PrimaryKey )
                {
                    //if someone posts a different node with a different ID in the URI template that's a problem.
                    ctx.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                    idsMatch = false;
                }
            }

            if( idsMatch )
            {
                var SvcDriver = new CswWebSvcDriver<CswNbtAPIReturn, CswNbtAPIGenericRequest>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceUPDATE.Edit,
                    ParamObj: Req
                    );

                SvcDriver.run();
                ctx.OutgoingResponse.StatusCode = Ret.Status;
            }

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
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceDELETE.Delete,
                    ParamObj: Req
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
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceApiSearch.Search,
                    ParamObj: Req
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
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceApiViews.RunGrid,
                    ParamObj: CswConvert.ToInt32( viewid )
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
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: Ret,
                    WebSvcMethodPtr: CswNbtWebServiceApiViews.RunTree,
                    ParamObj: CswConvert.ToInt32( viewid )
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

        #region Options

        [OperationContract]
        [WebInvoke( Method = "OPTIONS", UriTemplate = "/v1/*", ResponseFormat = WebMessageFormat.Json )]
        public void GetOptions()
        {

            CswCommaDelimitedString Options = new CswCommaDelimitedString();

            string Url = _Context.Request.Url.PathAndQuery;
            Url = Url.Substring( Url.IndexOf( "/" ) + 1 );  // trim starting /
            Url = Url.Substring( Url.IndexOf( "/" ) + 1 );  // trim /{webappname}
            Url = Url.Substring( Url.IndexOf( "/" ) + 1 );  // trim /api

            if( Url.StartsWith( "v1" ) )
            {
                if( Url.StartsWith( "v1/search" ) )
                {
                    Options = new CswCommaDelimitedString() { CswNbtWebServiceApiSearch.VERB };
                }
                else if( Url.StartsWith( "v1/grid" ) )
                {
                    Options = new CswCommaDelimitedString() { CswNbtWebServiceApiViews.VERB };
                }
                else if( Url.StartsWith( "v1/tree" ) )
                {
                    Options = new CswCommaDelimitedString() { CswNbtWebServiceApiViews.VERB };
                }
                else
                {
                    if( Url.Split( '/' ).Length > 2 ) // "v1/{metadataname}/{id}"
                    {
                        Options = new CswCommaDelimitedString() { CswNbtWebServiceREAD.VERB, CswNbtWebServiceUPDATE.VERB, CswNbtWebServiceDELETE.VERB };
                    }
                    else  // "v1/{metadataname}"
                    {
                        Options = new CswCommaDelimitedString() { CswNbtWebServiceREAD.VERB, CswNbtWebServiceCREATE.VERB };
                    }
                }
            }
            _Context.Response.Headers.Remove( "Access-Control-Allow-Methods" );    // Remove the header set in global.asax's EnableCrossDmainAjaxCall()
            _Context.Response.Headers.Add( "Access-Control-Allow-Methods", Options.ToString() );
        }

        #endregion Options
    }
}

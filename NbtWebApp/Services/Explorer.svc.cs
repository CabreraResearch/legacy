using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;
using NbtWebApp.Actions.Explorer;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Explorer
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Initialize" )]
        [Description( "Initialize the Node Explorer" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtExplorerReturn Initialize( CswNbtExplorerRequest Request )
        {
            CswNbtExplorerReturn Ret = new CswNbtExplorerReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtExplorerReturn, CswNbtExplorerRequest>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceNodeExplorer.Initialize,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetFilterOpts" )]
        [Description( "Initialize the Node Explorer" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtExplorerReturn GetFilterOpts()
        {
            CswNbtExplorerReturn Ret = new CswNbtExplorerReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtExplorerReturn, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceNodeExplorer.GetFilterOpts,
                ParamObj : string.Empty
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetRelating" )]
        [Description( "Get nodes of a type relating to another" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtExplorerRelatingReturn GetRelating( CswNbtExplorerRequest Req )
        {
            CswNbtExplorerRelatingReturn Ret = new CswNbtExplorerRelatingReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtExplorerRelatingReturn, CswNbtExplorerRequest>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceNodeExplorer.GetRelating,
                ParamObj : Req
                );

            SvcDriver.run();
            return ( Ret );
        }


    }

}
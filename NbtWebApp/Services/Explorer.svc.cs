using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;
using NbtWebApp.Actions.Explorer;

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
        public CswNbtExplorerReturn Initialize( string NodeId )
        {
            CswNbtExplorerReturn Ret = new CswNbtExplorerReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtExplorerReturn, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceNodeExplorer.Initialize,
                ParamObj : NodeId
                );

            SvcDriver.run();
            return ( Ret );
        }
    }

}
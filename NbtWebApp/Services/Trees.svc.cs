using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Trees
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Trees
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "run")]
        [Description( "Returns the results of a View formatted as a Tree." )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtSdTrees.Contract.Response getTreeOfView( CswNbtSdTrees.Contract.Request Request )
        {
            CswNbtSdTrees.Contract.Response Ret = new CswNbtSdTrees.Contract.Response();

            var SvcDriver = new CswWebSvcDriver<CswNbtSdTrees.Contract.Response, CswNbtSdTrees.Contract.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr : CswNbtSdTrees.runTree,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "run/{ViewName}" )]
        [Description( "Returns the results of a View formatted as a Tree." )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtSdTrees.Contract.Response getTreeByViewName( string ViewName )
        {
            CswNbtSdTrees.Contract.Response Ret = new CswNbtSdTrees.Contract.Response();

            var SvcDriver = new CswWebSvcDriver<CswNbtSdTrees.Contract.Response, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtSdTrees.runTree,
                ParamObj : ViewName
                );
            
            SvcDriver.run();
            return ( Ret );
        }
    }
}

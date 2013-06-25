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
    /// WCF Web Methods for Search features - Currently includes searching list options
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Search
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "searchListOptions" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceSearch.CswNbtSearchReturn searchListOptions( CswNbtWebServiceSearch.CswNbtSearchRequest Request )
        {
            CswNbtWebServiceSearch.CswNbtSearchReturn Ret = new CswNbtWebServiceSearch.CswNbtSearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceSearch.CswNbtSearchReturn, CswNbtWebServiceSearch.CswNbtSearchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSearch.doListOptionsSearch,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}

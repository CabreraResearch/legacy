using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for ChemCatCentral Searches
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class ChemCatCentral
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// Search ChemCatCentral database.
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetAvailableDataSources" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn GetAvailableDataSources( CswC3Params CswC3Params )
        {
            //JObject Ret = new JObject();

            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            //CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
            //ReturnVal = ws.doChemCatCentralSearch( CswC3SearchParams );

            //CswNbtViewReturn Ret = new CswNbtViewReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3Params>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.GetAvailableDataSources,
                ParamObj: CswC3Params
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// Search ChemCatCentral database.
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Search" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn runC3Search( CswC3SearchParams CswC3SearchParams )
        {
            //JObject Ret = new JObject();

            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            //CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
            //ReturnVal = ws.doChemCatCentralSearch( CswC3SearchParams );

            //CswNbtViewReturn Ret = new CswNbtViewReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3SearchParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.RunChemCatCentralSearch,
                ParamObj: CswC3SearchParams
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}

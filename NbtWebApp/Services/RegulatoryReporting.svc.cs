using System.ComponentModel;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.WebServices;
using ChemSW.Security;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Regulatory Reporting operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class RegulatoryReporting
    {
        private HttpContext _Context = HttpContext.Current;
        
        [OperationContract]
        //        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Xml )]
        [WebInvoke( Method = "GET", UriTemplate = "getHMISDataTable?ControlZone={ControlZone}&Class={Class}" )]
        [Description( "Get all reportable hazardous Materials and their total quantities for a given Control Zone, optionally filtered to a Class" )]
        [FaultContract( typeof( FaultException ) )]
        public DataTable getHMISDataTable( string ControlZone, string Class )
        {
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest()
                {
                    ControlZone = ControlZone,
                    Class = Class
                };
            CswNbtWebServiceRegulatoryReporting.HMISDataTableReturn Ret = new CswNbtWebServiceRegulatoryReporting.HMISDataTableReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceRegulatoryReporting.HMISDataTableReturn, HMISData.HMISDataRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRegulatoryReporting.getHMISDataTable,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret.Data );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all reportable Materials and their max and average quantities in a given Location for the given timeframe" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceRegulatoryReporting.TierIIDataReturn getTierIIData( TierIIData.TierIIDataRequest Request )
        {
            CswNbtWebServiceRegulatoryReporting.TierIIDataReturn Ret = new CswNbtWebServiceRegulatoryReporting.TierIIDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceRegulatoryReporting.TierIIDataReturn, TierIIData.TierIIDataRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRegulatoryReporting.getTierIIData,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}

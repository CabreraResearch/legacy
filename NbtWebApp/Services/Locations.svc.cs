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
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Locations
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Assign specified inventory group to specified locations" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse assignInventoryGroupToLocations( CswNbtWebServiceLocationsCis.AssignInventoryGroupData.AssignRequest Request )
        {
            CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse ReturnVal = new CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse, CswNbtWebServiceLocationsCis.AssignInventoryGroupData.AssignRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ReturnVal,
                WebSvcMethodPtr: CswNbtWebServiceLocationsCis.assignInventoryGroupToLocations,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( ReturnVal );
        }

    }//Locations
}

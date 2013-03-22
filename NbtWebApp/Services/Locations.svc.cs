using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Container operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Locations
    {


        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Assign specified inventory group to specified locations" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLocations.AssignInventoryGroupResponse assignInventoryGroupToLocations( CswNbtWebServiceLocations.AssignInventoryGroupData.AssignRequest Request )
        {
            CswNbtWebServiceLocations.AssignInventoryGroupResponse ReturnVal = new CswNbtWebServiceLocations.AssignInventoryGroupResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLocations.AssignInventoryGroupResponse, CswNbtWebServiceLocations.AssignInventoryGroupData.AssignRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ReturnVal,
                WebSvcMethodPtr: CswNbtWebServiceLocations.assignInventoryGroupToLocations,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( ReturnVal );
        }

    }//Locations
}

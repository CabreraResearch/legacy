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
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Generate a list of Locations" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLocations.CswNbtLocationReturn list( bool IsMobile = true )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceLocations.CswNbtLocationReturn Ret = new CswNbtWebServiceLocations.CswNbtLocationReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLocations.CswNbtLocationReturn, bool>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLocations.getLocationsList,
                ParamObj: IsMobile
                );

            SvcDriver.run();
            return ( Ret );
        }

    }
}

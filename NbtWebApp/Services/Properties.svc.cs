using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp.Services
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Properties
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Save a file" )]
        [FaultContract( typeof( FaultException ) )]
        public string GetMenuMode( string propid )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( propid );

            string ret = "Button"; //default is button
            var SvcDriver = new CswWebSvcDriver<string, CswPropIdAttr>(
                    CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj : ret,
                    WebSvcMethodPtr : CswNbtWebServiceProperties.GetButtonMode,
                    ParamObj : PropIdAttr
                    );

            SvcDriver.run();

            return ret;
        }


    }
}

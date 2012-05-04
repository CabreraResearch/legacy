using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using NbtWebAppServices.Response;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtWsLocations
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtSessionResources _CswNbtSessionResources = null;

        [OperationContract]
        [WebGet( UriTemplate = "Locations/list" )]
        public CswNbtWebServiceResponse list()
        {
            CswNbtWebServiceResponse Ret = new CswNbtWebServiceResponse( _Context );
            try
            {
                _CswNbtSessionResources = Ret.CswNbtSessionResources;
            }
            catch( Exception Ex )
            {
                Ret.addError( Ex );
            }
            Ret.finalizeResponse( null );
            return Ret;
        }


    }
}
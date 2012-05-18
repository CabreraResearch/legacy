using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtWcfTestingUriMethods
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebGet]
        [Description( "Test for connectivity against NbtServices." )]
        public bool ping()
        {
            return true;
        }

    }
}
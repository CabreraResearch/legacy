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
    public class CswNbtWsDefault
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtSessionResources _CswNbtSessionResources = null;

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        public CswNbtWebServiceResponseBase post( CswNbtWebServiceRequest.CswNbtSessionRequest request )
        {
            CswNbtWebServiceResponseBase Ret = new CswNbtWebServiceResponseBase( _Context );
            if( Ret.Status.Success )
            {
                try
                {
                    _CswNbtSessionResources = Ret.CswNbtSessionResources;
                }
                catch( Exception ex )
                {
                    Ret.addError( ex );
                }
            }
            Ret.finalizeResponse();
            return Ret; //_AddAuthenticationStatus( SessionAuthenticationStatus.Authenticated );
        }

        [OperationContract]
        [WebGet]
        public CswNbtWebServiceResponseBase get()
        {
            CswNbtWebServiceResponseBase Ret = new CswNbtWebServiceResponseBase( _Context );
            if( Ret.Status.Success )
            {
                try
                {
                    _CswNbtSessionResources = Ret.CswNbtSessionResources;
                }
                catch( Exception Ex )
                {
                    Ret.addError( Ex );
                }
            }
            Ret.finalizeResponse();
            return Ret;
        } // get()
    }
}
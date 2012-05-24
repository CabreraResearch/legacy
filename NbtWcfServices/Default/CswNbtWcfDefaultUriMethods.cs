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
    public class CswNbtWcfDefaultUriMethods
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtWcfSessionResources _CswNbtWcfSessionResources = null;

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        public CswNbtWcfResponseBase post( CswNbtWcfRequest.CswNbtSessionRequest request )
        {
            CswNbtWcfResponseBase Ret = new CswNbtWcfResponseBase( _Context, request.IsMobile );
            if( Ret.Status.Success )
            {
                try
                {
                    _CswNbtWcfSessionResources = Ret.CswNbtWcfSessionResources;
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
        public CswNbtWcfResponseBase get( bool IsMobile )
        {
            CswNbtWcfResponseBase Ret = new CswNbtWcfResponseBase( _Context, IsMobile );
            if( Ret.Status.Success )
            {
                try
                {
                    _CswNbtWcfSessionResources = Ret.CswNbtWcfSessionResources;
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
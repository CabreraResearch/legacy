using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace NbtWebApp.WebSvc.Core
{
    class CswWebSvcHttpBehavior : WebHttpBehavior
    {
        protected override IDispatchMessageFormatter GetReplyDispatchFormatter( OperationDescription OperationDescription,
                                                                               ServiceEndpoint Endpoint )
        {
            WebGetAttribute WebGetAttribute = OperationDescription.Behaviors.Find<WebGetAttribute>();
            CswWebSvcDynamicAttribute MapAcceptedContentTypeToResponseEncodingAttribute = OperationDescription.Behaviors.Find<CswWebSvcDynamicAttribute>();

            if( WebGetAttribute != null && MapAcceptedContentTypeToResponseEncodingAttribute != null )
            {
                // We need two formatters, since we don't know what type we will need until runtime
                WebGetAttribute.ResponseFormat = WebMessageFormat.Json;
                IDispatchMessageFormatter JsonDispatchMessageFormatter = base.GetReplyDispatchFormatter( OperationDescription, Endpoint );

                WebGetAttribute.ResponseFormat = WebMessageFormat.Xml;
                IDispatchMessageFormatter XmlDispatchMessageFormatter = base.GetReplyDispatchFormatter( OperationDescription, Endpoint );

                return new CswWebSvcDynamicSerializer
                {
                    JsonDispatchMessageFormatter = JsonDispatchMessageFormatter,
                    XmlDispatchMessageFormatter = XmlDispatchMessageFormatter
                };
            }
            return base.GetReplyDispatchFormatter( OperationDescription, Endpoint );
        }
    }
}

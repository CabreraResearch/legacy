using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace NbtWebApp.WebSvc.Core
{
    public class CswWebSvcDynamicSerializer : IDispatchMessageFormatter
    {
        public IDispatchMessageFormatter JsonDispatchMessageFormatter { get; set; }
        public IDispatchMessageFormatter XmlDispatchMessageFormatter { get; set; }

        public void DeserializeRequest( Message Message, object[] Parameters )
        {
            throw new NotImplementedException();
        }

        public Message SerializeReply( MessageVersion MessageVersion, object[] Parameters, object Result )
        {
            Message Request = OperationContext.Current.RequestContext.RequestMessage;

            Message Ret = null;

            // This code is based on ContentTypeBasedDispatch example in WCF REST Starter Kit Samples
            // It calls either 
            HttpRequestMessageProperty Prop = (HttpRequestMessageProperty) Request.Properties[HttpRequestMessageProperty.Name];

            string accepts = Prop.Headers[HttpRequestHeader.Accept];
            if( accepts != null )
            {
                if( accepts.Contains( "text/xml" ) || accepts.Contains( "application/xml" ) )
                {
                    Ret = XmlDispatchMessageFormatter.SerializeReply( MessageVersion, Parameters, Result );
                }
                else if( accepts.Contains( "application/json" ) )
                {
                    Ret = JsonDispatchMessageFormatter.SerializeReply( MessageVersion, Parameters, Result );
                }
            }
            else
            {
                string contentType = Prop.Headers[HttpRequestHeader.ContentType];
                if( contentType != null )
                {
                    if( contentType.Contains( "text/xml" ) || contentType.Contains( "application/xml" ) )
                    {
                        Ret = XmlDispatchMessageFormatter.SerializeReply( MessageVersion, Parameters, Result );
                    }
                    else if( contentType.Contains( "application/json" ) )
                    {
                        Ret = JsonDispatchMessageFormatter.SerializeReply( MessageVersion, Parameters, Result );
                    }
                }
            }
            return Ret ?? ( Ret = JsonDispatchMessageFormatter.SerializeReply( MessageVersion, Parameters, Result ) );
        }



    }
}
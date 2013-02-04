//// from: http://megakemp.wordpress.com/2009/02/06/managing-shared-cookies-in-wcf/

//using System.ServiceModel.Channels;
//using System.ServiceModel.Description;

//namespace ChemSW
//{
//    public class CookieManagerBehavior : IEndpointBehavior
//    {
//        private CookieManagerMessageInspector c = new CookieManagerMessageInspector();
        
//        public void AddBindingParameters( ServiceEndpoint endpoint, BindingParameterCollection bindingParameters )
//        {
//        }

//        public void ApplyClientBehavior( ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime )
//        {
//            clientRuntime.MessageInspectors.Add( c );
//        }

//        public void ApplyDispatchBehavior( ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher )
//        {
//        }

//        public void Validate( ServiceEndpoint endpoint )
//        {
//        }
//    }
//}
using System;
using System.ServiceModel.Description;

namespace NbtWebApp.WebSvc.Core
{
    public class CswWebSvcDynamicAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters( OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters )
        {
        }

        public void ApplyClientBehavior( OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation )
        {
        }

        public void ApplyDispatchBehavior( OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation )
        {
        }

        public void Validate( OperationDescription operationDescription )
        {
        }
    }
}
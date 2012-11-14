using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace NbtWebApp.WebSvc.Core
{
    class CswWebSvcHost : WebServiceHost
    {
        public CswWebSvcHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses) {}

        protected override void OnOpening()
        {
            base.OnOpening();

            foreach (ServiceEndpoint Endpoint in Description.Endpoints)
            {
                if (Endpoint.Behaviors.Find<WebHttpBehavior>() != null)
                {
                    Endpoint.Behaviors.Remove<WebHttpBehavior>();
                    Endpoint.Behaviors.Add(new CswWebSvcHttpBehavior());
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;


namespace NbtWebApp.WebSvc.Core
{
    public class CswWebSvcHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type ServiceType, 
                                                         Uri[] BaseAddresses)
        {
            CswWebSvcHost CswWebSvcHost = 
                new CswWebSvcHost(ServiceType, BaseAddresses);
            return CswWebSvcHost;
        }
    }
}

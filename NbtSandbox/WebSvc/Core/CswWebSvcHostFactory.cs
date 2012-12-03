using System;
using System.ServiceModel;
using System.ServiceModel.Activation;


namespace NbtWebApp.WebSvc.Core
{
    public class CswWebSvcHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost( Type ServiceType,
                                                         Uri[] BaseAddresses )
        {
            CswWebSvcHost CswWebSvcHost =
                new CswWebSvcHost( ServiceType, BaseAddresses );
            return CswWebSvcHost;
        }
    }
}

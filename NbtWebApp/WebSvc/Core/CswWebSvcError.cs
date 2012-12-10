using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Log;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Security;
using ChemSW.Session;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace NbtWebApp
{

    [DataContract]
    public class CswWcfFault
    {

    }


    public class CswWcfErrorHandler : IErrorHandler
    {
        public bool HandleError( Exception error )
        {
            return true; //process the error in any way you want, and then return to indicate that the session is still valid
        }

        public void ProvideFault( Exception error, MessageVersion version, ref Message msg )
        {
            var vfc = new CswWcfFault();
            var fe = new FaultException<CswWcfFault>( vfc );
            var fault = fe.CreateMessageFault();
            msg = Message.CreateMessage( version, fault, "FATAL EXCEPTION: The following error was not handled by the normal exception trap: " + error.Message );
        }
    }

    public class CswWcfErrorHandlerExtension : BehaviorExtensionElement, IServiceBehavior
    {
        public override Type BehaviorType
        {
            get { return GetType(); }
        }

        protected override object CreateBehavior()
        {
            return this;
        }

        private IErrorHandler GetInstance()
        {
            return new CswWcfErrorHandler();
        }

        void IServiceBehavior.AddBindingParameters( ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters )
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior( ServiceDescription serviceDescription, ServiceHostBase serviceHostBase )
        {
            IErrorHandler errorHandlerInstance = GetInstance();
            foreach( ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers )
            {
                dispatcher.ErrorHandlers.Add( errorHandlerInstance );
            }
        }

        void IServiceBehavior.Validate( ServiceDescription serviceDescription, ServiceHostBase serviceHostBase )
        {
            foreach( ServiceEndpoint endpoint in serviceDescription.Endpoints )
            {
                if( endpoint.Contract.Name.Equals( "IMetadataExchange" ) &&
                    endpoint.Contract.Namespace.Equals( "http://schemas.microsoft.com/2006/04/mex" ) )
                    continue;

                foreach( OperationDescription description in endpoint.Contract.Operations )
                {
                    if( description.Faults.Count == 0 )
                    {
                        throw new InvalidOperationException( "FaultContractAttribute not found on this method" );
                    }
                }
            }
        }
    }


} // namespace ChemSW.Nbt.WebServices

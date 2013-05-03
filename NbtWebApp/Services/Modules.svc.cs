using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Services
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Modules
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Initialize" )]
        [Description( "Initialize the modules page" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtModulesPageReturn Initialize()
        {
            CswNbtModulesPageReturn ret = new CswNbtModulesPageReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtModulesPageReturn, object>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : ret,
                WebSvcMethodPtr : CswNbtWebServiceModules.Initialize,
                ParamObj : null
                );
        
            SvcDriver.run();
            return ( ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "HandleModule" )]
        [Description( "Handle a module click event" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtModulesPageReturn HandleModule(CswNbtDataContractModule Module)
        {
            CswNbtModulesPageReturn ret = new CswNbtModulesPageReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtModulesPageReturn, CswNbtDataContractModule>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : ret,
                WebSvcMethodPtr : CswNbtWebServiceModules.HandleModule,
                ParamObj : Module
                );

            SvcDriver.run();
            return ( ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "HandleModule" )]
        [Description( "Handle a module click event" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtModulesPageReturn HandleModule(CswNbtDataContractModule Module)
        {
            CswNbtModulesPageReturn ret = new CswNbtModulesPageReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtModulesPageReturn, CswNbtDataContractModule>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : ret,
                WebSvcMethodPtr : CswNbtWebServiceModules.HandleModule,
                ParamObj : Module
                );

            SvcDriver.run();
            return ( ret );
        }

    }

    [DataContract]
    public class CswNbtModulesPageReturn: CswWebSvcReturn
    {
        public CswNbtModulesPageReturn()
        {
            Data = new CswNbtDataContractModulePage();
        }

        [DataMember]
        public CswNbtDataContractModulePage Data;
    }
}
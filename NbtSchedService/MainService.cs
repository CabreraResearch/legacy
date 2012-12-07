using System;
using System.ServiceProcess;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace ChemSW.Nbt.SchedService
{
    public partial class MainService : ServiceBase
    {
        CswScheduleService _CswScheduleService = null;


        public WebServiceHost serviceHost = null;


        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart( string[] args )
        {


            //Uri baseAddress = new Uri( "http://localhost:8080/SchedService" );
            //using( ServiceHost host = new ServiceHost( typeof( CswSchedSvcAdminEndPoint ), baseAddress ) )
            //{
            //    // Enable metadata publishing.
            //    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            //    smb.HttpGetEnabled = true;
            //    smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            //    host.Description.Behaviors.Add( smb );

            //    // Open the ServiceHost to start listening for messages. Since
            //    // no endpoints are explicitly configured, the runtime will create
            //    // one endpoint per base address for each service contract implemented
            //    // by the service.
            //    host.Open();

            //    // Close the ServiceHost.
            //    host.Close();
            //}


            ///In this dispensation, you can invoke each servus from a browser as follows: 
            /// http://localhost:8010/SchedService/putServiceVersion?Version=foo
            /// http://localhost:8010/SchedService/getServiceVersion
            /// 
            /// This dispensation does not require any configuration in app.config
            if( serviceHost != null )
            {
                serviceHost.Close();
            }
            serviceHost = new WebServiceHost( typeof( CswSchedSvcAdminEndPoint ), new Uri( "http://localhost:8010/SchedService" ) );
            ServiceEndpoint serviceEndpoint = serviceHost.AddServiceEndpoint( typeof( ICswSchedSvcAdmin ), new WebHttpBinding(), "" );
            ServiceDebugBehavior serviceDebugBehavior = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.HttpHelpPageEnabled = false;

            serviceHost.Open();


            _CswScheduleService = new CswScheduleService( new CswScheduleLogicFactoryNbt(), new CswScheduleResourceFactoryNbt(), new CswScheduleLogicDetailPersistenceFactoryNbt() );
            _CswScheduleService.start();


        }//OnStart()

        protected override void OnStop()
        {
            _CswScheduleService.stop();

        }//OnStop()

    }//class MainService

}//namespace ChemSW.Nbt.SchedService

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


        //public ServiceHost serviceHost = null;


        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart( string[] args )
        {


            ////Uri baseAddress = new Uri( "http://localhost:8080/SchedService" );
            //if( serviceHost != null )
            //{
            //    serviceHost.Close();
            //}
            //serviceHost = new System.ServiceModel.ServiceHost( typeof( CswSchedSvcAdminEndPoint ) );

            _CswScheduleService = new CswScheduleService( new CswScheduleLogicFactoryNbt(), new CswScheduleResourceFactoryNbt(), new CswScheduleLogicDetailPersistenceFactoryNbt() );
            CswSchedSvcAdminEndPoint.CswScheduleService = _CswScheduleService;
            _CswScheduleService.start();
//            serviceHost.Open();


        }//OnStart()

        protected override void OnStop()
        {
            _CswScheduleService.stop();

        }//OnStop()

    }//class MainService

}//namespace ChemSW.Nbt.SchedService

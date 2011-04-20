using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt;

namespace ChemSW.Nbt.SchedService
{
    public partial class MainService : ServiceBase
    {
        CswNbtSchdItemRunner _CswNbtSchdItemRunner = null;

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart( string[] args )
        {
            CswScheduleService _CswScheduleService = new CswScheduleService( new CswScheduleLogicFactoryNbt(), new CswScheduleResourceFactoryNbt(), new CswScheduleLogicDetailPersistenceFactoryNbt() );
            _CswScheduleService.start();


        }//OnStart()

        protected override void OnStop()
        {
            _CswNbtSchdItemRunner.stop();

        }//OnStop()

    }//class MainService

}//namespace ChemSW.Nbt.SchedService

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
        CswScheduleService _CswScheduleService = null;

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart( string[] args )
        {
            _CswScheduleService = new CswScheduleService( new CswScheduleLogicFactoryNbt(), new CswScheduleResourceFactoryNbt(), new CswScheduleLogicDetailPersistenceFactoryNbt() );
            _CswScheduleService.start();


        }//OnStart()

        protected override void OnStop()
        {
            _CswScheduleService.stop();

        }//OnStop()

    }//class MainService

}//namespace ChemSW.Nbt.SchedService

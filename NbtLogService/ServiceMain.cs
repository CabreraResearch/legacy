using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Text;
using ChemSW.Nbt.Config;
using ChemSW.Log;
using ChemSW.Config;


namespace CswLogServiceNbt
{
    public partial class ServiceMain : ServiceBase
    {
        CswLogService _CswLogService = null;
        public ServiceMain()
        {
            InitializeComponent();
            _CswLogService = new CswLogService( new CswSetupVblsNbt( SetupMode.Executable ) ); 

        }

        protected override void OnStart( string[] args )
        {

            _CswLogService.start(); 
        }

        protected override void OnStop()
        {
            _CswLogService.start(); 
        }
    }
}

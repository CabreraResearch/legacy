using System.ServiceProcess;
using ChemSW.Config;
using ChemSW.Log;
using ChemSW.Nbt.Config;


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

using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.Security;
using ChemSW.RscAdo;
using ChemSW.Security;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleResourceFactoryNbt : ICswResourceFactory
    {


        public ICswResources make()
        {

            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode.NbtExe );
            CswEnumPooledConnectionState PooledConnectionState;

            if( SetupVbls.doesSettingExist( "CloseSchedulerDbConnections" ) )
            {
                if( true == CswConvert.ToBoolean( SetupVbls["CloseSchedulerDbConnections"] ) )
                {
                    PooledConnectionState = RscAdo.CswEnumPooledConnectionState.Closed;

                }
                else
                {
                    PooledConnectionState = RscAdo.CswEnumPooledConnectionState.Open;
                }
            }
            else
            {
                PooledConnectionState = RscAdo.CswEnumPooledConnectionState.Closed;
            }


            CswNbtResources ReturnVal = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false, null, PooledConnectionState );
            ReturnVal.InitCurrentUser = InitUser;
            return ( ReturnVal );
        }

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswEnumSystemUserNames.SysUsr_SchedSvc );
        }

    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


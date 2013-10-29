using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.RscAdo;
using ChemSW.Security;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleResourceFactoryNbt : ICswResourceFactory
    {
        public ICswResources make()
        {
            CswSetupVbls SetupVbls = new CswSetupVbls( CswEnumSetupMode.NbtExe );
            CswEnumPooledConnectionState PooledConnectionState;

            if( SetupVbls.doesSettingExist( "CloseSchedulerDbConnections" ) )
            {
                if( true == CswConvert.ToBoolean( SetupVbls["CloseSchedulerDbConnections"] ) )
                {
                    PooledConnectionState = CswEnumPooledConnectionState.Closed;
                }
                else
                {
                    PooledConnectionState = CswEnumPooledConnectionState.Open;
                }
            }
            else
            {
                PooledConnectionState = CswEnumPooledConnectionState.Closed;
            }

            CswNbtResources ReturnVal = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, CswEnumSetupMode.NbtExe, true, null, PooledConnectionState );
            ReturnVal.InitCurrentUser = InitUser;
            return ( ReturnVal );
        }

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswEnumSystemUserNames.SysUsr_SchedSvc );
        }

    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


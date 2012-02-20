using ChemSW.Config;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.Nbt.Config;
using ChemSW.Core;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleResourceFactoryNbt : ICswResourceFactory
    {


        public ICswResources make()
        {

            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode.NbtExe );
            PooledConnectionState PooledConnectionState; 

            if( SetupVbls.doesSettingExist( "CloseSchedulerDbConnections" ) )
            {
                if( true == CswConvert.ToBoolean( SetupVbls["CloseSchedulerDbConnections"] ) )
                {
                    PooledConnectionState = RscAdo.PooledConnectionState.Closed;

                }
                else
                {
                    PooledConnectionState = RscAdo.PooledConnectionState.Open;
                }
            }
            else
            {
                PooledConnectionState = RscAdo.PooledConnectionState.Closed;
            }


            CswNbtResources ReturnVal = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false, null, PooledConnectionState );
			ReturnVal.InitCurrentUser = InitUser;
            return ( ReturnVal );
        }

		public ICswUser InitUser(ICswResources Resources)
		{
			return new CswNbtSystemUser( Resources, "NbtScheduleServiceUser" );
		}

    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


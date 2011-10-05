using System;
using System.Collections.Generic;
using ChemSW.MtSched.Core;
using ChemSW.Core;
using ChemSW.Config;
using ChemSW.Nbt;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleResourceFactoryNbt : ICswResourceFactory
    {


        public ICswResources make()
        {
			CswNbtResources ReturnVal = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false );
			ReturnVal.InitCurrentUser = InitUser;
            return ( ReturnVal );
        }

		public ICswUser InitUser(ICswResources Resources)
		{
			return new CswNbtSystemUser( Resources, "NbtScheduleServiceUser" );
		}

    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


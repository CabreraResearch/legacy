using System;
using System.Collections.Generic;
using ChemSW.MtSched.Core;
using ChemSW.Core;
using ChemSW.Config;
using ChemSW.Nbt;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleResourceFactoryNbt : ICswResourceFactory
    {


        public ICswResources make()
        {
            return ( CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, new Config.CswSetupVblsNbt( SetupMode.Executable ), new Config.CswDbCfgInfoNbt( SetupMode.Executable ), CswTools.getConfigurationFilePath(SetupMode.Executable), true, false ) );
        }
    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


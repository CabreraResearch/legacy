using System;
using System.Collections.Generic;
using ChemSW.MtSched.Core;

namespace ChemSW.Cis.Sched
{

    public class CswScheduleLogicFactoryNbt : ICswScheduleLogicFactory
    {

        public List<ICswScheduleLogic> getRules()
        {
            List<ICswScheduleLogic> ReturnVal = new List<ICswScheduleLogic>();

            //ReturnVal.Add( new CswScheduleLogicCis3eFetchDocuments() );
            //ReturnVal.Add( new CswScheduleLogicCis3eGetUpdatedItems() );
            //ReturnVal.Add( new CswScheduleLogicCis3eNukeCatalogue() );
            //ReturnVal.Add( new CswScheduleLogicCis3eSynchTypes() );
            //ReturnVal.Add( new CswScheduleLogicCis3eUpdatePackageInfo() );

            return ( ReturnVal );

        }//getRules()


    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


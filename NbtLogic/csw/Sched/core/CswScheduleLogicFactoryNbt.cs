using System.Collections.Generic;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicFactoryNbt : ICswScheduleLogicFactory
    {

        public List<ICswScheduleLogic> getRules()
        {
            List<ICswScheduleLogic> ReturnVal = new List<ICswScheduleLogic>();

            ReturnVal.Add( new CswScheduleLogicNbtBatchOps() );
            ReturnVal.Add( new CswScheduleLogicNbtGenEmailRpt() );
            ReturnVal.Add( new CswScheduleLogicNbtGenNode() );
            ReturnVal.Add( new CswScheduleLogicNbtUpdtInspection() );
            ReturnVal.Add( new CswScheduleLogicNbtUpdtMTBF() );
            ReturnVal.Add( new CswScheduleLogicNbtUpdtPropVals() );
            ReturnVal.Add( new CswScheduleLogicNbtDisableCswAdmin() );
            //containExpiration

            return ( ReturnVal );

        }//getRules()


    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


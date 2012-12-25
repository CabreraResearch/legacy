using System.Collections.Generic;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Rules;

namespace ChemSW.Nbt.Sched
{

    public enum NbtScheduleRuleNames { Unknown, UpdtPropVals, UpdtMTBF, UpdtInspection, GenNode, GenEmailRpt, DisableChemSwAdmin, BatchOp, ExpiredContainers, MolFingerprints }
    public class CswScheduleLogicFactoryNbt : CswScheduleLogicFactoryBase
    {

        public CswScheduleLogicFactoryNbt()
        {
            _Rules.Add( new CswScheduleLogicNbtBatchOps() );
            _Rules.Add( new CswScheduleLogicNbtGenEmailRpt() );
            _Rules.Add( new CswScheduleLogicNbtGenNode() );
            _Rules.Add( new CswScheduleLogicNbtUpdtInspection() );
            _Rules.Add( new CswScheduleLogicNbtUpdtMTBF() );
            _Rules.Add( new CswScheduleLogicNbtUpdtPropVals() );
            _Rules.Add( new CswScheduleLogicNbtDisableCswAdmin() );
            _Rules.Add( new CswScheduleLogicNbtExpiredContainers() );
            _Rules.Add( new CswScheduleLogicNbtMolFingerprints() );
            _Rules.Add( new CswScheduleLogicNbtContainerReconciliationActions() );
        }

    }//CswReportTimingDaily

}//namespace ChemSW.MailRpt


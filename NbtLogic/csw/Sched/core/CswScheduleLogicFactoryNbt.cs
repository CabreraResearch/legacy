using System.Collections.Generic;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Rules;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicFactoryNbt: CswScheduleLogicFactoryBase
    {

        protected override List<ICswScheduleLogic> _getRulesFromImplmentationPlatform()
        {
            List<ICswScheduleLogic> ReturnVal = new List<ICswScheduleLogic>();

            ReturnVal.Add( new CswScheduleLogicNbtBatchOps() );
            ReturnVal.Add( new CswScheduleLogicNbtGenEmailRpt() );
            ReturnVal.Add( new CswScheduleLogicNbtGenNode() );
            ReturnVal.Add( new CswScheduleLogicNbtUpdtInspection() );
            ReturnVal.Add( new CswScheduleLogicNbtUpdtMTBF() );
            ReturnVal.Add( new CswScheduleLogicNbtUpdtPropVals() );
            ReturnVal.Add( new CswScheduleLogicNbtDisableCswAdmin() );
            ReturnVal.Add( new CswScheduleLogicNbtExpiredContainers() );
            ReturnVal.Add( new CswScheduleLogicNbtMolFingerprints() );
            ReturnVal.Add( new CswScheduleLogicNbtContainerReconciliationActions() );
            ReturnVal.Add( new CswScheduleLogicNbtGenRequests() );
            ReturnVal.Add( new CswScheduleLogicNbtTierII() );
            ReturnVal.Add( new CswScheduleLogicNbtCAFImport() );
            ReturnVal.Add( new CswScheduleLogicNbtExtChemDataSync() );
            ReturnVal.Add( new CswScheduleLogicNbtPurgeSessionData() );
            ReturnVal.Add( new CswScheduleLogicNbtNodeCounts() );
            ReturnVal.Add( new CswScheduleLogicNbtValidateAccessIds() );

            return ( ReturnVal );

        }
    }

}//_getRulesFromImplmentationPlatform()

using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicDetailPersistenceFactoryNbt : ICswScheduleLogicDetailPersistenceFactory
    {

        public ICswScheduleLogicDetailPersistence make() { return ( new CswScheduleLogicDetailPersistenceNbt() ); }

    }//CswScheduleLogicDetailPersistenceFactoryCis

}//namespace ChemSW.Nbt.Sched



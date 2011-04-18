using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicDetailPersistenceFactoryNbt : ICswScheduleLogicDetailPersistenceFactory
    {

        public ICswScheduleLogicDetailPersistence make() { return ( new CswScheduleLogicDetailPersistenceNbt() ); }

    }//CswScheduleLogicDetailPersistenceFactoryCis

}//namespace ChemSW.Nbt.Sched



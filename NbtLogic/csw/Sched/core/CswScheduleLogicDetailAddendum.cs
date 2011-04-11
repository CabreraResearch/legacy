using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt;

namespace ChemSW.Nbt.Sched
{
    public enum NbtScheduleRuleNames { UpdtPropVals, UpdtMTBF, UpdtInspection, GenNode, GenEmailRpt }
    public abstract class CswScheduleLogicDetailAddendum
    {
        /// <summary>
        /// These methods replace what used to be the CswNbtDbBasedSchdEvents.handleOnSchdItemWasRun
        /// idiom for the data-driven rules. Until such time as we have a common data-idiom for 
        /// all background events in NBT, the derivatives of this class will have to handle the 
        /// specific events. Note however that the run parmaeters and status tracking for the 
        /// multi-threaded schedule service will be read/written from the class to which 
        /// the methods in this classes are addended (i.e., CswScheduleLogicDetailPersistenceNbt).
        /// </summary>
        abstract public void read( CswScheduleLogicDetail CswScheduleLogicDetail );

        abstract public void write( CswScheduleLogicDetail CswScheduleLogicDetail );

    }//CswScheduleLogicDetailAddendum

}//namespace ChemSW.Nbt.Sched



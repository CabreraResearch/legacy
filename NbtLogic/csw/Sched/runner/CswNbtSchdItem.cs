using System;
using System.Collections;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Sched
{

    public abstract class CswNbtSchdItem //: ICswNbtSchedule
    {
        public delegate void CswNbtSchedEventHdlr( CswNbtSchdItem CswNbtSchdItem, CswNbtNode CswNbtNode );
        public CswNbtSchedEventHdlr OnScheduleItemWasRun = null;
        public string SchedItemName;

        public abstract bool doesItemRunNow();
        //public abstract DueType DueType { get; }
        //public abstract Int32 WarnDays { get; }
        public abstract void run();
        public abstract void reset();
        public abstract bool Succeeded { get; }
        public abstract string StatusMessage { get; }
        public abstract string Name { get; }

    }//ICswSchdItem

}//namespace ChemSW.Nbt.Sched

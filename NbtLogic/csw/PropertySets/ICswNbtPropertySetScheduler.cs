using System;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// This interface defines Object Classes to which the scheduler should pay attention 
    /// </summary>
    public interface ICswNbtPropertySetScheduler
    {
        string SchedulerFinalDueDatePropertyName { get; }
        string SchedulerNextDueDatePropertyName { get; }
        string SchedulerRunStatusPropertyName { get; }
        string SchedulerWarningDaysPropertyName { get; }
        string SchedulerDueDateIntervalPropertyName { get; }
        string SchedulerRunTimePropertyName { get; }

        CswNbtNodePropDateTime FinalDueDate { get; }
        CswNbtNodePropDateTime NextDueDate { get; }
        CswNbtNodePropComments RunStatus { get; }
        CswNbtNodePropNumber WarningDays { get; }
        CswNbtNodePropTimeInterval DueDateInterval { get; }
        CswNbtNodePropDateTime RunTime { get; }
        CswNbtNodePropLogical Enabled { get; }

        void updateNextDueDate( DateTime AfterDate, bool ForceUpdate, bool DeleteFutureNodes );

    }//ICswNbtPropertySetScheduler

}//namespace ChemSW.Nbt.ObjClasses

using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;


namespace ChemSW.Nbt.Sched
{
    /// <summary>
    /// Handles Scheduler events
    /// </summary>
    public class CswNbtDbBasedSchdEvents
    {

        /// <summary>
        /// Updates the NextDueDate of a PropertySetScheduler Node after the schedule item is run
        /// </summary>
        public void handleOnSchdItemWasRun( CswNbtSchdItem CswNbtSchdItem, CswNbtNode CswNbtSchedulerNode )
        {
            ICswNbtPropertySetScheduler SchedulerNode = CswNbtNodeCaster.AsPropertySetScheduler( CswNbtSchedulerNode );
            SchedulerNode.RunStatus.StaticText = CswNbtSchdItem.StatusMessage;
            if( CswNbtSchdItem.Succeeded )
            {
				DateTime CandidateNextDueDate = SchedulerNode.DueDateInterval.getNextOccuranceAfter( SchedulerNode.NextDueDate.DateTimeValue );
				if( SchedulerNode.FinalDueDate.DateTimeValue.Date != DateTime.MinValue &&
					( SchedulerNode.FinalDueDate.DateTimeValue.Date < DateTime.Now.Date ||
					  CandidateNextDueDate > SchedulerNode.FinalDueDate.DateTimeValue.Date ) )
                {
                    CandidateNextDueDate = DateTime.MinValue;
                }
				SchedulerNode.NextDueDate.DateTimeValue = CandidateNextDueDate;
            }
            CswNbtSchedulerNode.postChanges( false );

        }//handleOnSchdItemWasRun

    }//CswNbtDbBasedSchdEvents

}//namespace ChemSW.Nbt.Sched

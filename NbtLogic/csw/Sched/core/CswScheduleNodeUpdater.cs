using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;


namespace ChemSW.Nbt.Sched
{
    public class CswScheduleNodeUpdater
    {

        private CswNbtResources _CswNbtResources = null;
        public CswScheduleNodeUpdater( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public void update( CswNbtNode CswNbtNodeSchedualable, string StatusMessage )
        {
            ICswNbtPropertySetScheduler SchedulerNode = CswNbtNodeCaster.AsPropertySetScheduler( CswNbtNodeSchedualable );
            //case 25702 - add comment:
            SchedulerNode.RunStatus.AddComment( StatusMessage );
            DateTime CandidateNextDueDate = SchedulerNode.DueDateInterval.getNextOccuranceAfter( SchedulerNode.NextDueDate.DateTimeValue );
            if( SchedulerNode.FinalDueDate.DateTimeValue.Date != DateTime.MinValue &&
                ( SchedulerNode.FinalDueDate.DateTimeValue.Date < DateTime.Now.Date ||
                  CandidateNextDueDate > SchedulerNode.FinalDueDate.DateTimeValue.Date ) )
            {
                CandidateNextDueDate = DateTime.MinValue;
            }

            SchedulerNode.NextDueDate.DateTimeValue = CandidateNextDueDate;
            CswNbtNodeSchedualable.postChanges( false );

        }//update() 


    }//CswScheduleNodeUpdater

}//namespace ChemSW.MtSched



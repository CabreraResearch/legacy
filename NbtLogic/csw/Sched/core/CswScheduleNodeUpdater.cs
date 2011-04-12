using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.MtSched.Core;
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
            SchedulerNode.RunStatus.StaticText = StatusMessage;
            DateTime CandidateNextDueDate = SchedulerNode.DueDateInterval.getNextOccuranceAfter( SchedulerNode.NextDueDate.DateValue );
            if( SchedulerNode.FinalDueDate.DateValue.Date != DateTime.MinValue &&
                ( SchedulerNode.FinalDueDate.DateValue.Date < DateTime.Now.Date ||
                  CandidateNextDueDate > SchedulerNode.FinalDueDate.DateValue.Date ) )
            {
                CandidateNextDueDate = DateTime.MinValue;
            }

            SchedulerNode.NextDueDate.DateValue = CandidateNextDueDate;
            CswNbtNodeSchedualable.postChanges( false );

        }//update() 


    }//CswScheduleNodeUpdater

}//namespace ChemSW.MtSched



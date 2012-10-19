using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Batch;

namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// This class implements functions useful for all 'Scheduler' Property Set implementers
    /// For example: Generator, Mail Report
    /// </summary>
    public class CswNbtPropertySetSchedulerImpl
    {
        private ICswNbtPropertySetScheduler _Scheduler;
        private CswNbtNode _CswNbtNode;
        private CswNbtResources _CswNbtResources;

        public CswNbtPropertySetSchedulerImpl( CswNbtResources CswNbtResources, ICswNbtPropertySetScheduler Scheduler, CswNbtNode CswNbtNode )
        {
            _CswNbtResources = CswNbtResources;
            _Scheduler = Scheduler;
            _CswNbtNode = CswNbtNode;
        }

        bool _UpdateFutureTasks = false;

        public void updateNextDueDate( bool DeleteFutureNodes = false )
        {
            if( _Scheduler.DueDateInterval.WasModified ||
                _Scheduler.FinalDueDate.WasModified ||
                _CswNbtNode.New ||
                DeleteFutureNodes )
            {
                DateTime CandidateDueDate = DateTime.MinValue;
                if( _Scheduler.DueDateInterval.RateInterval.RateType != CswRateInterval.RateIntervalType.Unknown )
                {
                    CandidateDueDate = _Scheduler.DueDateInterval.getNextOccuranceAfter( DateTime.Now.Date );
                    if( _Scheduler.FinalDueDate.DateTimeValue != DateTime.MinValue &&
                        CandidateDueDate > _Scheduler.FinalDueDate.DateTimeValue )
                    {
                        CandidateDueDate = DateTime.MinValue;
                    }
                }
                _Scheduler.NextDueDate.DateTimeValue = CandidateDueDate;
                _UpdateFutureTasks = true;
            }//If one of the main properties is modified

        } // _updateNextDueDate()

        public void setLastFutureDate()
        {
            if( _UpdateFutureTasks )
            {
                CswNbtActGenerateFutureNodes CswNbtActGenerateFutureNodes = new CswNbtActGenerateFutureNodes( _CswNbtResources );

                DateTime LatestFutureDate = CswNbtActGenerateFutureNodes.getDateOfLastExistingFutureNode( _CswNbtNode );
                if( LatestFutureDate > DateTime.MinValue )
                {
                    CswNbtActGenerateFutureNodes.deleteExistingFutureNodes( _CswNbtNode );
                    CswNbtActGenerateFutureNodes.makeNodesBatch( _CswNbtNode, LatestFutureDate );
                }//if there are future nodes
            }
        } // _setLastFutureDate()

    }// class CswNbtPropertySetSchedulerImpl

}//namespace ChemSW.Nbt.ObjClasses

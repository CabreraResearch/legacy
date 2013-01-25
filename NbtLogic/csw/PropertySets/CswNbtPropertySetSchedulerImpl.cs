using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

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

        public static DateTime getNextDueDate( CswNbtNode Node, CswNbtNodePropDateTime NodePropNextDueDate, CswNbtNodePropTimeInterval NodePropInterval )
        {
            DateTime Ret = DateTime.MinValue;
            if( NodePropInterval.WasModified ||
                Node.New )
            {
                if( NodePropInterval.RateInterval.RateType != CswRateInterval.RateIntervalType.Unknown )
                {
                    DateTime AfterDate = DateTime.Now;
                    DateTime NextDueDate = NodePropNextDueDate.DateTimeValue;
                    if( NodePropInterval.WasModified || Node.New )
                    {
                        // Next Due Date might be invalid if the interval was altered
                        NextDueDate = DateTime.MinValue;
                    }
                    if( CswDateTime.GreaterThanNoMs( NextDueDate, AfterDate ) )
                    {
                        AfterDate = NextDueDate;
                    }

                    Ret = NodePropInterval.getNextOccuranceAfter( AfterDate );
                    
                } // if( _Scheduler.DueDateInterval.RateInterval.RateType != CswRateInterval.RateIntervalType.Unknown )
            }
            return Ret;
        } // updateNextDueDate()

        public void updateNextDueDate( bool ForceUpdate, bool DeleteFuture )
        {
            if( _Scheduler.FinalDueDate.WasModified ||
                DeleteFuture ||
                ForceUpdate )
            {
                DateTime CandidateNextDueDate = getNextDueDate( _CswNbtNode, _Scheduler.NextDueDate, _Scheduler.DueDateInterval );
                if(DateTime.MinValue != CandidateNextDueDate) {

                    DateTime FinalDueDate = _Scheduler.FinalDueDate.DateTimeValue;
                    if( DateTime.MinValue != FinalDueDate &&
                        CswDateTime.GreaterThanNoMs( CandidateNextDueDate, FinalDueDate ) )
                    {
                        CandidateNextDueDate = DateTime.MinValue;
                    }
                } // if( _Scheduler.DueDateInterval.RateInterval.RateType != CswRateInterval.RateIntervalType.Unknown )
                _Scheduler.NextDueDate.DateTimeValue = CandidateNextDueDate;

                _UpdateFutureTasks = DeleteFuture;
            }
        } // updateNextDueDate()

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

﻿using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using System;

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

        public static DateTime getNextDueDate( CswNbtNode Node, CswNbtNodePropDateTime NodePropNextDueDate, CswNbtNodePropTimeInterval NodePropInterval, bool ForceUpdate = false, bool DeleteFuture = false )
        {
            DateTime Ret = DateTime.MinValue;
            if( NodePropInterval.WasModified ||
                Node.New || 
                ForceUpdate || 
                DeleteFuture )
            {
                if (NodePropInterval.RateInterval.RateType != CswResources.UnknownEnum)
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
                    
                } // if( _Scheduler.DueDateInterval.RateInterval.RateType != CswEnumRateIntervalType.Unknown )
            }
            return Ret;
        } // updateNextDueDate()

        public void updateNextDueDate( bool ForceUpdate, bool DeleteFuture )
        {
            if( _Scheduler.DueDateInterval.WasModified || 
                _Scheduler.FinalDueDate.WasModified ||
                _CswNbtNode.New ||
                DeleteFuture ||
                ForceUpdate )
            {
                DateTime CandidateNextDueDate = getNextDueDate( _CswNbtNode, _Scheduler.NextDueDate, _Scheduler.DueDateInterval, ForceUpdate, DeleteFuture );
                if(DateTime.MinValue != CandidateNextDueDate) {

                    DateTime FinalDueDate = _Scheduler.FinalDueDate.DateTimeValue;
                    if( DateTime.MinValue != FinalDueDate &&
                        CswDateTime.GreaterThanNoMs( CandidateNextDueDate, FinalDueDate ) )
                    {
                        CandidateNextDueDate = DateTime.MinValue;
                    }
                } // if( _Scheduler.DueDateInterval.RateInterval.RateType != CswEnumRateIntervalType.Unknown )
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

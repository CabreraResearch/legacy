using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtUpdtPropVals : ICswScheduleLogic
    {

        public string RuleName
        {

            get { return ( NbtScheduleRuleNames.UpdtPropVals.ToString() ); }
        }

        public bool doesItemRunNow()
        {
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrence, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
        }


        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        private CswNbtResources _CswNbtResources = null;
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            //_CswNbtResources.AuditContext = "Scheduler Task: Update Property Values";
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
        }


        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }


        private const string _NodesPerCycleParamName = "NodesPerCycle";
        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;


            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {

                    if( _CswNbtResources == null )
                        throw new CswDniException( "_CswNbtResources is null" );

                    // Find which nodes are out of date
                    CswStaticSelect OutOfDateNodesQuerySelect = _CswNbtResources.makeCswStaticSelect( "OutOfDateNodes_select", "ValuesToUpdate" );
                    DataTable OutOfDateNodes = null;

                    Int32 NodesPerCycle = 1;
                    if( _CswScheduleLogicDetail.RunParams.ContainsKey( _NodesPerCycleParamName ) )
                    {
                        NodesPerCycle = Convert.ToInt32( _CswScheduleLogicDetail.RunParams[_NodesPerCycleParamName] );
                        OutOfDateNodesQuerySelect.getTable( false, false, 0, NodesPerCycle );
                        OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( false, false, 0, NodesPerCycle );

                        if( NodesPerCycle > OutOfDateNodes.Rows.Count ) //in case we didn't actually retrieve that amount
                        {
                            NodesPerCycle = OutOfDateNodes.Rows.Count;
                        }
                    }
                    else
                    {
                        OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( false, false, 0, 25 ); //use default page value
                        if( OutOfDateNodes.Rows.Count <= 0 )
                        {
                            NodesPerCycle = 0; //loop control
                        }
                    }

                    Int32 ErroneousNodeCount = 0;
                    string ErroneousNodes = "The following Node Id's do not have corresponding nodes: ";
                    for( Int32 idx = 0; ( idx < NodesPerCycle ); idx++ )
                    {
                        // Update one of them at random (which will keep us from encountering errors which gum up the queue)
                        Random rand = new Random();
                        Int32 index = rand.Next( 0, OutOfDateNodes.Rows.Count );

                        CswPrimaryKey nodeid = new CswPrimaryKey( "nodes", CswConvert.ToInt32( OutOfDateNodes.Rows[index]["nodeid"].ToString() ) );
                        CswNbtNode Node = _CswNbtResources.Nodes[nodeid];
                        if( Node != null )
                        {
                            if( Node.getNodeType() != null )
                            {
                                CswNbtActUpdatePropertyValue CswNbtActUpdatePropertyValue = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                                CswNbtActUpdatePropertyValue.UpdateNode( Node, false );
                                Node.postChanges( false );
                            }
                        }
                        else
                        {
                            ErroneousNodeCount++;
                            ErroneousNodes = nodeid.ToString();
                        }


                    }//if we have noes to process

                    if( 0 == ErroneousNodeCount )
                    {
                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    }
                    else
                    {
                        _CswScheduleLogicDetail.StatusMessage = ErroneousNodes;
                    }

                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtUpdtPropVals exception: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;//last line
                }



            }//if we're not shutting down

        }//threadCallBack() 

        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = MtSched.Core.LogicRunStatus.Idle;
        }

        public void releaseResources()
        {
            _CswNbtResources.release();
        }
    }//CswScheduleLogicNbtUpdtPropVals


}//namespace ChemSW.Nbt.Sched

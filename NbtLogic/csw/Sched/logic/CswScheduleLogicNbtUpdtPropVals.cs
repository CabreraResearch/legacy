using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtUpdtPropVals : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.UpdtPropVals ); }
        }

        //Determine the number of props that need to be updated and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            CswNbtResources NbtResources = ( CswNbtResources ) CswResources;
            CswStaticSelect OutOfDateNodesQuerySelect = NbtResources.makeCswStaticSelect( "OutOfDateNodes_select", "ValuesToUpdate" );
            DataTable OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( false, false );
            _CswScheduleLogicDetail.LoadCount = OutOfDateNodes.Rows.Count;
            return _CswScheduleLogicDetail.LoadCount;
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }


        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    // Find which nodes are out of date
                    CswStaticSelect OutOfDateNodesQuerySelect = CswNbtResources.makeCswStaticSelect( "OutOfDateNodes_select", "ValuesToUpdate" );
                    DataTable OutOfDateNodes = null;

                    int NodesPerCycle = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    OutOfDateNodesQuerySelect.getTable( false, false, 0, NodesPerCycle );
                    OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( false, false, 0, NodesPerCycle );

                    if( NodesPerCycle > OutOfDateNodes.Rows.Count ) //in case we didn't actually retrieve that amount
                    {
                        NodesPerCycle = OutOfDateNodes.Rows.Count;
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
                        CswNbtNode Node = CswNbtResources.Nodes[nodeid];
                        if( Node != null )
                        {
                            if( Node.getNodeType() != null )
                            {
                                CswNbtActUpdatePropertyValue CswNbtActUpdatePropertyValue = new CswNbtActUpdatePropertyValue( CswNbtResources );
                                CswNbtActUpdatePropertyValue.UpdateNode( Node, false );
                                // Case 28997: 
                                Node.postChanges( ForceUpdate: true );
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

                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line
                }
                catch( Exception Exception )
                {
                    CswNbtResources.logError( Exception );
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtUpdtPropVals exception: " + Exception.Message + "; " + Exception.StackTrace;
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;//last line
                }

            }//if we're not shutting down

        }//threadCallBack() 

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtUpdtPropVals

}//namespace ChemSW.Nbt.Sched

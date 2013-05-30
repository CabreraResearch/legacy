using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtUpdtPropVals : ICswScheduleLogic
    {
        private CswArbitrarySelect getValuesToUpdate( ICswResources CswResources )
        {
            string SQL = @"select o.objectclass, t.nodetypename, n.nodeid, p.nodetypepropid, j.jctnodepropid
                             from jct_nodes_props j
                             join nodes n on j.nodeid = n.nodeid
                             join nodetypes t on n.nodetypeid = t.nodetypeid
                             join object_class o on t.objectclassid = o.objectclassid
                             join nodetype_props p on p.nodetypepropid = j.nodetypepropid
                            where (j.pendingupdate = '1' or n.pendingupdate = '1')
                              and t.enabled = 1
                            order by n.nodeid, p.nodetypepropid";
            return CswResources.makeCswArbitrarySelect( "OutOfDateNodes_select", SQL );
        }

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.UpdtPropVals ); }
        }

        //Determine the number of props that need to be updated and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswArbitrarySelect OutOfDateNodesQuerySelect = getValuesToUpdate( CswResources );
            DataTable OutOfDateNodes = OutOfDateNodesQuerySelect.getTable();
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
                    CswArbitrarySelect OutOfDateNodesQuerySelect = getValuesToUpdate( CswNbtResources );
                    DataTable OutOfDateNodes = null;

                    Int32 NodesPerCycle = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    if( NodesPerCycle <= 0 )
                    {
                        NodesPerCycle = 25;
                    }
                    OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( 0, NodesPerCycle, false, false );
                    NodesPerCycle = OutOfDateNodes.Rows.Count;  //in case we didn't actually retrieve that amount

                    Int32 ErroneousNodeCount = 0;
                    string ErroneousNodes = "The following Nodes failed to update:\n";
                    for( Int32 idx = 0; ( idx < NodesPerCycle ); idx++ )
                    {
                        CswPrimaryKey nodeid = new CswPrimaryKey( "nodes", CswConvert.ToInt32( OutOfDateNodes.Rows[idx]["nodeid"].ToString() ) );
                        try//Case 29526 - if updating the node fails for whatever reason, log it and move on
                        {
                            CswNbtNode Node = CswNbtResources.Nodes[nodeid];
                            CswNbtActUpdatePropertyValue CswNbtActUpdatePropertyValue = new CswNbtActUpdatePropertyValue( CswNbtResources );
                            CswNbtActUpdatePropertyValue.UpdateNode( Node, false );
                            // Case 28997: 
                            Node.postChanges( ForceUpdate: true );
                        }
                        catch( Exception ex )
                        {
                            if( false == ErroneousNodes.Contains( CswConvert.ToString( nodeid ) ) )
                            {
                                ErroneousNodeCount++;
                                ErroneousNodes += nodeid + " - " + ex.Message + ex.StackTrace + "\n\n";
                            }
                        }

                    }//if we have nodes to process

                    _CswScheduleLogicDetail.StatusMessage = 0 == ErroneousNodeCount ? "Completed without error" : ErroneousNodes;

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

using System;
using System.Data;
using System.Collections;
using System.Text;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Actions;

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
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrance, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
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
			_CswNbtResources.AuditContext = "Scheduler Task: Update Property Values";
		}


        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private string _CompletionMessage = string.Empty;
        public string CompletionMessage
        {
            get { return ( _CompletionMessage ); }
        }

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
                    DataTable OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( false, false, 0, 25 );

                    if( OutOfDateNodes.Rows.Count > 0 )
                    {
                        // Update one of them at random (which will keep us from encountering errors which gum up the queue)
                        Random rand = new Random();
                        Int32 index = rand.Next( 0, OutOfDateNodes.Rows.Count );
                        CswPrimaryKey nodeid = new CswPrimaryKey( "nodes", CswConvert.ToInt32( OutOfDateNodes.Rows[index]["nodeid"].ToString() ) );
                        //Int32 propid = CswConvert.ToInt32(OutOfDateNodes.Rows[index]["nodetypepropid"].ToString());
                        //Int32 jctnodepropid = CswConvert.ToInt32(OutOfDateNodes.Rows[index]["jctnodepropid"].ToString());
                        CswNbtNode Node = _CswNbtResources.Nodes[nodeid];
                        if( Node == null )
                            throw new CswDniException( "Node not found (" + nodeid.ToString() + ")" );
                        // Don't update nodes of disabled nodetypes
                        if( Node.NodeType != null )
                        {
                            CswNbtActUpdatePropertyValue CswNbtActUpdatePropertyValue = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                            CswNbtActUpdatePropertyValue.UpdateNode( Node, false );
                            Node.postChanges( false );
                        }

                    }//if there were out of date nodes

                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }

                catch( Exception Exception )
                {
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

using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtNodeUpdateEvents : ICswScheduleLogic
    {
        #region Properties

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }
        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.NodeUpdateEvents ); }
        }

        private Int32 _ErroneousNodeCount = 0;
        private String _ErroneousNodes = "The following Nodes failed to update:\n";

        #endregion Properties

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //Returns the number of Nodes that require update events
        private CswArbitrarySelect getValuesToUpdate( ICswResources CswResources )
        {
            string SQL = @"select n.nodeid
                             from nodes n
                            where n.pendingevents = '1'";
            return CswResources.makeCswArbitrarySelect( "OutOfDateNodes_select", SQL );
        }

        //Determine the number of Nodes that need to be updated and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            CswArbitrarySelect PendingEventsNodesSelect = getValuesToUpdate( CswResources );
            DataTable NodesRequiringUpdateEvents = PendingEventsNodesSelect.getTable();
            Int32 LoadCount = NodesRequiringUpdateEvents.Rows.Count;
            return LoadCount;
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
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
                    processUpdateEvents( CswNbtResources );
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _CswScheduleLogicDetail.StatusMessage = 0 == _ErroneousNodeCount ? "Completed without error" : _ErroneousNodes;
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded;
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtNodeUpdateEvents exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

        #region Schedule-Specific Logic

        public void processUpdateEvents( CswNbtResources CswNbtResources )
        {
            CswArbitrarySelect PendingEventsNodesSelect = getValuesToUpdate( CswNbtResources );
            Int32 NodesPerCycle = _getNodesPerCycle( CswNbtResources );
            DataTable NodesRequiringUpdate = PendingEventsNodesSelect.getTable( 0, NodesPerCycle, false );
            NodesPerCycle = NodesRequiringUpdate.Rows.Count;

            for( Int32 idx = 0; ( idx < NodesPerCycle ); idx++ )
            {
                CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodesRequiringUpdate.Rows[idx]["nodeid"].ToString() ) );
                executeNodeUpdateEvents( CswNbtResources , NodeId);
            }
        }

        public void executeNodeUpdateEvents( CswNbtResources CswNbtResources, CswPrimaryKey NodeId )
        {
            try
            {
                CswNbtNode Node = CswNbtResources.Nodes[NodeId];
                
                //TODO - put node event logic here
                //Node.postChanges( ForceUpdate: true );
            }
            catch( Exception ex )
            {
                if( false == _ErroneousNodes.Contains( NodeId.ToString() ) )
                {
                    _ErroneousNodeCount++;
                    _ErroneousNodes += NodeId + " - " + ex.Message + ex.StackTrace + "\n\n";
                }
            }
        }

        #endregion Schedule-Specific Logic

        #region Private Helper Functions

        private Int32 _getNodesPerCycle( CswNbtResources CswNbtResources )
        {
            Int32 NodesPerCycle = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
            if( NodesPerCycle <= 0 )
            {
                NodesPerCycle = 25;
            }
            return NodesPerCycle;
        }

        #endregion Private Helper Functions

    }
}

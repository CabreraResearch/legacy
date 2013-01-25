using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;


namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenRequests: ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.GenRequest ); }
        }

        public bool hasLoad( ICswResources CswResources )
        {
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
            return ( true );
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
        }

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }



        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;
        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;


        }//initScheduleLogicDetail()


        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( _CswNbtResources );


            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    Int32 RequestsLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.generatorlimit.ToString() ) );
                    if( RequestsLimit <= 0 )
                    {
                        RequestsLimit = 1;
                    }

                    CswNbtActRequesting ActRequesting = new CswNbtActRequesting( _CswNbtResources );
                    CswNbtView AllRecurringRequests = ActRequesting.getAllRecurringRequestsItemsView();
                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( AllRecurringRequests, RequireViewPermissions : false, IncludeSystemNodes : false, IncludeHiddenNodes : false );

                    Int32 TotalRequestsProcessed = 0;
                    string RequestDescriptions = string.Empty;
                    Int32 TotatRequests = Tree.getChildNodeCount();

                    for( Int32 ChildN = 0; ( ChildN < TotatRequests && TotalRequestsProcessed < RequestsLimit ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); ChildN++ )
                    {
                        Tree.goToNthChild( ChildN );
                        CswNbtObjClassRequestMaterialDispense CurrentRequest = Tree.getNodeForCurrentPosition();

                        if( CurrentRequest.IsRecurring.Checked == Tristate.True )
                        {

                            try
                            {
                                CswNbtObjClassRequestMaterialDispense NewRequest = CswNbtObjClassRequestMaterialDispense.fromPropertySet( CurrentRequest.copyNode( PostChanges : false ) );
                                if( null != NewRequest )
                                {
                                    CswNbtObjClassUser Requestor = _CswNbtResources.Nodes[NewRequest.Requestor.RelatedNodeId];
                                    if( null != Requestor )
                                    {
                                        CswNbtActRequesting NewAction = new CswNbtActRequesting( _CswNbtResources, Requestor );
                                        NewRequest.Request.RelatedNodeId = NewAction.getCurrentRequestNodeId();
                                        NewRequest.IsRecurring.Checked = Tristate.False;
                                        NewRequest.postChanges( ForceUpdate : false );

                                        CurrentRequest.NextReorderDate.DateTimeValue = CswNbtPropertySetSchedulerImpl.getNextDueDate( CurrentRequest.Node, CurrentRequest.NextReorderDate, CurrentRequest.RecurringFrequency );
                                        CurrentRequest.postChanges( ForceUpdate : false );
                                    }
                                }

                                TotalRequestsProcessed += 1;
                                RequestDescriptions += CurrentRequest.Description + "; ";

                            }//try

                            catch( Exception Exception )
                            {
                                string Message = "Unable to process generator " + CurrentRequest.Description + ", which will now be disabled, due to the following exception: " + Exception.Message;
                                RequestDescriptions += Message;
                                CurrentRequest.postChanges( false );
                                _CswNbtResources.logError( new CswDniException( Message ) );

                            }//catch

                        } // if( CurrentGenerator.Enabled.Checked == Tristate.True )

                        Tree.goToParentNode();

                    }//iterate generators

                    _CswScheduleLogicDetail.StatusMessage = TotalRequestsProcessed.ToString() + " generators processed: " + RequestDescriptions;
                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenNode::GetUpdatedItems() exception: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }//catch

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
    }//CswScheduleLogicNbtGenNode


}//namespace ChemSW.Nbt.Sched

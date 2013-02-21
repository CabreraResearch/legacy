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
                    if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                    {
                        Int32 RequestsLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.generatorlimit.ToString() ) );
                        if( RequestsLimit <= 0 )
                        {
                            RequestsLimit = 1;
                        }

                        CswNbtActRequesting ActRequesting = new CswNbtActRequesting( _CswNbtResources );
                        CswNbtView AllRecurringRequests = ActRequesting.getDueRecurringRequestsItemsView();
                        ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( AllRecurringRequests, RequireViewPermissions: false, IncludeSystemNodes: false, IncludeHiddenNodes: false );

                        Int32 TotalRequestsProcessed = 0;
                        string RequestDescriptions = string.Empty;
                        Int32 TotatRequests = Tree.getChildNodeCount();

                        for( Int32 ChildN = 0; ( ChildN < TotatRequests && TotalRequestsProcessed < RequestsLimit ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); ChildN++ )
                        {
                            string Description = "";
                            try
                            {
                                Tree.goToNthChild( ChildN );
                                CswNbtObjClassRequestMaterialDispense CurrentRequestItem = Tree.getNodeForCurrentPosition();

                                if( null != CurrentRequestItem && // The Request Item isn't null
                                    CurrentRequestItem.IsRecurring.Checked == Tristate.True && // This is actually a recurring request
                                    false == CurrentRequestItem.RecurringFrequency.Empty && // The recurring frequency has been defined
                                    CurrentRequestItem.RecurringFrequency.RateInterval.RateType != CswRateInterval.RateIntervalType.Hourly || // Recurring on any frequency other than hourly
                                    ( CurrentRequestItem.NextReorderDate.DateTimeValue.Date <= DateTime.Today && // Recurring no more than once per hour
                                      DateTime.Now.AddHours( 1 ).Subtract( CurrentRequestItem.NextReorderDate.DateTimeValue ).Hours >= 1 ) ) //if we wait until the rule is overdue, then we'll never run more than once per hour.
                                {
                                    Description = CurrentRequestItem.Description.StaticText;

                                    CswNbtObjClassRequest RecurringRequest = _CswNbtResources.Nodes[CurrentRequestItem.Request.RelatedNodeId];
                                    if( null != RecurringRequest )
                                    {
                                        CswNbtObjClassUser Requestor = _CswNbtResources.Nodes[RecurringRequest.Requestor.RelatedNodeId];
                                        if( null != Requestor )
                                        {
                                            CswNbtObjClassRequestMaterialDispense NewRequestItem = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CurrentRequestItem.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                                            //CswNbtObjClassRequestMaterialDispense.fromPropertySet( CurrentRequest.copyNode( PostChanges : false ) );
                                            if( null != NewRequestItem )
                                            {
                                                // We'd get all of this for free if we used copyNode, 
                                                // but then we'd have to manually do as much work in the other direction:
                                                // un-hiding properties, etc.
                                                CswNbtActRequesting ThisUserAct = new CswNbtActRequesting( _CswNbtResources, Requestor );
                                                CswNbtObjClassRequest UsersCartNode = ThisUserAct.getCurrentRequestNode();
                                                if( null != UsersCartNode )
                                                {
                                                    // Most importantly, put the new request item in the current cart
                                                    NewRequestItem.Request.RelatedNodeId = UsersCartNode.NodeId;

                                                    NewRequestItem.Requestor.RelatedNodeId = CurrentRequestItem.Requestor.RelatedNodeId;
                                                    NewRequestItem.Material.RelatedNodeId = CurrentRequestItem.Material.RelatedNodeId;
                                                    NewRequestItem.Material.CachedNodeName = CurrentRequestItem.Material.CachedNodeName;
                                                    NewRequestItem.InventoryGroup.RelatedNodeId = CurrentRequestItem.InventoryGroup.RelatedNodeId;
                                                    NewRequestItem.Location.SelectedNodeId = CurrentRequestItem.Location.SelectedNodeId;
                                                    NewRequestItem.Location.CachedPath = CurrentRequestItem.Location.CachedPath;
                                                    NewRequestItem.Comments.CommentsJson = CurrentRequestItem.Comments.CommentsJson;
                                                    NewRequestItem.Type.Value = CurrentRequestItem.Type.Value;

                                                    if( CurrentRequestItem.Type.Value == CswNbtObjClassRequestMaterialDispense.Types.Bulk )
                                                    {
                                                        NewRequestItem.Quantity.Quantity = CurrentRequestItem.Quantity.Quantity;
                                                        NewRequestItem.Quantity.CachedUnitName = CurrentRequestItem.Quantity.CachedUnitName;
                                                        NewRequestItem.Quantity.UnitId = CurrentRequestItem.Quantity.UnitId;
                                                    }
                                                    else
                                                    {
                                                        NewRequestItem.Size.RelatedNodeId = CurrentRequestItem.Size.RelatedNodeId;
                                                        NewRequestItem.Size.CachedNodeName = CurrentRequestItem.Size.CachedNodeName;
                                                        NewRequestItem.Count.Value = CurrentRequestItem.Count.Value;
                                                    }
                                                    NewRequestItem.Status.Value = CswNbtObjClassRequestMaterialDispense.Statuses.Pending;
                                                    NewRequestItem.setRequestDescription();
                                                    NewRequestItem.postChanges( ForceUpdate: false );

                                                    CurrentRequestItem.NextReorderDate.DateTimeValue = CswNbtPropertySetSchedulerImpl.getNextDueDate( CurrentRequestItem.Node, CurrentRequestItem.NextReorderDate, CurrentRequestItem.RecurringFrequency, ForceUpdate: true );
                                                    CurrentRequestItem.postChanges( ForceUpdate: false );
                                                }
                                            }
                                        }
                                        RequestDescriptions += CurrentRequestItem.Description + "; ";
                                    }
                                }
                                Tree.goToParentNode();
                            } // if ~( not null, is recurring and is due)
                            catch( Exception Exception )
                            {
                                string Message = "Unable to create recurring request " + Description + ", due to the following exception: " + Exception.Message;
                                RequestDescriptions += Message;
                                _CswNbtResources.logError( new CswDniException( Message ) );

                            } //catch
                            finally
                            {
                                TotalRequestsProcessed += 1;
                            }
                        } //iterate requests
                        
                        string StatusMessage = "No Recurring Requests found to process";
                        if( TotalRequestsProcessed > 0 )
                        {
                            StatusMessage = TotalRequestsProcessed.ToString() + " requests processed: " + RequestDescriptions;
                        }
                        _CswScheduleLogicDetail.StatusMessage = StatusMessage;
                        _LogicRunStatus = LogicRunStatus.Succeeded; //last line
                    }
                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenRequests::threadCallBack() exception: " + Exception.Message;
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
    }//CswScheduleLogicNbtGenRequests


}//namespace ChemSW.Nbt.Sched

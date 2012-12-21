using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Batch;
using ChemSW.Config;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtContainerReconciliationActions : ICswScheduleLogic
    {
        #region Properties

        private CswNbtResources _CswNbtResources;
        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.Reconciliation.ToString() ); }
        }

        #endregion Properties

        #region Scheduler Methods

        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetailIn )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetailIn;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
        }

        public bool doesItemRunNow()
        {
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrence, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
        }

        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = LogicRunStatus.Idle;
        }

        public void releaseResources()
        {
            _CswNbtResources.release();
        }

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;
            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                    {
                        makeReconciliationActionBatchProcess();                        
                    }
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = LogicRunStatus.Succeeded;
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtContainerReconciliationActions exception: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

        #region Schedule-Specific Logic

        public void makeReconciliationActionBatchProcess()
        {
            CswNbtView ContainerLocationsView = getOutstandingContainerLocations();
            CswCommaDelimitedString ContainerLocations = getContainerLocationIds( ContainerLocationsView );
            if( ContainerLocations.Count > 0 )
            {
                CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( _CswNbtResources );
                Int32 ContainersProcessedPerIteration =
                    CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                BatchOp.makeBatchOp( ContainerLocations, ContainersProcessedPerIteration );
            }
        }

        public CswNbtView getOutstandingContainerLocations()
        {
            CswNbtView ContainerLocationsView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass ContainerLocationOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            CswNbtViewRelationship ParentRelationship = ContainerLocationsView.AddViewRelationship( ContainerLocationOc, true );
            CswNbtMetaDataObjectClassProp ActionAppliedOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ActionApplied );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ActionAppliedOcp,
                Value: Tristate.True.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.Checked,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            CswNbtMetaDataObjectClassProp ActionOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Action );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ActionOcp,
                Value: String.Empty,
                SubFieldName: CswNbtSubField.SubFieldName.Value,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ActionOcp,
                Value: CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.Value,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            return ContainerLocationsView;
        }

        public CswCommaDelimitedString getContainerLocationIds( CswNbtView ContainerLocationsView )
        {
            CswCommaDelimitedString ContainerLocations = new CswCommaDelimitedString();
            ICswNbtTree ContainerLocationsTree = _CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            int ContainerLocationCount = ContainerLocationsTree.getChildNodeCount();
            for( int i = 0; i < ContainerLocationCount; i++ )
            {
                ContainerLocationsTree.goToNthChild( i );
                ContainerLocations.Add( ContainerLocationsTree.getNodeIdForCurrentPosition().ToString() );
                ContainerLocationsTree.goToParentNode();
            }
            return ContainerLocations;
        }

        #endregion Schedule-Specific Logic

    }//CswScheduleLogicNbtUpdtMTBF
}//namespace ChemSW.Nbt.Sched

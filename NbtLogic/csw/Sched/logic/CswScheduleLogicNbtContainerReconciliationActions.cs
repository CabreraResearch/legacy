using System;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtContainerReconciliationActions : ICswScheduleLogic
    {
        #region Properties

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

        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetailIn )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetailIn;
        }


        public bool hasLoad( ICswResources CswResources )
        {
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
            return ( true );
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
        }

        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = LogicRunStatus.Idle;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                    {
                        makeReconciliationActionBatchProcess( CswNbtResources );
                    }
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = LogicRunStatus.Succeeded;
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtContainerReconciliationActions exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

        #region Schedule-Specific Logic

        public void makeReconciliationActionBatchProcess( CswNbtResources CswNbtResources )
        {
            CswNbtView ContainerLocationsView = getOutstandingContainerLocations( CswNbtResources );
            CswCommaDelimitedString ContainerLocations = getContainerLocationIds( CswNbtResources, ContainerLocationsView );
            if( ContainerLocations.Count > 0 )
            {
                CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( CswNbtResources );
                Int32 ContainersProcessedPerIteration =
                    CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                BatchOp.makeBatchOp( ContainerLocations, ContainersProcessedPerIteration );
            }
        }

        public CswNbtView getOutstandingContainerLocations( CswNbtResources CswNbtResources )
        {
            CswNbtView ContainerLocationsView = new CswNbtView( CswNbtResources );
            CswNbtMetaDataObjectClass ContainerLocationOc = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
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

        public CswCommaDelimitedString getContainerLocationIds( CswNbtResources CswNbtResources, CswNbtView ContainerLocationsView )
        {
            CswCommaDelimitedString ContainerLocations = new CswCommaDelimitedString();
            ICswNbtTree ContainerLocationsTree = CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
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

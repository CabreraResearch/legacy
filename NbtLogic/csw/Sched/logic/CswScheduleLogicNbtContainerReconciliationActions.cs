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

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
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
            get { return ( CswEnumNbtScheduleRuleNames.Reconciliation.ToString() ); }
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
                    if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
                    {
                        makeReconciliationActionBatchProcess( CswNbtResources );
                    }
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded;
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtContainerReconciliationActions exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
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
                    CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                BatchOp.makeBatchOp( ContainerLocations, ContainersProcessedPerIteration );
            }
        }

        public CswNbtView getOutstandingContainerLocations( CswNbtResources CswNbtResources )
        {
            CswNbtView ContainerLocationsView = new CswNbtView( CswNbtResources );
            CswNbtMetaDataObjectClass ContainerLocationOc = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtViewRelationship ParentRelationship = ContainerLocationsView.AddViewRelationship( ContainerLocationOc, true );
            CswNbtMetaDataObjectClassProp ActionAppliedOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ActionApplied );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ActionAppliedOcp,
                Value: CswEnumTristate.True.ToString(),
                SubFieldName: CswEnumNbtSubFieldName.Checked,
                FilterMode: CswEnumNbtFilterMode.NotEquals );
            CswNbtMetaDataObjectClassProp ActionOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Action );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ActionOcp,
                Value: String.Empty,
                SubFieldName: CswEnumNbtSubFieldName.Value,
                FilterMode: CswEnumNbtFilterMode.NotNull );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ActionOcp,
                Value: CswEnumNbtContainerLocationActionOptions.NoAction.ToString(),
                SubFieldName: CswEnumNbtSubFieldName.Value,
                FilterMode: CswEnumNbtFilterMode.NotEquals );
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

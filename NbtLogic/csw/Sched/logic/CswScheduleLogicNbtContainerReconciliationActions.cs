using System;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
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
        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.Reconciliation ); }
        }

        #endregion Properties

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //Returns the number of ContainerLocation nodes that require action
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = 0;
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtView ContainerLocationsView = getOutstandingContainerLocations( NbtResources );
                CswCommaDelimitedString ContainerLocations = getContainerLocationIds( NbtResources, ContainerLocationsView );
                _CswScheduleLogicDetail.LoadCount = ContainerLocations.Count;
            }
            return _CswScheduleLogicDetail.LoadCount;
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

        public void makeReconciliationActionBatchProcess( CswNbtResources _CswNbtResources )
        {
            CswNbtView ContainerLocationsView = getOutstandingContainerLocations( _CswNbtResources );
            CswCommaDelimitedString ContainerLocations = getContainerLocationIds( _CswNbtResources, ContainerLocationsView );
            if( ContainerLocations.Count > 0 )
            {
                Int32 ContainersProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                int TotalProcessedThisIteration = 0;
                foreach( string ContainerLocation in ContainerLocations )
                {
                    _executeReconciliationActions( _CswNbtResources, CswConvert.ToPrimaryKey( CswConvert.ToString( ContainerLocation ) ) );
                    TotalProcessedThisIteration++;
                    if( TotalProcessedThisIteration >= ContainersProcessedPerIteration )
                    {
                        break;
                    }
                }
            }
        }

        public CswNbtView getOutstandingContainerLocations( CswNbtResources _CswNbtResources )
        {
            CswNbtView ContainerLocationsView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass ContainerLocationOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
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

        public CswCommaDelimitedString getContainerLocationIds( CswNbtResources _CswNbtResources, CswNbtView ContainerLocationsView )
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

        #region Private Helper Functions

        private void _executeReconciliationActions( CswNbtResources _CswNbtResources, CswPrimaryKey ContainerLocationId )
        {
            CswNbtObjClassContainerLocation ContainerLocation = _CswNbtResources.Nodes[ContainerLocationId];
            if( null != ContainerLocation )
            {
                if( _isMostRecentContainerLocation( _CswNbtResources, ContainerLocation ) )
                {
                    _executeReconciliationAction( _CswNbtResources, ContainerLocation );
                }
                ContainerLocation.ActionApplied.Checked = CswEnumTristate.True;
                ContainerLocation.postChanges( false );
            }
        }

        private bool _isMostRecentContainerLocation( CswNbtResources _CswNbtResources, CswNbtObjClassContainerLocation ContainerLocation )
        {
            bool isMostRecent = true;
            CswNbtView ContainerLocationsView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass ContainerLocationOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtViewRelationship ParentRelationship = ContainerLocationsView.AddViewRelationship( ContainerLocationOc, true );
            ParentRelationship.NodeIdsToFilterOut.Add( ContainerLocation.NodeId );
            CswNbtMetaDataObjectClassProp ContainerOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ContainerOcp,
                Value: ContainerLocation.Container.RelatedNodeId.PrimaryKey.ToString(),
                SubFieldName: CswEnumNbtSubFieldName.NodeID,
                FilterMode: CswEnumNbtFilterMode.Equals );
            CswNbtMetaDataObjectClassProp ScanDateOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ScanDateOcp,
                Value: ContainerLocation.ScanDate.DateTimeValue.ToString(),
                SubFieldName: CswEnumNbtSubFieldName.Value,
                FilterMode: CswEnumNbtFilterMode.GreaterThan );
            ICswNbtTree ContainerLocationsTree = _CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            if( ContainerLocationsTree.getChildNodeCount() > 0 )
            {
                isMostRecent = false;
            }
            return isMostRecent;
        }

        private void _executeReconciliationAction( CswNbtResources _CswNbtResources, CswNbtObjClassContainerLocation ContainerLocation )
        {
            CswNbtObjClassContainer Container = _CswNbtResources.Nodes[ContainerLocation.Container.RelatedNodeId];
            if( null != Container )
            {
                CswEnumNbtContainerLocationActionOptions Action = ContainerLocation.Action.Value;
                if( Action == CswEnumNbtContainerLocationActionOptions.Undispose ||
                    Action == CswEnumNbtContainerLocationActionOptions.UndisposeAndMove )
                {
                    Container.UndisposeContainer( OverridePermissions: true, CreateContainerLocation: false );
                }
                if( Action == CswEnumNbtContainerLocationActionOptions.MoveToLocation ||
                    Action == CswEnumNbtContainerLocationActionOptions.UndisposeAndMove )
                {
                    Container.Location.SelectedNodeId = ContainerLocation.Location.SelectedNodeId;
                    Container.Location.RefreshNodeName();
                    Container.Location.CreateContainerLocation = false;
                }
                Container.Missing.Checked = Action == CswEnumNbtContainerLocationActionOptions.MarkMissing
                    ? CswEnumTristate.True
                    : CswEnumTristate.False;
                Container.postChanges( false );
            }
        }

        #endregion Private Helper Functions

    }//CswScheduleLogicNbtUpdtMTBF
}//namespace ChemSW.Nbt.Sched

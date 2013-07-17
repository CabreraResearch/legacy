using System;
using System.Collections.ObjectModel;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtExpiredContainers : ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.ExpiredContainers ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        #endregion Properties

        #region State

        private Collection<CswPrimaryKey> _ExpiredContainerIds = new Collection<CswPrimaryKey>();

        private void _setLoad( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                ICswNbtTree ExpiredContainersTree = _getExpiredContainersTree( NbtResources );
                for( int i = 0; i < ExpiredContainersTree.getChildNodeCount(); i++ )
                {
                    ExpiredContainersTree.goToNthChild( i );
                    _ExpiredContainerIds.Add( ExpiredContainersTree.getNodeIdForCurrentPosition() );
                    ExpiredContainersTree.goToParentNode();
                }
            }
        }

        #endregion State

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //Determine the number of expired containers and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            if( _ExpiredContainerIds.Count == 0 )
            {
                _setLoad( CswResources );
            }
            _CswScheduleLogicDetail.LoadCount = _ExpiredContainerIds.Count;
            return _CswScheduleLogicDetail.LoadCount;
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
                        int ContainersProcessedPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                        int TotalProcessedThisIteration = 0;
                        while( TotalProcessedThisIteration < ContainersProcessedPerIteration && _ExpiredContainerIds.Count > 0 && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ) )
                        {
                            CswNbtObjClassContainer expiredContainer = CswNbtResources.Nodes[_ExpiredContainerIds[0]];
                            if( null != expiredContainer )
                            {
                                expiredContainer.Status.Value = CswEnumNbtContainerStatuses.Expired;
                                expiredContainer.postChanges( false );
                            }
                            _ExpiredContainerIds.RemoveAt( 0 );
                            TotalProcessedThisIteration++;
                        }
                    }

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicExpiredContainers::GetExpiredContainers() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        private ICswNbtTree _getExpiredContainersTree( CswNbtResources CswNbtResources )
        {
            CswNbtView expiredContainersView = new CswNbtView( CswNbtResources );
            CswNbtMetaDataObjectClass containerOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp expirationDateOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );
            CswNbtMetaDataObjectClassProp statusOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status );
            CswNbtViewRelationship parent = expiredContainersView.AddViewRelationship( containerOC, true );
            expiredContainersView.AddViewPropertyAndFilter( parent,
                MetaDataProp: expirationDateOCP,
                Value: DateTime.Today.ToShortDateString(),
                SubFieldName: CswNbtFieldTypeRuleDateTime.SubFieldName.Value,
                FilterMode: CswEnumNbtFilterMode.LessThan );
            expiredContainersView.AddViewPropertyAndFilter( parent,
                MetaDataProp: statusOCP,
                Value: CswEnumNbtContainerStatuses.Expired,
                SubFieldName: CswNbtFieldTypeRuleList.SubFieldName.Value,
                FilterMode: CswEnumNbtFilterMode.NotEquals );
            ICswNbtTree expiredContainersTree = CswNbtResources.Trees.getTreeFromView( expiredContainersView, false, false, false );
            return expiredContainersTree;
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtExpiredContainers

}//namespace ChemSW.Nbt.Sched

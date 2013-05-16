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
    public class CswScheduleLogicNbtExpiredContainers : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.ExpiredContainers ); }
        }

        //Determine the number of expired containers and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = 0;
            CswNbtResources NbtResources = ( CswNbtResources ) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                ICswNbtTree ExpiredContainersTree = _getExpiredContainersTree( NbtResources );
                _CswScheduleLogicDetail.LoadCount = ExpiredContainersTree.getChildNodeCount();
            }
            return _CswScheduleLogicDetail.LoadCount;
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

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
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
                        ICswNbtTree expiredContainersTree = _getExpiredContainersTree( CswNbtResources );
                        int ContainersProcessedPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                        int ContainersToProcess = Math.Min( expiredContainersTree.getChildNodeCount(), ContainersProcessedPerIteration );
                        for( int i = 0; i < ContainersToProcess; i++ )
                        {
                            expiredContainersTree.goToNthChild( i );
                            CswNbtObjClassContainer expiredContainer = CswNbtResources.Nodes[expiredContainersTree.getNodeIdForCurrentPosition()];
                            if( null != expiredContainer )
                            {
                                expiredContainer.Status.Value = CswEnumNbtContainerStatuses.Expired;
                                expiredContainer.postChanges( false );
                            }
                            expiredContainersTree.goToParentNode();
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
            CswNbtViewRelationship parent = expiredContainersView.AddViewRelationship( containerOC, true );
            expiredContainersView.AddViewPropertyAndFilter( parent,
                MetaDataProp: expirationDateOCP,
                Value: DateTime.Today.ToShortDateString(),
                SubFieldName: CswEnumNbtSubFieldName.Value,
                FilterMode: CswEnumNbtFilterMode.LessThan );
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
    }//CswScheduleLogicNbtExpiredContainers

}//namespace ChemSW.Nbt.Sched

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
            get { return ( NbtScheduleRuleNames.ExpiredContainers ); }
        }

        //Determine the number of expired containers and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = 0;
            CswNbtResources NbtResources = ( CswNbtResources ) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                ICswNbtTree ExpiredContainersTree = _getExpiredContainersTree( NbtResources );
                _CswScheduleLogicDetail.LoadCount = ExpiredContainersTree.getChildNodeCount();
            }
            return _CswScheduleLogicDetail.LoadCount;
        }

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
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
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
                    {
                        ICswNbtTree expiredContainersTree = _getExpiredContainersTree( CswNbtResources );
                        int expiredContainersCount = expiredContainersTree.getChildNodeCount();
                        CswCommaDelimitedString expiredContainers = new CswCommaDelimitedString();
                        for( int i = 0; i < expiredContainersCount; i++ )
                        {
                            expiredContainersTree.goToNthChild( i );
                            expiredContainers.Add( expiredContainersTree.getNodeIdForCurrentPosition().ToString() );
                            expiredContainersTree.goToParentNode();
                        }

                        CswNbtBatchOpExpiredContainers batchOp = new CswNbtBatchOpExpiredContainers( CswNbtResources );
                        int ContainersProcessedPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                        batchOp.makeBatchOp( expiredContainers, ContainersProcessedPerIteration );
                    }

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicExpiredContainers::GetExpiredContainers() exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        private ICswNbtTree _getExpiredContainersTree( CswNbtResources CswNbtResources )
        {
            CswNbtView expiredContainersView = new CswNbtView( CswNbtResources );
            CswNbtMetaDataObjectClass containerOC = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp expirationDateOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );
            CswNbtViewRelationship parent = expiredContainersView.AddViewRelationship( containerOC, true );
            expiredContainersView.AddViewPropertyAndFilter( parent,
                MetaDataProp: expirationDateOCP,
                Value: DateTime.Today.ToShortDateString(),
                SubFieldName: CswNbtSubField.SubFieldName.Value,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThan );
            ICswNbtTree expiredContainersTree = CswNbtResources.Trees.getTreeFromView( expiredContainersView, false, false, false );
            return expiredContainersTree;
        }

        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = LogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtExpiredContainers

}//namespace ChemSW.Nbt.Sched

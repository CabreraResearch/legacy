using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Batch;
using ChemSW.Config;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtExpiredContainers : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.ExpiredContainers.ToString() ); }
        }

        public bool doesRuleHaveLoad()
        {
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrence, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
        }



        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }


        private CswNbtResources _CswNbtResources = null;
        public void initScheduleLogicDetail(  CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

        }


        public void initResource( ICswResources RuleResources )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;

        }//initResource()


        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    CswNbtView expiredContainersView = new CswNbtView( _CswNbtResources );
                    CswNbtMetaDataObjectClass containerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                    CswNbtMetaDataObjectClassProp expirationDateOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );
                    CswNbtViewRelationship parent = expiredContainersView.AddViewRelationship( containerOC, true );
                    expiredContainersView.AddViewPropertyAndFilter( parent,
                        MetaDataProp: expirationDateOCP,
                        Value: DateTime.Today.ToShortDateString(),
                        SubFieldName: CswNbtSubField.SubFieldName.Value,
                        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThan );

                    CswCommaDelimitedString expiredContainers = new CswCommaDelimitedString();
                    ICswNbtTree expiredContainersTree = _CswNbtResources.Trees.getTreeFromView( expiredContainersView, false, false, false );
                    int expiredContainersCount = expiredContainersTree.getChildNodeCount();
                    for( int i = 0; i < expiredContainersCount; i++ )
                    {
                        expiredContainersTree.goToNthChild( i );
                        expiredContainers.Add( expiredContainersTree.getNodeIdForCurrentPosition().ToString() );
                        expiredContainersTree.goToParentNode();
                    }

                    CswNbtBatchOpExpiredContainers batchOp = new CswNbtBatchOpExpiredContainers( _CswNbtResources );
                    int ContainersProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                    batchOp.makeBatchOp( expiredContainers, ContainersProcessedPerIteration );

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtUpdtMTBF::GetUpdatedItems() exception: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;

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

        public void releaseResources()
        {
            _CswNbtResources.release();
        }

    }//CswScheduleLogicNbtUpdtMTBF


}//namespace ChemSW.Nbt.Sched

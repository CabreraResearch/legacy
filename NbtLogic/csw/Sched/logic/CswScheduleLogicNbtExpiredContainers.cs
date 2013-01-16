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

    public class CswScheduleLogicNbtExpiredContainers : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.ExpiredContainers.ToString() ); }
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
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }


        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;

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
                        CswNbtView expiredContainersView = new CswNbtView( CswNbtResources );
                        CswNbtMetaDataObjectClass containerOC = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                        CswNbtMetaDataObjectClassProp expirationDateOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );
                        CswNbtViewRelationship parent = expiredContainersView.AddViewRelationship( containerOC, true );
                        expiredContainersView.AddViewPropertyAndFilter( parent,
                            MetaDataProp: expirationDateOCP,
                            Value: DateTime.Today.ToShortDateString(),
                            SubFieldName: CswNbtSubField.SubFieldName.Value,
                            FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThan );

                        CswCommaDelimitedString expiredContainers = new CswCommaDelimitedString();
                        ICswNbtTree expiredContainersTree = CswNbtResources.Trees.getTreeFromView( expiredContainersView, false, false, false );
                        int expiredContainersCount = expiredContainersTree.getChildNodeCount();
                        for( int i = 0; i < expiredContainersCount; i++ )
                        {
                            expiredContainersTree.goToNthChild( i );
                            expiredContainers.Add( expiredContainersTree.getNodeIdForCurrentPosition().ToString() );
                            expiredContainersTree.goToParentNode();
                        }

                        CswNbtBatchOpExpiredContainers batchOp = new CswNbtBatchOpExpiredContainers( CswNbtResources );
                        int ContainersProcessedPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                        batchOp.makeBatchOp( expiredContainers, ContainersProcessedPerIteration );

                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                        _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line
                    }

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicExpiredContainers::GetExpiredContainers() exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
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
    }//CswScheduleLogicNbtExpiredContainers


}//namespace ChemSW.Nbt.Sched

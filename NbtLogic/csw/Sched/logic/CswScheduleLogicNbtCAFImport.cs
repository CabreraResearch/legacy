using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Config;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Batch;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtCAFImport : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.CAFImport.ToString() ); }
        }

        public bool doesItemRunNow()
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
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

        }

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    int NumberToProcess = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                    CAFImportManager importManager = new CAFImportManager( _CswNbtResources, NumberToProcess );
                    importManager.Import();

                    //CswNbtMetaDataObjectClass batchOpOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.BatchOpClass );
                    //CswNbtMetaDataObjectClassProp nameOCP = batchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PropertyName.OpName );

                    //CswNbtView batchOpsView = new CswNbtView( _CswNbtResources );
                    //CswNbtViewRelationship parent = batchOpsView.AddViewRelationship( batchOpOC, false );
                    //batchOpsView.AddViewPropertyAndFilter( parent,
                    //    MetaDataProp: nameOCP,
                    //    Value: NbtBatchOpName.CAFImport.ToString(),
                    //    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                    //ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( batchOpsView, false, true, true );

                    //if( tree.getChildNodeCount() == 0 )
                    //{
                    //    CswNbtBatchOpCAFImport batchOp = new CswNbtBatchOpCAFImport( _CswNbtResources );
                    //    batchOp.makeBatchOp();

                    //}

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtCAFImport::ImportItems() exception: " + Exception.Message;
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

    }//CswScheduleLogicNbtCAFImpot


}//namespace ChemSW.Nbt.Sched

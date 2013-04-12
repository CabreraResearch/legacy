using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Config;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtCAFImport : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.CAFImport ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
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

        //this rule should probably not even exist - at the very least, it should never run
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = 0;
            return _CswScheduleLogicDetail.LoadCount;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
                try
                {
                    int NumberToProcess = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    CAFImportManager importManager = new CAFImportManager( _CswNbtResources, 1 );
                    importManager.Import();

                    //CswNbtMetaDataObjectClass batchOpOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.BatchOpClass );
                    //CswNbtMetaDataObjectClassProp nameOCP = batchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PropertyName.OpName );

                    //CswNbtView batchOpsView = new CswNbtView( _CswNbtResources );
                    //CswNbtViewRelationship parent = batchOpsView.AddViewRelationship( batchOpOC, false );
                    //batchOpsView.AddViewPropertyAndFilter( parent,
                    //    MetaDataProp: nameOCP,
                    //    Value: NbtBatchOpName.CAFImport.ToString(),
                    //    FilterMode: CswEnumNbtFilterMode.Equals );

                    //ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( batchOpsView, false, true, true );

                    //if( tree.getChildNodeCount() == 0 )
                    //{
                    //    CswNbtBatchOpCAFImport batchOp = new CswNbtBatchOpCAFImport( _CswNbtResources );
                    //    batchOp.makeBatchOp();

                    //}

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtCAFImport::ImportItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;

                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

    }//CswScheduleLogicNbtCAFImpot

}//namespace ChemSW.Nbt.Sched

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
using System.Collections.Generic;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtMolFingerprints : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.MolFingerprints.ToString() ); }
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
                    /*
                     *    get molFT
                     *    get all NTPs where ft = molft
                     *    foreach NTP in NTPs
                     *       get nodes that have this NTP 
                     * 
                     */
                    List<int> nonFingerprintedMols = new List<int>();

                    CswNbtMetaDataFieldType molFT = _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MOL );
                    CswTableSelect ts = _CswNbtResources.makeCswTableSelect( "getMolNTPs", "nodetype_props" );
                    DataTable nodetype_props = ts.getTable( "where fieldtypeid = " + molFT.FieldTypeId );
                    foreach( DataRow row in nodetype_props.Rows )
                    {
                        CswTableSelect ts_nodes = _CswNbtResources.makeCswTableSelect( "selectNonNullMolNodes", "jct_nodes_props" );
                        DataTable nonNullMolNodes = ts_nodes.getTable( "where clobdata is not null and nodetypepropid = " + row["nodetypepropid"] );

                        foreach( DataRow jctnode_row in nonNullMolNodes.Rows )
                        {
                            int nodeid = CswConvert.ToInt32( jctnode_row["nodeid"] );
                            if( false == _CswNbtResources.StructureSearchManager.DoesRecordExist( nodeid ) ) 
                            {
                                nonFingerprintedMols.Add( nodeid );
                            }
                        }
                    }

                    //create batch op                    

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

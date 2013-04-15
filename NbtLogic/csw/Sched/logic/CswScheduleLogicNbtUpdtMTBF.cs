using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtUpdtMTBF : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.UpdtMTBF.ToString() ); }
        }

        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        public bool hasLoad( ICswResources CswResources )
        {
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
            return ( true );
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
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



        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            //CswNbtResources.AuditContext = "Scheduler Task: Update MTBF";
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {

                    // BZ 6779
                    // Set all MTBF fields pendingupdate = 1
                    Int32 MTBFId = CswNbtResources.MetaData.getFieldType( ChemSW.Nbt.MetaData.CswEnumNbtFieldType.MTBF ).FieldTypeId;

                    CswTableSelect NTPSelect = CswNbtResources.makeCswTableSelect( "UpdateMTBF_NTP_Select", "nodetype_props" );
                    DataTable NTPTable = NTPSelect.getTable( "fieldtypeid", MTBFId );
                    string NTPIds = string.Empty;
                    foreach( DataRow NTPRow in NTPTable.Rows )
                    {
                        if( NTPIds != string.Empty ) NTPIds += ",";
                        NTPIds += CswConvert.ToInt32( NTPRow["nodetypepropid"] );
                    }

                    if( NTPIds != string.Empty )
                    {
                        CswTableUpdate JNPUpdate = CswNbtResources.makeCswTableUpdate( "UpdateMTBF_JNP_Update", "jct_nodes_props" );
                        DataTable JNPTable = JNPUpdate.getTable( "where nodetypepropid in (" + NTPIds + ")" );
                        foreach( DataRow JNPRow in JNPTable.Rows )
                        {
                            JNPRow["pendingupdate"] = CswConvert.ToDbVal( true );
                        }
                        JNPUpdate.update( JNPTable );
                    }

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = MtSched.Core.CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtUpdtMTBF::GetUpdatedItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = MtSched.Core.CswEnumScheduleLogicRunStatus.Failed;

                }//catch



            }//if we're not shutting down

        }//threadCallBack()


        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = MtSched.Core.CswEnumScheduleLogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtUpdtMTBF


}//namespace ChemSW.Nbt.Sched

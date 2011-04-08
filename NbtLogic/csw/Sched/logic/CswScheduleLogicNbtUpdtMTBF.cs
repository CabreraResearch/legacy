using System;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.MtSched.Core;
using ChemSW.DB;
using ChemSW.Exceptions;    

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtUpdtMTBF : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( "NbtUpdtMTBF" ); }
        }

        public bool doesItemRunNow()
        {
            throw new NotImplementedException(); 
            /*
            CswTableSelect SchedItemSelect = _CswNbtResources.makeCswTableSelect( "UpdateMTBF_doesItemRunNow_Select", "schedule_items" );
            DataTable SchedItemTable = SchedItemSelect.getTable( "where itemname = '" + this.SchedItemName + "'" );
            return ( SchedItemTable.Rows.Count == 0 ||
                     DateTime.Now.Subtract( CswConvert.ToDateTime( SchedItemTable.Rows[0]["lastrun"] ) ).TotalHours >= 24 );
             */
        }



        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private string _CompletionMessage = string.Empty;
        public string CompletionMessage
        {
            get { return ( _CompletionMessage ); }
        }


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
        }

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    // BZ 6779
                    // Set all MTBF fields pendingupdate = 1
                    Int32 MTBFId = _CswNbtResources.MetaData.getFieldType( ChemSW.Nbt.MetaData.CswNbtMetaDataFieldType.NbtFieldType.MTBF ).FieldTypeId;

                    CswTableSelect NTPSelect = _CswNbtResources.makeCswTableSelect( "UpdateMTBF_NTP_Select", "nodetype_props" );
                    DataTable NTPTable = NTPSelect.getTable( "fieldtypeid", MTBFId );
                    string NTPIds = string.Empty;
                    foreach( DataRow NTPRow in NTPTable.Rows )
                    {
                        if( NTPIds != string.Empty ) NTPIds += ",";
                        NTPIds += CswConvert.ToInt32( NTPRow["nodetypepropid"] );
                    }

                    if( NTPIds != string.Empty )
                    {
                        CswTableUpdate JNPUpdate = _CswNbtResources.makeCswTableUpdate( "UpdateMTBF_JNP_Update", "jct_nodes_props" );
                        DataTable JNPTable = JNPUpdate.getTable( "where nodetypepropid in (" + NTPIds + ")" );
                        foreach( DataRow JNPRow in JNPTable.Rows )
                        {
                            JNPRow["pendingupdate"] = CswConvert.ToDbVal( true );
                        }
                        JNPUpdate.update( JNPTable );
                    }

                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CompletionMessage = "Csw3ETasks::GetUpdatedItems() exception: " + Exception.Message;
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;
                    _CswNbtResources.logError( new CswDniException( _CompletionMessage ) );

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

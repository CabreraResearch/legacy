using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicDetailPersistenceNbt : ICswScheduleLogicDetailPersistence
    {

        private string _TableName = "scheduledrules";
        private string _ColName_RuleName = "rulename";
        private string _ColName_MaxRunTimeMs = "maxruntimems";
        private string _ColName_ThreadId = "threadid";
        private string _ColName_ReprobateThreshold = "reprobatethreshold";
        private string _ColName_TotalRogueCount = "totalroguecount";
        private string _ColName_FailedCount = "failedcount";
        private string _ColName_Reprobate = "reprobate";
        private string _ColName_Disabled = "disabled";
        private string _ColName_StatusMessage = "statusmessage";
        private string _ColName_Recurrence = "recurrence";
        private string _ColName_Interval = "interval";
        private string _ColName_RunStartTime = "runstarttime";
        private string _ColName_RunEndTime = "runendtime";
        private string _ColName_LastRun = "lastrun";



        private Dictionary<NbtScheduleRuleNames, CswScheduleLogicDetailAddendum> _ScheduleLogicAddenda = new Dictionary<NbtScheduleRuleNames, CswScheduleLogicDetailAddendum>(); 


        public CswScheduleLogicDetailPersistenceNbt()
        {
        }//

        private ICswResources _CswResources = null;
        public void init( ICswResources CswResources )
        {
            _CswResources = CswResources;
            _ScheduleLogicAddenda.Add( NbtScheduleRuleNames.UpdtInspection, new CswScheduleLogicDetailAddendumUpdtInspections( (CswNbtResources) _CswResources ) );
            _ScheduleLogicAddenda.Add( NbtScheduleRuleNames.GenNode, new CswScheduleLogicDetailAddendumGenNode( (CswNbtResources) _CswResources ) );
            _ScheduleLogicAddenda.Add( NbtScheduleRuleNames.GenEmailRpt, new CswScheduleLogicDetailAddendumGenEmailRpts( (CswNbtResources) _CswResources ) );

        }//init() 

        public CswScheduleLogicDetail read( string RuleName )
        {

            CswScheduleLogicDetail ReturnVal = new CswScheduleLogicDetail();

            ReturnVal.RuleName = RuleName;

            CswTableSelect CswTableSelectBgTasks = _CswResources.makeCswTableSelect( "CswScheduleLogicDetail_read", _TableName );
            DataTable DataTableBgTasks = CswTableSelectBgTasks.getTable( " where lower(rulename)='" + RuleName.ToLower() + "'", true );
            DataRow DataRowBgTasks = DataTableBgTasks.Rows[0];


            if( false == DataRowBgTasks.IsNull( _ColName_MaxRunTimeMs ) )
            {
                ReturnVal.MaxRunTimeMs = CswConvert.ToInt32( DataRowBgTasks[_ColName_MaxRunTimeMs] );
            }


            if( false == DataRowBgTasks.IsNull( _ColName_ThreadId ) )
            {
                ReturnVal.ThreadId = CswConvert.ToInt32( DataRowBgTasks[_ColName_ThreadId] );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_Reprobate ) )
            {
                ReturnVal.Reprobate = CswConvert.ToBoolean( DataRowBgTasks[_ColName_Reprobate] );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_TotalRogueCount ) )
            {
                ReturnVal.TotalRoqueCount = CswConvert.ToInt32( DataRowBgTasks[_ColName_TotalRogueCount] );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_StatusMessage ) )
            {
                ReturnVal.StatusMessage = DataRowBgTasks[_ColName_StatusMessage].ToString();
            }

            if( false == DataRowBgTasks.IsNull( _ColName_StatusMessage ) )
            {
                ReturnVal.StatusMessage = DataRowBgTasks[_ColName_StatusMessage].ToString();
            }

            if( false == DataRowBgTasks.IsNull( _ColName_Recurrence ) )
            {
                Enum.TryParse<Recurrance>( DataRowBgTasks[_ColName_Recurrence].ToString(), true, out ReturnVal.Recurrance );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_Interval ) )
            {
                ReturnVal.Interval = CswConvert.ToInt32( DataRowBgTasks[_ColName_Interval] );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_ReprobateThreshold ) )
            {
                ReturnVal.ReprobateThreshold = CswConvert.ToInt32( DataRowBgTasks[_ColName_ReprobateThreshold] );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_RunStartTime ) )
            {
                ReturnVal.RunStartTime = CswConvert.ToDateTime( DataRowBgTasks[_ColName_RunStartTime] );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_RunEndTime ) )
            {
                ReturnVal.RunEndTime = CswConvert.ToDateTime( DataRowBgTasks[_ColName_RunEndTime] );
            }

            if( false == DataRowBgTasks.IsNull( _ColName_Disabled ) )
            {
                ReturnVal.Disabled = CswConvert.ToBoolean( DataRowBgTasks[_ColName_Disabled] );
            }


            //foreach( DataRow CurrentRow in DataTableBgTasksInfo.Rows )
            //{
            //    ReturnVal.RunParams.Add( CurrentRow[_ColName_ParameterName].ToString(), CurrentRow[_ColName_ParameterValue] );
            //}

            NbtScheduleRuleNames RuleEnum; 
            Enum.TryParse<NbtScheduleRuleNames>( RuleName,true,out RuleEnum );
            _ScheduleLogicAddenda[RuleEnum].read( ReturnVal ); 
            

            return ( ReturnVal );

        }//read

        public void write( CswScheduleLogicDetail CswScheduleLogicDetail )
        {

            if( false == string.IsNullOrEmpty( CswScheduleLogicDetail.RuleName ) )
            {
                try
                {
                    _CswResources.beginTransaction();

                    CswTableUpdate CswTableUpdateBgTasks = _CswResources.makeCswTableUpdate( "CswScheduleLogicInfo_Update", "background_tasks" );
                    DataTable BgTasksDataTable = CswTableUpdateBgTasks.getTable( " where lower(taskname)='" + CswScheduleLogicDetail.RuleName.ToLower() + "'" );
                    DataRow BgTasksDataRow = BgTasksDataTable.Rows[0];

                    BgTasksDataRow[_ColName_MaxRunTimeMs] = CswScheduleLogicDetail.MaxRunTimeMs;
                    BgTasksDataRow[_ColName_ThreadId] = CswScheduleLogicDetail.ThreadId;
                    BgTasksDataRow[_ColName_Reprobate] = CswConvert.ToDbVal( CswScheduleLogicDetail.Reprobate );
                    BgTasksDataRow[_ColName_TotalRogueCount] = CswScheduleLogicDetail.TotalRoqueCount;
                    BgTasksDataRow[_ColName_StatusMessage] = CswScheduleLogicDetail.StatusMessage;
                    BgTasksDataRow[_ColName_Recurrence] = CswScheduleLogicDetail.Recurrance.ToString();
                    BgTasksDataRow[_ColName_Interval] = CswScheduleLogicDetail.Interval;
                    BgTasksDataRow[_ColName_ReprobateThreshold] = CswScheduleLogicDetail.ReprobateThreshold;
                    BgTasksDataRow[_ColName_RunStartTime] = CswScheduleLogicDetail.RunStartTime;
                    BgTasksDataRow[_ColName_RunEndTime] = CswScheduleLogicDetail.RunEndTime;
                    BgTasksDataRow[_ColName_FailedCount] = CswScheduleLogicDetail.FailedCount;
                    BgTasksDataRow[_ColName_Disabled] = CswConvert.ToDbVal( CswScheduleLogicDetail.Disabled );

                    CswTableUpdateBgTasks.update( BgTasksDataTable );

                    /*
                    if( CswScheduleLogicDetail.RunParams.Count > 0 )
                    {

                        CswTableUpdate CswTableUpdateBgTasksInfo = _CswResources.makeCswTableUpdate( "CswScheduleLogicInfoParams_Update", "background_tasks_info" );

                        CswCommaDelimitedString CswCommaDelimitedStringSelectCols = new CswCommaDelimitedString();
                        CswCommaDelimitedStringSelectCols.Add( _ColName_ParameterName );
                        CswCommaDelimitedStringSelectCols.Add( _ColName_ParameterValue );


                        CswCommaDelimitedString CswCommaDelimitedStringParams = new CswCommaDelimitedString( CswScheduleLogicDetail.RunParams.Keys.Count, true );
                        foreach( string CurrentParamName in CswScheduleLogicDetail.RunParams.Keys )
                        {
                            CswCommaDelimitedStringParams.Add( CurrentParamName );
                        }//iterate params



                        string WhereClause = " where backgroundtaskid = (select backgroundtaskid from background_tasks where lower(taskname)='" + CswScheduleLogicDetail.RuleName.ToLower() + "') and lower(" + _ColName_ParameterName + ") in (" + CswCommaDelimitedStringParams + ")";
                        DataTable BgTasksInfoDataTable = CswTableUpdateBgTasksInfo.getTable( CswCommaDelimitedStringSelectCols, "", Int32.MinValue, WhereClause, false );

                        foreach( DataRow CurrentRow in BgTasksInfoDataTable.Rows )
                        {
                            CurrentRow[_ColName_ParameterValue] = CswScheduleLogicDetail.RunParams[CurrentRow[_ColName_ParameterName].ToString()];
                        }//iterate rolws

                        CswTableUpdateBgTasksInfo.update( BgTasksInfoDataTable );

                    }//if we have run params to update
                     */


                    NbtScheduleRuleNames RuleEnum;
                    Enum.TryParse<NbtScheduleRuleNames>( CswScheduleLogicDetail.RuleName, true, out RuleEnum );
                    _ScheduleLogicAddenda[RuleEnum].write( CswScheduleLogicDetail ); 

                    _CswResources.commitTransaction();

                } //try 

                catch( Exception Exception )
                {
                    _CswResources.Rollback();
                    string Message = "Error writing rule meta data: " + Exception.Message;
                    _CswResources.CswLogger.reportError( new Exception( Message ) );
                }

            }//if string has data

        }//write() 

    }//CswScheduleLogicDetailPersistenceCis

}//namespace ChemSW.Nbt.Sched



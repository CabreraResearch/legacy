using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{
    public enum NbtScheduleRuleNames { Unknown, UpdtPropVals, UpdtMTBF, UpdtInspection, GenNode, GenEmailRpt, DisableChemSwAdmin, BatchOp, ExpiredContainers, MolFingerprints, Reconciliation, TierII }
    public enum NbtScheduledRuleColumns { Unknown, ScheduledRuleId, RuleName, MaxRunTimeMs, ThreadId, ReprobateThreshold, TotalRogueCount, FailedCount, Reprobate, Disabled, StatusMessage, Recurrence, Interval, RunStartTime, RunEndTime, LastRun }
    public enum NbtScheduledRuleParamsColumns { Unknown, ScheduledRuleParamId, ParamName, ParamVal }
    public class CswScheduleLogicDetailPersistenceNbt : ICswScheduleLogicDetailPersistence
    {

        private string _TableNameScheduledRules = "scheduledrules";
        private string _TableNameScheduledRuleParams = "scheduledruleparams";

        public CswScheduleLogicDetailPersistenceNbt()
        {
        }//

        private ICswResources _CswResources = null;
        public void init( ICswResources CswResources )
        {
            _CswResources = CswResources;


        }//init() 

        public CswScheduleLogicDetail read( string RuleName )
        {

            CswScheduleLogicDetail ReturnVal = new CswScheduleLogicDetail();

            ReturnVal.RuleName = RuleName;

            CswTableSelect CswTableSelectScheduledRules = _CswResources.makeCswTableSelect( "CswScheduleLogicDetail_read", _TableNameScheduledRules );
            DataTable DataTableScheduledRules = CswTableSelectScheduledRules.getTable( " where lower(rulename)='" + RuleName.ToLower() + "'", true );
            DataRow DataRowScheduledRules = DataTableScheduledRules.Rows[0];

            CswTableSelect CswTableSelectScheduledRuleParams = _CswResources.makeCswTableSelect( "CswScheduleLogicDetail_read", _TableNameScheduledRuleParams );
            DataTable DataTableScheduledRuleParams = CswTableSelectScheduledRuleParams.getTable( " where scheduledruleid=" + DataRowScheduledRules[NbtScheduledRuleColumns.ScheduledRuleId.ToString()].ToString(), true );

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.MaxRunTimeMs.ToString() ) )
            {
                ReturnVal.MaxRunTimeMs = CswConvert.ToInt32( DataRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.ThreadId.ToString() ) )
            {
                ReturnVal.ThreadId = CswConvert.ToInt32( DataRowScheduledRules[NbtScheduledRuleColumns.ThreadId.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.Reprobate.ToString() ) )
            {
                ReturnVal.Reprobate = CswConvert.ToBoolean( DataRowScheduledRules[NbtScheduledRuleColumns.Reprobate.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.TotalRogueCount.ToString() ) )
            {
                ReturnVal.TotalRogueCount = CswConvert.ToInt32( DataRowScheduledRules[NbtScheduledRuleColumns.TotalRogueCount.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.FailedCount.ToString() ) )
            {
                ReturnVal.FailedCount = CswConvert.ToInt32( DataRowScheduledRules[NbtScheduledRuleColumns.FailedCount.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.StatusMessage.ToString() ) )
            {
                ReturnVal.StatusMessage = DataRowScheduledRules[NbtScheduledRuleColumns.StatusMessage.ToString()].ToString();
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.StatusMessage.ToString() ) )
            {
                ReturnVal.StatusMessage = DataRowScheduledRules[NbtScheduledRuleColumns.StatusMessage.ToString()].ToString();
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.Recurrence.ToString() ) )
            {
                Enum.TryParse<Recurrence>( DataRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()].ToString(), true, out ReturnVal.Recurrence );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.Interval.ToString() ) )
            {
                ReturnVal.Interval = CswConvert.ToInt32( DataRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.ReprobateThreshold.ToString() ) )
            {
                ReturnVal.ReprobateThreshold = CswConvert.ToInt32( DataRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.RunStartTime.ToString() ) )
            {
                ReturnVal.RunStartTime = CswConvert.ToDateTime( DataRowScheduledRules[NbtScheduledRuleColumns.RunStartTime.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.RunEndTime.ToString() ) )
            {
                ReturnVal.RunEndTime = CswConvert.ToDateTime( DataRowScheduledRules[NbtScheduledRuleColumns.RunEndTime.ToString()] );
            }

            if( false == DataRowScheduledRules.IsNull( NbtScheduledRuleColumns.Disabled.ToString() ) )
            {
                ReturnVal.Disabled = CswConvert.ToBoolean( DataRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] );
            }


            foreach( DataRow CurrentRow in DataTableScheduledRuleParams.Rows )
            {
                ReturnVal.RunParams.Add( CurrentRow[NbtScheduledRuleParamsColumns.ParamName.ToString()].ToString(), CurrentRow[NbtScheduledRuleParamsColumns.ParamVal.ToString()] );
            }

            return ( ReturnVal );

        }//read

        public void write( CswScheduleLogicDetail CswScheduleLogicDetail )
        {

            if( false == string.IsNullOrEmpty( CswScheduleLogicDetail.RuleName.ToString() ) )
            {
                try
                {
                    _CswResources.beginTransaction();

                    CswTableUpdate CswTableUpdateScheduledRules = _CswResources.makeCswTableUpdate( _TableNameScheduledRules + "_Update", _TableNameScheduledRules );
                    DataTable DataTableScheduledRules = CswTableUpdateScheduledRules.getTable( " where lower(rulename)='" + CswScheduleLogicDetail.RuleName.ToLower() + "'" );
                    DataRow DataRowScheduledRules = DataTableScheduledRules.Rows[0];

                    //These are really control parameters that should be set only from the configuratiion application (or directly
                    //from the rules table). If we write these values here, we end up overwriting whatever changes are made to these
                    //values while the schedule service is running. I can't imagine a situation in which a schedule rule or even
                    //the schedule service infrastructure should be writing these values. 
                    //DataRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = CswScheduleLogicDetail.MaxRunTimeMs;
                    //DataRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = CswScheduleLogicDetail.Recurrence.ToString();
                    //DataRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = CswScheduleLogicDetail.Interval;
                    //DataRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = CswScheduleLogicDetail.ReprobateThreshold;

                    DataRowScheduledRules[NbtScheduledRuleColumns.ThreadId.ToString()] = CswScheduleLogicDetail.ThreadId;
                    DataRowScheduledRules[NbtScheduledRuleColumns.Reprobate.ToString()] = CswConvert.ToDbVal( CswScheduleLogicDetail.Reprobate );
                    DataRowScheduledRules[NbtScheduledRuleColumns.TotalRogueCount.ToString()] = CswScheduleLogicDetail.TotalRogueCount;
                    DataRowScheduledRules[NbtScheduledRuleColumns.StatusMessage.ToString()] = CswScheduleLogicDetail.StatusMessage;
                    DataRowScheduledRules[NbtScheduledRuleColumns.RunStartTime.ToString()] = CswScheduleLogicDetail.RunStartTime;
                    DataRowScheduledRules[NbtScheduledRuleColumns.RunEndTime.ToString()] = CswScheduleLogicDetail.RunEndTime;
                    DataRowScheduledRules[NbtScheduledRuleColumns.FailedCount.ToString()] = CswScheduleLogicDetail.FailedCount;
                    DataRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = CswConvert.ToDbVal( CswScheduleLogicDetail.Disabled );

                    CswTableUpdateScheduledRules.update( DataTableScheduledRules );

                    if( CswScheduleLogicDetail.RunParams.Count > 0 )
                    {

                        CswTableUpdate CswTableUpdateScheduledRuleParams = _CswResources.makeCswTableUpdate( _TableNameScheduledRuleParams + "_update", _TableNameScheduledRuleParams );

                        CswCommaDelimitedString CswCommaDelimitedStringSelectCols = new CswCommaDelimitedString();
                        CswCommaDelimitedStringSelectCols.Add( NbtScheduledRuleParamsColumns.ParamName.ToString() );
                        CswCommaDelimitedStringSelectCols.Add( NbtScheduledRuleParamsColumns.ParamVal.ToString() );


                        CswCommaDelimitedString CswCommaDelimitedStringParams = new CswCommaDelimitedString( CswScheduleLogicDetail.RunParams.Keys.Count, "'" );
                        foreach( string CurrentParamName in CswScheduleLogicDetail.RunParams.Keys )
                        {
                            CswCommaDelimitedStringParams.Add( CurrentParamName );
                        }//iterate params



                        string WhereClause = " where " + NbtScheduledRuleColumns.ScheduledRuleId.ToString() + " = (select " + NbtScheduledRuleColumns.ScheduledRuleId.ToString() + " from " + _TableNameScheduledRules + " where lower(" + NbtScheduledRuleColumns.RuleName.ToString() + ")='" + CswScheduleLogicDetail.RuleName.ToLower() + "') and lower(" + NbtScheduledRuleParamsColumns.ParamName + ") in (" + CswCommaDelimitedStringParams + ")";
                        DataTable DataTableScheduledRuleParams = CswTableUpdateScheduledRuleParams.getTable( CswCommaDelimitedStringSelectCols, "", Int32.MinValue, WhereClause, false );

                        foreach( DataRow CurrentRow in DataTableScheduledRuleParams.Rows )
                        {
                            CurrentRow[NbtScheduledRuleParamsColumns.ParamVal.ToString()] = CswScheduleLogicDetail.RunParams[CurrentRow[NbtScheduledRuleParamsColumns.ParamName.ToString()].ToString()];
                        }//iterate rolws

                        CswTableUpdateScheduledRuleParams.update( DataTableScheduledRuleParams );

                    }//if we have run params to update


                    _CswResources.commitTransaction();
                    _CswResources.clearUpdates();


                } //try 

                catch( Exception Exception )
                {
                    _CswResources.Rollback();
                    string Message = "Error writing rule meta data: " + Exception.Message;
                    _CswResources.CswLogger.reportError( new Exception( Message.ToString() ) );
                }

            }//if string has data

        }//write() 

    }//CswScheduleLogicDetailPersistenceCis

}//namespace ChemSW.Nbt.Sched



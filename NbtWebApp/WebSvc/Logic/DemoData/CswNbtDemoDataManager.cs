using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Grid.ExtJs;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.NbtSchedSvcRef;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Security;
using Newtonsoft.Json.Linq;
//using ChemSW.Nbt.NbtWebSvcSchedService;


namespace ChemSW.Nbt.WebServices
{
    public class CswNbtDemoDataManager
    {
        #region ctor

        private readonly CswNbtResources _CswNbtResources;

        public CswNbtDemoDataManager( CswNbtResources NbtManagerResources )
        {
            _CswNbtResources = NbtManagerResources;
        } //ctor

        #endregion ctor

        #region private

        /// TO DO: cover our privates

        #endregion private

        #region public



        public static void getDemoDataGrid( ICswResources CswResources, CswNbtDemoDataReturn Return, object Request )
        {

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            DataTable GridTable = new DataTable( "demodatatable" );

            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Name, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Type, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.IsUsedBy, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.IsRequiredBy, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Delete, typeof( char ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.ConvertToDemo, typeof( char ) );


            //Test data to populate grid
            DataRow NewRow = GridTable.NewRow();
            GridTable.Rows.Add( NewRow );
            NewRow[CswNbtDemoDataReturn.ColumnNames.Name] = "Container";
            NewRow[CswNbtDemoDataReturn.ColumnNames.Type] = "Node";
            NewRow[CswNbtDemoDataReturn.ColumnNames.IsUsedBy] = "5";
            NewRow[CswNbtDemoDataReturn.ColumnNames.IsRequiredBy] = "1";
            NewRow[CswNbtDemoDataReturn.ColumnNames.Delete] = CswConvert.ToDbVal( true );
            NewRow[CswNbtDemoDataReturn.ColumnNames.ConvertToDemo] = CswConvert.ToDbVal( false );


            CswNbtGrid Grid = new CswNbtGrid( CswNbtResources );
            Return.Data.Grid = Grid.DataTableToGrid( GridTable, IncludeEditFields: false );

            //if( null != LogicDetails && LogicDetails.Count > 0 && 
            //    null != Ret && 
            //    null != Ret.Data )
            //{
            //    DataTable GridTable = new DataTable( "scheduledrulestable" );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.RuleName, typeof(string) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Recurrance, typeof( string ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Interval, typeof( Int32 ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.ReprobateThreshold, typeof( Int32 ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.MaxRunTimeMs, typeof( Int32 ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Reprobate, typeof(bool) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.RunStartTime, typeof(DateTime) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.RunEndTime, typeof( DateTime ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.TotalRogueCount, typeof( Int32 ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.FailedCount, typeof( Int32 ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.ThreadId, typeof( Int32 ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.StatusMessage, typeof( string ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Disabled, typeof( bool ) );
            //    GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.HasChanged, typeof( bool ) );

            //    foreach( CswScheduleLogicDetail LogicDetail in LogicDetails )
            //    {
            //        if( null != LogicDetail )
            //        {
            //            DataRow Row = GridTable.NewRow();
            //            Row[CswScheduleLogicDetail.ColumnNames.RuleName] = LogicDetail.RuleName;
            //            Row[CswScheduleLogicDetail.ColumnNames.Recurrance] = LogicDetail.Recurrence;
            //            Row[CswScheduleLogicDetail.ColumnNames.Interval] = LogicDetail.Interval;
            //            Row[CswScheduleLogicDetail.ColumnNames.ReprobateThreshold] = LogicDetail.ReprobateThreshold;
            //            Row[CswScheduleLogicDetail.ColumnNames.MaxRunTimeMs] = LogicDetail.MaxRunTimeMs;
            //            Row[CswScheduleLogicDetail.ColumnNames.Reprobate] = LogicDetail.Reprobate;
            //            Row[CswScheduleLogicDetail.ColumnNames.RunStartTime] = LogicDetail.RunStartTime;
            //            Row[CswScheduleLogicDetail.ColumnNames.RunEndTime] = LogicDetail.RunEndTime;
            //            Row[CswScheduleLogicDetail.ColumnNames.TotalRogueCount] = LogicDetail.TotalRogueCount;
            //            Row[CswScheduleLogicDetail.ColumnNames.FailedCount] = LogicDetail.FailedCount;
            //            Row[CswScheduleLogicDetail.ColumnNames.ThreadId] = LogicDetail.ThreadId;
            //            Row[CswScheduleLogicDetail.ColumnNames.StatusMessage] = LogicDetail.StatusMessage;
            //            Row[CswScheduleLogicDetail.ColumnNames.Disabled] = LogicDetail.Disabled;
            //            Row[CswScheduleLogicDetail.ColumnNames.HasChanged] = false;

            //            GridTable.Rows.Add(Row);
            //        }
            //    }
            //    CswNbtGrid gd = new CswNbtGrid( NbtResources );
            //    Ret.Data.Grid = gd.DataTableToGrid( GridTable );
            //}
        }//getDemoDataGrid()

        /*
        public static void getScheduledRulesGrid( ICswResources CswResources, CswNbtScheduledRulesReturn Return, string AccessId )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswSchedSvcReturn svcReturn = new CswSchedSvcReturn();
            //TODO: switch Resources to alternate AccessId, if different than our current AccessId
            // GOTO CswSchedSvcAdminEndPoint for actual implementation

            
            //Here are using the web reference for the schedule service. The 
            //Overwrite the app.config endpoint uri with the one defined in SetupVbls
            //The CswSchedSvcAdminEndPointClient::getRules() method will return a collection 
            //of objects each which represents a scheduled rule, for the accessid specified
            //as an input parameter on CswSchedSvcParams. You can find the server side of this connection in 
            //CswCommon/Csw/MtSched/port
            CswSchedSvcAdminEndPointClient SchedSvcRef = new CswSchedSvcAdminEndPointClient();
            EndpointAddress URI = new EndpointAddress( CswResources.SetupVbls["SchedServiceUri"] );
            SchedSvcRef.Endpoint.Address = URI;
            CswSchedSvcParams CswSchedSvcParams = new CswSchedSvcParams();
            CswSchedSvcParams.CustomerId = AccessId;
            svcReturn = SchedSvcRef.getRules( CswSchedSvcParams ); 


            if( null != svcReturn )
            {
                _addScheduledRulesGrid( NbtResources, svcReturn.Data, Return );
            }
            Return.Data.CustomerId = AccessId;
        }//getScheduledRulesGrid()


        public bool updateScheduledRule( HttpContext Context )
        {
            bool RetSuccess = false;

            Int32 ScheduledRuleId = CswConvert.ToInt32( Context.Request["id"] );
            Int32 FailedCount = CswConvert.ToInt32( Context.Request["FAILEDCOUNT"] );
            bool Reprobate = CswConvert.ToBoolean( Context.Request["REPROBATE"] );
            bool Disabled = CswConvert.ToBoolean( Context.Request["DISABLED"] );

            Recurrence Recurrence = CswConvert.ToString( Context.Request["RECURRENCE"] );

            Int32 Interval = CswConvert.ToInt32( Context.Request["INTERVAL"] );
            Int32 ReprobateThreshold = CswConvert.ToInt32( Context.Request["REPROBATETHRESHOLD"] );
            Int32 MaxRunTimeMs = CswConvert.ToInt32( Context.Request["MAXRUNTIMEMS"] );

            CswTableUpdate RulesUpdate = _CswNbtResources.makeCswTableUpdate( "Scheduledrules_update_on_accessid_" + _CswNbtResources.AccessId + "_id_" + ScheduledRuleId, "scheduledrules" );
            DataTable RulesTable = RulesUpdate.getTable( "scheduledruleid", ScheduledRuleId, true );
            if( RulesTable.Rows.Count == 1 )
            {
                DataRow ThisRule = RulesTable.Rows[0];
                if( FailedCount == 0 || false == Reprobate )
                {
                    ThisRule["FAILEDCOUNT"] = CswConvert.ToDbVal( 0 );
                }
                else if( 0 <= FailedCount )
                {
                    ThisRule["FAILEDCOUNT"] = CswConvert.ToDbVal( FailedCount );
                }

                ThisRule["REPROBATE"] = CswConvert.ToDbVal( Reprobate );
                ThisRule["DISABLED"] = CswConvert.ToDbVal( Disabled );

                if( null != Recurrence &&
                    Recurrence != CswNbtResources.UnknownEnum )
                {
                    ThisRule["RECURRENCE"] = CswConvert.ToDbVal( Recurrence.ToString() );
                }
                if( 0 < Interval )
                {
                    ThisRule["INTERVAL"] = CswConvert.ToDbVal( Interval );
                }
                if( 0 < ReprobateThreshold )
                {
                    ThisRule["REPROBATETHRESHOLD"] = CswConvert.ToDbVal( ReprobateThreshold );
                }
                if( 5000 < MaxRunTimeMs )
                {
                    ThisRule["MAXRUNTIMEMS"] = CswConvert.ToDbVal( MaxRunTimeMs );
                }
                RetSuccess = RulesUpdate.update( RulesTable );
            }

            if( false == RetSuccess )
            {
                throw new CswDniException( ErrorType.Error, "Attempt to update the Scheduled Rules table failed.", "Could not update scheduledruleid=" + ScheduledRuleId + " on Customer ID " + _CswNbtResources.AccessId + "." );
            }
            _finalize( _CswNbtResources );
            return RetSuccess;
        }

        private enum ScheduledRuleActions
        {
            Unknown,
            ClearAllReprobates
        }

        public static void updateAllScheduledRules( ICswResources CswResources, CswNbtScheduledRulesReturn Return, CswNbtScheduledRulesReturn.Ret Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswSchedSvcReturn svcReturn = new CswSchedSvcReturn();
            CswSchedSvcAdminEndPointClient SchedSvcRef = new CswSchedSvcAdminEndPointClient();
            //Overwrite the app.config endpoint uri with the one defined in SetupVbls
            EndpointAddress URI = new EndpointAddress( CswResources.SetupVbls["SchedServiceUri"] );
            SchedSvcRef.Endpoint.Address = URI;

            CswSchedSvcParams CswSchedSvcParams = new CswSchedSvcParams();
            CswSchedSvcParams.CustomerId = Request.CustomerId;
            CswSchedSvcParams.LogicDetails = new Collection<CswScheduleLogicDetail>();
            String GridPrefix = "ScheduledRules";
            foreach( CswExtJsGridRow GridRow in Request.Grid.rowData.rows )
            {
                if( CswConvert.ToBoolean( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.HasChanged )] ) )
                {
                    DateTime StartTime = String.IsNullOrEmpty(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.RunStartTime)])
                                             ? DateTime.MinValue
                                             : DateTime.Parse(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.RunStartTime)]);
                    DateTime EndTime = String.IsNullOrEmpty(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.RunEndTime)])
                                           ? DateTime.MinValue
                                           : DateTime.Parse(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.RunEndTime)]);

                    CswScheduleLogicDetail Rule = new CswScheduleLogicDetail
                    {
                        RuleName = GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.RuleName)],
                        Recurrence = GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.Recurrance)],
                        Interval = CswConvert.ToInt32(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.Interval)]),
                        ReprobateThreshold = CswConvert.ToInt32(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.ReprobateThreshold)]),
                        MaxRunTimeMs = CswConvert.ToInt32(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.MaxRunTimeMs)]),
                        Reprobate = CswConvert.ToBoolean(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.Reprobate)]),
                        RunStartTime = StartTime,
                        RunEndTime = EndTime,
                        TotalRogueCount = CswConvert.ToInt32(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.TotalRogueCount)]),
                        FailedCount = CswConvert.ToInt32(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.FailedCount)]),
                        ThreadId = CswConvert.ToInt32(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.ThreadId)]),
                        StatusMessage = GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.StatusMessage)],
                        Disabled = CswConvert.ToBoolean(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.Disabled)])
                    };
                    CswSchedSvcParams.LogicDetails.Add(Rule);
                }
            }

            svcReturn = SchedSvcRef.updateScheduledRules( CswSchedSvcParams );
            if( null != svcReturn )
            {
                _addScheduledRulesGrid( NbtResources, svcReturn.Data, Return );
            }
        }
        */

        #endregion public



    } // class CswNbtDemoDataManager

} // namespace ChemSW.Nbt.WebServices

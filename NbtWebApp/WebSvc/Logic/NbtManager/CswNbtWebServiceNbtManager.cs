using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using ChemSW.Core;
using ChemSW.Core.Colors;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Grid.ExtJs;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.NbtSchedSvcRef;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Security;
using NbtWebApp.WebSvc.Logic.Scheduler;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNbtManager
    {
        #region ctor

        private readonly CswNbtResources _OtherResources;
        private readonly CswNbtResources _NbtManagerResources = null;
        private bool _AllowAllAccessIds = false;
        private CswEnumNbtActionName _Action = CswEnumNbtActionName.Unknown;

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources, string AccessId, CswEnumNbtActionName ActionName, bool AllowAnyAdmin = false )
        {
            _NbtManagerResources = NbtManagerResources;
            _Action = ActionName;
            _checkNbtManagerPermission( AllowAnyAdmin );
            _OtherResources = makeOtherResources( AccessId );
        } //ctor

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources, CswEnumNbtActionName ActionName, bool AllowAnyAdmin = false )
        {
            _NbtManagerResources = NbtManagerResources;
            _Action = ActionName;
            _checkNbtManagerPermission( AllowAnyAdmin );
        } //ctor
        #endregion ctor

        #region private

        private void _checkNbtManagerPermission( bool AllowAnyAdmin )
        {
            if( _NbtManagerResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.NBTManager ) &&
                ( _NbtManagerResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername ||
                _NbtManagerResources.CurrentNbtUser.IsAdministrator() ) )
            {
                _AllowAllAccessIds = true;
            }
        }

        private void _ValidateAccessId( string AccessId )
        {
            if( string.IsNullOrEmpty( AccessId ) ||
                false == _NbtManagerResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "The supplied Customer ID " + AccessId + " does not exist or is not enabled.", "No configuration could be loaded for AccessId " + AccessId + "." );
            }
        }

        private void _finalize( CswNbtResources OtherResources )
        {
            OtherResources.finalize();
            OtherResources.release();
        }

        #endregion private

        #region public

        #region Grid

        public CswNbtResources makeOtherResources( string AccessId )
        {

            _ValidateAccessId( AccessId );
            CswNbtResources OtherResources = CswNbtResourcesFactory.makeCswNbtResources( _NbtManagerResources );
            OtherResources.AccessId = AccessId;
            OtherResources.InitCurrentUser = InitUser;
            return OtherResources;
        }
        private ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswEnumSystemUserNames.SysUsr_NbtWebSvcMgr );
        }

        public JObject getActiveAccessIds()
        {
            JObject RetObj = new JObject();
            JArray CustomerIds = new JArray();
            RetObj["customerids"] = CustomerIds;

            if( _AllowAllAccessIds )
            {
                foreach( string AccessId in from string _AccessId in _NbtManagerResources.CswDbCfgInfo.AccessIds
                                            orderby _AccessId
                                            select _AccessId )
                {
                    if( _NbtManagerResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
                    {
                        CustomerIds.Add( AccessId );
                    }
                }
            }
            else
            {
                CustomerIds.Add( _NbtManagerResources.AccessId );
            }
            return RetObj;
        }

        private static void _addScheduledRulesGrid( CswNbtResources NbtResources, Collection<CswScheduleLogicDetail> LogicDetails, CswNbtScheduledRulesReturn Ret )
        {
            if( null != LogicDetails && LogicDetails.Count > 0 &&
                null != Ret &&
                null != Ret.Data )
            {
                DataTable GridTable = new DataTable( "scheduledrulestable" );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.RuleName, typeof( string ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.Recurrance, typeof( string ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.Interval, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.ReprobateThreshold, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.MaxRunTimeMs, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.Reprobate, typeof( bool ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.RunStartTime, typeof( DateTime ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.RunEndTime, typeof( DateTime ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.TotalRogueCount, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.FailedCount, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.ThreadId, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.StatusMessage, typeof( string ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.Disabled, typeof( bool ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.HasChanged, typeof( bool ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.Priority, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.LoadCount, typeof( Int32 ) );
                GridTable.Columns.Add( CswEnumScheduleLogicDetailColumnNames.LastLoadCheck, typeof( DateTime ) );

                foreach( CswScheduleLogicDetail LogicDetail in LogicDetails )
                {
                    if( null != LogicDetail )
                    {
                        DataRow Row = GridTable.NewRow();
                        Row[CswEnumScheduleLogicDetailColumnNames.RuleName] = LogicDetail.RuleName;
                        Row[CswEnumScheduleLogicDetailColumnNames.Recurrance] = LogicDetail.Recurrence;
                        Row[CswEnumScheduleLogicDetailColumnNames.Interval] = LogicDetail.Interval;
                        Row[CswEnumScheduleLogicDetailColumnNames.ReprobateThreshold] = LogicDetail.ReprobateThreshold;
                        Row[CswEnumScheduleLogicDetailColumnNames.MaxRunTimeMs] = LogicDetail.MaxRunTimeMs;
                        Row[CswEnumScheduleLogicDetailColumnNames.Reprobate] = LogicDetail.Reprobate;
                        Row[CswEnumScheduleLogicDetailColumnNames.RunStartTime] = LogicDetail.RunStartTime;
                        Row[CswEnumScheduleLogicDetailColumnNames.RunEndTime] = LogicDetail.RunEndTime;
                        Row[CswEnumScheduleLogicDetailColumnNames.TotalRogueCount] = LogicDetail.TotalRogueCount;
                        Row[CswEnumScheduleLogicDetailColumnNames.FailedCount] = LogicDetail.FailedCount;
                        Row[CswEnumScheduleLogicDetailColumnNames.ThreadId] = LogicDetail.ThreadId;
                        Row[CswEnumScheduleLogicDetailColumnNames.StatusMessage] = LogicDetail.StatusMessage;
                        Row[CswEnumScheduleLogicDetailColumnNames.Priority] = LogicDetail.Priority;
                        Row[CswEnumScheduleLogicDetailColumnNames.LoadCount] = LogicDetail.LoadCount;
                        Row[CswEnumScheduleLogicDetailColumnNames.LastLoadCheck] = LogicDetail.LastLoadCheck;
                        Row[CswEnumScheduleLogicDetailColumnNames.Disabled] = LogicDetail.Disabled;
                        Row[CswEnumScheduleLogicDetailColumnNames.HasChanged] = false;

                        GridTable.Rows.Add( Row );
                    }
                }
                CswNbtGrid gd = new CswNbtGrid( NbtResources );
                Ret.Data.Grid = gd.DataTableToGrid( GridTable );
            }
        }

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
            foreach( CswExtJsGridRow GridRow in Request.Grid.rowData.rows )
            {
                if( CswConvert.ToBoolean( GridRow.data[new CswExtJsGridDataIndex( "ScheduledRules", CswEnumScheduleLogicDetailColumnNames.HasChanged )] ) )
                {
                    CswScheduleLogicDetail Rule = _getLogicDetailFromGridRow( GridRow, "ScheduledRules" );
                    CswSchedSvcParams.LogicDetails.Add( Rule );
                }
            }

            svcReturn = SchedSvcRef.updateScheduledRules( CswSchedSvcParams );
            if( null != svcReturn )
            {
                _updateScheduledRulesTable( NbtResources, CswSchedSvcParams.LogicDetails );
                _addScheduledRulesGrid( NbtResources, svcReturn.Data, Return );
            }
        }

        public CswNbtObjClassUser getCswAdmin( string AccessId )
        {
            CswNbtResources OtherResources = makeOtherResources( AccessId );
            CswNbtObjClassUser ChemSWAdminUserNode = OtherResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
            return ChemSWAdminUserNode;
        }

        public string getCustomerAccessId( string LoginButtonPropId )
        {
            if( string.IsNullOrEmpty( LoginButtonPropId ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Authentication in this context is not possible.", "Authentication in this context is not possible." );
            }
            CswPropIdAttr PropAttr = new CswPropIdAttr( LoginButtonPropId );

            if( null == PropAttr ||
                null == PropAttr.NodeId ||
                Int32.MinValue == PropAttr.NodeId.PrimaryKey )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Authentication in this context is not possible.", "Authentication in this context is not possible." );
            }
            CswNbtObjClassCustomer CustomerNode = _NbtManagerResources.Nodes.GetNode( PropAttr.NodeId );
            return CustomerNode.CompanyID.Text;
        }

        #endregion

        #region Timeline

        private static bool _inUse( string filePath )
        {
            bool ret = false;
            StreamReader file = null;
            try
            {
                file = new StreamReader( filePath );
            }
            catch
            {
                ret = true;
            }
            finally
            {
                if( null != file )
                {
                    file.Close();
                }
            }
            return ret;
        }

        public static void getTimelineFilters( ICswResources CswResources, CswNbtSchedServiceTimeLineReturn Return, string FileName )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            HashSet<string> Seen = new HashSet<string>();

            string LogFileLocation = NbtResources.SetupVbls[CswEnumSetupVariableNames.LogFileLocation];
            _getLogFiles( NbtResources, Return, LogFileLocation ); //Order the log files by last modified date

            string selectedFile = "";
            if( Return.Data.FilterData.LogFiles.Count > 0 )
            {
                selectedFile = Return.Data.FilterData.LogFiles[0];
            }
            foreach( string log in Return.Data.FilterData.LogFiles )
            {
                if( log.Equals( FileName ) )
                {
                    selectedFile = log;
                }
            }

            if( false == string.IsNullOrEmpty( selectedFile ) )
            {
                string filePath = LogFileLocation + @"\" + selectedFile;
                StreamReader file = new StreamReader( filePath );
                string line;
                while( ( line = file.ReadLine() ) != null )
                {
                    line = line.Replace( "\"", "" );
                    string[] splitLine = line.Split( ',' );
                    string MsgType = splitLine[0];

                    if( ( MsgType.Equals( "PerOp" ) || MsgType.Equals( "Error" ) ) ) //We only care about "Error" or "PerOp" rows
                    {
                        string Schema = splitLine[1];
                        string OpName = splitLine[23].Split( ':' )[0]; //this is something like "GenNode: Execution" and all we want is "GenNode"
                        OpName = MsgType.Equals( "Error" ) ? "Error" : OpName; //If we have an "error" row, the Op gets renamed to "Error"

                        _populateFilterData( Return, OpName, Schema, Seen );

                        DateTime ThirtyMinAgo = DateTime.Now.AddMinutes( -30 ); //30 min ago
                        Return.Data.FilterData.DefaultStartTime = ThirtyMinAgo.ToString( "hh:mm:ss tt" );
                        Return.Data.FilterData.DefaultEndTime = DateTime.Now.ToString( "hh:mm:ss tt" );

                        Return.Data.FilterData.DefaultStartDay = DateTime.Now.ToString( "M/d/yyyy" );
                        Return.Data.FilterData.DefaultEndDay = DateTime.Now.ToString( "M/d/yyyy" );
                    }
                }
            }
        }

        public static void getTimelines( ICswResources CswResources, CswNbtSchedServiceTimeLineReturn Return, CswNbtSchedServiceTimeLineRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            DateTime StartDate = new DateTime();
            int counter = 0;

            CswColorGenerator ColorGenerator = new CswColorGenerator();

            int SeriesNo = 30;

            Dictionary<string, Series> TimeLineData = new Dictionary<string, Series>();
            HashSet<string> seen = new HashSet<string>();

            string LogFileLocation = NbtResources.SetupVbls[CswEnumSetupVariableNames.LogFileLocation];
            _getLogFiles( NbtResources, Return, LogFileLocation ); //Order the log files by last modified date

            if( Return.Data.FilterData.LogFiles.Count > 0 )
            {
                //If no log file is selected, default to the last log file modified
                string selectedFile = String.IsNullOrEmpty( Request.SelectedLogFile ) ? Return.Data.FilterData.LogFiles[0] : Request.SelectedLogFile;
                StreamReader file = new StreamReader( LogFileLocation + @"\" + selectedFile );
                string line;
                while( ( line = file.ReadLine() ) != null )
                {
                    line = line.Replace( "\"", "" );
                    string[] splitLine = line.Split( ',' );
                    string MsgType = splitLine[0];

                    if( ( MsgType.Equals( "PerOp" ) || MsgType.Equals( "Error" ) ) ) //if the row is not "PerOp" it is useless to us
                    {
                        string Schema = splitLine[1];
                        string StartTime = splitLine[20];
                        string OpName = splitLine[23].Split( ':' )[0]; //this is something like "GenNode: Execution" and all we want is "GenNode"
                        double ExecutionTime = CswConvert.ToDouble( splitLine[28] );
                        string ErrMsg = splitLine[9];

                        if( MsgType.Equals( "Error" ) )
                        {
                            OpName = "Error";
                            ExecutionTime = double.MinValue;
                        }
                        string LegendName = Schema + " " + OpName;

                        _populateFilterData( Return, OpName, Schema, seen );

                        DateTime thisStartDate = CswConvert.ToDateTime( StartTime );

                        DateTime FilterDateStart = CswConvert.ToDateTime( Request.FilterStartTimeTo );
                        DateTime FilterDateEnd = CswConvert.ToDateTime( Request.FilterEndTimeTo );
                        CswCommaDelimitedString FilterSchemas = new CswCommaDelimitedString();
                        FilterSchemas.FromString( Request.FilterSchemaTo );
                        CswCommaDelimitedString FilterOps = new CswCommaDelimitedString();
                        FilterOps.FromString( Request.FilterOpTo );

                        if( ( ( thisStartDate >= FilterDateStart && thisStartDate <= FilterDateEnd ) || ( DateTime.MinValue == FilterDateStart && DateTime.MinValue == FilterDateEnd ) ) &&
                            ( FilterSchemas.Contains( Schema ) || String.IsNullOrEmpty( Request.FilterSchemaTo ) ) &&
                            ( FilterOps.Contains( OpName ) || String.IsNullOrEmpty( Request.FilterOpTo ) ) )
                        {
                            if( FilterSchemas.IsEmpty || FilterOps.IsEmpty ) //If no schema filter is set we want to generate a timeline of each schema + all the rules that ran
                            {
                                if( FilterOps.IsEmpty )
                                {
                                    if( MsgType.Equals( "Error" ) )
                                    {
                                        //If we're mashing all schema errors together, we do not show what the msg or schema was was since there are probably many, just that an error occured. 
                                        LegendName = "Error";
                                        OpName = "";
                                        ErrMsg = "";
                                        Schema = "";
                                    }
                                    else
                                    {
                                        //If we're mashing all schema together, we don't care what op ran since there are many, just show that this schema was running something
                                        LegendName = Schema;
                                        OpName = "";
                                    }
                                }
                                else
                                {
                                    //If no schema is selected, but there are Op filters, we mash the Ops on each Schema together
                                    LegendName = OpName;
                                    Schema = "";
                                }
                            }

                            if( 0 == counter )
                            {
                                StartDate = CswConvert.ToDateTime( StartTime );
                            }
                            counter++;

                            double DataStartS = ( thisStartDate - StartDate ).TotalMilliseconds / 1000;
                            double DataEndS = double.MinValue;
                            if( MsgType.Equals( "PerOp" ) )
                            {
                                DataEndS = DataStartS + ExecutionTime / 1000;
                            }

                            Series ThisSeries;
                            if( TimeLineData.ContainsKey( LegendName ) )
                            {
                                ThisSeries = TimeLineData[LegendName];
                            }
                            else
                            {
                                ThisSeries = new Series()
                                    {
                                        label = LegendName,
                                        SchemaName = Schema,
                                        OpName = OpName,
                                        SeriesNo = SeriesNo,
                                        ErrorMsg = ErrMsg
                                    };
                                TimeLineData.Add( LegendName, ThisSeries );
                                SeriesNo += 90;
                            }
                            _processData( ThisSeries, DataStartS, DataEndS, ExecutionTime, thisStartDate.ToString(), ColorGenerator );
                        }
                    } //if( splitLine.Length >= 28 && splitLine[0].Equals( "PerOp" ) )
                } // while( ( line = file.ReadLine() ) != null && counter <= maxLines )

                foreach( Series series in TimeLineData.Values )
                {
                    Return.Data.Series.Add( series );
                }
            }
        }//getTimelines()

        private static void _getLogFiles( CswNbtResources NbtResources, CswNbtSchedServiceTimeLineReturn Return, string LogFileLocation )
        {
            List<string> logFiles = new List<string>( Directory.GetFiles( LogFileLocation ) );

            //Add files to collection by most recent
            while( logFiles.Count > 0 )
            {
                DateTime newestLogFileDate = DateTime.MinValue;
                string newestLogFile = string.Empty;
                foreach( string fileName in logFiles )
                {
                    DateTime lastModified = File.GetLastWriteTime( fileName );
                    if( lastModified >= newestLogFileDate )
                    {
                        newestLogFile = fileName;
                        newestLogFileDate = lastModified;
                    }
                }
                logFiles.Remove( newestLogFile );
                string newestFileNameWithoutPath = Path.GetFileName( newestLogFile );
                if( false == _inUse( newestLogFile ) )
                {
                    Return.Data.FilterData.LogFiles.Add( newestFileNameWithoutPath );
                }
            }
        }

        private static void _processData( Series ThisSeries, double DataStartS, double DataEndS, double ExecutionTime, string StartTime, CswColorGenerator ColorGenerator )
        {
            if( ThisSeries.data.Count > 0 && DataStartS - ThisSeries.data.Last()[0] <= 3 ) //if pts are only up to 3 seconds apart, combine them
            {
                ThisSeries.data.Last()[0] = DataEndS;
            }
            else
            {
                if( String.IsNullOrEmpty( ThisSeries.color ) )
                {
                    ThisSeries.color = ColorGenerator.GetNextColor();
                }
                DataPoint point = new DataPoint()
                {
                    Start = DataStartS,
                    End = ThisSeries.SeriesNo,
                    ExecutionTime = ExecutionTime,
                    StartDate = StartTime,
                };
                DataPoint point2 = new DataPoint()
                    {
                        Start = DataEndS,
                        End = ThisSeries.SeriesNo,
                        ExecutionTime = ExecutionTime,
                        StartDate = StartTime,
                    };
                ThisSeries.DataPoints.Add( point );
                ThisSeries.DataPoints.Add( point2 );
                ThisSeries.DataPoints.Add( null );

                if( ThisSeries.data.Count > 0 && null != ThisSeries.data.Last() )
                {
                    ThisSeries.data.Add( null );
                    if( ThisSeries.label.Contains( "Error" ) )
                    {
                        ThisSeries.data.Add( null );
                        ThisSeries.color = "#ff0000";
                    }
                }

                Collection<double> thisStartPt = new Collection<double>();
                thisStartPt.Add( DataStartS );
                thisStartPt.Add( ThisSeries.SeriesNo );
                ThisSeries.data.Add( thisStartPt );

                if( double.MinValue < DataEndS )
                {
                    Collection<double> thisEndPt = new Collection<double>();
                    thisEndPt.Add( DataEndS );
                    thisEndPt.Add( ThisSeries.SeriesNo );
                    ThisSeries.data.Add( thisEndPt );
                }
            }
        }

        /// <summary>
        /// Populate the Schema and Operation filters
        /// </summary>
        private static void _populateFilterData( CswNbtSchedServiceTimeLineReturn Return, String OpName, String Schema, HashSet<string> seen )
        {
            FilterData.FilterOption opOpt = new FilterData.FilterOption()
            {
                text = OpName,
                value = OpName
            };
            if( false == seen.Contains( OpName ) )
            {
                Return.Data.FilterData.Operations.Add( opOpt );
                seen.Add( OpName );
            }

            FilterData.FilterOption schemaOpt = new FilterData.FilterOption()
            {
                text = Schema,
                value = Schema
            };
            if( false == seen.Contains( Schema ) )
            {
                Return.Data.FilterData.Schema.Add( schemaOpt );
                seen.Add( Schema );
            }
        }

        #endregion

        #region Monitor

        public static void getScheduledRuleStatus( ICswResources _CswResources, CswNbtScheduledRuleStatusReturn Return, object Request )
        {
            CswCommaDelimitedString RuleStatus = new CswCommaDelimitedString();
            CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( (CswNbtResources) _CswResources, CswEnumNbtActionName.View_Scheduled_Rules );
            JObject AccessIds = ws.getActiveAccessIds();
            foreach( String AccessId in AccessIds["customerids"] )
            {
                CswNbtScheduledRulesReturn Rules = new CswNbtScheduledRulesReturn();
                getScheduledRulesGrid( _CswResources, Rules, AccessId );
                foreach( CswExtJsGridRow GridRow in Rules.Data.Grid.rowData.rows )
                {
                    CswScheduleLogicDetail Rule = _getLogicDetailFromGridRow( GridRow, "ScheduledRules" );
                    if( false == Rule.Disabled )
                    {
                        if( Rule.Reprobate )
                        {
                            DateTime TimeOfReprobate = Rule.LastRun > Rule.LastLoadCheck ? Rule.LastRun : Rule.LastLoadCheck;
                            RuleStatus.Add( "REPROBATE on " + AccessId + ": " + Rule.RuleName + " as of " + TimeOfReprobate );
                        }
                        else
                        {
                            TimeSpan LastCheckInterval = DateTime.Now - Rule.LastLoadCheck;
                            if( LastCheckInterval > Rule.getRunTimeInterval().Add( new TimeSpan( 1, 0, 0 ) ) )//1 hour buffer
                            {
                                RuleStatus.Add( "ERROR on " + AccessId + ": " + Rule.RuleName + " last checked load on " + Rule.LastLoadCheck );
                            }
                        }
                    }
                }
            }
            if( RuleStatus.Count == 0 )
            {
                RuleStatus.Add( "ALL SCHEDULED SERVICE RULES OK" );
            }
            Return.Data.RuleStatus = RuleStatus.ToString();
        }

        #endregion Monitor

        #endregion public

        #region private

        private static void _updateScheduledRulesTable( CswNbtResources NbtResources, IEnumerable<CswScheduleLogicDetail> ScheduledRules )
        {
            foreach( CswScheduleLogicDetail ScheduledRule in ScheduledRules )
            {
                CswTableUpdate RulesUpdate = NbtResources.makeCswTableUpdate( "Scheduledrules_update_on_accessid_" + NbtResources.AccessId + "_for_" + ScheduledRule.RuleName, "scheduledrules" );
                DataTable RulesTable = RulesUpdate.getTable( "where rulename = '" + ScheduledRule.RuleName + "'" );
                if( RulesTable.Rows.Count == 1 )
                {
                    DataRow ThisRule = RulesTable.Rows[0];
                    ThisRule["recurrence"] = ScheduledRule.Recurrence;
                    ThisRule["interval"] = ScheduledRule.Interval;
                    ThisRule["reprobatethreshold"] = ScheduledRule.ReprobateThreshold;
                    ThisRule["reprobate"] = CswConvert.ToDbVal( ScheduledRule.Reprobate );
                    ThisRule["maxruntimems"] = ScheduledRule.MaxRunTimeMs;
                    ThisRule["totalroguecount"] = ScheduledRule.TotalRogueCount;
                    ThisRule["failedcount"] = ScheduledRule.FailedCount;
                    ThisRule["statusmessage"] = ScheduledRule.StatusMessage;
                    ThisRule["priority"] = ScheduledRule.Priority;
                    ThisRule["disabled"] = CswConvert.ToDbVal( ScheduledRule.Disabled );
                    ThisRule["runstarttime"] = ScheduledRule.RunStartTime == DateTime.MinValue ? (object) DBNull.Value : ScheduledRule.RunStartTime;
                    ThisRule["runendtime"] = ScheduledRule.RunEndTime == DateTime.MinValue ? (object) DBNull.Value : ScheduledRule.RunEndTime;
                    ThisRule["lastrun"] = ScheduledRule.LastRun == DateTime.MinValue ? (object) DBNull.Value : ScheduledRule.LastRun;
                    ThisRule["nextrun"] = ScheduledRule.NextRun == DateTime.MinValue ? (object) DBNull.Value : ScheduledRule.NextRun;
                    ThisRule["threadid"] = ScheduledRule.ThreadId;
                    ThisRule["loadcount"] = ScheduledRule.LoadCount;
                    ThisRule["lastloadcheck"] = ScheduledRule.LastLoadCheck == DateTime.MinValue ? (object) DBNull.Value : ScheduledRule.LastLoadCheck;
                    RulesUpdate.update( RulesTable );
                }
                else
                {
                    NbtResources.CswLogger.reportAppState( "Scheduled Rule " + ScheduledRule.RuleName + " does not exist in the database." );
                }
            }
        }

        private static CswScheduleLogicDetail _getLogicDetailFromGridRow( CswExtJsGridRow GridRow, String GridPrefix )
        {
            DateTime StartTime = String.IsNullOrEmpty( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RunStartTime )] )
                                             ? DateTime.MinValue
                                             : DateTime.Parse( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RunStartTime )] );
            DateTime EndTime = String.IsNullOrEmpty( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RunEndTime )] )
                                   ? DateTime.MinValue
                                   : DateTime.Parse( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RunEndTime )] );

            DateTime LoadCheckTime = String.IsNullOrEmpty( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.LastLoadCheck )] )
                                   ? DateTime.MinValue
                                   : DateTime.Parse( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.LastLoadCheck )] );

            CswScheduleLogicDetail Rule = new CswScheduleLogicDetail
            {
                RuleName = GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.RuleName )],
                Recurrence = GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Recurrance )],
                Interval = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Interval )] ),
                ReprobateThreshold = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.ReprobateThreshold )] ),
                MaxRunTimeMs = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.MaxRunTimeMs )] ),
                Reprobate = CswConvert.ToBoolean( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Reprobate )] ),
                RunStartTime = StartTime,
                RunEndTime = EndTime,
                TotalRogueCount = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.TotalRogueCount )] ),
                FailedCount = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.FailedCount )] ),
                ThreadId = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.ThreadId )] ),
                StatusMessage = GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.StatusMessage )],
                Priority = Convert.ToUInt16( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Priority )] ),
                LoadCount = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.LoadCount )] ),
                LastLoadCheck = LoadCheckTime,
                Disabled = CswConvert.ToBoolean( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswEnumScheduleLogicDetailColumnNames.Disabled )] )
            };

            return Rule;
        }

        #endregion private

    } // class CswNbtWebServiceNbtManager

} // namespace ChemSW.Nbt.WebServices

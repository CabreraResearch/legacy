using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Grid.ExtJs;
using ChemSW.Log;
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

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNbtManager
    {
        #region ctor

        private readonly CswNbtResources _OtherResources;
        private readonly CswNbtResources _NbtManagerResources = null;
        private bool _AllowAllAccessIds = false;
        private CswNbtActionName _Action = CswNbtActionName.Unknown;

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources, string AccessId, CswNbtActionName ActionName, bool AllowAnyAdmin = false )
        {
            _NbtManagerResources = NbtManagerResources;
            _Action = ActionName;
            _checkNbtManagerPermission( AllowAnyAdmin );
            _OtherResources = makeOtherResources( AccessId );
        } //ctor

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources, CswNbtActionName ActionName, bool AllowAnyAdmin = false )
        {
            _NbtManagerResources = NbtManagerResources;
            _Action = ActionName;
            _checkNbtManagerPermission( AllowAnyAdmin );
        } //ctor
        #endregion ctor

        #region private

        private void _checkNbtManagerPermission( bool AllowAnyAdmin )
        {
            if( _NbtManagerResources.Modules.IsModuleEnabled( CswNbtModuleName.NBTManager ) &&
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
                throw new CswDniException( ErrorType.Error, "The supplied Customer ID " + AccessId + " does not exist or is not enabled.", "No configuration could be loaded for AccessId " + AccessId + "." );
            }
        }

        private void _finalize( CswNbtResources OtherResources )
        {
            OtherResources.finalize();
            OtherResources.release();
        }

        #endregion private

        #region public

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
            return new CswNbtSystemUser( Resources, CswSystemUserNames.SysUsr_NbtWebSvcMgr );
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
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.RuleName, typeof(string) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Recurrance, typeof( string ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Interval, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.ReprobateThreshold, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.MaxRunTimeMs, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Reprobate, typeof(bool) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.RunStartTime, typeof(DateTime) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.RunEndTime, typeof( DateTime ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.TotalRogueCount, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.FailedCount, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.ThreadId, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.StatusMessage, typeof( string ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Priority, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.LoadCount, typeof( Int32 ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.Disabled, typeof( bool ) );
                GridTable.Columns.Add( CswScheduleLogicDetail.ColumnNames.HasChanged, typeof( bool ) );

                foreach( CswScheduleLogicDetail LogicDetail in LogicDetails )
                {
                    if( null != LogicDetail )
                    {
                        DataRow Row = GridTable.NewRow();
                        Row[CswScheduleLogicDetail.ColumnNames.RuleName] = LogicDetail.RuleName;
                        Row[CswScheduleLogicDetail.ColumnNames.Recurrance] = LogicDetail.Recurrence;
                        Row[CswScheduleLogicDetail.ColumnNames.Interval] = LogicDetail.Interval;
                        Row[CswScheduleLogicDetail.ColumnNames.ReprobateThreshold] = LogicDetail.ReprobateThreshold;
                        Row[CswScheduleLogicDetail.ColumnNames.MaxRunTimeMs] = LogicDetail.MaxRunTimeMs;
                        Row[CswScheduleLogicDetail.ColumnNames.Reprobate] = LogicDetail.Reprobate;
                        Row[CswScheduleLogicDetail.ColumnNames.RunStartTime] = LogicDetail.RunStartTime;
                        Row[CswScheduleLogicDetail.ColumnNames.RunEndTime] = LogicDetail.RunEndTime;
                        Row[CswScheduleLogicDetail.ColumnNames.TotalRogueCount] = LogicDetail.TotalRogueCount;
                        Row[CswScheduleLogicDetail.ColumnNames.FailedCount] = LogicDetail.FailedCount;
                        Row[CswScheduleLogicDetail.ColumnNames.ThreadId] = LogicDetail.ThreadId;
                        Row[CswScheduleLogicDetail.ColumnNames.StatusMessage] = LogicDetail.StatusMessage;
                        Row[CswScheduleLogicDetail.ColumnNames.Priority] = LogicDetail.Priority;
                        Row[CswScheduleLogicDetail.ColumnNames.LoadCount] = LogicDetail.LoadCount;
                        Row[CswScheduleLogicDetail.ColumnNames.Disabled] = LogicDetail.Disabled;
                        Row[CswScheduleLogicDetail.ColumnNames.HasChanged] = false;

                        GridTable.Rows.Add(Row);
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
                        Priority = Convert.ToUInt16( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.Priority )] ),
                        LoadCount = CswConvert.ToInt32( GridRow.data[new CswExtJsGridDataIndex( GridPrefix, CswScheduleLogicDetail.ColumnNames.LoadCount )] ),
                        Disabled = CswConvert.ToBoolean(GridRow.data[new CswExtJsGridDataIndex(GridPrefix, CswScheduleLogicDetail.ColumnNames.Disabled)])
                    };
                    CswSchedSvcParams.LogicDetails.Add(Rule);
                }
            }

            svcReturn = SchedSvcRef.updateScheduledRules( CswSchedSvcParams );
            if( null != svcReturn )
            {
                _updateScheduledRulesTable( NbtResources, CswSchedSvcParams.LogicDetails );
                _addScheduledRulesGrid( NbtResources, svcReturn.Data, Return );
            }
        }

        public CswNbtObjClassCustomer openCswAdminOnTargetSchema( string PropId, ref string TempPassword )
        {
            CswNbtObjClassCustomer RetNodeAsCustomer = null;

            if( string.IsNullOrEmpty( PropId ) )
            {
                throw new CswDniException( ErrorType.Error, "Authentication in this context is not possible.", "Authentication in this context is not possible." );
            }
            CswPropIdAttr PropAttr = new CswPropIdAttr( PropId );

            if( null == PropAttr ||
                null == PropAttr.NodeId ||
                Int32.MinValue == PropAttr.NodeId.PrimaryKey )
            {
                throw new CswDniException( ErrorType.Error, "Authentication in this context is not possible.", "Authentication in this context is not possible." );
            }
            CswNbtNode CustomerNode = _NbtManagerResources.Nodes.GetNode( PropAttr.NodeId );

            if( null == CustomerNode ||
                CustomerNode.getObjectClass().ObjectClass != NbtObjectClass.CustomerClass )
            {
                throw new CswDniException( ErrorType.Error, "Authentication in this context is not possible.", "Authentication in this context is not possible." );
            }

            RetNodeAsCustomer = (CswNbtObjClassCustomer) CustomerNode;
            string AccessId = RetNodeAsCustomer.CompanyID.Text;

            CswNbtResources OtherResources = makeOtherResources( AccessId );
            CswNbtNode ChemSWAdminUserNode = OtherResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
            CswNbtObjClassUser AdminNodeAsUser = (CswNbtObjClassUser) ChemSWAdminUserNode;
            TempPassword = CswNbtObjClassUser.makeRandomPassword( 20 );

            AdminNodeAsUser.AccountLocked.Checked = Tristate.False;
            AdminNodeAsUser.PasswordProperty.Password = TempPassword;
            AdminNodeAsUser.postChanges( true );
            _finalize( OtherResources );

            /* 
             * case 25694 - clear the current user, or else it will be confused with nodes in the new schemata 
             * case 25206
             */
            _NbtManagerResources.clearCurrentUser();
            _NbtManagerResources.InitCurrentUser = InitUser;

            return RetNodeAsCustomer;
        }

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
                    ThisRule["runstarttime"] = ScheduledRule.RunStartTime == DateTime.MinValue ? ( object ) DBNull.Value : ScheduledRule.RunStartTime;
                    ThisRule["runendtime"] = ScheduledRule.RunEndTime == DateTime.MinValue ? ( object ) DBNull.Value : ScheduledRule.RunEndTime;
                    ThisRule["lastrun"] = ScheduledRule.LastRun == DateTime.MinValue ? ( object ) DBNull.Value : ScheduledRule.LastRun;
                    ThisRule["threadid"] = ScheduledRule.ThreadId;
                    ThisRule["loadcount"] = ScheduledRule.LoadCount;
                    RulesUpdate.update( RulesTable );
                }
                else
                {
                    NbtResources.CswLogger.reportAppState( "Scheduled Rule " + ScheduledRule.RuleName + " does not exist in the database." );
                }
            }
        }

        #endregion private

    } // class CswNbtWebServiceNbtManager

} // namespace ChemSW.Nbt.WebServices

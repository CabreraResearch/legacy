using System;
using System.Data;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNbtManager
    {
        private readonly CswNbtResources _OtherResources;
        private readonly CswNbtResources _NbtManagerResources = null;

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources, string AccessId )
        {
            _NbtManagerResources = NbtManagerResources;
            _OtherResources = makeOtherResources( AccessId );
        } //ctor

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources )
        {
            _NbtManagerResources = NbtManagerResources;
            if( false == _NbtManagerResources.IsModuleEnabled( CswNbtResources.CswNbtModule.NBTManager ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use NBT Manager web services if the NBT Manager module is not enabled.", "Attempted to instance CswNbtWebServiceNbtManager, while the NBT Manager module is not enabled." );
            }
        } //ctor

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
            return new CswNbtSystemUser( Resources, "CswNbtWebServiceNbtManager_SystemUser" );
        }

        public JObject getActiveAccessIds()
        {
            JObject RetObj = new JObject();
            JArray CustomerIds = new JArray();
            RetObj["customerids"] = CustomerIds;

            foreach( string AccessId in _NbtManagerResources.CswDbCfgInfo.AccessIds )
            {
                if( _NbtManagerResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
                {
                    CustomerIds.Add( AccessId );
                }
            }
            return RetObj;
        }

        private void _ValidateAccessId( string AccessId )
        {
            if( string.IsNullOrEmpty( AccessId ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot get Scheduled Rules without a Customer ID.", "getScheduledRulesGrid was called with a null or empty AccessID." );
            }
            if( false == _NbtManagerResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
            {
                throw new CswDniException( ErrorType.Error, "The supplied Customer ID " + AccessId + " does not exist or is not enabled.", "No configuration could be loaded for AccessId " + AccessId + "." );
            }
        }

        public JObject getScheduledRulesGrid()
        {
            JObject RetObj;
            CswTableSelect ScheduledRulesSelect = _OtherResources.makeCswTableSelect( "Scheduledrules_select_on_" + _OtherResources.AccessId, "scheduledrules" );
            DataTable ScheduledRulesTable = ScheduledRulesSelect.getTable();

            CswGridData GridData = new CswGridData( _OtherResources );
            string TablePkColumn = "scheduledruleid";
            GridData.PkColumn = TablePkColumn;
            GridData.HidePkColumn = true;

            CswCommaDelimitedString ExcludedColumns = new CswCommaDelimitedString()
                                                          {
                                                              "THREADID"
                                                          };
            CswCommaDelimitedString ReadOnlyColumns = new CswCommaDelimitedString()
                                                          {
                                                              "RULENAME",
                                                              "TOTALROGUECOUNT",
                                                              "RUNSTARTTIME",
                                                              "RUNENDTIME",
                                                              "LASTRUN",
                                                              "STATUSMESSAGE"
                                                          };

            foreach( string ColumnName in ExcludedColumns )
            {
                ScheduledRulesTable.Columns.Remove( ColumnName );
            }

            GridData.EditableColumns = new CswCommaDelimitedString();
            foreach( DataColumn Column in ScheduledRulesTable.Columns )
            {
                if( false == ReadOnlyColumns.Contains( Column.ColumnName ) )
                {
                    GridData.EditableColumns.Add( Column.ColumnName );
                }
            }

            RetObj = GridData.DataTableToJSON( ScheduledRulesTable, true );

            return RetObj;
        }

        public bool updateScheduledRule( HttpContext Context )
        {
            bool RetSuccess = false;

            Int32 ScheduledRuleId = CswConvert.ToInt32( Context.Request["id"] );
            Int32 FailedCount = CswConvert.ToInt32( Context.Request["FAILEDCOUNT"] );
            bool Reprobate = CswConvert.ToBoolean( Context.Request["REPROBATE"] );
            bool Disabled = CswConvert.ToBoolean( Context.Request["DISABLED"] );

            string RecurranceString = CswConvert.ToString( Context.Request["RECURRENCE"] );
            Recurrance Recurrance;
            Enum.TryParse( RecurranceString, true, out Recurrance );

            Int32 Interval = CswConvert.ToInt32( Context.Request["INTERVAL"] );
            Int32 ReprobateThreshold = CswConvert.ToInt32( Context.Request["REPROBATETHRESHOLD"] );
            Int32 MaxRunTimeMs = CswConvert.ToInt32( Context.Request["MAXRUNTIMEMS"] );

            CswTableUpdate RulesUpdate = _OtherResources.makeCswTableUpdate( "Scheduledrules_update_on_accessid_" + _OtherResources.AccessId + "_id_" + ScheduledRuleId, "scheduledrules" );
            DataTable RulesTable = RulesUpdate.getTable( "scheduledruleid", ScheduledRuleId, true );
            if( RulesTable.Rows.Count == 1 )
            {
                DataRow ThisRule = RulesTable.Rows[0];
                if( 0 <= FailedCount )
                {
                    ThisRule["FAILEDCOUNT"] = CswConvert.ToDbVal( FailedCount );
                }

                ThisRule["REPROBATE"] = CswConvert.ToDbVal( Reprobate );
                ThisRule["DISABLED"] = CswConvert.ToDbVal( Disabled );

                if( Recurrance != Recurrance.Unknown )
                {
                    ThisRule["RECURRENCE"] = CswConvert.ToDbVal( Recurrance.ToString() );
                }
                if( 0 < Interval )
                {
                    ThisRule["INTERVAL"] = CswConvert.ToDbVal( FailedCount );
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
                throw new CswDniException( ErrorType.Error, "Attempt to update the Scheduled Rules table failed.", "Could not update scheduledruleid=" + ScheduledRuleId + " on Customer ID " + _OtherResources.AccessId + "." );
            }
            _finalize();
            return RetSuccess;
        }

        private void _finalize()
        {
            _OtherResources.finalize();
        }

    } // class CswNbtWebServiceNbtManager

} // namespace ChemSW.Nbt.WebServices

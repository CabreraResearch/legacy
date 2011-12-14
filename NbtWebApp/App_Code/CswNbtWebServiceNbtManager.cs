using System;
using System.Data;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNbtManager
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtWebServiceNbtManager( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.NBTManager ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use NBT Manager web services if the NBT Manager module is not enabled.", "Attempted to instance CswNbtWebServiceNbtManager, while the NBT Manager module is not enabled." );
            }
        } //ctor

        public JObject getActiveAccessIds()
        {
            JObject RetObj = new JObject();
            JArray CustomerIds = new JArray();
            RetObj["customerids"] = CustomerIds;

            foreach( string AccessId in _CswNbtResources.CswDbCfgInfo.AccessIds )
            {
                if( _CswNbtResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
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
            if( false == _CswNbtResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
            {
                throw new CswDniException( ErrorType.Error, "The supplied Customer ID " + AccessId + " does not exist or is not enabled.", "No configuration could be loaded for AccessId " + AccessId + "." );
            }
        }

        public JObject getScheduledRulesGrid( string AccessId )
        {
            JObject RetObj;
            _ValidateAccessId( AccessId );

            _CswNbtResources.AccessId = AccessId;
            CswTableSelect ScheduledRulesSelect = _CswNbtResources.makeCswTableSelect( "Scheduledrules_select_on_" + AccessId, "scheduledrules" );
            DataTable ScheduledRulesTable = ScheduledRulesSelect.getTable();

            CswGridData GridData = new CswGridData( _CswNbtResources );
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

            string AccessId = CswConvert.ToString( Context.Request["AccessId"] );
            _ValidateAccessId( AccessId );
            _CswNbtResources.AccessId = AccessId;

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

            CswTableUpdate RulesUpdate = _CswNbtResources.makeCswTableUpdate( "Scheduledrules_update_on_accessid_" + AccessId + "_id_" + ScheduledRuleId, "scheduledrules" );
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
                throw new CswDniException( ErrorType.Error, "Attempt to update the Scheduled Rules table failed.", "Could not update scheduledruleid=" + ScheduledRuleId + " on Customer ID " + AccessId + "." );
            }

            return RetSuccess;
        }

    } // class CswNbtWebServiceNbtManager

} // namespace ChemSW.Nbt.WebServices

using System;
using System.Data;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
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
    public class CswNbtWebServiceNbtManager
    {
        #region ctor

        private readonly CswNbtResources _OtherResources;
        private readonly CswNbtResources _NbtManagerResources = null;

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources, string AccessId, bool AllowAnyAdmin = false )
        {
            _NbtManagerResources = NbtManagerResources;
            _checkNbtManagerPermission( AllowAnyAdmin );
            _OtherResources = makeOtherResources( AccessId );
        } //ctor

        public CswNbtWebServiceNbtManager( CswNbtResources NbtManagerResources, bool AllowAnyAdmin = false )
        {
            _NbtManagerResources = NbtManagerResources;
            _checkNbtManagerPermission( AllowAnyAdmin );
        } //ctor
        #endregion ctor

        #region private

        private void _checkNbtManagerPermission( bool AllowAnyAdmin )
        {
            if( false == _NbtManagerResources.Modules.IsModuleEnabled( CswNbtModuleName.NBTManager ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use NBT Manager web services if the NBT Manager module is not enabled.", "Attempted to instance CswNbtWebServiceNbtManager, while the NBT Manager module is not enabled." );
            }
            if( false == _NbtManagerResources.CurrentNbtUser.IsAdministrator() && ( false == AllowAnyAdmin || _NbtManagerResources.CurrentNbtUser.Username != CswNbtObjClassUser.ChemSWAdminUsername ) )
            {
                throw new CswDniException( ErrorType.Error, "Authentication in this context is not possible.", "Attempted to authenticate as " + _NbtManagerResources.CurrentNbtUser.Username + " on a privileged method." );
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
            return new CswNbtSystemUser( Resources, SystemUserNames.SysUsr_NbtWebSvcMgr );
        }

        public JObject getActiveAccessIds()
        {
            JObject RetObj = new JObject();
            JArray CustomerIds = new JArray();
            RetObj["customerids"] = CustomerIds;

            foreach( string AccessId in from string _AccessId in _NbtManagerResources.CswDbCfgInfo.AccessIds
                                        orderby _AccessId
                                        select _AccessId )
            {
                if( _NbtManagerResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
                {
                    CustomerIds.Add( AccessId );
                }
            }
            return RetObj;
        }


        public static void getScheduledRulesGrid( ICswResources CswResources, CswNbtScheduledRulesReturn Return, string PlaceHolder )
        {
            CswSchedSvcAdminEndPointClient SchedSvcRef = new CswSchedSvcAdminEndPointClient();
            // GOTO CswSchedSvcAdminEndPoint for actual implementation
            CswSchedSvcReturn svcReturn = SchedSvcRef.getRules();
            Return.Data.grid = svcReturn.ExtJsGrid;
        }//getScheduledRulesGrid()

        
        public bool updateScheduledRule( HttpContext Context )
        {
            bool RetSuccess = false;

            Int32 ScheduledRuleId = CswConvert.ToInt32( Context.Request["id"] );
            Int32 FailedCount = CswConvert.ToInt32( Context.Request["FAILEDCOUNT"] );
            bool Reprobate = CswConvert.ToBoolean( Context.Request["REPROBATE"] );
            bool Disabled = CswConvert.ToBoolean( Context.Request["DISABLED"] );

            string RecurrenceString = CswConvert.ToString( Context.Request["RECURRENCE"] );
            Recurrence Recurrence;
            Enum.TryParse( RecurrenceString, true, out Recurrence );

            Int32 Interval = CswConvert.ToInt32( Context.Request["INTERVAL"] );
            Int32 ReprobateThreshold = CswConvert.ToInt32( Context.Request["REPROBATETHRESHOLD"] );
            Int32 MaxRunTimeMs = CswConvert.ToInt32( Context.Request["MAXRUNTIMEMS"] );

            CswTableUpdate RulesUpdate = _OtherResources.makeCswTableUpdate( "Scheduledrules_update_on_accessid_" + _OtherResources.AccessId + "_id_" + ScheduledRuleId, "scheduledrules" );
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

                if( Recurrence != Recurrence.Unknown )
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
                throw new CswDniException( ErrorType.Error, "Attempt to update the Scheduled Rules table failed.", "Could not update scheduledruleid=" + ScheduledRuleId + " on Customer ID " + _OtherResources.AccessId + "." );
            }
            _finalize( _OtherResources );
            return RetSuccess;
        }

        private enum ScheduledRuleActions
        {
            Unknown,
            ClearAllReprobates
        }

        public static void updateAllScheduledRules( ICswResources CswResources, CswNbtScheduledRulesReturn Return, string PlaceHolder )
        {
            //bool RetSuccess = false;

            //ScheduledRuleActions RuleAction;
            //Enum.TryParse( Action, true, out RuleAction );

            //switch( RuleAction )
            //{
            //    case ScheduledRuleActions.Unknown:
            //        throw new CswDniException( ErrorType.Error, "Method was invoked with an invalid action", "Cannot call this web method with action " + Action + "." );
            //    case ScheduledRuleActions.ClearAllReprobates:
            //        CswTableUpdate RulesUpdate = _OtherResources.makeCswTableUpdate( "ClearAllReprobates_on_accessid_" + _OtherResources.AccessId + "_id", "scheduledrules" );
            //        DataTable RulesTable = RulesUpdate.getTable();
            //        foreach( DataRow Row in RulesTable.Rows )
            //        {
            //            Row["FAILEDCOUNT"] = CswConvert.ToDbVal( 0 );
            //            Row["REPROBATE"] = CswConvert.ToDbVal( 0 );
            //            Row["STATUSMESSAGE"] = DBNull.Value;
            //        }

            //        RetSuccess = RulesUpdate.update( RulesTable );
            //        break;
            //}

            //if( false == RetSuccess )
            //{
            //    throw new CswDniException( ErrorType.Error, "Attempt to update the Scheduled Rules table failed.", "Could not update scheduledrules on Customer ID " + _OtherResources.AccessId + "." );
            //}
            //_finalize( _OtherResources );
            //return RetSuccess;
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



    } // class CswNbtWebServiceNbtManager

} // namespace ChemSW.Nbt.WebServices

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Web.Services;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.Logic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using ChemSW.Nbt.LandingPage;
using ChemSW.Security;
using ChemSW.Session;
using ChemSW.WebSvc;
using Newtonsoft.Json.Linq;
using NbtWebApp.WebSvc.Logic.Menus.LandingPages;




namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// NBT Web service interface
    /// </summary>
    /// 
    [ScriptService]
    [WebService( Namespace = "ChemSW.Nbt.WebServices" )]
    [WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
    public class wsNBT : WebService
    {
        // case 25887
        CswTimer Timer = new CswTimer();


        #region Session and Resource Management

        private CswSessionResourcesNbt _CswSessionResources;
        private CswNbtResources _CswNbtResources;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

        private void _initResources()
        {
            _CswSessionResources = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, Context, string.Empty, SetupMode.NbtWeb );
            _CswNbtResources = _CswSessionResources.CswNbtResources;
            _CswNbtStatisticsEvents = _CswSessionResources.CswNbtStatisticsEvents;
            _CswNbtResources.beginTransaction();

            _CswNbtResources.logMessage( "WebServices: Session Started (_initResources called)" );

        }//_initResources() 

        private AuthenticationStatus _attemptRefresh( bool ThrowOnError = false )
        {
            AuthenticationStatus ret = _CswSessionResources.attemptRefresh();

            if( ThrowOnError &&
                ret != AuthenticationStatus.Authenticated )
            {
                throw new CswDniException( ErrorType.Warning, "Current session is not authenticated, please login again.", "Cannot execute web method without a valid session." );
            }

            if( ret == AuthenticationStatus.Authenticated )
            {
                // Set audit context
                string ContextViewId = string.Empty;
                string ContextActionName = string.Empty;
                if( Context.Request.Cookies["csw_currentviewid"] != null )
                {
                    ContextViewId = Context.Request.Cookies["csw_currentviewid"].Value;
                }
                if( Context.Request.Cookies["csw_currentactionname"] != null )
                {
                    ContextActionName = Context.Request.Cookies["csw_currentactionname"].Value;
                }

                if( ContextViewId != string.Empty )
                {
                    CswNbtView ContextView = _getView( ContextViewId );
                    if( ContextView != null )
                    {
                        _CswNbtResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
                    }
                }
                else if( ContextActionName != string.Empty )
                {
                    CswNbtAction ContextAction = _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ContextActionName )];
                    if( ContextAction != null )
                    {
                        _CswNbtResources.AuditContext = CswNbtAction.ActionNameEnumToString( ContextAction.Name ) + " (Action_" + ContextAction.ActionId.ToString() + ")";
                    }
                }
            }

            _CswNbtResources.ServerInitTime = Timer.ElapsedDurationInMilliseconds;

            return ret;

        } // _attemptRefresh()

        private void _deInitResources( CswNbtResources OtherResources = null )
        {
            if( _CswSessionResources != null )
            {
                _CswSessionResources.endSession();
                _CswNbtResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                _CswSessionResources.finalize();
                _CswSessionResources.release();
            }

            if( null != OtherResources )
            {
                OtherResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                OtherResources.finalize();
                OtherResources.release();
            }

            _CswNbtResources.TotalServerTime = Timer.ElapsedDurationInMilliseconds;

        } // _deInitResources()

        #region Sessions Action

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSessions()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    SortedList<string, CswSessionsListEntry> SessionList = _CswSessionResources.CswSessionManager.SessionsList.AllSessions;
                    foreach( CswSessionsListEntry Entry in SessionList.Values )
                    {
                        // Filter to the administrator's access id only
                        if( Entry.AccessId == _CswNbtResources.AccessId || _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername )
                        {
                            JObject JSession = new JObject();
                            JSession["sessionid"] = Entry.SessionId;
                            JSession["username"] = Entry.UserName;
                            JSession["logindate"] = Entry.LoginDate.ToString();
                            JSession["timeoutdate"] = Entry.TimeoutDate.ToString();
                            JSession["accessid"] = Entry.AccessId;
                            JSession["ismobile"] = Entry.IsMobile;
                            ReturnVal[Entry.SessionId] = JSession;
                        } // if (Entry.AccessId == Master.AccessID)
                    } // foreach (CswAuthenticator.SessionListEntry Entry in SessionList.Values)
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getSessions()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string endSession( string SessionId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    _CswSessionResources.CswSessionManager.clearSession( SessionId );
                    ReturnVal["result"] = "true";
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // endSession()



        #endregion Sessions Action

        #endregion Session and Resource Management

        #region Error Handling

        private void _error( Exception ex, out ErrorType Type, out string Message, out string Detail, out bool Display )
        {
            if( _CswNbtResources != null )
            {
                _CswNbtResources.CswLogger.reportError( ex );
                _CswNbtResources.Rollback();
            }

            CswDniException newEx = null;
            if( ex is CswDniException )
            {
                newEx = (CswDniException) ex;
            }
            else
            {
                newEx = new CswDniException( ex.Message, ex );
            }

            Display = true;
            if( _CswNbtResources != null )
            {
                if( newEx.Type == ErrorType.Warning )
                {
                    Display = ( _CswNbtResources.ConfigVbls.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Display = ( _CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }
            }

            Type = newEx.Type;
            Message = newEx.MsgFriendly;
            Detail = newEx.MsgEscoteric + "; " + ex.StackTrace;
        } // _error()

        //private void _jAddAuthenticationStatus( JObject JObj, AuthenticationStatus AuthenticationStatusIn, bool ForMobile = false )
        //{
        //    // ******************************************
        //    // IT IS VERY IMPORTANT for this function not to require the use of database resources, 
        //    // since it occurs AFTER the call to _deInitResources(), and thus will leak Oracle connections 
        //    // (see case 26273)
        //    // ******************************************

        //    if( JObj != null )
        //    {
        //        JObj["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
        //        if( false == ForMobile )
        //        {
        //            if( _CswSessionResources != null &&
        //                 _CswSessionResources.CswSessionManager != null )
        //            {
        //                //CswDateTime CswTimeout = new CswDateTime( _CswNbtResources, _CswSessionResources.CswSessionManager.TimeoutDate );
        //                //JObj["timeout"] = CswTimeout.ToClientAsJavascriptString();
        //                JObj["timeout"] = CswDateTime.ToClientAsJavascriptString( _CswSessionResources.CswSessionManager.TimeoutDate );
        //            }
        //            JObj["timer"] = new JObject();
        //            JObj["timer"]["serverinit"] = Math.Round( ServerInitTime, 3 );
        //            if( null != _CswNbtResources )
        //            {
        //                JObj["timer"]["dbinit"] = Math.Round( _CswNbtResources.CswLogger.DbInitTime, 3 );
        //                JObj["timer"]["dbquery"] = Math.Round( _CswNbtResources.CswLogger.DbQueryTime, 3 );
        //                JObj["timer"]["dbcommit"] = Math.Round( _CswNbtResources.CswLogger.DbCommitTime, 3 );
        //                JObj["timer"]["dbdeinit"] = Math.Round( _CswNbtResources.CswLogger.DbDeInitTime, 3 );
        //                JObj["timer"]["treeloadersql"] = Math.Round( _CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );
        //            }
        //            JObj["timer"]["servertotal"] = Math.Round( Timer.ElapsedDurationInMilliseconds, 3 );
        //            JObj["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
        //        }
        //    }
        //}//_jAuthenticationStatus()

        /*
        /// <summary>
        /// Returns error as JProperty
        /// </summary>
        private JObject jError( Exception ex )
        {
            JObject Ret = new JObject();
            string Message = string.Empty;
            string Detail = string.Empty;
            ErrorType Type = ErrorType.Error;
            bool Display = true;
            _error( ex, out Type, out Message, out Detail, out Display );

            Ret["success"] = "false";
            Ret["error"] = new JObject();
            Ret["error"]["display"] = Display.ToString().ToLower();
            Ret["error"]["type"] = Type.ToString();
            Ret["error"]["message"] = Message;
            Ret["error"]["detail"] = Detail;

            _deInitResources(); //<-- An hackadelic solution than which no greater hackadelic solution can be conceived for case 26204


            return Ret;

        }
         */

        #endregion Error Handling

        #region Web Methods

        #region Authentication

        private AuthenticationStatus _doCswAdminAuthenticate( string PropId )
        {
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources, true );
            string TempPassword = string.Empty;
            CswNbtObjClassCustomer NodeAsCustomer = ws.openCswAdminOnTargetSchema( PropId, ref TempPassword );

            // case 26549 - we need to remove the old session
            _CswSessionResources.CswSessionManager.clearSession( ExpireCookie: false );

            AuthenticationStatus = _authenticate( NodeAsCustomer.CompanyID.Text, CswNbtObjClassUser.ChemSWAdminUsername, TempPassword, false );

            if( AuthenticationStatus != AuthenticationStatus.Authenticated )
            {
                throw new CswDniException( ErrorType.Error, "Authentication in this context is not possible.", "Authentication in this context is not possible." );
            }

            return AuthenticationStatus;
        } // _doCswAdminAuthenticate()

        // Authenticates and sets up resources for an accessid and user
        private AuthenticationStatus _authenticate( string AccessId, string UserName, string Password, bool IsMobile )
        {
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;

            try
            {
                string ParsedAccessId = AccessId.ToLower().Trim();
                if( !string.IsNullOrEmpty( ParsedAccessId ) )
                {
                    _CswSessionResources.CswSessionManager.setAccessId( ParsedAccessId );
                }
                else
                {
                    throw new CswDniException( ErrorType.Warning, "There is no configuration information for this AccessId", "AccessId is null or empty." );
                }
            }
            catch( CswDniException ex )
            {
                if( !ex.Message.Contains( "There is no configuration information for this AccessId" ) )
                {
                    throw ex;
                }
                else
                {
                    AuthenticationStatus = AuthenticationStatus.NonExistentAccessId;
                }
            }

            if( AuthenticationStatus == AuthenticationStatus.Unknown )
            {
                AuthenticationStatus = _CswSessionResources.CswSessionManager.beginSession( UserName, Password, CswWebSvcCommonMethods.getIpAddress(), IsMobile );
            }

            // case 21211
            if( AuthenticationStatus == AuthenticationStatus.Authenticated )
            {
                // case 21036
                if( IsMobile && false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Mobile ) )
                {
                    AuthenticationStatus = AuthenticationStatus.ModuleNotEnabled;
                    _CswSessionResources.CswSessionManager.clearSession();
                }
                CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
                //Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "passwordexpiry_days" ) );

                if( _CswNbtResources.CurrentNbtUser.PasswordIsExpired )
                {
                    // BZ 9077 - Password expired
                    AuthenticationStatus = AuthenticationStatus.ExpiredPassword;
                }
                else if( LicenseManager.MustShowLicense( _CswNbtResources.CurrentUser ) )
                {
                    // BZ 8133 - make sure they've seen the License
                    AuthenticationStatus = AuthenticationStatus.ShowLicense;
                }

            }

            //bury the overhead of nuking old sessions in the overhead of authenticating
            _CswSessionResources.purgeExpiredSessions();

            return AuthenticationStatus;
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string reauthenticate( string PropId )
        {
            JObject ReturnVal = new JObject();
            try
            {
                _initResources();
                AuthenticationStatus AuthenticationStatus = _doCswAdminAuthenticate( PropId );
                CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            return ( ReturnVal.ToString() );
        }//authenticate()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string authenticate( string AccessId, string UserName, string Password, string ForMobile )
        {
            JObject ReturnVal = new JObject();
            try
            {
                _initResources();

                bool IsMobile = CswConvert.ToBoolean( ForMobile );
                AuthenticationStatus AuthenticationStatus = _authenticate( AccessId, UserName, Password, IsMobile );

                if( AuthenticationStatus == AuthenticationStatus.ExpiredPassword )
                {
                    ICswNbtUser CurrentUser = _CswNbtResources.CurrentNbtUser;
                    ReturnVal.Add( new JProperty( "nodeid", CurrentUser.UserId.ToString() ) );
                    CswNbtNodeKey FakeKey = new CswNbtNodeKey( _CswNbtResources );
                    FakeKey.NodeId = CurrentUser.UserId;
                    FakeKey.NodeSpecies = NodeSpecies.Plain;
                    FakeKey.NodeTypeId = CurrentUser.UserNodeTypeId;
                    FakeKey.ObjectClassId = CurrentUser.UserObjectClassId;
                    ReturnVal.Add( new JProperty( "cswnbtnodekey", FakeKey.ToString() ) );
                    CswPropIdAttr PasswordPropIdAttr = new CswPropIdAttr( CurrentUser.UserId, CurrentUser.PasswordPropertyId );
                    ReturnVal.Add( new JProperty( "passwordpropid", PasswordPropIdAttr.ToString() ) );
                }

                //if( AuthenticationStatus == AuthenticationStatus.Authenticated ||
                //    AuthenticationStatus == AuthenticationStatus.ExpiredPassword ||
                //    AuthenticationStatus == AuthenticationStatus.ShowLicense )
                //{
                //    // initial quick launch setup
                //    CswNbtWebServiceQuickLaunchItems wsQL = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources );
                //    wsQL.initQuickLaunchItems();
                //}
                CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            return ReturnVal.ToString();
        }//authenticate()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string deauthenticate()
        {
            JObject ReturnVal = new JObject();

            try
            {
                _initResources();

                _CswSessionResources.CswSessionManager.clearSession();
                ReturnVal.Add( new JProperty( "Deauthentication", "Succeeded" ) );
                //_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus.Deauthenticated );
                CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus.Deauthenticated );
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            return ( ReturnVal.ToString() );

        }//deAuthenticate()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string RenewSession()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    _CswSessionResources.CswSessionManager.updateLastAccess( true );
                    ReturnVal.Add( new JProperty( "Renew", "Succeeded" ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ( ReturnVal.ToString() );

        }//RenewSession()

        #endregion Authentication

        #region Impersonation

        private bool _validateImpersonation( CswNbtObjClassUser UserToImpersonate )
        {
            return ( UserToImpersonate.Username != _CswNbtResources.CurrentNbtUser.Username &&
                     UserToImpersonate.Rolename != CswNbtObjClassRole.ChemSWAdminRoleName &&
                     UserToImpersonate.Username != CswNbtObjClassUser.ChemSWAdminUsername );

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string impersonate( string UserId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        CswPrimaryKey UserPk = _getNodeId( UserId );
                        CswNbtNode UserNode = _CswNbtResources.Nodes[UserPk];
                        if( UserNode != null )
                        {
                            CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) UserNode;
                            if( _validateImpersonation( UserNodeAsUser ) )
                            {
                                // clear Recent 
                                _CswNbtResources.SessionDataMgr.removeAllSessionData( _CswNbtResources.Session.SessionId );

                                _CswSessionResources.CswSessionManager.impersonate( UserPk, UserNodeAsUser.Username );

                                ReturnVal.Add( new JProperty( "result", "true" ) );
                            }
                            else
                            {
                                throw new CswDniException( ErrorType.Warning,
                                                   "You do not have permission to use this feature.",
                                                   "User " + _CswNbtResources.CurrentNbtUser.Username + " attempted to impersonate userid " + UserId + " but lacked permission to do so." );
                            }
                        }
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                   "You do not have permission to use this feature.",
                                                   "User " + _CswNbtResources.CurrentNbtUser.Username + " attempted to impersonate userid " + UserId + " but lacked permission to do so." );
                    }
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ( ReturnVal.ToString() );

        } // impersonate()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string endImpersonation()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    // We don't check for admin permissions here because the impersonated user may not have them!

                    // clear Recent 
                    _CswNbtResources.SessionDataMgr.removeAllSessionData( _CswNbtResources.Session.SessionId );

                    _CswSessionResources.CswSessionManager.endImpersonation();
                    ReturnVal.Add( new JProperty( "result", "true" ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ( ReturnVal.ToString() );

        } // endImpersonation()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getUsers()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        JArray UsersArray = new JArray();
                        CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
                        foreach( CswNbtObjClassUser ThisUser in ( from _UserNode in UserOC.getNodes( false, false )
                                                                  select (CswNbtObjClassUser) _UserNode ) )
                        {
                            if( _validateImpersonation( ThisUser ) )
                            {
                                JObject ThisUserObj = new JObject();
                                ThisUserObj["userid"] = ThisUser.NodeId.ToString();
                                ThisUserObj["username"] = ThisUser.Username;
                                UsersArray.Add( ThisUserObj );
                            }
                        }
                        ReturnVal["users"] = UsersArray;
                        ReturnVal.Add( new JProperty( "result", "true" ) );
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                   "You do not have permission to use this feature.",
                                                   "User " + _CswNbtResources.CurrentNbtUser.Username + " attempted to run getUsers()." );
                    }
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ( ReturnVal.ToString() );

        } // getUsers()

        #endregion Impersonation

        #region Render Core UI

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getDashboard()
        {

            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceHeader( _CswNbtResources );
                    ReturnVal = ws.getDashboard();
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getDashboard()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getHeaderMenu()
        {

            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceHeader( _CswNbtResources );
                    ReturnVal = ws.getHeaderMenu( _CswSessionResources );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getHeaderMenu()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getMainMenu( string ViewId, string SafeNodeKey, string NodeTypeId, string PropIdAttr, string LimitMenuTo, string ReadOnly )
        {

            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceMainMenu( _CswNbtResources, LimitMenuTo );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.getMenu( View, SafeNodeKey, CswConvert.ToInt32( NodeTypeId ), PropIdAttr, CswConvert.ToBoolean( ReadOnly ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getMainMenu()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getDefaultContent( string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.getDefaultContent( View );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getDefaultContent()

        /// <summary>
        /// Get the display mode for a view
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewMode( string ViewId )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtView View = _getView( ViewId );
                if( null != View )
                {
                    ReturnVal["viewmode"] = View.ViewMode.ToString();
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "The ViewId provided does not match a known view.", "ViewId: " + ViewId + " does not exist." );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getViewMode()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getRuntimeViewFilters( string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View = _getView( ViewId );
                    var ws = new CswNbtWebServiceView( _CswNbtResources );
                    ReturnVal = ws.getRuntimeViewFilters( View );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getRuntimeViewFilters()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string updateRuntimeViewFilters( string ViewId, string FiltersJson )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View = _getView( ViewId );
                    var ws = new CswNbtWebServiceView( _CswNbtResources );
                    ReturnVal = ws.updateRuntimeViewFilters( View, JObject.Parse( FiltersJson ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getRuntimeViewFilters()

        #region Grid Views

        private void _clearGroupBy( CswNbtViewRelationship Relationship )
        {
            Relationship.clearGroupBy();
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                _clearGroupBy( ChildRelationship );
            }
        }

        private CswNbtView _prepGridView( string ViewId, string CswNbtNodeKey, ref CswNbtNodeKey RealNodeKey )
        {
            bool IsQuickLaunch = false;
            return _prepGridView( ViewId, CswNbtNodeKey, ref RealNodeKey, ref IsQuickLaunch );
        }

        private CswNbtView _prepGridView( string ViewId, string CswNbtNodeKey, ref CswNbtNodeKey RealNodeKey, ref bool IsQuickLaunch )
        {
            CswNbtView RetView = _getView( ViewId );
            if( null != RetView )
            {
                if( RetView.Visibility == NbtViewVisibility.Property )
                {
                    RealNodeKey = _getNodeKey( CswNbtNodeKey );
                    if( null != RealNodeKey )
                    {
                        ( RetView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Clear(); // case 21676. Clear() to avoid cache persistence.
                        ( RetView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( RealNodeKey.NodeId );
                        IsQuickLaunch = false;
                    }
                }

                foreach( CswNbtViewRelationship ChildRelationship in RetView.Root.ChildRelationships )
                {
                    _clearGroupBy( ChildRelationship );
                }
                //if( RetView.ViewId.isSet() )
                //{
                //    RetView.save();
                //}
                if( RetView.SessionViewId.isSet() )
                {
                    RetView.SaveToCache( false );
                }
            }
            return RetView;
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getThinGrid( string ViewId, string IncludeNodeKey, string MaxRows )
        {
            UseCompression();
            JObject ReturnVal = new JObject();
            bool IsQuickLaunch = false;

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                CswNbtView View = _prepGridView( ViewId, IncludeNodeKey, ref RealNodeKey, ref IsQuickLaunch );
                Int32 RowLimit = CswConvert.ToInt32( MaxRows );
                if( null != View )
                {
                    var ws = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey: RealNodeKey, ForReport: false );
                    ReturnVal["rows"] = ws.getThinGridRows( RowLimit );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getThinGrid()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string gridExportCSV( string ViewId, string SafeNodeKey )
        {
            UseCompression();
            JObject ReturnVal = new JObject();
            bool IsQuickLaunch = false;

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                CswNbtView View = _prepGridView( ViewId, SafeNodeKey, ref RealNodeKey, ref IsQuickLaunch );
                if( null != View )
                {
                    var ws = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey: RealNodeKey, ForReport: false );
                    ws.ExportCsv( Context );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // gridExportCSV()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string runGrid( string ViewId, string IncludeNodeKey, string IncludeInQuickLaunch, string ForReport )
        {
            UseCompression();
            JObject ReturnVal = new JObject();
            bool IsQuickLaunch = CswConvert.ToBoolean( IncludeInQuickLaunch );

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                CswNbtView View = _prepGridView( ViewId, IncludeNodeKey, ref RealNodeKey, ref IsQuickLaunch );

                if( null != View )
                {
                    var ws = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey: RealNodeKey, ForReport: CswConvert.ToBoolean( ForReport ) );
                    ReturnVal = ws.runGrid( IsQuickLaunch );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // runGrid()




        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getGridRowCount( string ViewId, string IncludeNodeKey )
        {
            UseCompression();
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                CswNbtView View = _prepGridView( ViewId, IncludeNodeKey, ref RealNodeKey );

                if( null != View )
                {
                    var g = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey: RealNodeKey, ForReport: CswConvert.ToBoolean( false ) );
                    ReturnVal = g.getGridRowCount();
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getGrid()


        #endregion Grid Views

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string getTableSearch( string SearchTerm )
        //{
        //    JObject ReturnVal = new JObject();
        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    UseCompression();
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {
        //            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, SearchTerm );
        //            ReturnVal = wsTable.getTable( null );
        //        }

        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //} // getTable()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getTableView( string ViewId, string NodeId, string NodeKey )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            UseCompression();
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        CswNbtNode Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
                        CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, _CswNbtStatisticsEvents, View );
                        ReturnVal = wsTable.getTable();
                        View.SaveToCache( true );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getTable()

        private void UseCompression()
        {
            string encoding = "gzip";
            if( Context.Request.Headers["Accept-Encoding"] != null &&
                Context.Request.Headers["Accept-Encoding"].Contains( encoding ) )
            {
                Context.Response.Filter = new GZipStream( Context.Response.Filter, CompressionMode.Compress );
                Context.Response.AppendHeader( "Content-Encoding", encoding );
            }
        } // UseCompression()

        /// <summary>
        /// Prepare a tree of nodes for fetching, derived from a View
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string runTree( string ViewId, string IdPrefix, string IncludeNodeId, string IncludeNodeKey, bool IncludeNodeRequired, bool IncludeInQuickLaunch, string DefaultSelect )
        {
            UseCompression();
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        var ws = new CswNbtWebServiceTree( _CswNbtResources, View, IdPrefix );
                        CswPrimaryKey RealIncludeNodeId = _getNodeId( IncludeNodeId );

                        CswNbtNodeKey RealIncludeNodeKey = null;
                        if( !string.IsNullOrEmpty( IncludeNodeKey ) )
                            RealIncludeNodeKey = new CswNbtNodeKey( _CswNbtResources, IncludeNodeKey );

                        ReturnVal = ws.runTree( RealIncludeNodeId, RealIncludeNodeKey, IncludeNodeRequired, IncludeInQuickLaunch, DefaultSelect );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // runTree()

        ///// <summary>
        ///// Fetch a page of first level nodes from a prepared tree (see runTree)
        ///// </summary>
        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string fetchTreeFirstLevel( string ViewId, string IdPrefix, Int32 PageSize, Int32 PageNo, bool ForSearch )
        //{
        //    JObject ReturnVal = new JObject();

        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {

        //            CswNbtView View = _getView( ViewId );
        //            if( null != View )
        //            {
        //                var ws = new CswNbtWebServiceTree( _CswNbtResources, View, IdPrefix );
        //                ReturnVal = ws.fetchTreeFirstLevel( PageSize, PageNo, ForSearch );
        //            }
        //        }

        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //} // fetchTree()

        ///// <summary>
        ///// Fetch a page of child nodes from a prepared tree (see runTree) and parent range
        ///// </summary>
        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string fetchTreeLevel( string ViewId, string IdPrefix, Int32 Level, Int32 ParentRangeStart, Int32 ParentRangeEnd, Int32 PageSize, Int32 PageNo, bool ForSearch )
        //{
        //    JObject ReturnVal = new JObject();

        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {

        //            CswNbtView View = _getView( ViewId );
        //            if( null != View )
        //            {
        //                var ws = new CswNbtWebServiceTree( _CswNbtResources, View, IdPrefix );
        //                ReturnVal = ws.fetchTreeChildren( Level, ParentRangeStart, ParentRangeEnd, ForSearch );
        //            }
        //        }

        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //} // fetchTree()

        /// <summary>
        /// Generates a tree of nodes from the view
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getTreeOfView( string ViewId, string IdPrefix, bool IsFirstLoad, string ParentNodeKey, string IncludeNodeKey, bool IncludeNodeRequired,
                                       bool UsePaging, string ShowEmpty, bool ForSearch, bool IncludeInQuickLaunch )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    bool ShowEmptyTree = CswConvert.ToBoolean( ShowEmpty );
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        var ws = new CswNbtWebServiceTree( _CswNbtResources, View, IdPrefix );

                        CswNbtNodeKey RealParentNodeKey = null;
                        if( !string.IsNullOrEmpty( ParentNodeKey ) )
                            RealParentNodeKey = new CswNbtNodeKey( _CswNbtResources, ParentNodeKey );

                        CswNbtNodeKey RealIncludeNodeKey = null;
                        if( !string.IsNullOrEmpty( IncludeNodeKey ) )
                            RealIncludeNodeKey = new CswNbtNodeKey( _CswNbtResources, IncludeNodeKey );

                        ReturnVal = ws.getTree( IsFirstLoad, RealParentNodeKey, RealIncludeNodeKey, IncludeNodeRequired, UsePaging, ShowEmptyTree, ForSearch, IncludeInQuickLaunch );
                        //ws.runTree( View, IdPrefix, RealIncludeNodeKey, IncludeNodeRequired, IncludeInQuickLaunch, Context.Cache );
                        //ReturnVal = ws.fetchTree( View, Context.Cache, IdPrefix, 1, 1, 1000, ForSearch );

                        //CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
                        //View.SaveToCache(true);
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getTreeOfView()

        /// <summary>
        /// Generates a tree of nodes from the view
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getTreeOfNode( string IdPrefix, string NodePk )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    if( string.Empty != NodePk )
                    {
                        CswPrimaryKey NodeId = _getNodeId( NodePk );
                        CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
                        CswNbtView View = Node.getNodeType().CreateDefaultView();
                        View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( NodeId );

                        var ws = new CswNbtWebServiceTree( _CswNbtResources, View, IdPrefix );
                        //ReturnVal = ws.getTree( true, null, null, false, false, false, false, true );
                        ReturnVal = ws.runTree( null, null, false, true, "firstchild" );
                        //CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
                        //View.SaveToCache( true );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();


        } // getTreeOfNode()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getNodes( string NodeTypeId, string ObjectClassId, string ObjectClass, string RelatedToObjectClass, string RelatedToNodeId )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                ReturnVal = ws.getNodes( NodeTypeId, ObjectClassId, ObjectClass, RelatedToObjectClass, RelatedToNodeId );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();


        } // getNodes()

        #endregion Render Core UI

        #region View Editing

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewGrid( bool All, string SelectedViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceView ws = new CswNbtWebServiceView( _CswNbtResources );
                    ReturnVal = ws.getViewGrid( All );

                    // This translates CswNbtSessionDataIds into CswNbtViewIds for the client
                    CswNbtView SelectedView = _getView( SelectedViewId );
                    if( SelectedView != null && SelectedView.ViewId != null && SelectedView.ViewId.isSet() )
                    {
                        ReturnVal.Add( new JProperty( "selectedpk", SelectedView.ViewId.get().ToString() ) );
                    }
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getViewGrid()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewInfo( string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        ReturnVal = View.ToJson();
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();
        } // getViewInfo()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveViewInfo( string ViewId, string ViewJson )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        View.LoadJson( ViewJson );
                        View.save();

                        //if( View.Visibility != NbtViewVisibility.Property )
                        //    CswViewListTree.ClearCache( Session );
                        _CswNbtResources.ViewSelect.removeSessionView( View );
                        _CswNbtResources.ViewSelect.clearCache();

                        ReturnVal.Add( new JProperty( "succeeded", "true" ) );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();


        } // saveViewInfo()



        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewChildOptions( string ViewJson, string ArbitraryId, string StepNo )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtWebServiceView ws = new CswNbtWebServiceView( _CswNbtResources );
                    ReturnVal = ws.getViewChildOptions( ViewJson, ArbitraryId, CswConvert.ToInt32( StepNo ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getViewChildOptions()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string copyView( string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView SourceView = _getView( ViewId );
                    if( null != SourceView )
                    {
                        CswNbtView NewView = new CswNbtView( _CswNbtResources );
                        string NewViewNameOrig = SourceView.ViewName;
                        string Suffix = " Copy";
                        if( !NewViewNameOrig.EndsWith( Suffix ) && NewViewNameOrig.Length < ( CswNbtView.ViewNameLength - Suffix.Length - 2 ) )
                            NewViewNameOrig = NewViewNameOrig + Suffix;
                        string NewViewName = NewViewNameOrig;
                        if( NewViewNameOrig.Length > ( CswNbtView.ViewNameLength - 2 ) )
                            NewViewNameOrig = NewViewNameOrig.Substring( 0, ( CswNbtView.ViewNameLength - 2 ) );
                        Int32 Increment = 1;
                        while( !CswNbtView.ViewIsUnique( _CswNbtResources, new CswNbtViewId(), NewViewName, SourceView.Visibility, SourceView.VisibilityUserId, SourceView.VisibilityRoleId ) )
                        {
                            Increment++;
                            NewViewName = NewViewNameOrig + " " + Increment.ToString();
                        }

                        NewView.saveNew( NewViewName, SourceView.Visibility, SourceView.VisibilityRoleId, SourceView.VisibilityUserId, SourceView );
                        //NewView.save();
                        ReturnVal.Add( new JProperty( "copyviewid", NewView.ViewId.ToString() ) );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();

        } // copyView()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string deleteView( string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {

                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView DoomedView = _getView( ViewId );
                    if( null != DoomedView )
                    {
                        // Remove from quick launch
                        _CswNbtResources.SessionDataMgr.removeSessionData( DoomedView );

                        DoomedView.Delete();
                        ReturnVal.Add( new JProperty( "succeeded", true ) );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // deleteView()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string createView( string ViewName, string Category, string ViewMode, string Visibility, string VisibilityRoleId, string VisibilityUserId, string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {

                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    NbtViewVisibility RealVisibility = (NbtViewVisibility) Visibility;
                    CswPrimaryKey RealVisibilityRoleId = null;
                    CswPrimaryKey RealVisibilityUserId = null;
                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        //Enum.TryParse<NbtViewVisibility>( Visibility, out RealVisibility );
                        if( RealVisibility == NbtViewVisibility.Role )
                        {
                            RealVisibilityRoleId = _getNodeId( VisibilityRoleId );
                        }
                        else if( RealVisibility == NbtViewVisibility.User )
                        {
                            RealVisibilityUserId = _getNodeId( VisibilityUserId );
                        }
                    }
                    else
                    {
                        RealVisibility = NbtViewVisibility.User;
                        RealVisibilityUserId = _CswNbtResources.CurrentUser.UserId;
                    }

                    CswNbtView CopyView = null;
                    if( ViewId != string.Empty )
                    {
                        CopyView = _getView( ViewId );
                    }

                    CswNbtView NewView = new CswNbtView( _CswNbtResources );
                    NewView.saveNew( ViewName, RealVisibility, RealVisibilityRoleId, RealVisibilityUserId, CopyView );
                    NewView.Category = Category;

                    if( ViewMode != string.Empty )
                    {
                        //NbtViewRenderingMode RealViewMode = NbtViewRenderingMode.Unknown;
                        //Enum.TryParse<NbtViewRenderingMode>( ViewMode, out RealViewMode );
                        NewView.ViewMode = (NbtViewRenderingMode) ViewMode;
                    }

                    NewView.save();
                    ReturnVal.Add( new JProperty( "newviewid", NewView.ViewId.ToString() ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // createView()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getAllViewPropFilters( string ViewId, string NewPropArbIds, string ViewJson )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                if( ViewId != string.Empty )
                {
                    var ws = new CswNbtViewBuilder( _CswNbtResources );

                    CswNbtView View = _getView( ViewId );
                    if( View != null )
                    {
                        ReturnVal = ws.getVbProperties( View );
                    }
                    ws.getVbProperties( ReturnVal, NewPropArbIds, ViewJson );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewPropFilterUI( string ViewJson, string ViewId, string PropArbitraryId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtViewBuilder( _CswNbtResources );
                    if( ViewJson != string.Empty )
                    {
                        ReturnVal = ws.getVbProp( ViewJson, PropArbitraryId );
                    }
                    else if( ViewId != string.Empty )
                    {
                        CswNbtView View = _getView( ViewId );
                        if( View != null )
                        {
                            ReturnVal = ws.getVbProp( View, PropArbitraryId );
                        }
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string makeViewPropFilter( string ViewJson, string PropFiltJson )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtViewBuilder( _CswNbtResources );
                    ReturnVal = ws.makeViewPropFilter( ViewJson, PropFiltJson );
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        #endregion View Editing

        #region Tabs and Props

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getTabs( string EditMode, string NodeId, string SafeNodeKey, string Date, string filterToPropId, string Multi, string ConfigMode )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents, CswConvert.ToBoolean( Multi ), CswConvert.ToBoolean( ConfigMode ) );
                    _setEditMode( EditMode );
                    CswDateTime InDate = new CswDateTime( _CswNbtResources );
                    InDate.FromClientDateTimeString( Date );
                    ReturnVal = ws.getTabs( NodeId, SafeNodeKey, InDate, filterToPropId );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getTabs()



        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId, string Date, string filterToPropId, string Multi, string ConfigMode, string RelatedNodeId, string RelatedNodeTypeId, string RelatedObjectClassId )
        {
            CswTimer GetPropsTimer = new CswTimer();

            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents, CswConvert.ToBoolean( Multi ), CswConvert.ToBoolean( ConfigMode ) );
                    _setEditMode( EditMode );
                    CswDateTime InDate = new CswDateTime( _CswNbtResources );
                    InDate.FromClientDateTimeString( Date );
                    CswNbtNodeKey NodeKey = _getNodeKey( SafeNodeKey );
                    Int32 NodeTypePk = CswConvert.ToInt32( NodeTypeId );
                    if( null != NodeKey && Int32.MinValue == NodeTypePk )
                    {
                        NodeTypePk = NodeKey.NodeTypeId;
                    }
                    ReturnVal = ws.getProps( NodeId, SafeNodeKey, TabId, NodeTypePk, InDate, filterToPropId, RelatedNodeId, RelatedNodeTypeId, RelatedObjectClassId );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            _CswNbtResources.logTimerResult( "wsNBT.getProps()", GetPropsTimer.ElapsedDurationInSecondsAsString );

            return ReturnVal.ToString();
        } // getProps()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSingleProp( string EditMode, string NodeId, string SafeNodeKey, string PropId, string NodeTypeId, string NewPropJson )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();

                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    _setEditMode( EditMode );
                    ReturnVal = ws.getSingleProp( NodeId, SafeNodeKey, PropId, CswConvert.ToInt32( NodeTypeId ), NewPropJson );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getSingleProp()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getPropNames( string Type, string Id )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    Int32 nId = CswConvert.ToInt32( Id );

                    if( nId != Int32.MinValue )
                    {
                        IEnumerable<ICswNbtMetaDataProp> Props = null;
                        string PropType = string.Empty;
                        if( Type == "NodeTypeId" )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( nId );
                            Props = NodeType.getNodeTypeProps();
                            PropType = NbtViewPropType.NodeTypePropId.ToString();
                        }
                        else if( Type == "ObjectClassId" )
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( nId );
                            Props = ObjectClass.getObjectClassProps();
                            PropType = NbtViewPropType.ObjectClassPropId.ToString();
                        }

                        if( null != Props )
                        {
                            foreach( ICswNbtMetaDataProp Prop in Props )
                            {
                                string PropId = "prop_" + Prop.PropId.ToString();
                                ReturnVal[PropId] = new JObject();
                                ReturnVal[PropId]["proptype"] = PropType;
                                ReturnVal[PropId]["propname"] = Prop.PropNameWithQuestionNo;
                                ReturnVal[PropId]["propid"] = Prop.PropId.ToString();
                            }
                        }
                    } // if( nId != Int32.MinValue )

                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }


            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getPropNames()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NewPropsJson, string NodeTypeId, string ViewId )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswPrimaryKey NodePk = _getNodeId( NodeId );
                    if( null == NodePk )
                    {
                        CswNbtNodeKey NbtNodeKey = _getNodeKey( SafeNodeKey );
                        if( null != NbtNodeKey )
                        {
                            NodePk = NbtNodeKey.NodeId;
                        }
                    }

                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    _setEditMode( EditMode );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.saveProps( NodePk, CswConvert.ToInt32( TabId ), NewPropsJson, CswConvert.ToInt32( NodeTypeId ), View );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // saveProps()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string copyPropValues( string SourceNodeKey, string[] CopyNodeIds, string[] CopyNodeKeys, string[] PropIds )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.copyPropValues( SourceNodeKey, CopyNodeIds, CopyNodeKeys, PropIds );
                }

                _deInitResources();

            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // copyPropValue()	

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getBlob()
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    //_purgeTempFiles( "xls" );
                    var ws = new CswNbtWebServiceBinaryData( _CswNbtResources );
                    ws.displayBlobData( Context );
                }

                _deInitResources();

            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getBlob()	


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getQuantity( string SizeId, string Action )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPrimaryKey SizePk = _getNodeId( SizeId );
                if( null != SizePk )
                {
                    var ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.getQuantityFromSize( SizePk, Action );
                }
                _deInitResources();

            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getQuantity()	

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSize( string RelatedNodeId )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPrimaryKey RelatedNodePk = _getNodeId( RelatedNodeId );
                if( null != RelatedNodePk )
                {
                    var ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.getSizeFromRelatedNodeId( RelatedNodePk );
                }
                _deInitResources();

            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getQuantity()	

        #endregion Tabs and Props

        #region MetaData

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getNodeTypes( string ObjectClassName, string ObjectClassId, string ExcludeNodeTypeIds, string RelatedToNodeTypeId, string RelatedObjectClassPropName, string FilterToPermission )
        {
            //sometimes we only want nodetypes for which the user has Create permissions - how do we determine this?
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                int OCId = CswConvert.ToInt32( ObjectClassId );
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtMetaDataObjectClass ObjectClass = null;
                    if( false == string.IsNullOrEmpty( ObjectClassName ) )
                    {
                        NbtObjectClass OC = ObjectClassName;
                        if( CswNbtResources.UnknownEnum != OC )
                        {
                            ObjectClass = _CswNbtResources.MetaData.getObjectClass( OC );
                        }
                    }
                    else if( Int32.MinValue != OCId )
                    {
                        ObjectClass = _CswNbtResources.MetaData.getObjectClass( OCId );
                    }
                    var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
                    ReturnVal = ws.getNodeTypes( ObjectClass, ExcludeNodeTypeIds, CswConvert.ToInt32( RelatedToNodeTypeId ), RelatedObjectClassPropName, FilterToPermission );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getNodeTypes()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string IsNodeTypeNameUnique( string NodeTypeName )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ReturnVal["succeeded"] = wsTools.isNodeTypeNameUnique( NodeTypeName, _CswNbtResources, true );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // IsNodeTypeNameUnique()s

        #endregion MetaData

        #region Misc

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getWatermark()
        {
            JObject ReturnVal = new JObject();

            // No authentication necessary
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Ignore;
            try
            {
                _initResources();

                if( _CswNbtResources.SetupVbls.doesSettingExist( "Watermark" ) )
                {
                    string Watermark = _CswNbtResources.SetupVbls.readSetting( "Watermark" );
                    if( string.Empty != Watermark )
                    {
                        ReturnVal["watermark"] = Watermark;
                    }
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getSessions()



        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string report( string reportid, string rformat )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;

            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtNode rpt = _CswNbtResources.Nodes[_getNodeId( reportid )];
                    CswNbtWebServiceReport ws = new CswNbtWebServiceReport( _CswNbtResources, rpt );
                    ReturnVal = ws.runReport( rformat, Context );

                } // if (AuthenticationStatus.Authenticated == AuthenticationStatus)
                _deInitResources();
            } // try
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();

        } // report

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string doesReportSupportCrystal( string reportid )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;

            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtNode rpt = _CswNbtResources.Nodes[_getNodeId( reportid )];
                    CswNbtObjClassReport rptAsReport = (CswNbtObjClassReport) rpt;
                    ReturnVal["result"] = ( false == rptAsReport.RPTFile.Empty ).ToString().ToLower();

                } // if (AuthenticationStatus.Authenticated == AuthenticationStatus)
                _deInitResources();
            } // try
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();

        } // doesReportSupportCrystal()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getAbout()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceHeader( _CswNbtResources );
                    ReturnVal = ws.makeVersionJson( _CswSessionResources );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();
        } // getAbout()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getLicense()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
                    ReturnVal.Add( new JProperty( "license", LicenseManager.LatestLicenseText ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string acceptLicense()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
                    LicenseManager.RecordLicenseAcceptance( _CswNbtResources.CurrentUser );
                    ReturnVal.Add( new JProperty( "result", "succeeded" ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public void fileForProp()
        {
            //Come back to implement Multi
            JObject ReturnVal = new JObject();
            ReturnVal["data"] = new JObject();
            ReturnVal["data"]["success"] = false;
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    for( Int32 I = 0; I < Context.Request.Files.Count; I += 1 )
                    {
                        HttpPostedFile File = Context.Request.Files[I];
                        string FileName = File.FileName;
                        string PropId = Context.Request["propid"];
                        string Column = Context.Request["column"];

                        Stream FileStream = File.InputStream;

                        if( false == string.IsNullOrEmpty( PropId ) )
                        {
                            string ContentType = File.ContentType;

                            // Read the binary data
                            BinaryReader br = new BinaryReader( FileStream );
                            long Length = FileStream.Length;
                            byte[] FileData = new byte[Length];
                            for( long CurrentIndex = 0; CurrentIndex < Length; CurrentIndex++ )
                            {
                                FileData[CurrentIndex] = br.ReadByte();
                            }

                            // Save the binary data
                            CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                            string Href;
                            bool ret = ws.SetPropBlobValue( FileData, FileName, ContentType, PropId, Column, out Href );

                            ReturnVal["data"]["success"] = ret;
                            if( ret )
                            {
                                ReturnVal["data"]["filename"] = FileName;
                                ReturnVal["data"]["contenttype"] = ContentType;
                                ReturnVal["data"]["href"] = Href;
                            }

                        } //if( false == string.IsNullOrEmpty( PropId ) )
                    } //for( Int32 I = 0; I < Context.Request.Files.Count; I += 1 )
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            //This is the only way to get the content back down to the client using jQuery File Upload.
            //DO NOT TOUCH.
            Context.Response.ContentType = "text/plain";
            Context.Response.Write( ReturnVal.ToString() );

        } // fileForProp()



        //dch
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public void saveMolPropFile()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    string PropId = Context.Request["propid"];
                    CswTempFile TempTools = new CswTempFile( _CswNbtResources );
                    Stream MolStream = TempTools.getFileInputStream( Context, "qqfile" );

                    if( null != MolStream && false == string.IsNullOrEmpty( PropId ) )
                    {
                        // Read the binary data
                        BinaryReader br = new BinaryReader( MolStream );
                        long Length = MolStream.Length;
                        byte[] FileData = new byte[Length];
                        for( long CurrentIndex = 0; CurrentIndex < Length; CurrentIndex++ )
                        {
                            FileData[CurrentIndex] = br.ReadByte();
                        }
                        string MolData = CswTools.ByteArrayToString( FileData ).Replace( "\r", "" );
                        CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                        ReturnVal = ws.saveMolProp( MolData, PropId );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
                ReturnVal["success"] = false;
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            //This is the only way to get the content back down to the client using jQuery File Upload.
            //DO NOT TOUCH.
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.Flush();
            Context.Response.Write( ReturnVal.ToString() );
            //return ReturnVal.ToString();

        } // saveMolPropFile()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveMolPropText( string molData, string PropId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.saveMolProp( molData, PropId );
                }
                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, ex );
                ReturnVal["success"] = false;
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );


            return ReturnVal.ToString();

        } // saveMolProp()

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string getLabels( string PropId )
        //{
        //    JObject ReturnVal = new JObject();
        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {

        //            CswNbtWebServicePrintLabels ws = new CswNbtWebServicePrintLabels( _CswNbtResources );
        //            ReturnVal = ws.getLabels( PropId );
        //        }

        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //}

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string getEPLText( string PropId, string PrintLabelNodeId )
        //{
        //    JObject ReturnVal = new JObject();

        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {

        //            CswNbtWebServicePrintLabels ws = new CswNbtWebServicePrintLabels( _CswNbtResources );
        //            ReturnVal = ws.getEPLText( PropId, PrintLabelNodeId );
        //        }

        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //}

        #endregion Misc

        #region NodeType Layout

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string MoveProp( string PropId, string TabId, string NewRow, string NewColumn, string EditMode )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    _setEditMode( EditMode );
                    bool ret = ws.moveProp( PropId, CswConvert.ToInt32( TabId ), CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ) );
                    ReturnVal.Add( new JProperty( "moveprop", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // MoveProp()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string removeProp( string PropId, string TabId, string EditMode )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    _setEditMode( EditMode );
                    bool ret = ws.removeProp( PropId, CswConvert.ToInt32( TabId ) );
                    ReturnVal.Add( new JProperty( "removeprop", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // removeProp()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getPropertiesForLayoutAdd( string NodeId, string NodeKey, string NodeTypeId, string TabId, string LayoutType )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType RealLayoutType = LayoutType;
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal["add"] = ws.getPropertiesForLayoutAdd( NodeId, NodeKey, NodeTypeId, TabId, RealLayoutType );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getPropertiesForLayoutAdd()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string addPropertyToLayout( string PropId, string TabId, string LayoutType )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType RealLayoutType = LayoutType;
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    bool ret = ws.addPropertyToLayout( PropId, TabId, RealLayoutType );
                    ReturnVal.Add( new JProperty( "result", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // addPropertyToLayout()

        #endregion NodeType Layout

        #region Search

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string getClientSearchJson( string ViewId, string SelectedNodeTypeIdNum, string IdPrefix, string NodeKey )
        //{
        //    JObject ReturnVal = new JObject();

        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {
        //            CswNbtNodeKey NbtNodeKey = _getNodeKey( NodeKey );
        //            var ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
        //            CswNbtView View = _getView( ViewId );
        //            ReturnVal = ws.getSearchJson( View, SelectedNodeTypeIdNum, NbtNodeKey );
        //        }

        //        _deInitResources();
        //    }

        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //}

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string getNodeTypeSearchProps( string RelatedIdType, string NodeTypeOrObjectClassId, string IdPrefix, string NodeKey )
        //{
        //    JObject ReturnVal = new JObject();
        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {
        //            var ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
        //            ReturnVal = ( ws.getSearchProps( RelatedIdType, NodeTypeOrObjectClassId, NodeKey ) );
        //        }

        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }


        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //} // getSearch()

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string getSearchableViews( string IsMobile, string OrderBy )
        //{
        //    JObject ReturnVal = new JObject();

        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {

        //            ICswNbtUser UserId = _CswNbtResources.CurrentNbtUser;
        //            bool ForMobile = CswConvert.ToBoolean( IsMobile );
        //            var ws = new CswNbtWebServiceView( _CswNbtResources );
        //            //SearchNode =  ws.getSearchableViewTree( UserId, ForMobile, true, OrderBy ); 
        //        }

        //        _deInitResources();

        //    }

        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    return ReturnVal.ToString();

        //} // getSearch()

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string doViewSearch( object SearchJson )
        //{
        //    JObject ReturnVal = new JObject();

        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {
        //            var ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
        //            CswNbtViewSearchPair SearchPair = ws.doViewBasedSearch( SearchJson );
        //            if( null != SearchPair )
        //            {
        //                ReturnVal.Add( new JProperty( "parentviewid", SearchPair.ParentViewId ) );
        //                ReturnVal.Add( new JProperty( "searchviewid", SearchPair.SearchViewId ) );
        //                ReturnVal.Add( new JProperty( "viewmode", SearchPair.ViewMode.ToString().ToLower() ) );
        //            }
        //        }

        //        _deInitResources();
        //    }

        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //} // getSearch()

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string doNodeTypeSearch( object SearchJson )
        //{
        //    JObject ReturnVal = new JObject();
        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();

        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
        //        {
        //            var ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
        //            CswNbtViewSearchPair SearchPair = ws.doNodesSearch( SearchJson );
        //            if( null != SearchPair )
        //            {
        //                ReturnVal.Add( new JProperty( "parentviewid", SearchPair.ParentViewId ) );
        //                ReturnVal.Add( new JProperty( "searchviewid", SearchPair.SearchViewId ) );
        //                ReturnVal.Add( new JProperty( "viewmode", SearchPair.ViewMode.ToString().ToLower() ) );
        //            }
        //        }

        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
        //    }

        //    CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //}

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string doUniversalSearch( string SearchTerm, string NodeTypeId, string ObjectClassId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.doUniversalSearch( SearchTerm, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( ObjectClassId ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // doUniversalSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string restoreUniversalSearch( string SessionDataId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtSessionDataId RealSessionDataId = new CswNbtSessionDataId( SessionDataId );
                    ReturnVal = ws.restoreUniversalSearch( RealSessionDataId );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // restoreUniversalSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string filterUniversalSearch( string SessionDataId, string Filter, string Action )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtSessionDataId RealSessionDataId = new CswNbtSessionDataId( SessionDataId );
                    ReturnVal = ws.filterUniversalSearch( RealSessionDataId, JObject.Parse( Filter ), Action );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // filterUniversalSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveSearchAsView( string SessionDataId, string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View = _getView( ViewId );
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtSessionDataId RealSessionDataId = new CswNbtSessionDataId( SessionDataId );
                    ReturnVal = ws.saveSearchAsView( RealSessionDataId, View );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // saveSearchAsView()

        #endregion Search

        #region Node DML

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string DeleteNodes( string[] NodePks, string[] NodeKeys )
        {
            JObject ret = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                    ret = ws.DeleteNodes( NodePks, NodeKeys );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {

                ret = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ret, AuthenticationStatus );
            return ret.ToString();
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string DeleteDemoDataNodes()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal["Succeeded"] = ws.deleteDemoDataNodes();
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string CopyNode( string NodeId, string NodeKey )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswPrimaryKey RealNodePk = _getNodeId( NodeId );
                    if( null == RealNodePk )
                    {
                        CswNbtNodeKey RealNodeKey = _getNodeKey( NodeKey );
                        if( null != RealNodeKey )
                        {
                            RealNodePk = RealNodeKey.NodeId;
                        }
                    }
                    if( null != RealNodePk )
                    {
                        CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                        CswPrimaryKey NewNodePk = ws.CopyNode( RealNodePk );
                        if( NewNodePk != null )
                        {
                            ReturnVal["NewNodeId"] = NewNodePk.ToString();
                        }
                        else
                        {
                            ReturnVal["NewNodeId"] = "";
                        }
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string clearProp( string PropId, string IncludeBlob )
        {
            //Come back to implement Multi
            JObject ReturnVal = new JObject( new JProperty( "Succeeded", false.ToString().ToLower() ) );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    bool ret = ws.ClearPropValue( PropId, CswConvert.ToBoolean( IncludeBlob ) );
                    ReturnVal = new JObject( new JProperty( "Succeeded", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // clearProp()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string onObjectClassButtonClick( string NodeTypePropAttr, string SelectedText )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPropIdAttr PropId = new CswPropIdAttr( NodeTypePropAttr );
                if( null == PropId.NodeId ||
                    Int32.MinValue == PropId.NodeId.PrimaryKey ||
                    Int32.MinValue == PropId.NodeTypePropId )
                {
                    throw new CswDniException( ErrorType.Error, "Cannot execute a button click without valid parameters.", "Attempted to call OnObjectClassButtonClick with invalid NodeId and NodeTypePropId." );
                }

                CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                ReturnVal = ws.doObjectClassButtonClick( PropId, SelectedText );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getFeedbackNode( string nodetypeid, string author, string actionname, string viewid, string selectednodeid, string viewmode )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtSdTabsAndProps tabsandprops = new CswNbtSdTabsAndProps( _CswNbtResources );

                CswNbtMetaDataNodeType feedbackNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( nodetypeid ) );
                CswNbtObjClassFeedback newFeedbackNode = tabsandprops.getAddNode( feedbackNT );

                //newFeedbackNode.Author.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                //newFeedbackNode.DateSubmitted.DateTimeValue = System.DateTime.Now;

                //if we have an action this is all we want/need/care about
                if( false == String.IsNullOrEmpty( actionname ) )
                {
                    newFeedbackNode.Action.Text = actionname.Replace( "%20", "_" );
                }
                //else //if we DONT have an action, we want the info required to load a view
                //{
                //    if( false == String.IsNullOrEmpty( viewid ) )
                //    {
                //        CswNbtViewId CurrentViewId = new CswNbtViewId( viewid );

                //        CswNbtView cookieView = _getView( viewid ); //this view doesn't exist in the the DB, which is why we save it below

                //        CswNbtView view = _CswNbtResources.ViewSelect.restoreView( newFeedbackNode.View.ViewId ); //WARNING!!!! calling View.ViewId creates a ViewId if there isn't one!
                //        view.LoadXml( cookieView.ToXml() );
                //        view.ViewId = newFeedbackNode.View.ViewId; //correct view.ViewId because of above problem.
                //        view.ViewName = cookieView.ViewName; //same as above, but name
                //        view.Visibility = NbtViewVisibility.Hidden; // see case 26799
                //        view.save();
                //    }
                //    newFeedbackNode.SelectedNodeId.Text = selectednodeid;
                //    newFeedbackNode.CurrentViewMode.Text = viewmode;
                //}
                //newFeedbackNode.Subject.Text = "Hello";
                newFeedbackNode.postChanges( false ); //DO I REALLY BREAK THIS?

                //CswNbtSdTabsAndProps tabsandprops = new CswNbtSdTabsAndProps( _CswNbtResources );
                _CswNbtResources.EditMode = NodeEditMode.Add;
                ////CswNbtMetaDataNodeType feedbackNT = _CswNbtResources.MetaData.getNodeType( newFeedbackNode.NodeTypeId );
                //CswNbtMetaDataNodeTypeTab feedbackNTT = feedbackNT.getFirstNodeTypeTab();
                ReturnVal["propdata"] = tabsandprops.getProps( newFeedbackNode.Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true ); //DO I REALLY BREAK THIS?
                ReturnVal["nodeid"] = newFeedbackNode.NodeId.ToString();

            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        }

        #endregion Node DML

        #region Permissions

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string isAdministrator()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ReturnVal.Add( new JProperty( "Administrator", _CswNbtResources.CurrentNbtUser.IsAdministrator().ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // isAdministrator()

        #endregion Permissions

        #region Connectivity
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string ConnectTest()
        {
            // no session needed here
            JObject Connected = new JObject();
            Connected["result"] = "OK";
            //            _jAddAuthenticationStatus( Connected, AuthenticationStatus.Authenticated, true );  // we don't want to trigger session timeouts
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, Connected, AuthenticationStatus.Authenticated, IsMobile: true );
            return ( Connected.ToString() );
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string ConnectTestDb()
        {
            JObject Connected = new JObject();

            // init resources



            CswNbtResources myResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtWeb, true, false, new CswSuperCycleCacheWeb( Context.Cache ) );
            myResources.InitCurrentUser = ConnectTestDb_InitUser;

            // use the first accessid
            myResources.AccessId = myResources.CswDbCfgInfo.AccessIds[0];

            // try the database
            CswTableSelect ConfigVarsTableSelect = myResources.makeCswTableSelect( "ConnectTestDb_Select", "configuration_variables" );
            DataTable ConfigVarsTable = ConfigVarsTableSelect.getTable();
            if( ConfigVarsTable.Rows.Count > 0 )
            {
                Connected["result"] = "OK";
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, Connected, AuthenticationStatus.Authenticated, IsMobile: true );
            //_jAddAuthenticationStatus( Connected, AuthenticationStatus.Authenticated );  // we don't want to trigger session timeouts
            return ( Connected.ToString() );

        } // ConnectTestDb()

        public ICswUser ConnectTestDb_InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, SystemUserNames.SysUsr_DbConnectTest );
        }



        [WebMethod( EnableSession = false )]
        public void ConnectTestFail()
        {
            // no session needed here

            // this exception needs to be UNCAUGHT
            throw new Exception( "Emulated connection failure" );
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string ConnectTestRandomFail()
        {
            // no session needed here

            // this exception needs to be UNCAUGHT
            Random r = new Random();
            Int32 rand = r.Next( 0, 3 );
            if( rand == 0 )
            {
                throw new Exception( "Emulated connection failure" );
            }
            else
            {
                JObject Connected = new JObject();
                return ( Connected.ToString() );
            }
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string ReturnTrue()
        {
            return "true";
        }
        #endregion Connectivity

        #region Logging

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string collectClientLogInfo( string Context, string UserName, string CustomerId, string LogInfo )
        {
            try
            {
                _initResources();

                if( !string.IsNullOrEmpty( UserName ) &&
                    !string.IsNullOrEmpty( CustomerId ) )
                {
                    string LogMessage = @"Application context '" + Context + "' requested logging for username '" + UserName + "' on AccessId '" + CustomerId + "'.";

                    _CswNbtResources.logMessage( LogMessage );
                }
                if( !string.IsNullOrEmpty( LogInfo ) )
                {
                    _CswNbtResources.logMessage( LogInfo );
                }
                _deInitResources();
            }

            catch
            {
                //nada
            }

            return new JObject( new JProperty( "succeeded", "true" ) ).ToString();
        } // collectClientLogInfo()

        #endregion Logging

        #region Actions

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string SaveActionToQuickLaunch( string ActionName )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    _CswNbtResources.SessionDataMgr.saveSessionData( _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionName )], true );
                    ReturnVal = new JObject( new JProperty( "succeeded", "true" ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // SaveActionToQuickLaunch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getGeneratorsTree()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceGenerators ws = new CswNbtWebServiceGenerators( _CswNbtResources );
                    ReturnVal = ws.getGeneratorsTree();
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getGeneratorsTree()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string futureScheduling( string SelectedGeneratorNodeKeys, string EndDate )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswCommaDelimitedString RealSelectedGeneratorNodeKeys = new CswCommaDelimitedString();
                    RealSelectedGeneratorNodeKeys.FromString( SelectedGeneratorNodeKeys );

                    DateTime RealEndDate = CswConvert.ToDateTime( EndDate );

                    CswNbtWebServiceGenerators ws = new CswNbtWebServiceGenerators( _CswNbtResources );
                    ReturnVal = ws.futureScheduling( RealSelectedGeneratorNodeKeys, RealEndDate );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // futureScheduling()


        #endregion Actions

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetFeedbackCaseNumber( string nodeId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtNode node = _CswNbtResources.Nodes[_getNodeId( nodeId )];
                    if( null != node )
                    {
                        if( node.getObjectClass().ObjectClass == NbtObjectClass.FeedbackClass )
                        {
                            CswNbtObjClassFeedback feedbackNode = node;
                            ReturnVal["casenumber"] = feedbackNode.CaseNumber.Sequence;
                            ReturnVal["noderef"] = node.NodeLink;
                        }
                    }
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            //_jAddAuthenticationStatus( ReturnVal, AuthenticationStatus.Authenticated );
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus.Authenticated );

            return ReturnVal.ToString();
        } // GetFeedbackCaseNumber


        #region Nbt Manager

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getActiveAccessIds()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources );
                ReturnVal = ws.getActiveAccessIds();

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getActiveAccessIds()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getScheduledRulesGrid( string AccessId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources, AccessId );
                ReturnVal = ws.getScheduledRulesGrid();

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getScheduledRulesGrid()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string updateScheduledRule()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                string AccessId = CswConvert.ToString( Context.Request["AccessId"] );
                CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources, AccessId );
                ReturnVal["success"] = ws.updateScheduledRule( Context );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // updateScheduledRule()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string updateAllScheduledRules( string AccessId, string Action )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources, AccessId );
                ReturnVal["success"] = ws.updateAllScheduledRules( Action );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // updateScheduledRule()

        #endregion Nbt Manager

        #region CISPro

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string createMaterial( string NodeTypeId, string Supplier, string Tradename, string PartNo )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceCreateMaterial ws = new CswNbtWebServiceCreateMaterial( _CswNbtResources, _CswNbtStatisticsEvents );
                ReturnVal = ws.createMaterial( CswConvert.ToInt32( NodeTypeId ), Supplier, Tradename, PartNo );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // createMaterial()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getMaterialSizes( string MaterialId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );
                ReturnVal = CswNbtWebServiceCreateMaterial.getMaterialSizes( _CswNbtResources, _getNodeId( MaterialId ) );
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getMaterialSizes()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSizeNodeProps( string SizeDefinition, string SizeNodeTypeId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                ReturnVal = CswNbtWebServiceCreateMaterial.getSizeNodeProps( _CswNbtResources, _CswNbtStatisticsEvents, CswConvert.ToInt32( SizeNodeTypeId ), SizeDefinition, false );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getMaterialSizes()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSizeLogicalsVisibility( string SizeNodeTypeId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );
                CswNbtWebServiceCreateMaterial ws = new CswNbtWebServiceCreateMaterial( _CswNbtResources, _CswNbtStatisticsEvents );
                ReturnVal = ws.getSizeLogicalsVisibility( CswConvert.ToInt32( SizeNodeTypeId ) );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getSizeLogicalsVisibility()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string commitMaterial( string MaterialDefinition )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceCreateMaterial ws = new CswNbtWebServiceCreateMaterial( _CswNbtResources, _CswNbtStatisticsEvents );
                _setEditMode( NodeEditMode.Edit );
                ReturnVal = ws.commitMaterial( MaterialDefinition );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // commitMaterial()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string receiveMaterial( string ReceiptDefinition )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                _setEditMode( NodeEditMode.Add );
                ReturnVal = CswNbtActReceiving.receiveMaterial( ReceiptDefinition, _CswNbtResources );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // receiveMaterial()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getMaterialUnitsOfMeasure( string MaterialId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                ReturnVal = CswNbtWebServiceCreateMaterial.getMaterialUnitsOfMeasure( MaterialId, _CswNbtResources );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getMaterialUnitsOfMeasure()

        #endregion CISPro

        #region Requesting

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getCurrentRequestId()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( _CswNbtResources );
                ReturnVal = ws.getCurrentRequestId();

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getCurrentRequestId()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getCurrentRequest()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( _CswNbtResources );
                ReturnVal = ws.getCurrentRequest();

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getCurrentRequest()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getRequestHistory()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestHistory );
                ReturnVal = ws.getRequestHistory();

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getRequestHistory()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string submitRequest( string RequestId, string RequestName )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPrimaryKey NodeId = _getNodeId( RequestId );
                if( null != NodeId )
                {
                    CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( _CswNbtResources );
                    ReturnVal = ws.submitRequest( NodeId, RequestName );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // submitRequest()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string copyRequest( string CopyFromRequestId, string CopyToRequestId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPrimaryKey CopyFromNodeId = _getNodeId( CopyFromRequestId );
                CswPrimaryKey CopyToNodeId = _getNodeId( CopyToRequestId );
                if( null != CopyFromNodeId && null != CopyToNodeId )
                {
                    CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart, CopyFromNodeId );
                    ReturnVal = ws.copyRequest( CopyFromNodeId, CopyToNodeId );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // copyRequest()

        #endregion Requesting

        #region Auditing

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getAuditHistoryGrid( string NodeId, string NbtNodeKey, string JustDateColumn )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceAuditing ws = new CswNbtWebServiceAuditing( _CswNbtResources );
                    CswPrimaryKey RealNodeId = _getNodeId( NodeId );
                    if( null == RealNodeId )
                    {
                        CswNbtNodeKey RealNodeKey = _getNodeKey( NbtNodeKey );
                        if( null != RealNodeKey && null != RealNodeKey.NodeId )
                        {
                            RealNodeId = RealNodeKey.NodeId;
                        }
                    }
                    ReturnVal = ws.getAuditHistoryGrid( _CswNbtResources.Nodes[RealNodeId], CswConvert.ToBoolean( JustDateColumn ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getInspectionStatusGrid()


        #endregion Auditing

        #region test
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetTestData()
        {
            JObject RetJson = new JObject( new JProperty( "A", "Static Page 1" ), new JProperty( "B", "Static Page 2" ), new JProperty( "C", "Dynamic Page A" ), new JProperty( "D", "Dynamic Page B" ) );
            //_jAddAuthenticationStatus( RetJson, AuthenticationStatus.Authenticated );
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, RetJson, AuthenticationStatus.Authenticated );
            return ( RetJson.ToString() );
        } // RunView()
        #endregion test

        #region Quotas

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getQuotas()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceQuotas( _CswNbtResources );
                    ReturnVal = ws.GetQuotas();
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getQuotas()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveQuotas( string Quotas )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceQuotas( _CswNbtResources );
                    ReturnVal["result"] = ws.SaveQuotas( Quotas ).ToString().ToLower();
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // saveQuotas()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getQuotaPercent()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceQuotas( _CswNbtResources );
                    ReturnVal["result"] = Math.Round( ws.GetHighestQuotaPercent() ).ToString();
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getQuotaPercent()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string checkQuota( string NodeTypeId, string NodeKey )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    Int32 NbtNodeTypeId = CswConvert.ToInt32( NodeTypeId );
                    if( Int32.MinValue == NbtNodeTypeId )
                    {
                        CswNbtNodeKey NbtNodekey = _getNodeKey( NodeKey );
                        if( null != NbtNodekey )
                        {
                            NbtNodeTypeId = NbtNodekey.NodeTypeId;
                        }
                    }
                    if( Int32.MinValue != NbtNodeTypeId )
                    {
                        var ws = new CswNbtWebServiceQuotas( _CswNbtResources );
                        ReturnVal["result"] = ws.CheckQuota( NbtNodeTypeId ).ToString().ToLower();
                    }
                    else
                    {
                        ReturnVal["result"] = false;
                    }
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getQuotaPercent()


        #endregion Quotas

        #region Modules

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getModules()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceModules( _CswNbtResources );
                    ReturnVal = ws.GetModules();
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getModules()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveModules( string Modules )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceModules( _CswNbtResources );
                    ReturnVal["result"] = ws.SaveModules( Modules ).ToString().ToLower();
                }

                _deInitResources();
            }

            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // saveModules()

        #endregion Modules

        #region Inspection Design

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string finalizeInspectionDesign( string DesignGrid, string InspectionDesignName, string InspectionTargetName, string IsNewInspection, string IsNewTarget, string Category )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;

            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( string.IsNullOrEmpty( InspectionDesignName ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "Inspection Name cannot be blank.", "InspectionName was null or empty." );
                    }
                    if( string.IsNullOrEmpty( InspectionTargetName ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "New Inspection must have a target.", "InspectionTarget was null or empty." );
                    }

                    CswNbtWebServiceInspectionDesign ws = new CswNbtWebServiceInspectionDesign( _CswNbtResources );

                    if( CswConvert.ToBoolean( IsNewInspection ) )
                    {
                        ReturnVal = ws.createInspectionDesignTabsAndProps( DesignGrid, InspectionDesignName, InspectionTargetName, Category );
                    }
                    else
                    {
                        ReturnVal = ws.recycleInspectionDesign( InspectionDesignName, InspectionTargetName, Category );
                    }

                    //do Schedules in a separate piece

                    ReturnVal["success"] = "true";

                } // if (AuthenticationStatus.Authenticated == AuthenticationStatus)
                _deInitResources();
            } // try
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // finalizeInspectionDesign()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public void previewInspectionFile()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            DataTable ExcelDataTable = null;
            string ErrorMessage = string.Empty;
            string WarningMessage = string.Empty;

            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswTempFile TempTools = new CswTempFile( _CswNbtResources );
                    TempTools.purgeTempFiles( "xls" );

                    string TempFileName = "excelupload_" + _CswNbtResources.CurrentUser.Username + "_" + DateTime.Now.ToString( "MMddyyyy_HHmmss" ) + ".xls";

                    // Load the excel file into a data table
                    CswNbtWebServiceInspectionDesign ws = new CswNbtWebServiceInspectionDesign( _CswNbtResources );

                    HttpPostedFile File = Context.Request.Files[0];
                    Stream FileStream = File.InputStream;
                    string FullPathAndFileName = TempTools.cacheInputStream( FileStream, TempFileName );

                    ExcelDataTable = ws.convertExcelFileToDataTable( FullPathAndFileName, ref ErrorMessage, ref WarningMessage );

                    // determine if we were successful or failure
                    if( ExcelDataTable == null || false == string.IsNullOrEmpty( ErrorMessage ) )
                    {
                        if( string.IsNullOrEmpty( ErrorMessage ) )
                        {
                            ErrorMessage = "Could not read Excel file.";
                        }
                        throw new CswDniException( ErrorType.Warning, "Could not read Excel file.", ErrorMessage );
                    }

                    CswNbtGrid gd = new CswNbtGrid( _CswNbtResources );
                    //gd.PkColumn = "RowNumber";
                    ReturnVal = gd.DataTableToJSON( ExcelDataTable, true );

                    ReturnVal["success"] = "true";
                    if( false == string.IsNullOrEmpty( WarningMessage ) )
                    {
                        ReturnVal["error"] = WarningMessage;
                    }
                } // if (AuthenticationStatus.Authenticated == AuthenticationStatus)
                _deInitResources();
            } // try
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            Context.Response.Clear();
            Context.Response.ContentType = "application/json; charset=utf-8";
            Context.Response.AddHeader( "content-disposition", "attachment; filename=export.json" );
            Context.Response.Flush();
            Context.Response.Write( ReturnVal.ToString() );
        } // finalizeInspectionDesign()

        #endregion Inspection Design

        #region Dispense Container

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string finalizeDispenseContainer( string SourceContainerNodeId, string DispenseType, string Quantity,
            string UnitId, string ContainerNodeTypeId, string DesignGrid, string RequestItemId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceContainer ws = new CswNbtWebServiceContainer( _CswNbtResources );
                    if( DispenseType.Contains( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense.ToString() ) && false == String.IsNullOrEmpty( DesignGrid ) )
                    {
                        ReturnVal = ws.upsertDispenseContainers( SourceContainerNodeId, ContainerNodeTypeId, DesignGrid, RequestItemId );
                    }
                    else
                    {
                        ReturnVal = ws.updateDispensedContainer( SourceContainerNodeId, DispenseType, Quantity, UnitId, RequestItemId );
                    }

                    ReturnVal["success"] = "true";

                } // if (AuthenticationStatus.Authenticated == AuthenticationStatus)
                _deInitResources();
            } // try
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();
        } // finalizeDispenseContainer()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getDispenseContainerView( string RequestItemId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );
                CswNbtWebServiceContainer ws = new CswNbtWebServiceContainer( _CswNbtResources );

                CswPrimaryKey RequestItemPk = _getNodeId( RequestItemId );
                if( null != RequestItemPk )
                {
                    CswNbtView ContainerView = ws.getDispensibleContainersView( RequestItemPk );
                    ContainerView.SaveToCache( false );
                    ReturnVal["viewid"] = ContainerView.SessionViewId.ToString();
                }

                _deInitResources();
            } // try
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();
        } // getDispenseContainerView()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getDispenseSourceContainerData( string ContainerId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );
                CswNbtWebServiceContainer ws = new CswNbtWebServiceContainer( _CswNbtResources );

                CswPrimaryKey ContainerPk = _getNodeId( ContainerId );
                if( null != ContainerPk )
                {
                    ReturnVal = ws.getContainerData( ContainerPk );
                }

                _deInitResources();
            } // try
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();
        } // getDispenseSourceContainerData()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string convertUnit( string ValueToConvert, string OldUnitId, string NewUnitId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPrimaryKey OldUnitPk = CswConvert.ToPrimaryKey( OldUnitId );
                CswPrimaryKey NewUnitPk = CswConvert.ToPrimaryKey( NewUnitId );
                CswNbtUnitConversion Conversion = new CswNbtUnitConversion( _CswNbtResources, OldUnitPk, NewUnitPk );
                double convertedValue = Conversion.convertUnit( CswConvert.ToDouble( ValueToConvert ) );
                ReturnVal["convertedvalue"] = convertedValue.ToString();

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();
        } // convertUnit()

        #endregion Dispense Container

        #endregion Web Methods

        #region Private

        private void _setEditMode( string EditModeStr )
        {
            _CswNbtResources.EditMode = EditModeStr;
        }

        private void _setEditMode( NodeEditMode EditMode )
        {
            _CswNbtResources.EditMode = EditMode;
        }

        private CswNbtView _getView( string ViewId )
        {
            CswNbtView View = null;
            if( CswNbtViewId.isViewIdString( ViewId ) )
            {
                CswNbtViewId realViewid = new CswNbtViewId( ViewId );
                View = _CswNbtResources.ViewSelect.restoreView( realViewid );
            }
            else if( CswNbtSessionDataId.isSessionDataIdString( ViewId ) )
            {
                CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ViewId );
                View = _CswNbtResources.ViewSelect.getSessionView( SessionViewid );
            }
            return View;
        } // _getView()

        private CswPrimaryKey _getNodeId( string NodeId )
        {
            CswPrimaryKey RetPk = null;
            CswPrimaryKey TryPk = null;
            if( CswTools.IsInteger( NodeId ) )
            {
                // If we use this, it means someone somewhere is using nodeids incorrectly
                // And the day may come when it must be fixed.
                TryPk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeId ) );
            }
            else if( false == string.IsNullOrWhiteSpace( NodeId ) )
            {
                TryPk = new CswPrimaryKey();
                TryPk.FromString( NodeId );
            }
            if( null != TryPk && Int32.MinValue != TryPk.PrimaryKey )
            {
                RetPk = TryPk;
            }
            return RetPk;
        }

        private CswNbtNodeKey _getNodeKey( string NodeKeyString )
        {
            CswNbtNodeKey RetKey = null;
            CswNbtNodeKey TryKey = null;
            if( false == string.IsNullOrEmpty( NodeKeyString ) )
            {
                TryKey = new CswNbtNodeKey( _CswNbtResources, NodeKeyString );
            }
            if( null != TryKey && null != TryKey.NodeId && Int32.MinValue != TryKey.NodeId.PrimaryKey )
            {
                RetKey = TryKey;
            }
            return RetKey;
        }

        #endregion Private
    }//wsNBT

} // namespace ChemSW.WebServices

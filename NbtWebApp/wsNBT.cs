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
using ChemSW.Nbt.Conversion;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.Logic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using ChemSW.Security;
using ChemSW.Session;
using ChemSW.WebSvc;
using Newtonsoft.Json.Linq;



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
            _CswSessionResources = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, Context, string.Empty, CswEnumSetupMode.NbtWeb );
            _CswNbtResources = _CswSessionResources.CswNbtResources;
            _CswNbtStatisticsEvents = _CswSessionResources.CswNbtStatisticsEvents;
            _CswNbtResources.beginTransaction();

            _CswNbtResources.logMessage( "WebServices: Session Started (_initResources called)" );

        }//_initResources() 

        private CswEnumAuthenticationStatus _attemptRefresh( bool ThrowOnError = false )
        {
            CswEnumAuthenticationStatus ret = _CswSessionResources.attemptRefresh();

            if( ThrowOnError &&
                ret != CswEnumAuthenticationStatus.Authenticated )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Current session is not authenticated, please login again.", "Cannot execute web method without a valid session." );
            }

            if( ret == CswEnumAuthenticationStatus.Authenticated )
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
                    _CswNbtResources.setAuditActionContext( CswNbtAction.ActionNameStringToEnum( ContextActionName ) );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    SortedList<string, CswSessionsListEntry> SessionList = _CswSessionResources.CswSessionManager.SessionsList.AllSessions;
                    foreach( CswSessionsListEntry Entry in from _Entry in SessionList.Values orderby _Entry.TimeoutDate select _Entry )
                    {
                        // Filter to the administrator's access id only
                        if( Entry.AccessId == _CswNbtResources.AccessId || _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername )
                        {
                            //TODO - Case 30573 - Fix so that the client apps only use one session instead of creating a new one for every call
                            //TODO - Then remove this if statement.
                            if( false == Entry.UserName.Contains( "printer" ) && Entry.UserName != "lpc" )
                            {
                                JObject JSession = new JObject();
                                JSession["sessionid"] = Entry.SessionId;
                                JSession["username"] = Entry.UserName;
                                JSession["logindate"] = Entry.LoginDate.ToString();
                                JSession["timeoutdate"] = Entry.TimeoutDate.ToString();
                                JSession["accessid"] = Entry.AccessId;
                                JSession["ismobile"] = Entry.IsMobile;
                                ReturnVal[Entry.SessionId] = JSession;
                            }
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtSessionDataMgr CswNbtSessionDataMgr = new CswNbtSessionDataMgr( _CswNbtResources );
                    CswNbtSessionDataMgr.removeAllSessionData( SessionId );
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

        #region Web Methods

        #region Authentication

        private CswEnumAuthenticationStatus _doCswAdminAuthenticate( string PropId )
        {
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Ignore;
            CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources, CswResources.UnknownEnum, true ); //No action associated with this method

            string CustomerAccessId = ws.getCustomerAccessId( PropId );
            if( false == _CswNbtResources.doesAccessIdExist( CustomerAccessId ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "The selected customer ID has not been defined.", "Cannot login to a customer schema unless it has been fully configured." );
            }
            _CswNbtResources.AccessId = CustomerAccessId;
            CswNbtObjClassUser UserNode = ws.getCswAdmin( CustomerAccessId );

            _CswNbtResources.CswSessionManager.changeSchema( CustomerAccessId, CswNbtObjClassUser.ChemSWAdminUsername, UserNode.UserId );

            return AuthenticationStatus;
        } // _doCswAdminAuthenticate()

        // Authenticates and sets up resources for an accessid and user
        private CswEnumAuthenticationStatus _authenticate( CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest )
        {
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;

            try
            {
                string ParsedAccessId = AuthenticationRequest.CustomerId.ToLower().Trim();
                if( !string.IsNullOrEmpty( ParsedAccessId ) )
                {
                    _CswSessionResources.CswSessionManager.setAccessId( ParsedAccessId );
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "There is no configuration information for this AccessId", "AccessId is null or empty." );
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
                    AuthenticationStatus = CswEnumAuthenticationStatus.NonExistentAccessId;
                }
            }

            if( AuthenticationStatus == CswEnumAuthenticationStatus.Unknown )
            {
                AuthenticationRequest.IpAddress = CswWebSvcCommonMethods.getIpAddress();
                AuthenticationStatus = _CswSessionResources.CswSessionManager.beginSession( AuthenticationRequest );
            }

            // case 21211
            if( AuthenticationStatus == CswEnumAuthenticationStatus.Authenticated )
            {
                // Removed for case 28617.  See case 28621.
                //// case 21036
                //if( IsMobile &&
                //    false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SI ) )
                //{
                //    AuthenticationStatus = CswEnumAuthenticationStatus.ModuleNotEnabled;
                //    _CswSessionResources.CswSessionManager.clearSession();
                //}
                CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
                if( LicenseManager.MustShowLicense( _CswNbtResources.CurrentUser ) )
                {
                    if( LicenseManager.AllowShowLicense( _CswNbtResources.CurrentUser ) )
                    {
                        // BZ 8133 - make sure they've seen the License
                        AuthenticationStatus = CswEnumAuthenticationStatus.ShowLicense;
                    }
                    else
                    {
                        // case 30086 - prevent login if admin hasn't accepted the license yet
                        AuthenticationStatus = CswEnumAuthenticationStatus.NoLicense;
                        _CswSessionResources.CswSessionManager.clearSession();
                    }
                }
                else if( _CswNbtResources.CurrentNbtUser.PasswordIsExpired )
                {
                    // BZ 9077 - Password expired
                    AuthenticationStatus = CswEnumAuthenticationStatus.ExpiredPassword;
                }
                else if( 1 < _CswNbtResources.CswSessionManager.SessionsList.getSessionCountForUser( _CswNbtResources.AccessId, _CswNbtResources.CurrentUser.Username )
                    && CswNbtObjClassUser.ChemSWAdminUsername != _CswNbtResources.CurrentUser.Username )
                {
                    AuthenticationStatus = CswEnumAuthenticationStatus.AlreadyLoggedIn;
                }
            }

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

                CswEnumAuthenticationStatus AuthenticationStatus = _doCswAdminAuthenticate( PropId );
                ReturnVal["username"] = CswNbtObjClassUser.ChemSWAdminUsername;
                ReturnVal["customerid"] = _CswNbtResources.AccessId;

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

                CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest = new CswWebSvcSessionAuthenticateData.Authentication.Request();
                AuthenticationRequest.CustomerId = AccessId;
                AuthenticationRequest.UserName = UserName;
                AuthenticationRequest.Password = Password;
                AuthenticationRequest.IsMobile = CswConvert.ToBoolean( ForMobile );
                CswEnumAuthenticationStatus AuthenticationStatus = _authenticate( AuthenticationRequest );

                if( AuthenticationStatus == CswEnumAuthenticationStatus.ExpiredPassword )
                {
                    ICswNbtUser CurrentUser = _CswNbtResources.CurrentNbtUser;
                    ReturnVal.Add( new JProperty( "nodeid", CurrentUser.UserId.ToString() ) );
                    CswNbtNodeKey FakeKey = new CswNbtNodeKey();
                    FakeKey.NodeId = CurrentUser.UserId;
                    FakeKey.NodeSpecies = CswEnumNbtNodeSpecies.Plain;
                    FakeKey.NodeTypeId = CurrentUser.UserNodeTypeId;
                    FakeKey.ObjectClassId = CurrentUser.UserObjectClassId;
                    ReturnVal.Add( new JProperty( "nodekey", FakeKey.ToString() ) );
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
        public string nbtManagerReauthenticate()
        {
            JObject ReturnVal = new JObject();
            try
            {
                _initResources();
                CswEnumAuthenticationStatus AuthenticationStatus = _attemptRefresh( true );

                // This is for the case where we login to a customer schema,
                // and then impersonate another user on that schema. We should still
                // be able to return to NBT Manager as long as chemsw_admin
                // was doing the impersonation
                if( _CswNbtResources.CswSessionManager.isImpersonating() )
                {
                    _CswNbtResources.CswSessionManager.endImpersonation();
                }

                if( _CswNbtResources.CurrentNbtUser.Username.Equals( CswNbtObjClassUser.ChemSWAdminUsername ) )
                {
                    // Return to NBT Manager schema and user who initially logged in
                    string NbtMgrAccessId = _CswNbtResources.CswSessionManager.getNbtMgrAccessId;
                    _CswNbtResources.AccessId = NbtMgrAccessId;

                    string NbtMgrUserName = _CswNbtResources.CswSessionManager.NbtMgrUserName;
                    CswPrimaryKey NbtMgrUserId = _CswNbtResources.CswSessionManager.NbtMgrUserId;
                    if( null != NbtMgrUserId )
                    {
                        CswNbtObjClassUser NbtMgrUserNode = _CswNbtResources.Nodes.GetNode( NbtMgrUserId );
                        if( null != NbtMgrUserNode )
                        {
                            // We want to clear the LastAccessId here because we are returning to the NBT Manager Schema
                            _CswNbtResources.CswSessionManager.changeSchema( NbtMgrAccessId, NbtMgrUserName, NbtMgrUserNode.UserId, ClearNbtMgrAccessId: true );

                            ReturnVal["username"] = NbtMgrUserName;
                            ReturnVal["customerid"] = _CswNbtResources.AccessId;
                        }
                    }
                }

                CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            return ( ReturnVal.ToString() );

        }

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
                CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, CswEnumAuthenticationStatus.Deauthenticated );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
                                throw new CswDniException( CswEnumErrorType.Warning,
                                                   "You do not have permission to use this feature.",
                                                   "User " + _CswNbtResources.CurrentNbtUser.Username + " attempted to impersonate userid " + UserId + " but lacked permission to do so." );
                            }
                        }
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Warning,
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        JArray UsersArray = new JArray();
                        CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
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
                        throw new CswDniException( CswEnumErrorType.Warning,
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
        public string getMainMenu( string ViewId, string SafeNodeKey, string NodeTypeId, string PropIdAttr, string LimitMenuTo, string ReadOnly, string NodeId )
        {

            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceMainMenu( _CswNbtResources, LimitMenuTo );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.getMenu( View, SafeNodeKey, CswConvert.ToInt32( NodeTypeId ), PropIdAttr, CswConvert.ToBoolean( ReadOnly ), NodeId );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        ReturnVal = ws.getDefaultContent( View );
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
        } // getDefaultContent()

        /// <summary>
        /// Get the display mode for a view
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewMode( string ViewId )
        {
            JObject ReturnVal = new JObject();

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
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
                    throw new CswDniException( CswEnumErrorType.Error, "The ViewId provided does not match a known view.", "ViewId: " + ViewId + " does not exist." );
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
        public string getRuntimeViewFilters( string ViewId, string ViewString )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View;
                    if( false == String.IsNullOrEmpty( ViewString ) )
                    {
                        View = new CswNbtView( _CswNbtResources );
                        View.LoadXml( ViewString );
                    }
                    else
                    {
                        View = _getView( ViewId );
                    }
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            return _prepGridView( ViewId, ref RealNodeKey, ref IsQuickLaunch, CswNbtNodeKey );
        }

        private CswNbtView _prepGridView( string ViewId, ref CswNbtNodeKey RealNodeKey, ref bool IsQuickLaunch, string CswNbtNodeKey = "", string NbtPrimaryKey = "" )
        {
            CswNbtView RetView = _getView( ViewId );
            if( null != RetView )
            {
                if( RetView.Visibility == CswEnumNbtViewVisibility.Property )
                {
                    CswPrimaryKey RealNodeId = null;
                    RealNodeKey = getNodeKey( CswNbtNodeKey );
                    if( null != RealNodeKey )
                    {
                        RealNodeId = RealNodeKey.NodeId;
                    }
                    if( null == RealNodeId )
                    {
                        RealNodeId = _getNodeId( NbtPrimaryKey );
                    }
                    if( null != RealNodeId )
                    {
                        ( RetView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Clear(); // case 21676. Clear() to avoid cache persistence.
                        ( RetView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( RealNodeId );
                        IsQuickLaunch = false;
                    }
                }

                foreach( CswNbtViewRelationship ChildRelationship in RetView.Root.ChildRelationships )
                {
                    _clearGroupBy( ChildRelationship );
                }
                if( RetView.SessionViewId.isSet() )
                {
                    RetView.SaveToCache( false );
                }
            }
            return RetView;
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getThinGrid( string ViewId, string NodeId, string MaxRows )
        {
            UseCompression();
            JObject ReturnVal = new JObject();
            bool IsQuickLaunch = false;

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                CswNbtView View = _prepGridView( ViewId, ref RealNodeKey, ref IsQuickLaunch, NbtPrimaryKey: NodeId );
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

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                CswNbtView View = _prepGridView( ViewId, ref RealNodeKey, ref IsQuickLaunch, SafeNodeKey );
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
        public string runGrid( string Title, string ViewId, string IncludeNodeKey, string IncludeNodeId, string IncludeInQuickLaunch, string ForReport )
        {
            UseCompression();
            JObject ReturnVal = new JObject();
            bool IsQuickLaunch = CswConvert.ToBoolean( IncludeInQuickLaunch );

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                CswNbtView View = _prepGridView( ViewId, ref RealNodeKey, ref IsQuickLaunch, IncludeNodeKey, IncludeNodeId );

                if( null != View )
                {
                    var ws = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey: RealNodeKey, ForReport: CswConvert.ToBoolean( ForReport ) );
                    ReturnVal = ws.runGrid( Title, IsQuickLaunch );
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
        public string getGridRowCount( string ViewId, string NodeId )
        {
            UseCompression();
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtNodeKey RealNodeKey = null;
                bool IsQuickLaunch = false;
                CswNbtView View = _prepGridView( ViewId, ref RealNodeKey, ref IsQuickLaunch, NbtPrimaryKey: NodeId );

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
        //public string getTableView( string ViewId, string NodeId, string NodeKey, string NodeTypeId )
        public string getTableView( string ViewId, string NodeTypeId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            UseCompression();
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, _CswNbtStatisticsEvents, CswConvert.ToInt32( NodeTypeId ) );
                        ReturnVal = wsTable.getTable( View );
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
        public string runTree( string ViewId, string IncludeNodeId, string IncludeNodeKey, bool IncludeNodeRequired, bool IncludeInQuickLaunch, string DefaultSelect )
        {
            UseCompression();
            JObject ReturnVal = new JObject();

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        var ws = new CswNbtWebServiceTree( _CswNbtResources, View );
                        CswPrimaryKey RealIncludeNodeId = _getNodeId( IncludeNodeId );

                        CswNbtNodeKey RealIncludeNodeKey = null;
                        if( !string.IsNullOrEmpty( IncludeNodeKey ) )
                            RealIncludeNodeKey = new CswNbtNodeKey( IncludeNodeKey );

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

        /// <summary>
        /// Generates a tree of nodes from the view
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getTreeOfNode( string NodePk )
        {
            JObject ReturnVal = new JObject();

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    if( string.Empty != NodePk )
                    {
                        CswPrimaryKey NodeId = _getNodeId( NodePk );
                        CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
                        CswNbtView View = Node.getNodeType().CreateDefaultView( false );
                        View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( NodeId );

                        var ws = new CswNbtWebServiceTree( _CswNbtResources, View );
                        ReturnVal = ws.runTree( null, null, false, true, "firstchild" );
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

        #endregion Render Core UI

        #region View Editing

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewGrid( bool All, string SelectedViewId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
        public string getViewInfo( string ViewId, string ViewString )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView View;
                    if( false == String.IsNullOrEmpty( ViewString ) )
                    {
                        View = new CswNbtView( _CswNbtResources );
                        View.LoadJson( ViewString );
                    }
                    else
                    {
                        View = _getView( ViewId );
                    }
                    if( null != View )
                    {
                        ReturnVal["view"] = View.ToJson();
                    }

                    CswNbtWebServiceView ws = new CswNbtWebServiceView( _CswNbtResources );
                    ReturnVal["viewlist"] = ws.getAllViewNames();
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

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
        public string copyView( string ViewId, string CopyToViewId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtView SourceView = _getView( ViewId );
                    if( null != SourceView )
                    {
                        CswNbtView CopyToView = _getView( CopyToViewId );
                        if( null != CopyToView )
                        {
                            CopyToView.CopyFromView( SourceView );

                            ReturnVal.Add( new JProperty( "copyviewid", CopyToView.ViewId.ToString() ) );

                        }
                        else
                        {
                            string NewViewNameOrig = SourceView.ViewName.Trim();
                            string Suffix = " Copy";

                            //Truncate to give us 10 extra characters
                            if( NewViewNameOrig.Length >= ( CswNbtView.ViewNameLength - 10 ) )
                            //We need enough space to append " Copy nnn"
                            {
                                NewViewNameOrig = NewViewNameOrig.Substring( 0, ( CswNbtView.ViewNameLength - 11 ) );
                            }

                            //Get a baseline ViewName
                            if( NewViewNameOrig.EndsWith( Suffix ) )
                            {
                                NewViewNameOrig = NewViewNameOrig.Substring( 0, NewViewNameOrig.Length - Suffix.Length );
                            }
                            else
                            {
                                //If we're copying a "copy n" view
                                CswCommaDelimitedString ViewNamePieces = new CswCommaDelimitedString();
                                string ParsedName = NewViewNameOrig.Replace( " ", "," );
                                ViewNamePieces.FromString( ParsedName );
                                if( Suffix != ViewNamePieces.Last() && ViewNamePieces.Contains( Suffix.Trim() ) )
                                {
                                    Int32 CopyNo = CswConvert.ToInt32( ViewNamePieces.Last() );
                                    if( Int32.MinValue != CopyNo )
                                    {
                                        //NewViewNameOrig = NewViewNameOrig.Substring( 0, ( ( NewViewNameOrig.Length - ( Suffix.Length + CopyNo.ToString().Length ) ) ) );
                                        if( NewViewNameOrig.EndsWith( Suffix + " " + CopyNo ) )
                                        {
                                            Int32 NSuffixLength = ( Suffix + " " + CopyNo ).Length;
                                            NewViewNameOrig = NewViewNameOrig.Substring( 0,
                                                                                         NewViewNameOrig.Length -
                                                                                         NSuffixLength );
                                        }
                                    }
                                }
                            }

                            //Now add the suffix
                            NewViewNameOrig = NewViewNameOrig + Suffix;
                            string NewViewName = NewViewNameOrig;

                            Int32 Increment = 1;
                            while( false ==
                                   CswNbtView.ViewIsUnique( _CswNbtResources, new CswNbtViewId(), NewViewName,
                                                            SourceView.Visibility, SourceView.VisibilityUserId,
                                                            SourceView.VisibilityRoleId ) )
                            {
                                //I oppose this while() loop.
                                Increment++;
                                NewViewName = NewViewNameOrig + " " + Increment.ToString();
                            }

                            CswNbtView NewView = new CswNbtView( _CswNbtResources );
                            NewView.saveNew( NewViewName, SourceView.Visibility, SourceView.VisibilityRoleId,
                                             SourceView.VisibilityUserId, SourceView );
                            //NewView.save();

                            ReturnVal.Add( new JProperty( "copyviewid", NewView.ViewId.ToString() ) );
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

        } // copyView()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string deleteView( string ViewId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {

                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {

                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswEnumNbtViewVisibility RealVisibility = (CswEnumNbtViewVisibility) Visibility;
                    CswPrimaryKey RealVisibilityRoleId = null;
                    CswPrimaryKey RealVisibilityUserId = null;
                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        if( RealVisibility == CswEnumNbtViewVisibility.Role )
                        {
                            RealVisibilityRoleId = _getNodeId( VisibilityRoleId );
                        }
                        else if( RealVisibility == CswEnumNbtViewVisibility.User )
                        {
                            RealVisibilityUserId = _getNodeId( VisibilityUserId );
                        }
                    }
                    else
                    {
                        RealVisibility = CswEnumNbtViewVisibility.User;
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
                        NewView.ViewMode = ViewMode;
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
        public string getProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId, string Date, string filterToPropId, string Multi, string ConfigMode, string RelatedNodeId, string ForceReadOnly )
        {
            CswTimer GetPropsTimer = new CswTimer();

            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents, CswConvert.ToBoolean( Multi ), CswConvert.ToBoolean( ConfigMode ) );
                    _setEditMode( EditMode );
                    CswDateTime InDate = new CswDateTime( _CswNbtResources );
                    InDate.FromClientDateTimeString( Date );
                    CswNbtNodeKey NodeKey = getNodeKey( SafeNodeKey );
                    Int32 NodeTypePk = CswConvert.ToInt32( NodeTypeId );
                    if( null != NodeKey && Int32.MinValue == NodeTypePk )
                    {
                        NodeTypePk = NodeKey.NodeTypeId;
                    }
                    ReturnVal = ws.getProps( NodeId, SafeNodeKey, TabId, NodeTypePk, filterToPropId, RelatedNodeId, CswConvert.ToBoolean( ForceReadOnly ), InDate );
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
        public string getIdentityTabProps( string EditMode, string NodeId, string SafeNodeKey, //string NodeTypeId, 
            string Date, string filterToPropId, string Multi, string ConfigMode, string RelatedNodeId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents, CswConvert.ToBoolean( Multi ), CswConvert.ToBoolean( ConfigMode ) );
                    _setEditMode( EditMode );
                    CswDateTime InDate = new CswDateTime( _CswNbtResources );
                    InDate.FromClientDateTimeString( Date );

                    CswPrimaryKey RealNodeId = _getNodeId( NodeId );
                    if( false == CswTools.IsPrimaryKey( RealNodeId ) )
                    {
                        CswNbtNodeKey RealNodeKey = getNodeKey( SafeNodeKey );
                        if( null != RealNodeKey )
                        {
                            RealNodeId = RealNodeKey.NodeId;
                        }
                    }

                    ReturnVal = ws.getIdentityTabProps( RealNodeId, filterToPropId, RelatedNodeId, InDate );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getIdentitTabProps()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSingleProp( string EditMode, string NodeId, string SafeNodeKey, string PropId, string NodeTypeId, string NewPropJson )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();

                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
                            PropType = CswEnumNbtViewPropType.NodeTypePropId.ToString();
                        }
                        else if( Type == "ObjectClassId" )
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( nId );
                            Props = ObjectClass.getObjectClassProps();
                            PropType = CswEnumNbtViewPropType.ObjectClassPropId.ToString();
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
        public string saveProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NewPropsJson, string IdentityTabJson, string NodeTypeId, string ViewId, bool RemoveTempStatus )
        {
            JObject ReturnVal = new JObject();

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswPrimaryKey NodePk = _getNodeId( NodeId );
                    if( null == NodePk )
                    {
                        CswNbtNodeKey NbtNodeKey = getNodeKey( SafeNodeKey );
                        if( null != NbtNodeKey )
                        {
                            NodePk = NbtNodeKey.NodeId;
                        }
                    }

                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    _setEditMode( EditMode );
                    CswNbtView View = _getView( ViewId );

                    JObject NewPropsObj = CswConvert.ToJObject( NewPropsJson );

                    // To prevent the same property from saving twice (and from triggering business logic twice), 
                    // we need to collapse the Identity tab propJson into the regular tab propJson.
                    if( false == string.IsNullOrEmpty( IdentityTabJson ) &&
                        IdentityTabJson != "null" ) //null can be deserialized to string
                    {
                        JObject IdentityPropsObj = CswConvert.ToJObject( IdentityTabJson );
                        foreach( JProperty RootIdentityProp in IdentityPropsObj.Children() )
                        {
                            if( false == NewPropsJson.Contains( RootIdentityProp.Name ) )
                            {
                                NewPropsObj.Add( RootIdentityProp );
                            }
                        }
                    }

                    ReturnVal = ws.saveProps( NodePk, CswConvert.ToInt32( TabId ), NewPropsObj, CswConvert.ToInt32( NodeTypeId ), View, IsIdentityTab: false, RemoveTempStatus: RemoveTempStatus );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // saveMaterial()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getBlob()
        {
            JObject ReturnVal = new JObject();

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
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

        #endregion Tabs and Props

        #region MetaData

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getObjectClasses()
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
                    ReturnVal = ws.getObjectClasses();
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getObjectClasses()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getNodeTypes( string PropertySetName, string ObjectClassName, string ObjectClassId, string ExcludeNodeTypeIds, string RelatedToNodeTypeId, string RelatedObjectClassPropName, string RelationshipNodeTypePropId, string FilterToPermission, string Searchable )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                int OCId = CswConvert.ToInt32( ObjectClassId );
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtMetaDataPropertySet PropertySet = null;
                    CswNbtMetaDataObjectClass ObjectClass = null;
                    Int32 realRelationshipNodeTypePropId = Int32.MinValue;
                    if( false == string.IsNullOrEmpty( RelationshipNodeTypePropId ) )
                    {
                        CswPropIdAttr propAttr = new CswPropIdAttr( RelationshipNodeTypePropId );
                        realRelationshipNodeTypePropId = propAttr.NodeTypePropId;
                    }
                    else if( false == string.IsNullOrEmpty( ObjectClassName ) )
                    {
                        CswEnumNbtObjectClass OC = ObjectClassName;
                        if( CswNbtResources.UnknownEnum != OC )
                        {
                            ObjectClass = _CswNbtResources.MetaData.getObjectClass( OC );
                        }
                    }
                    else if( Int32.MinValue != OCId )
                    {
                        ObjectClass = _CswNbtResources.MetaData.getObjectClass( OCId );
                    }
                    else if( false == string.IsNullOrEmpty( PropertySetName ) )
                    {
                        CswEnumNbtPropertySetName PS = PropertySetName;
                        if( CswNbtResources.UnknownEnum != PS )
                        {
                            PropertySet = _CswNbtResources.MetaData.getPropertySet( PS );
                        }
                    }
                    var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
                    ReturnVal = ws.getNodeTypes( PropertySet, ObjectClass, ExcludeNodeTypeIds, CswConvert.ToInt32( RelatedToNodeTypeId ), RelatedObjectClassPropName, realRelationshipNodeTypePropId, FilterToPermission, CswConvert.ToBoolean( Searchable ) );
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
        public string getNodeTypeTabs( string NodeTypeName, string NodeTypeId, string FilterToPermission )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
                    ReturnVal = ws.getNodeTypeTabs( NodeTypeName, NodeTypeId, FilterToPermission );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );
            return ReturnVal.ToString();
        } // getNodeTypeTabs()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string IsNodeTypeNameUnique( string NodeTypeName )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Ignore;
            try
            {
                _initResources();

                if( _CswNbtResources.SetupVbls.doesSettingExist( CswEnumSetupVariableNames.Watermark ) )
                {
                    string Watermark = _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.Watermark];
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
        public string getAbout()
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
        public string getObjectClassButtons( string ObjectClassId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.getObjectClassButtons( ObjectClassId );
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
        } // getObjectClassButtons()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getLocationView( string NodeId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.getLocationView( NodeId );
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
        } // getLocationView()

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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswEnumNbtLayoutType RealLayoutType = LayoutType;
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

            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswEnumNbtLayoutType RealLayoutType = LayoutType;
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

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string doUniversalSearch( string SearchTerm, string SearchType, string NodeTypeId, string ObjectClassId, string Page, string Limit )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    ReturnVal = ws.doUniversalSearch( SearchTerm, (CswEnumSqlLikeMode) SearchType, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( ObjectClassId ), CswConvert.ToInt32( Page ), CswConvert.ToInt32( Limit ) );
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
        public string restoreUniversalSearch( string SessionDataId, string Limit )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );

                    CswPrimaryKey Pk = CswConvert.ToPrimaryKey( SessionDataId );
                    if( CswTools.IsPrimaryKey( Pk ) && Pk.TableName == CswNbtSearchManager.SearchTableName )
                    {
                        ReturnVal = ws.restoreUniversalSearch( Pk, CswConvert.ToInt32( Limit ) );
                    }
                    else
                    {
                        CswNbtSessionDataId RealSessionDataId = new CswNbtSessionDataId( SessionDataId );
                        ReturnVal = ws.restoreUniversalSearch( RealSessionDataId, CswConvert.ToInt32(Limit) );
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
        } // restoreUniversalSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string filterUniversalSearch( string SessionDataId, string Filter, string Action, string Limit )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtSessionDataId RealSessionDataId = new CswNbtSessionDataId( SessionDataId );
                    ReturnVal = ws.filterUniversalSearch( RealSessionDataId, JObject.Parse( Filter ), Action, Convert.ToInt32( Limit ) );
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
        public string filterUniversalSearchByNodeType( string SessionDataId, string NodeTypeId, string Limit )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtSessionDataId RealSessionDataId = new CswNbtSessionDataId( SessionDataId );
                    ReturnVal = ws.filterUniversalSearchByNodeType( RealSessionDataId, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( Limit ) );
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
        public string saveSearch( string SessionDataId, string Name, string Category, string Limit )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswNbtSessionDataId RealSessionDataId = new CswNbtSessionDataId( SessionDataId );
                    ReturnVal = ws.saveSearch( RealSessionDataId, Name, Category, CswConvert.ToInt32( Limit ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // saveSearch()
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]

        public string deleteSearch( string SearchId, string Limit )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceSearch ws = new CswNbtWebServiceSearch( _CswNbtResources, _CswNbtStatisticsEvents );
                    CswPrimaryKey RealSearchId = new CswPrimaryKey( CswNbtSearchManager.SearchTableName, CswConvert.ToInt32( SearchId ) );
                    ReturnVal = ws.deleteSearch( RealSearchId, CswConvert.ToInt32( Limit ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // saveSearch()

        #endregion Search

        #region Node DML

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string DeleteNodes( string[] NodePks, string[] NodeKeys )
        {
            JObject ret = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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

        //[WebMethod( EnableSession = false )]
        //[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        //public string DeleteDemoDataNodes()
        //{
        //    JObject ReturnVal = new JObject();
        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh( true );

        //        if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
        //        {
        //            CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
        //            ReturnVal = ws.deleteDemoDataNodes();
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
        public string CopyNode( string NodeId, string NodeKey )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswPrimaryKey RealNodePk = _getNodeId( NodeId );
                    if( null == RealNodePk )
                    {
                        CswNbtNodeKey RealNodeKey = getNodeKey( NodeKey );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
        public string onObjectClassButtonClick( string NodeTypePropAttr, string SelectedText, string TabIds, string Props, string EditMode, string NodeIds, string PropIds )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPropIdAttr PropId = new CswPropIdAttr( NodeTypePropAttr );
                if( null == PropId.NodeId ||
                    Int32.MinValue == PropId.NodeId.PrimaryKey ||
                    Int32.MinValue == PropId.NodeTypePropId )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Cannot execute a button click without valid parameters.", "Attempted to call OnObjectClassButtonClick with invalid NodeId and NodeTypePropId." );
                }

                CswEnumNbtNodeEditMode NodeEditMode = EditMode;
                _CswNbtResources.EditMode = NodeEditMode;

                CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                ReturnVal = ws.doObjectClassButtonClick( PropId, SelectedText, TabIds, CswConvert.ToJObject( Props ), NodeIds, PropIds );

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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtSdTabsAndProps tabsandprops = new CswNbtSdTabsAndProps( _CswNbtResources );

                CswNbtMetaDataNodeType feedbackNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( nodetypeid ) );

                CswNbtNodeCollection.AfterMakeNode After = delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassFeedback newFeedbackNode = NewNode;
                        //if we have an action this is all we want/need/care about
                        if( false == String.IsNullOrEmpty( actionname ) )
                        {
                            newFeedbackNode.Action.Text = actionname.Replace( "%20", " " );
                        }
                        else //if we DONT have an action, we want the info required to load a view
                        {
                            if( false == String.IsNullOrEmpty( viewid ) )
                            {
                                CswNbtViewId CurrentViewId = new CswNbtViewId( viewid );

                                CswNbtView cookieView = _getView( viewid ); //this view doesn't exist in the the DB, which is why we save it below

                                CswNbtView view = _CswNbtResources.ViewSelect.restoreView( newFeedbackNode.View.ViewId ); //WARNING!!!! calling View.ViewId creates a ViewId if there isn't one!
                                view.LoadXml( cookieView.ToXml() );
                                view.ViewId = newFeedbackNode.View.ViewId; //correct view.ViewId because of above problem.
                                view.ViewName = cookieView.ViewName; //same as above, but name
                                view.Visibility = CswEnumNbtViewVisibility.Hidden; // see case 26799
                                view.save();
                            }
                            newFeedbackNode.SelectedNodeId.Text = selectednodeid;
                            newFeedbackNode.CurrentViewMode.Text = viewmode;
                        }
                        //newFeedbackNode.postChanges( false );
                    };

                CswNbtObjClassFeedback ret = tabsandprops.getAddNodeAndPostChanges( feedbackNT, After );

                ReturnVal["propdata"] = tabsandprops.getProps( ret.Node, "", null, CswEnumNbtLayoutType.Add ); //DO I REALLY BREAK THIS?
                ReturnVal["nodeid"] = ret.NodeId.ToString();

                _deInitResources();
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, Connected, CswEnumAuthenticationStatus.Authenticated, IsMobile: true );
            return ( Connected.ToString() );
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string ConnectTestDb()
        {
            JObject Connected = new JObject();

            // init resources



            CswNbtResources myResources = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, CswEnumSetupMode.NbtWeb, true, new CswSuperCycleCacheWeb( Context.Cache ) );
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

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, Connected, CswEnumAuthenticationStatus.Authenticated, IsMobile: true );
            //_jAddAuthenticationStatus( Connected, AuthenticationStatus.Authenticated );  // we don't want to trigger session timeouts
            return ( Connected.ToString() );

        } // ConnectTestDb()

        public ICswUser ConnectTestDb_InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswEnumSystemUserNames.SysUsr_DbConnectTest );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtAction Action = _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionName )];
                    if( null != Action && Action.Name != CswResources.UnknownEnum )
                    {
                        _CswNbtResources.SessionDataMgr.saveSessionData( Action, true );
                        ReturnVal = new JObject( new JProperty( "succeeded", "true" ) );
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
        } // SaveActionToQuickLaunch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getGeneratorsTree()
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtNode node = _CswNbtResources.Nodes[_getNodeId( nodeId )];
                    if( null != node )
                    {
                        if( node.getObjectClass().ObjectClass == CswEnumNbtObjectClass.FeedbackClass )
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
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, CswEnumAuthenticationStatus.Authenticated );

            return ReturnVal.ToString();
        } // GetFeedbackCaseNumber


        #region Nbt Manager

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getActiveAccessIds()
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources, CswEnumNbtActionName.View_Scheduled_Rules );
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

        #endregion Nbt Manager

        #region CISPro

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveMaterial( string NodeTypeId, string SupplierId, string Suppliername, string Tradename, string PartNo, string NodeId, string IsConstituent, bool CorporateSupplier = false )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceCreateMaterial ws = new CswNbtWebServiceCreateMaterial( _CswNbtResources, _CswNbtStatisticsEvents );
                ReturnVal = ws.saveMaterial( CswConvert.ToInt32( NodeTypeId ), SupplierId, Suppliername, Tradename, PartNo, NodeId, CswConvert.ToBoolean( IsConstituent ), CorporateSupplier );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // saveMaterial()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSizeNodeProps( string SizeDefinition, string SizeNodeTypeId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
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
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSizeLogicalsVisibility( string SizeNodeTypeId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceCreateMaterial ws = new CswNbtWebServiceCreateMaterial( _CswNbtResources, _CswNbtStatisticsEvents );
                _setEditMode( CswEnumNbtNodeEditMode.Edit );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                _setEditMode( CswEnumNbtNodeEditMode.Add );
                CswNbtActReceiving Receiving = new CswNbtActReceiving( _CswNbtResources );
                ReturnVal = Receiving.receiveMaterial( ReceiptDefinition );

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
        public string getMaterialUnitsOfMeasure( string PhysicalStateValue )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                ReturnVal = CswNbtWebServiceCreateMaterial.getMaterialUnitsOfMeasure( PhysicalStateValue, _CswNbtResources );

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
        public string getRequestItemGrid( string SessionViewId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( _CswNbtResources );
                ReturnVal = ws.getRequestViewGrid( SessionViewId );

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = CswWebSvcCommonMethods.jError( _CswNbtResources, Ex );
            }

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getCurrentRequest()

        #endregion Requesting

        #region Auditing

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getAuditHistoryGrid( string NodeId, string NbtNodeKey, string JustDateColumn )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceAuditing ws = new CswNbtWebServiceAuditing( _CswNbtResources );
                    CswPrimaryKey RealNodeId = _getNodeId( NodeId );
                    if( null == RealNodeId )
                    {
                        CswNbtNodeKey RealNodeKey = getNodeKey( NbtNodeKey );
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
            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, _CswSessionResources, RetJson, CswEnumAuthenticationStatus.Authenticated );
            return ( RetJson.ToString() );
        } // RunView()
        #endregion test

        #region Quotas

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getQuotas()
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceQuotas( _CswNbtResources );
                    double realQuota = ws.GetHighestQuotaPercent();
                    int roundedQuota = (int) Math.Round( realQuota );
                    if( realQuota > 0 && roundedQuota == 0 )
                    {
                        roundedQuota = 1;
                    }
                    ReturnVal["result"] = roundedQuota;
                    ReturnVal["showquota"] = ws.IsQuotaSet();
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

        #region Inspection Design

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string finalizeInspectionDesign( string DesignGrid, string InspectionDesignName, string InspectionTargetName, string IsNewInspection, string IsNewTarget, string Category )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;

            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( string.IsNullOrEmpty( InspectionDesignName ) )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Inspection Name cannot be blank.", "InspectionName was null or empty." );
                    }
                    if( string.IsNullOrEmpty( InspectionTargetName ) )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "New Inspection must have a target.", "InspectionTarget was null or empty." );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            DataTable ExcelDataTable = null;
            string ErrorMessage = string.Empty;
            string WarningMessage = string.Empty;

            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
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
                        throw new CswDniException( CswEnumErrorType.Warning, "Could not read Excel file.", ErrorMessage );
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( CswEnumAuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceContainer ws = new CswNbtWebServiceContainer( _CswNbtResources );
                    if( DispenseType.Contains( CswEnumNbtContainerDispenseType.Dispense.ToString() ) && false == String.IsNullOrEmpty( DesignGrid ) )
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
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
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
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
        public string convertUnit( string ValueToConvert, string OldUnitId, string NewUnitId, string MaterialId )
        {
            JObject ReturnVal = new JObject();
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh( true );

                CswPrimaryKey OldUnitPk = CswConvert.ToPrimaryKey( OldUnitId );
                CswPrimaryKey NewUnitPk = CswConvert.ToPrimaryKey( NewUnitId );
                CswPrimaryKey MaterialPk = CswConvert.ToPrimaryKey( MaterialId );
                CswNbtUnitConversion Conversion = new CswNbtUnitConversion( _CswNbtResources, OldUnitPk, NewUnitPk, MaterialPk );
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

        private void _setEditMode( CswEnumNbtNodeEditMode EditMode )
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

        public static CswNbtNodeKey getNodeKey( string NodeKeyString )
        {
            CswNbtNodeKey RetKey = null;
            CswNbtNodeKey TryKey = null;
            if( false == string.IsNullOrEmpty( NodeKeyString ) )
            {
                TryKey = new CswNbtNodeKey( NodeKeyString );
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

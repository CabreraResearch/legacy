using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Web.Services;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using ChemSW.NbtWebControls;
using ChemSW.Security;
using ChemSW.Session;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// NBT Web service interface
    /// </summary>
    /// 
    [ScriptService]
    [WebService( Namespace = "http://localhost/NbtWebApp" )]
    [WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
    public class wsNBT : System.Web.Services.WebService
    {
        #region Session and Resource Management

        private CswSessionResourcesNbt _CswSessionResources;
        private CswNbtResources _CswNbtResources;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

        /// <summary>
        /// These are files we do NOT want to keep around after temporarily using them.  There is a function that purges old files.  
        /// </summary>
        private string _TempPath
        {
            get
            {
                // ApplicationPhysicalPath already has \\ at the end
                return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "temp" );
            }
        }

        private void _initResources()
        {
            _CswSessionResources = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, string.Empty, SetupMode.NbtWeb );
            _CswNbtResources = _CswSessionResources.CswNbtResources;
            _CswNbtStatisticsEvents = _CswSessionResources.CswNbtStatisticsEvents;
            _CswNbtResources.beginTransaction();

            _CswNbtResources.logMessage( "WebServices: Session Started (_initResources called)" );

        }//_initResources() 

        private AuthenticationStatus _attemptRefresh()
        {
            AuthenticationStatus ret = _CswSessionResources.attemptRefresh();

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
            return ret;
        } // _attemptRefresh()

        private void _deInitResources()
        {
            _CswSessionResources.endSession();
            if( _CswNbtResources != null )
            {
                _CswNbtResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                _CswNbtResources.finalize();
                _CswNbtResources.release();
            }
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
                            ReturnVal[Entry.SessionId] = JSession;
                        } // if (Entry.AccessId == Master.AccessID)
                    } // foreach (CswAuthenticator.SessionListEntry Entry in SessionList.Values)
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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

        private void _jAddAuthenticationStatus( JObject JObj, AuthenticationStatus AuthenticationStatusIn, bool ForMobile = false )
        {
            if( JObj != null )
            {
                JObj.Add( new JProperty( "AuthenticationStatus", AuthenticationStatusIn.ToString() ) );
                if( _CswSessionResources != null &&
                    _CswSessionResources.CswSessionManager != null &&
                    !ForMobile )
                {
                    JObj.Add( new JProperty( "timeout", _CswSessionResources.CswSessionManager.TimeoutDate.ToString() ) );
                }
            }
        }//_jAuthenticationStatus()

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
            return Ret;

        }

        #endregion Error Handling

        #region Web Methods

        private static readonly string _IDPrefix = string.Empty;

        #region Authentication

        // Authenticates and sets up resources for an accessid and user
        private AuthenticationStatus _authenticate( string AccessId, string UserName, string Password, bool IsMobile )
        {
            AuthenticationStatus AuthenticationStatus = ChemSW.Security.AuthenticationStatus.Unknown;
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
                AuthenticationStatus = _CswSessionResources.CswSessionManager.beginSession( UserName, Password, CswWebControls.CswNbtWebTools.getIpAddress(), IsMobile );

            // case 21211
            if( AuthenticationStatus == AuthenticationStatus.Authenticated )
            {
                // case 21036
                if( IsMobile && false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                {
                    AuthenticationStatus = AuthenticationStatus.ModuleNotEnabled;
                    _CswSessionResources.CswSessionManager.clearSession();
                }
                CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
                //Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "passwordexpiry_days" ) );

                if( _CswNbtResources.CurrentNbtUser.PasswordProperty.IsExpired )
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
        public string authenticate( string AccessId, string UserName, string Password, string ForMobile )
        {
            JObject ReturnVal = new JObject();
            try
            {
                _initResources();

                bool IsMobile = CswConvert.ToBoolean( ForMobile );
                AuthenticationStatus AuthenticationStatus = _authenticate( AccessId, UserName, Password, IsMobile );

                if( AuthenticationStatus == ChemSW.Security.AuthenticationStatus.ExpiredPassword )
                {
                    CswNbtObjClassUser CurrentUser = _CswNbtResources.CurrentNbtUser.UserNode;
                    ReturnVal.Add( new JProperty( "nodeid", CurrentUser.NodeId.ToString() ) );
                    CswNbtNodeKey FakeKey = new CswNbtNodeKey( _CswNbtResources );
                    FakeKey.NodeId = CurrentUser.NodeId;
                    FakeKey.NodeSpecies = CurrentUser.Node.NodeSpecies;
                    FakeKey.NodeTypeId = CurrentUser.NodeTypeId;
                    FakeKey.ObjectClassId = CurrentUser.ObjectClass.ObjectClassId;
                    ReturnVal.Add( new JProperty( "cswnbtnodekey", wsTools.ToSafeJavaScriptParam( FakeKey.ToString() ) ) );
                    CswPropIdAttr PasswordPropIdAttr = new CswPropIdAttr( CurrentUser.Node, CurrentUser.PasswordProperty.NodeTypeProp );
                    ReturnVal.Add( new JProperty( "passwordpropid", PasswordPropIdAttr.ToString() ) );
                }

                if( AuthenticationStatus == AuthenticationStatus.Authenticated ||
                    AuthenticationStatus == AuthenticationStatus.ExpiredPassword ||
                    AuthenticationStatus == AuthenticationStatus.ShowLicense )
                {
                    // initial quick launch setup
                    CswNbtWebServiceQuickLaunchItems wsQL = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources );
                    wsQL.initQuickLaunchItems();
                }

                _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus, IsMobile );
                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            return ( ReturnVal.ToString() );
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
                _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus.Deauthenticated );
                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ( ReturnVal.ToString() );

        }//RenewSession()

        #endregion Authentication

        #region Render Core UI


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getQuickLaunchItems()
        {
            //XElement ReturnVal = new XElement( "quicklaunch" );
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswPrimaryKey UserId = _CswNbtResources.CurrentNbtUser.UserId;
                    var Ws = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources ); //, new CswWebClientStorageCookies( Context.Request, Context.Response ) ); // , Session );
                    if( null != UserId )
                    {
                        ReturnVal = Ws.getQuickLaunchItems();
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getQuickLaunchItems()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewTree( bool IsSearchable, bool UseSession )
        {
            JObject ReturnVal = new JObject();
            //XElement ReturnVal = new XElement( "viewtree" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceView( _CswNbtResources );
                    ReturnVal = ws.getViewTree( IsSearchable );
                    _deInitResources();
                }
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getViews()

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

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                    ReturnVal = ws.getHeaderMenu();
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getHeaderMenu()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getMainMenu( string ViewId, string SafeNodeKey, string PropIdAttr )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceMainMenu( _CswNbtResources );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.getMenu( View, SafeNodeKey, PropIdAttr );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();


        } // getMainMenu()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getGrid( string ViewId, string SafeNodeKey, string ShowEmpty, string IsReport )
        {
            JObject ReturnVal = new JObject();
            string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    bool ShowEmptyGrid = CswConvert.ToBoolean( ShowEmpty );
                    bool ForReporting = CswConvert.ToBoolean( IsReport );
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        CswNbtNodeKey ParentNodeKey = null;
                        if( !string.IsNullOrEmpty( ParsedNodeKey ) )
                        {
                            ParentNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
                        }
                        var g = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey );
                        ReturnVal = g.getGrid( ShowEmptyGrid, ForReporting );
                        //CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
                        View.SaveToCache( true );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getGrid()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public void getGridRows( string ViewId, string SafeNodeKey, string ShowEmpty )
        {
            JObject ReturnVal = new JObject();
            string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    bool ShowEmptyGrid = CswConvert.ToBoolean( ShowEmpty );
                    CswNbtView View = _getView( ViewId );
                    if( null != View )
                    {
                        CswNbtNodeKey ParentNodeKey = null;
                        if( !string.IsNullOrEmpty( ParsedNodeKey ) )
                        {
                            ParentNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
                        }
                        var g = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey );
                        ReturnVal = g.getGridRows( ShowEmptyGrid );

                        View.SaveToCache( true );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.AddHeader( "content-disposition", "attachment; filename=export.json" );
            Context.Response.Flush();
            Context.Response.Write( ReturnVal.ToString() );

        } // getGrid()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getTable( string ViewId, string NodeId, string NodeKey )
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
                        CswNbtNode Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
                        CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources );
                        ReturnVal = wsTable.getTable( View, Node );
                        View.SaveToCache( true );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getGrid()

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
                        var ws = new CswNbtWebServiceTree( _CswNbtResources );

                        CswNbtNodeKey RealParentNodeKey = null;
                        if( !string.IsNullOrEmpty( ParentNodeKey ) )
                            RealParentNodeKey = new CswNbtNodeKey( _CswNbtResources, wsTools.FromSafeJavaScriptParam( ParentNodeKey ) );

                        CswNbtNodeKey RealIncludeNodeKey = null;
                        if( !string.IsNullOrEmpty( IncludeNodeKey ) )
                            RealIncludeNodeKey = new CswNbtNodeKey( _CswNbtResources, wsTools.FromSafeJavaScriptParam( IncludeNodeKey ) );

                        ReturnVal = ws.getTree( View, IdPrefix, IsFirstLoad, RealParentNodeKey, RealIncludeNodeKey, IncludeNodeRequired, UsePaging, ShowEmptyTree, ForSearch, IncludeInQuickLaunch );

                        //CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
                        //View.SaveToCache(true);
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                        CswPrimaryKey NodeId = new CswPrimaryKey();
                        NodeId.FromString( NodePk );
                        CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
                        CswNbtView View = Node.NodeType.CreateDefaultView();
                        View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( NodeId );

                        var ws = new CswNbtWebServiceTree( _CswNbtResources );
                        ReturnVal = ws.getTree( View, IdPrefix, true, null, null, false, false, false, false, true );
                        //CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
                        View.SaveToCache( true );
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();


        } // getTreeOfNode()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getNodes( string NodeTypeId, string ObjectClassId, string ObjectClass )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    Int32 RealNodeTypeId = CswConvert.ToInt32( NodeTypeId );
                    Int32 RealObjectClassId = CswConvert.ToInt32( ObjectClassId );
                    CswNbtMetaDataObjectClass.NbtObjectClass RealObjectClass = CswNbtMetaDataObjectClass.NbtObjectClass.Unknown;
                    Enum.TryParse<CswNbtMetaDataObjectClass.NbtObjectClass>( ObjectClass, true, out RealObjectClass );

                    Collection<CswNbtNode> Nodes = new Collection<CswNbtNode>();
                    if( RealNodeTypeId != Int32.MinValue )
                    {
                        CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( RealNodeTypeId );
                        Nodes = MetaDataNodeType.getNodes( true, false );
                    }
                    else
                    {
                        CswNbtMetaDataObjectClass MetaDataObjectClass = null;
                        if( RealObjectClassId != Int32.MinValue )
                        {
                            MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClassId );
                        }
                        else if( RealObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.Unknown )
                        {
                            MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClass );
                        }
                        if( null != MetaDataObjectClass )
                        {
                            Nodes = MetaDataObjectClass.getNodes( true, false );
                        }
                    }

                    foreach( CswNbtNode Node in Nodes )
                    {
                        ReturnVal.Add(
                            new JProperty( Node.NodeId.ToString(), Node.NodeName )
                        );
                    }

                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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

                        NewView.makeNew( NewViewName, SourceView.Visibility, SourceView.VisibilityRoleId, SourceView.VisibilityUserId, SourceView );
                        NewView.save();
                        ReturnVal.Add( new JProperty( "copyviewid", NewView.ViewId.ToString() ) );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
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
                        DoomedView.Delete();
                        ReturnVal.Add( new JProperty( "succeeded", true ) );
                    }
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // deleteView()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string createView( string ViewName, string ViewMode, string Visibility, string VisibilityRoleId, string VisibilityUserId, string ViewId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {

                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    NbtViewVisibility RealVisibility = NbtViewVisibility.Unknown;
                    CswPrimaryKey RealVisibilityRoleId = null;
                    CswPrimaryKey RealVisibilityUserId = null;
                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        Enum.TryParse<NbtViewVisibility>( Visibility, out RealVisibility );
                        if( RealVisibility == NbtViewVisibility.Role )
                        {
                            RealVisibilityRoleId = new CswPrimaryKey();
                            RealVisibilityRoleId.FromString( VisibilityRoleId );
                        }
                        else if( RealVisibility == NbtViewVisibility.User )
                        {
                            RealVisibilityUserId = new CswPrimaryKey();
                            RealVisibilityUserId.FromString( VisibilityUserId );
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
                    NewView.makeNew( ViewName, RealVisibility, RealVisibilityRoleId, RealVisibilityUserId, CopyView );

                    if( ViewMode != string.Empty )
                    {
                        NbtViewRenderingMode RealViewMode = NbtViewRenderingMode.Unknown;
                        Enum.TryParse<NbtViewRenderingMode>( ViewMode, out RealViewMode );
                        NewView.ViewMode = RealViewMode;
                    }

                    NewView.save();
                    ReturnVal.Add( new JProperty( "newviewid", NewView.ViewId.ToString() ) );
                }

                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // createView()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getViewPropFilterUI( string ViewJson, string PropArbitraryId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new wsViewBuilder( _CswNbtResources );
                    ReturnVal = ws.getVbProp( ViewJson, PropArbitraryId );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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

                    var ws = new wsViewBuilder( _CswNbtResources );
                    ReturnVal = ws.makeViewPropFilter( ViewJson, PropFiltJson );
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        #endregion View Editing

        #region Tabs and Props

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getTabs( string EditMode, string NodeId, string SafeNodeKey, string NodeTypeId, string Date, string filterToPropId, string Multi )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, CswConvert.ToBoolean( Multi ) );
                    NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
                    CswDateTime InDate = new CswDateTime( _CswNbtResources );
                    InDate.FromClientDateTimeString( Date );
                    ReturnVal = ws.getTabs( RealEditMode, NodeId, ParsedNodeKey, CswConvert.ToInt32( NodeTypeId ), InDate, filterToPropId );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getTabs()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId, string Date, string filterToPropId, string Multi )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, CswConvert.ToBoolean( Multi ) );
                    NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
                    CswDateTime InDate = new CswDateTime( _CswNbtResources );
                    InDate.FromClientDateTimeString( Date );
                    ReturnVal = ws.getProps( RealEditMode, NodeId, ParsedNodeKey, TabId, CswConvert.ToInt32( NodeTypeId ), InDate, filterToPropId );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                    string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
                    //if( !string.IsNullOrEmpty( ParsedNodeKey ) )
                    //{
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
                    ReturnVal = ws.getSingleProp( RealEditMode, NodeId, ParsedNodeKey, PropId, CswConvert.ToInt32( NodeTypeId ), NewPropJson );
                    //}
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                        ICollection Props = null;
                        string PropType = string.Empty;
                        if( Type == "NodeTypeId" )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( nId );
                            Props = NodeType.NodeTypeProps;
                            PropType = CswNbtViewProperty.CswNbtPropType.NodeTypePropId.ToString();
                        }
                        else if( Type == "ObjectClassId" )
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( nId );
                            Props = ObjectClass.ObjectClassProps;
                            PropType = CswNbtViewProperty.CswNbtPropType.ObjectClassPropId.ToString();
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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getPropNames()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveProps( string EditMode, string NodeIds, string SafeNodeKeys, string TabId, string NewPropsJson, string NodeTypeId, string ViewId )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswCommaDelimitedString ParsedNodeKeys = new CswCommaDelimitedString();
                    ParsedNodeKeys.FromString( wsTools.FromSafeJavaScriptParam( SafeNodeKeys ) );
                    CswCommaDelimitedString ParsedNodeIds = new CswCommaDelimitedString();
                    ParsedNodeIds.FromString( NodeIds );
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.saveProps( RealEditMode, ParsedNodeIds, ParsedNodeKeys, CswConvert.ToInt32( TabId ), NewPropsJson, CswConvert.ToInt32( NodeTypeId ), View );
                }
                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // saveProps()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string copyPropValues( string SourceNodeKey, string[] CopyNodeIds, string[] PropIds )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    string ParsedSourceNodeKey = wsTools.FromSafeJavaScriptParam( SourceNodeKey );
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    bool ret = ws.copyPropValues( ParsedSourceNodeKey, CopyNodeIds, PropIds );
                    ReturnVal.Add( new JProperty( "succeeded", ret ) );
                }

                _deInitResources();

            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // copyPropValue()	

        #endregion Tabs and Props

        #region MetaData

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getNodeTypes( string ObjectClassName )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtMetaDataObjectClass ObjectClass = null;
                    if( false == string.IsNullOrEmpty( ObjectClassName ) )
                    {
                        CswNbtMetaDataObjectClass.NbtObjectClass OC;
                        Enum.TryParse( ObjectClassName, true, out OC );
                        if( CswNbtMetaDataObjectClass.NbtObjectClass.Unknown != OC )
                        {
                            ObjectClass = _CswNbtResources.MetaData.getObjectClass( OC );
                        }

                    }
                    var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
                    ReturnVal = ws.getNodeTypes( ObjectClass );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getNodeTypes()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string IsNodeTypeNameUnique( string NewInspectionName )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ReturnVal["succeeded"] = wsTools.IsNodeTypeNameUnique( NewInspectionName, _CswNbtResources );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // IsNodeTypeNameUnique()s

        #endregion MetaData

        #region Misc

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
                    ReturnVal = ws.makeVersionJson();
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string fileForProp()
        {
            //Come back to implement Multi
            JObject ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ) );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    // putting these in the param list causes the webservice to fail with
                    // "System.InvalidOperationException: Request format is invalid: application/octet-stream"
                    string FileName = Context.Request["qqfile"];
                    string PropId = Context.Request["propid"];
                    string Column = Context.Request["column"];
                    string Multi = Context.Request["multi"];

                    if( !string.IsNullOrEmpty( FileName ) && !string.IsNullOrEmpty( PropId ) )
                    {
                        // Unfortunately, Context.Request.ContentType is always application/octet-stream
                        // So we have to detect the content type
                        string[] SplitFileName = FileName.Split( '.' );
                        string Extension = SplitFileName[SplitFileName.Length - 1];
                        string ContentType = "application/" + Extension;
                        switch( Extension )
                        {
                            case "jpg":
                            case "jpeg":
                                ContentType = "image/pjpeg";
                                break;
                            case "gif":
                                ContentType = "image/gif";
                                break;
                            case "png":
                                ContentType = "image/png";
                                break;
                        }

                        if( Context.Request.InputStream != null )
                        {
                            // Read the binary data
                            BinaryReader br = new BinaryReader( Context.Request.InputStream );
                            long Length = Context.Request.InputStream.Length;
                            byte[] FileData = new byte[Length];
                            for( long CurrentIndex = 0; CurrentIndex < Length; CurrentIndex++ )
                            {
                                FileData[CurrentIndex] = br.ReadByte();
                            }

                            // Save the binary data
                            CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                            bool ret = ws.SetPropBlobValue( FileData, FileName, ContentType, PropId, Column );

                            ReturnVal = new JObject( new JProperty( "success", ret.ToString().ToLower() ) );

                        } // if( Context.Request.InputStream != null )

                    } // if( FileName != string.Empty && PropId != string.Empty )

                }
                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // fileForProp()



        //dch
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveMolPropFile()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    // putting these in the param list causes the webservice to fail with
                    // "System.InvalidOperationException: Request format is invalid: application/octet-stream"
                    string FileName = Context.Request["qqfile"];
                    string PropId = Context.Request["propid"];

                    if( false == string.IsNullOrEmpty( FileName ) && false == string.IsNullOrEmpty( PropId ) )
                    {
                        // Read the binary data
                        BinaryReader br = new BinaryReader( Context.Request.InputStream );
                        long Length = Context.Request.InputStream.Length;
                        byte[] FileData = new byte[Length];
                        for( long CurrentIndex = 0; CurrentIndex < Length; CurrentIndex++ )
                        {
                            FileData[CurrentIndex] = br.ReadByte();
                        }

                        // Save the binary data
                        CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                        string MolData = CswTools.ByteArrayToString( FileData ).Replace( "\r", "" );
                        bool Success = ws.saveMolProp( MolData, PropId );

                        ReturnVal["success"] = Success;
                        if( Success )
                        {
                            ReturnVal["molData"] = MolData;
                        }

                    } // if( FileName != string.Empty && PropId != string.Empty )

                }
                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
                ReturnVal["success"] = false;
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // saveMolPropFile()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveMolProp( string molData, string PropId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( false == string.IsNullOrEmpty( molData ) && false == string.IsNullOrEmpty( PropId ) )
                    {
                        CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                        bool Succeeded = ws.saveMolProp( molData, PropId );
                        ReturnVal["success"] = Succeeded;
                        if( Succeeded )
                        {
                            ReturnVal["molData"] = molData;
                        }

                    } // if( FileName != string.Empty && PropId != string.Empty )

                }
                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
                ReturnVal["success"] = false;
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // saveMolProp()





        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getLabels( string PropId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtWebServicePrintLabels ws = new CswNbtWebServicePrintLabels( _CswNbtResources );
                    ReturnVal = ws.getLabels( PropId );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getEPLText( string PropId, string PrintLabelNodeId )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtWebServicePrintLabels ws = new CswNbtWebServicePrintLabels( _CswNbtResources );
                    ReturnVal.Add( new JProperty( "epl", ws.getEPLText( PropId, PrintLabelNodeId ) ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        #endregion Misc

        #region NodeType Layout

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string MoveProp( string PropId, string NewRow, string NewColumn, string EditMode )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
                    bool ret = ws.moveProp( PropId, CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ), RealEditMode );
                    ReturnVal.Add( new JProperty( "moveprop", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // MoveProp()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string removeProp( string PropId, string EditMode )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
                    bool ret = ws.removeProp( PropId, RealEditMode );
                    ReturnVal.Add( new JProperty( "removeprop", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // removeProp()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getPropertiesForLayoutAdd( string NodeId, string NodeKey, string NodeTypeId, string TabId, string EditMode )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    NodeEditMode RealEditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditMode );
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Unknown;
                    switch( RealEditMode )
                    {
                        case NodeEditMode.AddInPopup: LayoutType = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add; break;
                        case NodeEditMode.Preview: LayoutType = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview; break;
                        default: LayoutType = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit; break;
                    }
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    ReturnVal = ws.getPropertiesForLayoutAdd( NodeId, NodeKey, NodeTypeId, TabId, LayoutType );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getPropertiesForLayoutAdd()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string addPropertyToLayout( string PropId, string TabId, string EditMode )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( EditMode );

                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    bool ret = ws.addPropertyToLayout( PropId, TabId, LayoutType );
                    ReturnVal.Add( new JProperty( "result", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // addPropertyToLayout()

        #endregion NodeType Layout

        #region Search

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getClientSearchJson( string ViewId, string SelectedNodeTypeIdNum, string IdPrefix, string NodeKey )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceSearch( _CswNbtResources );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.getSearchJson( View, SelectedNodeTypeIdNum, NodeKey );
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getNodeTypeSearchProps( string RelatedIdType, string NodeTypeOrObjectClassId, string IdPrefix, string NodeKey )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceSearch( _CswNbtResources );
                    ReturnVal = ( ws.getSearchProps( RelatedIdType, NodeTypeOrObjectClassId, NodeKey ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getSearchableViews( string IsMobile, string OrderBy )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    ICswNbtUser UserId = _CswNbtResources.CurrentNbtUser;
                    bool ForMobile = CswConvert.ToBoolean( IsMobile );
                    var ws = new CswNbtWebServiceView( _CswNbtResources );
                    //SearchNode =  ws.getSearchableViewTree( UserId, ForMobile, true, OrderBy ); 
                }

                _deInitResources();

            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            return ReturnVal.ToString();

        } // getSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string doViewSearch( object SearchJson )
        {
            JObject ReturnVal = new JObject();

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceSearch( _CswNbtResources );
                    CswNbtViewSearchPair SearchPair = ws.doViewBasedSearch( SearchJson );
                    if( null != SearchPair )
                    {
                        ReturnVal.Add( new JProperty( "parentviewid", SearchPair.ParentViewId ) );
                        ReturnVal.Add( new JProperty( "searchviewid", SearchPair.SearchViewId ) );
                        ReturnVal.Add( new JProperty( "viewmode", SearchPair.ViewMode.ToString().ToLower() ) );
                    }
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string doNodeTypeSearch( object SearchJson )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceSearch( _CswNbtResources );
                    CswNbtViewSearchPair SearchPair = ws.doNodesSearch( SearchJson );
                    if( null != SearchPair )
                    {
                        ReturnVal.Add( new JProperty( "parentviewid", SearchPair.ParentViewId ) );
                        ReturnVal.Add( new JProperty( "searchviewid", SearchPair.SearchViewId ) );
                        ReturnVal.Add( new JProperty( "viewmode", SearchPair.ViewMode.ToString().ToLower() ) );
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }

        #endregion Search

        #region Node DML

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string DeleteNodes( string[] NodePks, string[] NodeKeys )
        {
            JObject ReturnVal = new JObject();
            List<CswPrimaryKey> NodePrimaryKeys = new List<CswPrimaryKey>();
            bool ret = true;
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( NodeKeys.Length > 0 )
                    {
                        foreach( string NodeKey in NodeKeys )
                        {
                            string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( NodeKey );
                            CswNbtNodeKey NbtNodeKey = null;
                            if( !string.IsNullOrEmpty( ParsedNodeKey ) )
                            {
                                NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
                                if( null != NbtNodeKey.NodeId )
                                {
                                    NodePrimaryKeys.Add( NbtNodeKey.NodeId );
                                }
                            }
                        }
                    }
                    if( NodePks.Length > 0 )
                    {
                        foreach( string NodePk in NodePks )
                        {
                            CswPrimaryKey PrimaryKey = new CswPrimaryKey();
                            PrimaryKey.FromString( NodePk );
                            if( PrimaryKey.PrimaryKey != Int32.MinValue && !NodePrimaryKeys.Contains( PrimaryKey ) )
                            {
                                NodePrimaryKeys.Add( PrimaryKey );
                            }
                        }
                    }
                    if( NodePrimaryKeys.Count > 0 )
                    {
                        foreach( CswPrimaryKey Npk in NodePrimaryKeys )
                        {
                            CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                            ret = ret && ws.DeleteNode( Npk );
                        }
                    }

                    ReturnVal.Add( new JProperty( "Succeeded", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        }


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string CopyNode( string NodePk )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswPrimaryKey RealNodePk = new CswPrimaryKey();
                    RealNodePk.FromString( NodePk );
                    if( RealNodePk.PrimaryKey != Int32.MinValue )
                    {
                        CswNbtWebServiceNode ws = new CswNbtWebServiceNode( _CswNbtResources, _CswNbtStatisticsEvents );
                        CswPrimaryKey NewNodePk = ws.CopyNode( RealNodePk );
                        if( NewNodePk != null )
                        {
                            ReturnVal.Add( new JProperty( "NewNodeId", NewNodePk.ToString() ) );
                        }
                        else
                        {
                            ReturnVal.Add( new JProperty( "NewNodeId", "" ) );
                        }
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    bool ret = ws.ClearPropValue( PropId, CswConvert.ToBoolean( IncludeBlob ) );
                    ReturnVal = new JObject( new JProperty( "Succeeded", ret.ToString().ToLower() ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // clearProp()

        #endregion Node DML

        #region Welcome Region


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getWelcomeItems( string RoleId )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
                    // Only administrators can get welcome content for other roles
                    if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    {
                        ReturnVal = ws.GetWelcomeItems( RoleId );
                    }
                    else
                    {
                        ReturnVal = ws.GetWelcomeItems( _CswNbtResources.CurrentNbtUser.RoleId.ToString() );
                    }
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();


        } // getWelcomeItems()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getWelcomeButtonIconList()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
                    ReturnVal = ws.getButtonIconList();
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getWelcomeButtonIconList()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string addWelcomeItem( string RoleId, string Type, string ViewType, string ViewValue, string NodeTypeId, string Text, string IconFileName )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
                    // Only administrators can add welcome content to other roles
                    string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
                    if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                        UseRoleId = RoleId;
                    CswNbtWebServiceWelcomeItems.WelcomeComponentType ComponentType = (CswNbtWebServiceWelcomeItems.WelcomeComponentType) Enum.Parse( typeof( CswNbtWebServiceWelcomeItems.WelcomeComponentType ), Type );
                    CswViewListTree.ViewType RealViewType = CswViewListTree.ViewType.Unknown;
                    if( ViewType != string.Empty )
                    {
                        RealViewType = (CswViewListTree.ViewType) Enum.Parse( typeof( CswViewListTree.ViewType ), ViewType, true );
                    }
                    ws.AddWelcomeItem( ComponentType, RealViewType, ViewValue, CswConvert.ToInt32( NodeTypeId ), Text, Int32.MinValue, Int32.MinValue, IconFileName, UseRoleId );
                    ReturnVal.Add( new JProperty( "Succeeded", true ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // addWelcomeItem()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string deleteWelcomeItem( string RoleId, string WelcomeId )
        {
            bool ret = false;
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
                    // Only administrators can add welcome content to other roles
                    string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
                    if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                        UseRoleId = RoleId;
                    ret = ws.DeleteWelcomeItem( UseRoleId, CswConvert.ToInt32( WelcomeId ) );
                    //ReturnVal = "{ \"Succeeded\": \"" + ret.ToString().ToLower() + "\" }";
                    ReturnVal.Add( "Succeeded", ret );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // deleteWelcomeItem()



        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string moveWelcomeItems( string RoleId, string WelcomeId, string NewRow, string NewColumn )
        {
            bool ret = false;
            //string ReturnVal = string.Empty;
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceWelcomeItems ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
                    // Only administrators can move welcome content for other roles
                    string UseRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
                    if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                        UseRoleId = RoleId;
                    ret = ws.MoveWelcomeItems( UseRoleId, CswConvert.ToInt32( WelcomeId ), CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ) );
                    //ReturnVal = "{ \"Succeeded\": \"" + ret.ToString().ToLower() + "\" }";
                    ReturnVal.Add( "Succeeded", ret );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // moveWelcomeItems()

        #endregion Welcome Region

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
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
            _jAddAuthenticationStatus( Connected, AuthenticationStatus.Authenticated );  // we don't want to trigger session timeouts
            return ( Connected.ToString() );
        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string ConnectTestDb()
        {
            JObject Connected = new JObject();

            // init resources
            CswNbtResources myResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtWeb, true, false );
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

            _jAddAuthenticationStatus( Connected, AuthenticationStatus.Authenticated );  // we don't want to trigger session timeouts
            return ( Connected.ToString() );
        } // ConnectTestDb()

        public ICswUser ConnectTestDb_InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, "ConnectTestDb" );
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
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // SaveActionToQuickLaunch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getInspectionStatusGrid()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceInspections ws = new CswNbtWebServiceInspections( _CswNbtResources );
                    ReturnVal = ws.getInspectionStatusGrid();
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getInspectionStatusGrid()

        #endregion Actions

        #region Mobile
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string UpdateProperties( string SessionId, string ParentId, string UpdatedViewJson, bool ForMobile )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceMobile wsM = new CswNbtWebServiceMobile( _CswNbtResources, ForMobile );
                    bool CompletedNodes = wsM.updateViewProps( UpdatedViewJson );
                    ReturnVal = wsM.getNode( ParentId );
                    if( CompletedNodes )
                    {
                        ReturnVal.Add( new JProperty( "completed", true ) );
                    }
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus, ForMobile );

            return ReturnVal.ToString();

        } // UpdateNodeProps()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetViewsList( string SessionId, string ParentId, bool ForMobile )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ICswNbtUser CurrentUser = _CswNbtResources.CurrentNbtUser;
                    if( null != CurrentUser )
                    {
                        CswNbtWebServiceMobile wsView = new CswNbtWebServiceMobile( _CswNbtResources, ForMobile );
                        ReturnVal = wsView.getViewsList( ParentId, CurrentUser );
                    }
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus.Authenticated, ForMobile );

            return ReturnVal.ToString();
        } // GetViews()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetView( string SessionId, string ParentId, bool ForMobile )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ICswNbtUser CurrentUser = _CswNbtResources.CurrentNbtUser;
                    if( null != CurrentUser )
                    {
                        CswNbtWebServiceMobile wsView = new CswNbtWebServiceMobile( _CswNbtResources, ForMobile );
                        ReturnVal = wsView.getView( ParentId, CurrentUser );
                    }
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus.Authenticated, ForMobile );

            return ReturnVal.ToString();
        } // GetViews()

        #endregion Mobile

        #region Auditing

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getAuditHistoryGrid( string NodeId, string JustDateColumn )
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
                    CswPrimaryKey RealNodeId = new CswPrimaryKey();
                    RealNodeId.FromString( NodeId );
                    ReturnVal = ws.getAuditHistoryGrid( _CswNbtResources.Nodes[RealNodeId], CswConvert.ToBoolean( JustDateColumn ) );
                }
                _deInitResources();
            }
            catch( Exception Ex )
            {
                ReturnVal = jError( Ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getInspectionStatusGrid()


        #endregion Auditing

        #region test
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetTestData()
        {
            JObject RetJson = new JObject( new JProperty( "A", "Static Page 1" ), new JProperty( "B", "Static Page 2" ), new JProperty( "C", "Dynamic Page A" ), new JProperty( "D", "Dynamic Page B" ) );
            _jAddAuthenticationStatus( RetJson, AuthenticationStatus.Authenticated );
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

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }
            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }
            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

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
                    ReturnVal["result"] = Math.Round( ws.GetQuotaPercent() ).ToString();
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }
            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getQuotaPercent()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string checkQuota( string NodeTypeId )
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
                    ReturnVal["result"] = ws.CheckQuota( CswConvert.ToInt32( NodeTypeId ) ).ToString().ToLower();
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }
            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        } // getQuotaPercent()


        #endregion Quotas

        #region Inspection Design

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string uploadInspectionFile( string NewInspectionName, string TargetName, string TempFileName )
        {
            JObject ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ) );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            DataTable ExcelDataTable = null;
            string ErrorMessage = string.Empty;
            string WarningMessage = string.Empty;
            int NumRowsImported = 0;

            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    if( !string.IsNullOrEmpty( TempFileName ) )
                    {
                        if( !string.IsNullOrEmpty( NewInspectionName ) )
                        {
                            if( !string.IsNullOrEmpty( TargetName ) )
                            {
                                string FullPathAndFileName = _TempPath + "\\" + TempFileName;

                                // Load the excel file into a data table
                                CswWebServiceInspectionDesign ws = new CswWebServiceInspectionDesign( _CswNbtResources );
                                ExcelDataTable = ws.ConvertExcelFileToDataTable( FullPathAndFileName, ref ErrorMessage, ref WarningMessage );
                                if( ( ExcelDataTable != null ) && ( string.IsNullOrEmpty( ErrorMessage ) ) )
                                {
                                    NumRowsImported = ws.CreateNodes( ExcelDataTable, NewInspectionName, TargetName, ref ErrorMessage, ref WarningMessage );

                                    ReturnVal = new JObject( new JProperty( "success", true.ToString().ToLower() ) );

                                    if( !string.IsNullOrEmpty( WarningMessage ) )
                                        ReturnVal.Add( new JProperty( "error", WarningMessage ) );

                                }
                                else
                                {
                                    if( string.IsNullOrEmpty( ErrorMessage ) )
                                        ErrorMessage = "Could not read Excel file.";
                                    ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ), new JProperty( "error", ErrorMessage ) );
                                }
                            } // if( Context.Request.InputStream != null )
                            else
                            {
                                ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ), new JProperty( "error", "Did not receive target name." ) );
                            }
                        } // if (!string.IsNullOrEmpty(FileName))
                        else
                        {
                            ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ), new JProperty( "error", "Did not receive new inspection name." ) );
                        }
                    } // if (!string.IsNullOrEmpty(FileName))
                    else
                    {
                        ReturnVal = new JObject( new JProperty( "success", false.ToString().ToLower() ), new JProperty( "error", "Did not receive temp file name." ) );
                    }
                } // if (AuthenticationStatus.Authenticated == AuthenticationStatus)
                _deInitResources();
            } // try
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // uploadInspectionFile()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string previewInspectionFile()
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
                    // putting these in the param list causes the webservice to fail with
                    // "System.InvalidOperationException: Request format is invalid: application/octet-stream"
                    // These variables seem to work in Google chrome but NOT in IE
                    string FileName = Context.Request["qqfile"];
                    //string PropId = Context.Request["propid"];
                    string NewInspectionName = Context.Request["InspectionName"];

                    if( string.IsNullOrEmpty( FileName ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "Invalid file name for Inspection Import.", "Imported file name was null or empty." );
                    }
                    if( string.IsNullOrEmpty( NewInspectionName ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "Inpsection Name is required.", "Inspection name was null or empty." );
                    }
                    if( 0 == Context.Request.InputStream.Length || false == Context.Request.InputStream.CanRead )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot read the loaded file.", "File was empty or corrupt" );
                    }

                    string TempFileName = "excelupload_" + _CswNbtResources.CurrentUser.Username + "_" + DateTime.Now.ToString( "MMddyyyy_HHmmss" ) + ".xls";
                    string FullPathAndFileName = _TempPath + "\\" + TempFileName;
                    // upload user file to temporary file
                    // our Excel file reader only likes to read files from disk - does not read files from memory or stream
                    using( FileStream OutputFile = File.Create( FullPathAndFileName ) )
                    {
                        Context.Request.InputStream.CopyTo( OutputFile );
                    }

                    // Load the excel file into a data table
                    CswWebServiceInspectionDesign ws = new CswWebServiceInspectionDesign( _CswNbtResources );
                    ExcelDataTable = ws.ConvertExcelFileToDataTable( FullPathAndFileName, ref ErrorMessage, ref WarningMessage );

                    // determine if we were successful or failure
                    if( ExcelDataTable == null || false == string.IsNullOrEmpty( ErrorMessage ) )
                    {
                        if( string.IsNullOrEmpty( ErrorMessage ) )
                        {
                            ErrorMessage = "Could not read Excel file.";
                        }
                        throw new CswDniException( ErrorType.Warning, "Could not read Excel file.", ErrorMessage );
                    }

                    ReturnVal["success"] = "true";
                    ReturnVal["tempFileName"] = TempFileName;

                    ws.AddPrimaryKeys( ref ExcelDataTable );
                    CswGridData gd = new CswGridData( _CswNbtResources );
                    gd.PkColumn = "RowNumber";

                    ReturnVal["jqGridOpt"] = gd.DataTableToJSON( ExcelDataTable, true );

                    if( false == string.IsNullOrEmpty( WarningMessage ) )
                    {
                        ReturnVal["error"] = WarningMessage;
                    }

                    _purgeTempFiles( "xls" );

                } // if (AuthenticationStatus.Authenticated == AuthenticationStatus)
                _deInitResources();
            } // try
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }
            return ReturnVal.ToString();
        } // uploadInspectionFile()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getInspectionPointGroups( string InspectionTargetName )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {


                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();
        }

        #endregion Inspection Design

        #endregion Web Methods

        #region Private

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

        /// <summary>  Purge files in the temporary directory  </summary>
        /// <param name="FileExtension">  Optional extension type of files to purge.  Default is to purge all files  </param>
        /// <param name="HoursToKeepFiles">  Optional number of hours to keep temporary files around.  Default is 12 hours  </param>
        private void _purgeTempFiles( string FileExtension = ".*", int HoursToKeepFiles = 12 )
        {
            DirectoryInfo myDirectoryInfo = new DirectoryInfo( _TempPath );
            FileInfo[] myFileInfoArray = myDirectoryInfo.GetFiles();

            FileExtension = FileExtension.ToLower().Trim();
            if( !FileExtension.StartsWith( "." ) )
            {
                FileExtension = "." + FileExtension;
            }
            foreach( FileInfo myFileInfo in myFileInfoArray )
            {
                if( ( FileExtension == "*" ) || ( myFileInfo.Extension.ToString().ToLower() == FileExtension ) )
                {
                    if( DateTime.Now.Subtract( myFileInfo.CreationTime ).TotalHours > HoursToKeepFiles )
                    {
                        myFileInfo.Delete();
                    }
                }
            }
        }

        #endregion Private
    }//wsNBT

} // namespace ChemSW.WebServices

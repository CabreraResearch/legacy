using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using ChemSW.NbtWebControls;
using ChemSW.Security;
using Newtonsoft.Json;
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

        private string _FilesPath
        {
            get
            {
                return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
            }
        }

        private void _initResources()
        {
            _CswSessionResources = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );
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
                    Display = ( _CswNbtResources.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Display = ( _CswNbtResources.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }
            }

            Type = newEx.Type;
            Message = newEx.MsgFriendly;
            Detail = newEx.MsgEscoteric + "; " + ex.StackTrace;
        } // _error()



        /*
         * The two _xAddAuthenticationStatus() methods must _not_ add the authentication status as a element. 
         * I tried that, and it turned out that in the JQuery world the code that displays an xml tree
         * ends up seeing the authentication node even if it is a peer of the tree and not in the tree. 
         * Please trust me: we're talking major whackadelia. But it works fine as an attribute. 
         */
        private void _xAddAuthenticationStatus( XElement XElement, AuthenticationStatus AuthenticationStatusIn )
        {
            if( XElement != null )
            {
                XElement.SetAttributeValue( "authenticationstatus", AuthenticationStatusIn.ToString() );
                if( _CswSessionResources != null && _CswSessionResources.CswSessionManager != null )
                    XElement.SetAttributeValue( "timeout", _CswSessionResources.CswSessionManager.TimeoutDate.ToString() );
            }
        }//_xAuthenticationStatus()


        private void _xAddAuthenticationStatus( XmlDocument XmlDocument, AuthenticationStatus AuthenticationStatusIn )
        {
            if( XmlDocument != null )
            {
                if( XmlDocument.DocumentElement == null )
                    CswXmlDocument.SetDocumentElement( XmlDocument, "root" );
                CswXmlDocument.AppendXmlAttribute( XmlDocument.DocumentElement, "authenticationstatus", AuthenticationStatusIn.ToString() );
                if( _CswSessionResources != null && _CswSessionResources.CswSessionManager != null )
                    CswXmlDocument.AppendXmlAttribute( XmlDocument.DocumentElement, "timeout", _CswSessionResources.CswSessionManager.TimeoutDate.ToString() );
            }
        }//_xAuthenticationStatus()

        private void _jAddAuthenticationStatus( JObject JObj, AuthenticationStatus AuthenticationStatusIn )
        {
            if( JObj != null )
            {
                JObj.Add( new JProperty( "AuthenticationStatus", AuthenticationStatusIn.ToString() ) );
                if( _CswSessionResources != null && _CswSessionResources.CswSessionManager != null )
                    JObj.Add( new JProperty( "timeout", _CswSessionResources.CswSessionManager.TimeoutDate.ToString() ) );
            }
        }//_jAuthenticationStatus()



        /// <summary>
        /// Returns error as XmlDocument
        /// </summary>
        private XmlDocument xmlError( Exception ex )
        {
            string Message = string.Empty;
            string Detail = string.Empty;
            ErrorType Type = ErrorType.Error;
            bool Display = true;
            _error( ex, out Type, out Message, out Detail, out Display );

            XmlDocument ErrorXmlDoc = new XmlDocument();
            CswXmlDocument.SetDocumentElement( ErrorXmlDoc, "error" );
            CswXmlDocument.AppendXmlAttribute( ErrorXmlDoc.DocumentElement, "display", Display.ToString().ToLower() );
            CswXmlDocument.AppendXmlAttribute( ErrorXmlDoc.DocumentElement, "type", Type.ToString() );
            CswXmlDocument.AppendXmlAttribute( ErrorXmlDoc.DocumentElement, "message", Message );
            CswXmlDocument.AppendXmlAttribute( ErrorXmlDoc.DocumentElement, "detail", Detail );
            return ErrorXmlDoc;
        }

        /// <summary>
        /// Returns error as XElement
        /// </summary>
        private XElement _xError( Exception ex )
        {
            string Message = string.Empty;
            string Detail = string.Empty;
            ErrorType Type = ErrorType.Error;
            bool Display = true;
            _error( ex, out Type, out Message, out Detail, out Display );

            return new XElement( "error",
                new XAttribute( "display", Display.ToString().ToLower() ),
                new XAttribute( "type", Type.ToString() ),
                new XAttribute( "message", Message ),
                new XAttribute( "detail", Detail ) );
        }

        /// <summary>
        /// Returns error as JProperty
        /// </summary>
        private JObject jError( Exception ex )
        {
            string Message = string.Empty;
            string Detail = string.Empty;
            ErrorType Type = ErrorType.Error;
            bool Display = true;
            _error( ex, out Type, out Message, out Detail, out Display );

            return new JObject(
                new JProperty( "error",
                        new JObject(
                            new JProperty( "display", Display.ToString().ToLower() ),
                            new JProperty( "type", Type.ToString() ),
                            new JProperty( "message", Message ),
                            new JProperty( "detail", Detail ) ) ) );
        }

        #endregion Error Handling

        #region Web Methods

        private static readonly string _IDPrefix = string.Empty;

        #region Authentication

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string authenticate( string AccessId, string UserName, string Password, string ForMobile )
        {
            JObject ReturnVal = new JObject();
            try
            {
                _initResources();

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
                bool IsMobile = CswConvert.ToBoolean( ForMobile );
                if( AuthenticationStatus == AuthenticationStatus.Unknown )
                    AuthenticationStatus = _CswSessionResources.CswSessionManager.beginSession( UserName, Password, CswWebControls.CswNbtWebTools.getIpAddress(), IsMobile );

                // case 21211
                if( AuthenticationStatus == AuthenticationStatus.Authenticated )
                {
                    CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
                    Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.getConfigVariableValue( "passwordexpiry_days" ) );

                    if( _CswNbtResources.CurrentNbtUser.PasswordProperty.ChangedDate == DateTime.MinValue ||
                        _CswNbtResources.CurrentNbtUser.PasswordProperty.ChangedDate.AddDays( PasswordExpiryDays ).Date <= DateTime.Now.Date )
                    {
                        // BZ 9077 - Password expired
                        AuthenticationStatus = AuthenticationStatus.ExpiredPassword;
                        ReturnVal.Add( new JProperty( "nodeid", _CswNbtResources.CurrentNbtUser.UserNode.NodeId.ToString() ) );
                        CswNbtNodeKey FakeKey = new CswNbtNodeKey( _CswNbtResources );
                        FakeKey.NodeId = _CswNbtResources.CurrentNbtUser.UserNode.NodeId;
                        FakeKey.NodeSpecies = _CswNbtResources.CurrentNbtUser.UserNode.Node.NodeSpecies;
                        FakeKey.NodeTypeId = _CswNbtResources.CurrentNbtUser.UserNode.NodeTypeId;
                        FakeKey.ObjectClassId = _CswNbtResources.CurrentNbtUser.UserNode.ObjectClass.ObjectClassId;
                        ReturnVal.Add( new JProperty( "cswnbtnodekey", wsTools.ToSafeJavaScriptParam( FakeKey.ToString() ) ) );
                        CswPropIdAttr PasswordPropIdAttr = new CswPropIdAttr( _CswNbtResources.CurrentNbtUser.UserNode.Node, _CswNbtResources.CurrentNbtUser.PasswordProperty.NodeTypeProp );
                        ReturnVal.Add( new JProperty( "passwordpropid", PasswordPropIdAttr.ToString() ) );
                    }
                    else if( LicenseManager.MustShowLicense( _CswNbtResources.CurrentUser ) )
                    {
                        // BZ 8133 - make sure they've seen the License
                        AuthenticationStatus = AuthenticationStatus.ShowLicense;
                    }

                    // initial quick launch setup
                    CswNbtWebServiceQuickLaunchItems wsQL = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources );
                    wsQL.initQuickLaunchItems();
                }

                _CswSessionResources.purgeExpiredSessions(); //bury the overhead of nuking old sessions in the overhead of authenticating

                _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
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

        #endregion Authentication

        #region Render Core UI


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getQuickLaunchItems()
        {
            XElement ReturnVal = new XElement( "quicklaunch" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswPrimaryKey UserId = _CswNbtResources.CurrentNbtUser.UserId;
                    var ws = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources ); //, new CswWebClientStorageCookies( Context.Request, Context.Response ) ); // , Session );
                    if( null != UserId )
                    {
                        ReturnVal.Add( ws.getQuickLaunchItems() );
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getQuickLaunchItems()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getViewTree( bool IsSearchable, bool UseSession )
        {
            XElement ReturnVal = new XElement( "viewtree" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceView( _CswNbtResources );
                    //ReturnVal = XElement.Parse( ws.getViewTree( Session, IsSearchable, UseSession ) );
                    ReturnVal = XElement.Parse( ws.getViewTree( IsSearchable ) );
                    _deInitResources();
                }
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getViews()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getDashboard()
        {
            XElement ReturnVal = new XElement( "dashboard" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceHeader( _CswNbtResources );
                    ReturnVal = XElement.Parse( ws.getDashboard() );
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;


        } // getDashboard()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getHeaderMenu()
        {
            XElement ReturnVal = new XElement( "header" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceHeader( _CswNbtResources );
                    ReturnVal = XElement.Parse( ws.getHeaderMenu() );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getHeaderMenu()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getMainMenu( string ViewId, string SafeNodeKey, string PropIdAttr )
        {
            XElement ReturnVal = new XElement( "menu" );
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
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;


        } // getMainMenu()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getGrid( string ViewId, string SafeNodeKey, string ShowEmpty )
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
                        ReturnVal = g.getGrid( ShowEmptyGrid );
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

        /// <summary>
        /// Generates a tree of nodes from the view
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getTreeOfView( string ViewId, string IDPrefix, bool IsFirstLoad, string ParentNodeKey, string IncludeNodeKey, bool IncludeNodeRequired,
                                       bool UsePaging, string ShowEmpty, bool ForSearch, bool IncludeInQuickLaunch )
        {
            XElement ReturnVal = new XElement( "tree" );

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

                        ReturnVal = ws.getTree( View, IDPrefix, IsFirstLoad, RealParentNodeKey, RealIncludeNodeKey, IncludeNodeRequired, UsePaging, ShowEmptyTree, ForSearch, IncludeInQuickLaunch );

                        //CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
                        //View.SaveToCache(true);
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getTreeOfView()

        /// <summary>
        /// Generates a tree of nodes from the view
        /// </summary>
        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getTreeOfNode( string IDPrefix, string NodePk )
        {
            XElement ReturnVal = new XElement( "tree" );

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
                        ReturnVal = ws.getTree( View, IDPrefix, true, null, null, false, false, false, false, true );
                        //CswNbtWebServiceQuickLaunchItems.addToQuickLaunch( View ); //, Session );
                        View.SaveToCache( true );
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;


        } // getTreeOfNode()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getNodes( string NodeTypeId, string ObjectClassId, string ObjectClass )
        {
            XElement ReturnVal = new XElement( "nodes" );

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

                    Collection<CswNbtNode> Nodes = null;
                    if( RealNodeTypeId != Int32.MinValue )
                    {
                        CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( RealNodeTypeId );
                        Nodes = MetaDataNodeType.getNodes( true, false );
                    }
                    else
                    {
                        CswNbtMetaDataObjectClass MetaDataObjectClass = null;
                        if( RealObjectClassId != Int32.MinValue )
                            MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClassId );
                        else if( RealObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.Unknown )
                            MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClass );
                        Nodes = MetaDataObjectClass.getNodes( true, false );
                    }

                    foreach( CswNbtNode Node in Nodes )
                    {
                        ReturnVal.Add(
                            new XElement( "node",
                                new XAttribute( "id", Node.NodeId.ToString() ),
                                new XAttribute( "name", Node.NodeName ) ) );
                    }

                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;


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
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XmlDocument getViewInfo( string ViewId )
        {
            XmlDocument ReturnVal = new XmlDocument();
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
                        ReturnVal = View.ToXml();
                    }

                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = xmlError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
            return ReturnVal;
        } // getViewInfo()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement saveViewInfo( string ViewId, string ViewXml )
        {
            XElement ReturnVal = new XElement( "result" );

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
                        View.LoadXml( ViewXml );
                        View.save();

                        //if( View.Visibility != NbtViewVisibility.Property )
                        //    CswViewListTree.ClearCache( Session );
                        _CswNbtResources.ViewSelect.removeSessionView( View );
                        _CswNbtResources.ViewSelect.clearCache();

                        ReturnVal.Add( new XAttribute( "succeeded", "true" ) );
                    }
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;


        } // saveViewInfo()



        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getViewChildOptions( string ViewXml, string ArbitraryId, string StepNo )
        {
            XElement ReturnVal = new XElement( "result" );

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtWebServiceView ws = new CswNbtWebServiceView( _CswNbtResources );
                    ReturnVal = ws.getViewChildOptions( ViewXml, ArbitraryId, CswConvert.ToInt32( StepNo ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

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
                    Enum.TryParse<NbtViewVisibility>( Visibility, out RealVisibility );

                    CswPrimaryKey RealVisibilityRoleId = null;
                    CswPrimaryKey RealVisibilityUserId = null;
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
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getViewPropFilterUI( string ViewXml, string PropArbitraryId )
        {
            XElement ReturnVal = new XElement( "nodetypeprops" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new wsViewBuilder( _CswNbtResources );
                    ReturnVal = ws.getViewBuilderProps( ViewXml, PropArbitraryId );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        }


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement makeViewPropFilter( string ViewXml, string PropFiltJson )
        {
            XElement ReturnVal = new XElement( "nodetypeprops" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new wsViewBuilder( _CswNbtResources );
                    ReturnVal = ws.getViewPropFilter( ViewXml, PropFiltJson );
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        }

        #endregion View Editing

        #region Tabs and Props

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getTabs( string EditMode, string NodeId, string SafeNodeKey, string NodeTypeId, string Date, string filterToPropId )
        {
            XElement ReturnVal = new XElement( "tabs" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
                    //if( !string.IsNullOrEmpty( ParsedNodeKey ) || ( EditMode == "AddInPopup" && !string.IsNullOrEmpty( NodeTypeId ) ) )
                    //{
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
                    ReturnVal = ws.getTabs( RealEditMode, NodeId, ParsedNodeKey, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToDateTime( Date ), filterToPropId );
                    //}
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getTabs()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XmlDocument getProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId, string Date )
        {
            XmlDocument ReturnVal = new XmlDocument();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
                    //if( !string.IsNullOrEmpty( ParsedNodeKey ) || EditMode == "AddInPopup" )
                    //{
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
                    ReturnVal = ws.getProps( RealEditMode, NodeId, ParsedNodeKey, TabId, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToDateTime( Date ) );
                    //}
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = xmlError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;
        } // getProps()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XmlDocument getSingleProp( string EditMode, string NodeId, string SafeNodeKey, string PropId, string NodeTypeId, string NewPropXml )
        {
            XmlDocument ReturnVal = new XmlDocument();
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
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
                    ReturnVal = ws.getSingleProp( RealEditMode, NodeId, ParsedNodeKey, PropId, CswConvert.ToInt32( NodeTypeId ), NewPropXml );
                    //}
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = xmlError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;
        } // getSingleProp()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getPropNames( string Type, string Id )
        {
            XElement ReturnVal = new XElement( "properties" );
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

                        foreach( ICswNbtMetaDataProp Prop in Props )
                        {
                            ReturnVal.Add(
                                new XElement( "prop",
                                    new XAttribute( "proptype", PropType ),
                                    new XAttribute( "propname", Prop.PropNameWithQuestionNo ),
                                    new XAttribute( "propid", Prop.PropId.ToString() ) ) );
                        }
                    } // if( nId != Int32.MinValue )

                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getPropNames()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string saveProps( string EditMode, string NodeId, string SafeNodeKey, string NewPropsXml, string NodeTypeId, string ViewId )
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
                    var ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.saveProps( RealEditMode, NodeId, ParsedNodeKey, NewPropsXml, CswConvert.ToInt32( NodeTypeId ), View );
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

        #region Misc

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XmlDocument getAbout()
        {
            XmlDocument ReturnVal = new XmlDocument();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceHeader( _CswNbtResources );
                    string Data = ws.makeVersionXml();
                    ReturnVal.LoadXml( Data.Replace( "&", "&amp;" ) );

                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = xmlError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
            return ReturnVal;
        } // getAbout()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getNodeTypes()
        {
            XElement ReturnVal = new XElement( "nodetypes" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceMetaData( _CswNbtResources );
                    ReturnVal = ws.getNodeTypes();
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getNodeTypes()


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
                            bool ret = ws.SetPropBlobValue( FileData, FileName, ContentType, PropId );

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

        #region Search

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getClientSearchXml( string ViewId, string SelectedNodeTypeIdNum, string IdPrefix, string NodeKey )
        {
            XElement ReturnVal = new XElement( "search" );

            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    var ws = new CswNbtWebServiceSearch( _CswNbtResources, IdPrefix );
                    CswNbtView View = _getView( ViewId );
                    ReturnVal = ws.getSearchXml( View, SelectedNodeTypeIdNum, NodeKey );
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        }

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getNodeTypeSearchProps( string RelatedIdType, string NodeTypeOrObjectClassId, string IdPrefix, string NodeKey )
        {
            XElement ReturnVal = new XElement( "search" );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    var ws = new CswNbtWebServiceSearch( _CswNbtResources, IdPrefix );
                    ReturnVal = ( ws.getSearchProps( RelatedIdType, NodeTypeOrObjectClassId, NodeKey ) );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

        } // getSearch()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getSearchableViews( string IsMobile, string OrderBy )
        {
            XElement ReturnVal = new XElement( "result" );

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
                ReturnVal = _xError( ex );
            }

            return ReturnVal;

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
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse( typeof( CswNbtWebServiceTabsAndProps.NodeEditMode ), EditMode );
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
        public string clearProp( string PropId, bool IncludeBlob )
        {
            JObject ReturnVal = new JObject( new JProperty( "Succeeded", false.ToString().ToLower() ) );
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
                    bool ret = ws.ClearPropValue( PropId, IncludeBlob );
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
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getWelcomeItems( string RoleId )
        {
            XElement ReturnVal = new XElement( "welcome" );
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
                        ReturnVal = XElement.Parse( ws.GetWelcomeItems( RoleId ) );
                    }
                    else
                    {
                        ReturnVal = XElement.Parse( ws.GetWelcomeItems( _CswNbtResources.CurrentNbtUser.RoleId.ToString() ) );
                    }
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }


            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;


        } // getWelcomeItems()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XElement getWelcomeButtonIconList()
        {
            XElement ReturnVal = new XElement( "buttonicons" );
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
                ReturnVal = _xError( ex );
            }

            _xAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal;

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
            _jAddAuthenticationStatus( Connected, AuthenticationStatus.Authenticated );  // we don't want to trigger session timeouts
            return ( Connected.ToString() );
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

            catch( Exception ex )
            {
                //nada
            }

            return new JObject( new JProperty( "succeeded", "true" ) ).ToString();
        } // UpdateProperties()

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
        public string UpdateProperties( string SessionId, string ParentId, string UpdatedViewXml, bool ForMobile )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {

                    CswNbtWebServiceMobileUpdateProperties wsUP = new CswNbtWebServiceMobileUpdateProperties( _CswNbtResources, ForMobile );
                    string ViewXml = wsUP.Run( ParentId, UpdatedViewXml ).ToString();
                    ReturnVal.Add( new JProperty( "xml", ViewXml ) );
                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // UpdateProperties()


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetViewsList( string SessionId, string ParentId, bool ForMobile )
        {
            XElement ReturnVal = new XElement( "views" );
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
                        CswNbtWebServiceMobileView wsView = new CswNbtWebServiceMobileView( _CswNbtResources, ForMobile );
                        ReturnVal = wsView.getViewsList( ParentId, CurrentUser );
                    }

                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }

            string JsonText = JsonConvert.SerializeXNode( ReturnVal );
            //string JsonText = @"{""views"":{""view"":[{""@id"":""ViewId_3026"",""@name"":""Inspections Due Today""},{""@id"":""ViewId_3027"",""@name"":""Inspections Due Tomorrow""},{""@id"":""ViewId_3025"",""@name"":""Inspections Due Within 7 Days""},{""@id"":""ViewId_2640"",""@name"":""My Open Inspections""}]}}";
            JObject JsonO = JObject.Parse( JsonText );

            _jAddAuthenticationStatus( JsonO, AuthenticationStatus.Authenticated );


            return JsonO.ToString();
        } // GetViews()

        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string GetView( string SessionId, string ParentId, bool ForMobile )
        {
            XElement ReturnVal = new XElement( "views" );
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
                        CswNbtWebServiceMobileView wsView = new CswNbtWebServiceMobileView( _CswNbtResources, ForMobile );
                        ReturnVal = wsView.getView( ParentId, CurrentUser );
                    }

                }

                _deInitResources();
            }

            catch( Exception ex )
            {
                ReturnVal = _xError( ex );
            }
            string JsonText = JsonConvert.SerializeXNode( ReturnVal );
            //string JsonText = @"{""searches"":{""search"":[{""@name"":""FE Inspection Point"",""@id"":""search_ocp_292""},{""@name"":""Location"",""@id"":""search_ocp_1255""}],""node"":[{""@id"":""nodeid_nodes_24709"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""291 North End"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24709"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24709"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24709"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24709"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24709"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24709"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24709"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24709"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24709"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24709"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24709"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Pending"",""@ocpname"":""Status"",""Value"":""Pending"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24709"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24709"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24709"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24709"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24709"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""291 North End"",""@ocpname"":""Target"",""NodeID"":""23531"",""Name"":""291 North End"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24709"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/15/2011"",""@ocpname"":""Due Date"",""Value"":""7/15/2011""},{""@id"":""prop_4072_nodeid_nodes_24709"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""291"",""@ocpname"":"""",""Value"":""291""},{""@id"":""prop_4017_nodeid_nodes_24709"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Outer Hallway""}]}},{""@id"":""nodeid_nodes_24768"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""300 South End Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Center Hall"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24768"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24768"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24768"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24768"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24768"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24768"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24768"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24768"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24768"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24768"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24768"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24768"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24768"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24768"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24768"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24768"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""300 South End Hallway"",""@ocpname"":""Target"",""NodeID"":""23472"",""Name"":""300 South End Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24768"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24768"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""300"",""@ocpname"":"""",""Value"":""300""},{""@id"":""prop_4017_nodeid_nodes_24768"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Center Hall"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Center Hall""}]}},{""@id"":""nodeid_nodes_24769"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""324 South End Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Checkout"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24769"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24769"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24769"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24769"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24769"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24769"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24769"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24769"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24769"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24769"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24769"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24769"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24769"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24769"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24769"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24769"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""324 South End Hallway"",""@ocpname"":""Target"",""NodeID"":""23560"",""Name"":""324 South End Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24769"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24769"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""324"",""@ocpname"":"""",""Value"":""324""},{""@id"":""prop_4017_nodeid_nodes_24769"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Checkout"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Checkout""}]}},{""@id"":""nodeid_nodes_24770"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""326 South End Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Inner Hallway"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24770"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24770"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24770"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24770"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24770"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24770"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24770"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24770"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24770"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24770"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24770"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24770"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24770"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24770"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24770"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24770"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""326 South End Hallway"",""@ocpname"":""Target"",""NodeID"":""23475"",""Name"":""326 South End Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24770"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24770"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""326"",""@ocpname"":"""",""Value"":""326""},{""@id"":""prop_4017_nodeid_nodes_24770"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Inner Hallway"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Inner Hallway""}]}},{""@id"":""nodeid_nodes_24771"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""4 In Main Entrance"",""@search_ocp_1255"":""Site 1 > Admissions > First Floor > Registrar"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24771"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24771"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24771"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24771"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24771"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24771"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24771"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24771"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24771"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24771"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24771"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24771"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24771"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24771"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24771"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24771"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""4 In Main Entrance"",""@ocpname"":""Target"",""NodeID"":""23526"",""Name"":""4 In Main Entrance"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24771"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24771"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""4"",""@ocpname"":"""",""Value"":""4""},{""@id"":""prop_4017_nodeid_nodes_24771"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > First Floor > Registrar"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > First Floor > Registrar""}]}},{""@id"":""nodeid_nodes_24772"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""41 Middle of Lab"",""@search_ocp_1255"":""Site 1 > North Research Lab > Lab 1"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24772"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24772"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24772"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24772"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24772"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24772"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24772"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24772"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24772"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24772"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24772"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24772"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24772"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24772"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24772"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24772"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""41 Middle of Lab"",""@ocpname"":""Target"",""NodeID"":""23499"",""Name"":""41 Middle of Lab"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24772"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24772"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""41"",""@ocpname"":"""",""Value"":""41""},{""@id"":""prop_4017_nodeid_nodes_24772"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > North Research Lab > Lab 1"",""@ocpname"":""Location"",""Value"":""Site 1 > North Research Lab > Lab 1""}]}},{""@id"":""nodeid_nodes_24773"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""6 In Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > First Floor > Welcome Hall"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24773"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24773"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24773"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24773"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24773"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24773"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24773"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24773"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24773"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24773"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24773"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24773"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24773"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24773"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24773"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24773"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""6 In Hallway"",""@ocpname"":""Target"",""NodeID"":""23520"",""Name"":""6 In Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24773"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24773"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""6"",""@ocpname"":"""",""Value"":""6""},{""@id"":""prop_4017_nodeid_nodes_24773"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > First Floor > Welcome Hall"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > First Floor > Welcome Hall""}]}},{""@id"":""nodeid_nodes_24776"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""1 South Wall"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Office 201"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24776"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24776"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24776"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24776"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24776"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24776"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24776"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24776"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24776"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24776"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24776"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24776"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24776"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24776"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24776"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24776"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""1 South Wall"",""@ocpname"":""Target"",""NodeID"":""23496"",""Name"":""1 South Wall"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24776"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24776"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""1"",""@ocpname"":"""",""Value"":""1""},{""@id"":""prop_4017_nodeid_nodes_24776"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Office 201"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Office 201""}]}},{""@id"":""nodeid_nodes_24777"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""10 Mid Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24777"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24777"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24777"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24777"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24777"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24777"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24777"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24777"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24777"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24777"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24777"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24777"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24777"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24777"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24777"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24777"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""10 Mid Hallway"",""@ocpname"":""Target"",""NodeID"":""23490"",""Name"":""10 Mid Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24777"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24777"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""10"",""@ocpname"":"""",""Value"":""10""},{""@id"":""prop_4017_nodeid_nodes_24777"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Outer Hallway""}]}},{""@id"":""nodeid_nodes_24778"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""11 South End Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24778"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24778"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24778"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24778"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24778"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24778"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24778"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24778"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24778"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24778"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24778"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24778"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24778"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24778"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24778"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24778"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""11 South End Hallway"",""@ocpname"":""Target"",""NodeID"":""23487"",""Name"":""11 South End Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24778"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24778"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""11"",""@ocpname"":"""",""Value"":""11""},{""@id"":""prop_4017_nodeid_nodes_24778"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Outer Hallway""}]}},{""@id"":""nodeid_nodes_24758"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""1 South Wall"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Office 201"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24758"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24758"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24758"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24758"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24758"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24758"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24758"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24758"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24758"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24758"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24758"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24758"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24758"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24758"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24758"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24758"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""1 South Wall"",""@ocpname"":""Target"",""NodeID"":""23496"",""Name"":""1 South Wall"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24758"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24758"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""1"",""@ocpname"":"""",""Value"":""1""},{""@id"":""prop_4017_nodeid_nodes_24758"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Office 201"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Office 201""}]}},{""@id"":""nodeid_nodes_24759"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""10 Mid Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24759"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24759"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24759"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24759"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24759"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24759"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24759"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24759"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24759"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24759"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24759"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24759"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24759"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24759"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24759"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24759"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""10 Mid Hallway"",""@ocpname"":""Target"",""NodeID"":""23490"",""Name"":""10 Mid Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24759"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24759"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""10"",""@ocpname"":"""",""Value"":""10""},{""@id"":""prop_4017_nodeid_nodes_24759"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Outer Hallway""}]}},{""@id"":""nodeid_nodes_24760"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""11 South End Hallway"",""@search_ocp_1255"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24760"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24760"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24760"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24760"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24760"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24760"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24760"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24760"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24760"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24760"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24760"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24760"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24760"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24760"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24760"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24760"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""11 South End Hallway"",""@ocpname"":""Target"",""NodeID"":""23487"",""Name"":""11 South End Hallway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24760"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24760"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""11"",""@ocpname"":"""",""Value"":""11""},{""@id"":""prop_4017_nodeid_nodes_24760"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Second Floor > Outer Hallway"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Second Floor > Outer Hallway""}]}},{""@id"":""nodeid_nodes_24761"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""14 Doorway"",""@search_ocp_1255"":""Site 1 > Admissions > Visitor Entrance"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24761"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24761"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24761"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24761"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24761"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24761"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24761"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24761"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24761"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24761"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24761"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24761"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24761"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24761"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24761"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24761"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""14 Doorway"",""@ocpname"":""Target"",""NodeID"":""23493"",""Name"":""14 Doorway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24761"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24761"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""14"",""@ocpname"":"""",""Value"":""14""},{""@id"":""prop_4017_nodeid_nodes_24761"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Visitor Entrance"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Visitor Entrance""}]}},{""@id"":""nodeid_nodes_24762"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""23 Across from Stairs"",""@search_ocp_1255"":""Site 1 > Library > First Floor > Reading Room"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24762"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24762"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24762"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24762"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24762"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24762"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24762"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24762"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24762"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24762"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24762"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24762"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24762"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24762"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24762"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24762"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""23 Across from Stairs"",""@ocpname"":""Target"",""NodeID"":""23502"",""Name"":""23 Across from Stairs"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24762"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24762"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""23"",""@ocpname"":"""",""Value"":""23""},{""@id"":""prop_4017_nodeid_nodes_24762"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > First Floor > Reading Room"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > First Floor > Reading Room""}]}},{""@id"":""nodeid_nodes_24763"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""24 Next to Elevator"",""@search_ocp_1255"":""Site 1 > Library > Ground Floor > Reception"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24763"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24763"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24763"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24763"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24763"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24763"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24763"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24763"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24763"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24763"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24763"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24763"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24763"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24763"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24763"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24763"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""24 Next to Elevator"",""@ocpname"":""Target"",""NodeID"":""23511"",""Name"":""24 Next to Elevator"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24763"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24763"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""24"",""@ocpname"":"""",""Value"":""24""},{""@id"":""prop_4017_nodeid_nodes_24763"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Ground Floor > Reception"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Ground Floor > Reception""}]}},{""@id"":""nodeid_nodes_24764"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""25  Inner Wall"",""@search_ocp_1255"":""Site 1 > Library > Second Floor > Checkout"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24764"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24764"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24764"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24764"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24764"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24764"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24764"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24764"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24764"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24764"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24764"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24764"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24764"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24764"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24764"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24764"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""25  Inner Wall"",""@ocpname"":""Target"",""NodeID"":""23514"",""Name"":""25  Inner Wall"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24764"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24764"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""25"",""@ocpname"":"""",""Value"":""25""},{""@id"":""prop_4017_nodeid_nodes_24764"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Second Floor > Checkout"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Second Floor > Checkout""}]}},{""@id"":""nodeid_nodes_24765"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""26 Across from Stairway"",""@search_ocp_1255"":""Site 1 > Library > Third Floor > Archives"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24765"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24765"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24765"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24765"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24765"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24765"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24765"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24765"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24765"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24765"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24765"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24765"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24765"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24765"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24765"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24765"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""26 Across from Stairway"",""@ocpname"":""Target"",""NodeID"":""23517"",""Name"":""26 Across from Stairway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24765"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24765"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""26"",""@ocpname"":"""",""Value"":""26""},{""@id"":""prop_4017_nodeid_nodes_24765"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Third Floor > Archives"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Third Floor > Archives""}]}},{""@id"":""nodeid_nodes_24766"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""270 By Emergency Exit"",""@search_ocp_1255"":""Site 1 > Library > Third Floor > Main Hall"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24766"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24766"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24766"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24766"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24766"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24766"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24766"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24766"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24766"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24766"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24766"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24766"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24766"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24766"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24766"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24766"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""270 By Emergency Exit"",""@ocpname"":""Target"",""NodeID"":""23505"",""Name"":""270 By Emergency Exit"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24766"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24766"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""270"",""@ocpname"":"""",""Value"":""270""},{""@id"":""prop_4017_nodeid_nodes_24766"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Third Floor > Main Hall"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Third Floor > Main Hall""}]}},{""@id"":""nodeid_nodes_24767"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""272 By Emergency Exit"",""@search_ocp_1255"":""Site 1 > Library > Second Floor > Center Hall"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24767"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24767"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24767"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24767"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24767"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24767"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24767"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24767"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24767"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24767"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24767"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24767"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24767"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24767"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24767"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24767"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""272 By Emergency Exit"",""@ocpname"":""Target"",""NodeID"":""23508"",""Name"":""272 By Emergency Exit"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24767"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""6/1/2011"",""@ocpname"":""Due Date"",""Value"":""6/1/2011""},{""@id"":""prop_4072_nodeid_nodes_24767"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""272"",""@ocpname"":"""",""Value"":""272""},{""@id"":""prop_4017_nodeid_nodes_24767"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Second Floor > Center Hall"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Second Floor > Center Hall""}]}},{""@id"":""nodeid_nodes_24799"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""25  Inner Wall"",""@search_ocp_1255"":""Site 1 > Library > Second Floor > Checkout"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24799"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24799"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24799"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24799"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24799"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24799"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24799"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24799"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24799"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24799"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24799"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Pending"",""@ocpname"":""Status"",""Value"":""Pending"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24799"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24799"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24799"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24799"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24799"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""25  Inner Wall"",""@ocpname"":""Target"",""NodeID"":""23514"",""Name"":""25  Inner Wall"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24799"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/13/2011"",""@ocpname"":""Due Date"",""Value"":""7/13/2011""},{""@id"":""prop_4072_nodeid_nodes_24799"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""25"",""@ocpname"":"""",""Value"":""25""},{""@id"":""prop_4017_nodeid_nodes_24799"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Second Floor > Checkout"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Second Floor > Checkout""}]}},{""@id"":""nodeid_nodes_24800"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""26 Across from Stairway"",""@search_ocp_1255"":""Site 1 > Library > Third Floor > Archives"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24800"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24800"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24800"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24800"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24800"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24800"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24800"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24800"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24800"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24800"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24800"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Pending"",""@ocpname"":""Status"",""Value"":""Pending"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24800"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24800"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24800"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24800"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24800"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""26 Across from Stairway"",""@ocpname"":""Target"",""NodeID"":""23517"",""Name"":""26 Across from Stairway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24800"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/13/2011"",""@ocpname"":""Due Date"",""Value"":""7/13/2011""},{""@id"":""prop_4072_nodeid_nodes_24800"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""26"",""@ocpname"":"""",""Value"":""26""},{""@id"":""prop_4017_nodeid_nodes_24800"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Third Floor > Archives"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Third Floor > Archives""}]}},{""@id"":""nodeid_nodes_24801"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""270 By Emergency Exit"",""@search_ocp_1255"":""Site 1 > Library > Third Floor > Main Hall"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24801"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24801"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24801"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24801"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24801"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24801"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24801"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24801"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24801"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24801"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24801"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Pending"",""@ocpname"":""Status"",""Value"":""Pending"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24801"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24801"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24801"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24801"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24801"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""270 By Emergency Exit"",""@ocpname"":""Target"",""NodeID"":""23505"",""Name"":""270 By Emergency Exit"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24801"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/13/2011"",""@ocpname"":""Due Date"",""Value"":""7/13/2011""},{""@id"":""prop_4072_nodeid_nodes_24801"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""270"",""@ocpname"":"""",""Value"":""270""},{""@id"":""prop_4017_nodeid_nodes_24801"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Third Floor > Main Hall"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Third Floor > Main Hall""}]}},{""@id"":""nodeid_nodes_24802"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""272 By Emergency Exit"",""@search_ocp_1255"":""Site 1 > Library > Second Floor > Center Hall"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24802"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24802"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24802"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24802"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24802"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24802"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24802"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24802"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24802"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24802"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24802"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Pending"",""@ocpname"":""Status"",""Value"":""Pending"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24802"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24802"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24802"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24802"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24802"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""272 By Emergency Exit"",""@ocpname"":""Target"",""NodeID"":""23508"",""Name"":""272 By Emergency Exit"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24802"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/13/2011"",""@ocpname"":""Due Date"",""Value"":""7/13/2011""},{""@id"":""prop_4072_nodeid_nodes_24802"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""272"",""@ocpname"":"""",""Value"":""272""},{""@id"":""prop_4017_nodeid_nodes_24802"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Second Floor > Center Hall"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Second Floor > Center Hall""}]}},{""@id"":""nodeid_nodes_24779"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""14 Doorway"",""@search_ocp_1255"":""Site 1 > Admissions > Visitor Entrance"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24779"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24779"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24779"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24779"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24779"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24779"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24779"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24779"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24779"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24779"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24779"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24779"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24779"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24779"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24779"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24779"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""14 Doorway"",""@ocpname"":""Target"",""NodeID"":""23493"",""Name"":""14 Doorway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24779"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24779"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""14"",""@ocpname"":"""",""Value"":""14""},{""@id"":""prop_4017_nodeid_nodes_24779"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Admissions > Visitor Entrance"",""@ocpname"":""Location"",""Value"":""Site 1 > Admissions > Visitor Entrance""}]}},{""@id"":""nodeid_nodes_24780"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""23 Across from Stairs"",""@search_ocp_1255"":""Site 1 > Library > First Floor > Reading Room"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24780"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24780"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24780"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24780"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24780"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24780"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24780"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24780"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24780"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24780"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24780"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24780"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24780"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24780"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24780"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24780"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""23 Across from Stairs"",""@ocpname"":""Target"",""NodeID"":""23502"",""Name"":""23 Across from Stairs"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24780"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24780"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""23"",""@ocpname"":"""",""Value"":""23""},{""@id"":""prop_4017_nodeid_nodes_24780"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > First Floor > Reading Room"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > First Floor > Reading Room""}]}},{""@id"":""nodeid_nodes_24781"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""24 Next to Elevator"",""@search_ocp_1255"":""Site 1 > Library > Ground Floor > Reception"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24781"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24781"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24781"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24781"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24781"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24781"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24781"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24781"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24781"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24781"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24781"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24781"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24781"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24781"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24781"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24781"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""24 Next to Elevator"",""@ocpname"":""Target"",""NodeID"":""23511"",""Name"":""24 Next to Elevator"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24781"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24781"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""24"",""@ocpname"":"""",""Value"":""24""},{""@id"":""prop_4017_nodeid_nodes_24781"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Ground Floor > Reception"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Ground Floor > Reception""}]}},{""@id"":""nodeid_nodes_24782"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""25  Inner Wall"",""@search_ocp_1255"":""Site 1 > Library > Second Floor > Checkout"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24782"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24782"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24782"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24782"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24782"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24782"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24782"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24782"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24782"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24782"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24782"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24782"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24782"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24782"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24782"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24782"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""25  Inner Wall"",""@ocpname"":""Target"",""NodeID"":""23514"",""Name"":""25  Inner Wall"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24782"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24782"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""25"",""@ocpname"":"""",""Value"":""25""},{""@id"":""prop_4017_nodeid_nodes_24782"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Second Floor > Checkout"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Second Floor > Checkout""}]}},{""@id"":""nodeid_nodes_24783"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""26 Across from Stairway"",""@search_ocp_1255"":""Site 1 > Library > Third Floor > Archives"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24783"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24783"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24783"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24783"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24783"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24783"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24783"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24783"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24783"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24783"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24783"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24783"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24783"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24783"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24783"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24783"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""26 Across from Stairway"",""@ocpname"":""Target"",""NodeID"":""23517"",""Name"":""26 Across from Stairway"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24783"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24783"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""26"",""@ocpname"":"""",""Value"":""26""},{""@id"":""prop_4017_nodeid_nodes_24783"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Third Floor > Archives"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Third Floor > Archives""}]}},{""@id"":""nodeid_nodes_24784"",""@name"":""Physical Inspection"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""Plain"",""@search_ocp_292"":""270 By Emergency Exit"",""@search_ocp_1255"":""Site 1 > Library > Third Floor > Main Hall"",""subitems"":{""prop"":[{""@id"":""prop_4019_nodeid_nodes_24784"",""@name"":""Q1.1 Inspection Point is Located in the Designated Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4018_nodeid_nodes_24784"",""@name"":""Q1.2 Inspection Point is Free of Obstructions?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4021_nodeid_nodes_24784"",""@name"":""Q1.3 Operating Instructions on Nameplate are Legible and Facing Outward?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4012_nodeid_nodes_24784"",""@name"":""Q1.4 Fire Extinguisher has Physical Damage?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4013_nodeid_nodes_24784"",""@name"":""Q1.5 Fire Extinguisher is Full?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4004_nodeid_nodes_24784"",""@name"":""Q1.6 Are Safety Seals and Tamper Indicators Broken or Missing?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4009_nodeid_nodes_24784"",""@name"":""Q1.7 Extinguisher is Leaking or Corroded?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""No"",""@ocpname"":"""",""Answer"":""No"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""No"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4010_nodeid_nodes_24784"",""@name"":""Q1.8 Extinguisher Nozzle is Free from Clogs?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4022_nodeid_nodes_24784"",""@name"":""Q1.9 Pressure Gauge Reading or Indicator is in Operable Range or Position?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4015_nodeid_nodes_24784"",""@name"":""Q1.10 HMIS Label is in Place?"",""@tab"":""Questions"",""@isreadonly"":""false"",""@fieldtype"":""Question"",""@gestalt"":""Yes"",""@ocpname"":"""",""Answer"":""Yes"",""allowedanswers"":""Yes,No,N/A"",""compliantanswers"":""Yes"",""Comments"":null,""CorrectiveAction"":null,""IsCompliant"":""True"",""DateAnswered"":""7/11/2011"",""DateCorrected"":null},{""@id"":""prop_4025_nodeid_nodes_24784"",""@name"":""Status"",""@tab"":""Action"",""@isreadonly"":""true"",""@fieldtype"":""List"",""@gestalt"":""Overdue"",""@ocpname"":""Status"",""Value"":""Overdue"",""Options"":"",Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled""},{""@id"":""prop_4011_nodeid_nodes_24784"",""@name"":""Finished"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Finished"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4006_nodeid_nodes_24784"",""@name"":""Cancel Reason"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Memo"",""@gestalt"":"""",""@ocpname"":""Cancel Reason"",""Text"":{""@rows"":""4"",""@columns"":""40"",""#cdata-section"":""""}},{""@id"":""prop_4007_nodeid_nodes_24784"",""@name"":""Cancelled"",""@tab"":""Action"",""@isreadonly"":""false"",""@fieldtype"":""Logical"",""@gestalt"":""N"",""@ocpname"":""Cancelled"",""Checked"":""false"",""Required"":""true""},{""@id"":""prop_4020_nodeid_nodes_24784"",""@name"":""Name"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Text"",""@gestalt"":""Physical Inspection"",""@ocpname"":""Name"",""Text"":{""@length"":""40"",""#text"":""Physical Inspection""}},{""@id"":""prop_4026_nodeid_nodes_24784"",""@name"":""FE Inspection Point"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Relationship"",""@gestalt"":""270 By Emergency Exit"",""@ocpname"":""Target"",""NodeID"":""23505"",""Name"":""270 By Emergency Exit"",""nodetypeid"":""795"",""allowadd"":""true"",""options"":{""option"":[{""@id"":""23496"",""@value"":""1 South Wall""},{""@id"":""23490"",""@value"":""10 Mid Hallway""},{""@id"":""23487"",""@value"":""11 South End Hallway""},{""@id"":""23493"",""@value"":""14 Doorway""},{""@id"":""23502"",""@value"":""23 Across from Stairs""},{""@id"":""23511"",""@value"":""24 Next to Elevator""},{""@id"":""23514"",""@value"":""25  Inner Wall""},{""@id"":""23517"",""@value"":""26 Across from Stairway""},{""@id"":""23505"",""@value"":""270 By Emergency Exit""},{""@id"":""23508"",""@value"":""272 By Emergency Exit""},{""@id"":""23531"",""@value"":""291 North End""},{""@id"":""23472"",""@value"":""300 South End Hallway""},{""@id"":""23560"",""@value"":""324 South End Hallway""},{""@id"":""23475"",""@value"":""326 South End Hallway""},{""@id"":""23526"",""@value"":""4 In Main Entrance""},{""@id"":""23499"",""@value"":""41 Middle of Lab""},{""@id"":""23520"",""@value"":""6 In Hallway""}]}},{""@id"":""prop_4008_nodeid_nodes_24784"",""@name"":""Due Date"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""Date"",""@gestalt"":""7/12/2011"",""@ocpname"":""Due Date"",""Value"":""7/12/2011""},{""@id"":""prop_4072_nodeid_nodes_24784"",""@name"":""Barcode"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""270"",""@ocpname"":"""",""Value"":""270""},{""@id"":""prop_4017_nodeid_nodes_24784"",""@name"":""Location"",""@tab"":""Detail"",""@isreadonly"":""false"",""@fieldtype"":""PropertyReference"",""@gestalt"":""Site 1 > Library > Third Floor > Main Hall"",""@ocpname"":""Location"",""Value"":""Site 1 > Library > Third Floor > Main Hall""}]}},{""@id"":""nodeid_nodes_24785"",""@name"":""Results Truncated at 30"",""@nodetype"":""Physical Inspection"",""@objectclass"":""InspectionDesignClass"",""@iconfilename"":""test.gif"",""@nodespecies"":""More"",""@search_ocp_292"":""272 By Emergency Exit"",""@search_ocp_1255"":""Site 1 > Library > Second Floor > Center Hall"",""subitems"":null}]}}";
            JObject JsonO = JObject.Parse( JsonText );

            _jAddAuthenticationStatus( JsonO, AuthenticationStatus.Authenticated );


            return JsonO.ToString();
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

        #endregion Web Methods

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

    }//wsNBT

} // namespace ChemSW.WebServices

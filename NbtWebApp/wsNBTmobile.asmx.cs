using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Web.Services;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Statistics;
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
    public class wsNBTmobile : WebService
    {
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
            return ret;
        } // _attemptRefresh()

        private void _deInitResources( CswNbtResources OtherResources = null )
        {
            _CswSessionResources.endSession();
            if( null != _CswNbtResources )
            {
                _CswNbtResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                _CswNbtResources.finalize();
                _CswNbtResources.release();
            }
            if( null != OtherResources )
            {
                OtherResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                OtherResources.finalize();
                OtherResources.release();
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

        #region Authentication

        private AuthenticationStatus _doCswAdminAuthenticate( string PropId )
        {
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            CswNbtWebServiceNbtManager ws = new CswNbtWebServiceNbtManager( _CswNbtResources );
            string TempPassword = string.Empty;
            CswNbtObjClassCustomer NodeAsCustomer = ws.openCswAdminOnTargetSchema( PropId, ref TempPassword );

            AuthenticationStatus = _authenticate( NodeAsCustomer.CompanyID.Text, CswNbtObjClassUser.ChemSWAdminUsername, TempPassword, false );

            if( AuthenticationStatus != AuthenticationStatus.Authenticated )
            {
                throw new CswDniException( ErrorType.Error, "Authentication in this context is not possible.", "Authentication in this context is not possible." );
            }

            return AuthenticationStatus;
        }

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
        public string reauthenticate( string PropId )
        {
            JObject ReturnVal = new JObject();
            try
            {
                _initResources();
                AuthenticationStatus AuthenticationStatus = _doCswAdminAuthenticate( PropId );
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
                    CswNbtObjClassUser CurrentUser = _CswNbtResources.CurrentNbtUser.UserNode;
                    ReturnVal.Add( new JProperty( "nodeid", CurrentUser.NodeId.ToString() ) );
                    CswNbtNodeKey FakeKey = new CswNbtNodeKey( _CswNbtResources );
                    FakeKey.NodeId = CurrentUser.NodeId;
                    FakeKey.NodeSpecies = CurrentUser.Node.NodeSpecies;
                    FakeKey.NodeTypeId = CurrentUser.NodeTypeId;
                    FakeKey.ObjectClassId = CurrentUser.ObjectClass.ObjectClassId;
                    ReturnVal.Add( new JProperty( "cswnbtnodekey", FakeKey.ToString() ) );
                    CswPropIdAttr PasswordPropIdAttr = new CswPropIdAttr( CurrentUser.Node, CurrentUser.PasswordProperty.NodeTypeProp );
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

        #region SI mobile methods


        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getDueInspectionsForUser()
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    //this is the "MINE" method
                    /*    
                           CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, CswConvert.ToBoolean( Multi ) );
                           _setEditMode( EditMode );
                           CswDateTime InDate = new CswDateTime( _CswNbtResources );
                           InDate.FromClientDateTimeString( Date );
                    
                           ReturnVal = ws.getTabs( NodeId, SafeNodeKey, CswConvert.ToInt32( NodeTypeId ), InDate, filterToPropId );
                     */
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getDueInspectionsForUser()



        [WebMethod( EnableSession = false )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string getDueInspectionsForDateRange( string beginDate, string endDate )
        {
            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    DateTime begins = CswConvert.ToDateTime( beginDate );
                    DateTime ends = CswConvert.ToDateTime( endDate );

                    CswNbtMetaDataObjectClass InspectionOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                    CswNbtMetaDataObjectClassProp InspectionStatusOCP = InspectionOC.getObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
                    CswNbtMetaDataObjectClassProp InspectionDueOCP = InspectionOC.getObjectClassProp( CswNbtObjClassInspectionDesign.DatePropertyName );

                    // get Due/ActionRequired inspections
                    CswNbtView DueView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship InspectionRel = OOCView.AddViewRelationship( InspectionOC, false );
                    CswNbtViewProperty StatusViewProp = OOCView.AddViewProperty( InspectionRel, InspectionStatusOCP );
                    CswNbtViewProperty DueViewProp = OOCView.AddViewProperty( InspectionRel, InspectionDueOCP );


                    //only Pending and ActionRequired within the date range 
                    OOCView.AddViewPropertyFilter( StatusViewProp, InspectionStatusOCP.getFieldTypeRule().SubFields.Default.Name,
                    CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                    CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ),
                    false );
                    OOCView.AddViewPropertyFilter( StatusViewProp, InspectionStatusOCP.getFieldTypeRule().SubFields.Default.Name,
                    CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                    CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ),
                    false );
                    OOCView.AddViewPropertyFilter( StatusViewProp, InspectionStatusOCP.getFieldTypeRule().SubFields.Default.Name,
                    CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                    CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ),
                    false );
                    OOCView.AddViewPropertyFilter( StatusViewProp, InspectionStatusOCP.getFieldTypeRule().SubFields.Default.Name,
                    CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                    CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ),
                    false );

                    OOCView.AddViewPropertyFilter( StatusViewProp, InspectionStatusOCP.getFieldTypeRule().SubFields.Default.Name,
                    CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                    CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ),
                    false );
                    OOCView.AddViewPropertyFilter( StatusViewProp, InspectionStatusOCP.getFieldTypeRule().SubFields.Default.Name,
                    CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                    CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ),
                    false );



                    /*    
                           CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, CswConvert.ToBoolean( Multi ) );
                           _setEditMode( EditMode );
                           CswDateTime InDate = new CswDateTime( _CswNbtResources );
                           InDate.FromClientDateTimeString( Date );
                      */
                    ReturnVal = ws.getTabs( NodeId, SafeNodeKey, CswConvert.ToInt32( NodeTypeId ), InDate, filterToPropId );
                }

                _deInitResources();
            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }


            _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

            return ReturnVal.ToString();

        } // getDueInspectionsForUser()

        #endregion SI mobile methods


        #endregion Web Methods

        #region Private

        private void _setEditMode( string EditModeStr )
        {
            _CswNbtResources.EditMode = (NodeEditMode) Enum.Parse( typeof( NodeEditMode ), EditModeStr );
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

        private Collection<CswPrimaryKey> _getNodePks( CswCommaDelimitedString NodeIds, CswCommaDelimitedString NodeKeys )
        {
            Collection<CswPrimaryKey> RetCol = new Collection<CswPrimaryKey>();
            foreach( string NodeId in NodeIds )
            {
                CswPrimaryKey NodePk = _getNodeId( NodeId );
                if( null != NodePk )
                {
                    RetCol.Add( NodePk );
                }
            }

            if( 0 == RetCol.Count )
            {
                foreach( string NodeKey in NodeKeys )
                {
                    CswNbtNodeKey NbtNodeKey = _getNodeKey( NodeKey );
                    if( null != NbtNodeKey )
                    {
                        RetCol.Add( NbtNodeKey.NodeId );
                    }
                }
            }

            return RetCol;
        }

        #endregion Private
    }//wsNBT

} // namespace ChemSW.WebServices

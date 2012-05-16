using System;
using System.Web;
using ChemSW.Config;
// supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Security;
// supports ScriptService attribute
using Newtonsoft.Json.Linq;

// supports ScriptService attribute



namespace ChemSW.Nbt.WebServices
{
    public class CswNbtServiceDriver
    {

        private HttpContext _Context = null;
        private SetupMode _SetupMode = SetupMode.Unknown;
        private CswNbtServiceLogic _CswNbtServiceLogic = null;
        public CswNbtServiceDriver( HttpContext Context, SetupMode SetupMode, CswNbtServiceLogic CswNbtServiceLogic )
        {
            _Context = Context;
            _SetupMode = SetupMode;

            _CswNbtServiceLogic = CswNbtServiceLogic;
        }


        # region resources and authentication
        CswTimer Timer = new CswTimer();
        double ServerInitTime = 0;



        private CswNbtServiceLogicResources _CswNbtServiceDriverResources = null;


        private void _initResources()
        {

            _CswNbtServiceDriverResources = new CswNbtServiceLogicResources( _Context.Application, _Context.Request, _Context.Response, _Context, SetupMode.NbtWeb );
            _CswNbtServiceDriverResources.CswNbtResources.beginTransaction();
            _CswNbtServiceDriverResources.CswNbtResources.logMessage( "WebServices: Session Started (_initResources called)" );

        }//_initResources() 

        private AuthenticationStatus _attemptRefresh( bool ThrowOnError = false )
        {
            AuthenticationStatus ret = _CswNbtServiceDriverResources.CswSessionResourcesNbt.attemptRefresh();

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
                if( _Context.Request.Cookies["csw_currentviewid"] != null )
                {
                    ContextViewId = _Context.Request.Cookies["csw_currentviewid"].Value;
                }
                if( _Context.Request.Cookies["csw_currentactionname"] != null )
                {
                    ContextActionName = _Context.Request.Cookies["csw_currentactionname"].Value;
                }

                /*
                **** MUST BE REINSTATED **** 
                if( ContextViewId != string.Empty )
                {
                    CswNbtView ContextView = _getView( ContextViewId );
                    if( ContextView != null )
                    {
                        _CswNbtServiceDriverResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
                    }
                }
                else if( ContextActionName != string.Empty )
                {
                    CswNbtAction ContextAction = _CswNbtServiceDriverResources.Actions[CswNbtAction.ActionNameStringToEnum( ContextActionName )];
                    if( ContextAction != null )
                    {
                        _CswNbtServiceDriverResources.AuditContext = CswNbtAction.ActionNameEnumToString( ContextAction.Name ) + " (Action_" + ContextAction.ActionId.ToString() + ")";
                    }
                }
                 */
            }
            ServerInitTime = Timer.ElapsedDurationInMilliseconds;
            return ret;
        } // _attemptRefresh()

        private void _deInitResources( CswNbtResources OtherResources = null )
        {
            _CswNbtServiceDriverResources.CswSessionResourcesNbt.endSession();
            if( null != _CswNbtServiceDriverResources.CswNbtResources )
            {
                _CswNbtServiceDriverResources.CswNbtResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                _CswNbtServiceDriverResources.CswNbtResources.finalize();
                _CswNbtServiceDriverResources.CswNbtResources.release();
            }
            if( null != OtherResources )
            {
                OtherResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                OtherResources.finalize();
                OtherResources.release();
            }
        } // _deInitResources()
        # endregion


        public string run()
        {

            JObject ReturnVal = new JObject();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            try
            {
                _initResources();
                AuthenticationStatus = _attemptRefresh();

                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    ReturnVal = _CswNbtServiceLogic.doLogic( _CswNbtServiceDriverResources );
                }

            }
            catch( Exception ex )
            {
                ReturnVal = jError( ex );
            }

            finally
            {

                _deInitResources();
                _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );
            }

            return ( ReturnVal.ToString() );

        }

        private void _jAddAuthenticationStatus( JObject JObj, AuthenticationStatus AuthenticationStatusIn, bool ForMobile = false )
        {
            if( JObj != null )
            {
                JObj["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
                if( false == ForMobile )
                {
                    if( _CswNbtServiceDriverResources.CswSessionResourcesNbt != null &&
                         _CswNbtServiceDriverResources.CswSessionResourcesNbt.CswSessionManager != null )
                    {
                        CswDateTime CswTimeout = new CswDateTime( _CswNbtServiceDriverResources.CswNbtResources, _CswNbtServiceDriverResources.CswSessionResourcesNbt.CswSessionManager.TimeoutDate );
                        JObj["timeout"] = CswTimeout.ToClientAsJavascriptString();
                    }
                    JObj["timer"] = new JObject();
                    JObj["timer"]["serverinit"] = Math.Round( ServerInitTime, 3 );
                    if( null != _CswNbtServiceDriverResources.CswNbtResources )
                    {

                        JObj["timer"]["dbinit"] = Math.Round( _CswNbtServiceDriverResources.CswNbtResources.CswLogger.DbInitTime, 3 );
                        JObj["timer"]["dbquery"] = Math.Round( _CswNbtServiceDriverResources.CswNbtResources.CswLogger.DbQueryTime, 3 );
                        JObj["timer"]["dbcommit"] = Math.Round( _CswNbtServiceDriverResources.CswNbtResources.CswLogger.DbCommitTime, 3 );
                        JObj["timer"]["dbdeinit"] = Math.Round( _CswNbtServiceDriverResources.CswNbtResources.CswLogger.DbDeInitTime, 3 );
                        JObj["timer"]["treeloadersql"] = Math.Round( _CswNbtServiceDriverResources.CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );
                    }
                    JObj["timer"]["servertotal"] = Math.Round( Timer.ElapsedDurationInMilliseconds, 3 );
                    JObj["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
                }
            }
        }//_jAuthenticationStatus()

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


        private void _error( Exception ex, out ErrorType Type, out string Message, out string Detail, out bool Display )
        {
            if( _CswNbtServiceDriverResources.CswNbtResources != null )
            {
                _CswNbtServiceDriverResources.CswNbtResources.CswLogger.reportError( ex );
                _CswNbtServiceDriverResources.CswNbtResources.Rollback();
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
            if( _CswNbtServiceDriverResources.CswNbtResources != null )
            {
                if( newEx.Type == ErrorType.Warning )
                {

                    Display = ( _CswNbtServiceDriverResources.CswNbtResources.ConfigVbls.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Display = ( _CswNbtServiceDriverResources.CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }
            }

            Type = newEx.Type;
            Message = newEx.MsgFriendly;
            Detail = newEx.MsgEscoteric + "; " + ex.StackTrace;
        } // _error()
    } // class CswNbtServiceDriver



} // namespace ChemSW.Nbt.WebServices

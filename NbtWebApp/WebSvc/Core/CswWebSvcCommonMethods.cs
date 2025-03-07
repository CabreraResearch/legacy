using System;
using System.Web;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Log;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Security;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.WebSvc
{
    public class CswWebSvcCommonMethods
    {
        /// <summary>
        /// </summary>
        public static CswWebSvcReturnBase.ErrorMessage wError( CswNbtResources CswNbtResources, Exception ex )
        {
            CswWebSvcReturnBase.ErrorMessage Ret = new CswWebSvcReturnBase.ErrorMessage();
            string Message, Detail;
            CswEnumErrorType Type;
            bool Display;
            error( CswNbtResources, ex, out Type, out Message, out Detail, out Display );

            Ret.ShowError = Display;
            Ret.Type = Type.ToString();
            Ret.Message = Message;
            Ret.Detail = Detail;

            return Ret;

        }//jError() 

        /// <summary>
        /// </summary>
        public static JObject jError( CswNbtResources CswNbtResources, Exception ex )
        {
            JObject Ret = new JObject();
            string Message, Detail;
            CswEnumErrorType Type;
            bool Display;
            error( CswNbtResources, ex, out Type, out Message, out Detail, out Display );

            Ret["success"] = "false";
            Ret["error"] = new JObject();
            Ret["error"]["display"] = Display.ToString().ToLower();
            Ret["error"]["type"] = Type.ToString();
            Ret["error"]["message"] = Message;
            Ret["error"]["detail"] = Detail;
            return Ret;
        }//jError() 

        public static void error( CswNbtResources CswNbtResources, Exception ex, out CswEnumErrorType Type, out string Message, out string Detail, out bool Display )
        {
            if( CswNbtResources != null )
            {
                CswNbtResources.CswLogger.reportError( ex );
                CswNbtResources.Rollback();
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
            Type = newEx.Type;
            Message = newEx.MsgFriendly;
            Detail = newEx.MsgEscoteric + "; ";

            if( CswNbtResources != null )
            {
                if( newEx.Type == CswEnumErrorType.Warning )
                {
                    Display = ( CswNbtResources.ConfigVbls.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Display = ( CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }

                if( CswNbtResources.ShowFullStackTraceInUI )
                {
                    Detail += ex.StackTrace;
                }
            }
        } // _error()

        public static CswNbtView getView( CswNbtResources CswNbResources, string ViewId )
        {
            CswNbtView ReturnVal = null;

            if( CswNbtViewId.isViewIdString( ViewId ) )
            {
                CswNbtViewId realViewid = new CswNbtViewId( ViewId );
                ReturnVal = CswNbResources.ViewSelect.restoreView( realViewid );
            }
            else if( CswNbtSessionDataId.isSessionDataIdString( ViewId ) )
            {
                CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ViewId );
                ReturnVal = CswNbResources.ViewSelect.getSessionView( SessionViewid );
            }

            return ( ReturnVal );
        } // _getView()

        public static string getIpAddress()
        {
            string IPAddress = String.Empty;

            if( HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null )
            {
                IPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            if( string.IsNullOrEmpty( IPAddress ) && HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null )
            {
                IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            return IPAddress;
        }

        public static void jAddAuthenticationStatus( CswNbtResources CswNbtResources, CswSessionResourcesNbt CswSessionResources, JObject SvcReturn, CswEnumAuthenticationStatus AuthenticationStatusIn, bool IsMobile = false )
        {
            if( SvcReturn != null )
            {
                SvcReturn["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
                SvcReturn["AuthenticationStatusText"] = CswEnumAuthenticationStatus.EuphamizedText[AuthenticationStatusIn];
                if( false == IsMobile )
                {
                    SvcReturn["server"] = Environment.MachineName;
                    if( null != CswNbtResources )
                    {
                        if( null != CswNbtResources.CswSessionManager )
                        {
                            SvcReturn["timeout"] = CswDateTime.ToClientAsJavascriptString( CswNbtResources.CswSessionManager.TimeoutDate );
                        }
                        if( AuthenticationStatusIn == CswEnumAuthenticationStatus.ExpiredPassword )
                        {
                            ICswNbtUser CurrentUser = CswNbtResources.CurrentNbtUser;
                            SvcReturn.Add( new JProperty( "nodeid", CurrentUser.UserId.ToString() ) );
                            CswNbtNodeKey FakeKey = new CswNbtNodeKey()
                            {
                                NodeId = CurrentUser.UserId,
                                NodeSpecies = CswEnumNbtNodeSpecies.Plain,
                                NodeTypeId = CurrentUser.UserNodeTypeId,
                                ObjectClassId = CurrentUser.UserObjectClassId
                            };
                            SvcReturn.Add( new JProperty( "nodekey", FakeKey.ToString() ) );
                            CswPropIdAttr PasswordPropIdAttr = new CswPropIdAttr( CurrentUser.UserId, CurrentUser.PasswordPropertyId );
                            SvcReturn.Add( new JProperty( "passwordpropid", PasswordPropIdAttr.ToString() ) );
                        }

                        SvcReturn["timer"] = new JObject();

                        SvcReturn["timer"]["serverinit"] = Math.Round( CswNbtResources.ServerInitTime, 3 );
                        CswEnumLogLevels LogLevel = CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.Logging_Level );
                        if( LogLevel == CswNbtResources.UnknownEnum )
                        {
                            LogLevel = CswEnumLogLevels.Error;
                        }
                        SvcReturn["LogLevel"] = LogLevel.ToString().ToLower();

                        SvcReturn["timer"]["customerid"] = CswNbtResources.AccessId;
                        SvcReturn["timer"]["dbinit"] = Math.Round( CswNbtResources.CswLogger.DbInitTime, 3 );
                        SvcReturn["timer"]["dbquery"] = Math.Round( CswNbtResources.CswLogger.DbQueryTime, 3 );
                        SvcReturn["timer"]["dbcommit"] = Math.Round( CswNbtResources.CswLogger.DbCommitTime, 3 );
                        SvcReturn["timer"]["dbdeinit"] = Math.Round( CswNbtResources.CswLogger.DbDeInitTime, 3 );
                        SvcReturn["timer"]["treeloadersql"] = Math.Round( CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );
                        SvcReturn["timer"]["servertotal"] = Math.Round( CswNbtResources.TotalServerTime, 3 );

                        // Return any messages acquired along the way
                        if( null != CswNbtResources.Messages && CswNbtResources.Messages.Count > 0 )
                        {
                            JArray Messages = new JArray();
                            SvcReturn["messages"] = Messages;
                            foreach( CswWebSvcReturnBase.ErrorMessage Message in CswNbtResources.Messages )
                            {
                                Messages.Add( Message.ToJObject() );
                            }
                        }

                    }//if( null != CswNbtResources )
                }
            }
        }

        public static void wAddAuthenticationStatus( CswNbtResources CswNbtResources, CswSessionResourcesNbt CswSessionResources, CswWebSvcReturn SvcReturn, CswEnumAuthenticationStatus AuthenticationStatusIn, HttpContext HttpContext )
        {
            // ******************************************
            // IT IS VERY IMPORTANT for this function not to require the use of database resources, 
            // since it occurs AFTER the call to _deInitResources(), and thus will leak Oracle connections 
            // (see case 26273)
            // ******************************************
            if( null != SvcReturn )
            {
                SvcReturn.Authentication = SvcReturn.Authentication ?? new CswWebSvcSessionAuthenticateData.Authentication.Response();
                SvcReturn.Authentication.AuthenticationStatus = AuthenticationStatusIn;
                if( null != CswNbtResources )
                {
                    if( null != CswNbtResources.CswSessionManager )
                    {
                        SvcReturn.Authentication.TimeOut = CswDateTime.ToClientAsJavascriptString( CswNbtResources.CswSessionManager.TimeoutDate );
                    }
                    if( SvcReturn.Authentication.AuthenticationStatus == CswEnumAuthenticationStatus.ExpiredPassword )
                    {
                        SvcReturn.Authentication.ExpirationReset = new CswWebSvcSessionAuthenticateData.Authentication.Response.Expired();

                        ICswNbtUser CurrentUser = CswNbtResources.CurrentNbtUser;
                        SvcReturn.Authentication.ExpirationReset.UserId = CurrentUser.UserId.ToString();
                        CswNbtNodeKey FakeKey = new CswNbtNodeKey()
                            {
                                NodeId = CurrentUser.UserId,
                                NodeSpecies = CswEnumNbtNodeSpecies.Plain,
                                NodeTypeId = CurrentUser.UserNodeTypeId,
                                ObjectClassId = CurrentUser.UserObjectClassId
                            };
                        SvcReturn.Authentication.ExpirationReset.UserKey = FakeKey.ToString();
                        CswPropIdAttr PasswordPropIdAttr = new CswPropIdAttr( CurrentUser.UserId, CurrentUser.PasswordPropertyId );
                        SvcReturn.Authentication.ExpirationReset.PasswordId = PasswordPropIdAttr.ToString();
                    }

                    SvcReturn.Performance = SvcReturn.Performance ?? new CswWebSvcReturnBase.Performance();
                    SvcReturn.Performance.ServerInit = Math.Round( CswNbtResources.ServerInitTime, 3 );
                    SvcReturn.Performance.DbDeinit = Math.Round( CswNbtResources.CswLogger.DbInitTime, 3 );
                    SvcReturn.Performance.DbQuery = Math.Round( CswNbtResources.CswLogger.DbQueryTime, 3 );
                    SvcReturn.Performance.DbCommit = Math.Round( CswNbtResources.CswLogger.DbCommitTime, 3 );
                    SvcReturn.Performance.DbDeinit = Math.Round( CswNbtResources.CswLogger.DbDeInitTime, 3 );
                    SvcReturn.Performance.TreeLoaderSql = Math.Round( CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );
                    SvcReturn.Performance.ServerTotal = Math.Round( CswNbtResources.TotalServerTime, 3 );

                    SvcReturn.Logging = SvcReturn.Logging ?? new CswWebSvcReturnBase.Logging();
                    SvcReturn.Logging.CustomerId = CswNbtResources.AccessId;
                    SvcReturn.Logging.Server = Environment.MachineName;
                    CswEnumLogLevels LogLevel = CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.Logging_Level );

                    if( LogLevel == CswNbtResources.UnknownEnum )
                    {
                        LogLevel = CswEnumLogLevels.Error;
                    }
                    SvcReturn.Logging.LogLevel = LogLevel;

                    // Return any messages acquired along the way
                    if( null != CswNbtResources.Messages && CswNbtResources.Messages.Count > 0 )
                    {
                        SvcReturn.Status.Messages = CswNbtResources.Messages;
                    }
                }
            }
            HttpCookie AuthStatusCookie = HttpContext.Request.Cookies["CswAuthStatus"];
            if( null == AuthStatusCookie )
            {
                AuthStatusCookie = new HttpCookie( "CswAuthStatus" ) { HttpOnly = true };
            }
            if( AuthStatusCookie.Value != AuthenticationStatusIn.ToString() )
            {
                AuthStatusCookie.Value = AuthenticationStatusIn;
                HttpContext.Response.Cookies.Add( AuthStatusCookie );
            }
        } // wAddAuthenticationStatus()

    } // class CswWebSvcCommonMethods

} // namespace ChemSW.Nbt.WebServices

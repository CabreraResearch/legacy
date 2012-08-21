using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Security;
using Newtonsoft.Json.Linq;


namespace ChemSW.WebSvc
{
    public class CswWebSvcCommonMethods
    {


        public static JObject jError( CswNbtResources CswNbtResources, Exception ex )
        {
            JObject Ret = new JObject();
            string Message = string.Empty;
            string Detail = string.Empty;
            ErrorType Type = ErrorType.Error;
            bool Display = true;
            error( CswNbtResources, ex, out Type, out Message, out Detail, out Display );

            Ret["success"] = "false";
            Ret["error"] = new JObject();
            Ret["error"]["display"] = Display.ToString().ToLower();
            Ret["error"]["type"] = Type.ToString();
            Ret["error"]["message"] = Message;
            Ret["error"]["detail"] = Detail;

            return Ret;

        }//jError() 

        public static void error( CswNbtResources CswNbtResources, Exception ex, out ErrorType Type, out string Message, out string Detail, out bool Display )
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
            if( CswNbtResources != null )
            {
                if( newEx.Type == ErrorType.Warning )
                {
                    Display = ( CswNbtResources.ConfigVbls.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Display = ( CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }
            }

            Type = newEx.Type;
            Message = newEx.MsgFriendly;
            Detail = newEx.MsgEscoteric + "; " + ex.StackTrace;

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

        public static void jAddAuthenticationStatus( CswNbtResources CswNbtResources, CswSessionResourcesNbt CswSessionResources, JObject JObj, AuthenticationStatus AuthenticationStatusIn, bool ForMobile = false )
        {
            // ******************************************
            // IT IS VERY IMPORTANT for this function not to require the use of database resources, 
            // since it occurs AFTER the call to _deInitResources(), and thus will leak Oracle connections 
            // (see case 26273)
            // ******************************************

            if( JObj != null )
            {
                JObj["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
                //if( false == ForMobile ) <== SINCE MOBILE IS USING WCF, DO WE NEED THIS? 
                //{

                if( ( null != CswNbtResources ) && ( null != CswNbtResources.CswSessionManager ) )
                {
                    JObj["timeout"] = CswDateTime.ToClientAsJavascriptString( CswNbtResources.CswSessionManager.TimeoutDate );
                }


                JObj["timer"] = new JObject();
                JObj["timer"]["serverinit"] = Math.Round( CswNbtResources.ServerInitTime, 3 );
                if( null != CswNbtResources )
                {
                    JObj["timer"]["dbinit"] = Math.Round( CswNbtResources.CswLogger.DbInitTime, 3 );
                    JObj["timer"]["dbquery"] = Math.Round( CswNbtResources.CswLogger.DbQueryTime, 3 );
                    JObj["timer"]["dbcommit"] = Math.Round( CswNbtResources.CswLogger.DbCommitTime, 3 );
                    JObj["timer"]["dbdeinit"] = Math.Round( CswNbtResources.CswLogger.DbDeInitTime, 3 );
                    JObj["timer"]["treeloadersql"] = Math.Round( CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );
                }
                JObj["timer"]["servertotal"] = Math.Round( CswNbtResources.TotalServerTime, 3 );
                JObj["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
            }
        }//_jAuthenticationStatus()


    } // class CswWebSvcCommonMethods

} // namespace ChemSW.Nbt.WebServices

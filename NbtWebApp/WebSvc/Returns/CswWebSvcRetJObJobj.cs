using System;
using ChemSW.Security;
using ChemSW.WebSvc;
using Newtonsoft.Json.Linq;

// supports ScriptService attribute
// supports ScriptService attribute



// supports ScriptService attribute


namespace ChemSW.Nbt.WebServices
{
    public class CswWebSvcRetJObJobj : ICswWebSvcRetObj
    {
        private CswNbtResources _CswNbtResources = null;
        public ICswResources CswResources
        {
            set
            {
                _CswNbtResources = (CswNbtResources) value;
            }

        }

        JObject _JObject = new JObject();

        public JObject JObject
        {
            set
            {
                _JObject = value;
            }

            get
            {
                return ( _JObject );
            }
        }

        public void addException( Exception Exception )
        {
            _JObject = CswWebSvcCommonMethods.jError( _CswNbtResources, Exception );
        }//

        public void finalize( AuthenticationStatus AuthenticationStatus )
        {

            CswWebSvcCommonMethods.jAddAuthenticationStatus( _CswNbtResources, null, _JObject, AuthenticationStatus );
            // ******************************************
            // IT IS VERY IMPORTANT for this function not to require the use of database resources, 
            // since it occurs AFTER the call to _deInitResources(), and thus will leak Oracle connections 
            // (see case 26273)
            // ******************************************

            //if( _JObject != null )
            //{

            //    _JObject["AuthenticationStatus"] = AuthenticationStatus.ToString();
            //    //if( false == ForMobile ) <== SINCE MOBILE IS USING WCF, DO WE NEED THIS? 
            //    //{

            //    if( ( null != _CswNbtResources ) && ( null != _CswNbtResources.CswSessionManager ) )
            //    {
            //        _JObject["timeout"] = CswDateTime.ToClientAsJavascriptString( _CswNbtResources.CswSessionManager.TimeoutDate );
            //    }


            //    _JObject["timer"] = new JObject();
            //    _JObject["timer"]["serverinit"] = Math.Round( _CswNbtResources.ServerInitTime, 3 );
            //    if( null != _CswNbtResources )
            //    {
            //        _JObject["timer"]["dbinit"] = Math.Round( _CswNbtResources.CswLogger.DbInitTime, 3 );
            //        _JObject["timer"]["dbquery"] = Math.Round( _CswNbtResources.CswLogger.DbQueryTime, 3 );
            //        _JObject["timer"]["dbcommit"] = Math.Round( _CswNbtResources.CswLogger.DbCommitTime, 3 );
            //        _JObject["timer"]["dbdeinit"] = Math.Round( _CswNbtResources.CswLogger.DbDeInitTime, 3 );
            //        _JObject["timer"]["treeloadersql"] = Math.Round( _CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );
            //    }
            //    _JObject["timer"]["servertotal"] = Math.Round( _CswNbtResources.TotalServerTime, 3 );
            //    _JObject["AuthenticationStatus"] = AuthenticationStatus.ToString();
            //    //}
            //}

        }//finaize() 



    } //CswWebSvcRetJObj

} // namespace ChemSW.Nbt.WebServices

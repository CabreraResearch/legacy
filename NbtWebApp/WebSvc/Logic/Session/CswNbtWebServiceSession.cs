using System;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceSession
    {
        /// <summary>
        /// Session Return Object
        /// </summary>
        [DataContract]
        public class CswNbtSessionReturn : CswWebSvcReturn
        {
            public CswNbtSessionReturn()
            {
                Data = new CswWebSvcReturnBase.Data();
            }
            [DataMember]
            public CswWebSvcReturnBase.Data Data;
        }

        [DataContract]
        public class CswNbtAuthReturn : CswWebSvcReturn
        {
            public CswNbtAuthReturn()
            {
                Data = new CswNbtUserDefaultsData();
            }
            [DataMember]
            public CswNbtUserDefaultsData Data;
        }

        /// <summary>
        /// Return Object for Login Data
        /// </summary>
        [DataContract]
        public class LoginDataReturn : CswWebSvcReturn
        {
            public LoginDataReturn()
            {
                Data = new LoginData();
            }
            [DataMember]
            public LoginData Data;
        }

        [DataContract]
        public class CswNbtUserDefaultsData : CswWebSvcReturnBase.Data
        {
            [DataMember]
            public string DefaultLocationId;
            [DataMember]
            public string DefaultPrinterId;
            [DataMember]
            public string JurisdictionId;
            [DataMember]
            public string WorkUnitId;
            [DataMember]
            public string DateFormat;
            [DataMember]
            public string TimeFormat;
        }


        public static void getDefaults( ICswResources CswResources, CswNbtAuthReturn Ret, CswWebSvcSessionAuthenticateData.Authentication.Request Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( CswTools.IsPrimaryKey( NbtResources.CurrentNbtUser.DefaultLocationId ) )
            {
                Ret.Data.DefaultLocationId = NbtResources.CurrentNbtUser.DefaultLocationId.ToString();
            }
            if( CswTools.IsPrimaryKey( NbtResources.CurrentNbtUser.DefaultPrinterId ) )
            {
                Ret.Data.DefaultPrinterId = NbtResources.CurrentNbtUser.DefaultPrinterId.ToString();
            }
            if( CswTools.IsPrimaryKey( NbtResources.CurrentNbtUser.JurisdictionId ) )
            {
                Ret.Data.JurisdictionId = NbtResources.CurrentNbtUser.JurisdictionId.ToString();
            }
            if( CswTools.IsPrimaryKey( NbtResources.CurrentNbtUser.WorkUnitId ) )
            {
                Ret.Data.WorkUnitId = NbtResources.CurrentNbtUser.WorkUnitId.ToString();
            }
            Ret.Data.DateFormat = NbtResources.CurrentNbtUser.DateFormat;
            Ret.Data.TimeFormat = NbtResources.CurrentNbtUser.TimeFormat;
        }

        public static void doNothing( ICswResources CswResources, object Ret, object Req )
        {
            // like he says
        }

        public static void deauthenticate( ICswResources CswResources, object Ret, object Request )
        {
            if( null != CswResources )
            {
                CswNbtResources Resources = (CswNbtResources) CswResources;
                Resources.CswSessionManager.clearSession();
            }
        }

        /// <summary>
        /// WCF wrapper around resetPassword
        /// </summary>
        public static void resetPassword( ICswResources CswResources, CswNbtSessionReturn Ret, CswWebSvcSessionAuthenticateData.Authentication.Response.Expired Request )
        {
            Ret.Data.Succeeded = false;
            try
            {
                if( false == string.IsNullOrEmpty( Request.NewPassword ) )
                {
                    CswNbtResources Resources = (CswNbtResources) CswResources;

                    CswNbtObjClassUser User = Resources.Nodes[Request.UserId];
                    if( null != User )
                    {
                        User.PasswordProperty.Password = Request.NewPassword;
                        User.postChanges( ForceUpdate: false );
                        Ret.Data.Succeeded = true;
                    }
                }
            }
            catch( Exception Ex )
            {
                Ret.addException( Ex );
            }
        }

        /// <summary>
        /// Gets all login data for the current accessid in the given timeframe
        /// </summary>
        public static void getLoginData( ICswResources CswResources, LoginDataReturn Return, LoginData.LoginDataRequest Request )
        {
            CswNbtActLoginData _CswNbtActLoginData = new CswNbtActLoginData( (CswNbtResources) CswResources );
            Return.Data = _CswNbtActLoginData.getLoginData( Request );
        }

        /// <summary>
        /// Adds a new login record
        /// </summary>
        public static void postLoginData( ICswResources CswResources, LoginDataReturn Return, LoginData.Login Request )
        {
            CswNbtActLoginData _CswNbtActLoginData = new CswNbtActLoginData( (CswNbtResources) CswResources );
            _CswNbtActLoginData.postLoginData( Request );
        }
    } // class CswNbtWebServiceSession

} // namespace ChemSW.Nbt.WebServices

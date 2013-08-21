using System;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;
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

        [DataContract]
        public class SchemaDetails
        {
            [DataMember]
            public bool CorrectVersion;
            [DataMember]
            public CswWebSvcReturnBase.ErrorMessage ErrorMessage;
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
            public CswNbtUserDefaultsData()
            {
                SchemaData = new SchemaDetails();
            }
            [DataMember]
            public string DefaultLocationId;
            [DataMember]
            public string DefaultPrinterId;
            [DataMember]
            public string DefaultBalanceId;
            [DataMember]
            public string JurisdictionId;
            [DataMember]
            public string WorkUnitId;
            [DataMember]
            public string DateFormat;
            [DataMember]
            public string TimeFormat;
            [DataMember]
            public SchemaDetails SchemaData;
        }

        private static SchemaDetails _checkSchemaVersion( CswNbtResources CswNbtResources )
        {
            SchemaDetails Ret = new SchemaDetails(); ;

            CswSchemaScriptsProd CswSchemaScriptsProd = new CswSchemaScriptsProd( CswNbtResources );

            CswSchemaVersion CurrentVersion = CswSchemaScriptsProd.CurrentVersion( CswNbtResources );
            CswSchemaVersion LatestVersion = CswSchemaScriptsProd.LatestVersion;

            if( CurrentVersion < LatestVersion )
            {
                Ret.CorrectVersion = false;
                CswWebSvcReturnBase.ErrorMessage Error = new CswWebSvcReturnBase.ErrorMessage();
                Error.Type = CswEnumErrorType.Error;
                Error.Message = "The current schema is not updated to the latest version. The application may not work correctly. Please contact your adminstrator.";
                Error.Detail = "The current schema is at version " + CurrentVersion + " and the latest version is version " + LatestVersion + ".";
                Ret.ErrorMessage = Error;
            }
            else
            {
                Ret.CorrectVersion = true;
            }

            return Ret;
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
            if( CswTools.IsPrimaryKey( NbtResources.CurrentNbtUser.DefaultBalanceId ) )
            {
                Ret.Data.DefaultBalanceId = NbtResources.CurrentNbtUser.DefaultBalanceId.ToString();
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

            SchemaDetails SchemaDetails = _checkSchemaVersion( NbtResources );
            Ret.Data.SchemaData = SchemaDetails;
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
            CswNbtResources _CswNbtResources = ( CswNbtResources ) CswResources;
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                CswNbtActLoginData _CswNbtActLoginData = new CswNbtActLoginData( _CswNbtResources );
                Return.Data = _CswNbtActLoginData.getLoginData( Request );
            }
        }
    } // class CswNbtWebServiceSession

} // namespace ChemSW.Nbt.WebServices

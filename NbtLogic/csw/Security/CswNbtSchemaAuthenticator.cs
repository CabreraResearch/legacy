using System;
using ChemSW.Encryption;
using ChemSW.Nbt.csw.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.Security
{
    public class CswNbtSchemaAuthenticator: ICswSchemaAuthenticater
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtSchemaAuthenticator( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        public ICswUser getUser( string UserName )
        {
            return new CswNbtUser( _CswNbtResources, UserName );
        }

        public CswEnumAuthenticationStatus AuthenticateWithSchema( CswEncryption CswEncryption, CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest, out ICswUser AuthenticatedUser )
        {
            CswNbtSchemaAuthenticationLogic AuthenticationLogic = new CswNbtSchemaAuthenticationLogic( _CswNbtResources );
            CswNbtObjClassUser UserNode = null;
            if( AuthenticationRequest.AuthenticationStatus != CswEnumAuthenticationStatus.TooManyUsers )
            {
                UserNode = _authorizeUser( CswEncryption, AuthenticationRequest );
                AuthenticationRequest.AuthenticationStatus = AuthenticationLogic.GetAuthStatus( UserNode );
            }
            AuthenticationLogic.LogAuthenticationAttempt( UserNode, AuthenticationRequest );
            AuthenticatedUser = UserNode;
            return AuthenticationRequest.AuthenticationStatus;
        }

        private CswNbtObjClassUser _authorizeUser( CswEncryption CswEncryption, CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest )
        {
            CswNbtObjClassUser UserNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( AuthenticationRequest.UserName, RequireViewPermissions : false );
            if( UserNode != null )
            {
                string encryptedpassword = CswEncryption.getMd5Hash( AuthenticationRequest.Password );
                if( UserNode.EncryptedPassword == encryptedpassword )
                {
                    UserNode.clearFailedLoginCount();
                    UserNode.LastLogin.DateTimeValue = DateTime.Now;
                }
                else
                {
                    UserNode.incFailedLoginCount();
                }
                UserNode.postChanges( false );
            }
            return UserNode;
        }

    }//CswNbtAuthenticator
}//ChemSW.Nbt

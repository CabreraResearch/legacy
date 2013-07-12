using System;
using ChemSW.Encryption;
using ChemSW.Nbt.csw.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;

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

        public CswEnumAuthenticationStatus AuthenticateWithSchema( CswEncryption CswEncryption, string username, string password, string IPAddress, CswEnumAuthenticationStatus AuthStatus, out ICswUser AuthenticatedUser )
        {
            CswNbtSchemaAuthenticationLogic AuthenticationLogic = new CswNbtSchemaAuthenticationLogic( _CswNbtResources );
            CswNbtObjClassUser UserNode = null;
            if( AuthStatus != CswEnumAuthenticationStatus.TooManyUsers )
            {
                UserNode = _authorizeUser( CswEncryption, username, password );
                AuthStatus = AuthenticationLogic.GetAuthStatus( UserNode );
            }
            AuthenticationLogic.LogAuthenticationAttempt( UserNode, username, IPAddress, AuthStatus );
            AuthenticatedUser = UserNode;
            return AuthStatus;
        }

        private CswNbtObjClassUser _authorizeUser( CswEncryption CswEncryption, string username, string password )
        {
            CswNbtObjClassUser UserNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( username, RequireViewPermissions : false );
            if( UserNode != null && false == UserNode.IsArchived() && false == UserNode.IsAccountLocked() )
            {
                string encryptedpassword = CswEncryption.getMd5Hash( password );
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

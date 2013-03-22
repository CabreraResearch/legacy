using System;
using ChemSW.Encryption;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;

namespace ChemSW.Nbt.Security
{
    public class CswNbtSchemaAuthenticator : ICswSchemaAuthenticater
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

        public AuthenticationStatus AuthenticateWithSchema( CswEncryption CswEncryption, string username, string password, string IPAddress, out ICswUser AuthenticatedUser )
        {
            CswNbtObjClassUser UserNode = _authorizeUser( CswEncryption, username, password );
            AuthenticationStatus AuthStatus = _getAuthStatus( UserNode );
            _logAuthenticationAttempt( UserNode, username, IPAddress, AuthStatus );
            AuthenticatedUser = UserNode;
            return AuthStatus;
        }

        private CswNbtObjClassUser _authorizeUser( CswEncryption CswEncryption, string username, string password )
        {
            CswNbtObjClassUser UserNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( username, RequireViewPermissions: false );
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

        private AuthenticationStatus _getAuthStatus( CswNbtObjClassUser UserNode )
        {
            AuthenticationStatus AuthStatus = AuthenticationStatus.Failed;
            if( UserNode == null )
            {
                AuthStatus = AuthenticationStatus.Failed;
            }
            else if( UserNode.IsArchived() )
            {
                AuthStatus = AuthenticationStatus.Archived;
            }
            else if( UserNode.IsAccountLocked() )
            {
                AuthStatus = AuthenticationStatus.Locked;
            }
            else if( UserNode.getFailedLoginCount() == 0 )
            {
                AuthStatus = AuthenticationStatus.Authenticated;
            }
            return AuthStatus;
        }

        private void _logAuthenticationAttempt( CswNbtObjClassUser UserNode, String username, String IPAddress, AuthenticationStatus AuthStatus )
        {
            Int32 FailedLoginCount = null != UserNode ? UserNode.getFailedLoginCount() : 0;
            AuthStatus = UserNode == null ? (AuthenticationStatus) AuthenticationStatus.Unknown : AuthStatus;

            LoginData.Login LoginRecord = new LoginData.Login
            {
                Username = username,
                IPAddress = IPAddress,
                LoginDate = DateTime.Now.ToString(),
                LoginStatus = "Failed",
                FailureReason = "",
                FailedLoginCount = FailedLoginCount
            };

            LoginRecord.setStatus( AuthStatus );

            CswNbtActLoginData _CswNbtActLoginData = new CswNbtActLoginData( _CswNbtResources );
            _CswNbtActLoginData.postLoginData( LoginRecord );
        }

    }//CswNbtAuthenticator
}//ChemSW.Nbt

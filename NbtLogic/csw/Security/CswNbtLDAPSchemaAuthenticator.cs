using System;
using System.DirectoryServices;
using ChemSW.Encryption;
using ChemSW.Nbt.csw.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;

namespace ChemSW.Nbt.Security
{
    public class CswNbtLDAPSchemaAuthenticator: ICswSchemaAuthenticater
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtLDAPSchemaAuthenticator( CswNbtResources Resources )
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
                bool authenticated = _authenticateActiveDirectory( "CN", username, password );
                if( authenticated )
                {
                    UserNode.clearFailedLoginCount();
                    UserNode.LastLogin.DateTimeValue = DateTime.Now;
                    //TODO: sync attribute data with user node
                }
                else
                {
                    UserNode.incFailedLoginCount();
                }
                UserNode.postChanges( false );
            }
            return UserNode;
        }

        private bool _authenticateActiveDirectory( string domain, string username, string password )
        {
            bool ret = true;

            try
            {
                using( DirectoryEntry entry = new DirectoryEntry( "LDAP://" + domain, domain + "\\" + username, password ) )
                {
                    DirectorySearcher search = new DirectorySearcher()
                        {
                            SearchRoot = entry
                        };
                    search.Filter = "(SAMAccountName=" + username + ")";
                    search.PropertiesToLoad.Add( domain );
                    SearchResult result = search.FindOne();
                    if( result.Equals( null ) )
                    {
                        ret = false;
                    }
                }
            }
            catch 
            {
                //if we're here, the user is likely not logged into the VPN
                ret = false;
            }

            return ret;
        }

    }//CswNbtAuthenticator
}//ChemSW.Nbt

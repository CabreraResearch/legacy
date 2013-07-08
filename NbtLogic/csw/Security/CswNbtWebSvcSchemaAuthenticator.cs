using System;
using System.ServiceModel;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Nbt.csw.Security;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;

namespace ChemSW.Nbt.Security
{
    public class CswNbtWebSvcSchemaAuthenticator: ICswSchemaAuthenticater
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtWebSvcSchemaAuthenticator( CswNbtResources Resources )
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
                CswAuthorizationToken token = _authenticate( username, password );
                if( null != token )
                {
                    if( token.Authorized )
                    {
                        UserNode.clearFailedLoginCount();
                        UserNode.LastLogin.DateTimeValue = DateTime.Now;

                        if( null != token.UserId )
                        {
                            CswNbtMetaDataNodeTypeProp EmployeeIdNTP = UserNode.NodeType.getNodeTypeProp( "Employee Id" );
                            UserNode.Node.Properties[EmployeeIdNTP].AsText.Text = token.UserId; //TODO: promote Employee Id to OCP
                        }
                        if( null != token.FirstName )
                        {
                            UserNode.FirstNameProperty.Text = token.FirstName;
                        }
                        if( null != token.LastName )
                        {
                            UserNode.LastNameProperty.Text = token.LastName;
                        }
                        if( null != token.Email )
                        {
                            UserNode.EmailProperty.Text = token.Email;
                        }
                    }
                    else if( false == string.IsNullOrEmpty( token.ErrorMsg ) )
                    {
                        _CswNbtResources.logMessage( token.ErrorMsg );
                    }
                    else
                    {
                        UserNode.incFailedLoginCount();
                    }
                }
                UserNode.postChanges( false );
            }
            return UserNode;
        }

        private CswAuthorizationToken _authenticate( string username, string password )
        {
            CswAuthorizationToken Token = null;

            BasicHttpBinding AuthorizationBinding = new BasicHttpBinding();
            EndpointAddress AuthorizationEndpoint = new EndpointAddress( _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.WebSvcAuthorizationPath] );
            var AuthorizationChannelFactory = new ChannelFactory<ICswAuthorizationWebSvc>( AuthorizationBinding, AuthorizationEndpoint );
            ICswAuthorizationWebSvc Service = AuthorizationChannelFactory.CreateChannel();
            Token = Service.Get( username, password );
            return Token;
        }

    }//CswNbtAuthenticator
}//ChemSW.Nbt

using System;
using System.ServiceModel;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Nbt.csw.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;
using ChemSW.WebSvc;

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
            CswNbtObjClassUser ret = null;
            CswNbtObjClassUser UserNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( AuthenticationRequest.UserName, RequireViewPermissions : false );
            if( UserNode != null && false == UserNode.IsArchived() && false == UserNode.IsAccountLocked() )
            {
                CswAuthorizationToken token = _authenticate( AuthenticationRequest );
                if( null != token )
                {
                    if( token.Authorized )
                    {
                        UserNode.LastLogin.DateTimeValue = DateTime.Now;

                        if( null != token.UserId )
                        {
                            UserNode.EmployeeId.Text = token.UserId;
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
                        UserNode.postChanges( false );
                        ret = UserNode;
                    }
                    else if( false == string.IsNullOrEmpty( token.ErrorMsg ) )
                    {
                        _CswNbtResources.logMessage( token.ErrorMsg );
                    }
                }
            }
            return ret;
        }

        private CswAuthorizationToken _authenticate( CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest )
        {
            CswAuthorizationToken Token = null;

            BasicHttpBinding AuthorizationBinding = new BasicHttpBinding();
            EndpointAddress AuthorizationEndpoint = new EndpointAddress( _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.WebSvcAuthorizationPath] );
            var AuthorizationChannelFactory = new ChannelFactory<ICswAuthorizationWebSvc>( AuthorizationBinding, AuthorizationEndpoint );
            ICswAuthorizationWebSvc Service = AuthorizationChannelFactory.CreateChannel();
            Token = Service.Get( AuthenticationRequest );
            return Token;
        }

    }//CswNbtAuthenticator
}//ChemSW.Nbt

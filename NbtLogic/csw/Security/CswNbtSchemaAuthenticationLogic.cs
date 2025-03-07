﻿
using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.csw.Security
{
    public class CswNbtSchemaAuthenticationLogic
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtSchemaAuthenticationLogic( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        public CswEnumAuthenticationStatus GetAuthStatus( CswNbtObjClassUser UserNode )
        {
            CswEnumAuthenticationStatus AuthStatus = CswEnumAuthenticationStatus.Failed;
            if( UserNode == null )
            {
                AuthStatus = CswEnumAuthenticationStatus.Failed;
            }
            else if( UserNode.getFailedLoginCount() == 0 )
            {
                AuthStatus = CswEnumAuthenticationStatus.Authenticated;
                if( UserNode.IsArchived() )
                {
                    AuthStatus = CswEnumAuthenticationStatus.Archived;
                }
                else if( UserNode.IsAccountLocked() )
                {
                    AuthStatus = CswEnumAuthenticationStatus.Locked;
                }
            }
            return AuthStatus;
        }

        public void LogAuthenticationAttempt( CswNbtObjClassUser UserNode, CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest )
        {
            if( false == AuthenticationRequest.SuppressLog )
            {
                Int32 FailedLoginCount = null != UserNode ? UserNode.getFailedLoginCount() : 0;
                LoginData.Login LoginRecord = new LoginData.Login( AuthenticationRequest, UserNode )
                    {
                        LoginDate = DateTime.Now.ToString(),
                        FailedLoginCount = FailedLoginCount
                    };
                CswNbtActLoginData _CswNbtActLoginData = new CswNbtActLoginData( _CswNbtResources );
                _CswNbtActLoginData.postLoginData( LoginRecord );
            }
        }

    }
}

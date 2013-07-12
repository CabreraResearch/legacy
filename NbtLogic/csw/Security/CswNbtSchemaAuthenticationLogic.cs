
using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;

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
            else if( UserNode.IsArchived() )
            {
                AuthStatus = CswEnumAuthenticationStatus.Archived;
            }
            else if( UserNode.IsAccountLocked() )
            {
                AuthStatus = CswEnumAuthenticationStatus.Locked;
            }
            else if( UserNode.getFailedLoginCount() == 0 )
            {
                AuthStatus = CswEnumAuthenticationStatus.Authenticated;
            }
            return AuthStatus;
        }

        public void LogAuthenticationAttempt( CswNbtObjClassUser UserNode, String username, String IPAddress, CswEnumAuthenticationStatus AuthStatus )
        {
            Int32 FailedLoginCount = null != UserNode ? UserNode.getFailedLoginCount() : 0;
            if( AuthStatus != CswEnumAuthenticationStatus.TooManyUsers )
            {
                AuthStatus = UserNode == null ? (CswEnumAuthenticationStatus) CswEnumAuthenticationStatus.Unknown : AuthStatus;
            }

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

    }
}

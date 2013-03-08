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

        private ICswUser _User = null;
        public ICswUser getUser( string UserName )
        {
            return new CswNbtUser( _CswNbtResources, UserName );
        }

        public AuthenticationStatus AuthenticateWithSchema( CswEncryption CswEncryption, string username, string password, string IPAddress, out ICswUser AuthenticatedUser )
        {
            AuthenticationStatus ReturnVal = AuthenticationStatus.Failed;
            _User = null;

            LoginData.Login LoginRecord = new LoginData.Login
            {
                Username = username,
                IPAddress = IPAddress,
                LoginDate = DateTime.Now.ToString(),
                LoginStatus = "Failed",
                FailureReason = "",
                FailedLoginCount = 0
            };

            CswNbtObjClassUser UserNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( username, false );   // can't require permissions if we aren't authenticated yet
            if( UserNode != null )
            {
                if( false == UserNode.IsArchived() )
                {
                    if( false == UserNode.IsAccountLocked() )
                    {
                        string encryptedpassword = CswEncryption.getMd5Hash( password );
                        if( UserNode.EncryptedPassword == encryptedpassword )
                        {
                            UserNode.clearFailedLoginCount();
                            UserNode.LastLogin.DateTimeValue = DateTime.Now;
                            _User = UserNode;
                            ReturnVal = AuthenticationStatus.Authenticated;
                            LoginRecord.LoginStatus = "Success";
                        }
                        else
                        {
                            UserNode.incFailedLoginCount();
                            ReturnVal = AuthenticationStatus.Failed;
                        }
                        LoginRecord.FailedLoginCount = UserNode.getFailedLoginCount();
                        UserNode.postChanges( false );
                    }
                    else
                    {
                        ReturnVal = AuthenticationStatus.Locked;
                    }
                }
                else
                {
                    ReturnVal = AuthenticationStatus.Archived;
                }
                LoginRecord.setFailureReason( ReturnVal );
            }
            else
            {
                LoginRecord.setFailureReason( AuthenticationStatus.Unknown );
            }
            
            CswNbtActLoginData _CswNbtActLoginData = new CswNbtActLoginData( _CswNbtResources );
            _CswNbtActLoginData.postLoginData( LoginRecord );

            AuthenticatedUser = _User;
            return ( ReturnVal );
        } // AuthenticateWithSchema()
    }//CswNbtAuthenticator
}//ChemSW.Nbt

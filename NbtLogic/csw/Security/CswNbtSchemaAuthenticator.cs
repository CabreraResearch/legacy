using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Configuration;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Exceptions;
using ChemSW.Session;
using System.Text.RegularExpressions;
using ChemSW.Security;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;

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
            if( _User == null || _User.Username != UserName )
                _User = CswNbtNodeCaster.AsUser( _CswNbtResources.Nodes.makeUserNodeFromUsername( UserName ) ) as ICswUser;
            return _User;
        }

        public AuthenticationStatus AuthenticateWithSchema( CswEncryption CswEncryption, string username, string password, string IPAddress, out ICswUser AuthenticatedUser )
        {
            AuthenticationStatus ReturnVal = AuthenticationStatus.Failed;
            _User = null;
            
            CswNbtNode UserAsNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( username );
            if( UserAsNode != null )
            {
                CswNbtObjClassUser UserObjClass = CswNbtNodeCaster.AsUser( UserAsNode );
                if( !UserObjClass.IsAccountLocked() )
                {
                    string encryptedpassword = CswEncryption.getMd5Hash( password );
                    if( UserObjClass.EncryptedPassword == encryptedpassword )
                    {
                        UserObjClass.clearFailedLoginCount();
                        UserObjClass.LastLogin.DateValue = DateTime.Now;
                        _User = UserObjClass;
                        ReturnVal = AuthenticationStatus.Authenticated;
                    }
                    else
                    {
                        UserObjClass.incFailedLoginCount();
                        UserObjClass = null;
                        ReturnVal = AuthenticationStatus.Failed;
                    }

                    //bz # 6555
                    UserAsNode.postChanges( false );
                } // if (!User.IsAccountLocked())
                else
                {
                    ReturnVal = AuthenticationStatus.Locked;
                }

            } // if (User != null)

            AuthenticatedUser = _User;
            return ( ReturnVal );

        } // AuthenticateWithSchema()

    }//CswNbtAuthenticator

}//ChemSW.Nbt


using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.csw.Security
{
    public class CswNbtSchemaAuthenticatorFactory
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtSchemaAuthenticatorFactory( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public ICswSchemaAuthenticater Make( ICswSetupVbls SetupVbls )
        {
            string LDAP_dll_Path = SetupVbls[CswEnumSetupVariableNames.LDAPAuthenticationDllPath];
            if( string.IsNullOrEmpty( LDAP_dll_Path ) )
            {
                return  new CswNbtSchemaAuthenticator( _CswNbtResources );
            }
            else
            {
                if( File.Exists( LDAP_dll_Path ) )
                {
                    return new CswNbtLDAPSchemaAuthenticator( _CswNbtResources );
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot authenticate via LDAP, authentication dll is missing.", "The customer supplied DLL to fetch User attributes is missing." );
                }
            }
        }
    }
}


using ChemSW.Config;
using ChemSW.Core;
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
            string WebSvcAuthorizationPath = SetupVbls[CswEnumSetupVariableNames.WebSvcAuthorizationPath];
            if( string.IsNullOrEmpty( WebSvcAuthorizationPath ) )
            {
                return new CswNbtSchemaAuthenticator( _CswNbtResources );
            }
            else
            {
                return new CswNbtWebSvcSchemaAuthenticator( _CswNbtResources );
            }
        }
    }
}

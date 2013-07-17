using System.ServiceModel;
using ChemSW.Config;
using ChemSW.Core;

namespace ChemSW.Nbt.ChemCatCentral
{

    public class CswNbtC3ClientManager
    {

        private CswNbtResources _CswNbtResources = null;
        private CswC3Params _CswC3Params = null;
        private CswC3SearchParams _CswC3SearchParams = null;

        public CswNbtC3ClientManager( CswNbtResources CswNbtResources, CswC3Params CswC3Params )
        {
            _CswNbtResources = CswNbtResources;
            _CswC3Params = CswC3Params;

        }//ctor1

        public CswNbtC3ClientManager( CswNbtResources CswNbtResources, CswC3SearchParams CswC3SearchParams )
        {
            _CswNbtResources = CswNbtResources;
            _CswC3SearchParams = CswC3SearchParams;

        }//ctor2

        public CswNbtC3ClientManager( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

        }//ctor3

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChemCatCentral.SearchClient initializeC3Client()
        {
            if( null != _CswC3Params )
            {
                _setConfigurationVariables( _CswC3Params, _CswNbtResources );
            }
            else if( null != _CswC3SearchParams )
            {
                _setConfigurationVariables( _CswC3SearchParams, _CswNbtResources );
            }

            ChemCatCentral.SearchClient C3SearchClient = new ChemCatCentral.SearchClient();
            _setEndpointAddress( _CswNbtResources, C3SearchClient );

            return C3SearchClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CswNbtResources"></param>
        /// <returns></returns>
        public bool checkC3ServiceReferenceStatus()
        {
            bool Status = true;

            try
            {
                ChemCatCentral.SearchClient C3ServiceTest = initializeC3Client();
                C3ServiceTest.isAlive();
            }
            catch
            {
                Status = false;
            }

            return Status;
        }

        public string getCurrentC3Version()
        {
            string CurrentVersion = string.Empty;

            ChemCatCentral.SearchClient C3Service = initializeC3Client();
            C3Service.isAlive();
            CurrentVersion = C3Service.getCurrentVersion();

            return CurrentVersion;
        }

        public string getLastExtChemDataImportDate( SearchClient SearchClient )
        {
            string Ret = string.Empty;

            CswRetObjSearchResults ReturnObject = SearchClient.getLastExtChemDataImportDate( _CswC3Params );
            Ret = ReturnObject.LastExtChemDataImportDate;

            return Ret;
        }

        /// <summary>
        /// Set the c3 parameter object's CustomerLoginName, LoginPassword, and AccessId
        /// parameters using the values from the configuration_variables table in the db.
        /// </summary>
        /// <param name="CswC3Params"></param>
        private static void _setConfigurationVariables( CswC3Params CswC3Params, CswNbtResources _CswNbtResources )
        {

            CswC3Params.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Username );
            CswC3Params.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Password );
            CswC3Params.AccessId = _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.C3AccessId];

        }

        /// <summary>
        /// Set the c3 search parameter object's CustomerLoginName, LoginPassword, and AccessId
        /// parameters using the values from the configuration_variables table in the db.
        /// </summary>
        /// <param name="CswC3SearchParams"></param>
        private static void _setConfigurationVariables( CswC3SearchParams CswC3SearchParams, CswNbtResources _CswNbtResources )
        {
            CswC3SearchParams.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Username );
            CswC3SearchParams.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Password );
            CswC3SearchParams.AccessId = _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.C3AccessId];
            CswC3SearchParams.MaxRows = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "treeview_resultlimit" ) );
        }

        /// <summary>
        /// Dynamically set the endpoint address for a ChemCatCentral SearchClient.
        /// </summary>
        /// <param name="CswNbtResources"></param>
        /// <param name="C3SearchClient"></param>
        private static void _setEndpointAddress( CswNbtResources CswNbtResources, ChemCatCentral.SearchClient C3SearchClient )
        {
            if( null != C3SearchClient )
            {
                string C3_UrlStem = CswNbtResources.SetupVbls[CswEnumSetupVariableNames.C3UrlStem];
                EndpointAddress URI = new EndpointAddress( C3_UrlStem );
                C3SearchClient.Endpoint.Address = URI;

                //string Protocol = Path.GetPathRoot( C3_UrlStem );
                if( false == string.IsNullOrEmpty( C3_UrlStem ) )
                {
                    if( C3_UrlStem.StartsWith( "https://" ) )
                    {
                        WebHttpBinding SecureBinding = new WebHttpBinding( "chemCatSSL" );
                        C3SearchClient.Endpoint.Binding = SecureBinding;
                    }
                }
            }
        }


    }//CswNbtC3ClientManager

}//namespace ChemSW.Nbt.PropTypes

using System;
using System.ServiceModel;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.ChemCatCentral
{

    public class CswNbtC3ClientManager
    {

        private CswNbtResources _CswNbtResources;
        private CswC3Params _CswC3Params;
        private CswC3SearchParams _CswC3SearchParams;

        private string _Message;
        private CswEnumErrorType _MessageType;

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
        public SearchClient initializeC3Client( CswEnumErrorType MessageType = null, string Message = "" )
        {
            _Message = Message;
            _MessageType = MessageType;

            SearchClient Ret = new SearchClient();
            bool Success = true;

            // Set the configuration variables
            if( null != _CswC3Params )
            {
                Success = _setConfigurationVariables( _CswC3Params, _CswNbtResources );
            }
            else if( null != _CswC3SearchParams )
            {
                Success = _setConfigurationVariables( _CswC3SearchParams, _CswNbtResources );
            }

            // Set the endpoint address 
            Success = Success && _setEndpointAddress( _CswNbtResources, Ret );

            if( Success )
            {
                return Ret;
            }
            return null;
        }//initializeC3Client()

        private bool _checkC3ServiceReferenceStatus( SearchClient SearchClient )
        {
            bool Status = false;

            try
            {
                Status = SearchClient.isAlive();
            }
            catch( Exception e )
            {
                _createMessage( _MessageType ?? CswEnumErrorType.Error, ( _Message ?? "Endpoint address incorrect or ChemCatCentral service not available: " ) + e );
            }

            return Status;
        }//checkC3ServiceReferenceStatus()

        public string getCurrentC3Version()
        {
            string CurrentVersion = string.Empty;

            SearchClient C3Service = initializeC3Client();
            if( null != C3Service )
            {
                C3Service.isAlive();
                CurrentVersion = C3Service.getCurrentVersion();
            }

            return CurrentVersion;
        }//getCurrentC3Version()

        public CswC3ServiceLogicGetDataSourcesDataSource[] getDataSourceDates()
        {
            CswC3ServiceLogicGetDataSourcesDataSource[] DataSourceDates = null;

            SearchClient C3Service = initializeC3Client();
            if( null != C3Service )
            {
                C3Service.isAlive();
                CswRetObjSearchResults DataSources = C3Service.getDataSourceDates( _CswC3Params );

                if( null != DataSources.DataSourceDates )
                {
                    DataSourceDates = DataSources.DataSourceDates.Data;
                }
            }

            return DataSourceDates;
        }

        public string getLastExtChemDataImportDate( SearchClient SearchClient )
        {
            string Ret = string.Empty;

            CswRetObjSearchResults ReturnObject = SearchClient.getLastExtChemDataImportDate( _CswC3Params );
            Ret = ReturnObject.LastExtChemDataImportDate;

            return Ret;
        }//getLastExtChemDataImportDate()

        /// <summary>
        /// Get the most recent LOLI data import date.
        /// </summary>
        /// <param name="SearchClient"></param>
        /// <returns></returns>
        public string getLastLOLIImportDate( SearchClient SearchClient )
        {
            string Ret = string.Empty;

            CswRetObjSearchResults ReturnObject = SearchClient.getLastLOLIImportDate( _CswC3Params );
            Ret = ReturnObject.LastLOLIImportDate;

            return Ret;
        }//getLastLOLIImportDate()

        /// <summary>
        /// Set the c3 parameter object's CustomerLoginName, LoginPassword, and AccessId
        /// parameters using the values from the configuration_variables table in the db.
        /// </summary>
        /// <param name="CswC3Params"></param>
        private bool _setConfigurationVariables( CswC3Params CswC3Params, CswNbtResources _CswNbtResources )
        {
            bool Ret = true;

            CswC3Params.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Username );
            CswC3Params.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Password );
            CswC3Params.AccessId = _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.C3AccessId];

            if( string.IsNullOrEmpty( CswC3Params.CustomerLoginName ) || string.IsNullOrEmpty( CswC3Params.LoginPassword ) || string.IsNullOrEmpty( CswC3Params.AccessId ) )
            {
                _createMessage( CswEnumErrorType.Error, "C3_Username, C3_Password, or C3AccessId value is null" );
                Ret = false;
            }

            return Ret;

        }//_setConfigurationVariables()

        /// <summary>
        /// Set the c3 search parameter object's CustomerLoginName, LoginPassword, and AccessId
        /// parameters using the values from the configuration_variables table in the db.
        /// </summary>
        /// <param name="CswC3SearchParams"></param>
        private bool _setConfigurationVariables( CswC3SearchParams CswC3SearchParams, CswNbtResources _CswNbtResources )
        {
            bool Ret = true;

            CswC3SearchParams.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Username );
            CswC3SearchParams.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3_Password );
            CswC3SearchParams.AccessId = _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.C3AccessId];
            CswC3SearchParams.MaxRows = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "treeview_resultlimit" ) );

            if( string.IsNullOrEmpty( CswC3SearchParams.CustomerLoginName ) || string.IsNullOrEmpty( CswC3SearchParams.LoginPassword ) || string.IsNullOrEmpty( CswC3SearchParams.AccessId ) )
            {
                _createMessage( CswEnumErrorType.Error, "C3_Username, C3_Password, or C3AccessId value is null" );
                Ret = false;
            }

            return Ret;
        }//_setConfigurationVariables()

        /// <summary>
        /// Dynamically set the endpoint address for a ChemCatCentral SearchClient.
        /// </summary>
        /// <param name="CswNbtResources"></param>
        /// <param name="C3SearchClient"></param>
        private bool _setEndpointAddress( CswNbtResources CswNbtResources, SearchClient C3SearchClient )
        {
            bool Ret = true;

            if( null != C3SearchClient )
            {
                string C3_UrlStem = CswNbtResources.SetupVbls[CswEnumSetupVariableNames.C3UrlStem];
                if( false == string.IsNullOrEmpty( C3_UrlStem ) )
                {
                    EndpointAddress URI = new EndpointAddress( C3_UrlStem );
                    C3SearchClient.Endpoint.Address = URI;

                    if( C3_UrlStem.StartsWith( "https://" ) )
                    {
                        WebHttpBinding SecureBinding = new WebHttpBinding( "chemCatSSL" );
                        C3SearchClient.Endpoint.Binding = SecureBinding;
                    }

                    // Check the service reference
                    if( false == _checkC3ServiceReferenceStatus( C3SearchClient ) )
                    {
                        Ret = false;
                    }
                }
                else
                {
                    _createMessage( CswEnumErrorType.Error, "C3_UrlStem value is null" );
                    Ret = false;
                }
            }
            else
            {
                // This should never happen
                Ret = false;
            }

            return Ret;
        }//_setEndpointAddress()

        private void _createMessage( CswEnumErrorType ErrorType, string Detail, bool ShowError = true, string Message = "" )
        {
            if( string.IsNullOrEmpty( Message ) || string.IsNullOrEmpty( _Message ) )
            {
                Message = "Unable to connect to ChemCatCentral";
            }

            if( null != _MessageType ) { ErrorType = _MessageType; }

            CswWebSvcReturnBase.ErrorMessage MessageObj = new CswWebSvcReturnBase.ErrorMessage();
            MessageObj.Type = ErrorType;
            MessageObj.Message = Message;
            MessageObj.Detail = Detail;
            MessageObj.ShowError = ShowError;

            _CswNbtResources.Messages.Add( MessageObj );
        }//_createMessage()


    }//CswNbtC3ClientManager

}//namespace ChemSW.Nbt.PropTypes

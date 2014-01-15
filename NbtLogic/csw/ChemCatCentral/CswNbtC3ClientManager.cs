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
            _setRegulationDatabase();
            _setDataService();

        }//ctor2

        private string _RegulationDatabase;
        public string RegulationDatabase
        {
            get
            {
                return _RegulationDatabase;
            }
        }

        private void _setRegulationDatabase()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.LOLISync ) )
            {
                _RegulationDatabase = "LOLI";
            }
            else if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.ArielSync ) )
            {
                _RegulationDatabase = "Ariel";
            }
            else
            {
                _RegulationDatabase = string.Empty;
            }
        }

        private string _DataService;
        public string DataService
        {
            get { return _DataService; }
        }

        private void _setDataService()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3Products ) )
            {
                _DataService = "C3";
            }
            else if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3ACD ) )
            {
                _DataService = "ACD";
            }
            else
            {
                _DataService = string.Empty;
            }
        }

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

            return Success ? Ret : null;
        }//initializeC3Client()

        public string getCurrentC3Version()
        {
            string CurrentVersion = string.Empty;
            bool Status = false;

            SearchClient C3Service = initializeC3Client();
            if( null != C3Service )
            {
                Status = _checkConnection( C3Service );
                if( Status )
                {
                    CurrentVersion = C3Service.getCurrentVersion();
                }
            }

            return CurrentVersion;
        }//getCurrentC3Version()

        public CswC3ServiceLogicGetDataSourcesDataSource[] getDataSourceDates()
        {
            CswC3ServiceLogicGetDataSourcesDataSource[] DataSourceDates = null;
            bool Status = false;

            SearchClient C3Service = initializeC3Client();
            if( null != C3Service )
            {
                Status = _checkConnection( C3Service );
                if( Status )
                {
                    CswRetObjSearchResults DataSources = C3Service.getDataSourceDates( _CswC3Params );
                    if( null != DataSources.DataSourceDates )
                    {
                        DataSourceDates = DataSources.DataSourceDates.Data;
                    }
                }
            }

            return DataSourceDates;
        }

        /// <summary>
        /// Get the most recent ExtChemData import date.
        /// </summary>
        /// <param name="SearchClient"></param>
        /// <returns></returns>
        public string getLastExtChemDataImportDate( SearchClient SearchClient )
        {
            string Ret = string.Empty;

            CswRetObjSearchResults ReturnObject = SearchClient.getLastExtChemDataImportDate( _CswC3SearchParams );
            Ret = ReturnObject.LastExtChemDataImportDate;

            return Ret;
        }//getLastExtChemDataImportDate()

        /// <summary>
        /// Get the most recent Regulation database data import date.
        /// </summary>
        /// <param name="SearchClient"></param>
        /// <returns></returns>
        public string getLastRegulationDataImportDate( SearchClient SearchClient )
        {
            string Ret = string.Empty;

            if( false == string.IsNullOrEmpty( _RegulationDatabase ) )
            {
                // We set the Regulation Database so that C3 knows which date to retrieve
                _CswC3SearchParams.RegulationDatabase = _RegulationDatabase;
                CswRetObjSearchResults ReturnObject = SearchClient.getLastestRegDbDate( _CswC3SearchParams );
                Ret = ReturnObject.LastestRegulationDbDate;
            }

            return Ret;
        }//getLastRegulationDataImportDate()

        #region Private Helper Methods

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
            if( string.IsNullOrEmpty( Message ) && string.IsNullOrEmpty( _Message ) )
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

        private bool _checkC3ServiceReferenceStatus( SearchClient SearchClient )
        {
            bool Ret = false;

            try
            {
                Ret = _checkConnection( SearchClient );
            }
            catch( Exception e )
            {
                _createMessage( _MessageType ?? CswEnumErrorType.Error, ( _Message ?? "Endpoint address incorrect or ChemCatCentral service not available: " ) + e );
            }

            return Ret;
        }//checkC3ServiceReferenceStatus()

        private bool _checkConnection( SearchClient SearchClient )
        {
            bool Ret = false;
            CswWebSvcReturnBaseStatus Status = new CswWebSvcReturnBaseStatus();
            CswC3Params CswC3Params = new CswC3Params();

            if( null != _CswC3Params )
            {
                CswC3Params = _CswC3Params;
            }
            else
            {
                CswC3Params.AccessId = _CswC3SearchParams.AccessId;
                CswC3Params.CustomerLoginName = _CswC3SearchParams.CustomerLoginName;
                CswC3Params.LoginPassword = _CswC3SearchParams.LoginPassword;
            }

            Status = SearchClient.canConnect( CswC3Params );
            if( Status.Success )
            {
                Ret = true;
            }
            else
            {
                foreach( CswWebSvcReturnBaseErrorMessage Message in Status.Messages )
                {
                    _createMessage( Message.Type, Message.Detail, true, Message.Message );
                }
            }

            return Ret;
        }

        #endregion


    }//CswNbtC3ClientManager

}//namespace ChemSW.Nbt.PropTypes

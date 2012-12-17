using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Config;
using ChemSW.Nbt.ChemCatCentral;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceC3Search
    {
        #region Ctor

        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceC3Search( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Ctor

        #region DataContracts

        [DataContract]
        public class CswNbtC3SearchReturn : CswWebSvcReturn
        {
            public CswNbtC3SearchReturn()
            {
                Data = new C3SearchResponse();
            }

            [DataMember]
            public C3SearchResponse Data;
        }

        [DataContract]
        public class C3SearchResponse
        {
            [DataMember]
            public List<string> AvailableDataSources = new List<string>();

            [DataMember]
            public List<string> SearchTypes = new List<string>();

            [DataMember]
            public string SearchResults = string.Empty;
        }

        #endregion

        public static void GetAvailableDataSources( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswC3Params = new CswC3Params();

            CswC3Params.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue(
                CswConfigurationVariables.ConfigurationVariableNames.C3_Username );
            CswC3Params.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue(
                CswConfigurationVariables.ConfigurationVariableNames.C3_Password );
            CswC3Params.AccessId = _CswNbtResources.ConfigVbls.getConfigVariableValue(
                CswConfigurationVariables.ConfigurationVariableNames.C3_AccessId );


            ChemCatCentral.SearchClient C3Search = new ChemCatCentral.SearchClient();
            CswRetObjSearchResults SourcesList = C3Search.getDataSources( CswC3Params );
            List<string> newlist = new List<string>( SourcesList.AvailableDataSources );

            Return.Data.AvailableDataSources = newlist;

        }

        public static void GetSearchTypes( ICswResources CswResources, CswNbtC3SearchReturn Return,
                                           CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswC3Params = new CswC3Params();

            CswC3Params.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue(
                CswConfigurationVariables.ConfigurationVariableNames.C3_Username );
            CswC3Params.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue(
                CswConfigurationVariables.ConfigurationVariableNames.C3_Password );
            CswC3Params.AccessId = _CswNbtResources.ConfigVbls.getConfigVariableValue(
                CswConfigurationVariables.ConfigurationVariableNames.C3_AccessId );

            List<string> newlist = new List<string>();

            foreach( CswC3SearchParams.SearchFieldType SearchType in Enum.GetValues( typeof( CswC3SearchParams.SearchFieldType ) ) )
            {
                newlist.Add( SearchType.ToString() );
            }

            Return.Data.SearchTypes = newlist;

        }

        public static void RunChemCatCentralSearch( ICswResources CswResources, CswNbtC3SearchReturn Return,
                                                   CswC3SearchParams CswC3SearchParams )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswC3SearchParams.CustomerLoginName = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_Username );
            CswC3SearchParams.LoginPassword = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_Password );
            CswC3SearchParams.AccessId = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_AccessId );

            JObject ret = new JObject();

            ChemCatCentral.SearchClient C3Search = new ChemCatCentral.SearchClient();
            CswRetObjSearchResults SearchResults = C3Search.search( CswC3SearchParams );

            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, null, Int32.MinValue );
            ret["table"] = wsTable.getTable( SearchResults );
            ret["filters"] = "";
            ret["searchterm"] = CswC3SearchParams.Query;
            ret["filtersapplied"] = "";
            //Search.SaveToCache( true );
            ret["sessiondataid"] = "";
            ret["searchtype"] = "chemcatcentral";

            Return.Data.SearchResults = ret.ToString();

        }

        #region Public

        #endregion
    }

}
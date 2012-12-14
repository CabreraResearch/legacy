using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
            public string SearchResults = string.Empty;
        }

        #endregion

        public static void GetAvailableDataSources( ICswResources CswResources, CswNbtC3SearchReturn Return,
                                                   CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            ChemCatCentral.SearchClient C3Search = new ChemCatCentral.SearchClient();
            CswRetObjSearchResults SourcesList = C3Search.getDataSources( CswC3Params );
            List<string> newlist = new List<string>( SourcesList.AvailableDataSources );

            Return.Data.AvailableDataSources = newlist;

        }

        public static void RunChemCatCentralSearch( ICswResources CswResources, CswNbtC3SearchReturn Return,
                                                   CswC3SearchParams CswC3SearchParams )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

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
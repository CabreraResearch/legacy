using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceSearchMenu
    {
        #region Ctor

        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceSearchMenu( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Ctor

        #region DataContracts

        [DataContract]
        public class CswNbtSearchMenuReturn : CswWebSvcReturn
        {
            public CswNbtSearchMenuReturn()
            {
                Data = new SearchMenuResponse();
            }

            [DataMember]
            public SearchMenuResponse Data;
        }

        [DataContract]
        public class SearchMenuResponse
        {
            public SearchMenuResponse()
            {
                searchTypes = new Collection<SearchType>();
            }

            [DataMember] 
            public Collection<SearchType> searchTypes;

            [DataContract]
            public class SearchType
            {
                [DataMember]
                public string name = string.Empty;

                [DataMember]
                public string iconfilename = string.Empty;
            }
        }

        #endregion

        public static void GetSearchMenuItems( ICswResources CswResources, CswNbtSearchMenuReturn Return, object Request )
        {
            _CswNbtResources = (CswNbtResources) CswResources;

            Collection<SearchMenuResponse.SearchType> searchTypes = new Collection<SearchMenuResponse.SearchType>();

            SearchMenuResponse.SearchType ss = new SearchMenuResponse.SearchType();
            ss.name = "Structure Search";
            ss.iconfilename = "Images/newicons/16/atommag.png";
            searchTypes.Add(ss);

            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.C3 ) )
            {
                SearchMenuResponse.SearchType c3 = new SearchMenuResponse.SearchType();
                c3.name = "ChemCatCentral Search";
                c3.iconfilename = "Images/newicons/16/cat.png";
                searchTypes.Add(c3);
            }

            Return.Data.searchTypes = searchTypes;


        }

        #region Private helper methods

        #endregion
    }

}
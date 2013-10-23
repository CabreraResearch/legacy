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
        public class CswNbtSearchMenuReturn: CswWebSvcReturn
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
                searchTargets = new Collection<SearchTarget>();
            }

            [DataMember]
            public Collection<SearchTarget> searchTargets;

            [DataContract]
            public class SearchTarget
            {
                [DataMember]
                public string name = string.Empty;

                [DataMember]
                public string iconfilename = string.Empty;
            }
        }

        #endregion

        public static void GetSearchMenuItems( ICswResources CswResources, CswNbtSearchMenuReturn Return, bool universalSearchOnly )
        {
            _CswNbtResources = (CswNbtResources) CswResources;

            Collection<SearchMenuResponse.SearchTarget> SearchTargets = new Collection<SearchMenuResponse.SearchTarget>();

            if( false == universalSearchOnly )
            {
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
                {
                    SearchMenuResponse.SearchTarget ss = new SearchMenuResponse.SearchTarget();
                    ss.name = "Structure Search";
                    ss.iconfilename = "Images/newicons/16/atommag.png";
                    SearchTargets.Add( ss );
                }

                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3 ) )
                {
                    SearchMenuResponse.SearchTarget c3 = new SearchMenuResponse.SearchTarget();
                    c3.name = "ChemCatCentral Search";
                    c3.iconfilename = "Images/newicons/16/cat.png";
                    SearchTargets.Add( c3 );
                }
            }

            Return.Data.searchTargets = SearchTargets;


        }

        #region Private helper methods

        #endregion
    }

}
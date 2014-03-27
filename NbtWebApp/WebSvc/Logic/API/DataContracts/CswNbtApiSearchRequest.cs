using System.Runtime.Serialization;
using ChemSW.Core;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    public class CswNbtApiSearchRequest
    {
        [DataMember( Name = "query" )]
        public string Query { get; set; }

        public CswEnumSqlLikeMode SearchType = CswEnumSqlLikeMode.Begins;

        [DataMember( Name = "searchtype" )]
        private string _searchType
        {
            get { return SearchType.ToString(); }
            set { SearchType = (CswEnumSqlLikeMode) value; }
        }

        [DataMember( Name = "nodetype" )]
        public string NodeType { get; set; }

        public CswNbtApiSearchRequest( string query, string searchtype )
        {
            Query = query;
            SearchType = (CswEnumSqlLikeMode) searchtype;
        }
    }
}
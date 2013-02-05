using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Search
{
    /// <summary>
    /// Functions for fetching and handling Universal Searches
    /// </summary>
    public class CswNbtSearchManager
    {
        /// <summary>
        /// CswNbtResources reference
        /// </summary>
        protected CswNbtResources _CswNbtResources;

        public const string SearchTableName = "search";

        public CswNbtSearchManager(CswNbtResources Resources)
        {
            _CswNbtResources = Resources;
        }

        public Collection<CswNbtSearch> getSearches( ICswNbtUser User = null )
        {
            Collection<CswNbtSearch> ret = new Collection<CswNbtSearch>();
            if( null == User )
            {
                User = _CswNbtResources.CurrentNbtUser;
            }

            if( null != User && null != User.UserId && Int32.MinValue != User.UserId.PrimaryKey )
            {
                CswTableSelect SearchSelect = _CswNbtResources.makeCswTableSelect( "CswNbtSearchManager_getSearches", SearchTableName );
                DataTable SearchTable = SearchSelect.getTable( "userid", User.UserId.PrimaryKey );
                foreach( DataRow SearchRow in SearchTable.Rows )
                {
                    CswNbtSearch thisSearch = new CswNbtSearch( _CswNbtResources );
                    thisSearch.FromSearchRow( SearchRow );
                    ret.Add( thisSearch );
                }
            }
            return ret;
        } // getSearches()

        public CswNbtSearch restoreSearch(CswPrimaryKey SearchId)
        {
            CswNbtSearch ret = null;
            if( CswTools.IsPrimaryKey( SearchId ) )
            {
                CswTableSelect SearchSelect = _CswNbtResources.makeCswTableSelect( "CswNbtSearchManager_restoreSearch", SearchTableName );
                DataTable SearchTable = SearchSelect.getTable( "searchid", SearchId.PrimaryKey );
                if(SearchTable.Rows.Count > 0)
                {
                    ret = new CswNbtSearch( _CswNbtResources );
                    ret.FromSearchRow( SearchTable.Rows[0] );
                }
            }
            return ret;
        } // restoreSearch()

    } // class CswNbtSearchManager


} // namespace ChemSW.Nbt




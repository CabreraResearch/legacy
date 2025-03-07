using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Logic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceSearch
    {
        #region Data Contracts

        [DataContract]
        public class CswNbtSearchReturn : CswWebSvcReturn
        {
            public CswNbtSearchReturn()
            {
                Data = new SearchResponse();
            }

            [DataMember]
            public SearchResponse Data;
        }

        [DataContract]
        public class SearchResponse
        {

            [DataMember]
            public Collection<CswNbtNodeTypePropListOption> Options;
        }

        [DataContract]
        public class CswNbtSearchRequest
        {
            [DataMember]
            public string NodeTypePropId { get; set; }

            [DataMember]
            public string SearchTerm { get; set; }
        }

        #endregion

        private readonly CswNbtResources _CswNbtResources;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        private CswNbtViewBuilder _ViewBuilder;

        /// <summary>
        /// Searching against these field types is not yet supported
        /// </summary>
        private ArrayList _ProhibittedFieldTypes
        {
            get
            {
                ArrayList InvalidFieldTypes = new ArrayList();
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Button ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.LogicalSet ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.ViewPickList ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.ViewReference ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.NodeTypeSelect ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.MOL ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.MTBF ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Grid ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Password ) );
                return InvalidFieldTypes;
            }
        }

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _ViewBuilder = new CswNbtViewBuilder( _CswNbtResources, _ProhibittedFieldTypes );
        }//ctor

        #region UniversalSearch

        public JObject doUniversalSearch( string SearchTerm, CswEnumSqlLikeMode SearchType, Int32 NodeTypeId, Int32 ObjectClassId, Int32 PropertySetId, Int32 Page, Int32 PageLimit, bool OnlyMergeableNodeTypes, List<string> ExcludeNodeIds, bool IncludeInRecent )
        {
            CswNbtSearch Search = getSearch( SearchTerm, SearchType, NodeTypeId, ObjectClassId, PropertySetId );
            Search.OnlyMergeableNodeTypes = OnlyMergeableNodeTypes;
            Search.ExcludeNodeIds = ExcludeNodeIds;
            return _finishUniversalSearch( Search, Page, PageLimit, IncludeInRecent );
        }

        public CswNbtSearch getSearch( string SearchTerm, CswEnumSqlLikeMode SearchType, Int32 NodeTypeId, Int32 ObjectClassId, Int32 PropertySetId )
        {
            CswNbtSearch Search = new CswNbtSearch( _CswNbtResources )
            {
                SearchTerm = SearchTerm,
                SearchType = SearchType
            };
            if( Int32.MinValue != NodeTypeId )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( null != NodeType )
                {
                    Search.addFilter( NodeType, false );
                }
            }
            if( Int32.MinValue != ObjectClassId )
            {
                CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
                if( null != ObjectClass )
                {
                    Search.addFilter( ObjectClass, false );
                }
            }
            if( Int32.MinValue != PropertySetId )
            {
                CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( PropertySetId );
                if( null != PropertySet )
                {
                    Search.addFilter( PropertySet, false );
                }
            }
            return Search;
        }

        public JObject restoreUniversalSearch( CswPrimaryKey SearchId, int Limit )
        {
            CswNbtSearch Search = _CswNbtResources.SearchManager.restoreSearch( SearchId );
            return _finishUniversalSearch( Search, PageLimit: Limit );
        } // restoreUniversalSearch()

        public JObject restoreUniversalSearch( CswNbtSessionDataId SessionDataId, int Limit )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( null != SessionDataItem && SessionDataItem.DataType == CswEnumNbtSessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                ret = _finishUniversalSearch( Search, PageLimit: Limit );
            }
            return ret;
        } // restoreUniversalSearch()

        public JObject filterUniversalSearch( CswNbtSessionDataId SessionDataId, JObject Filter, string Action, int Limit )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswEnumNbtSessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                if( Action == "add" )
                {
                    Search.addFilter( Filter );
                }
                else
                {
                    Search.removeFilter( Filter );
                }
                ret = _finishUniversalSearch( Search, PageLimit: Limit );
            }
            return ret;
        }

        public JObject filterUniversalSearchByNodeType( CswNbtSessionDataId SessionDataId, Int32 NodeTypeId, int Limit )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswEnumNbtSessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                Search.addFilter( NodeTypeId, true );
                ret = _finishUniversalSearch( Search, PageLimit: Limit );
            }
            return ret;
        }
        private JObject _finishUniversalSearch( CswNbtSearch Search, Int32 Page = 0, Int32 PageLimit = 0, bool IncludeInRecent = true )
        {
            ICswNbtTree Tree = Search.Results();
            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, _CswNbtStatisticsEvents, Int32.MinValue );

            Search.SaveToCache( IncludeInRecent );

            JObject ret = Search.ToJObject();
            ret["table"] = wsTable.makeTableFromTree( Tree, Search.getFilteredPropIds(), Page, PageLimit );
            ret["filters"] = Search.FilterOptions( Tree );
            ret["searchtarget"] = "universal";
            ret["searchtype"] = Search.SearchType.ToString();
            ret["alternateoption"] = _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3 );

            return ret;
        }

        public JObject saveSearch( CswNbtSessionDataId SessionDataId, string Name, string Category, int Limit )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswEnumNbtSessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                Search.Name = Name;
                Search.Category = Category;
                Search.SaveToDb();
                ret = _finishUniversalSearch( Search, PageLimit: Limit );
            }
            return ret;
        } // saveSearch

        public JObject deleteSearch( CswPrimaryKey SearchId, int Limit )
        {
            CswNbtSearch doomedSearch = _CswNbtResources.SearchManager.restoreSearch( SearchId );
            if( null != doomedSearch )
            {
                doomedSearch.delete();
            }
            return _finishUniversalSearch( doomedSearch, PageLimit: Limit );
        } // deleteSearch



        #endregion UniversalSearch

        #region ListOptions Search

        public static void doListOptionsSearch( ICswResources CswResources, CswNbtSearchReturn Return, CswNbtSearchRequest Request )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswPropIdAttr PropIdAttr = new CswPropIdAttr( Request.NodeTypePropId );
            Int32 NodeTypePropId = CswConvert.ToInt32( PropIdAttr.NodeTypePropId );
            if( NodeTypePropId != Int32.MinValue )
            {
                CswNbtMetaDataNodeTypeProp ThisNTP = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                CswNbtNode Node = _CswNbtResources.Nodes.GetNode( PropIdAttr.NodeId );
                if( null != ThisNTP && null != Node )
                {
                    // Note: We are assuming that this property is a list!
                    // Todo: Handle the above condition
                    Node.Properties[ThisNTP].AsList.filterOptions( Request.SearchTerm );

                    Return.Data.Options = Node.Properties[ThisNTP].AsList.Options.Options;
                }
            }//if( NodeTypePropId != Int32.MinValue )
        }//doListOptionsSearch()

        #endregion

    } // class CswNbtWebServiceSearch


} // namespace ChemSW.Nbt.WebServices

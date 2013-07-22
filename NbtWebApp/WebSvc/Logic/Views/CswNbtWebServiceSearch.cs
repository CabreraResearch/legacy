using System;
using System.Collections;
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

        public JObject doUniversalSearch( string SearchTerm, Int32 NodeTypeId, Int32 ObjectClassId )
        {
            CswNbtSearch Search = getSearch( SearchTerm, NodeTypeId, ObjectClassId );
            return _finishUniversalSearch( Search );
        }

        public CswNbtSearch getSearch( string SearchTerm, Int32 NodeTypeId, Int32 ObjectClassId )
        {
            CswNbtSearch Search = new CswNbtSearch( _CswNbtResources )
            {
                SearchTerm = SearchTerm
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
            return Search;
        }

        public JObject restoreUniversalSearch( CswPrimaryKey SearchId )
        {
            CswNbtSearch Search = _CswNbtResources.SearchManager.restoreSearch( SearchId );
            return _finishUniversalSearch( Search );
        } // restoreUniversalSearch()

        public JObject restoreUniversalSearch( CswNbtSessionDataId SessionDataId )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( null != SessionDataItem && SessionDataItem.DataType == CswEnumNbtSessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        } // restoreUniversalSearch()

        public JObject filterUniversalSearch( CswNbtSessionDataId SessionDataId, JObject Filter, string Action )
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
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        }

        public JObject filterUniversalSearchByNodeType( CswNbtSessionDataId SessionDataId, Int32 NodeTypeId )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswEnumNbtSessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                Search.addFilter( NodeTypeId, true );
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        }
        private JObject _finishUniversalSearch( CswNbtSearch Search )
        {
            ICswNbtTree Tree = Search.Results();
            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, _CswNbtStatisticsEvents, Int32.MinValue );

            Search.SaveToCache( true );

            JObject ret = Search.ToJObject();
            ret["table"] = wsTable.makeTableFromTree( Tree, Search.getFilteredPropIds() );
            ret["filters"] = Search.FilterOptions( Tree );
            ret["searchtype"] = "universal";
            ret["alternateoption"] = _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3 );

            return ret;
        }

        public JObject saveSearch( CswNbtSessionDataId SessionDataId, string Name, string Category )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswEnumNbtSessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                Search.Name = Name;
                Search.Category = Category;
                Search.SaveToDb();
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        } // saveSearch

        public JObject deleteSearch( CswPrimaryKey SearchId )
        {
            CswNbtSearch doomedSearch = _CswNbtResources.SearchManager.restoreSearch( SearchId );
            if( null != doomedSearch )
            {
                doomedSearch.delete();
            }
            return _finishUniversalSearch( doomedSearch );
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

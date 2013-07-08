using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeTypeTab : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionNodeTypeTab( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "nodetypetabsetid",
                                                          "tabname",
                                                          _CswNbtMetaDataResources.NodeTypeTabTableSelect,
                                                          _CswNbtMetaDataResources.NodeTypeTabTableUpdate,
                                                          makeNodeTypeTab,
                                                          _makeModuleWhereClause );
        }

        public void AddToCache( CswNbtMetaDataNodeTypeTab NewObj )
        {
            _CollImpl.AddToCache( NewObj );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataNodeTypeTab makeNodeTypeTab( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataNodeTypeTab( Resources, Row );
        }

        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeTabId, bool BypassModuleCheck = false )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getByPk( NodeTypeTabId, BypassModuleCheck );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeId, Int32 NodeTypeTabId )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getByPk( NodeTypeTabId );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeId, string NodeTypeTabName )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and lower(tabname) = '" + CswTools.SafeSqlParam( NodeTypeTabName.ToLower() ) + "'" );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTabVersion( Int32 NodeTypeId, Int32 NodeTypeTabId )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and firsttabversionid = (select firsttabversionid from nodetype_tabset where nodetypetabsetid = " + NodeTypeTabId.ToString() + ")" );
        }
        public Collection<Int32> getNodeTypeTabIds( Int32 NodeTypeId )
        {
            return _CollImpl.getPks( "where nodetypeid = " + NodeTypeId.ToString() );
        }
        public IEnumerable<CswNbtMetaDataNodeTypeTab> getNodeTypeTabs( Int32 NodeTypeId )
        {
            return _CollImpl.getWhere( "where nodetypeid = " + NodeTypeId.ToString() ).Cast<CswNbtMetaDataNodeTypeTab>();
        }

        private string _makeModuleWhereClause()
        {
            return " nodetype_tabset.nodetypeid in (select nodetypeid from nodetypes where enabled = '1') ";
        }
    } // class CswNbtMetaDataCollectionNodeTypeTab
} // namespace ChemSW.Nbt.MetaData
//using System;
//using System.Collections.Generic;
//using System.Data;

//namespace ChemSW.Nbt.MetaData
//{

//    public class CswNbtMetaDataTableCache
//    {


//        public enum MetaDataTable
//        {
//            ObjectClass,
//            ObjectClassProp,
//            NodeType,
//            FieldType,
//            NodeTypeProp,
//            NodeTypeTab,
//            JctNodesProps
//        };

//        private bool CacheEnabled = false;


//        private Dictionary<MetaDataTable, DataTable> _Tables = new Dictionary<MetaDataTable, DataTable>();

//        private ICswSuperCycleCache _CswSuperCycleCache = null;
//        private CswNbtResources _CswNbtResources = null;
//        public CswNbtMetaDataTableCache( CswNbtResources CswNbtResources )
//        {
//            _CswNbtResources = CswNbtResources;
//            _CswSuperCycleCache = CswNbtResources.CswSuperCycleCache;
            
//            if( _CswNbtResources.SetupVbls.doesSettingExist( "cachemetadata" ) && "1" == _CswNbtResources.SetupVbls["cachemetadata"] )
//            {
//                CacheEnabled = true;
//            }
//        }//ctor


//        public void makeCacheStale()
//        {
//            //force tables to get nulled and hence reloaded one at a time,
//            //reducing the amount of write locking
//            _CswSuperCycleCache.CacheDirtyThreshold = DateTime.Now;
//        }//clear() 

//        public DataTable get( MetaDataTable TableId )
//        {
//            DataTable ret = null;
//            if( CacheEnabled )
//            {
//                ret = ( (DataTable) _CswSuperCycleCache.get( TableId.ToString() ) );
//            }
//            return ret;
//        }//getTable() 


//        public void put( MetaDataTable TableId, DataTable DataTable )
//        {
//            if( CacheEnabled )
//            {
//                _CswSuperCycleCache.put( TableId.ToString(), DataTable );
//            }
//        }//put() 


//    }//CswNbtMetaDataTableCache

//}//namespace ChemSW.Nbt.MetaData

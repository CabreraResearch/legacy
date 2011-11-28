using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{

    public class CswNbtMetaDataTableCache
    {


        public enum MetaDataTable
        {
            ObjectClass,
            ObjectClassProp,
            NodeType,
            FieldType,
            NodeTypeProp,
            NodeTypeTab,
            JctNodesProps
        };


        private Dictionary<MetaDataTable, DataTable> _Tables = new Dictionary<MetaDataTable, DataTable>();

        private ICswSuperCycleCache _CswSuperCycleCache = null;
        public CswNbtMetaDataTableCache( ICswSuperCycleCache CswSuperCycleCache )
        {
            _CswSuperCycleCache = CswSuperCycleCache;
        }//ctor


        public void clear()
        {
            _CswSuperCycleCache.delete( Enum.GetNames( typeof( MetaDataTable ) ) );
        }//clear() 

        public DataTable get( MetaDataTable TableId )
        {
            return ( (DataTable) _CswSuperCycleCache.get( TableId.ToString() ) );
        }//getTable() 


        public void put( MetaDataTable TableId, DataTable DataTable )
        {
            _CswSuperCycleCache.put( TableId.ToString(), DataTable );
        }//put() 


    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

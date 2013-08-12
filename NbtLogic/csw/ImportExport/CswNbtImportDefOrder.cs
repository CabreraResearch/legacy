using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDefOrder
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly DataRow _row;

        public CswNbtImportDefOrder( CswNbtResources CswNbtResources, DataRow OrderRow )
        {
            _CswNbtResources = CswNbtResources;
            _row = OrderRow;
        }

        public Int32 ImportOrderId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.importdeforderid] ); }
        }
        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.importdefid] ); }
        }
        public Int32 Order
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.importorder] ); }
        }
        public string NodeTypeName
        {
            get { return _row[CswNbtImportTables.ImportDefOrder.nodetypename].ToString(); }
        }
        public Int32 Instance
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.instance] ); }
        }

        public CswNbtMetaDataNodeType NodeType
        {
            get { return _CswNbtResources.MetaData.getNodeType( NodeTypeName ); }
        }

        public string PkColName
        {
            get
            {
                string ret = CswNbtImportDefBinding.SafeColName( NodeType.NodeTypeName );
                if( Int32.MinValue != Instance )
                {
                    ret += "_" + Instance;
                }
                ret += "_nodeid";
                return ret;
            }
        }
    }
}

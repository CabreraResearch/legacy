using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DOrder
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly DataRow _row;

        public CswNbt2DOrder( CswNbtResources CswNbtResources, DataRow OrderRow )
        {
            _CswNbtResources = CswNbtResources;
            _row = OrderRow;
        }

        public Int32 ImportOrderId
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportDefOrder.importorderid] ); }
        }
        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportDefOrder.importdefinitionid] ); }
        }
        public Int32 Order
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportDefOrder.importorder] ); }
        }
        public string NodeTypeName
        {
            get { return _row[CswNbt2DImportTables.ImportDefOrder.nodetypename].ToString(); }
        }
        public Int32 Instance
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportDefOrder.instance] ); }
        }

        public CswNbtMetaDataNodeType NodeType
        {
            get { return _CswNbtResources.MetaData.getNodeType( NodeTypeName ); }
        }

        public string PkColName
        {
            get
            {
                string ret = CswNbt2DBinding.SafeColName( NodeType.NodeTypeName );
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

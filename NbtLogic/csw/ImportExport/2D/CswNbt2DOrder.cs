using System;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DOrder
    {
        public Int32 Order;
        public CswNbtMetaDataNodeType NodeType;
        public string Instance;

        public string PkColName
        {
            get
            {
                string ret = CswNbt2DBinding.SafeColName( NodeType.NodeTypeName );
                if( false == string.IsNullOrEmpty( Instance ) )
                {
                    ret += "_" + Instance;
                }
                ret += "_nodeid";
                return ret;
            }
        }
    }
}

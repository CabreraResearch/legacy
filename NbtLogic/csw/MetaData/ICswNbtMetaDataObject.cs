using System;
using System.Data;

namespace ChemSW.Nbt.MetaData
{
    public interface ICswNbtMetaDataObject
    {
        DataRow _DataRow { get; }
        Int32 UniqueId { get; }
        string UniqueIdFieldName { get; }
        //void Reassign( DataRow NewRow );
    }
}

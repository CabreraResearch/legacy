using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ChemSW.Nbt.MetaData
{
    public interface ICswNbtMetaDataObject
    {
        DataRow _DataRow { get; }
        Int32 UniqueId { get; }
        string UniqueIdFieldName { get; }
        void Reassign( DataRow NewRow );
    }
}

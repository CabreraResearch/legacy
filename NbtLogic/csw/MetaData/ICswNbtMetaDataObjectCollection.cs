using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace ChemSW.Nbt.MetaData
{
    public interface ICswNbtMetaDataObjectCollection
    {
        void ClearKeys();
        ICswNbtMetaDataObject RegisterNew( DataRow Row );
        ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride );
        void RegisterExisting( ICswNbtMetaDataObject Object );
        Collection<ICswNbtMetaDataObject> All { get; }
        void Deregister( ICswNbtMetaDataObject Object );
        void Remove( ICswNbtMetaDataObject Object );
    }
}

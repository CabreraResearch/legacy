using System;
using System.Data;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Represents a NodeType, Object Class, or Property Set
    /// </summary>
    public interface ICswNbtMetaDataDefinitionObject : ICswNbtMetaDataObject
    {
        CswNbtView CreateDefaultView( bool includeDefaultFilters = true );
        ICswNbtMetaDataProp getBarcodeProperty();

        bool HasLabel { get; }
    }


}

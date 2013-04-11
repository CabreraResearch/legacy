using System;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Mode of operation for select-box driven properties
    /// </summary>
    public enum CswEnumNbtPropertySelectMode
    {
        /// <summary>
        /// Only allow selecting a single item
        /// </summary>
        Single,
        /// <summary>
        /// Allow selecting multiple items
        /// </summary>
        Multiple,
        /// <summary>
        /// Not Applicable
        /// </summary>
        Blank
    };

}

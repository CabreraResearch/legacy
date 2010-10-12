using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Mode of operation for select-box driven properties
    /// </summary>
    public enum PropertySelectMode
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

    public interface ICswNbtMetaDataProp
    {
        Int32 PropId { get; }
        Int32 ObjectClassPropId { get; }
        string PropName { get; }
        string PropNameWithQuestionNo { get; }
        bool IsRequired { get; }
        CswNbtMetaDataFieldType FieldType { get; }
        string FKType { get; }
        Int32 FKValue { get; }
        string ListOptions { get; }
        PropertySelectMode Multi { get; }
        ICswNbtFieldTypeRule FieldTypeRule { get; }
        bool IsUserRelationship();
        bool IsUnique { get; }
        bool IsGlobalUnique { get; }
        string StaticText { get; }
        Int32 NumberPrecision { get; }
        double MinValue { get; }
        double MaxValue { get; }
    }
}

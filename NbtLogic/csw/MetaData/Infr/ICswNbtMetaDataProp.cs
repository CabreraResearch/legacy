using System;
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
        Int32 FirstPropVersionId { get; }
        Int32 ObjectClassPropId { get; }
        string PropName { get; }
        string PropNameWithQuestionNo { get; }
        bool IsRequired { get; }
        CswNbtMetaDataFieldType getFieldType();
        CswNbtMetaDataFieldType.NbtFieldType getFieldTypeValue();
        string FKType { get; }
        Int32 FKValue { get; }
        string ValuePropType { get; }
        Int32 ValuePropId { get; }
        string ListOptions { get; }
        PropertySelectMode Multi { get; }
        ICswNbtFieldTypeRule getFieldTypeRule();
        bool IsUserRelationship();
        bool IsUnique();
        bool IsGlobalUnique();
        string StaticText { get; }
        Int32 NumberPrecision { get; }
        double MinValue { get; }
        double MaxValue { get; }
    }
}

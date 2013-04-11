using System;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.MetaData
{
    public interface ICswNbtMetaDataProp
    {
        Int32 PropId { get; }
        Int32 FirstPropVersionId { get; }
        Int32 ObjectClassPropId { get; }
        string PropName { get; }
        string PropNameWithQuestionNo { get; }
        bool IsRequired { get; }
        CswNbtMetaDataFieldType getFieldType();
        CswEnumNbtFieldType getFieldTypeValue();
        string FKType { get; }
        Int32 FKValue { get; }
        string ValuePropType { get; }
        Int32 ValuePropId { get; }
        string ListOptions { get; }
        CswEnumNbtPropertySelectMode Multi { get; }
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

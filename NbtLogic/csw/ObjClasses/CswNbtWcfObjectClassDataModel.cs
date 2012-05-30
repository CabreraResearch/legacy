using System;
using System.Runtime.Serialization;
using ChemSW.Nbt.MetaData;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWcfObjectClassDataModel
    {
        //public class ObjectClass
        //{

        //}

        [DataContract]
        public class ObjectClassProp
        {
            /*The problem with default getters and setters (why we're not using them here) is two-fold: 
             * 1. they're immutable 
             * 2. they default to null for nullable types and 0 for numbers.
             * Creating our own default values using pairs of private/public properties is overkill for this purpose.
             */
            [DataMember( IsRequired = true )]
            public string PropName = string.Empty;
            [DataMember( IsRequired = true )]
            public CswNbtMetaDataFieldType.NbtFieldType FieldType = CswNbtMetaDataFieldType.NbtFieldType.Unknown;

            [DataMember]
            public bool AuditLevel;
            [DataMember]
            public bool IsBatchEntry;
            [DataMember]
            public bool IsRequired;
            [DataMember]
            public bool IsUnique;
            [DataMember]
            public bool IsCompoundUnique;
            [DataMember]
            public bool ServerManaged;
            [DataMember]
            public Int32 ValueFieldId = Int32.MinValue;
            [DataMember]
            public Int32 NumberPrecision = Int32.MinValue;
            [DataMember]
            public string ListOptions = string.Empty;
            [DataMember]
            public string ViewXml = string.Empty;
            [DataMember]
            public bool IsFk;
            [DataMember]
            public string FkType = string.Empty;
            [DataMember]
            public Int32 FkValue = Int32.MinValue;
            [DataMember]
            public bool Multi;
            [DataMember]
            public bool ReadOnly;
            [DataMember]
            public Int32 DisplayColAdd = Int32.MinValue;
            [DataMember]
            public Int32 DisplayRowAdd = Int32.MinValue;
            [DataMember]
            public bool SetValOnAdd;
            [DataMember]
            public Int32 NumberMinValue = Int32.MinValue;
            [DataMember]
            public Int32 NumberMaxValue = Int32.MinValue;
            [DataMember]
            public string StaticText = string.Empty;
            [DataMember]
            public string Filter = string.Empty;
            [DataMember]
            public Int32 FilterPropId = Int32.MinValue;
            [DataMember]
            public bool UseNumbering;
            [DataMember]
            public string ValueOptions = string.Empty;
            [DataMember]
            public bool IsGlobalUnique;
            [DataMember]
            public string Extended = string.Empty;
        }

    }
}
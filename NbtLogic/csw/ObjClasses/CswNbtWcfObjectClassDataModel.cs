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
            public ObjectClassProp()
            {

            }
            [DataMember]
            public bool AuditLevel { get; set; }
            [DataMember]
            public CswNbtMetaDataFieldType.NbtFieldType FieldType { get; set; }
            [DataMember]
            public bool IsBatchEntry { get; set; }
            [DataMember]
            public bool IsRequired { get; set; }
            [DataMember]
            public bool IsUnique { get; set; }
            [DataMember]
            public bool IsCompoundUnique { get; set; }
            [DataMember]
            public bool ServerManaged { get; set; }
            [DataMember]
            public Int32 ValueFieldId { get; set; }
            [DataMember]
            public Int32 NumberPrecision { get; set; }
            [DataMember]
            public string ListOptions { get; set; }
            [DataMember]
            public string ViewXml { get; set; }
            [DataMember]
            public bool IsFk { get; set; }
            [DataMember]
            public string FkType { get; set; }
            [DataMember]
            public Int32 FkValue { get; set; }
            [DataMember]
            public bool Multi { get; set; }
            [DataMember]
            public bool ReadOnly { get; set; }
            [DataMember]
            public Int32 DisplayColAdd { get; set; }
            [DataMember]
            public Int32 DisplayRowAdd { get; set; }
            [DataMember]
            public bool SetValOnAdd { get; set; }
            [DataMember]
            public Int32 NumberMinValue { get; set; }
            [DataMember]
            public Int32 NumberMaxValue { get; set; }
            [DataMember]
            public string StaticText { get; set; }
            [DataMember]
            public string Filter { get; set; }
            [DataMember]
            public Int32 FilterPropId { get; set; }
            [DataMember]
            public bool UseNumbering { get; set; }
            [DataMember]
            public string ValueOptions { get; set; }
            [DataMember]
            public string PropName { get; set; }
            [DataMember]
            public bool IsGlobalUnique { get; set; }
            [DataMember]
            public string Extended { get; set; }
        }

    }
}
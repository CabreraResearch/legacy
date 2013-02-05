using System;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    [DataContract]
    public class CswNbtWcfMetaDataModel
    {
        //public class ObjectClass
        //{

        //}

        [DataContract]
        public class ObjectClassProp
        {
            public ObjectClassProp( CswNbtMetaDataObjectClass NbtObjectClass = null )
            {
                ObjectClass = NbtObjectClass;
            }

            public CswNbtMetaDataObjectClass ObjectClass;

            /*The problem with default getters and setters (why we're not using them here) is two-fold: 
             * 1. they're immutable 
             * 2. they default to null for nullable types and 0 for numbers.
             * Creating our own default values using pairs of private/public properties is overkill for this purpose.
             */
            [DataMember( IsRequired = true )]
            public string PropName = string.Empty;

            private CswNbtMetaDataFieldType.NbtFieldType _FieldType;

            [DataMember( IsRequired = true )]
            public CswNbtMetaDataFieldType.NbtFieldType FieldType
            {
                get { return _FieldType; }
                set
                {
                    _FieldType = value;
                    if ( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Location )
                    {
                        IsFk = true;
                        FkType = NbtViewRelatedIdType.ObjectClassId.ToString();
                        //TODO: We should automatically set FkValue, but we don't have an NbtResources. Not refactoring this today.
                    }
                }
            }

            [DataMember]
            public bool AuditLevel;
            [DataMember]
            public bool IsBatchEntry;

            [DataMember]
            public bool IsRequired
            {
                get { return _IsRequired; }
                set
                {
                    if ( false == string.IsNullOrEmpty( Filter ) && value )
                    {
                        throw new CswDniException( ErrorType.Error, "Conditional properties cannot be required", "Attempted to require a conditional property " + PropName );
                    }
                    _IsRequired = value;
                }
            }

            [DataMember]
            public bool IsUnique;
            [DataMember]
            public bool IsCompoundUnique;
            [DataMember]
            public bool ServerManaged;
            [DataMember]
            public Int32 ValueFieldId = Int32.MinValue;
            [DataMember]
            public Int32 ValuePropId = Int32.MinValue;
            [DataMember]
            public string ValuePropType = string.Empty;
            [DataMember]
            public Int32 NumberPrecision = Int32.MinValue;
            [DataMember]
            public string ListOptions = string.Empty;
            [DataMember]
            public string ViewXml = string.Empty;
            //TODO: We should really validate FkValues inside this class, especially in conjunction with setting the FieldType
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
            public string Filter
            {
                get { return _filter; }
                set
                {
                    if ( false == string.IsNullOrEmpty( value ) && IsRequired )
                    {
                        throw new CswDniException( ErrorType.Error, "Required properties cannot be conditional", "Attempted to assign a filter to required property " + PropName );
                    }
                    _filter = value;
                }
            }

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

            private string _filter;
            private bool _IsRequired;
        }

        [DataContract]
        public class NodeType
        {

            public NodeType( CswNbtMetaDataObjectClass NbtObjectClass )
            {
                if ( null == NbtObjectClass )
                {
                    throw new CswDniException( ErrorType.Error, "Cannot create a NodeType DataContract without a valid Object Class", "Attempted to instance CswNbtWcfMetaDataModel.NodeType with a null ObjectClass." );
                }
                ObjectClass = NbtObjectClass;
                ObjectClassId = ObjectClass.ObjectClassId;
                IconFileName = ObjectClass.IconFileName;
                SearchDeferObjectClassPropId = ObjectClass.SearchDeferPropId;
            }

            [DataMember( IsRequired = true )]
            public string NodeTypeName = string.Empty;
            [DataMember( IsRequired = true )]
            public CswNbtMetaDataObjectClass ObjectClass;
            [DataMember]
            public Int32 ObjectClassId;
            [DataMember]
            public string IconFileName;
            [DataMember]
            public string Category = string.Empty;
            [DataMember]
            public string NameTemplate = string.Empty;
            [DataMember]
            public Int32 SearchDeferObjectClassPropId = Int32.MinValue;
            [DataMember]
            public Int32 SearchDeferNodeTypePropId = Int32.MinValue;
        }

        [DataContract]
        public class NodeTypeProp
        {
            private void _init( CswNbtMetaDataNodeType NbtNodeType, CswNbtMetaDataFieldType NbtFieldType, string NbtPropName )
            {
                if ( null == NbtNodeType )
                {
                    throw new CswDniException( ErrorType.Error, "Cannot create a NodeTypeProp DataContract without a valid Node Type", "Attempted to instance CswNbtWcfMetaDataModel.NodeTypeProp with a null NodeType." );
                }
                if ( null == NbtFieldType )
                {
                    throw new CswDniException( ErrorType.Error, "Cannot create a NodeTypeProp DataContract without a valid Field Type", "Attempted to instance CswNbtWcfMetaDataModel.NodeTypeProp with a null FieldType." );
                }
                if ( string.IsNullOrEmpty( NbtPropName ) )
                {
                    throw new CswDniException( ErrorType.Warning, "Property Name is required", "Attempted to create a new nodetype prop with a null propname" );
                }
                NodeType = NbtNodeType;
                NodeTypeId = NodeType.NodeTypeId;
                FieldType = NbtFieldType;
                PropName = NbtPropName;
                UseNumbering = ( NodeType.getObjectClass().ObjectClass == NbtObjectClass.InspectionDesignClass &&
                                FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question );
                if ( FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect )
                {
                    Multi = Tristate.False;
                }
                else
                {
                    Multi = Tristate.Null;
                }
                ReadOnly = ( FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode ||
                            FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence );
                IsUnique = ( FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode ||
                            FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence );
            }
            public NodeTypeProp( CswNbtMetaDataNodeType NbtNodeType, CswNbtMetaDataFieldType NbtFieldType, string NbtPropName )
            {
                _init( NbtNodeType, NbtFieldType, NbtPropName );
            }

            [DataMember( IsRequired = true )]
            public string PropName = string.Empty;
            [DataMember( IsRequired = true )]
            public CswNbtMetaDataNodeType NodeType;
            [DataMember( IsRequired = true )]
            public CswNbtMetaDataFieldType FieldType;
            [DataMember]
            public Int32 NodeTypeId;
            [DataMember]
            public CswNbtMetaDataNodeTypeProp InsertAfterProp = null;
            [DataMember]
            public Int32 TabId = Int32.MinValue;
            [DataMember]
            public bool PreventVersioning;
            [DataMember]
            public CswNbtMetaDataObjectClassProp ObjectClassPropToCopy = null;
            [DataMember]
            public bool UseNumbering;
            [DataMember]
            public Tristate Multi;
            [DataMember]
            public bool ReadOnly;
            [DataMember]
            public bool IsRequired;
            [DataMember]
            public bool IsUnique;
            [DataMember]
            public bool IsCompoundUnique;
            //[DataMember]
            //public bool ServerManaged;
            //[DataMember]
            //public Int32 ValueFieldId = Int32.MinValue;
            //[DataMember]
            //public Int32 NumberPrecision = Int32.MinValue;
            //[DataMember]
            //public string ListOptions = string.Empty;
            //[DataMember]
            //public string ViewXml = string.Empty;
            //[DataMember]
            //public bool IsFk;
            //[DataMember]
            //public string FkType = string.Empty;
            //[DataMember]
            //public Int32 FkValue = Int32.MinValue;
            //[DataMember]
            //public Int32 DisplayColAdd = Int32.MinValue;
            //[DataMember]
            //public Int32 DisplayRowAdd = Int32.MinValue;
            //[DataMember]
            //public bool SetValOnAdd;
            //[DataMember]
            //public Int32 NumberMinValue = Int32.MinValue;
            //[DataMember]
            //public Int32 NumberMaxValue = Int32.MinValue;
            //[DataMember]
            //public string StaticText = string.Empty;
            //[DataMember]
            //public string Filter = string.Empty;
            //[DataMember]
            //public Int32 FilterPropId = Int32.MinValue;
            //[DataMember]
            //public string ValueOptions = string.Empty;
            //[DataMember]
            //public bool IsGlobalUnique;
            //[DataMember]
            //public string Extended = string.Empty;
            //[DataMember]
            //public Int32 QuestionNo = Int32.MinValue;
        }

    }
}
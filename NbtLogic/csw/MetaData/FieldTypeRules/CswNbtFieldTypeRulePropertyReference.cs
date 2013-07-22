using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRulePropertyReference : ICswNbtFieldTypeRule
    {

        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Value = CswEnumNbtSubFieldName.Value;
            public static CswEnumNbtSubFieldName Sequence = CswEnumNbtSubFieldName.Sequence;
            public static CswEnumNbtSubFieldName Number = CswEnumNbtSubFieldName.Number;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public static CswEnumNbtPropColumn SequenceNumberColumn = CswEnumNbtPropColumn.Field1_Numeric;

        public CswNbtSubField CachedValueSubField;
        public CswNbtSubField SequenceSubField;
        public CswNbtSubField SequenceNumberSubField;

        public CswNbtFieldTypeRulePropertyReference( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CachedValueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Value );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            CachedValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( CachedValueSubField );

            SequenceSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, SubFieldName.Sequence );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( SequenceSubField );

            SequenceNumberSubField = new CswNbtSubField( _CswNbtFieldResources, SequenceNumberColumn, SubFieldName.Number );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( SequenceNumberSubField );

        }//ctor        

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            //return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn ) );

            CswEnumNbtSubFieldName OldSubfieldName = CswNbtViewPropertyFilterIn.SubfieldName;
            CswEnumNbtFilterMode OldFilterMode = CswNbtViewPropertyFilterIn.FilterMode;
            string OldValue = CswNbtViewPropertyFilterIn.Value;

            // BZ 8558
            if( OldSubfieldName == SubFieldName.Value && OldValue.ToLower() == "me" )
            {
                CswNbtViewProperty Prop = (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent;

                ICswNbtMetaDataProp MetaDataProp = null;
                if( Prop.Type == CswEnumNbtViewPropType.NodeTypePropId )
                    MetaDataProp = Prop.NodeTypeProp;
                else if( Prop.Type == CswEnumNbtViewPropType.ObjectClassPropId )
                    MetaDataProp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( Prop.ObjectClassPropId );

                // Could be a propref of a propref, so we can't look at the relationship

                //ICswNbtMetaDataProp RelationshipProp = null;
                //if( MetaDataProp.FKType == NbtViewPropType.NodeTypePropId.ToString() )
                //    RelationshipProp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( MetaDataProp.FKValue);
                //else if( MetaDataProp.FKType == NbtViewPropType.ObjectClassPropId.ToString() )
                //    RelationshipProp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( MetaDataProp.FKValue );

                //if( RelationshipProp != null && RelationshipProp.IsUserRelationship() )
                //{
                if( CswNbtViewPropertyFilterIn.Value.ToLower() == "me" )
                {
                    CswNbtViewPropertyFilterIn.SubfieldName = SubFieldName.Value;
                    CswNbtViewPropertyFilterIn.FilterMode = CswEnumNbtFilterMode.Equals;
                    CswNbtViewPropertyFilterIn.Value = _CswNbtFieldResources.CswNbtResources.Nodes[RunAsUser.UserId].NodeName;
                }
                //}
            }
            string ret = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, false );

            CswNbtViewPropertyFilterIn.SubfieldName = OldSubfieldName;
            CswNbtViewPropertyFilterIn.FilterMode = OldFilterMode;
            CswNbtViewPropertyFilterIn.Value = OldValue;

            return ret;
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        /// <summary>
        /// Returns true if the relationship is invalid
        /// </summary>
        private bool _isInvalidRelationship( CswEnumNbtViewRelatedIdType RelatedIdType, Int32 FkValue, CswEnumNbtViewPropIdType inValuePropType, Int32 inValuePropId )
        {
            bool RetIsInvalid = false;

            //if( RelatedIdType == NbtViewRelatedIdType.NodeTypeId )
            //{
            //    CswNbtMetaDataNodeType FkRelationshipTargetNt = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeType( FkValue );
            //    RetIsInvalid = ( null == FkRelationshipTargetNt );
            //    if( false == RetIsInvalid )
            //    {
            //        CswNbtMetaDataNodeTypeProp ValueNtp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inValuePropId );
            //        RetIsInvalid = ( null == ValueNtp ||
            //                         _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeFirstVersion( ValueNtp.NodeTypeId ) != FkRelationshipTargetNt.getFirstVersionNodeType() );
            //    }
            //}
            //else if( RelatedIdType == NbtViewRelatedIdType.ObjectClassId )
            //{
            //    CswNbtMetaDataObjectClass FkRelationshipTargetOc = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( FkValue );
            //    RetIsInvalid = ( null == FkRelationshipTargetOc );

            //    if( false == RetIsInvalid )
            //    {
            //        CswNbtMetaDataObjectClassProp ValueOcp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inValuePropId );
            //        RetIsInvalid = ( null == ValueOcp || ( ValueOcp.getObjectClass().ObjectClass != FkRelationshipTargetOc.ObjectClass ) );
            //    }
            //}
            //else if( RelatedIdType == NbtViewRelatedIdType.PropertySetId )
            //{
            //    CswNbtMetaDataPropertySet FkRelationshipTargetPs = _CswNbtFieldResources.CswNbtResources.MetaData.getPropertySet( FkValue );
            //    RetIsInvalid = ( null == FkRelationshipTargetPs );

            //    if( false == RetIsInvalid )
            //    {
            //        CswNbtMetaDataObjectClassProp ValueOcp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inValuePropId );
            //        RetIsInvalid = ( null == ValueOcp ||
            //                         null == ValueOcp.getObjectClass().getPropertySet() ||
            //                         ValueOcp.getObjectClass().getPropertySet().Name != FkRelationshipTargetPs.Name );
            //    }
            //}

            if( inValuePropType == CswEnumNbtViewPropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp ValuePropNTP = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inValuePropId );
                RetIsInvalid = false == ( CswNbtViewRelationship.Matches( _CswNbtFieldResources.CswNbtResources, RelatedIdType, FkValue, ValuePropNTP.getNodeType() ) );
            }
            else if( inValuePropType == CswEnumNbtViewPropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp ValuePropOCP = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inValuePropId );
                RetIsInvalid = false == ( CswNbtViewRelationship.Matches( _CswNbtFieldResources.CswNbtResources, RelatedIdType, FkValue, ValuePropOCP.getObjectClass() ) );
            }

            return RetIsInvalid;
        }

        private bool _isInvalidFkTarget( CswEnumNbtViewPropIdType NewFkPropIdType, Int32 inFKValue, CswEnumNbtViewPropIdType inValuePropType, Int32 inValuePropId )
        {
            bool RetClearPropVal = false;
            if( NewFkPropIdType == CswEnumNbtViewPropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp FkNtp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inFKValue );
                RetClearPropVal = ( null == FkNtp );
                if( false == RetClearPropVal )
                {
                    RetClearPropVal = _isInvalidRelationship( FkNtp.FKType, FkNtp.FKValue, inValuePropType, inValuePropId );
                }
            }
            else if( NewFkPropIdType == CswEnumNbtViewPropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp FkOcp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inFKValue );
                RetClearPropVal = ( null == FkOcp );
                if( false == RetClearPropVal )
                {
                    RetClearPropVal = _isInvalidRelationship( FkOcp.FKType, FkOcp.FKValue, inValuePropType, inValuePropId );
                }
            }
            return RetClearPropVal;
        }

        public void onSetFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            Collection<CswNbtFieldTypeAttribute> Attributes = getAttributes();

            CswNbtFieldTypeAttribute FkTypeAttr = Attributes.FirstOrDefault( a => a.Column == CswEnumNbtPropertyAttributeColumn.Fktype );
            CswNbtFieldTypeAttribute FkValueAttr = Attributes.FirstOrDefault( a => a.Column == CswEnumNbtPropertyAttributeColumn.Fkvalue );
            CswNbtFieldTypeAttribute ValuePropTypeAttr = Attributes.FirstOrDefault( a => a.Column == CswEnumNbtPropertyAttributeColumn.Valueproptype );
            CswNbtFieldTypeAttribute ValuePropIdAttr = Attributes.FirstOrDefault( a => a.Column == CswEnumNbtPropertyAttributeColumn.Valuepropid );
            
            if( DesignNTPNode.AttributeProperty.ContainsKey( FkTypeAttr.Name ) )
            {
                CswNbtNodePropList FkTypeProp = DesignNTPNode.AttributeProperty[FkTypeAttr.Name].AsList;
                CswNbtNodePropRelationship FkValueProp = DesignNTPNode.AttributeProperty[FkValueAttr.Name].AsRelationship;
                CswNbtNodePropList ValuePropTypeProp = DesignNTPNode.AttributeProperty[ValuePropTypeAttr.Name].AsList;
                CswNbtNodePropRelationship ValuePropIdProp = DesignNTPNode.AttributeProperty[ValuePropIdAttr.Name].AsRelationship;

                if( FkTypeProp.WasModified || FkValueProp.WasModified ||
                    ValuePropTypeProp.WasModified || ValuePropIdProp.WasModified )
                {
                    //We're changing the relationship
                    if( _isInvalidFkTarget( FkTypeProp.Value, FkValueProp.RelatedNodeId.PrimaryKey, ValuePropTypeProp.Value, ValuePropIdProp.RelatedNodeId.PrimaryKey ) )
                    {
                        ValuePropTypeProp.Value = "";
                        ValuePropIdProp.RelatedNodeId = null;
                    }
                }
            }
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string IsFK = CswEnumNbtPropertyAttributeName.IsFK;
            public const string FKType = CswEnumNbtPropertyAttributeName.FKType;
            public const string Relationship = CswEnumNbtPropertyAttributeName.Relationship;
            public const string RelatedProperty = CswEnumNbtPropertyAttributeName.RelatedProperty;
            public const string RelatedPropType = CswEnumNbtPropertyAttributeName.RelatedPropType;
            public const string UseSequence = CswEnumNbtPropertyAttributeName.UseSequence;
            public const string Sequence = CswEnumNbtPropertyAttributeName.Sequence;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.PropertyReference );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.PropertyReference,
                    Name = AttributeName.IsFK,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Isfk
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.PropertyReference,
                    Name = AttributeName.FKType,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Fktype
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.PropertyReference,
                    Name = AttributeName.Relationship,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship,
                    Column = CswEnumNbtPropertyAttributeColumn.Fkvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.PropertyReference,
                    Name = AttributeName.RelatedProperty,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship,
                    Column = CswEnumNbtPropertyAttributeColumn.Valuepropid
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.PropertyReference,
                    Name = AttributeName.RelatedPropType,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Valueproptype
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.PropertyReference,
                    Name = AttributeName.UseSequence,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Attribute1
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.PropertyReference,
                    Name = AttributeName.Sequence,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship,
                    Column = CswEnumNbtPropertyAttributeColumn.Sequenceid,
                    SubFieldName = CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRulePropertyReference : ICswNbtFieldTypeRule
    {

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

            CachedValueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Value );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Begins );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotContains );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Ends );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.GreaterThan );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.LessThan );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            SubFields.add( CachedValueSubField );

            SequenceSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.Sequence );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Begins );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotContains );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Ends );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.GreaterThan );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.LessThan );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            SequenceSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            SubFields.add( SequenceSubField );

            SequenceNumberSubField = new CswNbtSubField( _CswNbtFieldResources, SequenceNumberColumn, CswEnumNbtSubFieldName.Number );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Begins );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotContains );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Ends );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.GreaterThan );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.LessThan );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            SequenceNumberSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
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
            CswNbtPropFilterSql.PropertyFilterMode OldFilterMode = CswNbtViewPropertyFilterIn.FilterMode;
            string OldValue = CswNbtViewPropertyFilterIn.Value;

            // BZ 8558
            if( OldSubfieldName == CachedValueSubField.Name && OldValue.ToLower() == "me" )
            {
                CswNbtViewProperty Prop = (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent;

                ICswNbtMetaDataProp MetaDataProp = null;
                if( Prop.Type == NbtViewPropType.NodeTypePropId )
                    MetaDataProp = Prop.NodeTypeProp;
                else if( Prop.Type == NbtViewPropType.ObjectClassPropId )
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
                    CswNbtViewPropertyFilterIn.SubfieldName = CachedValueSubField.Name;
                    CswNbtViewPropertyFilterIn.FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Equals;
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

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
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
        private bool _isInvalidRelationship( NbtViewRelatedIdType RelatedIdType, Int32 FkValue, NbtViewPropIdType inValuePropType, Int32 inValuePropId )
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

            if( inValuePropType == NbtViewPropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp ValuePropNTP = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inValuePropId );
                RetIsInvalid = false == ( CswNbtViewRelationship.Matches( _CswNbtFieldResources.CswNbtResources, RelatedIdType, FkValue, ValuePropNTP.getNodeType() ) );
            }
            else if( inValuePropType == NbtViewPropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp ValuePropOCP = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inValuePropId );
                RetIsInvalid = false == ( CswNbtViewRelationship.Matches( _CswNbtFieldResources.CswNbtResources, RelatedIdType, FkValue, ValuePropOCP.getObjectClass() ) );
            }

            return RetIsInvalid;
        }

        private bool _isInvalidFkTarget( NbtViewPropIdType NewFkPropIdType, Int32 inFKValue, NbtViewPropIdType inValuePropType, Int32 inValuePropId )
        {
            bool RetClearPropVal = false;
            if( NewFkPropIdType == NbtViewPropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp FkNtp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inFKValue );
                RetClearPropVal = ( null == FkNtp );
                if( false == RetClearPropVal )
                {
                    RetClearPropVal = _isInvalidRelationship( FkNtp.FKType, FkNtp.FKValue, inValuePropType, inValuePropId );
                }
            }
            else if( NewFkPropIdType == NbtViewPropIdType.ObjectClassPropId )
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

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            string OutFkType = inFKType;
            Int32 OutFkValue = inFKValue;
            string OutValuePropType = inValuePropType;
            Int32 OutValuePropId = inValuePropId;

            //New PropIdTypes
            NbtViewPropIdType NewFkPropIdType = (NbtViewPropIdType) inFKType;
            NbtViewPropIdType NewPropTypePropIdType = (NbtViewPropIdType) inValuePropType;

            //Current PropIdTypes
            NbtViewPropIdType CurrentFkPropIdType = (NbtViewPropIdType) MetaDataProp.FKType;
            NbtViewPropIdType CurrentPropTypePropIdType = (NbtViewPropIdType) MetaDataProp.ValuePropType;

            //We're changing the relationship
            if( NewFkPropIdType != CurrentFkPropIdType || inFKValue != MetaDataProp.FKValue )
            {
                bool ClearValueProp = _isInvalidFkTarget( NewFkPropIdType, inFKValue, inValuePropType, inValuePropId );
                OutFkType = inFKType;
                OutFkValue = inFKValue;
                if( ClearValueProp )
                {
                    OutValuePropType = string.Empty;
                    OutValuePropId = Int32.MinValue;
                }
                else
                {
                    OutValuePropType = NewPropTypePropIdType.ToString();
                    OutValuePropId = inValuePropId;
                }
            }
            doSetFk( OutFkType, OutFkValue, OutValuePropType, OutValuePropId );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

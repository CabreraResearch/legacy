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

            SequenceSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.Sequence );
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

            SequenceNumberSubField = new CswNbtSubField( _CswNbtFieldResources, SequenceNumberColumn, CswEnumNbtSubFieldName.Number );
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
            if( OldSubfieldName == CachedValueSubField.Name && OldValue.ToLower() == "me" )
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
                    CswNbtViewPropertyFilterIn.SubfieldName = CachedValueSubField.Name;
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

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            string OutFkType = inFKType;
            Int32 OutFkValue = inFKValue;
            string OutValuePropType = inValuePropType;
            Int32 OutValuePropId = inValuePropId;

            //New PropIdTypes
            CswEnumNbtViewPropIdType NewFkPropIdType = (CswEnumNbtViewPropIdType) inFKType;
            CswEnumNbtViewPropIdType NewPropTypePropIdType = (CswEnumNbtViewPropIdType) inValuePropType;

            //Current PropIdTypes
            CswEnumNbtViewPropIdType CurrentFkPropIdType = (CswEnumNbtViewPropIdType) MetaDataProp.FKType;
            CswEnumNbtViewPropIdType CurrentPropTypePropIdType = (CswEnumNbtViewPropIdType) MetaDataProp.ValuePropType;

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

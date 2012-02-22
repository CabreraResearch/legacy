using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRulePropertyReference : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRulePropertyReference( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CachedValueSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Value );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Begins );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Ends );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.GreaterThan );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.LessThan );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            CachedValueSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            SubFields.add( CachedValueSubField );

        }//ctor

        public CswNbtSubField CachedValueSubField;

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
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn ) );
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        private bool _validateRelationship( string FkType, Int32 FkValue, Int32 inValuePropId )
        {
            bool RetIsInvalid = false;
            NbtViewRelatedIdType RelatedIdType;
            Enum.TryParse( FkType, true, out RelatedIdType );
            switch( RelatedIdType )
            {
                case NbtViewRelatedIdType.NodeTypeId:
                    CswNbtMetaDataNodeType FkRelationshipTargetNt = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeType( FkValue );
                    RetIsInvalid = ( null == FkRelationshipTargetNt );
                    if( false == RetIsInvalid )
                    {
                        CswNbtMetaDataNodeTypeProp ValueNtp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inValuePropId );
                        RetIsInvalid = ( null == ValueNtp ||
                                         _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeFirstVersion( ValueNtp.NodeTypeId ) != FkRelationshipTargetNt.getFirstVersionNodeType() );
                    }
                    break;
                case NbtViewRelatedIdType.ObjectClassId:
                    CswNbtMetaDataObjectClass FkRelationshipTargetOc = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( FkValue );
                    RetIsInvalid = ( null == FkRelationshipTargetOc );

                    if( false == RetIsInvalid )
                    {
                        CswNbtMetaDataObjectClassProp ValueOcp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inValuePropId );
                        RetIsInvalid = ( null == ValueOcp || ( ValueOcp.getObjectClass().ObjectClass != FkRelationshipTargetOc.ObjectClass ) );
                    }
                    break;
            }
            return RetIsInvalid;
        }

        private bool _validateFkTarget( PropIdType NewFkPropIdType, Int32 inFKValue, Int32 inValuePropId )
        {
            bool RetClearPropVal = false;
            switch( NewFkPropIdType )
            {
                case PropIdType.NodeTypePropId:
                    CswNbtMetaDataNodeTypeProp FkNtp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inFKValue );
                    RetClearPropVal = ( null == FkNtp );
                    if( false == RetClearPropVal )
                    {
                        RetClearPropVal = _validateRelationship( FkNtp.FKType, FkNtp.FKValue, inValuePropId );
                    }
                    break;
                case PropIdType.ObjectClassPropId:
                    CswNbtMetaDataObjectClassProp FkOcp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inFKValue );
                    RetClearPropVal = ( null == FkOcp );
                    if( false == RetClearPropVal )
                    {
                        RetClearPropVal = _validateRelationship( FkOcp.FKType, FkOcp.FKValue, inValuePropId );
                    }
                    break;
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
            PropIdType NewFkPropIdType;
            Enum.TryParse( inFKType, true, out NewFkPropIdType );

            PropIdType NewPropTypePropIdType;
            Enum.TryParse( inValuePropType, true, out NewPropTypePropIdType );

            //Current PropIdTypes
            PropIdType CurrentFkPropIdType;
            Enum.TryParse( MetaDataProp.FKType, true, out CurrentFkPropIdType );

            PropIdType CurrentPropTypePropIdType;
            Enum.TryParse( MetaDataProp.ValuePropType, true, out CurrentPropTypePropIdType );

            //We're changing the relationship
            if( NewFkPropIdType != CurrentFkPropIdType ||
                NewPropTypePropIdType != CurrentPropTypePropIdType ||
                inFKValue != MetaDataProp.FKValue ||
                inValuePropId != MetaDataProp.ValuePropId
                )
            {
                bool ClearValueProp = _validateFkTarget( NewFkPropIdType, inFKValue, inValuePropId );
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
                doSetFk( OutFkType, OutFkValue, OutValuePropType, OutValuePropId );
            }
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

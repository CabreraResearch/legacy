using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRulePropertyReference : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;
        private ICswNbtMetaDataProp _MetaDataProp = null;

        public CswNbtFieldTypeRulePropertyReference( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );
            _MetaDataProp = MetaDataProp;

            CachedValueSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Value );
            CachedValueSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                              CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                              CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                              CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                              CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                              CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                              CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                              CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                              CswNbtPropFilterSql.PropertyFilterMode.Null;
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

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        private bool _validateFkTarget( CswNbtViewRelationship.PropIdType NewFkPropIdType, Int32 inFKValue, bool CheckProp = false, Int32 inValuePropId = Int32.MinValue )
        {
            bool RetClearPropVal = false;
            switch( NewFkPropIdType )
            {
                case CswNbtViewRelationship.PropIdType.NodeTypePropId:
                    CswNbtMetaDataNodeTypeProp FkNtp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inFKValue );
                    if( null == FkNtp )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot create a property reference without a valid property id.", "Attempted to instance a NodeTypeProp with inFKValue: " + inFKValue + ", but the response was null." );
                    }
                    if( CheckProp )
                    {
                        CswNbtMetaDataNodeType FkRelationshipTargetNt = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeType( FkNtp.FKValue );
                        RetClearPropVal = ( null == FkRelationshipTargetNt );
                        if( false == RetClearPropVal )
                        {
                            CswNbtMetaDataNodeTypeProp ValueNtp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( inValuePropId );
                            RetClearPropVal = ( null == ValueNtp || ( ValueNtp.NodeType.FirstVersionNodeType != FkRelationshipTargetNt.FirstVersionNodeType ) );
                        }
                    }
                    break;
                case CswNbtViewRelationship.PropIdType.ObjectClassPropId:
                    CswNbtMetaDataObjectClassProp FkOcp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inFKValue );
                    if( null == FkOcp )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot create a property reference without a valid property id.", "Attempted to instance an ObjectClassProp with inFKValue: " + inFKValue + ", but the response was null." );
                    }
                    if( CheckProp )
                    {
                        CswNbtMetaDataObjectClass FkRelationshipTargetOc = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( FkOcp.FKValue );
                        RetClearPropVal = ( null == FkRelationshipTargetOc );

                        if( false == RetClearPropVal )
                        {
                            CswNbtMetaDataObjectClassProp ValueOcp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( inValuePropId );
                            RetClearPropVal = ( null == ValueOcp || ( ValueOcp.ObjectClass.ObjectClass != FkRelationshipTargetOc.ObjectClass ) );
                        }
                    }
                    break;
            }
            return RetClearPropVal;
        }

        public void setFk( CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            string OutFkType = inFKType;
            Int32 OutFkValue = inFKValue;
            string OutValuePropType = inValuePropType;
            Int32 OutValuePropId = inValuePropId;

            //New PropIdTypes
            CswNbtViewRelationship.PropIdType NewFkPropIdType;
            Enum.TryParse( inFKType, true, out NewFkPropIdType );

            CswNbtViewRelationship.PropIdType NewPropTypePropIdType;
            Enum.TryParse( inValuePropType, true, out NewPropTypePropIdType );

            if( NewFkPropIdType != NewPropTypePropIdType )
            {
                throw new CswDniException( "Cannot create a Property reference if the property types do not match." );
            }

            //Current PropIdTypes
            CswNbtViewRelationship.PropIdType CurrentFkPropIdType;
            Enum.TryParse( _MetaDataProp.FKType, true, out CurrentFkPropIdType );

            CswNbtViewRelationship.PropIdType CurrentPropTypePropIdType;
            Enum.TryParse( _MetaDataProp.ValuePropType, true, out CurrentPropTypePropIdType );

            //We're changing the relationship
            if( ( false == string.IsNullOrEmpty( inFKType ) &&
                  string.IsNullOrEmpty( inValuePropType ) &&
                  Int32.MinValue != inFKValue &&
                  Int32.MinValue == inValuePropId &&
                  NewFkPropIdType != CswNbtViewRelationship.PropIdType.Unknown ) &&
                (
                  NewFkPropIdType != CurrentFkPropIdType ||
                  inFKValue != _MetaDataProp.FKValue
                ) //something has changed 
              )
            {
                _validateFkTarget( NewFkPropIdType, inFKValue );
                OutFkType = NewFkPropIdType.ToString();
                OutFkValue = inFKValue;
                OutValuePropType = string.Empty;
                OutValuePropId = Int32.MinValue;
                doSetFk( OutFkType, OutFkValue, OutValuePropType, OutValuePropId );
            }
            //We have valid values that are different that what is currently set
            else if( ( false == string.IsNullOrEmpty( inFKType ) &&
                  false == string.IsNullOrEmpty( inValuePropType ) &&
                  Int32.MinValue != inFKValue &&
                  Int32.MinValue != inValuePropId &&
                  NewFkPropIdType != CswNbtViewRelationship.PropIdType.Unknown &&
                  NewPropTypePropIdType != CswNbtViewRelationship.PropIdType.Unknown
                ) &&
                (
                  NewFkPropIdType != CurrentFkPropIdType ||
                  NewPropTypePropIdType != CurrentPropTypePropIdType ||
                  inFKValue != _MetaDataProp.FKValue ||
                  inValuePropId != _MetaDataProp.ValuePropId
                ) //something has changed 
              )
            {
                bool ClearValueProp = _validateFkTarget( NewFkPropIdType, inFKValue, true, inValuePropId );
                OutFkType = NewFkPropIdType.ToString();
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

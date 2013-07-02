using System;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleQuantity : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;
        public CswNbtSubField QuantitySubField;
        public CswNbtSubField UnitNameSubField;
        public CswNbtSubField UnitIdSubField;

        public CswNbtFieldTypeRuleQuantity( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            QuantitySubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Numeric, CswEnumNbtSubFieldName.Value, true );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( QuantitySubField, true );

            UnitIdSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, CswEnumNbtSubFieldName.NodeID, true );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.In );
            SubFields.add( UnitIdSubField );

            UnitNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Name );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( UnitNameSubField );

        }//ctor

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            // BZ 7941
            bool UseNumericHack = CswNbtViewPropertyFilterIn.SubfieldName == CswEnumNbtSubFieldName.Value;

            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, UseNumericHack ) );
        }


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries, UnitIdSubField );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            string OutFkType = inFKType;
            Int32 OutFkValue = inFKValue;
            string OutValuePropType = inValuePropType;
            Int32 OutValuePropId = inValuePropId;

            CswEnumNbtViewRelatedIdType NewFkPropIdType = (CswEnumNbtViewRelatedIdType) inFKType;
            CswEnumNbtViewRelatedIdType CurrentFkPropIdType = (CswEnumNbtViewRelatedIdType) MetaDataProp.FKType;

            if( ( false == string.IsNullOrEmpty( inFKType ) &&
                  Int32.MinValue != inFKValue &&
                  NewFkPropIdType != CswEnumNbtViewRelatedIdType.Unknown
                ) &&
                (
                  NewFkPropIdType != CurrentFkPropIdType ||
                  inFKValue != MetaDataProp.FKValue
                )
              )
            {
                _setDefaultView( MetaDataProp, NewFkPropIdType, inFKValue, false );
                OutFkType = NewFkPropIdType.ToString();
                OutFkValue = inFKValue;
                OutValuePropType = string.Empty;
                OutValuePropId = Int32.MinValue;
                doSetFk( OutFkType, OutFkValue, OutValuePropType, OutValuePropId );
            }
            else
            {
                _setDefaultView( MetaDataProp, CurrentFkPropIdType, MetaDataProp.FKValue, true );
            }
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Quantity );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = CswEnumNbtPropertyAttributeName.Precision,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numberprecision
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = CswEnumNbtPropertyAttributeName.MinimumValue,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numberminvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = CswEnumNbtPropertyAttributeName.MaximumValue,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numbermaxvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = CswEnumNbtPropertyAttributeName.UnitTarget,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship,
                    Column = CswEnumNbtPropertyAttributeColumn.Fkvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = CswEnumNbtPropertyAttributeName.UnitView,
                    AttributeFieldType = CswEnumNbtFieldType.ViewReference,
                    Column = CswEnumNbtPropertyAttributeColumn.Nodeviewid,
                    SubFieldName = CswEnumNbtSubFieldName.ViewID
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = CswEnumNbtPropertyAttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.Quantity
            } );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = CswEnumNbtPropertyAttributeName.QuantityOptional,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute1,
                AttributeFieldType = CswEnumNbtFieldType.Logical
            } );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = CswEnumNbtPropertyAttributeName.ExcludeRangeLimits,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute2,
                AttributeFieldType = CswEnumNbtFieldType.Logical
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            if( null != NodeTypeProp )
            {
                string FkType = NodeTypeProp.FKType;
                Int32 FkValue = NodeTypeProp.FKValue;

                if( false == string.IsNullOrEmpty( FkType ) &&
                    Int32.MinValue != FkValue )
                {
                    NodeTypeProp.SetFK( FkType, FkValue );
                }

                _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
            }
        }

        private CswNbtView _setDefaultView( CswNbtMetaDataNodeTypeProp MetaDataProp, CswEnumNbtViewRelatedIdType RelatedIdType, Int32 inFKValue, bool OnlyCreateIfNull )
        {
            CswNbtView RetView = _CswNbtFieldResources.CswNbtResources.ViewSelect.restoreView( MetaDataProp.ViewId );
            if( RelatedIdType != CswEnumNbtViewRelatedIdType.Unknown &&
                ( null == RetView ||
                  RetView.Root.ChildRelationships.Count == 0 ||
                  false == OnlyCreateIfNull ) )
            {

                if( null != RetView )
                {
                    RetView.Root.ChildRelationships.Clear();
                }

                //if( RelatedIdType == NbtViewRelatedIdType.ObjectClassId )
                //{
                //    CswNbtMetaDataObjectClass TargetOc = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( inFKValue );
                //    if( null == TargetOc )
                //    {
                //        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                //    }
                //    RetView = TargetOc.CreateDefaultView();
                //}
                //else if( RelatedIdType == NbtViewRelatedIdType.NodeTypeId )
                //{
                //    CswNbtMetaDataNodeType TargetNt = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeType( inFKValue );
                //    if( null == TargetNt )
                //    {
                //        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                //    }
                //    RetView = TargetNt.CreateDefaultView();
                //}
                //else if( RelatedIdType == NbtViewRelatedIdType.PropertySetId )
                //{
                //    CswNbtMetaDataPropertySet TargetPs = _CswNbtFieldResources.CswNbtResources.MetaData.getPropertySet( inFKValue );
                //    if( null == TargetPs )
                //    {
                //        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to propertysetid: " + inFKValue + ", but the target is null." );
                //    }
                //    RetView = TargetPs.CreateDefaultView();
                //}
                //else
                //{
                //    throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Invalid RelatedIdType: " + RelatedIdType + "." );
                //}
                ICswNbtMetaDataDefinitionObject targetObj = _CswNbtFieldResources.CswNbtResources.MetaData.getDefinitionObject( RelatedIdType, inFKValue );
                if( null != targetObj )
                {
                    RetView = targetObj.CreateDefaultView();
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Cannot create a relationship without a valid target.", "_setDefaultView() got an invalid RelatedIdType: " + RelatedIdType + " or value: " + inFKValue );
                }

                RetView.ViewId = MetaDataProp.ViewId;
                RetView.Visibility = CswEnumNbtViewVisibility.Property;
                RetView.ViewMode = CswEnumNbtViewRenderingMode.List;
                RetView.ViewName = MetaDataProp.PropName;
                RetView.save();
            }
            return RetView;
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

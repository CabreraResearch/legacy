using System;
using System.Collections.Generic;
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
        public CswNbtSubField Val_kg_SubField;
        public CswNbtSubField Val_Liters_SubField;

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

            Val_kg_SubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2_Numeric, CswEnumNbtSubFieldName.Val_kg, true );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( Val_kg_SubField, true );

            Val_Liters_SubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3_Numeric, CswEnumNbtSubFieldName.Val_Liters, true );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( Val_Liters_SubField, true );

        }//ctor

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }
        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber )
        {
            // BZ 7941
            bool UseNumericHack = CswNbtViewPropertyFilterIn.SubfieldName == CswEnumNbtSubFieldName.Value;
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber, UseNumericHack ) );
        }//makeWhereClause()

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

        public string getHelpText()
        {
            return string.Empty;
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

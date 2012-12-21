using System;
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

            QuantitySubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1_Numeric, CswNbtSubField.SubFieldName.Value, true );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.GreaterThan );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.LessThan );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            QuantitySubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            SubFields.add( QuantitySubField, true );

            UnitIdSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1_FK, CswNbtSubField.SubFieldName.NodeID, true );
            UnitIdSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            UnitIdSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            UnitIdSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            UnitIdSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            SubFields.add( UnitIdSubField );

            UnitNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Name );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Begins );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Ends );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotContains );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            UnitNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
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
            bool UseNumericHack = CswNbtViewPropertyFilterIn.SubfieldName == CswNbtSubField.SubFieldName.Value;

            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, UseNumericHack ) );
        }


        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
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

            NbtViewRelatedIdType NewFkPropIdType = (NbtViewRelatedIdType) inFKType;
            NbtViewRelatedIdType CurrentFkPropIdType = (NbtViewRelatedIdType) MetaDataProp.FKType;

            if( ( false == string.IsNullOrEmpty( inFKType ) &&
                  Int32.MinValue != inFKValue &&
                  NewFkPropIdType != NbtViewRelatedIdType.Unknown
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

        private CswNbtView _setDefaultView( CswNbtMetaDataNodeTypeProp MetaDataProp, NbtViewRelatedIdType RelatedIdType, Int32 inFKValue, bool OnlyCreateIfNull )
        {
            CswNbtView RetView = _CswNbtFieldResources.CswNbtResources.ViewSelect.restoreView( MetaDataProp.ViewId );
            if( RelatedIdType != NbtViewRelatedIdType.Unknown &&
                ( null == RetView ||
                  RetView.Root.ChildRelationships.Count == 0 ||
                  false == OnlyCreateIfNull ) )
            {

                if( null != RetView )
                {
                    RetView.Root.ChildRelationships.Clear();
                }

                if( RelatedIdType == NbtViewRelatedIdType.ObjectClassId )
                {
                    CswNbtMetaDataObjectClass TargetOc = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( inFKValue );
                    if( null == TargetOc )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                    }
                    RetView = TargetOc.CreateDefaultView();
                }
                else if( RelatedIdType == NbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType TargetNt = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeType( inFKValue );
                    if( null == TargetNt )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                    }
                    RetView = TargetNt.CreateDefaultView();
                }

                RetView.ViewId = MetaDataProp.ViewId;
                RetView.Visibility = NbtViewVisibility.Property;
                RetView.ViewMode = NbtViewRenderingMode.List;
                RetView.ViewName = MetaDataProp.PropName;
                RetView.save();
            }
            return RetView;
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleMetaDataList : ICswNbtFieldTypeRule
    {
        public static char delimiter = CswNbtNodeTypePropListOptions.delimiter;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleMetaDataList( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            IdSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, CswEnumNbtSubFieldName.Id );
            IdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            IdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            IdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            IdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SubFields.add( IdSubField );

            TypeSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Type );
            TypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            TypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            TypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            TypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SubFields.add( TypeSubField );

            TextSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.Text );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( TextSubField );                                  
        }//ctor

        public CswNbtSubField TypeSubField;
        public CswNbtSubField IdSubField;
        public CswNbtSubField TextSubField;

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


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( MetaDataProp, doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.MetaDataList );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.MetaDataList,
                Name = CswEnumNbtPropertyAttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.MetaDataList
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData


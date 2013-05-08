using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleNFPA : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleNFPA( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            RedSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Flammability, true );
            YellowSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.Reactivity, true );
            BlueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3, CswEnumNbtSubFieldName.Health, true );
            WhiteSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field4, CswEnumNbtSubFieldName.Special, true );

            RedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            RedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            RedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            RedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            RedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            RedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            YellowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            YellowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            YellowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            YellowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            YellowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            YellowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            BlueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            BlueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            BlueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            BlueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            BlueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            BlueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            WhiteSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            WhiteSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            WhiteSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            WhiteSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            WhiteSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            WhiteSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );

            SubFields.add( RedSubField );
            SubFields.add( YellowSubField );
            SubFields.add( BlueSubField );
            SubFields.add( WhiteSubField );
        }//ctor

        public CswNbtSubField RedSubField;
        public CswNbtSubField YellowSubField;
        public CswNbtSubField BlueSubField;
        public CswNbtSubField WhiteSubField;

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
            Collection<CswNbtFieldTypeAttribute> ret = new Collection<CswNbtFieldTypeAttribute>();
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.NFPA,
                    Name = CswEnumNbtPropertyAttributeName.DisplayMode,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Attribute1
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.NFPA,
                Name = CswEnumNbtPropertyAttributeName.HideSpecial,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute2
            } );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.NFPA,
                Name = CswEnumNbtPropertyAttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.NFPA
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleNFPA

}//namespace ChemSW.Nbt.MetaData

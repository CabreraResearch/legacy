using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleNFPA : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Flammability = CswEnumNbtSubFieldName.Flammability;
            public static CswEnumNbtSubFieldName Reactivity = CswEnumNbtSubFieldName.Reactivity;
            public static CswEnumNbtSubFieldName Health = CswEnumNbtSubFieldName.Health;
            public static CswEnumNbtSubFieldName Special = CswEnumNbtSubFieldName.Special;
        }


        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleNFPA( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            RedSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Flammability, true );
            YellowSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, SubFieldName.Reactivity, true );
            BlueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3, SubFieldName.Health, true );
            WhiteSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field4, SubFieldName.Special, true );

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

        public void onSetFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( MetaDataProp, DesignNTPNode );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string DisplayMode = CswEnumNbtPropertyAttributeName.DisplayMode;
            public const string HideSpecial = CswEnumNbtPropertyAttributeName.HideSpecial;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.NFPA );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.NFPA,
                    Name = AttributeName.DisplayMode,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Attribute1
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.NFPA,
                Name = AttributeName.HideSpecial,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute2
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.NFPA,
                Name = AttributeName.DefaultValue,
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

using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleButton : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Value = CswEnumNbtSubFieldName.Value;
            public static CswEnumNbtSubFieldName Options = CswEnumNbtSubFieldName.Options;
            public static CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Name;
            public static CswEnumNbtSubFieldName Icon = CswEnumNbtSubFieldName.Icon;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleButton( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            StateSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Value );
            SubFields.add( StateSubField );

            MenuOptionsSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, SubFieldName.Options );
            SubFields.add( MenuOptionsSubField );

            DisplayNameField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3, SubFieldName.Name );
            SubFields.add( DisplayNameField );

            IconSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field4, SubFieldName.Icon );
            SubFields.add( IconSubField );
        }//ctor

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        /// <summary>
        /// Optional subfield to store the state of the button (e.g. Clicked once, Submitted)
        /// </summary>
        public CswNbtSubField StateSubField;
        /// <summary>
        /// If a menu button, menu options will likely be stored on a per-node basis according to business logic
        /// </summary>
        public CswNbtSubField MenuOptionsSubField;
        /// <summary>
        /// Specify an icon for the button
        /// </summary>
        public CswNbtSubField IconSubField;
        /// <summary>
        /// Optional Override for Button's Displayed text
        /// </summary>
        public CswNbtSubField DisplayNameField;

        public bool SearchAllowed { get { return ( false ); } }

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
            public const string ButtonText = CswEnumNbtPropertyAttributeName.ButtonText;
            public const string ConfirmationDialogMessage = CswEnumNbtPropertyAttributeName.ConfirmationDialogMessage;
            public const string DisplayMode = CswEnumNbtPropertyAttributeName.DisplayMode;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Button );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Button,
                Name = AttributeName.ButtonText,
                AttributeFieldType = CswEnumNbtFieldType.Text,
                Column = CswEnumNbtPropertyAttributeColumn.Statictext
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Button,
                Name = AttributeName.ConfirmationDialogMessage,
                AttributeFieldType = CswEnumNbtFieldType.Text,
                Column = CswEnumNbtPropertyAttributeColumn.Valueoptions
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Button,
                Name = AttributeName.DisplayMode,
                AttributeFieldType = CswEnumNbtFieldType.List,
                Column = CswEnumNbtPropertyAttributeColumn.Extended
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleImageList : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleImageList( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            ValueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.ClobData, CswEnumNbtSubFieldName.Value );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ValueSubField );
        }//ctor

        public CswNbtSubField ValueSubField;

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

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string HeightInPixels = CswEnumNbtPropertyAttributeName.HeightInPixels;
            public const string WidthInPixels = CswEnumNbtPropertyAttributeName.WidthInPixels;
            public const string AllowMultipleValues = CswEnumNbtPropertyAttributeName.AllowMultipleValues;
            public const string ImageNames = CswEnumNbtPropertyAttributeName.ImageNames;
            public const string ImageUrls = CswEnumNbtPropertyAttributeName.ImageUrls;
            public const string Prefix = CswEnumNbtPropertyAttributeName.Prefix;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.ImageList );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = AttributeName.HeightInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textarearows
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = AttributeName.WidthInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textareacols
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = AttributeName.AllowMultipleValues,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Extended
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = AttributeName.ImageNames,
                    AttributeFieldType = CswEnumNbtFieldType.Memo,
                    Column = CswEnumNbtPropertyAttributeColumn.Listoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = AttributeName.ImageUrls,
                    AttributeFieldType = CswEnumNbtFieldType.Memo,
                    Column = CswEnumNbtPropertyAttributeColumn.Valueoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.ImageList,
                Name = AttributeName.DefaultValue,
                AttributeFieldType = CswEnumNbtFieldType.ImageList,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.ImageList,
                Name = AttributeName.Prefix,
                AttributeFieldType = CswEnumNbtFieldType.Text,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute1
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleImageList

}//namespace ChemSW.Nbt.MetaData

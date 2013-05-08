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

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = new Collection<CswNbtFieldTypeAttribute>();
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = CswEnumNbtPropertyAttributeName.HeightInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textarearows
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = CswEnumNbtPropertyAttributeName.WidthInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textareacols
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = CswEnumNbtPropertyAttributeName.AllowMultipleValues,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Extended
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = CswEnumNbtPropertyAttributeName.ImageNames,
                    AttributeFieldType = CswEnumNbtFieldType.Memo,
                    Column = CswEnumNbtPropertyAttributeColumn.Listoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.ImageList,
                    Name = CswEnumNbtPropertyAttributeName.ImageUrls,
                    AttributeFieldType = CswEnumNbtFieldType.Memo,
                    Column = CswEnumNbtPropertyAttributeColumn.Valueoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.ImageList,
                Name = CswEnumNbtPropertyAttributeName.DefaultValue,
                AttributeFieldType = CswEnumNbtFieldType.ImageList,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleImageList

}//namespace ChemSW.Nbt.MetaData

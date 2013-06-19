using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleImage : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleImage( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            //FileNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Name );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            //FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            //SubFields.add( FileNameSubField );

            //ContentTypeSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.ContentType );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            //ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            //SubFields.add( ContentTypeSubField );

            //SubFields.add( CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Image );
            //SubFields[CswEnumNbtSubFieldName.Image].SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull |
            //                                  CswEnumNbtFilterMode.Null;

        }//ctor

        //public CswNbtSubField FileNameSubField;
        //public CswNbtSubField ContentTypeSubField;
        //public CswNbtSubField BlobSubField;

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
                    OwnerFieldType = CswEnumNbtFieldType.Image,
                    Name = CswEnumNbtPropertyAttributeName.HeightInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textarearows
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Image,
                    Name = CswEnumNbtPropertyAttributeName.WidthInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textareacols
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleImage

}//namespace ChemSW.Nbt.MetaData

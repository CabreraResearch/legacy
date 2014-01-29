using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleImage : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
        }

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

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber )
        {
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber ) );
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( DesignNTPNode );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string HeightInPixels = CswEnumNbtPropertyAttributeName.HeightInPixels;
            public const string WidthInPixels = CswEnumNbtPropertyAttributeName.WidthInPixels;
            public const string MaximumValue = CswEnumNbtPropertyAttributeName.MaximumValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Image );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Image,
                    Name = AttributeName.HeightInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textarearows
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Image,
                Name = AttributeName.WidthInPixels,
                AttributeFieldType = CswEnumNbtFieldType.Number,
                Column = CswEnumNbtPropertyAttributeColumn.Textareacols
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Image,
                Name = AttributeName.MaximumValue,
                AttributeFieldType = CswEnumNbtFieldType.Number,
                Column = CswEnumNbtPropertyAttributeColumn.Numbermaxvalue
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public string getHelpText()
        {
            return string.Empty;
        }

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode ) { }

    }//CswNbtFieldTypeRuleImage

}//namespace ChemSW.Nbt.MetaData

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleBlob : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Name;
            public static CswEnumNbtSubFieldName ContentType = CswEnumNbtSubFieldName.ContentType;
            public static CswEnumNbtSubFieldName DateModified = CswEnumNbtSubFieldName.DateModified;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleBlob( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            FileNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Name );
            FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            FileNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( FileNameSubField );

            ContentTypeSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, SubFieldName.ContentType );
            ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ContentTypeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ContentTypeSubField );

            DateModifiedSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2_Date, SubFieldName.DateModified );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            DateModifiedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( DateModifiedSubField );

            //BlobSubField = new CswNbtSubField( CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Blob );
            //BlobSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull |
            //                           CswEnumNbtFilterMode.Null;
            //SubFields.add( BlobSubField );

        }//ctor

        public CswNbtSubField FileNameSubField;
        public CswNbtSubField ContentTypeSubField;
        public CswNbtSubField DateModifiedSubField;
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
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser,CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber ) );
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
            public const string Length = CswEnumNbtPropertyAttributeName.Length;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.File );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.File,
                Name = AttributeName.Length,
                AttributeFieldType = CswEnumNbtFieldType.Number,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute1
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

    }//CswNbtFieldTypeRuleBlob

}//namespace ChemSW.Nbt.MetaData

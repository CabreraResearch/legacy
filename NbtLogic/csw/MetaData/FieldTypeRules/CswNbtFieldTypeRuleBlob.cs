using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleBlob: ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleBlob( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );

            FileNameSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Name );
            FileNameSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                           CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                           CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                           CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                           CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( FileNameSubField );

            ContentTypeSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field2, CswNbtSubField.SubFieldName.ContentType );
            ContentTypeSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                              CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                              CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                              CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                              CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                              CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                              CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( ContentTypeSubField );

            //BlobSubField = new CswNbtSubField( CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Blob );
            //BlobSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.NotNull |
            //                           CswNbtPropFilterSql.PropertyFilterMode.Null;
            //SubFields.add( BlobSubField );

        }//ctor

        public CswNbtSubField FileNameSubField;
        public CswNbtSubField ContentTypeSubField;
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

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        public void afterCreateNodeTypeProp(  CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleBlob

}//namespace ChemSW.Nbt.MetaData

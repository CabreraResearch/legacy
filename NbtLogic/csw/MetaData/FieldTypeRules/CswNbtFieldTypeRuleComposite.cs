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

    public class CswNbtFieldTypeRuleComposite : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleComposite( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );

            CachedValueSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Value );
            CachedValueSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                              CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                              CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                              CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                              CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                              CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                              CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                              CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                              CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( CachedValueSubField );
        }//ctor

        public CswNbtSubField CachedValueSubField;

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

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

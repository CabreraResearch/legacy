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

    public class CswNbtFieldTypeRuleViewPickList: ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtPropFilterSql _CswNbtPropFilterSql = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleViewPickList( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );

            CachedViewNameSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Gestalt, CswNbtSubField.SubFieldName.Name );
            CachedViewNameSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                                 CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                                 CswNbtPropFilterSql.PropertyFilterMode.Null |
                                                 CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                                 CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                                 CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                                 CswNbtPropFilterSql.PropertyFilterMode.NotEquals;
            SubFields.add( CachedViewNameSubField);

            SelectedViewIdsSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.ViewID );
            SelectedViewIdsSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                                  CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                                  CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( SelectedViewIdsSubField );

        }//ctor

        public CswNbtSubField SelectedViewIdsSubField;
        public CswNbtSubField CachedViewNameSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }



        public bool SearchAllowed { get { return false; } } // return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

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

    }//CswNbtFieldTypeRuleViewPickList

}//namespace ChemSW.Nbt.MetaData

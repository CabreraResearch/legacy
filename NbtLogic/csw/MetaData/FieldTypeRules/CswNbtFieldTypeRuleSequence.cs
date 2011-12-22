using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleSequence : ICswNbtFieldTypeRule
    {
        public static CswNbtSubField.PropColumn SequenceNumberColumn = CswNbtSubField.PropColumn.Field1_Numeric;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleSequence( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            SequenceSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Sequence );
            SequenceSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                           CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                           CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                           CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                           CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                           CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( SequenceSubField );

            SequenceNumberSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, SequenceNumberColumn, CswNbtSubField.SubFieldName.Number );
            SequenceNumberSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                          CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                          CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                          CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                          CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                          CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                          CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                          CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                          CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( SequenceNumberSubField );

        }//ctor

        public CswNbtSubField SequenceSubField;
        public CswNbtSubField SequenceNumberSubField;

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

        public void setFk( CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

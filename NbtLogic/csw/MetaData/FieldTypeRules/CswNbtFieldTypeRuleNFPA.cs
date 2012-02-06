using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleNFPA : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleNFPA( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            RedSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Flammability );
            YellowSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field2, CswNbtSubField.SubFieldName.Reactivity );
            BlueSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field3, CswNbtSubField.SubFieldName.Health );
            WhiteSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field4, CswNbtSubField.SubFieldName.Special );

            RedSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                     CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                     CswNbtPropFilterSql.PropertyFilterMode.Null;
            YellowSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                     CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                     CswNbtPropFilterSql.PropertyFilterMode.Null;
            BlueSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                     CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                     CswNbtPropFilterSql.PropertyFilterMode.Null;
            WhiteSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                     CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                     CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                     CswNbtPropFilterSql.PropertyFilterMode.Null;

            SubFields.add( RedSubField );
            SubFields.add( YellowSubField );
            SubFields.add( BlueSubField );
            SubFields.add( WhiteSubField );
        }//ctor

        public CswNbtSubField RedSubField;
        public CswNbtSubField YellowSubField;
        public CswNbtSubField BlueSubField;
        public CswNbtSubField WhiteSubField;

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

    }//CswNbtFieldTypeRuleNFPA

}//namespace ChemSW.Nbt.MetaData

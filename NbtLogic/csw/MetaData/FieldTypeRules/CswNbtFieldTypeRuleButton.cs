using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleButton : ICswNbtFieldTypeRule
    {
        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleButton( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            StateSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Value );
            SubFields.add( StateSubField );

            MenuOptionsSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field2, CswNbtSubField.SubFieldName.Value );
            SubFields.add( MenuOptionsSubField );

            DisplayNameField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field3, CswNbtSubField.SubFieldName.Value );
            SubFields.add( DisplayNameField );

            IconSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field4, CswNbtSubField.SubFieldName.Value );
            SubFields.add( IconSubField );
        }//ctor

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        /// <summary>
        /// Optional subfield to store the state of the button (e.g. Clicked once, Submitted)
        /// </summary>
        public CswNbtSubField StateSubField;
        /// <summary>
        /// If a menu button, menu options will likely be stored on a per-node basis according to business logic
        /// </summary>
        public CswNbtSubField MenuOptionsSubField;
        /// <summary>
        /// Specify an icon for the button
        /// </summary>
        public CswNbtSubField IconSubField;
        /// <summary>
        /// Optional Override for Button's Displayed text
        /// </summary>
        public CswNbtSubField DisplayNameField;

        public bool SearchAllowed { get { return ( false ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn ) );
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
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

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

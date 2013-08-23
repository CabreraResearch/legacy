using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleLink : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleLink( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            TextSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Text );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( TextSubField, true );

            HrefSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.Href );
            HrefSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            HrefSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            HrefSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            HrefSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            HrefSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            HrefSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( HrefSubField );

        }//ctor

        public CswNbtSubField TextSubField;
        public CswNbtSubField HrefSubField;

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

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public string getHelpText()
        {
            return string.Empty;
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

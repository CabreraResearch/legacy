using System;
using System.Collections.Generic;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleMemo : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleMemo( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );


            TextSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Gestalt, CswEnumNbtSubFieldName.Text );   //bz # 6628: Gestalt instead of Field1
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            TextSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( TextSubField );

        }//ctor

        public CswNbtSubField TextSubField;

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

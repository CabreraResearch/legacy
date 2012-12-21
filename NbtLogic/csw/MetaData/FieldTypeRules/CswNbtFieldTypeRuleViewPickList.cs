using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleViewPickList : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        //private CswNbtPropFilterSql _CswNbtPropFilterSql = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleViewPickList( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;

            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CachedViewNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Gestalt, CswNbtSubField.SubFieldName.Name );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Begins );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotContains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Ends );
            CachedViewNameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            SubFields.add( CachedViewNameSubField );

            SelectedViewIdsSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.ViewID );
            SelectedViewIdsSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            SelectedViewIdsSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            SelectedViewIdsSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
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

    }//CswNbtFieldTypeRuleViewPickList

}//namespace ChemSW.Nbt.MetaData

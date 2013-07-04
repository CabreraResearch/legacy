using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleViewReference : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        //private CswNbtPropFilterSql _CswNbtPropFilterSql = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleViewReference( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CachedViewNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Name );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SubFields.add( CachedViewNameSubField );

            ViewIdSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, CswEnumNbtSubFieldName.ViewID );
            ViewIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            ViewIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ViewIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ViewIdSubField );

        }//ctor

        public CswNbtSubField ViewIdSubField;
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

        public new sealed class AttributeName
        {
            // public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            return new Collection<CswNbtFieldTypeAttribute>();
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleViewReference

}//namespace ChemSW.Nbt.MetaData

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleViewReference : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Name;
            public static CswEnumNbtSubFieldName ViewID = CswEnumNbtSubFieldName.ViewID;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        //private CswNbtPropFilterSql _CswNbtPropFilterSql = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleViewReference( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CachedViewNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Name );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SubFields.add( CachedViewNameSubField );

            ViewIdSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, SubFieldName.ViewID );
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

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( DesignNTPNode );
        }

        public sealed class AttributeName
        {
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.ViewReference );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.ViewReference,
                Name = AttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.ViewReference
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public string getHelpText()
        {
            return string.Empty;
        }

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode ) { }

    }//CswNbtFieldTypeRuleViewReference

}//namespace ChemSW.Nbt.MetaData

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleMultiList : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Value = CswEnumNbtSubFieldName.Value;
        }


        public static char delimiter = CswNbtNodeTypePropListOptions.delimiter;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleMultiList( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            ValueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.ClobData, SubFieldName.Value );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ValueSubField );
        }//ctor

        public CswNbtSubField ValueSubField;

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

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( DesignNTPNode );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string Options = CswEnumNbtPropertyAttributeName.Options;
            public const string ReadOnlyDelimiter = CswEnumNbtPropertyAttributeName.ReadOnlyDelimiter;
            public const string ReadOnlyHideThreshold = CswEnumNbtPropertyAttributeName.ReadOnlyHideThreshold;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.MultiList );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.MultiList,
                    Name = AttributeName.Options,
                    AttributeFieldType = CswEnumNbtFieldType.Text,
                    Column = CswEnumNbtPropertyAttributeColumn.Listoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.MultiList,
                    Name = AttributeName.ReadOnlyDelimiter,
                    AttributeFieldType = CswEnumNbtFieldType.Text,
                    Column = CswEnumNbtPropertyAttributeColumn.Extended
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.MultiList,
                    Name = AttributeName.ReadOnlyHideThreshold,
                    AttributeFieldType = CswEnumNbtFieldType.Text,
                    Column = CswEnumNbtPropertyAttributeColumn.Numbermaxvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.MultiList,
                Name = AttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.MultiList
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

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            CswNbtNodePropText OptionsProp = DesignNTPNode.AttributeProperty[AttributeName.Options].AsText;
            CswDelimitedString ListOptions = new CswDelimitedString( ',' );
            ListOptions.FromString( OptionsProp.Text.Trim() );
            List<string> ValidOptions = new List<string>();
            for( int i = 0; i < ListOptions.Count; i++ )
            {
                if( ValidOptions.Contains( ListOptions[i] ))
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "All options must be unique", "Duplicate option: " + ListOptions[i] );
                }
                else
                {
                    ValidOptions.Add( ListOptions[i] );
                }
            }
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

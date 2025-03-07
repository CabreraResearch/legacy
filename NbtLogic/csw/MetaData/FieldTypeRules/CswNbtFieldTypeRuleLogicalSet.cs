﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{
    class CswNbtFieldTypeRuleLogicalSet : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
        }


        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleLogicalSet( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            //CheckedSubField = new CswNbtSubField(CswEnumNbtPropColumn.Field1, CswNbtFieldTypeRule.SubFieldName. );
            //CheckedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals;
            //SubFields.add( CheckedSubField );

            ClobDataSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.ClobData, CswEnumNbtSubFieldName.Value );
            ClobDataSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ClobDataSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ClobDataSubField );

        }//ctor

        //public CswNbtSubField CheckedSubField;
        public CswNbtSubField ClobDataSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return false; } } //return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

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
            public const string Rows = CswEnumNbtPropertyAttributeName.Rows;
            public const string YOptions = CswEnumNbtPropertyAttributeName.YOptions;
            public const string XOptions = CswEnumNbtPropertyAttributeName.XOptions;
            public const string IsFK = CswEnumNbtPropertyAttributeName.IsFK;
            public const string FKType = CswEnumNbtPropertyAttributeName.FKType;
            public const string FKValue = CswEnumNbtPropertyAttributeName.FKValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.LogicalSet );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.LogicalSet,
                    Name = AttributeName.Rows,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textarearows
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.LogicalSet,
                    Name = AttributeName.YOptions,
                    AttributeFieldType = CswEnumNbtFieldType.Text,
                    Column = CswEnumNbtPropertyAttributeColumn.Listoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.LogicalSet,
                Name = AttributeName.XOptions,
                AttributeFieldType = CswEnumNbtFieldType.Text,
                Column = CswEnumNbtPropertyAttributeColumn.Valueoptions
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.LogicalSet,
                Name = AttributeName.IsFK,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Isfk
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.LogicalSet,
                Name = AttributeName.FKType,
                AttributeFieldType = CswEnumNbtFieldType.List,
                Column = CswEnumNbtPropertyAttributeColumn.Fktype
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.LogicalSet,
                Name = AttributeName.FKValue,
                AttributeFieldType = CswEnumNbtFieldType.Number,
                Column = CswEnumNbtPropertyAttributeColumn.Fkvalue
            } );
            //ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            //{
            //    OwnerFieldType = CswEnumNbtFieldType.LogicalSet,
            //    Name = CswEnumNbtPropertyAttributeName.DefaultValue,
            //    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
            //    AttributeFieldType = CswEnumNbtFieldType.LogicalSet
            //} );
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

    }//CswNbtFieldTypeRuleLogicalSet
}//namespace ChemSW.Nbt.MetaData

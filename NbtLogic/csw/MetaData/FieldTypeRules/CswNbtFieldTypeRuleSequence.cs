using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleSequence : ICswNbtFieldTypeRule
    {
        public static CswEnumNbtPropColumn SequenceNumberColumn = CswEnumNbtPropColumn.Field1_Numeric;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleSequence( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            SequenceSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Sequence );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SequenceSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( SequenceSubField );

            SequenceNumberSubField = new CswNbtSubField( _CswNbtFieldResources, SequenceNumberColumn, CswEnumNbtSubFieldName.Number );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SequenceNumberSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
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

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = new Collection<CswNbtFieldTypeAttribute>();
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Sequence,
                    Name = CswEnumNbtPropertyAttributeName.Sequence,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship,
                    Column = CswEnumNbtPropertyAttributeColumn.Sequenceid,
                    SubFieldName = CswEnumNbtSubFieldName.NodeID
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Sequence,
                    Name = CswEnumNbtPropertyAttributeName.DefaultValue,
                    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                    AttributeFieldType = CswEnumNbtFieldType.Text
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

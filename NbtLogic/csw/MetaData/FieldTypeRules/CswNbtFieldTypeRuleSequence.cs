using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleSequence : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Sequence = CswEnumNbtSubFieldName.Sequence;
            public static CswEnumNbtSubFieldName Number = CswEnumNbtSubFieldName.Number;
        }

        public static CswEnumNbtPropColumn SequenceNumberColumn = CswEnumNbtPropColumn.Field1_Numeric;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleSequence( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            SequenceSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Sequence );
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

            SequenceNumberSubField = new CswNbtSubField( _CswNbtFieldResources, SequenceNumberColumn, SubFieldName.Number );
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
            public const string Sequence = CswEnumNbtPropertyAttributeName.Sequence;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Sequence );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Sequence,
                    Name = AttributeName.Sequence,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship,
                    Column = CswEnumNbtPropertyAttributeColumn.Sequenceid,
                    SubFieldName = CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Sequence,
                    Name = AttributeName.DefaultValue,
                    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                    AttributeFieldType = CswEnumNbtFieldType.Text
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
            CswNbtNodePropRelationship SequenceProp = DesignNTPNode.AttributeProperty[CswEnumNbtPropertyAttributeName.Sequence].AsRelationship;
            if( SequenceProp.wasSubFieldModified( CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID ) )
            {
                if( null != DesignNTPNode.RelationalNodeTypeProp && null != DesignNTPNode.RelationalNodeType )
                {
                    // Update nodes
                    foreach( CswNbtNode CurrentNode in DesignNTPNode.RelationalNodeType.getNodes( false, false ) )
                    {
                        CswNbtNodePropWrapper CurrentProp = CurrentNode.Properties[DesignNTPNode.RelationalNodeTypeProp];
                        if( CurrentProp.AsSequence.Empty )
                        {
                            CurrentProp.AsSequence.setSequenceValue();
                        }
                    }
                } // if prop is sequence or barcode
            } // if(SequenceProp.wasSubFieldModified( CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID ) )
        } // onBeforeWriteDesignNode()

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

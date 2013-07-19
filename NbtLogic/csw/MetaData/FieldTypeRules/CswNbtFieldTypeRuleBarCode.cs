using System;
using System.Collections.ObjectModel;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleBarCode : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Barcode = CswEnumNbtSubFieldName.Barcode;
            public static CswEnumNbtSubFieldName Number = CswEnumNbtSubFieldName.Number;
        }

        public static CswEnumNbtPropColumn SequenceNumberColumn = CswEnumNbtPropColumn.Field1_Numeric;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleBarCode( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            BarcodeSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Barcode );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            BarcodeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( BarcodeSubField );

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

        public CswNbtSubField BarcodeSubField;
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

        public void onSetFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( MetaDataProp, DesignNTPNode );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            // Enforce only one Barcode property per nodetype
            CswTableSelect ProbeSelect = _CswNbtFieldResources.CswNbtResources.makeCswTableSelect( "barcode_check", "nodetype_props" );
            Int32 ExistingBarcodeCount = ProbeSelect.getRecordCount( "where nodetypeid=" + NodeTypeProp.NodeTypeId.ToString() +
                                                                      " and nodetypepropid <> " + NodeTypeProp.PropId +
                                                                      " and fieldtypeid=" + NodeTypeProp.FieldTypeId.ToString() );
            if( ExistingBarcodeCount > 0 )
                throw ( new CswDniException( CswEnumErrorType.Warning, "Nodetype already has a barcode", "Unable to add barcode node type property because the nodetype (" + NodeTypeProp.NodeTypeId.ToString() + ") already has a barcode" ) );

            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string Sequence = CswEnumNbtPropertyAttributeName.Sequence;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Barcode );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Barcode,
                    Name = AttributeName.Sequence,
                    Column = CswEnumNbtPropertyAttributeColumn.Sequenceid,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship,
                    SubFieldName = CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Barcode,
                    Name = AttributeName.DefaultValue,
                    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                    AttributeFieldType = CswEnumNbtFieldType.Text
                } );
            return ret;
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

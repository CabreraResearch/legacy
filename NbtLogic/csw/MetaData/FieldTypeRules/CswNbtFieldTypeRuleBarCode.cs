using System;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleBarCode : ICswNbtFieldTypeRule
    {
        public static CswEnumNbtPropColumn SequenceNumberColumn = CswEnumNbtPropColumn.Field1_Numeric;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleBarCode( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            BarcodeSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Barcode );
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

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( MetaDataProp, doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
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

        public string getHelpText()
        {
            return string.Empty;
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

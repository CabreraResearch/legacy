using System;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleBarCode : ICswNbtFieldTypeRule
    {
        public static CswNbtSubField.PropColumn SequenceNumberColumn = CswNbtSubField.PropColumn.Field1_Numeric;

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleBarCode( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            BarcodeSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Barcode );
            BarcodeSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                          CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                          CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                          CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                          CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                          CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                          CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                          CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                          CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( BarcodeSubField );

            SequenceNumberSubField = new CswNbtSubField( _CswNbtFieldResources, SequenceNumberColumn, CswNbtSubField.SubFieldName.Number );
            SequenceNumberSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                          CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                          CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                          CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                          CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                          CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                          CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                          CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                          CswNbtPropFilterSql.PropertyFilterMode.Null;
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

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
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
                throw ( new CswDniException( ErrorType.Warning, "Nodetype already has a barcode", "Unable to add barcode node type property because the nodetype (" + NodeTypeProp.NodeTypeId.ToString() + ") already has a barcode" ) );

            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

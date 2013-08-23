using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleLocation : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleLocation( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );


            NameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Name );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( NameSubField, true );

            NodeIdSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, CswEnumNbtSubFieldName.NodeID, true );
            NodeIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            NodeIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            NodeIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            NodeIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            NodeIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.In );
            SubFields.add( NodeIdSubField );

            RowSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.Row );
            RowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            RowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            RowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            RowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            RowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            RowSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( RowSubField );

            ColumnSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3, CswEnumNbtSubFieldName.Column );
            ColumnSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            ColumnSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            ColumnSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            ColumnSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            ColumnSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ColumnSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ColumnSubField );

            PathSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field4, CswEnumNbtSubFieldName.Path );
            PathSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            PathSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            PathSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            PathSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( PathSubField );


            BarcodeSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field5, CswEnumNbtSubFieldName.Barcode );
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



        }//ctor

        public CswNbtSubField NameSubField;
        public CswNbtSubField NodeIdSubField;
        public CswNbtSubField RowSubField;
        public CswNbtSubField ColumnSubField;
        public CswNbtSubField PathSubField;
        public CswNbtSubField BarcodeSubField;

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
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries, NodeIdSubField );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( MetaDataProp, doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            // Enforce only one Location property per nodetype
            foreach( CswNbtMetaDataNodeTypeProp OtherNodeTypeProp in _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProps( NodeTypeProp.NodeTypeId ) )
            {
                if( OtherNodeTypeProp != NodeTypeProp &&
                    OtherNodeTypeProp.getFieldTypeValue() == CswEnumNbtFieldType.Location )
                {
                    throw ( new CswDniException( CswEnumErrorType.Warning, "Nodetype already has a location", "Unable to add location node type property because the nodetype (" + NodeTypeProp.NodeTypeId.ToString() + ") already has a location" ) );
                }
            }

            // Locations have fixed fk relationship fields:
            Int32 LocationObjectClassId = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass ).ObjectClassId;
            NodeTypeProp.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), LocationObjectClassId, string.Empty, Int32.MinValue );

            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public string getHelpText()
        {
            return string.Empty;
        }

    }//CswNbtFieldTypeRuleLocation

}//namespace ChemSW.Nbt.MetaData

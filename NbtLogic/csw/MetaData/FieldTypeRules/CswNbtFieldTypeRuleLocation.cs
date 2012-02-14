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


            NameSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Name );
            NameSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                       CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                       CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                       CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                       CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                       CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                       CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( NameSubField, true );

            NodeIdSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field1_FK, CswNbtSubField.SubFieldName.NodeID,true );
            NodeIdSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                         CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( NodeIdSubField );

            RowSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field2, CswNbtSubField.SubFieldName.Row );
            RowSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                      CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                      CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                      CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                      CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                      CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( RowSubField );

            ColumnSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field3, CswNbtSubField.SubFieldName.Column );
            ColumnSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                         CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                         CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                         CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( ColumnSubField );

            PathSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field4, CswNbtSubField.SubFieldName.Path );
            PathSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                       CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                       CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( PathSubField );


            BarcodeSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field5, CswNbtSubField.SubFieldName.Barcode );
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

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            // Enforce only one Location property per nodetype
            foreach( CswNbtMetaDataNodeTypeProp OtherNodeTypeProp in _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProps( NodeTypeProp.NodeTypeId ) )
            {
                if( OtherNodeTypeProp != NodeTypeProp &&
                    NodeTypeProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Location )
                {
                    throw ( new CswDniException( ErrorType.Warning, "Nodetype already has a location", "Unable to add location node type property because the nodetype (" + NodeTypeProp.NodeTypeId.ToString() + ") already has a location" ) );
                }
            }

            // Locations have fixed fk relationship fields:
            Int32 LocationObjectClassId = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass ).ObjectClassId;
            NodeTypeProp.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), LocationObjectClassId, string.Empty, Int32.MinValue );

            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleLocation

}//namespace ChemSW.Nbt.MetaData

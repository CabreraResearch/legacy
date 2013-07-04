using System;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleNodeTypeSelect : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleNodeTypeSelect( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );


            SelectedNodeTypeIdsSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.NodeType );
            SelectedNodeTypeIdsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            SelectedNodeTypeIdsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SelectedNodeTypeIdsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( SelectedNodeTypeIdsSubField );

        }//ctor

        public CswNbtSubField SelectedNodeTypeIdsSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }



        private string _FilterTableAlias = "jnp.";

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];
            if( CswNbtSubField == null )
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured View", "CswNbtFieldTypeRuleDefaultImpl.renderViewPropFilter() could not find SubField '" + CswNbtViewPropertyFilterIn.SubfieldName + "' in field type '" + ( (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent ).FieldType.ToString() + "' for view '" + CswNbtViewPropertyFilterIn.View.ViewName + "'" );

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            string ReturnVal = "";
            string FullColumn = _FilterTableAlias + CswNbtSubField.Column.ToString();

            if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Contains )
            {
                // BZ 7938
                // We store the nodetypes by ID, but users will search by name.  So we have to decode.
                Collection<CswNbtMetaDataNodeType> MatchingNodeTypes = new Collection<CswNbtMetaDataNodeType>();
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypes() )
                {
                    if( NodeType.NodeTypeName.ToLower().IndexOf( CswNbtViewPropertyFilterIn.Value.ToLower() ) > -1 )
                    {
                        MatchingNodeTypes.Add( NodeType );
                    }
                }
                if( MatchingNodeTypes.Count > 0 )
                {
                    ReturnVal = "(";
                    bool first = true;
                    foreach( CswNbtMetaDataNodeType NodeType in MatchingNodeTypes )
                    {
                        if( !first )
                            ReturnVal += " or ";
                        ReturnVal += "'" + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + "' || " + FullColumn + " || '" + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + "' like '%" + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + NodeType.FirstVersionNodeTypeId.ToString() + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + "%'";
                        first = false;
                    }
                    ReturnVal += ")";
                }
                else
                {
                    // We didn't find a match.  This is better than nothing.
                    ReturnVal = "'" + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + "' || " + FullColumn + " || '" + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + "' like '%" + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + CswNbtViewPropertyFilterIn.Value + CswNbtNodePropNodeTypeSelect.delimiter.ToString() + "%'";
                }
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotNull )
            {
                ReturnVal = FullColumn + " is not null";
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Null )
            {
                ReturnVal = FullColumn + " is null";
            }
            else
            {
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for NodeTypeSelect fields" ) );
                //break;
            }

            return ( ReturnVal );

        }//renderViewPropFilter()

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

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string SelectMode = CswEnumNbtPropertyAttributeName.SelectMode;
            public const string IsFK = CswEnumNbtPropertyAttributeName.IsFK;
            public const string FKType = CswEnumNbtPropertyAttributeName.FKType;
            public const string ConstrainToObjectClass = CswEnumNbtPropertyAttributeName.ConstrainToObjectClass;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.NodeTypeSelect );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.NodeTypeSelect,
                    Name = AttributeName.SelectMode,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Multi
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.NodeTypeSelect,
                    Name = AttributeName.IsFK,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Isfk
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.NodeTypeSelect,
                    Name = AttributeName.FKType,
                    AttributeFieldType = CswEnumNbtFieldType.Text,
                    Column = CswEnumNbtPropertyAttributeColumn.Fktype
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.NodeTypeSelect,
                    Name = AttributeName.ConstrainToObjectClass,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Fkvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.NodeTypeSelect,
                Name = AttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.NodeTypeSelect
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

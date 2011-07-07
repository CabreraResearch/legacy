using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleNodeTypeSelect : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleNodeTypeSelect( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );


            SelectedNodeTypeIdsSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.NodeType );
            SelectedNodeTypeIdsSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                                      CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                                      CswNbtPropFilterSql.PropertyFilterMode.Null;
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
				throw new CswDniException( ErrorType.Error, "Misconfigured View", "CswNbtFieldTypeRuleDefaultImpl.renderViewPropFilter() could not find SubField '" + CswNbtViewPropertyFilterIn.SubfieldName + "' in field type '" + ( (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent ).FieldType.FieldType.ToString() + "' for view '" + CswNbtViewPropertyFilterIn.View.ViewName + "'" );

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
				throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            string ReturnVal = "";
            string FullColumn = _FilterTableAlias + CswNbtSubField.Column.ToString();
            switch( CswNbtViewPropertyFilterIn.FilterMode )
            {
                case CswNbtPropFilterSql.PropertyFilterMode.Contains:
                    // BZ 7938
                    // We store the nodetypes by ID, but users will search by name.  So we have to decode.
                    Collection<CswNbtMetaDataNodeType> MatchingNodeTypes = new Collection<CswNbtMetaDataNodeType>();
                    foreach( CswNbtMetaDataNodeType NodeType in _CswNbtFieldResources.CswNbtResources.MetaData.NodeTypes )
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
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.NotNull:
                    ReturnVal = FullColumn + " is not null";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.Null:
                    ReturnVal = FullColumn + " is null";
                    break;
                default:
                    throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for NodeTypeSelect fields" ) );
                    break;
            }

            return ( ReturnVal );

        }//renderViewPropFilter()

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

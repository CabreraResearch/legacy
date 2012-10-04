using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleUserSelect : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleUserSelect( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );


            SelectedUserIdsSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.NodeID );
            SelectedUserIdsSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            SelectedUserIdsSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            SelectedUserIdsSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            SubFields.add( SelectedUserIdsSubField );

        }//ctor

        public CswNbtSubField SelectedUserIdsSubField;

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
                throw new CswDniException( ErrorType.Error, "Misconfigured View", "CswNbtFieldTypeRuleDefaultImpl.renderViewPropFilter() could not find SubField '" + CswNbtViewPropertyFilterIn.SubfieldName + "' in field type '" + ( (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent ).FieldType.ToString() + "' for view '" + CswNbtViewPropertyFilterIn.View.ViewName + "'" );

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            string ReturnVal = "";
            string FullColumn = _FilterTableAlias + CswNbtSubField.Column.ToString();

            if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Contains )
            {
                // BZ 7938
                // We store the users by ID, but search by name.  So we have to decode.
                Collection<CswPrimaryKey> MatchingUserKeys = new Collection<CswPrimaryKey>();
                //ICswNbtTree UsersTree = _CswNbtFieldResources.CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass );
                //for( Int32 u = 0; u < UsersTree.getChildNodeCount(); u++ )
                //{
                //    UsersTree.goToNthChild( u );
                CswNbtMetaDataObjectClass UserOC = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
                foreach( CswNbtNode UserNode in UserOC.getNodes( false, false ) )
                {
                    string UserNodeName = UserNode.NodeName; //UsersTree.getNodeNameForCurrentPosition();
                    if( UserNodeName.ToLower().IndexOf( CswNbtViewPropertyFilterIn.Value ) > -1 )
                        MatchingUserKeys.Add( UserNode.NodeId ); //UsersTree.getNodeIdForCurrentPosition() );

                    //UsersTree.goToParentNode();
                }
                if( MatchingUserKeys.Count > 0 )
                {
                    ReturnVal = "(";
                    bool first = true;
                    foreach( CswPrimaryKey UserKey in MatchingUserKeys )
                    {
                        if( !first )
                            ReturnVal += " or ";
                        ReturnVal += "'" + CswNbtNodePropUserSelect.delimiter.ToString() + "' || " + FullColumn + " || '" + CswNbtNodePropUserSelect.delimiter.ToString() + "' like '%" + CswNbtNodePropUserSelect.delimiter.ToString() + UserKey.PrimaryKey.ToString() + CswNbtNodePropUserSelect.delimiter.ToString() + "%'";
                        first = false;
                    }
                    ReturnVal += ")";
                }
                else
                {
                    // We didn't find a match.  This is better than nothing.
                    ReturnVal = "'" + CswNbtNodePropUserSelect.delimiter.ToString() + "' || " + FullColumn + " || '" + CswNbtNodePropUserSelect.delimiter.ToString() + "' like '%" + CswNbtNodePropUserSelect.delimiter.ToString() + CswNbtViewPropertyFilterIn.Value + CswNbtNodePropUserSelect.delimiter.ToString() + "%'";
                }
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull )
            {
                ReturnVal = FullColumn + " is not null";
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
            {
                ReturnVal = FullColumn + " is null";
            }
            else
            {
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for UserSelect fields" ) );
                //break;
            }

            return ( ReturnVal );

        }//renderViewPropFilter()

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
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

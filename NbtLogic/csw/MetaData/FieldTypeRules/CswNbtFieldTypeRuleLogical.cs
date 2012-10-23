using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleLogical : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleLogical( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CheckedSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Checked );
            CheckedSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            CheckedSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            SubFields.add( CheckedSubField );

        }//ctor

        public CswNbtSubField CheckedSubField;

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

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            string ValueColumn = _FilterTableAlias + CswNbtSubField.Column.ToString();
            string ReturnVal = "";

            if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
            {
                if( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                {
                    ReturnVal = ValueColumn + " = '1' ";
                }
                else if( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
                {
                    ReturnVal = ValueColumn + " = '0' ";
                }
                else
                {
                    ReturnVal = ValueColumn + " is null";
                }
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
            {
                if( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                {
                    ReturnVal = "(" + ValueColumn + " = '0' or " + ValueColumn + " is null) ";
                }
                else if( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
                {
                    ReturnVal = "(" + ValueColumn + " = '1' or " + ValueColumn + " is null) ";
                }
                else
                {
                    ReturnVal = "(" + ValueColumn + " = '1' or " + ValueColumn + " = '0') ";
                }
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtFieldTypeRuleLogical.renderViewPropFilter()" );
            }

            return ( ReturnVal );
        }

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
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
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

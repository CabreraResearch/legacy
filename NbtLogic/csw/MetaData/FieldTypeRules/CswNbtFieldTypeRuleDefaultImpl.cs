using System;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleDefaultImpl
    {
        private CswNbtFieldResources _CswNbtFieldResources = null;
        private CswNbtSubFieldColl _SubFields;

        public CswNbtFieldTypeRuleDefaultImpl( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _SubFields = new CswNbtSubFieldColl();      // this should be filled in by the parent class
        }//ctor

        public CswNbtSubFieldColl SubFields { get { return ( _SubFields ); } }
        public bool SearchAllowed { get { return ( true ); } }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            doSetFk( inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public string renderViewPropFilter( ICswNbtUser RunAsUser,CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string,string> ParameterCollection, int FilterNumber,  bool UseNumericHack = false )
        {

            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];
            if( CswNbtSubField == null )
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured View", "CswNbtFieldTypeRuleDefaultImpl.renderViewPropFilter() could not find SubField '" + CswNbtViewPropertyFilterIn.SubfieldName + "' in field type '" + ( (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent ).FieldType.ToString() + "' for view '" + CswNbtViewPropertyFilterIn.View.ViewName + "'" );

            return ( _CswNbtFieldResources.CswNbtPropFilterSql.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, CswNbtSubField, ParameterCollection, FilterNumber, UseNumericHack ) );

        }//makeWhereClause()


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            // Default implementation
            return FilterMode.ToString();
        }

        // This is used by CswNbtNodeProp for unique value enforcement
        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false, CswNbtSubField SubField = null )
        {
            if( SubField == null )
            {
                SubField = SubFields.Default;
            }
            string StringValueToCheck = PropertyValueToCheck.GetSubFieldValue( SubField );
            CswEnumNbtFilterMode FilterMode;
            //case 27670 - in order to reserve the right for compound unique props to be empty, it has to be explicitly stated when creating the ForCompundUnique view
            if( EnforceNullEntries && String.IsNullOrEmpty( StringValueToCheck ) )
            {
                FilterMode = CswEnumNbtFilterMode.Null;
            }
            else
            {
                FilterMode = CswEnumNbtFilterMode.Equals;
            }

            View.AddViewPropertyFilter( UniqueValueViewProperty, SubField.Name, FilterMode, StringValueToCheck.Trim(), false );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
        }


    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

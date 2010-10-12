using System;
using System.Collections.Generic;
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

    public class CswNbtFieldTypeRuleDefaultImpl
    {
        private CswNbtFieldResources _CswNbtFieldResources = null;
        private CswNbtSubFieldColl _SubFields;

        public CswNbtFieldTypeRuleDefaultImpl( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _SubFields = new CswNbtSubFieldColl();      // this should be filled in by the parent class
        }//ctor

        public CswNbtSubFieldColl SubFields { get { return ( _SubFields ); } }
        public bool SearchAllowed { get { return ( true ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtSubFieldColl SubFields, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            return renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, false );
        }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtSubFieldColl SubFields, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, bool UseNumericHack )
        {

            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];
            if( CswNbtSubField == null )
                throw new CswDniException( "Misconfigured View", "CswNbtFieldTypeRuleDefaultImpl.renderViewPropFilter() could not find SubField '" + CswNbtViewPropertyFilterIn.SubfieldName + "' in field type '" + ( (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent ).FieldType.FieldType.ToString() + "' for view '" + CswNbtViewPropertyFilterIn.View.ViewName + "'" );

            return ( _CswNbtFieldResources.CswNbtPropFilterSql.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, CswNbtSubField, UseNumericHack ) );

        }//makeWhereClause()


        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            // Default implementation
            return FilterMode.ToString();
        }

        // This is used by CswNbtNodeProp for unique value enforcement
        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck )
        {
            CswNbtPropFilterSql.PropertyFilterMode FilterMode;
            string StringValueToCheck = PropertyValueToCheck.GetPropRowValue( SubFields.Default.Column );
            if( StringValueToCheck == string.Empty )
                FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Null;
            else
                FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Equals;

            CswNbtViewPropertyFilter UniqueValPropertyFilter = View.AddViewPropertyFilter( UniqueValueViewProperty, SubFields.Default.Name, FilterMode, StringValueToCheck, false );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
        }


    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData

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

    public class CswNbtFieldTypeRuleRelationship : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;
        private ICswNbtMetaDataProp _MetaDataProp = null;

        public CswNbtFieldTypeRuleRelationship( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );
            _MetaDataProp = MetaDataProp;

            NodeIDSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1_FK, CswNbtSubField.SubFieldName.NodeID );
            NodeIDSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                         CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( NodeIDSubField );

            NameSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Name );
            NameSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                       CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                       CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                       CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                       CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                       CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                       CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( NameSubField, true );
        }//ctor

        public CswNbtSubField NodeIDSubField;
        public CswNbtSubField NameSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return (_CswNbtFieldTypeRuleDefault.SearchAllowed); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            CswNbtSubField.SubFieldName OldSubfieldName = CswNbtViewPropertyFilterIn.SubfieldName;
            CswNbtPropFilterSql.PropertyFilterMode OldFilterMode = CswNbtViewPropertyFilterIn.FilterMode;
            string OldValue = CswNbtViewPropertyFilterIn.Value;

            // BZ 8558
            if( OldSubfieldName == NameSubField.Name && OldValue.ToLower() == "me" ) 
            {
                CswNbtViewProperty Prop = (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent;
                ICswNbtMetaDataProp MetaDataProp = null;
                if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                    MetaDataProp = Prop.NodeTypeProp;
                else if( Prop.Type == CswNbtViewProperty.CswNbtPropType.ObjectClassPropId )
                    MetaDataProp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( Prop.ObjectClassPropId );

                if( MetaDataProp != null && MetaDataProp.IsUserRelationship() )
                {
                    if( CswNbtViewPropertyFilterIn.Value.ToLower() == "me" )
                    {
                        CswNbtViewPropertyFilterIn.SubfieldName = NodeIDSubField.Name;
                        CswNbtViewPropertyFilterIn.FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Equals;
                        CswNbtViewPropertyFilterIn.Value = RunAsUser.UserId.PrimaryKey.ToString();
                    }
                }
            }
            string ret = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, false );

            CswNbtViewPropertyFilterIn.SubfieldName = OldSubfieldName;
            CswNbtViewPropertyFilterIn.FilterMode = OldFilterMode;
            CswNbtViewPropertyFilterIn.Value = OldValue;

            return ret;
        }

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        public void afterCreateNodeTypeProp(  CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleRelationship

}//namespace ChemSW.Nbt.MetaData

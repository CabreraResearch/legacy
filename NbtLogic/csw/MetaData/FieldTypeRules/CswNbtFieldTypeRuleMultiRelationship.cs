//using System;
//using System.Collections.ObjectModel;
//using ChemSW.Nbt.PropTypes;
//using ChemSW.Nbt.Security;

//namespace ChemSW.Nbt.MetaData.FieldTypeRules
//{

//    public class CswNbtFieldTypeRuleMultiRelationship : ICswNbtFieldTypeRule
//    {

//        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
//        private CswNbtFieldResources _CswNbtFieldResources = null;

//        public CswNbtFieldTypeRuleMultiRelationship( CswNbtFieldResources CswNbtFieldResources )
//        {
//            _CswNbtFieldResources = CswNbtFieldResources;
//            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

//            //SubFields.add( CswEnumNbtPropColumn.Gestalt, "Permissions" ); 
//            //SubFields[ "Permissions" ].SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains;
//        }//ctor

//        //public CswNbtSubField SubField;

//        public CswNbtSubFieldColl SubFields
//        {
//            get
//            {
//                return ( _CswNbtFieldTypeRuleDefault.SubFields );
//            }//get
//        }


//        public bool SearchAllowed { get { return ( false ); } }

//        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
//        {
//            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn ) );
//        }//makeWhereClause()

//        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
//        {
//            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
//        }

//        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
//        {
//            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
//        }

//        public void onSetFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
//        {
//            _CswNbtFieldTypeRuleDefault.onSetFk( MetaDataProp, DesignNTPNode );
//        }

        
//        public Collection<CswNbtFieldTypeAttribute> getAttributes()
//        {
//            throw new NotImplementedException();
//        }

//        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
//        {
//            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
//        }

//    }//ICswNbtFieldTypeRule

//}//namespace ChemSW.Nbt.MetaData

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleMol : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Mol = CswEnumNbtSubFieldName.Mol;
        }


        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleMol( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            MolSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.ClobData, SubFieldName.Mol );
            // BZ 8638
            //MolSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull |
            //                          CswEnumNbtFilterMode.Null;
            MolSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            MolSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( MolSubField );
        }//ctor

        public CswNbtSubField MolSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }


        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber )
        {
            return ( string.Empty );
        }//makeWhereClause()


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( DesignNTPNode );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            //public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            return _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.MOL ); 
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public string getHelpText()
        {
            return string.Empty;
        }

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode ) { }

    }//CswNbtFieldTypeRuleMol

}//namespace ChemSW.Nbt.MetaData

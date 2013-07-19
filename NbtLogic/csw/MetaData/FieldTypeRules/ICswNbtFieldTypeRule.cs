using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public interface ICswNbtFieldTypeRule
    {

        string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilter );
        CswNbtSubFieldColl SubFields { get; }
        bool SearchAllowed { get; }
        string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode );
        void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false );
        void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp );
        //void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue );
        void onSetFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtObjClassDesignNodeTypeProp DesignNTPNode );
        Collection<CswNbtFieldTypeAttribute> getAttributes();
    }//ICswNbtFieldTypeRule

    public class ICswNbtFieldTypeRuleSubFieldName
    {
    }

    public class ICswNbtFieldTypeRuleAttributeName
    {
        public const string AuditLevel = CswEnumNbtPropertyAttributeName.AuditLevel;
        public const string CompoundUnique = CswEnumNbtPropertyAttributeName.CompoundUnique;
        public const string DisplayConditionFilter = CswEnumNbtPropertyAttributeName.DisplayConditionFilter;
        public const string DisplayConditionProperty = CswEnumNbtPropertyAttributeName.DisplayConditionProperty;
        public const string DisplayConditionSubfield = CswEnumNbtPropertyAttributeName.DisplayConditionSubfield;
        public const string DisplayConditionValue = CswEnumNbtPropertyAttributeName.DisplayConditionValue;
        public const string FieldType = CswEnumNbtPropertyAttributeName.FieldType;
        public const string HelpText = CswEnumNbtPropertyAttributeName.HelpText;
        public const string NodeTypeValue = CswEnumNbtPropertyAttributeName.NodeTypeValue;
        public const string ObjectClassPropName = CswEnumNbtPropertyAttributeName.ObjectClassPropName;
        public const string PropName = CswEnumNbtPropertyAttributeName.PropName;
        public const string ReadOnly = CswEnumNbtPropertyAttributeName.ReadOnly;
        public const string Required = CswEnumNbtPropertyAttributeName.Required;
        public const string ServerManaged = CswEnumNbtPropertyAttributeName.ServerManaged;
        public const string Unique = CswEnumNbtPropertyAttributeName.Unique;
        public const string UseNumbering = CswEnumNbtPropertyAttributeName.UseNumbering;
    }

}//namespace ChemSW.Nbt.MetaData

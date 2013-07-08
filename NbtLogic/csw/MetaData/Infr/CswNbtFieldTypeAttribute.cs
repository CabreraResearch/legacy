using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeAttribute
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtFieldTypeAttribute( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        public CswEnumNbtFieldType OwnerFieldType;
        public CswEnumNbtFieldType AttributeFieldType;
        public CswEnumNbtPropertyAttributeName Name;
        public CswEnumNbtPropertyAttributeColumn Column;

        private CswEnumNbtSubFieldName _SubFieldName;
        public CswEnumNbtSubFieldName SubFieldName
        {
            get
            {
                if( null == _SubFieldName && AttributeFieldType != CswNbtResources.UnknownEnum )
                {
                    ICswNbtFieldTypeRule Rule = _CswNbtResources.MetaData.getFieldTypeRule( AttributeFieldType );
                    if( null != Rule && null != Rule.SubFields && null != Rule.SubFields.Default )
                    {
                        _SubFieldName = Rule.SubFields.Default.Name;
                    }
                }
                return _SubFieldName;
            }
            set { _SubFieldName = value; }
        }

        public string Value;

    } // class CswNbtFieldTypeAttribute

}//namespace ChemSW.Nbt.MetaData.FieldTypeRules

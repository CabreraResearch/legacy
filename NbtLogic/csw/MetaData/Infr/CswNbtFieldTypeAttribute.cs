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
        public CswEnumNbtFieldType OwnerFieldType;
        public CswEnumNbtFieldType AttributeFieldType;
        public CswEnumNbtPropertyAttributeName Name;
        public CswEnumNbtPropertyAttributeColumn Column;
        public CswEnumNbtSubFieldName SubFieldName;
        public string Value;

    } // class CswNbtFieldTypeAttribute

}//namespace ChemSW.Nbt.MetaData.FieldTypeRules

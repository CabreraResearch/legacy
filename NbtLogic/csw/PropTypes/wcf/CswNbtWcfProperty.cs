using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Csw.WebSvc;

namespace ChemSW.Nbt.PropTypes
{
    [DataContract]
    public class CswNbtWcfProperty
    {
        [DataMember]
        public string PropId;

        [DataMember]
        public string PropName;
        
        [DataMember]
        public string OriginalPropName;

        [DataMember]
        public CswAjaxDictionary<string> values = new CswAjaxDictionary<string>();
    }


}//namespace ChemSW.Nbt.PropTypes

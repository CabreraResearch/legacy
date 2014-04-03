using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Csw.WebSvc;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    [DataContract]
    public class CswNbtWcfPropertyVerbose
    {
        [DataMember]
        public bool canoverride;
        [DataMember]
        public bool copyable;
        [DataMember]
        public Int32 displaycol;
        [DataMember]
        public Int32 displayrow;
        [DataMember]
        public string fieldtype;
        [DataMember]
        public string gestalt;
        [DataMember]
        public string helptext;
        [DataMember]
        public bool highlight;
        [DataMember]
        public string id;
        [DataMember( Name = "readonly" )]
        public bool isreadonly;
        [DataMember]
        public bool issaveprop;
        [DataMember]
        public string name;
        [DataMember]
        public string ocpname;
        [DataMember]
        public string propnodeid;
        [DataMember]
        public bool required;
        [DataMember]
        public bool showpropertyname;
        [DataMember]
        public string tabgroup;

        [DataMember]
        public bool hassubprops;
        [DataMember]
        public Collection<CswNbtWcfPropertyVerbose> subprops = new Collection<CswNbtWcfPropertyVerbose>();

        [DataMember]
        public CswAjaxDictionary<string> values = new CswAjaxDictionary<string>();

        // Backwards compatibility
        public JProperty toJson()
        {
            JObject PropObj = new JObject();
            JProperty ret = new JProperty( "prop_" + id, PropObj );
            PropObj["id"] = id;
            PropObj["propnodeid"] = propnodeid;
            PropObj["name"] = name;
            PropObj["helptext"] = helptext;
            PropObj["fieldtype"] = fieldtype;
            PropObj["ocpname"] = ocpname;
            PropObj["displayrow"] = displayrow;
            PropObj["displaycol"] = displaycol;
            PropObj["tabgroup"] = tabgroup;
            PropObj["required"] = required;
            PropObj["copyable"] = copyable;
            PropObj["showpropertyname"] = showpropertyname;
            PropObj["readonly"] = isreadonly;
            PropObj["canoverride"] = canoverride;
            PropObj["gestalt"] = gestalt;
            PropObj["highlight"] = highlight;

            foreach( string key in values.Keys )
            {
                PropObj["values"] = new JObject();
                PropObj["values"][key] = values[key];
            }

            if( hassubprops )
            {
                PropObj["subprops"] = new JObject();
                foreach( CswNbtWcfPropertyVerbose sub in subprops )
                {
                    ( (JObject) PropObj["subprops"] ).Add( sub.toJson() );
                }
            }
            return ret;
        } // toJson()

    } // class CswNbtWcfPropertyVerbose


}//namespace ChemSW.Nbt.PropTypes

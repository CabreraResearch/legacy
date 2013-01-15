using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Grid.ExtJs
{
    [DataContract]
    public class CswNbtGridExtJsField
    {
        /// <summary>
        /// Name for the field (matches column definition's dataIndex)
        /// </summary>
        public CswNbtGridExtJsDataIndex dataIndex;

        /// <summary>
        /// Data type
        /// string, date, number
        /// </summary>
        [DataMember]
        public string type = "string";

        [DataMember]
        public string name = "";

        public JObject ToJson()
        {
            JObject Jfield = new JObject();
            Jfield["name"] = dataIndex.ToString();
            if( type != string.Empty )
            {
                Jfield["type"] = type;
            }
            Jfield["useNull"] = true;  // case 26817
            return Jfield;
        } // ToJson()

    } // class CswNbtGridExtJsField
} // namespace ChemSW.Nbt.Grid.ExtJs

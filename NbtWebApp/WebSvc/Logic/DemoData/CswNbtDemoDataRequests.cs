using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtDemoDataRequests 
    {
        [DataContract]
        public class CswDemoNodesGridRequest
        {
            [DataMember] 
            public List<string> NodeIds;

        }//class: DemoNodesGridRequest

        [DataContract]
        public class CswUpdateDemoNodesRequest
        {
            [DataMember]
            public List<string> view_ids_convert_to_non_demo;
            [DataMember]
            public List<string> node_ids_convert_to_non_demo;
            [DataMember]
            public List<string> node_ids_delete;
            [DataMember]
            public List<string> view_ids_delete;

        }//class: CswUpdateDemoNodesRequest

    }//CswNbtDemoDataRequests




} // namespace ChemSW.Nbt.WebServices

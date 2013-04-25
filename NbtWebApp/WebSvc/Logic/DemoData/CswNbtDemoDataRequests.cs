using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Grid.ExtJs;
using ChemSW.MtSched.Core;
using NbtWebApp.WebSvc.Returns;

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
            public List<string> node_ids_remove;
            [DataMember]
            public List<string> view_ids_remove;

        }//class: CswUpdateDemoNodesRequest

    }//CswNbtDemoDataRequests




} // namespace ChemSW.Nbt.WebServices

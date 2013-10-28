using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Actions.Explorer
{
    [DataContract]
    public class CswNbtExplorerRequest
    {
        [DataMember]
        public string NodeId { get; set; }

        [DataMember]
        public int Depth { get; set; }
    }

    [DataContract]
    public class CswNbtExplorerReturn: CswWebSvcReturn
    {
        public CswNbtExplorerReturn()
        {
            Data = new CswNbtArborGraph();
        }

        [DataMember]
        public CswNbtArborGraph Data;
    }

    [DataContract]
    public class CswNbtArborGraph: CswWebSvcReturn
    {
        [DataMember]
        public Dictionary<string, Collection<string>> Graph = new Dictionary<string, Collection<string>>();

        [DataMember]
        public Collection<CswNbtArborNode> Nodes = new Collection<CswNbtArborNode>();

        [DataMember]
        public Collection<CswNbtArborEdge> Edges = new Collection<CswNbtArborEdge>();
    }


}
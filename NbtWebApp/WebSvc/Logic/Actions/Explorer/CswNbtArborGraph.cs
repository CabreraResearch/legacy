using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Actions.Explorer
{

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
        public Collection<CswNbtArborNode> Nodes = new Collection<CswNbtArborNode>();

        [DataMember]
        public Collection<CswNbtArborEdge> Edges = new Collection<CswNbtArborEdge>();
    }
}
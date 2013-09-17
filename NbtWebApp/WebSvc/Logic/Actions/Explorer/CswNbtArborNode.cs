using System.Runtime.Serialization;

namespace NbtWebApp.Actions.Explorer
{
    [DataContract]
    public class CswNbtArborNode
    {
        //private CswPrimaryKey _NodeId;
        //[DataMember( Name = "NodeId" )]
        //public string NodeIdStr
        //{
        //    get { return _NodeId.ToString(); }
        //    set { _NodeId = CswConvert.ToPrimaryKey( "nodes_" + value ); }
        //}
        //
        //public CswPrimaryKey NodeId { get { return _NodeId; } }

        [DataMember(Name = "NodeId")]
        public string NodeIdStr { get; set; }

        [DataMember]
        public CswNbtArborNodeData Data { get; set; }

        [DataContract]
        public class CswNbtArborNodeData
        {
            [DataMember]
            public double Mass;

            [DataMember]
            public string Icon;

            [DataMember]
            public string Label { get; set; }
        }
    }
}
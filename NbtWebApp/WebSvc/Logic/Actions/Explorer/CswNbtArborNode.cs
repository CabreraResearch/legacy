using System.Runtime.Serialization;
using ChemSW.Core;

namespace NbtWebApp.Actions.Explorer
{
    [DataContract]
    public class CswNbtArborNode
    {
        private CswPrimaryKey _NodeId;
        [DataMember( Name = "NodeId" )]
        public string NodeIdStr
        {
            get { return _NodeId.ToString(); }
            set { _NodeId = CswConvert.ToPrimaryKey( value ); }
        }

        public CswPrimaryKey NodeId { get { return _NodeId; } }

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
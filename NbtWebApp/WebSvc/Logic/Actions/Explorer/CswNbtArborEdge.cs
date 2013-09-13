
using System.Runtime.Serialization;
using ChemSW.Core;

namespace NbtWebApp.Actions.Explorer
{
    [DataContract]
    public class CswNbtArborEdge
    {
        private CswPrimaryKey _OwnerNodeId;
        [DataMember( Name = "OwnerNodeId" )]
        public string OwnerNodeIdStr
        {
            get { return _OwnerNodeId.ToString(); }
            set { _OwnerNodeId = CswConvert.ToPrimaryKey( value ); }
        }

        private CswPrimaryKey _TargetNodeId;
        [DataMember( Name = "TargetNodeId" )]
        public string TargetNodeIdStr
        {
            get { return _TargetNodeId.ToString(); }
            set { _TargetNodeId = CswConvert.ToPrimaryKey( value ); }
        }

        public CswPrimaryKey TargetNodeId { get { return _TargetNodeId; } }
        public CswPrimaryKey OwnerNodeId { get { return _OwnerNodeId; } }

        [DataMember]
        public CswNbtArborEdgeData Data { get; set; }

        [DataContract]
        public class CswNbtArborEdgeData
        {
            [DataMember]
            public int Length;
        }

    }
}
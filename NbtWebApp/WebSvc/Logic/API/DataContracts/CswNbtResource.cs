using System;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    [DataContract]
    public class CswNbtResource: CswNbtAPIReturn
    {
        private CswPrimaryKey _nodeId = new CswPrimaryKey( "nodes", Int32.MinValue );
        public CswPrimaryKey NodeId
        {
            get { return _nodeId; }
            set { _nodeId = value; }
        }
        [DataMember( Name = "nodeid" )]
        private int _nodeIdIntStr
        {
            get { return _nodeId.PrimaryKey; }
            set { _nodeId = new CswPrimaryKey( "nodes", value ); }
        }

        [DataMember( Name = "nodename" )]
        public string NodeName { get; set; }

        [DataMember( Name = "uri" )]
        public string URI { get; set; }

        [DataMember( Name = "nodetype" )]
        public string NodeType { get; set; }

        [DataMember( Name = "objectclass" )]
        public string ObjectClass { get; set; }

        [DataMember( Name = "propertyset" )]
        public string PropertySet { get; set; }
    }
}
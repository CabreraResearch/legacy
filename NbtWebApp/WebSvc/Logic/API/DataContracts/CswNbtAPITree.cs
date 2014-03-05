using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    [DataContract]
    public class CswNbtAPITree: CswNbtAPIReturn
    {
        [DataMember( Name = "viewname" )]
        public string ViewName { get; set; }

        [DataMember( Name = "uri" )]
        public string URI { get; set; }

        CswNbtViewId ViewId = new CswNbtViewId();
        [DataMember( Name = "viewid" )]
        private int _ViewId
        {
            get { return ViewId.get(); }
            set { ViewId = new CswNbtViewId( value ); }
        }

        [DataMember( Name = "entities" )]
        private Collection<CswNbtTreeResource> _Entities = new Collection<CswNbtTreeResource>();

        public CswNbtTreeResource Add( string Name, CswPrimaryKey NodeId, string NodeType, string ObjClass, string PropertySet, string uri )
        {
            CswNbtTreeResource addedResource = new CswNbtTreeResource()
                 {
                     NodeId = NodeId,
                     NodeName = Name,
                     NodeType = NodeType,
                     ObjectClass = ObjClass,
                     PropertySet = PropertySet,
                     URI = uri
                 };
            _Entities.Add( addedResource );
            return addedResource;
        }

        [DataContract]
        public class CswNbtTreeResource: CswNbtResource
        {
            [DataMember( Name = "children" )]
            private Collection<CswNbtTreeResource> _Children = new Collection<CswNbtTreeResource>();

            public CswNbtTreeResource Add( string Name, CswPrimaryKey NodeId, string NodeType, string ObjClass, string PropertySet, string uri )
            {
                CswNbtTreeResource addedResource = new CswNbtTreeResource()
                {
                    NodeId = NodeId,
                    NodeName = Name,
                    NodeType = NodeType,
                    ObjectClass = ObjClass,
                    PropertySet = PropertySet,
                    URI = uri
                };
                _Children.Add( addedResource );
                return addedResource;
            }
        }
    }
}
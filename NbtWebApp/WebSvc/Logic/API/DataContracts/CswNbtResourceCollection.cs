using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    [DataContract]
    public class CswNbtResourceCollection: CswNbtAPIReturn
    {
        [DataMember(Name = "entities")]
        private Collection<CswNbtResource> _Entities = new Collection<CswNbtResource>(); 

        public void Add( string Name, CswPrimaryKey NodeId, string NodeType, string ObjClass, string PropertySet, string URI )
        {
            _Entities.Add(new CswNbtResource()
                {
                    NodeId = NodeId,
                    NodeName = Name,
                    NodeType = NodeType,
                    ObjectClass = ObjClass,
                    PropertySet = PropertySet,
                    URI = URI
                });
        }

        public int Count()
        {
            return _Entities.Count;
        }

        public Collection<CswNbtResource> getEntities()
        {
            return _Entities;
        }
    }
}
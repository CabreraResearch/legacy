using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtLocationsResponseModel
    {
        private Collection<CswNbtLocationNodeModel> _Locations;
        public CswNbtLocationsResponseModel()
        {
            _Locations = new Collection<CswNbtLocationNodeModel>();
        }

        public class CswNbtLocationNodeModel
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string LocationId { get; set; }
        }

        /* DataMember names are public, so make them user-friendly */
        [DataMember]
        public Collection<CswNbtLocationNodeModel> Locations { get { return _Locations; } }

        public void Add( CswNbtLocationNodeModel NodeModel )
        {
            _Locations.Add( NodeModel );
        }

    }
}
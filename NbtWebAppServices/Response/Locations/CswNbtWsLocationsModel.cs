using System.Collections.ObjectModel;

namespace NbtWebAppServices.Response
{
    public class CswNbtWsLocationsModel
    {
        public class CswNbtWsLocationNodeModel
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string LocationId { get; set; }
        }

        public class CswNbtWsLocationListModel
        {
            public Collection<CswNbtWsLocationNodeModel> LocationsList { get; set; }
        }
    }
}
using System.Runtime.Serialization;
using ChemSW.Nbt;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    [DataContract]
    public class CswNbtAPIGrid: CswNbtResourceCollection
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
    }
}
using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWebServiceResponseLocations : CswNbtWebServiceResponseBase
    {
        public CswNbtWebServiceResponseLocations( HttpContext Context )
            : base( Context )
        {
        }

        [DataMember]
        public CswNbtLocationsDataModel Data { get; set; }

    }
}
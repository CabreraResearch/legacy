using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWcfLocationsResponse : CswNbtWcfResponseBase
    {
        public CswNbtWcfLocationsResponse( HttpContext Context )
            : base( Context )
        {
        }

        [DataMember]
        public CswNbtWcfLocationsDataModel Data { get; set; }

    }
}
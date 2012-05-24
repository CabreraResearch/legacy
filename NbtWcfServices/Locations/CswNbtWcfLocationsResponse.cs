using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWcfLocationsResponse : CswNbtWcfResponseBase
    {
        public CswNbtWcfLocationsResponse( HttpContext Context, bool IsMobile, bool AttemptRefresh = true )
            : base( Context, IsMobile, AttemptRefresh )
        {
        }

        [DataMember]
        public CswNbtWcfLocationsDataModel Data { get; set; }

    }
}
using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWcfResponse : CswNbtWcfResponseBase
    {
        public CswNbtWcfResponse( HttpContext Context, bool IsMobile, bool AttemptRefresh = true )
            : base( Context, IsMobile, AttemptRefresh )
        {
        }

        [DataMember]
        public object Data { get; set; }
    }
}
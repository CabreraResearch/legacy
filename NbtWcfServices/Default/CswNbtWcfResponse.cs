using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWcfResponse : CswNbtWcfResponseBase
    {
        public CswNbtWcfResponse( HttpContext Context )
            : base( Context )
        {
        }

        [DataMember]
        public object Data { get; set; }
    }
}
using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWebServiceResponse : CswNbtWebServiceResponseBase
    {
        public CswNbtWebServiceResponse( HttpContext Context )
            : base( Context )
        {
        }

        [DataMember]
        public object Data { get; set; }
    }
}

using System.Runtime.Serialization;

namespace NbtWebAppServices.Response
{
    public class CswNbtWcfRequest : ICswNbtWcfRequest
    {
        [DataContract]
        public class CswNbtSessionRequest
        {
            [DataMember( IsRequired = true )]
            public string Password { get; set; }
            [DataMember( IsRequired = true )]
            public string CustomerId { get; set; }
            [DataMember( IsRequired = true )]
            public string UserName { get; set; }
            [DataMember( IsRequired = true )]
            public bool IsMobile { get; set; }
        }
    }
}
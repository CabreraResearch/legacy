
using System.Runtime.Serialization;

namespace NbtWebAppServices.Response
{
    public class CswNbtWcfRequest : ICswNbtWcfRequest
    {
        [DataContract]
        public class CswNbtSessionRequest
        {
            [DataMember( IsRequired = false )]
            public string Password = "";
            [DataMember( IsRequired = false )]
            public string CustomerId = "";
            [DataMember( IsRequired = false )]
            public string UserName = "";
            [DataMember( IsRequired = false )]
            public bool IsMobile = true;
        }
    }


}
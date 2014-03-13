
using System.Net;
using System.Runtime.Serialization;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    [DataContract]
    public class CswNbtAPIReturn
    {
        public HttpStatusCode Status = HttpStatusCode.OK;
    }
}
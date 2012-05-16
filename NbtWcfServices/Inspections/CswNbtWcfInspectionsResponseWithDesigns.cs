using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWcfInspectionsResponseWithDesigns : CswNbtWcfResponseBase
    {
        public CswNbtWcfInspectionsResponseWithDesigns( HttpContext Context )
            : base( Context )
        {
            Data = new CswNbtWcfInspectionsDataModel();
        }

        [DataMember]
        public CswNbtWcfInspectionsDataModel Data { get; set; }
    }
}
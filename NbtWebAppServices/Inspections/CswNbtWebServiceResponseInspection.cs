using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWebServiceResponseInspections : CswNbtWebServiceResponseBase
    {
        public CswNbtWebServiceResponseInspections( HttpContext Context )
            : base( Context )
        {
        }

        [DataMember]
        public CswNbtInspectionsDataModel.CswNbtInspection Data { get; set; }

    }
}
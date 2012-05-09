using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWebServiceResponseInspectionsAndDesign : CswNbtWebServiceResponseBase
    {
        public CswNbtWebServiceResponseInspectionsAndDesign( HttpContext Context )
            : base( Context )
        {
        }

        [DataMember]
        public CswNbtInspectionsDataModel Data { get; set; }
    }
}
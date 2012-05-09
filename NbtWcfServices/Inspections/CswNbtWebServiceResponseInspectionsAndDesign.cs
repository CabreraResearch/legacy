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
            Data = new CswNbtInspectionsDataModel();
        }

        [DataMember]
        public CswNbtInspectionsDataModel Data { get; set; }
    }
}
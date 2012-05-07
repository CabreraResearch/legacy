using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtLocationsResponse : CswNbtWebServiceResponseNoData
    {
        public CswNbtLocationsResponse( HttpContext Context )
            : base( Context )
        {
        }

        [DataMember]
        public CswNbtWsLocationsModel.CswNbtWsLocationListModel Data { get; set; }
    }
}
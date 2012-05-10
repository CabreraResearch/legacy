using System;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Nbt;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWebServiceResponseLocations : ICswNbtWebServiceResponse
    {
        private CswNbtWebServiceResponseNoData _DefaultResponse;

        public CswNbtWebServiceResponseLocations( HttpContext Context )
        {
            _DefaultResponse = new CswNbtWebServiceResponseNoData( Context );
        }

        [DataMember]
        public CswNbtLocationsResponseModel Data { get; set; }

        [DataMember]
        public CswNbtSessionAuthenticationStatus SessionAuthenticationStatus
        {
            get { return _DefaultResponse.SessionAuthenticationStatus; }
            set { _DefaultResponse.SessionAuthenticationStatus = value; }
        }

        [DataMember]
        public CswNbtWebServiceStatus Status
        {
            get { return _DefaultResponse.Status; }
            set { _DefaultResponse.Status = value; }
        }

        [DataMember]
        public CswNbtWebServicePerformance Performance
        {
            get { return _DefaultResponse.Performance; }
            set { _DefaultResponse.Performance = value; }
        }

        public CswNbtSessionResources CswNbtSessionResources
        {
            get { return _DefaultResponse.CswNbtSessionResources; }
            set { _DefaultResponse.CswNbtSessionResources = value; }
        }

        public void finalizeResponse( CswNbtResources OtherResources = null )
        {
            _DefaultResponse.finalizeResponse( OtherResources );
        }

        public void addError( Exception Exception )
        {
            _DefaultResponse.addError( Exception );
        }
    }
}
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceLocations
    {
        #region ctor

        private CswNbtResources _CswNbtResources;
        public CswNbtWebServiceLocations( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion ctor

        [DataContract]
        public class CswNbtLocationReturn : CswWebSvcReturn
        {
            public CswNbtLocationReturn()
            {
                Data = new Collection<CswNbtSdLocations.Location>();
            }
            [DataMember]
            public Collection<CswNbtSdLocations.Location> Data;
        }

        [DataContract]
        public class CswNbtLocationRequest
        {
            [DataContract]
            public class CswNbtLocationSearch
            {
                [DataMember]
                public string ViewId;

                [DataMember]
                public string Query;
            }
        }


        #region Public

        public Collection<CswNbtSdLocations.Location> getLocationsListMobile()
        {
            CswNbtSdLocations Sd = new CswNbtSdLocations( _CswNbtResources );
            return Sd.getLocationListMobile();
        }

        public static void getLocationsListMobile( ICswResources CswResources, CswNbtLocationReturn Return, bool Request )
        {
            if( null != CswResources )
            {
                CswNbtWebServiceLocations Ws = new CswNbtWebServiceLocations( (CswNbtResources) CswResources );
                Return.Data = Ws.getLocationsListMobile();
            }
        }

        public static void getLocationsList( ICswResources CswResources, CswNbtLocationReturn Return, string Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtSdLocations Sd = new CswNbtSdLocations( CswNbtResources );
            Return.Data = Sd.GetLocationsList( Request );
        }

        public static void searchLocations( ICswResources CswResources, CswNbtLocationReturn Return, CswNbtLocationRequest.CswNbtLocationSearch Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtSdLocations Sd = new CswNbtSdLocations( CswNbtResources );
            string Query = Request.Query;
            string ViewId = Request.ViewId;

            Return.Data = Sd.searchLocations( Query, ViewId );
        }

        #endregion Public

    }
}
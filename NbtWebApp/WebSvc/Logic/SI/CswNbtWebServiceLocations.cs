using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Nbt.PropTypes;
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
            //if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SI ) )
            //{
            //throw new CswDniException( CswEnumErrorType.Error, "Cannot use this web service without the required modules.", "Attempted to load an SI dependent service" );
            //}
        }

        #endregion ctor

        [DataContract]
        public class CswNbtLocationReturn: CswWebSvcReturn
        {
            public CswNbtLocationReturn()
            {
                Data = new Collection<CswNbtSdLocations.Location>();
            }
            [DataMember]
            public Collection<CswNbtSdLocations.Location> Data;
        }


        #region Public

        public Collection<CswNbtSdLocations.Location> getLocationsList()
        {
            CswNbtSdLocations Sd = new CswNbtSdLocations(_CswNbtResources);
            return Sd.getLocationList();
        }

        public static void getLocationsList( ICswResources CswResources, CswNbtLocationReturn Return, bool Request )
        {
            if( null != CswResources )
            {
                CswNbtWebServiceLocations Ws = new CswNbtWebServiceLocations( (CswNbtResources) CswResources );
                Return.Data = Ws.getLocationsList();
            }
        }

        public static void getLocationsList2( ICswResources CswResources, CswNbtLocationReturn Return, string Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            Return.Data = CswNbtNodePropLocation.GetLocationsList( CswNbtResources, Request );
        }

        #endregion Public

    }
}
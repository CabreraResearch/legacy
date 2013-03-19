using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.WebSvc.Returns;
using NbtWebAppServices.Response;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceInspections
    {
        #region ctor

        private CswNbtResources _CswNbtResources;
        public CswNbtWebServiceInspections( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SI ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use this web service without the required modules.", "Attempted to load an SI dependent service." );
            }
        }

        #endregion ctor

        public class Dates
        {
            public string StartingDate;
            public string EndingDate;
        }

        [DataContract]
        public class CswNbtInspectionGet: CswWebSvcReturn
        {
            public CswNbtInspectionGet()
            {
                Data = new CswNbtSdInspectionsDataModels.InspectionData();
            }
            [DataMember]
            public CswNbtSdInspectionsDataModels.InspectionData Data;
        }

        [DataContract]
        public class CswNbtInspectionSet: CswWebSvcReturn
        {
            public CswNbtInspectionSet()
            {
                Data = new CswNbtSdInspectionsDataModels.InspectionUpdateData();
            }
            [DataMember]
            public CswNbtSdInspectionsDataModels.InspectionUpdateData Data;
        }

        #region Public

        public CswNbtSdInspectionsDataModels.InspectionData getInspectionsByDateRange(Dates Dates)
        {
            SystemViewName ViewName = SystemViewName.SIInspectionsbyDate;
            CswNbtSdInspections Sd = new CswNbtSdInspections( _CswNbtResources, ViewName );
            return Sd.byDateRange( Dates.StartingDate, Dates.EndingDate );
        }

        public static void getInspectionsByDateRange( ICswResources CswResources, CswNbtInspectionGet Return, Dates Request )
        {
            if( null != CswResources )
            {
                CswNbtWebServiceInspections Ws = new CswNbtWebServiceInspections( (CswNbtResources) CswResources );
                Return.Data = Ws.getInspectionsByDateRange( Request );
            }
        }

        public CswNbtSdInspectionsDataModels.InspectionData getInspectionsByUser()
        {
            SystemViewName ViewName = SystemViewName.SIInspectionsbyUser;
            CswNbtSdInspections Sd = new CswNbtSdInspections( _CswNbtResources, ViewName );
            return Sd.byUser();
        }

        public static void getInspectionsByUser( ICswResources CswResources, CswNbtInspectionGet Return, string Request )
        {
            if( null != CswResources )
            {
                CswNbtWebServiceInspections Ws = new CswNbtWebServiceInspections( (CswNbtResources) CswResources );
                Return.Data = Ws.getInspectionsByUser();
            }
        }

        public CswNbtSdInspectionsDataModels.InspectionData getInspectionsByBarcode(string Barcode)
        {
            SystemViewName ViewName = SystemViewName.SIInspectionsbyBarcode;
            CswNbtSdInspections Sd = new CswNbtSdInspections( _CswNbtResources, ViewName );
            return Sd.byBarcode( Barcode );
        }

        public static void getInspectionsByBarcode( ICswResources CswResources, CswNbtInspectionGet Return, string Request )
        {
            if( null != CswResources )
            {
                CswNbtWebServiceInspections Ws = new CswNbtWebServiceInspections( (CswNbtResources) CswResources );
                Return.Data = Ws.getInspectionsByBarcode( Request );
            }
        }

        public CswNbtSdInspectionsDataModels.InspectionData getInspectionsByLocation( string LocationName )
        {
            SystemViewName ViewName = SystemViewName.SIInspectionsbyLocation;
            CswNbtSdInspections Sd = new CswNbtSdInspections( _CswNbtResources, ViewName );
            return Sd.byLocation( LocationName );
        }

        public static void getInspectionsByLocation( ICswResources CswResources, CswNbtInspectionGet Return, string Request )
        {
            if( null != CswResources )
            {
                CswNbtWebServiceInspections Ws = new CswNbtWebServiceInspections( (CswNbtResources) CswResources );
                Return.Data = Ws.getInspectionsByLocation( Request );
            }
        }

        public CswNbtSdInspectionsDataModels.InspectionUpdateData update( Collection<CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection> Inspections )
        {
            SystemViewName ViewName = SystemViewName.SIInspectionsbyUser;
            CswNbtSdInspections Sd = new CswNbtSdInspections( _CswNbtResources, ViewName, Inspections );
            return Sd.update();
        }

        public static void update( ICswResources CswResources, CswNbtInspectionSet Return, Collection<CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection> Request )
        {
            if( null != CswResources )
            {
                CswNbtWebServiceInspections Ws = new CswNbtWebServiceInspections( (CswNbtResources) CswResources );
                Return.Data = Ws.update( Request );
            }
        }

        #endregion Public

    }
}
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebAppServices.Response;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtWebServiceInspectionsUriMethods
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebGet( UriTemplate = "byDateRange?StartingDate={StartingDate}&EndingDate={EndingDate}" )]
        [Description( "Get all Inspections whose Due Date is greater than or equals the StartingDate and less than or equals the EndingDate (optional, de" )]
        public CswNbtWebServiceResponseInspectionsAndDesign byDateRange( string StartingDate, string EndingDate )
        {
            CswNbtWebServiceInspectionsGet WebServiceInspectionsGet = new CswNbtWebServiceInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyDate );
            DateTime Start = CswConvert.ToDateTime( StartingDate );
            DateTime End = CswConvert.ToDateTime( EndingDate );
            if( Start > End )
            {
                End = DateTime.Now;
            }
            CswDateTime CswStart = WebServiceInspectionsGet.getCswDate( Start );
            CswDateTime CswEnd = WebServiceInspectionsGet.getCswDate( End );
            WebServiceInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.DatePropertyName, CswStart.ToOracleNativeDateForQuery() );
            WebServiceInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.DatePropertyName, CswEnd.ToOracleNativeDateForQuery() );

            return WebServiceInspectionsGet.finalize();
        }

        [OperationContract]
        [WebGet]
        [Description( "Get all Inspections whose Inspector is the current user." )]
        public CswNbtWebServiceResponseInspectionsAndDesign byUser()
        {
            CswNbtWebServiceInspectionsGet WebServiceInspectionsGet = new CswNbtWebServiceInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyUser );
            return WebServiceInspectionsGet.finalize();
        } // get()

        [OperationContract]
        [WebGet]
        [Description( "Get all Inspections whose Target location path contains the supplied parameter (optional)." )]
        public CswNbtWebServiceResponseInspectionsAndDesign byLocation( string LocationName )
        {
            CswNbtWebServiceInspectionsGet WebServiceInspectionsGet = new CswNbtWebServiceInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyLocation );
            WebServiceInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.LocationPropertyName, LocationName );
            return WebServiceInspectionsGet.finalize();
        } // get()

        [OperationContract]
        [WebGet]
        [Description( "Get all Inspections whose Target or Location barcode contains the supplied parameter (optional)." )]
        public CswNbtWebServiceResponseInspectionsAndDesign byBarcode( string Barcode )
        {
            CswNbtWebServiceInspectionsGet WebServiceInspectionsGet = new CswNbtWebServiceInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyBarcode );
            WebServiceInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass, CswNbtObjClassLocation.BarcodePropertyName, Barcode );
            WebServiceInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass, CswNbtObjClassInspectionTarget.BarcodePropertyName, Barcode );
            return WebServiceInspectionsGet.finalize();
        } // get()

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Submit Updated Inspections" )]
        public CswNbtWebServiceResponseInspections update( Collection<CswNbtInspectionsDataModel.CswNbtInspection> Inspections )
        {
            CswNbtWebServiceInspectionsPost WebServiceInspectionsPost = new CswNbtWebServiceInspectionsPost( _Context, Inspections );
            return WebServiceInspectionsPost.finalize();
        }
    }
}
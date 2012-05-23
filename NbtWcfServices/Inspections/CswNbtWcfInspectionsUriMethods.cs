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
    public class CswNbtWcfInspectionsUriMethods
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebGet( UriTemplate = "byDateRange?StartingDate={StartingDate}&EndingDate={EndingDate}" )]
        [Description( "Get all Inspections whose Due Date is greater than or equals the StartingDate and less than or equals the EndingDate (optional, de" )]
        public CswNbtWcfInspectionsResponseWithDesigns byDateRange( string StartingDate, string EndingDate )
        {
            CswNbtWcfInspectionsGet WcfInspectionsGet = new CswNbtWcfInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyDate );
            DateTime Start = CswConvert.ToDateTime( StartingDate );
            if( DateTime.MinValue == Start )
            {
                Start = DateTime.Now;
            }
            DateTime End = CswConvert.ToDateTime( EndingDate );
            if( DateTime.MinValue == End )
            {
                End = DateTime.Now.AddDays( 2 );
            }
            if( Start > End )
            {
                End = Start;
                End = End.AddDays( 2 );
            }
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.DatePropertyName, Start.ToShortDateString(), CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.DatePropertyName, End.ToShortDateString(), CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals );

            return WcfInspectionsGet.finalize();
        }

        [OperationContract]
        [WebGet]
        [Description( "Get all Inspections whose Inspector is the current user." )]
        public CswNbtWcfInspectionsResponseWithDesigns byUser()
        {
            CswNbtWcfInspectionsGet WcfInspectionsGet = new CswNbtWcfInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyUser );
            return WcfInspectionsGet.finalize();
        } // get()

        [OperationContract]
        [WebGet]
        [Description( "Get all Inspections whose Target location path contains the supplied parameter." )]
        public CswNbtWcfInspectionsResponseWithDesigns byLocation( string LocationName )
        {
            LocationName = LocationName ?? "null";
            CswNbtWcfInspectionsGet WcfInspectionsGet = new CswNbtWcfInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyLocation );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.LocationPropertyName, LocationName, CswNbtPropFilterSql.PropertyFilterMode.Begins );
            return WcfInspectionsGet.finalize();
        } // get()

        [OperationContract]
        [WebGet]
        [Description( "Get all Inspections whose Target or Location barcode contains the supplied parameter." )]
        public CswNbtWcfInspectionsResponseWithDesigns byBarcode( string Barcode )
        {
            Barcode = Barcode ?? "null";
            CswNbtWcfInspectionsGet WcfInspectionsGet = new CswNbtWcfInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyBarcode );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass, CswNbtObjClassLocation.BarcodePropertyName, Barcode, CswNbtPropFilterSql.PropertyFilterMode.Begins );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass, CswNbtObjClassInspectionTarget.BarcodePropertyName, Barcode, CswNbtPropFilterSql.PropertyFilterMode.Begins );
            return WcfInspectionsGet.finalize();
        } // get()

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Submit Updated Inspections" )]
        public CswNbtWcfInspectionsResponse update( Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> Inspections )
        {
            CswNbtWcfInspectionsPost WcfInspectionsPost = new CswNbtWcfInspectionsPost( _Context, Inspections );
            return WcfInspectionsPost.finalize();
        }
    }
}
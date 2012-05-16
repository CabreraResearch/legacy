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
                End = DateTime.Now.AddDays( 7 );
            }
            if( Start > End )
            {
                End = DateTime.Now.AddDays( 7 );
            }
            CswDateTime CswStart = WcfInspectionsGet.getCswDate( Start );
            CswDateTime CswEnd = WcfInspectionsGet.getCswDate( End );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.DatePropertyName, CswStart.ToOracleNativeDateForQuery(), CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.DatePropertyName, CswEnd.ToOracleNativeDateForQuery(), CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals );

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
        [Description( "Get all Inspections whose Target location path contains the supplied parameter (optional)." )]
        public CswNbtWcfInspectionsResponseWithDesigns byLocation( string LocationName )
        {
            CswNbtWcfInspectionsGet WcfInspectionsGet = new CswNbtWcfInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyLocation );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.LocationPropertyName, LocationName );
            return WcfInspectionsGet.finalize();
        } // get()

        [OperationContract]
        [WebGet]
        [Description( "Get all Inspections whose Target or Location barcode contains the supplied parameter (optional)." )]
        public CswNbtWcfInspectionsResponseWithDesigns byBarcode( string Barcode )
        {
            CswNbtWcfInspectionsGet WcfInspectionsGet = new CswNbtWcfInspectionsGet( _Context, CswNbtActSystemViews.SystemViewName.SIInspectionsbyBarcode );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass, CswNbtObjClassLocation.BarcodePropertyName, Barcode );
            WcfInspectionsGet.addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass, CswNbtObjClassInspectionTarget.BarcodePropertyName, Barcode );
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
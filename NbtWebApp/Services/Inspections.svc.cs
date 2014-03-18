﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebAppServices.Response;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Inspections
    {
        private HttpContext _Context = HttpContext.Current;
        private static CswWebSvcSessionAuthenticateData.Authentication.Request AuthRequest
        {
            get
            {
                CswWebSvcSessionAuthenticateData.Authentication.Request Ret = new CswWebSvcSessionAuthenticateData.Authentication.Request();
                Ret.RequiredModules.Add( CswEnumNbtModuleName.SI );
                return Ret;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "byDateRange?StartingDate={StartingDate}&EndingDate={EndingDate}" )]
        [Description( "Get all Inspections whose Due Date is greater than or equals the StartingDate and less than or equals the EndingDate (optional, defaults to today+2 days)" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceInspections.CswNbtInspectionGet byDateRange( string StartingDate, string EndingDate )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceInspections.CswNbtInspectionGet Ret = new CswNbtWebServiceInspections.CswNbtInspectionGet();
            CswNbtWebServiceInspections.Dates Dates = new CswNbtWebServiceInspections.Dates()
                {
                    StartingDate = StartingDate,
                    EndingDate = EndingDate
                };
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceInspections.CswNbtInspectionGet, CswNbtWebServiceInspections.Dates>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceInspections.getInspectionsByDateRange,
                ParamObj: Dates
                );

            SvcDriver.run();
            return ( Ret );
        }
        
        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all Inspections whose Inspector is the current user." )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceInspections.CswNbtInspectionGet byUser()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceInspections.CswNbtInspectionGet Ret = new CswNbtWebServiceInspections.CswNbtInspectionGet();
            
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceInspections.CswNbtInspectionGet, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceInspections.getInspectionsByUser,
                ParamObj : ""
                );

            SvcDriver.run();
            return ( Ret );
        }


        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all Inspections whose Target or Location barcode contains the supplied parameter." )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceInspections.CswNbtInspectionGet byBarcode( string Barcode )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceInspections.CswNbtInspectionGet Ret = new CswNbtWebServiceInspections.CswNbtInspectionGet();
            
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceInspections.CswNbtInspectionGet, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceInspections.getInspectionsByBarcode,
                ParamObj : Barcode
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all Inspections whose Target location path contains the supplied parameter." )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceInspections.CswNbtInspectionGet byLocation( string LocationName )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceInspections.CswNbtInspectionGet Ret = new CswNbtWebServiceInspections.CswNbtInspectionGet();
            
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceInspections.CswNbtInspectionGet, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceInspections.getInspectionsByLocation,
                ParamObj : LocationName
                );

            SvcDriver.run();
            return ( Ret );
        }


        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json )]
        [Description( "Submit Updated Inspections" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceInspections.CswNbtInspectionSet update( Collection<CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection> Inspections )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceInspections.CswNbtInspectionSet Ret = new CswNbtWebServiceInspections.CswNbtInspectionSet();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceInspections.CswNbtInspectionSet, Collection<CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection>>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceInspections.update,
                ParamObj: Inspections
                );

            SvcDriver.run();
            return ( Ret );
        }

    }
}

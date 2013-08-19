using System;
using ChemSW.Nbt;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebAppServices.Response;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Import
    {
        private HttpContext _Context = HttpContext.Current;
        private static CswWebSvcSessionAuthenticateData.Authentication.Request AuthRequest
        {
            get
            {
                CswWebSvcSessionAuthenticateData.Authentication.Request Ret = new CswWebSvcSessionAuthenticateData.Authentication.Request();
                return Ret;
            }
        }

        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get possible import definitions" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportDefsReturn getImportDefs()
        {
            CswNbtImportWcf.ImportDefsReturn ret = new CswNbtImportWcf.ImportDefsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportDefsReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportDefs,
                ParamObj: null
                );

            SvcDriver.run();
            return ( ret );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get existing import jobs" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportJobsReturn getImportJobs()
        {
            CswNbtImportWcf.ImportJobsReturn ret = new CswNbtImportWcf.ImportJobsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportJobsReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportJobs,
                ParamObj: null
                );

            SvcDriver.run();
            return ( ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "uploadImportData?defname={ImportDefName}&overwrite={Overwrite}" )]
        [Description( "Upload Import Data" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportDataReturn uploadImportData( string ImportDefName, bool Overwrite )
        {
            CswNbtImportWcf.ImportDataReturn ret = new CswNbtImportWcf.ImportDataReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                CswNbtImportWcf.ImportDataParams parms = new CswNbtImportWcf.ImportDataParams();
                parms.PostedFile = _Context.Request.Files[0];
                parms.ImportDefName = ImportDefName;
                parms.Overwrite = Overwrite;

                var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportDataReturn, CswNbtImportWcf.ImportDataParams>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: ret,
                    WebSvcMethodPtr: CswNbtWebServiceImport.uploadImportData,
                    ParamObj: parms
                    );

                SvcDriver.run();
            }

            return ret;
        }



        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get current status of imports" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportStatusReturn getImportStatus()
        {
            CswNbtImportWcf.ImportStatusReturn ret = new CswNbtImportWcf.ImportStatusReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportStatusReturn, CswNbtImportWcf.ImportStatusRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportStatus,
                ParamObj: null
                );

            SvcDriver.run();

            return ret;
        } // getImportStatus()

    }
}

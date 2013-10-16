using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

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
                CswNbtImportWcf.ImportFileParams parms = new CswNbtImportWcf.ImportFileParams();
                parms.PostedFile = _Context.Request.Files[0];
                parms.ImportDefName = ImportDefName;
                parms.Overwrite = Overwrite;

                var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportDataReturn, CswNbtImportWcf.ImportFileParams>(
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
        [WebInvoke( Method = "POST", UriTemplate = "uploadImportDefinition?defname={ImportDefName}" )]
        [Description( "Upload Import Data" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn uploadImportDefinition( string ImportDefName )
        {
            CswWebSvcReturn ret = new CswWebSvcReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                CswNbtImportWcf.ImportFileParams parms = new CswNbtImportWcf.ImportFileParams();
                parms.PostedFile = _Context.Request.Files[0];
                parms.ImportDefName = ImportDefName;

                var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, CswNbtImportWcf.ImportFileParams>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: ret,
                    WebSvcMethodPtr: CswNbtWebServiceImport.uploadImportDefinition,
                    ParamObj: parms
                    );

                SvcDriver.run();
            }

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get current status of imports" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportStatusReturn getImportStatus( CswNbtImportWcf.ImportStatusRequest parms )
        {
            CswNbtImportWcf.ImportStatusReturn ret = new CswNbtImportWcf.ImportStatusReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportStatusReturn, CswNbtImportWcf.ImportStatusRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportStatus,
                ParamObj: parms
                );

            SvcDriver.run();

            return ret;
        } // getImportStatus()

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Start import" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn startImport( CswNbtImportWcf.StartImportParams Params )
        {
            CswWebSvcReturn Ret = new CswWebSvcReturn();

            var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, CswNbtImportWcf.StartImportParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.startCAFImport,
                ParamObj: Params
                );

            SvcDriver.run();

            return Ret;
        }//startImport()

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Generate SQL for CAF" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.GenerateSQLReturn generateCAFSql( string ImportDefName )
        {
            CswNbtImportWcf.GenerateSQLReturn Ret = new CswNbtImportWcf.GenerateSQLReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.GenerateSQLReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.generateCAFSql,
                ParamObj: ImportDefName
                );

            SvcDriver.run();

            return Ret;
        }//startImport()
        
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Retrieve bindings for current definition" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportBindingsReturn getBindingsForDefinition( string ImportDefName )
        {
            CswNbtImportWcf.ImportBindingsReturn  Ret = new CswNbtImportWcf.ImportBindingsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportBindingsReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getBindingsForDefinition,
                ParamObj: ImportDefName 
                );

            SvcDriver.run();

            return Ret;
        }
        

    }
}

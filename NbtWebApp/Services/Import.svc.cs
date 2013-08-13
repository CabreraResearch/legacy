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
        public CswNbtWebServiceImport.ImportDefsReturn getImportDefs()
        {
            CswNbtWebServiceImport.ImportDefsReturn ret = new CswNbtWebServiceImport.ImportDefsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceImport.ImportDefsReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportDefs,
                ParamObj: null
                );

            SvcDriver.run();
            return ( ret );
        }


        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "uploadImportData?propid={propid}&blobdataid={blobdataid}&caption={caption}" )]
        [Description( "Upload Import Data" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceImport.ImportDataReturn uploadImportData( string ImportDefName, bool Overwrite )
        {
            CswNbtWebServiceImport.ImportDataReturn ret = new CswNbtWebServiceImport.ImportDataReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                CswNbtWebServiceImport.ImportDataParams parms = new CswNbtWebServiceImport.ImportDataParams();
                parms.PostedFile = _Context.Request.Files[0];
                parms.ImportDefName = ImportDefName;
                parms.Overwrite = Overwrite;

                var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceImport.ImportDataReturn, CswNbtWebServiceImport.ImportDataParams>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: ret,
                    WebSvcMethodPtr: CswNbtWebServiceImport.uploadImportData,
                    ParamObj: parms
                    );

                SvcDriver.run();
            }

            return ret;
        }
    }
}

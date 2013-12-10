using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.Actions.ChemWatch;

namespace NbtWebApp
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class ChemWatch
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Initialize" )]
        [Description( "Get the initialization data for the ChemWatch action" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn Initialize( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.Initialize,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetMatchingSuppliers" )]
        [Description( "Search for matching Suppliers" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn GetMatchingSuppliers( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.GetMatchingSuppliers,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "MaterialSearch" )]
        [Description( "Search for Materials" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn MaterialSearch( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.MaterialSearch,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "SDSDocumentSearch" )]
        [Description( "Search for SDS Documents" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn SDSDocumentSearch( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.SDSDocumentSearch,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "CreateSDSDocuments" )]
        [Description( "Create selected SDS Documents" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn CreateSDSDocuments( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.CreateSDSDocuments,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "GetSDSDocument?filename={filename}" )]
        [Description( "Get a specific SDS Document file" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream GetSDSDocument( string FileName )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.GetSDSDocument,
                ParamObj: FileName
                );
            SvcDriver.run();

            if( false == FileName.EndsWith( ".pdf" ) )
            {
                FileName = FileName + ".pdf";
            }

            WebOperationContext.Current.OutgoingResponse.Headers.Set( "Content-disposition", "inline; filename=" + FileName );
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";

            return Ret.Data.SDSDocument;
        }//startImport()
    }
}
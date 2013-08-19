using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Nbt.csw.Mobile;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Mobile.CISProNbt;

namespace NbtWebApp.Services
{

    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CISProNbtMobile
    {
        private HttpContext _Context = HttpContext.Current;

        private static CswWebSvcSessionAuthenticateData.Authentication.Request AuthRequest
        {
            get
            {
                CswWebSvcSessionAuthenticateData.Authentication.Request Ret = new CswWebSvcSessionAuthenticateData.Authentication.Request();
                Ret.RequiredModules.Add( CswEnumNbtModuleName.CISPro );
                Ret.RequiredModules.Add( CswEnumNbtModuleName.Containers );
                return Ret;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "saveOperations" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn saveOperations( CswNbtCISProNbtMobileData.MobileRequest OperationsArray )
        {
            CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn Ret = new CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn, CswNbtCISProNbtMobileData.MobileRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceCISProNbtMobile.saveOperations,
                ParamObj: OperationsArray
                );

            SvcDriver.run();
            Ret.Data.Success = true;
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Save a given CSV string of records to the temp directory and email a link to the user" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn RLSaveData( RapidLoaderData.RapidLoaderDataRequest Request )
        {
            CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn Ret = new CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn, RapidLoaderData.RapidLoaderDataRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceCISProNbtMobile.RLSaveData,
                ParamObj: Request
                );
            SvcDriver.run();
            return ( Ret );
        }
    }
}

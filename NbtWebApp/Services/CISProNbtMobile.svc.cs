using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
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
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceCISProNbtMobile.saveOperations,
                ParamObj: OperationsArray
                );

            SvcDriver.run();
            Ret.Data.Success = true;
            return ( Ret );
        }
    }
}

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Labels;
using NbtWebApp.WebSvc.Logic.Menus.LandingPages;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Landing Page operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class LandingPages
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Get all of the LandingPage items associated with a given RoleId or ActionId" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLandingPageItems.LandingPageItemsReturn getItems( CswNbtWebServiceLandingPageItems.LandingPageData.Request Request )
        {
            CswNbtWebServiceLandingPageItems.LandingPageItemsReturn Ret = new CswNbtWebServiceLandingPageItems.LandingPageItemsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLandingPageItems.LandingPageItemsReturn, CswNbtWebServiceLandingPageItems.LandingPageData.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLandingPageItems.getLandingPageItems,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Remove an item from the LandingPage" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLandingPageItems.LandingPageItemsReturn deleteItem( CswNbtWebServiceLandingPageItems.LandingPageData.Request Request )
        {
            CswNbtWebServiceLandingPageItems.LandingPageItemsReturn Ret = new CswNbtWebServiceLandingPageItems.LandingPageItemsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLandingPageItems.LandingPageItemsReturn, CswNbtWebServiceLandingPageItems.LandingPageData.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLandingPageItems.deleteLandingPageItem,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Move a LandingPage item to a new cell on the LandingPage" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLandingPageItems.LandingPageItemsReturn moveItem( CswNbtWebServiceLandingPageItems.LandingPageData.Request Request )
        {
            CswNbtWebServiceLandingPageItems.LandingPageItemsReturn Ret = new CswNbtWebServiceLandingPageItems.LandingPageItemsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLandingPageItems.LandingPageItemsReturn, CswNbtWebServiceLandingPageItems.LandingPageData.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLandingPageItems.moveLandingPageItem,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Add a new item to the LandingPage" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLandingPageItems.LandingPageItemsReturn addItem( CswNbtWebServiceLandingPageItems.LandingPageData.Request Request )
        {
            CswNbtWebServiceLandingPageItems.LandingPageItemsReturn Ret = new CswNbtWebServiceLandingPageItems.LandingPageItemsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLandingPageItems.LandingPageItemsReturn, CswNbtWebServiceLandingPageItems.LandingPageData.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLandingPageItems.addLandingPageItem,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;

namespace NbtWebApp.WebSvc.Logic.Menus.LandingPages
{
    /// <summary>
    /// WCF Web Methods for LandingPage Items
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtLandingPageItemsUriMethods
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
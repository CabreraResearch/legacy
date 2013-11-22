﻿using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.CISPro;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Request operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Requests
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "findMaterialCreate" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Get the appropriate Request type for Material Creation" )]
        public CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn getRequestMaterialCreateNodeType()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn Ret = new CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.getRequestMaterialCreate,
                ParamObj: null
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "fulfill" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Fulfill a Request Item" )]
        public CswNbtRequestDataModel.CswRequestReturn fulfillRequest( CswNbtRequestDataModel.RequestFulfill Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswRequestReturn Ret = new CswNbtRequestDataModel.CswRequestReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswRequestReturn, CswNbtRequestDataModel.RequestFulfill>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.fulfillContainerMoveRequest,
                ParamObj: Request
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "place" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Place a Request" )]
        public CswNbtRequestDataModel.CswRequestReturn submitRequest( CswNbtNode.Node Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswRequestReturn Ret = new CswNbtRequestDataModel.CswRequestReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswRequestReturn, CswNbtNode.Node>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.submitRequest,
                ParamObj: Request
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "cart" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Get the Current User's Request Cart" )]
        public CswNbtRequestDataModel.RequestCart getCart()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.RequestCart Ret = new CswNbtRequestDataModel.RequestCart();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.RequestCart, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.getCart,
                ParamObj: null
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", UriTemplate = "counts?CartId={CartId}" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Get the Count Current User's Request Items by Tab" )]
        public CswNbtRequestDataModel.RequestCart getCartCounts( string CartId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.RequestCart Ret = new CswNbtRequestDataModel.RequestCart();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.RequestCart, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.getCartCounts,
                ParamObj: null
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Cart/reset" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Reset the current user's cart counts" )]
        public void resetCartCounts( string CartId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswRequestReturn Ret = new CswNbtRequestDataModel.CswRequestReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswRequestReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr:( Resources, Obj, ParamObj ) => CswNbtActRequesting.resetCartCounts( Resources ),
                ParamObj: null
                );

            InitDriverType.run();
        }

        [OperationContract()]
        [WebInvoke( Method = "PUT", UriTemplate = "Favorites/create" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Create a new Favorite" )]
        public CswNbtRequestDataModel.CswRequestReturn createFavorite( CswNbtRequestDataModel.CswRequestReturn.Ret Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswRequestReturn Ret = new CswNbtRequestDataModel.CswRequestReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswRequestReturn, CswNbtRequestDataModel.CswRequestReturn.Ret>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.createFavorite,
                ParamObj: Request
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract()]
        [WebInvoke( Method = "POST", UriTemplate = "Favorites/copy" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Copy Request Items to or from a Favorites list" )]
        public CswNbtRequestDataModel.CswRequestReturn copyFavorite( CswNbtRequestDataModel.CswRequestReturn.Ret Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswRequestReturn Ret = new CswNbtRequestDataModel.CswRequestReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswRequestReturn, CswNbtRequestDataModel.CswRequestReturn.Ret>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceRequesting.copyFavorite,
                ParamObj: Request
                );

            InitDriverType.run();
            return ( Ret );
        }

        [OperationContract()]
        [WebInvoke( Method = "POST", UriTemplate = "Recurring/copy" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Copy Request Items to or from a Favorites list" )]
        public CswNbtRequestDataModel.CswRequestReturn copyRecurring( CswNbtRequestDataModel.CswRequestReturn.Ret Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtRequestDataModel.CswRequestReturn Ret = new CswNbtRequestDataModel.CswRequestReturn();
            var InitDriverType = new CswWebSvcDriver<CswNbtRequestDataModel.CswRequestReturn, CswNbtRequestDataModel.CswRequestReturn.Ret>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceRequesting.copyRecurring,
                ParamObj : Request
                );

            InitDriverType.run();
            return ( Ret );
        }
    }
}

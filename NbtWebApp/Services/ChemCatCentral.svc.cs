using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for ChemCatCentral Searches
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class ChemCatCentral
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// Returns either C3 or ACD depending on which module is enabled.
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetC3DataService" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn GetC3DataService()
        {
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.GetC3DataService,
                ParamObj: null
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetVendorOptions" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn GetVendorOptions()
        {
            CswC3Params CswC3Params = new CswC3Params();
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3Params>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.GetVendorOptions,
                ParamObj: CswC3Params
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetACDSuppliers" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn GetACDSuppliers()
        {
            CswC3Params CswC3Params = new CswC3Params();
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3Params>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.GetACDSuppliers,
                ParamObj: CswC3Params
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "UpdateACDPrefSuppliers" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn UpdateACDPrefSuppliers( string PrefSupplierIds )
        {
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.UpdateACDPrefSuppliers,
                ParamObj: PrefSupplierIds
                );

            SvcDriver.run();
            return ( Ret );
        }


        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetSearchProperties" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn GetSearchProperties()
        {
            CswC3Params CswC3Params = new CswC3Params();
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3Params>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.GetSearchProperties,
                ParamObj: CswC3Params
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "runC3FilteredSearch" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn runC3FilteredSearch( CswC3SearchParams CswC3SearchParams )
        {
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3SearchParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.RunC3FilteredSearch,
                ParamObj: CswC3SearchParams
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// Search ChemCatCentral database for information on a particular product.
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetProductDetails" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn getC3ProductDetails( CswC3SearchParams CswC3SearchParams )
        {
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3SearchParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.GetC3ProductDetails,
                ParamObj: CswC3SearchParams
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// Search ChemCatCentral database.
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Search" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3SearchReturn runC3Search( CswC3SearchParams CswC3SearchParams )
        {
            CswNbtWebServiceC3Search.CswNbtC3SearchReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3SearchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3SearchReturn, CswC3SearchParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.RunChemCatCentralSearch,
                ParamObj: CswC3SearchParams
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "importProduct" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceC3Search.CswNbtC3CreateMaterialReturn importC3Product( CswNbtWebServiceC3Search.CswNbtC3Import.Request Request )
        {
            CswNbtWebServiceC3Search.CswNbtC3CreateMaterialReturn Ret = new CswNbtWebServiceC3Search.CswNbtC3CreateMaterialReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceC3Search.CswNbtC3CreateMaterialReturn, CswNbtWebServiceC3Search.CswNbtC3Import.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceC3Search.importC3Product,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;


namespace NbtWebApp.Services
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Balances
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "ListConnectedBalances" )]
        [Description( "Get all active Balances" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtBalanceReturn listConnectedBalances()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtBalanceReturn Ret = new CswNbtBalanceReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtBalanceReturn, object>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceSerialBalance.listConnectedBalances,
                ParamObj: null
                );

            SvcDriver.run();
            return ( Ret );
        }

    }
}

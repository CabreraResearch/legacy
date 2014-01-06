using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp.Services
{
    /// <summary>
    /// WCF Web Methods for Design Mode operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Design
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "" )]
        public CswNbtWebServiceDesign.CswNbtDesignReturn getDesignNodeType( string NodeTypeId )
        {
            CswNbtWebServiceDesign.CswNbtDesignReturn Ret = new CswNbtWebServiceDesign.CswNbtDesignReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswNbtWebServiceDesign.CswNbtDesignReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceDesign.getDesignNodeType,
                ParamObj: NodeTypeId
                );

            GetViewDriverType.run();
            return ( Ret );
        }
    }
}

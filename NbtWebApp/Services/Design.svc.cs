using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Layout;

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

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "" )]
        public CswNbtLayoutDataReturn updateLayout( CswNbtLayoutDataCollection LayoutData )
        {
            CswNbtLayoutDataReturn Ret = new CswNbtLayoutDataReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswNbtLayoutDataReturn, CswNbtLayoutDataCollection>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceLayout.UpdateLayout,
                ParamObj : LayoutData
                );

            GetViewDriverType.run();
            return ( Ret );
        }
    }
}

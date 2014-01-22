using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Layout;
using NbtWebApp.WebSvc.Returns;

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
        public CswNbtLayoutDataReturn updateLayout( CswNbtNodeTypeLayout LayoutData )
        {
            CswNbtLayoutDataReturn Ret = new CswNbtLayoutDataReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswNbtLayoutDataReturn, CswNbtNodeTypeLayout>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceLayout.UpdateLayout,
                ParamObj : LayoutData
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Update the position of a tab on a layout" )]
        public CswWebSvcReturn updateTabOrder( CswNbtTabMoveRequest Request )
        {
            CswWebSvcReturn Ret = new CswWebSvcReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswWebSvcReturn, CswNbtTabMoveRequest>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceLayout.UpdateTabOrder,
                ParamObj : Request
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Add a new tab to the given nodetype" )]
        public CswNbtTabAddReturn createNewTab( CswNbtTabAddRequest Request )
        {
            CswNbtTabAddReturn Ret = new CswNbtTabAddReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswNbtTabAddReturn, CswNbtTabAddRequest>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceLayout.CreateNewTab,
                ParamObj : Request
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Remove a tab from the given edit layout" )]
        public CswWebSvcReturn deleteTab( int TabId )
        {
            CswWebSvcReturn Ret = new CswWebSvcReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswWebSvcReturn, int>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceLayout.DeleteTab,
                ParamObj : TabId
                );

            GetViewDriverType.run();
            return ( Ret );
        }


        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "" )]
        public CswNbtLayoutDataReturn removePropsFromLayout( CswNbtNodeTypeLayout LayoutData )
        {
            CswNbtLayoutDataReturn Ret = new CswNbtLayoutDataReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswNbtLayoutDataReturn, CswNbtNodeTypeLayout>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceLayout.RemovePropsFromLayout,
                ParamObj : LayoutData
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "" )]
        public CswNbtLayoutDataReturn getSearchImageLink( string NodeId )
        {
            CswNbtLayoutDataReturn Ret = new CswNbtLayoutDataReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswNbtLayoutDataReturn, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceLayout.GetSearchImageLink,
                ParamObj : NodeId
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Gets the Design NodeType definition for the given FieldTypeId" )]
        public CswNbtWebServiceDesign.CswNbtDesignReturn getDesignNodeTypePropDefinition( string FieldTypeId )
        {
            CswNbtWebServiceDesign.CswNbtDesignReturn Ret = new CswNbtWebServiceDesign.CswNbtDesignReturn();

            var GetViewDriverType = new CswWebSvcDriver<CswNbtWebServiceDesign.CswNbtDesignReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceDesign.getDesignNodeTypePropDefinition,
                ParamObj: FieldTypeId
                );

            GetViewDriverType.run();
            return ( Ret );
        }
    }
}

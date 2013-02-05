using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    
    [DataContract]
    public class NodeResponse : CswWebSvcReturn
    {
        public NodeResponse()
        {
            Data = new NodeSelect.Response();
        }
            
        [DataMember]
        public NodeSelect.Response Data = null;
    }
    
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Nodes
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Generate a Node Select" )]
        public NodeResponse get( NodeSelect.Request Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            NodeResponse Ret = new NodeResponse();
            var GetViewDriverType = new CswWebSvcDriver<NodeResponse, NodeSelect.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNode.getNodes,
                ParamObj: Request
                );

            GetViewDriverType.run();
            return ( Ret );
        }
        
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Get all sizes related to a given node" )]
        public NodeResponse getSizes( CswNbtNode.Node Request )
        {
            // TODO: This should really be a call to Nodes/get with a proper ViewId, 
            // but as this affects multiple wizards, it's not getting fixed today.
            NodeResponse Ret = new NodeResponse();
            var GetViewDriverType = new CswWebSvcDriver<NodeResponse, CswNbtNode.Node>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNode.getSizes,
                ParamObj: Request
                );

            GetViewDriverType.run();
            return ( Ret );
        }
    }
}

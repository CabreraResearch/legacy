using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Services
{

    [DataContract]
    public class MaterialResponse : CswWebSvcReturn
    {
        public MaterialResponse()
        {
            Data = new Response();
        }

        [DataMember]
        public Response Data = null;

        [DataContract]
        public class Response
        {
            [DataMember]
            public View SuppliersView = new View();

            [DataMember]
            public Collection<CswNbtNode.Node> SizeNodes = new Collection<CswNbtNode.Node>();

            [DataMember]
            public CswNbtNode.Node TempNode = new CswNbtNode.Node( null );

        }
    }

    /// <summary>
    /// WCF Web Methods for Materials operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Materials
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "GET" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Fetch the views relevant to Create Material" )]
        public MaterialResponse views()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            MaterialResponse Ret = new MaterialResponse();
            var GetViewDriverType = new CswWebSvcDriver<MaterialResponse, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceCreateMaterial.getCreateMaterialViews,
                ParamObj: ""
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "initialize" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Fetch the views relevant to Create Material" )]
        public MaterialResponse initializeCreateMaterial( string NodeId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            MaterialResponse Ret = new MaterialResponse();
            var GetViewDriverType = new CswWebSvcDriver<MaterialResponse, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceCreateMaterial.initializeCreateMaterial,
                ParamObj: NodeId
                );

            GetViewDriverType.run();
            return ( Ret );
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    public class NodeSelect
    {
        [DataContract]
        public class Request
        {
            [DataMember( IsRequired = false )]
            public Int32 NodeTypeId = Int32.MinValue;
            [DataMember( IsRequired = false )]
            public Int32 ObjectClassId = Int32.MinValue;

            private NbtObjectClass _ObjectClass;

            [DataMember( IsRequired = false )]
            public string ObjectClass
            {
                get { return _ObjectClass; }
                set { _ObjectClass = value; }
            }

            private NbtObjectClass _RelatedToObjectClass;

            [DataMember( IsRequired = false )]
            public string RelatedToObjectClass
            {
                get { return _RelatedToObjectClass; }
                set { _RelatedToObjectClass = value; }
            }

            private string _RelatedToNodeId = string.Empty;
            [DataMember( IsRequired = false )]
            public string RelatedToNodeId
            {
                get { return _RelatedToNodeId; }
                set
                {
                    _RelatedToNodeId = value;
                    CswPrimaryKey RelatedPk = CswConvert.ToPrimaryKey( _RelatedToNodeId );
                    if( CswTools.IsPrimaryKey( RelatedPk ) )
                    {
                        RelatedNodeId = RelatedPk;
                    }
                }
            }

            private string _ViewId = string.Empty;
            [DataMember( IsRequired = false )]
            public string ViewId
            {
                get { return _ViewId; }
                set
                {
                    _ViewId = value;
                    CswNbtSessionDataId ViewKey = new CswNbtSessionDataId( _ViewId );
                    if( null != ViewKey && ViewKey.isSet() )
                    {
                        NbtViewId = ViewKey;
                    }
                }
            }

            public CswNbtSessionDataId NbtViewId = null;

            public CswPrimaryKey RelatedNodeId = null;

        }

        [DataContract]
        public class Response : CswWebSvcReturn
        {
            public Response()
            {
                Data = new Ret();
            }

            public class Ret
            {
                [DataMember]
                public Int32 RelatedObjectClassId = Int32.MinValue;

                [DataMember]
                public bool CanAdd = false;
                [DataMember]
                public bool UseSearch = false;

                [DataMember]
                public Int32 NodeTypeId = Int32.MinValue;
                [DataMember]
                public Int32 ObjectClassId = Int32.MinValue;

                [DataMember]
                public Collection<CswNbtNode.Node> Nodes = new Collection<CswNbtNode.Node>();
            }

            [DataMember]
            public Ret Data = null;
        }



        [DataContract]
        public class PropertyView
        {
            [DataMember]
            public string PropName = string.Empty;
            [DataMember]
            public string NodeTypeId = string.Empty;
            [DataMember]
            public string NodeId = string.Empty;
            [DataMember]
            public string TargetNodeTypeId = string.Empty;
            [DataMember]
            public string TargetNodeTypeName = string.Empty;
        }
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
        public NodeSelect.Response get( NodeSelect.Request Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            NodeSelect.Response Ret = new NodeSelect.Response();
            var GetViewDriverType = new CswWebSvcDriver<NodeSelect.Response, NodeSelect.Request>(
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
        [Description( "Get the viewid of a property view" )]
        public NodeSelect.Response getRelationshipOpts( NodeSelect.PropertyView Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            NodeSelect.Response Ret = new NodeSelect.Response();
            var GetViewDriverType = new CswWebSvcDriver<NodeSelect.Response, NodeSelect.PropertyView>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNode.getRelationshipOpts,
                ParamObj: Request
                );

            GetViewDriverType.run();
            return ( Ret );
        }

    }
}

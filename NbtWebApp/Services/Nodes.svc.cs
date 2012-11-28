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
            [DataMember( IsRequired = false )]
            public NbtObjectClass ObjectClass = CswNbtResources.UnknownEnum;
            [DataMember( IsRequired = false )]
            public NbtObjectClass RelatedToObjectClass = CswNbtResources.UnknownEnum;

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
                public Collection<Node> Nodes = new Collection<Node>();
            }

            [DataMember]
            public Ret Data = null;
        }

        [DataContract]
        public class Node
        {
            public Node( CswNbtNode NbtNode )
            {
                if( null != NbtNode )
                {
                    _NodeId = NbtNode.NodeId.ToString();
                    _NodePk = NbtNode.NodeId;
                    NodeName = NbtNode.NodeName;
                }
            }

            private string _NodeId = string.Empty;
            private CswPrimaryKey _NodePk = null;

            public CswPrimaryKey NodePk
            {
                get { return _NodePk; }
                set
                {
                    _NodePk = value;
                    if( CswTools.IsPrimaryKey( value ) )
                    {
                        _NodeId = _NodePk.ToString();
                    }
                    else
                    {
                        _NodeId = string.Empty;
                    }
                }
            }

            [DataMember]
            public string NodeId
            {
                get { return _NodeId; }
                set
                {
                    _NodeId = value;
                    _NodePk = CswConvert.ToPrimaryKey( _NodeId );
                }
            }

            [DataMember( IsRequired = false )]
            public string NodeName = String.Empty;
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
    }
}

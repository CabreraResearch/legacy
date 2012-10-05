using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace API.Test.Services
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract( Namespace = "API.Test.Services" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Properties
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        [WebGet]
        [Description( "Generic method. Does nothing." )]
        public void DoWork()
        {
            // Add your operation implementation here
            return;
        }

        /// <summary>
        /// Get Properties
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "/getProps?EditMode={EditMode}&NodeId={NodeId}&SafeNodeKey={SafeNodeKey}&TabId={TabId}&NodeTypeId={NodeTypeId}&Date={Date}&filterToPropId={filterToPropId}&Multi={Multi}&ConfigMode={ConfigMode}&RelatedNodeId={RelatedNodeId}&RelatedNodeTypeId={RelatedNodeTypeId}&RelatedObjectClassId={RelatedObjectClassId}" )]
        [Description( "A thin WCF wrapper around the old wsNBT.getProps method" )]
        public string getProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId, string Date, string filterToPropId, string Multi, string ConfigMode, string RelatedNodeId, string RelatedNodeTypeId, string RelatedObjectClassId )
        {
            return "";
        }

        public class SuccessData
        {
            public bool Success;
        }

        /// <summary>
        /// Get Properties
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "DELETE" )]
        [Description( "Delete, in a RESTful WCF world." )]
        public SuccessData drop( string NodeId )
        {
            return new SuccessData { Success = true };
        }

        public class PropertyData
        {
            public Int32 PropId = 1;
            public string PropName = "";
            public string PropVal = "";
        }

        public class NodeData
        {
            public NodeData()
            {
                Properties = new Collection<PropertyData>();
            }
            public string NodeId = "";
            public Int32 NodeTypeId = 1;
            public Collection<PropertyData> Properties;
        }

        /// <summary>
        /// Create node
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "PUT", UriTemplate = "/{NodeTypeName}/create" )]
        [Description( "Create, in a RESTful WCF world." )]
        public NodeData create( string NodeTypeName )
        {
            return new NodeData();
        }

        /// <summary>
        /// Create node
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "/{NodeTypeName}/{TabName}/edit" )]
        [Description( "Edit, in a RESTful WCF world." )]
        public NodeData edit( string NodeTypeName, string TabName, NodeData Node )
        {
            return Node;
        }

        /// <summary>
        /// Create node
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "/{NodeTypeName}/{TabName}/{NodeId}?RelatedNodeId={RelatedNodeId}&RelatedNodeTypeId={RelatedNodeTypeId}" )]
        [Description( "Edit, in a RESTful WCF world." )]
        public NodeData get( string NodeTypeName, string TabName, string NodeId, string RelatedNodeId, string RelatedNodeTypeId, NodeData Node )
        {
            return Node;
        }

        // Add more operations here and mark them with [OperationContract]
    }
}

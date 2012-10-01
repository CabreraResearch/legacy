using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace API.Test
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
        public string getProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId, string Date, string filterToPropId, string Multi, string ConfigMode, string RelatedNodeId, string RelatedNodeTypeId, string RelatedObjectClassId )
        {
            return "";
        }


        // Add more operations here and mark them with [OperationContract]
    }
}

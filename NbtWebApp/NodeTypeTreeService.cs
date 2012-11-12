using System.Collections.Generic;   // supports IDictionary
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Web.Services;          // supports WebMethod attribute
using ChemSW.Config;
using ChemSW.Nbt;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Nbt.MetaData;
using Telerik.Web.UI;
namespace ChemSW.NbtWebControls
{
    /// <summary>
    /// NodeTypeTree WebService
    /// </summary>
    [ScriptService]
    [WebService( Namespace = "http://localhost/NbtWebApp" )]
    [WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
    public class NodeTypeTreeService : System.Web.Services.WebService
    {
        public NodeTypeTreeService()
        {
            //        CswNbtSession CswSessionWeb = new CswNbtSession( Context.Application, Context.Session, Context.Request, Context.Response );


            //bz # 9278


            CswSessionResourcesNbt CswInitialization = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, Context, string.Empty, SetupMode.NbtWeb );
            _CswNbtResources = CswInitialization.CswNbtResources;



            //string SessionId = string.Empty;
            //HttpCookie SessionCookie = Context.Request.Cookies.Get( CswSessionWeb.SessionCookieName );
            //if ( null != SessionCookie )
            //{
            //    SessionId = SessionCookie.Value;
            //    if ( string.Empty != SessionId )
            //    {
            //        CswNbtInitialization CswInitialization = new CswNbtInitialization( CswSessionWeb, _FilesPath, SetupMode.Web, null, null );
            //        _CswNbtResources = CswInitialization.CswNbtResources;
            //        _CswNbtResources.AccessId = "1"; //KLUDGE: SHould be masterid
            //        _CswNbtResources.CswResources.refreshDataDictionary();
            //        CswSessionWeb.CswNbtResources = _CswNbtResources;

            //        if ( CswSessionWeb.load( SessionId ) )
            //        {
            //            _CswNbtResources.AccessId = CswSessionWeb.AccessId;
            //        }
            //        else
            //        {
            //            throw ( new CswDniException( "Login required", "SessionId " + SessionCookie + " is not persisted" ) );
            //        }
            //    }
            //    else
            //    {
            //        throw ( new CswDniException( "Login required", "The cookie " + CswSessionWeb.SessionCookieName + " exists but has an empty value" ) );
            //    }
            //}
            //else
            //{
            //    throw ( new CswDniException( "Login required", "User should never have had a link to this page without having logged in" ) );
            //}


        }//ctor

        public CswNbtResources _CswNbtResources;

        [WebMethod( EnableSession = true )]
        public RadTreeNodeData[] GetNodeChildren( RadTreeNodeData node, object context )
        {

            try
            {

                // extract the "context" dictionary information. OnClientNodePopulating event loads the dictionary
                IDictionary<string, object> contextDictionary = (IDictionary<string, object>) context;
                // create the array of RadTreeNodeData that will be returned by this method
                List<RadTreeNodeData> result = new List<RadTreeNodeData>();

                if( contextDictionary.ContainsKey( "Parent" ) && contextDictionary["Parent"] != null &&
                    contextDictionary.ContainsKey( "Mode" ) && contextDictionary["Mode"] != null )
                {
                    NbtDesignMode Mode = NbtDesignMode.Standard;
                    if( contextDictionary["Mode"].ToString() == "i" )
                        Mode = NbtDesignMode.Inspection;

                    CswNodeTypeTree NodeTypeTree = new CswNodeTypeTree( _CswNbtResources );
                    NodeTypeTree.ShowTabsAndProperties = true;
                    NodeTypeTree.PropertySort = CswNodeTypeTree.PropertySortSetting.DisplayOrder;
                    NodeTypeTree.ShowConditionalPropertiesBeneath = true;
                    if( Mode == NbtDesignMode.Inspection )
                    {
                        NodeTypeTree.ShowQuestionNumbers = true;
                        NodeTypeTree.ObjectClassIdsToInclude = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass ).ObjectClassId.ToString();
                        NodeTypeTree.TreeName = "Inspection Design";
                        NodeTypeTree.TreeView.OnClientNodePopulating = "NodeTypeTree_OnNodePopulating_InspectionMode";
                    }

                    NodeTypeTree.setSelectedNode( contextDictionary["Parent"].ToString() );
                    NodeTypeTree.DataBind();
                    RadTreeNode ParentNode = NodeTypeTree.TreeView.FindNodeByValue( contextDictionary["Parent"].ToString() );
                    if( ParentNode != null )
                    {
                        foreach( RadTreeNode ChildNode in ParentNode.Nodes )
                        {
                            result.Add( CopyNodeToNodeData( ChildNode ) );
                        }
                    }
                }
                return result.ToArray();

            } //try 

            finally
            {
                _release();
            }
        } // GetNodeChildren()


        private RadTreeNodeData CopyNodeToNodeData( RadTreeNode SourceNode )
        {
            RadTreeNodeData NodeData = new RadTreeNodeData();
            NodeData.Text = SourceNode.Text;
            NodeData.Value = SourceNode.Value;
            NodeData.HoveredCssClass = SourceNode.HoveredCssClass;
            NodeData.SelectedCssClass = SourceNode.SelectedCssClass;
            NodeData.ImageUrl = SourceNode.ImageUrl;
            NodeData.ExpandMode = TreeNodeExpandMode.WebService;
            //NodeData.ExpandMode = TreeNodeExpandMode.ClientSide;
            return NodeData;
        } // CopyNodeToNodeData()


        private void _release()
        {
            if( _CswNbtResources != null )
            {
                _CswNbtResources.finalize();
                _CswNbtResources.release();
            }

        }

    } // NodeTypeTreeService
}
using System;
using System.Web.Services;          // supports WebMethod attribute
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Collections.Generic;   // supports IDictionary
using ChemSW.Nbt;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using Telerik.Web.UI;
using ChemSW.Config;

/// <summary>
/// TreeView WebService
/// </summary>
[ScriptService]
[WebService( Namespace = "http://localhost/NbtWebApp" )]
[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
public class TreeViewService : System.Web.Services.WebService
{
    public CswNbtResources CswNbtResources;


    private string _FilesPath
    {
        get
        {
            return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
        }
    }

    public TreeViewService()
    {
        //        CswNbtSession CswSessionWeb = new CswNbtSession( Context.Application, Context.Session, Context.Request, Context.Response );

        CswSessionResourcesNbt CswInitialization = new CswSessionResourcesNbt( Context.Application, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.NbtWeb );
        CswNbtResources = CswInitialization.CswNbtResources;
        //        CswSessionWeb.CswNbtResources = CswNbtResources;


        //string SessionId = string.Empty;
        //HttpCookie SessionCookie = Context.Request.Cookies.Get( CswSessionWeb.SessionCookieName );
        //if ( null != SessionCookie )
        //{
        //    SessionId = SessionCookie.Value;
        //    if ( string.Empty != SessionId )
        //    {
        //        CswNbtInitialization CswInitialization = new CswNbtInitialization( CswSessionWeb, _FilesPath, SetupMode.Web, null, null );
        //        CswNbtResources = CswInitialization.CswNbtResources;
        //        CswNbtResources.AccessId = "1"; //KLUDGE: SHould be masterid
        //        CswNbtResources.CswResources.refreshDataDictionary();
        //        CswSessionWeb.CswNbtResources = CswNbtResources;

        //        if ( CswSessionWeb.load( SessionId ) )
        //        {
        //            CswNbtResources.AccessId = CswSessionWeb.AccessId;
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


    [WebMethod( EnableSession = true )]
    public RadTreeNodeData[] GetNodeChildren( RadTreeNodeData node, object context )
    {
        try
        {
            // extract the "context" dictionary information. OnClientNodePopulating event loads the dictionary
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>) context;
            // create the array of RadTreeNodeData that will be returned by this method
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();

            if( contextDictionary.ContainsKey( "SessionViewId" ) && contextDictionary["SessionViewId"] != null &&
                contextDictionary.ContainsKey( "SelectedNodeKey" ) && contextDictionary["SelectedNodeKey"] != null &&
                contextDictionary.ContainsKey( "PageSize" ) && contextDictionary["PageSize"] != null &&
                contextDictionary.ContainsKey( "ParentNodeKey" ) && contextDictionary["ParentNodeKey"] != null )
            {
				CswNbtSessionDataId SessionViewId = new CswNbtSessionDataId( CswConvert.ToInt32( contextDictionary["SessionViewId"].ToString() ) );
                CswNbtNodeKey SelectedNodeKey = new CswNbtNodeKey( CswNbtResources, contextDictionary["SelectedNodeKey"].ToString() );
                CswNbtNodeKey ParentNodeKey = new CswNbtNodeKey( CswNbtResources, contextDictionary["ParentNodeKey"].ToString() );
                CswNbtView View = new CswNbtView( CswNbtResources );
				View = CswNbtResources.ViewSelect.getSessionView( SessionViewId );
                RadTreeView Tree = makeTree( View, ref ParentNodeKey, null, CswConvert.ToInt32( contextDictionary["PageSize"].ToString() ), null );

                RadTreeNode Node = Tree.FindNodeByValue( contextDictionary["ParentNodeKey"].ToString() );

                if( Node != null && Node.Nodes.Count > 0 )
                {
                    foreach( RadTreeNode ChildNode in Node.Nodes )
                    {
                        result.Add( CopyNodeToNodeData( ChildNode, View, SelectedNodeKey ) );
                    }
                }
            }
            return result.ToArray();

        }//try

        finally
        {
            _release();
        }
    }//GetNodeChildren()


    /// <summary>
    /// Fetches additional siblings of the provided node
    /// </summary>
    /// <param name="node">"More" Placeholder Node</param>
    /// <param name="context">Dictionary of other information, including ViewType and ViewId</param>
    /// <returns></returns>
    [WebMethod( EnableSession = true )]
    public RadTreeNodeData[] GetMoreNodes( RadTreeNodeData node, object context )
    {
        try
        {
            // extract the "context" dictionary information. OnClientNodePopulating event loads the dictionary
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>) context;
            // create the array of RadTreeNodeData that will be returned by this method
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();

            if( contextDictionary.ContainsKey( "SessionViewId" ) && contextDictionary["SessionViewId"] != null &&
                contextDictionary.ContainsKey( "PageSize" ) && contextDictionary["PageSize"] != null &&
                contextDictionary.ContainsKey( "SelectedNodeKey" ) && contextDictionary["SelectedNodeKey"] != null &&
                contextDictionary.ContainsKey( "ParentNodeKey" ) && contextDictionary["ParentNodeKey"] != null &&
                contextDictionary.ContainsKey( "MoreNodeKey" ) && contextDictionary["MoreNodeKey"] != null )
            {
                CswNbtNodeKey MoreKey = new CswNbtNodeKey( CswNbtResources, contextDictionary["MoreNodeKey"].ToString() );

				CswNbtSessionDataId SessionViewId = new CswNbtSessionDataId( CswConvert.ToInt32( contextDictionary["SessionViewId"].ToString() ) );
                CswNbtNodeKey SelectedNodeKey = new CswNbtNodeKey( CswNbtResources, contextDictionary["SelectedNodeKey"].ToString() );
                CswNbtNodeKey ParentNodeKey = new CswNbtNodeKey( CswNbtResources, contextDictionary["ParentNodeKey"].ToString() );
                CswNbtView View = new CswNbtView( CswNbtResources );
				View = CswNbtResources.ViewSelect.getSessionView( SessionViewId );

                CswNbtViewRelationship FirstChildRelationship = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( MoreKey.ViewNodeUniqueId );

                // This is for the proper generation of nodecounts
                if( ParentNodeKey.NodeSpecies != NodeSpecies.Plain )
                    ParentNodeKey = null;

                RadTreeView Tree = makeTree( View,
                                            ref ParentNodeKey,
                                            FirstChildRelationship,
                                            CswConvert.ToInt32( contextDictionary["PageSize"] ),
                                            MoreKey );

                RadTreeNode ParentNode = Tree.FindNodeByValue( contextDictionary["ParentNodeKey"].ToString() );

                if( ParentNode != null && ParentNode.Nodes.Count > 0 )
                {
                    foreach( RadTreeNode ChildNode in ParentNode.Nodes )
                    {
                        result.Add( CopyNodeToNodeData( ChildNode, View, SelectedNodeKey ) );
                    }
                }
            }
            return result.ToArray();

        }//try

        finally
        {
            _release();
        }

    }//GetMoreNodes()

    private RadTreeNodeData CopyNodeToNodeData( RadTreeNode SourceNode, CswNbtView View, CswNbtNodeKey SelectedNodeKey )
    {
        RadTreeNodeData NodeData = new RadTreeNodeData();
        NodeData.Text = SourceNode.Text;
        NodeData.Value = SourceNode.Value;
        NodeData.HoveredCssClass = SourceNode.HoveredCssClass;
        NodeData.SelectedCssClass = SourceNode.SelectedCssClass;
        NodeData.ImageUrl = SourceNode.ImageUrl;

        CswNbtNodeKey SourceNbtNodeKey = new CswNbtNodeKey( CswNbtResources, SourceNode.Value );
        CswNbtViewNode ViewNode = View.FindViewNodeByUniqueId( SourceNbtNodeKey.ViewNodeUniqueId );
        if( SourceNbtNodeKey.NodeSpecies == NodeSpecies.Group || ViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ).Count > 0 )
            NodeData.ExpandMode = TreeNodeExpandMode.WebService;
        else
            NodeData.ExpandMode = TreeNodeExpandMode.ClientSide;

        //NodeData.Checkable = ( SelectedNodeKey.NodeTypeId == SourceNbtNodeKey.NodeTypeId );

        return NodeData;
    }

    private ICswNbtTree _CswNbtTree = null;

    private RadTreeView makeTree( CswNbtView View, ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, CswNbtNodeKey IncludedKey )
    {
        _CswNbtTree = CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, false, true, IncludedKey, false );

        string XmlStr = _CswNbtTree.getTreeAsXml();
        RadTreeView TreeView1 = new RadTreeView();
        TreeView1.EnableEmbeddedSkins = false;
        TreeView1.Skin = "ChemSW";
        TreeView1.LoadXml( XmlStr );
        return TreeView1;


    }



    private void _release()
    {
        if( CswNbtResources != null )
        {
            CswNbtResources.finalize();
            CswNbtResources.release();
        }

    }

}

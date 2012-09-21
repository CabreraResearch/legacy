using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMainMenu
    {

        private readonly CswCommaDelimitedString _MenuItems = new CswCommaDelimitedString()
        {
            //"Search",
            "Add",
            "More",
            "Copy",
            "Delete",
            "Save View As",
            "Print",
            "Export",
            "Edit View",
            "Multi-Edit"
        };

        public enum MenuActions
        {
            Unknown,
            AddNode,
            CopyNode,
            DeleteNode,
            editview,
            //GenericSearch,
            multiedit,
            PrintView,
            PrintLabel,
            SaveViewAs//,
            //ViewSearch
        }

        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceMainMenu( CswNbtResources CswNbtResources, string LimitMenuTo = null )
        {
            _CswNbtResources = CswNbtResources;
            if( false == string.IsNullOrEmpty( LimitMenuTo ) )
            {
                CswCommaDelimitedString MenuItemsToKeep = new CswCommaDelimitedString();

                CswCommaDelimitedString MenuItemsToFind = new CswCommaDelimitedString();
                MenuItemsToFind.FromString( LimitMenuTo );
                if( MenuItemsToFind.Count > 0 )
                {
                    foreach( string Item in MenuItemsToFind )
                    {
                        if( _MenuItems.Contains( Item ) )
                        {
                            MenuItemsToKeep.Add( Item );
                        }
                    }
                    _MenuItems = MenuItemsToKeep;
                }
            }

        }

        public JObject getMenu( CswNbtView View, string SafeNodeKey, Int32 NodeTypeId, string PropIdAttr, bool ReadOnly )
        {

            CswTimer MainMenuTimer = new CswTimer();

            JObject Ret = new JObject();

            CswPrimaryKey RelatedNodeId = new CswPrimaryKey();
            string RelatedNodeName = string.Empty;
            string RelatedNodeTypeId = string.Empty;
            string RelatedObjectClassId = string.Empty;
            CswNbtNode Node = null;

            if( false == string.IsNullOrEmpty( SafeNodeKey ) )
            {
                CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, SafeNodeKey );
                Node = _CswNbtResources.Nodes[NbtNodeKey];
                if( null != Node )
                {
                    RelatedNodeId = Node.NodeId;
                    RelatedNodeName = Node.NodeName;
                    RelatedNodeTypeId = Node.NodeTypeId.ToString();
                    RelatedObjectClassId = Node.getObjectClassId().ToString();
                }
            }

            // MORE
            if( _MenuItems.Contains( "More" ) )
            {
                JObject MoreObj = new JObject();

                if( null == View && Int32.MinValue != NodeTypeId )
                {
                    // ADD for Searches
                    if( _MenuItems.Contains( "Add" ) && false == ReadOnly )
                    {
                        JObject AddObj = new JObject();

                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                        if( null != NodeType )
                        {
                            AddObj[NodeType.NodeTypeName] = makeAddMenuItem( NodeType, RelatedNodeId, RelatedNodeName, RelatedNodeTypeId, RelatedObjectClassId );
                            AddObj["haschildren"] = true;
                            Ret["Add"] = AddObj;
                        }
                    } // if( _MenuItems.Contains( "Add" ) && false == ReadOnly )
                } // if( null == View && Int32.MinValue != NodeTypeId )
                if( null != View )
                {
                    // ADD for Views
                    if( _MenuItems.Contains( "Add" ) && false == ReadOnly )
                    {
                        JObject AddObj = new JObject();

                        // case 21672
                        CswNbtViewNode ParentNode = View.Root;
                        bool LimitToFirstLevelRelationships = ( View.ViewMode == NbtViewRenderingMode.Grid );
                        if( LimitToFirstLevelRelationships && View.Visibility == NbtViewVisibility.Property )
                        {
                            if( string.IsNullOrEmpty( SafeNodeKey ) )
                            {
                                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, false );
                                Tree.goToNthChild( 0 );
                                CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
                                Node = _CswNbtResources.Nodes[NodeKey];
                                if( null != Node )
                                {
                                    RelatedNodeId = Node.NodeId;
                                    RelatedNodeName = Node.NodeName;
                                    RelatedNodeTypeId = Node.NodeTypeId.ToString();
                                    RelatedObjectClassId = Node.getObjectClassId().ToString();
                                }
                            }
                            ParentNode = View.Root.ChildRelationships[0];
                        }
                        foreach( JProperty AddNodeType in ParentNode.AllowedChildNodeTypes( LimitToFirstLevelRelationships )
                            .Select( Entry => new JProperty( Entry.NodeType.NodeTypeName,
                                                             makeAddMenuItem( Entry.NodeType, RelatedNodeId, RelatedNodeName, RelatedNodeTypeId, RelatedObjectClassId ) ) ) )
                        {
                            AddObj.Add( AddNodeType );
                        }

                        if( AddObj.HasValues )
                        {
                            AddObj["haschildren"] = true;
                            Ret["Add"] = AddObj;
                        }
                    }

                    // COPY
                    if( _MenuItems.Contains( "Copy" ) &&
                        false == ReadOnly &&
                        null != Node && Node.NodeSpecies == NodeSpecies.Plain &&
                        View.ViewMode != NbtViewRenderingMode.Grid &&
                        _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, Node.getNodeType() ) )
                    {
                        string BadPropertyName = string.Empty;
                        if( false == Node.getNodeType().IsUniqueAndRequired( ref BadPropertyName ) )
                        {
                            MoreObj["Copy"] = new JObject();
                            MoreObj["Copy"]["action"] = MenuActions.CopyNode.ToString();
                            MoreObj["Copy"]["nodeid"] = Node.NodeId.ToString();
                            MoreObj["Copy"]["nodename"] = Node.NodeName;
                            MoreObj["Copy"]["nodetypeid"] = Node.NodeTypeId.ToString();
                        }
                    }

                    // DELETE
                    if( _MenuItems.Contains( "Delete" ) &&
                        false == ReadOnly &&
                        false == string.IsNullOrEmpty( SafeNodeKey ) &&
                        null != Node &&
                        View.ViewMode != NbtViewRenderingMode.Grid &&
                        Node.NodeSpecies == NodeSpecies.Plain &&
                        _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, Node.getNodeType(), false, null, null, Node.NodeId, null ) )
                    {
                        MoreObj["Delete"] = new JObject();
                        MoreObj["Delete"]["action"] = MenuActions.DeleteNode.ToString();
                        MoreObj["Delete"]["nodeid"] = Node.NodeId.ToString();
                        MoreObj["Delete"]["nodename"] = Node.NodeName;
                    }

                    // SAVE VIEW AS
                    if( _MenuItems.Contains( "Save View As" ) &&
                        false == View.ViewId.isSet() &&
                        _CswNbtResources.Permit.can( _CswNbtResources.Actions[CswNbtActionName.Edit_View] ) )
                    {
                        View.SaveToCache( false );
                        MoreObj["Save View As"] = new JObject();
                        MoreObj["Save View As"]["action"] = MenuActions.SaveViewAs.ToString();
                        MoreObj["Save View As"]["viewid"] = View.SessionViewId.ToString();
                        MoreObj["Save View As"]["viewmode"] = View.ViewMode.ToString();
                    }

                    JObject PrintObj = null;

                    // PRINT LABEL
                    if( _MenuItems.Contains( "Print Label" ) &&
                        false == string.IsNullOrEmpty( SafeNodeKey ) &&
                        null != Node &&
                        null != Node.getNodeType() )
                    {
                        if( Node.getNodeType().HasLabel )
                        {
                            PrintObj = PrintObj ?? new JObject( new JProperty( "haschildren", true ) );
                            PrintObj["Print Label"] = new JObject();
                            PrintObj["Print Label"]["nodeid"] = Node.NodeId.ToString();
                            PrintObj["Print Label"]["nodetypeid"] = Node.NodeTypeId;
                            PrintObj["Print Label"]["action"] = MenuActions.PrintLabel.ToString();
                        }
                    }
                    // PRINT
                    if( _MenuItems.Contains( "Print" ) &&
                        View.ViewMode == NbtViewRenderingMode.Grid )
                    {
                        View.SaveToCache( false );
                        PrintObj = PrintObj ?? new JObject( new JProperty( "haschildren", true ) );
                        PrintObj["Print View"] = new JObject();
                        PrintObj["Print View"]["action"] = MenuActions.PrintView.ToString();
                    }

                    if( null != PrintObj )
                    {
                        MoreObj["Print"] = PrintObj;
                    }

                    // EXPORT
                    if( _MenuItems.Contains( "Export" ) )
                    {
                        if( NbtViewRenderingMode.Grid == View.ViewMode )
                        {
                            JObject ExportObj = new JObject();
                            MoreObj["Export"] = ExportObj;

                            View.SaveToCache( false );
                            ExportObj["CSV"] = new JObject();
                            ExportObj["CSV"]["popup"] = "wsNBT.asmx/gridExportCSV?ViewId=" + View.SessionViewId.ToString();

                            ExportObj["haschildren"] = true;
                        }
                    }
                } // if( null != View )

                // EDIT VIEW
                if( _MenuItems.Contains( "Edit View" ) &&
                    _CswNbtResources.Permit.can( CswNbtActionName.Edit_View ) )
                {
                    MoreObj["Edit View"] = new JObject();
                    MoreObj["Edit View"]["action"] = MenuActions.editview.ToString();
                }

                if( _MenuItems.Contains( "Multi-Edit" ) &&
                    false == ReadOnly &&
                    null != View &&
                    _CswNbtResources.Permit.can( CswNbtActionName.Multi_Edit ) &&
                    ( View.ViewMode != NbtViewRenderingMode.Grid ||
                    /* Per discussion with David, for the short term eliminate the need to validate the selection of nodes across different nodetypes in Grid views. */
                      View.Root.ChildRelationships.Count == 1 )
                    )
                {
                    MoreObj["Multi-Edit"] = new JObject();
                    MoreObj["Multi-Edit"]["action"] = MenuActions.multiedit.ToString();
                }

                if( MoreObj.Count > 0 )
                {
                    MoreObj["haschildren"] = true;
                    Ret["More"] = MoreObj;
                }
            } // if( _MenuItems.Contains( "More" ) )

            _CswNbtResources.logTimerResult( "CswNbtWebServiceMainMenu.getMenu()", MainMenuTimer.ElapsedDurationInSecondsAsString );


            return Ret;
        } // getMenu()

        public static JObject makeAddMenuItem( CswNbtMetaDataNodeType NodeType, CswPrimaryKey RelatedNodeId, string RelatedNodeName, string RelatedNodeTypeId, string RelatedObjectClassId )
        {
            JObject Ret = new JObject();
            Ret["text"] = default( string );
            Ret["nodetypeid"] = default( string );
            if( null != NodeType )
            {
                Ret["text"] = NodeType.NodeTypeName;
                Ret["nodetypeid"] = NodeType.NodeTypeId;
                Ret["icon"] = CswNbtMetaDataObjectClass.IconPrefix16 + NodeType.IconFileName;
            }
            Ret["relatednodeid"] = default( string );
            if( null != RelatedNodeId && Int32.MinValue != RelatedNodeId.PrimaryKey )
            {
                Ret["relatednodeid"] = RelatedNodeId.ToString();
            }

            Ret["relatednodename"] = RelatedNodeName;
            Ret["relatednodetypeid"] = RelatedNodeTypeId;
            Ret["relatedobjectclassid"] = RelatedObjectClassId;
            Ret["action"] = MenuActions.AddNode.ToString();

            return Ret;
        } // makeAddMenuItem()


    } // class CswNbtWebServiceMainMenu
} // namespace ChemSW.Nbt.WebServices

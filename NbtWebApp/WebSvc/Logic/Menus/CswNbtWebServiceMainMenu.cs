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

        public JObject getMenu( CswNbtView View, string SafeNodeKey, Int32 NodeTypeId, string PropIdAttr, bool ReadOnly, string NodeId )
        {

            CswTimer MainMenuTimer = new CswTimer();

            JObject Ret = new JObject();

            CswPrimaryKey RelatedNodeId = new CswPrimaryKey();
            string RelatedNodeName = string.Empty;
            CswNbtNode Node = null;

            if( false == String.IsNullOrEmpty( SafeNodeKey ) )
            {
                CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( SafeNodeKey );
                Node = _CswNbtResources.Nodes[NbtNodeKey];
            }
            else if( false == String.IsNullOrEmpty( NodeId ) )
            {
                CswPrimaryKey NodePk = CswConvert.ToPrimaryKey( NodeId );
                Node = _CswNbtResources.Nodes[NodePk];
            }
            if( null != Node )
            {
                RelatedNodeId = Node.NodeId;
                RelatedNodeName = Node.NodeName;
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
                        if( null != NodeType && _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, NodeType ) && NodeType.getObjectClass().CanAdd )
                        {
                            AddObj[NodeType.NodeTypeName] = makeAddMenuItem( NodeType, RelatedNodeId, RelatedNodeName );
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
                        bool LimitToFirstLevelRelationships = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid );
                        if( LimitToFirstLevelRelationships && View.Visibility == CswEnumNbtViewVisibility.Property )
                        {
                            if( null == Node )
                            {
                                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, false, false );
                                if( Tree.getChildNodeCount() > 0 )
                                {
                                    Tree.goToNthChild( 0 );
                                    CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
                                    Node = _CswNbtResources.Nodes[NodeKey];
                                    if( null != Node )
                                    {
                                        RelatedNodeId = Node.NodeId;
                                        RelatedNodeName = Node.NodeName;
                                    }
                                }
                            }
                            if( View.Root.ChildRelationships.Count > 0 )
                            {
                                ParentNode = View.Root.ChildRelationships[0];
                            }
                        }
                        foreach( JProperty AddNodeType in ParentNode.AllowedChildNodeTypes( LimitToFirstLevelRelationships )
                            .Select( Entry => new JProperty( Entry.NodeType.NodeTypeName,
                                                             makeAddMenuItem( Entry.NodeType, RelatedNodeId, RelatedNodeName ) ) ) )
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
                    if(
                        _MenuItems.Contains( "Copy" ) &&
                        false == ReadOnly &&
                        null != Node && Node.NodeSpecies == CswEnumNbtNodeSpecies.Plain &&
                        View.ViewMode != CswEnumNbtViewRenderingMode.Grid &&
                        _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, Node.getNodeType() ) &&
                        Node.getObjectClass().CanAdd //If you can't Add the node, you can't Copy it either
                        )
                    {
                        string BadPropertyName = string.Empty;
                        if( false == Node.getNodeType().IsUniqueAndRequired( ref BadPropertyName ) )
                        {
                            MoreObj["Copy"] = new JObject();
                            MoreObj["Copy"]["copytype"] = _getActionType( Node.getNodeType() );
                            MoreObj["Copy"]["action"] = CswEnumNbtMainMenuActions.CopyNode.ToString();
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
                        View.ViewMode != CswEnumNbtViewRenderingMode.Grid &&
                        Node.NodeSpecies == CswEnumNbtNodeSpecies.Plain &&
                        _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.Delete, Node.getNodeType(), Node.NodeId ) )
                    {
                        MoreObj["Delete"] = new JObject();
                        MoreObj["Delete"]["action"] = CswEnumNbtMainMenuActions.DeleteNode.ToString();
                        MoreObj["Delete"]["nodeid"] = Node.NodeId.ToString();
                        MoreObj["Delete"]["nodename"] = Node.NodeName;
                    }

                    // SAVE VIEW AS
                    if( _MenuItems.Contains( "Save View As" ) &&
                        false == View.ViewId.isSet() &&
                        _CswNbtResources.Permit.can( _CswNbtResources.Actions[CswEnumNbtActionName.Edit_View] ) )
                    {
                        View.SaveToCache( false );
                        MoreObj["Save View As"] = new JObject();
                        MoreObj["Save View As"]["action"] = CswEnumNbtMainMenuActions.SaveViewAs.ToString();
                        MoreObj["Save View As"]["viewid"] = View.SessionViewId.ToString();
                        MoreObj["Save View As"]["viewmode"] = View.ViewMode.ToString();
                    }

                    JObject PrintObj = null;

                    // PRINT LABEL

                    bool ValidForTreePrint = ( false == string.IsNullOrEmpty( SafeNodeKey ) &&
                                               View.ViewMode != CswEnumNbtViewRenderingMode.Grid &&                        
                                               null != Node &&
                                               null != Node.getNodeType() &&
                                               Node.getNodeType().HasLabel );

                    bool ValidForGridPrint = false;
                    bool TryValidForGridPrint = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid );
                    Int32 MultiPrintNodeTypeId = Int32.MinValue;
                    if( TryValidForGridPrint )
                    {
                        CswNbtViewRelationship TryRel = null;
                        if( View.Visibility != CswEnumNbtViewVisibility.Property && View.Root.ChildRelationships.Count == 1 )
                        {
                            TryRel = View.Root.ChildRelationships[0];
                        }
                        else if( View.Visibility == CswEnumNbtViewVisibility.Property &&
                                View.Root.ChildRelationships.Count == 1 &&
                                View.Root.ChildRelationships[0].ChildRelationships.Count == 1 )
                        {
                            TryRel = View.Root.ChildRelationships[0].ChildRelationships[0];
                        }

                        if( null != TryRel )
                        {
                            ICswNbtMetaDataDefinitionObject MdDef = TryRel.SecondMetaDataDefinitionObject();
                            if( null != MdDef )
                            {
                                if( MdDef.HasLabel )
                                {
                                    //This assumes that only NodeTypes will implement this property
                                    MultiPrintNodeTypeId = MdDef.UniqueId;
                                    ValidForGridPrint = true;
                                }
                            }
                        }
                    }
                    
                    if( _MenuItems.Contains( "Print" ) &&
                        ( ValidForTreePrint || ValidForGridPrint ) )
                    {
                        PrintObj = PrintObj ?? new JObject( new JProperty( "haschildren", true ) );
                        PrintObj["Print Label"] = new JObject();
                        PrintObj["Print Label"]["action"] = CswEnumNbtMainMenuActions.PrintLabel.ToString();

                        if( ValidForTreePrint )
                        {
                            PrintObj["Print Label"]["nodeid"] = Node.NodeId.ToString();
                            PrintObj["Print Label"]["nodetypeid"] = Node.NodeTypeId;
                            PrintObj["Print Label"]["nodename"] = Node.NodeName;
                        }
                        else if( ValidForGridPrint )
                        {
                            PrintObj["Print Label"]["nodetypeid"] = MultiPrintNodeTypeId;
                        }
                    }
                    // PRINT
                    if( _MenuItems.Contains( "Print" ) &&
                        View.ViewMode == CswEnumNbtViewRenderingMode.Grid )
                    {
                        View.SaveToCache( false );
                        PrintObj = PrintObj ?? new JObject( new JProperty( "haschildren", true ) );
                        PrintObj["Print View"] = new JObject();
                        PrintObj["Print View"]["action"] = CswEnumNbtMainMenuActions.PrintView.ToString();
                    }

                    if( null != PrintObj )
                    {
                        MoreObj["Print"] = PrintObj;
                    }

                    // EXPORT
                    if( _MenuItems.Contains( "Export" ) )
                    {
                        if( CswEnumNbtViewRenderingMode.Grid == View.ViewMode )
                        {
                            JObject ExportObj = new JObject();
                            MoreObj["Export"] = ExportObj;

                            View.SaveToCache( false );
                            ExportObj["CSV"] = new JObject();
                            string ExportLink = "wsNBT.asmx/gridExportCSV?ViewId=" + View.SessionViewId + "&SafeNodeKey='";
                            if( CswEnumNbtViewVisibility.Property == View.Visibility )
                            {
                                ExportLink += SafeNodeKey;
                            }
                            ExportLink += "'";

                            ExportObj["CSV"]["popup"] = ExportLink;

                            ExportObj["haschildren"] = true;
                        }
                    }
                } // if( null != View )

                // EDIT VIEW
                if( _MenuItems.Contains( "Edit View" ) &&
                    _CswNbtResources.Permit.can( CswEnumNbtActionName.Edit_View ) &&
                    ( null == View || ( false == View.IsSystem || CswNbtObjClassUser.ChemSWAdminUsername == _CswNbtResources.CurrentNbtUser.Username ) ) )
                {
                    MoreObj["Edit View"] = new JObject();
                    MoreObj["Edit View"]["action"] = CswEnumNbtMainMenuActions.editview.ToString();
                }

                if( _MenuItems.Contains( "Multi-Edit" ) &&
                    false == ReadOnly &&
                    null != View &&
                    _CswNbtResources.Permit.can( CswEnumNbtActionName.Multi_Edit ) &&
                    // Per discussion with David, for the short term eliminate the need to validate the selection of nodes across different nodetypes in Grid views.
                    // Case 21701: for Grid Properties, we need to look one level deeper
                    // Case 29032: furthermore (for Grids), we need to exclude ObjectClass relationships (which can also produce the multi-nodetype no-no
                    ( View.ViewMode != CswEnumNbtViewRenderingMode.Grid ||
                    ( ( View.Root.ChildRelationships.Count == 1 && View.Root.ChildRelationships[0].SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId ) &&
                    ( View.Visibility != CswEnumNbtViewVisibility.Property || ( View.Root.ChildRelationships[0].ChildRelationships.Count == 1 && View.Root.ChildRelationships[0].ChildRelationships[0].SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId ) ) ) )
                    )
                {
                    MoreObj["Multi-Edit"] = new JObject();
                    MoreObj["Multi-Edit"]["action"] = CswEnumNbtMainMenuActions.multiedit.ToString();
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

        public static JObject makeAddMenuItem( CswNbtMetaDataNodeType NodeType, CswPrimaryKey RelatedNodeId, string RelatedNodeName )
        {
            JObject Ret = new JObject();
            Ret["text"] = default( string );
            Ret["nodetypeid"] = default( string );
            if( null != NodeType )
            {
                Ret["text"] = NodeType.NodeTypeName;
                Ret["nodetypeid"] = NodeType.NodeTypeId;
                Ret["icon"] = CswNbtMetaDataObjectClass.IconPrefix16 + NodeType.IconFileName;
                Ret["action"] = _getActionType( NodeType );
                if( String.IsNullOrEmpty( Ret["action"].ToString() ) )
                {
                    Ret["relatednodeid"] = default( string );
                    if( null != RelatedNodeId && Int32.MinValue != RelatedNodeId.PrimaryKey )
                    {
                        Ret["relatednodeid"] = RelatedNodeId.ToString();
                    }
                    Ret["relatednodename"] = RelatedNodeName;
                    Ret["action"] = CswEnumNbtMainMenuActions.AddNode.ToString();
                }
            }
            return Ret;
        } // makeAddMenuItem()

        private static string _getActionType( CswNbtMetaDataNodeType NodeType )
        {
            string ActionType;
            switch( NodeType.getObjectClass().ObjectClass )
            {
                //Not yet an elegant way to handle Receiving from Add menu
                //case NbtObjectClass.ContainerClass:
                //    Ret["action"] = CswNbtActionName.Receiving.ToString();
                //    break;
                case CswEnumNbtObjectClass.ChemicalClass:
                case CswEnumNbtObjectClass.NonChemicalClass:
                    ActionType = CswEnumNbtActionName.Create_Material;
                    break;
                default:
                    ActionType = String.Empty;
                    break;
            }
            return ActionType;
        }

    } // class CswNbtWebServiceMainMenu
} // namespace ChemSW.Nbt.WebServices

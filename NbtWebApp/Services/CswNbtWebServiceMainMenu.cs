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

        /// <summary>
        /// Supported Export formats
        /// </summary>
        //public enum ExportOutputFormat
        //{
        //    CSV,
        //    Excel,
        //    PDF,
        //    Word,
        //    MobileXML,
        //    ReportXML
        //}

        private readonly CswCommaDelimitedString _MenuItems = new CswCommaDelimitedString()
                                                        {
                                                            //"Search",
                                                            "Add",
                                                            "Copy",
                                                            "Delete",
                                                            "Save View As",
                                                            "Print Label",
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

        public JObject getMenu( CswNbtView View, string SafeNodeKey, string PropIdAttr )
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

            //// SEARCH
            //if( _MenuItems.Contains( "Search" ) )
            //{
            //    JObject SearchObj = new JObject();
            //    bool HasChildren = false;
            //    if( View != null )
            //    {
            //        if( View.IsSearchable() )
            //        {
            //            View.SaveToCache( false );
            //            SearchObj["This View"] = new JObject();
            //            SearchObj["This View"]["text"] = "This View";
            //            SearchObj["This View"]["nodeid"] = NodeId;
            //            SearchObj["This View"]["nodetypeid"] = NodeTypeId;
            //            SearchObj["This View"]["sessionviewid"] = View.SessionViewId.ToString();
            //            SearchObj["This View"]["action"] = MenuActions.ViewSearch.ToString();
            //            HasChildren = true;
            //        }
            //        //if( View.Visibility != NbtViewVisibility.Property )
            //        //{
            //        //    SearchObj["Generic Search"] = new JObject();
            //        //    SearchObj["Generic Search"]["nodeid"] = NodeId;
            //        //    SearchObj["Generic Search"]["nodetypeid"] = NodeTypeId;
            //        //    SearchObj["Generic Search"]["action"] = MenuActions.GenericSearch.ToString();
            //        //    HasChildren = true;
            //        //}
            //        /* Case 24744: No Generic Search on Grid Props */
            //    }
            //    //else
            //    //{
            //    //    SearchObj["Generic Search"] = new JObject();
            //    //    SearchObj["Generic Search"]["action"] = MenuActions.GenericSearch.ToString();
            //    //    HasChildren = true;
            //    //}

            //    if( HasChildren )
            //    {
            //        SearchObj["haschildren"] = true;
            //        Ret["Search"] = SearchObj;
            //    }

            //}

            if( View != null )
            {
                // ADD
                if( _MenuItems.Contains( "Add" ) )
                {
                    JObject AddObj = new JObject();

                    // case 21672
                    CswNbtViewNode ParentNode = View.Root;
                    bool LimitToFirstGeneration = ( View.ViewMode == NbtViewRenderingMode.Grid );
                    if( LimitToFirstGeneration && View.Visibility == NbtViewVisibility.Property )
                    {
                        ParentNode = View.Root.ChildRelationships[0];
                    }
                    foreach( JProperty AddNodeType in ParentNode.AllowedChildNodeTypes( LimitToFirstGeneration )
                        .Select( Entry => new JProperty( Entry.NodeType.NodeTypeName,
                                                         makeAddMenuItem( Entry, RelatedNodeId, RelatedNodeName, RelatedNodeTypeId, RelatedObjectClassId ) ) ) )
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
                    null != Node && Node.NodeSpecies == NodeSpecies.Plain &&
                    View.ViewMode != NbtViewRenderingMode.Grid &&
                    _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, Node.getNodeType() ) )
                {
                    string BadPropertyName = string.Empty;
                    if( false == Node.getNodeType().IsUniqueAndRequired( ref BadPropertyName ) )
                    {
                        Ret["Copy"] = new JObject();
                        Ret["Copy"]["action"] = MenuActions.CopyNode.ToString();
                        Ret["Copy"]["nodeid"] = Node.NodeId.ToString();
                        Ret["Copy"]["nodename"] = Node.NodeName;
                        Ret["Copy"]["nodetypeid"] = Node.NodeTypeId.ToString();
                    }
                }

                // DELETE
                if( _MenuItems.Contains( "Delete" ) &&
                    false == string.IsNullOrEmpty( SafeNodeKey ) &&
                    null != Node &&
                    View.ViewMode != NbtViewRenderingMode.Grid &&
                    Node.NodeSpecies == NodeSpecies.Plain &&
                    _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, Node.getNodeType(), false, null, null, Node.NodeId, null ) )
                {
                    Ret["Delete"] = new JObject();
                    Ret["Delete"]["action"] = MenuActions.DeleteNode.ToString();
                    Ret["Delete"]["nodeid"] = Node.NodeId.ToString();
                    Ret["Delete"]["nodename"] = Node.NodeName;
                }

                // SAVE VIEW AS
                if( _MenuItems.Contains( "Save View As" ) &&
                    false == View.ViewId.isSet() &&
                    _CswNbtResources.Permit.can( _CswNbtResources.Actions[CswNbtActionName.Edit_View] ) )
                {
                    View.SaveToCache( false );
                    Ret["Save View As"] = new JObject();
                    Ret["Save View As"]["action"] = MenuActions.SaveViewAs.ToString();
                    Ret["Save View As"]["viewid"] = View.SessionViewId.ToString();
                    Ret["Save View As"]["viewmode"] = View.ViewMode.ToString();
                }

                // PRINT LABEL
                if( _MenuItems.Contains( "Print Label" ) &&
                    false == string.IsNullOrEmpty( SafeNodeKey ) &&
                    null != Node &&
                    null != Node.getNodeType() )
                {
                    CswNbtMetaDataNodeTypeProp BarcodeProperty = Node.getNodeType().getBarcodeProperty();
                    if( null != BarcodeProperty )
                    {
                        Ret["Print Label"] = new JObject();
                        Ret["Print Label"]["nodeid"] = Node.NodeId.ToString();
                        Ret["Print Label"]["propid"] = new CswPropIdAttr( Node, BarcodeProperty ).ToString();
                        Ret["Print Label"]["action"] = MenuActions.PrintLabel.ToString();
                    }
                }
                // PRINT
                if( _MenuItems.Contains( "Print" ) &&
                    View.ViewMode == NbtViewRenderingMode.Grid )
                {
                    View.SaveToCache( false );
                    Ret["Print"] = new JObject();
                    Ret["Print"]["action"] = MenuActions.PrintView.ToString();
                }

                // EXPORT
                if( _MenuItems.Contains( "Export" ) )
                {
                    if( NbtViewRenderingMode.Grid == View.ViewMode )
                    {
                        JObject ExportObj = new JObject();
                        Ret["Export"] = ExportObj;

                        View.SaveToCache( false );
                        ExportObj["CSV"] = new JObject();
                        //ExportObj["CSV"]["action"] = MenuActions.Webservice.ToString();
                        ExportObj["CSV"]["popup"] = "wsNBT.asmx/gridExportCSV?ViewId=" + View.SessionViewId.ToString();

                        ExportObj["haschildren"] = true;


                        //View.SaveToCache( false );
                        //string Url = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString();
                        //if( View.Visibility == NbtViewVisibility.Property &&
                        //    null != Node &&
                        //    false == string.IsNullOrEmpty( PropIdAttr ) )
                        //{
                        //    // Grid Property is a special case
                        //    CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                        //    Url = "Popup_Export.aspx?nodeid=" + Node.NodeId.ToString() + "&propid=" + PropId.NodeTypePropId;
                        //}

                        //foreach( ExportOutputFormat FormatType in Enum.GetValues( typeof( ExportOutputFormat ) )
                        //    .Cast<ExportOutputFormat>()
                        //    .Where( FormatType => ExportOutputFormat.MobileXML != FormatType || 
                        //            _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) ) )
                        //{
                        //    ExportObj[FormatType.ToString()] = new JObject();
                        //    ExportObj[FormatType.ToString()]["popup"] = Url + "&format=" + FormatType.ToString().ToLower() + "&renderingmode=" + View.ViewMode;
                        //}
                        //if( ExportObj.HasValues )
                        //{
                        //    ExportObj["haschildren"] = true;
                        //}
                    }
                    //else // tree or list
                    //{
                    //ExportObj["haschildren"] = true;
                    //ExportObj["Report XML"] = "";
                    //if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                    //{
                    //    if( null == View.SessionViewId )
                    //    {
                    //        View.SaveToCache( false, false );
                    //    }
                    //    string PopUp = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString() + "&format=" +
                    //                   ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode;
                    //    ExportObj["Mobile XML"] = new JObject();
                    //    ExportObj["Mobile XML"]["popup"] = PopUp;
                    //}
                    //}
                }
            } // if( null != View )

            // EDIT VIEW
            if( _MenuItems.Contains( "Edit View" ) &&
                _CswNbtResources.Permit.can( CswNbtActionName.Edit_View ) )
            {
                Ret["Edit View"] = new JObject();
                Ret["Edit View"]["action"] = MenuActions.editview.ToString();
            }

            if( _MenuItems.Contains( "Multi-Edit" ) &&
                null != View &&
                _CswNbtResources.Permit.can( CswNbtActionName.Multi_Edit ) &&
                ( View.ViewMode != NbtViewRenderingMode.Grid ||
                /* Per discussion with David, for the short term eliminate the need to validate the selection of nodes across different nodetypes in Grid views. */
                  View.Root.ChildRelationships.Count == 1 && View.Root.ChildRelationships[0].ChildRelationships.Count == 0 )
                )
            {
                Ret["Multi-Edit"] = new JObject();
                Ret["Multi-Edit"]["action"] = MenuActions.multiedit.ToString();
            }

            _CswNbtResources.logTimerResult( "CswNbtWebServiceMainMenu.getMenu()", MainMenuTimer.ElapsedDurationInSecondsAsString );


            return Ret;
        } // getMenu()

        public static JObject makeAddMenuItem( CswNbtViewNode.CswNbtViewAddNodeTypeEntry Entry, CswPrimaryKey RelatedNodeId, string RelatedNodeName, string RelatedNodeTypeId, string RelatedObjectClassId )
        {
            JObject Ret = new JObject();
            Ret["text"] = default( string );
            Ret["nodetypeid"] = default( string );
            if( null != Entry.NodeType )
            {
                Ret["text"] = Entry.NodeType.NodeTypeName;
                Ret["nodetypeid"] = Entry.NodeType.NodeTypeId;
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

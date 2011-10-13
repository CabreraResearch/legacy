using System;
using System.Linq;
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
        public enum ExportOutputFormat
        {
            CSV,
            Excel,
            PDF,
            Word,
            MobileXML,
            ReportXML
        }

        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceMainMenu( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public JObject getMenu( CswNbtView View, string SafeNodeKey, string PropIdAttr )
        {
            JObject Ret = new JObject();

            string NodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
            string NodeIdNum = string.Empty;
            CswNbtNode Node = null;
            Int32 NodeTypeId = Int32.MinValue;
            Int32 NodeId = Int32.MinValue;

            if( !string.IsNullOrEmpty( NodeKey ) )
            {
                CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
                Node = _CswNbtResources.Nodes[NbtNodeKey];
                NodeId = Node.NodeId.PrimaryKey;
                NodeTypeId = Node.NodeTypeId;
                NodeIdNum = NodeId.ToString();
            }

            // SEARCH

            Ret["Search"] = new JObject();
            Ret["Search"]["haschildren"] = true;
            // Generic search
            if( View != null )
            {
                if( View.IsSearchable() )
                {
                    View.SaveToCache( false );
                    Ret["Search"]["This View"] = new JObject();
                    Ret["Search"]["This View"]["text"] = "This View";
                    Ret["Search"]["This View"]["nodeid"] = NodeId;
                    Ret["Search"]["This View"]["nodetypeid"] = NodeTypeId;
                    Ret["Search"]["This View"]["sessionviewid"] = View.SessionViewId.ToString();
                    Ret["Search"]["This View"]["action"] = "ViewSearch";
                }
                if( View.Visibility != NbtViewVisibility.Property )
                {
                    Ret["Search"]["Generic Search"] = new JObject();
                    Ret["Search"]["Generic Search"]["nodeid"] = NodeId;
                    Ret["Search"]["Generic Search"]["nodetypeid"] = NodeTypeId;
                    Ret["Search"]["Generic Search"]["action"] = "GenericSearch";
                }
                else
                {
                    Ret["Search"]["Generic Search"] = new JObject();
                    Ret["Search"]["Generic Search"]["action"] = "GenericSearch";
                }

            }
            else
            {
                Ret["Search"]["Generic Search"] = new JObject();
                Ret["Search"]["Generic Search"]["action"] = "GenericSearch";
            }

            if( View != null )
            {
                // ADD
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
                                                     new JObject(
                                                         new JProperty( "text", Entry.NodeType.NodeTypeName ),
                                                         new JProperty( "nodetypeid", Entry.NodeType.NodeTypeId ),
                                                         new JProperty( "relatednodeid", NodeIdNum ), //for Grid Props
                                                         new JProperty( "action", "AddNode" ) ) ) ) )
                {
                    AddObj.Add( AddNodeType );
                }

                if( AddObj.HasValues )
                {
                    AddObj["haschildren"] = true;
                    Ret["Add"] = AddObj;
                }

                // COPY
                if( null != Node && Node.NodeSpecies == NodeSpecies.Plain &&
                    View.ViewMode != NbtViewRenderingMode.Grid &&
                    _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Create, Node.NodeType ) )
                {
                    string BadPropertyName = string.Empty;
                    if( false == Node.NodeType.IsUniqueAndRequired( ref BadPropertyName ) )
                    {
                        Ret["Copy"] = new JObject();
                        Ret["Copy"]["action"] = "CopyNode";
                        Ret["Copy"]["nodeid"] = Node.NodeId.ToString();
                        Ret["Copy"]["nodename"] = Node.NodeName;
                    }
                }

                // DELETE
                if( false == string.IsNullOrEmpty( NodeKey ) &&
                    null != Node &&
                    View.ViewMode != NbtViewRenderingMode.Grid &&
                    Node.NodeSpecies == NodeSpecies.Plain &&
                    _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, Node.NodeType, false, null, null, Node, null ) )
                {
                    Ret["Delete"] = new JObject();
                    Ret["Delete"]["action"] = "DeleteNode";
                    Ret["Delete"]["nodeid"] = Node.NodeId.ToString();
                    Ret["Delete"]["nodename"] = Node.NodeName;
                }

                // SAVE VIEW AS
                if( false == View.ViewId.isSet() && _CswNbtResources.Permit.can( _CswNbtResources.Actions[CswNbtActionName.Edit_View] ) )
                {
                    View.SaveToCache( false );
                    Ret["Save View As"] = new JObject();
                    Ret["Save View As"]["action"] = "SaveViewAs";
                    Ret["Save View As"]["viewid"] = View.SessionViewId.ToString();
                    Ret["Save View As"]["viewmode"] = View.ViewMode.ToString();
                }

                // PRINT LABEL
                if( false == string.IsNullOrEmpty( NodeKey ) && null != Node && null != Node.NodeType )
                {
                    CswNbtMetaDataNodeTypeProp BarcodeProperty = Node.NodeType.BarcodeProperty;
                    if( null != BarcodeProperty )
                    {
                        Ret["Print Label"] = new JObject();
                        Ret["Print Label"]["nodeid"] = Node.NodeId.ToString();
                        Ret["Print Label"]["propid"] = new CswPropIdAttr( Node, BarcodeProperty ).ToString();
                        Ret["Print Label"]["action"] = "PrintLabel";
                    }
                }
                // PRINT
                if( View.ViewMode == NbtViewRenderingMode.Grid )
                {
                    View.SaveToCache( false );
                    Ret["Print"] = new JObject();
                    Ret["Print"]["popup"] = "PrintGrid.aspx?sessionviewid=" + View.SessionViewId.ToString();
                }

                // EXPORT
                JObject ExportObj = new JObject();
                Ret["Export"] = ExportObj;

                if( NbtViewRenderingMode.Grid == View.ViewMode )
                {
                    string Url = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString();
                    if( View.Visibility == NbtViewVisibility.Property &&
                        null != Node &&
                        false == string.IsNullOrEmpty( PropIdAttr ) )
                    {
                        // Grid Property is a special case
                        CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                        Url = "Popup_Export.aspx?nodeid=" + Node.NodeId.ToString() + "&propid=" + PropId.NodeTypePropId;
                    }

                    foreach( ExportOutputFormat FormatType in Enum.GetValues( typeof( ExportOutputFormat ) )
                                                                  .Cast<ExportOutputFormat>()
                                                                  .Where( FormatType => ExportOutputFormat.MobileXML != FormatType || _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) ) )
                    {
                        ExportObj[FormatType] = new JObject();
                        ExportObj[FormatType]["popup"] = Url + "&format=" + FormatType.ToString().ToLower() + "&renderingmode=" + View.ViewMode;
                    }
                    if( ExportObj.HasValues )
                    {
                        ExportObj["haschildren"] = true;
                    }
                }
                else // tree or list
                {
                    ExportObj["haschildren"] = true;
                    ExportObj["Report XML"] = "";
                    if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                    {
                        if( null == View.SessionViewId )
                        {
                            View.SaveToCache( false, false );
                        }
                        string PopUp = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString() + "&format=" +
                                       ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode;
                        ExportObj["Mobile XML"] = new JObject();
                        ExportObj["Mobile XML"]["popup"] = PopUp;
                    }
                }
            } // if( null != View )

            // EDIT VIEW
            if( _CswNbtResources.Permit.can( CswNbtActionName.Edit_View ) )
            {
                Ret["Edit View"] = new JObject();
                Ret["Edit View"]["action"] = "editview";
            }

            if( null != View )
            {
                Ret["Multi-Edit"] = new JObject();
                Ret["Multi-Edit"]["action"] = "multiedit";
            }

            return Ret;
        } // getMenu()
    } // class CswNbtWebServiceMainMenu
} // namespace ChemSW.Nbt.WebServices

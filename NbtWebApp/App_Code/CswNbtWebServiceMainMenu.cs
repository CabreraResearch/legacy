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

            JObject SearchObj = new JObject(
                new JProperty( "haschildren", true )
                );
            Ret["Search"] = SearchObj;

            // Generic search
            if( View != null )
            {
                if( View.Visibility != NbtViewVisibility.Property )
                {
                    SearchObj.Add( new JProperty( "Generic Search",
                                                  new JObject(
                                                      new JProperty( "nodeid", NodeId ),
                                                      new JProperty( "nodetypeid", NodeTypeId ),
                                                      new JProperty( "action", "GenericSearch" )
                                                      ) ) );
                }
                else
                {
                    SearchObj.Add( new JProperty( "Generic Search",
                                                  new JObject(
                                                      new JProperty( "action", "GenericSearch" )
                                                      ) ) );
                }
                if( View.IsSearchable() )
                {
                    View.SaveToCache( false );
                    SearchObj.AddFirst( new JProperty( "This View",
                                                       new JObject(
                                                           new JProperty( "text", "This View" ),
                                                           new JProperty( "nodeid", NodeId ),
                                                           new JProperty( "nodetypeid", NodeTypeId ),
                                                           new JProperty( "sessionviewid", View.SessionViewId.ToString() ),
                                                           new JProperty( "action", "ViewSearch" )
                                                           ) ) );
                }
            }
            else
            {
                SearchObj.Add( new JProperty( "Generic Search",
                                              new JObject(
                                                  new JProperty( "action", "GenericSearch" )
                                                  ) ) );
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
                    AddObj.Add( new JProperty( "haschildren", true ) );
                    Ret["Add"] = AddObj;
                }

                // COPY
                if( null != Node && Node.NodeSpecies == NodeSpecies.Plain &&
                    View.ViewMode != NbtViewRenderingMode.Grid &&
                    _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Create, Node.NodeType ) )
                {
                    string BadPropertyName = string.Empty;
                    if( !Node.NodeType.IsUniqueAndRequired( ref BadPropertyName ) )
                    {
                        Ret["Copy"] = new JObject(
                            new JProperty( "action", "CopyNode" ),
                            new JProperty( "nodeid", Node.NodeId.ToString() ),
                            new JProperty( "nodename", Node.NodeName )
                            );
                    }
                }

                // DELETE
                if( !string.IsNullOrEmpty( NodeKey ) &&
                    null != Node &&
                    View.ViewMode != NbtViewRenderingMode.Grid &&
                    Node.NodeSpecies == NodeSpecies.Plain &&
                    _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, Node.NodeType, false, null, null, Node, null ) )
                {

                    Ret["Delete"] = new JObject(
                        new JProperty( "action", "DeleteNode" ),
                        new JProperty( "nodeid", Node.NodeId.ToString() ),
                        new JProperty( "nodename", Node.NodeName )
                        );
                }

                // SAVE VIEW AS
                if( !View.ViewId.isSet() && _CswNbtResources.Permit.can( _CswNbtResources.Actions[CswNbtActionName.Edit_View] ) )
                {
                    View.SaveToCache( false );
                    Ret["Save View As"] = new JObject(
                        new JProperty( "action", "SaveViewAs" ),
                        new JProperty( "viewid", View.SessionViewId.ToString() ),
                        new JProperty( "viewmode", View.ViewMode.ToString() )
                        );
                }

                // PRINT LABEL
                if( !string.IsNullOrEmpty( NodeKey ) && null != Node && Node.NodeType != null )
                {
                    CswNbtMetaDataNodeTypeProp BarcodeProperty = Node.NodeType.BarcodeProperty;
                    if( BarcodeProperty != null )
                    {
                        Ret["Print Label"] = new JObject(
                            new JProperty( "nodeid", Node.NodeId.ToString() ),
                            new JProperty( "propid", new CswPropIdAttr( Node, BarcodeProperty ).ToString() ),
                            new JProperty( "action", "PrintLabel" )
                            );

                    }
                }
                // PRINT
                if( View.ViewMode == NbtViewRenderingMode.Grid )
                {
                    View.SaveToCache( false );
                    Ret["Print"] = new JObject(
                        new JProperty( "popup", "PrintGrid.aspx?sessionviewid=" + View.SessionViewId.ToString() )
                        );
                }

                // EXPORT
                JObject ExportObj = new JObject();
                Ret["Export"] = ExportObj;

                if( NbtViewRenderingMode.Grid == View.ViewMode )
                {
                    string Url = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString();
                    if( View.Visibility == NbtViewVisibility.Property &&
                        null != Node &&
                        string.Empty != PropIdAttr )
                    {
                        // Grid Property is a special case
                        CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                        Url = "Popup_Export.aspx?nodeid=" + Node.NodeId.ToString() + "&propid=" + PropId.NodeTypePropId;
                    }

                    foreach( JProperty ExportType in from ExportOutputFormat FormatType
                                                         in Enum.GetValues( typeof( ExportOutputFormat ) )
                                                     where ExportOutputFormat.MobileXML != FormatType || _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile )
                                                     select new JProperty( FormatType.ToString(),
                                                                           new JObject(
                                                                               new JProperty( "popup", Url + "&format=" + FormatType.ToString().ToLower() + "&renderingmode=" + View.ViewMode ) ) ) )
                    {
                        ExportObj.Add( ExportType );
                    }
                    if( ExportObj.HasValues )
                    {
                        ExportObj.Add( new JProperty( "haschildren", true ) );
                    }
                }
                else // tree or list
                {
                    ExportObj.Add( new JProperty( "haschildren", true ) );
                    ExportObj.Add( new JProperty( "Report XML" ) );
                    if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                    {
                        string PopUp = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString() + "&format=" +
                                       ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode;
                        ExportObj.Add( new JProperty( "Mobile XML",
                                                      new JObject(
                                                          new JProperty( "popup", PopUp ) )
                                           ) );
                    }
                }
            } // if( null != View )

            // EDIT VIEW
            if( _CswNbtResources.Permit.can( CswNbtActionName.Edit_View ) )
            {
                Ret["Edit View"] = new JObject( new JProperty( "action", "editview" ) );
            }

            if( null != View )
            {
                Ret["Multi-Edit"] = new JObject( new JProperty( "action", "multiedit" ) );
            }

            return Ret;
        } // getMenu()
    } // class CswNbtWebServiceMainMenu
} // namespace ChemSW.Nbt.WebServices

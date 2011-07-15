using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMobileView
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly bool _ForMobile;
        private readonly Int32 MobilePageSize = 30;

        public CswNbtWebServiceMobileView( CswNbtResources CswNbtResources, bool ForMobile )
        {
            _CswNbtResources = CswNbtResources;
            _ForMobile = ForMobile;
            string PageSize = _CswNbtResources.getConfigVariableValue( CswNbtResources.ConfigurationVariables.mobileview_resultlimit.ToString() );
            if( CswTools.IsInteger( PageSize ) )
            {
                MobilePageSize = CswConvert.ToInt32( PageSize );
            }
        }

        private const string PropIdPrefix = "prop_";
        private const string NodeIdPrefix = "nodeid_";


        public JObject getViewsList( string ParentId, ICswNbtUser CurrentUser )
        {
            // All Views
            JObject RetJson = new JObject();
            Collection<CswNbtView> MobileViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, CurrentUser, false, _ForMobile, false, NbtViewRenderingMode.Any );
            foreach( CswNbtView MobileView in MobileViews )
            {
                RetJson.Add( new JProperty( MobileView.ViewId.ToString(), MobileView.ViewName ) );
            }

            return RetJson;
        } // Run()

        public JObject getView( string ViewId, ICswNbtUser CurrentUser )
        {
            JObject RetJson = new JObject();

            // Get the full XML for the entire view
            CswNbtViewId NbtViewId = new CswNbtViewId( ViewId );
            CswNbtView View = _CswNbtResources.ViewSelect.restoreView( NbtViewId );

            // case 20083
            if( _ForMobile )
            {
                RetJson.Add( _getSearchNodes( View ) );
            }

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false, MobilePageSize );

            if( Tree.getChildNodeCount() > 0 )
            {
                JProperty ParentNode = new JProperty( "nodes" );
                JObject Nodes = new JObject();
                ParentNode.Value = Nodes;
                _runTreeNodesRecursive( Tree, ref Nodes );
                RetJson.Add( ParentNode );
            }
            else
            {
                RetJson.Add( new JProperty( "nodes",
                                new JObject(
                                    new JProperty( NodeIdPrefix + Int32.MinValue, "No Results" ) ) ) );
            }

            return RetJson;
        } // Run()

        // case 20083 - search options
        private JProperty _getSearchNodes( CswNbtView View )
        {
            JProperty ReturnJson = new JProperty( "searches",
                new JObject(
                    from CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes
                    where View.ContainsNodeType( NodeType )
                    from CswNbtMetaDataNodeTypeProp MetaDataProp in NodeType.NodeTypeProps
                    where MetaDataProp.MobileSearch
                    let PropId = ( MetaDataProp.ObjectClassProp != null ) ? "search_ocp_" + MetaDataProp.ObjectClassPropId : "search_ntp_" + MetaDataProp.PropId
                    select new JProperty( PropId, CswTools.SafeJavascriptParam( MetaDataProp.PropNameWithQuestionNo ) ) ) );
            return ReturnJson;
        } // _getSearchNodes


        private void _runTreeNodesRecursive( ICswNbtTree Tree, ref JObject ParentJsonO )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeId = ThisNode.NodeId.ToString();

                JProperty ThisJProp = new JProperty( "subitems" );
                JObject ThisSubItems = new JObject();
                ThisJProp.Value = ThisSubItems;

                _runTreeNodesRecursive( Tree, ref ThisSubItems );

                if( Tree.getNodeShowInTreeForCurrentPosition() )
                {
                    if( _ForMobile && Tree.getChildNodeCount() == 0 && NodeSpecies.More != ThisNodeKey.NodeSpecies )   // is a leaf
                    {
                        _runProperties( ThisNode, ref ThisSubItems );
                    }

                    JProperty NodeWrap = null;
                    JObject NodeProps = null;
                    if( _ForMobile )
                    {
                        if( NodeSpecies.More == ThisNodeKey.NodeSpecies )
                        {
                            ThisNodeName = "Results Truncated at " + MobilePageSize;
                        }
                        NodeWrap = new JProperty( NodeIdPrefix + ThisNodeId );
                        NodeProps = new JObject(
                            new JProperty( "name", CswTools.SafeJavascriptParam( ThisNodeName ) ),
                            new JProperty( "nodetype", CswTools.SafeJavascriptParam( ThisNode.NodeType.NodeTypeName ) ),
                            new JProperty( "objectclass", CswTools.SafeJavascriptParam( ThisNode.ObjectClass.ObjectClass.ToString() ) ),
                            new JProperty( "iconfilename", CswTools.SafeJavascriptParam( ThisNode.NodeType.IconFileName ) ),
                            new JProperty( "nodespecies", CswTools.SafeJavascriptParam( ThisNodeKey.NodeSpecies.ToString() ) ),
                            ThisJProp );
                        NodeWrap.Value = NodeProps;
                    }

                    // case 20083 - search values
                    if( null != NodeProps )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in ThisNode.NodeType.NodeTypeProps.Cast<CswNbtMetaDataNodeTypeProp>().Where( MetaDataProp => MetaDataProp.MobileSearch ) )
                        {
                            NodeProps.Add( ( MetaDataProp.ObjectClassProp != null ) ?
                                    new JProperty( "search_ocp_" + MetaDataProp.ObjectClassPropId,
                                        CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) ) :
                                    new JProperty( "search_ntp_" + MetaDataProp.PropId,
                                        CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) ) );
                        }
                    }
                    ParentJsonO.Add( NodeWrap );
                } // if( Tree.getNodeShowInTreeForCurrentPosition() )
                else
                {
                    ParentJsonO.Add( ThisSubItems );
                }
                Tree.goToParentNode();
            }
        } // _runTreeNodesRecursive()

        private static void _runProperties( CswNbtNode Node, ref JObject SubItemsJProp )
        {
            //JArray PropArray = new JArray();
            foreach( CswNbtMetaDataNodeTypeProp Prop in from CswNbtMetaDataNodeTypeTab Tab
                                                            in Node.NodeType.NodeTypeTabs
                                                        from CswNbtMetaDataNodeTypeProp Prop
                                                            in Tab.NodeTypePropsByDisplayOrder
                                                        where !Prop.HideInMobile &&
                                                            Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password &&
                                                            Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid
                                                        select Prop )
            {
                CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
                string ReadOnly = ( Node.ReadOnly || Prop.ReadOnly ) ? "true" : "false";
                JProperty ThisProp = new JProperty( PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() );
                JObject ThisPropAttr = new JObject(
                                            new JProperty( "name", CswTools.SafeJavascriptParam( Prop.PropNameWithQuestionNo ) ),
                                            new JProperty( "tab", CswTools.SafeJavascriptParam( Prop.NodeTypeTab.TabName ) ),
                                            new JProperty( "isreadonly", ReadOnly ),
                                            new JProperty( "fieldtype", Prop.FieldType.FieldType.ToString() ),
                                            new JProperty( "gestalt", CswTools.SafeJavascriptParam( PropWrapper.Gestalt ) ),
                                            new JProperty( "ocpname", CswTools.SafeJavascriptParam( PropWrapper.ObjectClassPropName ) )
                                       );

                PropWrapper.ToJSON( ThisPropAttr );
                ThisProp.Value = ThisPropAttr;
                SubItemsJProp.Add( ThisProp );
            }
        }

        // _runProperties()


    } // class CswNbtWebServiceMobileView

} // namespace ChemSW.Nbt.WebServices

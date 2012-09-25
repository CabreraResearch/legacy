using System;
using System.Collections.Generic;
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
    public class CswNbtWebServiceMobile
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly bool _ForMobile;
        private readonly Int32 _MobilePageSize = 30;

        public CswNbtWebServiceMobile( CswNbtResources CswNbtResources, bool ForMobile )
        {
            _CswNbtResources = CswNbtResources;
            _ForMobile = ForMobile;
            string PageSize = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.mobileview_resultlim.ToString() );
            if( CswTools.IsInteger( PageSize ) )
            {
                _MobilePageSize = CswConvert.ToInt32( PageSize );
            }
        }

        #region Get

        private const string PropIdPrefix = "prop_";
        private const string NodeIdPrefix = "nodeid_";


        public JObject getViewsList( string ParentId, ICswNbtUser CurrentUser )
        {
            // All Views
            JObject RetJson = new JObject();
            Dictionary<CswNbtViewId, CswNbtView> MobileViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, CurrentUser, false, _ForMobile, false, NbtViewRenderingMode.Any );
            if( MobileViews.Count == 0 )
            {
                RetJson["-1"] = "No Views Have Been Configured for Mobile.";
            }
            else
            {
                foreach( CswNbtView MobileView in MobileViews.Values )
                {
                    RetJson[MobileView.ViewId.ToString()] = MobileView.ViewName;
                }
            }
            return RetJson;
        } // Run()

        public JObject getView( string ViewId, ICswNbtUser CurrentUser )
        {
            JObject RetJson = new JObject();

            // Get the full XML for the entire view
            CswNbtViewId NbtViewId = new CswNbtViewId( ViewId );
            if( NbtViewId.isSet() )
            {
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( NbtViewId );

                // case 20083
                if( _ForMobile )
                {
                    RetJson.Add( _getSearchNodes( View ) );
                }

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false, _MobilePageSize );

                if( Tree.getChildNodeCount() > 0 )
                {
                    JObject NodesObj = new JObject();
                    RetJson["nodes"] = NodesObj;
                    _runTreeNodesRecursive( Tree, NodesObj );
                }
                else
                {
                    RetJson["nodes"] = new JObject();
                    RetJson["nodes"][NodeIdPrefix + Int32.MinValue] = "No Results";
                }
            }

            return RetJson;
        } // Run()

        // case 20083 - search options
        private JProperty _getSearchNodes( CswNbtView View )
        {
            JProperty ReturnJson = new JProperty( "searches",
                new JObject(
                    from CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion()
                    where View.ContainsNodeType( NodeType )
                    from CswNbtMetaDataNodeTypeProp MetaDataProp in NodeType.getNodeTypeProps()
                    where MetaDataProp.MobileSearch
                    let PropId = ( MetaDataProp.ObjectClassPropId != Int32.MinValue ) ? "search_ocp_" + MetaDataProp.ObjectClassPropId : "search_ntp_" + MetaDataProp.PropId
                    select new JProperty( PropId, CswTools.SafeJavascriptParam( MetaDataProp.PropNameWithQuestionNo ) ) ) );
            return ReturnJson;
        } // _getSearchNodes

        private void _runTreeNodesRecursive( ICswNbtTree Tree, JObject ParentJsonO )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();

                if( Tree.getNodeShowInTreeForCurrentPosition() )
                {
                    bool RunProps = ( _ForMobile && Tree.getChildNodeCount() == 0 && NodeSpecies.More != ThisNodeKey.NodeSpecies ); // is a leaf
                    JProperty ThisJProp = _getNode( ThisNode, RunProps, ThisNodeKey.NodeSpecies );
                    ParentJsonO.Add( ThisJProp );
                    if( Tree.getChildNodeCount() > 0 )
                    {
                        JObject ThisNodeObj = (JObject) ThisJProp.Value;
                        JObject NodesObj = new JObject();
                        ThisNodeObj["nodes"] = NodesObj;
                        _runTreeNodesRecursive( Tree, NodesObj );
                    }
                } // if( Tree.getNodeShowInTreeForCurrentPosition() )
                else
                {
                    _runTreeNodesRecursive( Tree, ParentJsonO );
                }
                Tree.goToParentNode();
            }
        } // _runTreeNodesRecursive()

        public JObject getNode( string NodeId )
        {
            CswDelimitedString NodeStr = new CswDelimitedString( '_' );
            NodeStr.FromString( NodeId );
            if( NodeStr[0] == "nodeid" )
            {
                NodeStr.RemoveAt( 0 );
            }
            string NodePk = NodeStr.ToString();
            JObject Ret = new JObject();
            Ret.Add( _getNode( NodePk, true ) );
            return Ret;
        }

        private JProperty _getNode( string NodePkStr, bool RunProps = true )
        {
            CswPrimaryKey NodePk = new CswPrimaryKey();
            NodePk.FromString( NodePkStr );
            CswNbtNode ThisNode = _CswNbtResources.Nodes.GetNode( NodePk );
            return _getNode( ThisNode, RunProps );
        }

        private JProperty _getNode( CswNbtNode ThisNode, bool RunProps = true, NodeSpecies ThisNodeSpecies = null )
        {
            JProperty Ret = new JProperty( "No Results" );
            if( null != ThisNode )
            {
                string ThisNodeName = ThisNode.NodeName;
                 
                Ret = new JProperty( NodeIdPrefix + ThisNode.NodeId );
                JObject NodeProps = new JObject();
                Ret.Value = NodeProps;

                NodeSpecies NodeSpecie = ( ThisNodeSpecies != NodeSpecies.UnKnown ) ? ThisNodeSpecies : ThisNode.NodeSpecies;
                if( NodeSpecies.More == NodeSpecie )
                {
                    ThisNodeName = "Results Truncated at " + _MobilePageSize;
                }

                NodeProps["node_name"] = CswTools.SafeJavascriptParam( ThisNodeName );
                NodeProps["nodetype"] = CswTools.SafeJavascriptParam( ThisNode.getNodeType().NodeTypeName );
                NodeProps["objectclass"] = CswTools.SafeJavascriptParam( ThisNode.getObjectClass().ObjectClass.ToString() );
                NodeProps["nodespecies"] = CswTools.SafeJavascriptParam( NodeSpecie.ToString() );
                if( RunProps )
                {
                    JObject TabsObj = new JObject();
                    NodeProps["tabs"] = TabsObj;
                    _runProperties( ThisNode, TabsObj );
                }

                if( ThisNode.Locked )
                {
                    NodeProps["iconfilename"] = "Images/quota/lock.gif";
                }
                else if( false == string.IsNullOrEmpty( ThisNode.IconFileName ) )
                {
                    NodeProps["iconfilename"] = CswNbtMetaDataObjectClass.IconPrefix16 + CswTools.SafeJavascriptParam( ThisNode.IconFileName );
                }

                _addObjectClassProps( ThisNode, NodeProps );

                foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in _CswNbtResources.MetaData.getNodeTypeProps( ThisNode.NodeTypeId )
                                                                            .Cast<CswNbtMetaDataNodeTypeProp>()
                                                                            .Where( MetaDataProp => MetaDataProp.MobileSearch ) )
                {
                    if( ( MetaDataProp.getObjectClassProp() != null ) )
                    {
                        NodeProps["search_ocp_" + MetaDataProp.ObjectClassPropId] = CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt );
                    }
                    else
                    {
                        NodeProps["search_ntp_" + MetaDataProp.PropId] = CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt );
                    }
                }
            }
            return Ret;
        }

        private static void _addObjectClassProps( CswNbtNode Node, JObject NodeProps )
        {
            switch( Node.getObjectClass().ObjectClass )
            {
                case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass:
                    {
                        NodeProps["location"] = Node.Properties[CswNbtObjClassInspectionDesign.PropertyName.Location].Gestalt;
                        NodeProps["duedate"] = Node.Properties[CswNbtObjClassInspectionDesign.PropertyName.Date].Gestalt;
                        NodeProps["status"] = Node.Properties[CswNbtObjClassInspectionDesign.PropertyName.Status].Gestalt;
                        NodeProps["target"] = Node.Properties[CswNbtObjClassInspectionDesign.PropertyName.Target].Gestalt;
                        break;
                    }
                case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass:
                    {
                        NodeProps["location"] = Node.Properties[CswNbtObjClassInspectionTarget.PropertyName.Location].Gestalt;
                        NodeProps["description"] = Node.Properties[CswNbtObjClassInspectionTarget.PropertyName.Description].Gestalt;
                        NodeProps["status"] = Node.Properties[CswNbtObjClassInspectionTarget.PropertyName.Status].Gestalt;
                        //NodeProps["lastinspectiondate"] = Node.Properties[CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName].Gestalt;
                        break;
                    }

            }
        }

        private void _runProperties( CswNbtNode Node, JObject SubItemsJProp )
        {
            Collection<CswNbtMetaDataNodeTypeTab> Tabs = new Collection<CswNbtMetaDataNodeTypeTab>();
            foreach( CswNbtMetaDataNodeTypeTab Tab in from _Tab in _CswNbtResources.MetaData.getNodeTypeTabs( Node.NodeTypeId )
                                                      orderby _Tab.TabOrder
                                                      select _Tab )
            {
                Tabs.Add( Tab );
            }
            for( Int32 i = 0; i < Tabs.Count; i++ )
            {
                CswNbtMetaDataNodeTypeTab CurrentTab = Tabs[i];

                SubItemsJProp[CurrentTab.TabName] = new JObject();

                //if( i > 1 )
                //{
                //    CswNbtMetaDataNodeTypeTab PrevTab = Tabs[i - 1];
                //    TabObj.Add( new JProperty( "prevtab", PrevTab.TabName ) );
                //}
                SubItemsJProp[CurrentTab.TabName]["currenttab"] = Tabs[i].TabName;
                if( i < Tabs.Count - 1 )
                {
                    SubItemsJProp[CurrentTab.TabName]["nexttab"] = Tabs[i + 1].TabName;
                }

                foreach( CswNbtMetaDataNodeTypeProp Prop in from _Prop in CurrentTab.getNodeTypePropsByDisplayOrder()
                                                            where false == _Prop.HideInMobile &&
                                                                 _Prop.getFieldType().FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password &&
                                                                 _Prop.getFieldType().FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid
                                                            select _Prop )
                {
                    CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];

                    string PropId = PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId;
                    SubItemsJProp[CurrentTab.TabName][PropId] = new JObject();
                    SubItemsJProp[CurrentTab.TabName][PropId]["prop_name"] = CswTools.SafeJavascriptParam( Prop.PropNameWithQuestionNo );
                    SubItemsJProp[CurrentTab.TabName][PropId]["fieldtype"] = Prop.getFieldType().FieldType.ToString();
                    SubItemsJProp[CurrentTab.TabName][PropId]["gestalt"] = CswTools.SafeJavascriptParam( PropWrapper.Gestalt );
                    SubItemsJProp[CurrentTab.TabName][PropId]["ocpname"] = CswTools.SafeJavascriptParam( PropWrapper.ObjectClassPropName );

                    PropWrapper.ToJSON( (JObject) SubItemsJProp[CurrentTab.TabName][PropId] );
                }
            }
        }

        // _runProperties()

        #endregion Get

        #region Set

        public bool updateViewProps( string UpdatedViewJson )
        {
            bool Ret = false;
            JObject UpdatedJSON = JObject.Parse( UpdatedViewJson );
            if( null != UpdatedJSON.Property( "nodes" ) )
            {
                // this is a view
                JObject Nodes = (JObject) UpdatedJSON.Property( "nodes" ).Value;
                Ret = _updateViewProps( Nodes );
            }
            else if( null != UpdatedJSON.Property( "node_name" ) )
            {
                // this is a node
                Ret = _updateNodeProps( UpdatedJSON );
            }
            else
            {
                // this is a node collection
                Ret = _updateViewProps( UpdatedJSON );
            }
            return Ret;
        }

        public bool updateNodesProps( string UpdatedNodeJson )
        {
            JObject UpdatedNode = JObject.Parse( UpdatedNodeJson );
            return _updateNodeProps( UpdatedNode );
        } // Run()

        private bool _updateViewProps( JObject UpdatedNode )
        {
            bool Ret = false;
            foreach( JObject Node in UpdatedNode.Properties()
                                                .Select( Prop => (JObject) Prop.Value ) )
            {
                Ret = _updateNodeProps( Node ) || Ret;
            }
            return Ret;
        }

        private bool _updateNodeProps( JObject NodeObj )
        {
            bool Ret = false;
            Collection<JProperty> Props = new Collection<JProperty>();


            if( null != NodeObj.Property( "tabs" ) )
            {
                JObject Tabs = (JObject) NodeObj.Property( "tabs" ).Value;
                foreach( JProperty Prop in from Tab
                                               in Tabs.Properties()
                                           where ( null != Tab.Value )
                                           select (JObject) Tab.Value
                                               into TabProps
                                               from Prop
                                                   in TabProps.Properties()
                                               where ( null != Prop.Value &&
                                                        Prop.Name != "nexttab" &&
                                                        Prop.Name != "currenttab" )
                                               let PropAtr = (JObject) Prop.Value
                                               where null != PropAtr["wasmodified"] &&
                                                     CswConvert.ToBoolean( PropAtr["wasmodified"] )
                                               select Prop )
                {
                    Props.Add( Prop );
                }
            }


            // post changes once per node, not once per prop            
            Collection<CswNbtNode> NodesToPost = new Collection<CswNbtNode>();

            foreach( JProperty Prop in Props )
            {
                if( null != Prop.Name )
                {
                    string NodePropId = Prop.Name; // ~ "prop_4019_nodeid_nodes_24709"
                    string[] SplitNodePropId = NodePropId.Split( '_' );
                    Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[1] );
                    CswPrimaryKey NodePk = new CswPrimaryKey( SplitNodePropId[3], CswConvert.ToInt32( SplitNodePropId[4] ) );

                    CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
                    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );

                    JObject PropObj = (JObject) Prop.Value;

                    CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( Node.NodeTypeId, CswConvert.ToString( PropObj["currenttab"] ) );
                    Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null );

                    //Case 20964. Client needs to know whether the inspection is complete.
                    if( false == Ret && Node.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                    {
                        CswNbtMetaDataObjectClassProp MetaDataOCP = MetaDataProp.getObjectClassProp();
                        if( MetaDataOCP != null )
                        {
                            CswNbtMetaDataObjectClassProp Finish = _CswNbtResources.MetaData.getObjectClassProp( Node.getObjectClassId(), CswNbtObjClassInspectionDesign.PropertyName.Finish );
                            CswNbtMetaDataObjectClassProp Cancel = _CswNbtResources.MetaData.getObjectClassProp( Node.getObjectClassId(), CswNbtObjClassInspectionDesign.PropertyName.Cancel );
                            if( MetaDataOCP == Finish ||
                                MetaDataOCP == Cancel )
                            {
                                //Ret = Ret || Node.Properties[MetaDataProp].AsButton.Checked == Tristate.True;
                                CswNbtObjClass.NbtButtonData ButtonData = new CswNbtObjClass.NbtButtonData( MetaDataProp );
                                ( (CswNbtObjClassInspectionDesign) Node ).onButtonClick( ButtonData );
                            }
                        }
                    }

                    if( false == NodesToPost.Contains( Node ) )
                    {
                        NodesToPost.Add( Node );
                    }
                }
            }

            foreach( CswNbtNode Node in NodesToPost )
            {
                Node.postChanges( false );
            }
            return Ret;
        }

        #endregion Set

    } // class CswNbtWebServiceMobileView

} // namespace ChemSW.Nbt.WebServices

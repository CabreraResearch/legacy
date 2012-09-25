using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    [ToolboxData("<{0}:CswNodeTypeTree runat=server></{0}:CswNodeTypeTree>")]
    public class CswNodeTypeTree : CompositeControl, INamingContainer
    {
        protected static char delimiter = ',';
        public bool ShowCategories = true;
        public bool ShowTabsAndProperties = false;
        public bool ShowQuestionNumbers = false;
        public bool ShowConditionalPropertiesBeneath = false;

        public enum PropertySortSetting
        {
            Alphabetical,
            DisplayOrder
        }

        public PropertySortSetting PropertySort = PropertySortSetting.Alphabetical;

        private static string CategoryPrefix = "cat_";
        private static string NodeTypeBaseVersionPrefix = "ntbv_";
        private static string NodeTypePrefix = "nt_";
        private static string NodeTypeTabPrefix = "tab_";
        private static string NodeTypePropPrefix = "prop_";
        private static string NodeTypePropFilterPrefix = "pf_";

        private static string RootNodeId = "ntroot";
        public string TreeName = "Design";

        private CswNbtResources _CswNbtResources;
        
        public CswNodeTypeTree(CswNbtResources CswNbtResources)
        {
            _CswNbtResources = CswNbtResources;
            this.DataBinding += new EventHandler(CswNodeTypeTree_DataBinding);
        }

        private XmlNode _makeTreeViewXmlNode(XmlDocument XmlDoc, string ID, string Value, string Text, string ImageUrl, bool Selectable, bool IsOnSelectedPath, string ClientSideCommand )
        {
            XmlNode Node = XmlDoc.CreateElement("Node");
            XmlAttribute IDAttribute = XmlDoc.CreateAttribute("ID");
            IDAttribute.Value = ID;
            Node.Attributes.Append(IDAttribute);
            XmlAttribute ValueAttribute = XmlDoc.CreateAttribute("Value");
            ValueAttribute.Value = Value;
            Node.Attributes.Append(ValueAttribute);
            XmlAttribute TextAttribute = XmlDoc.CreateAttribute("Text");
            TextAttribute.Value = Text;
            Node.Attributes.Append(TextAttribute);
            XmlAttribute ImageUrlAttribute = XmlDoc.CreateAttribute("ImageUrl");
            ImageUrlAttribute.Value = ImageUrl;
            Node.Attributes.Append(ImageUrlAttribute);
            XmlAttribute SelectableAttribute = XmlDoc.CreateAttribute("Selectable");
            SelectableAttribute.Value = Selectable.ToString().ToLower();
            Node.Attributes.Append(SelectableAttribute);
            //XmlAttribute ExpandedAttribute = XmlDoc.CreateAttribute("Expanded");
            //ExpandedAttribute.Value = Expanded.ToString().ToLower();
            //Node.Attributes.Append(ExpandedAttribute);
            XmlAttribute CssClassAttribute = XmlDoc.CreateAttribute("CssClass");
            CssClassAttribute.Value = "TreeNode";
            Node.Attributes.Append(CssClassAttribute);
            XmlAttribute HoveredCssClassAttribute = XmlDoc.CreateAttribute("HoveredCssClass");
            HoveredCssClassAttribute.Value = "HoverTreeNode";
            Node.Attributes.Append(HoveredCssClassAttribute);
            XmlAttribute SelectedCssClassAttribute = XmlDoc.CreateAttribute( "SelectedCssClass" );
            SelectedCssClassAttribute.Value = "SelectedTreeNode";
            Node.Attributes.Append( SelectedCssClassAttribute );
            XmlAttribute ExpandModeAttribute = XmlDoc.CreateAttribute( "ExpandMode" );
            if( IsOnSelectedPath)
                ExpandModeAttribute.Value = TreeNodeExpandMode.ClientSide.ToString();
            else
                ExpandModeAttribute.Value = TreeNodeExpandMode.WebService.ToString();
            Node.Attributes.Append( ExpandModeAttribute );

            if (ClientSideCommand != string.Empty)
            {
                XmlAttribute ClientSideCommandAttribute = XmlDoc.CreateAttribute("ClientSideCommand");
                ClientSideCommandAttribute.Value = ClientSideCommand;
                Node.Attributes.Append(ClientSideCommandAttribute);
            }
            return Node;
        }

        #region Tree Loading

        protected void CswNodeTypeTree_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();

            // We're lazy loading, which means we only need to restore the root level and everything selected
            string SelectedCategory = string.Empty;
            CswNbtMetaDataNodeType SelectedBaseVersion = null;
            CswNbtMetaDataNodeType SelectedNodeType = null;
            CswNbtMetaDataNodeTypeTab SelectedTab = null;
            CswNbtMetaDataNodeTypeProp SelectedProperty = null;

            NodeTypeTreeSelectedType PriorSelectedType = NodeTypeTreeSelectedType.Root;
            string PriorSelectedValue = string.Empty;

            XmlDocument XmlDoc = new XmlDocument();
            XmlNode DocRoot = XmlDoc.CreateElement( "Tree" );
            XmlDoc.AppendChild( DocRoot );
            XmlNode Root = _makeTreeViewXmlNode( XmlDoc,
                                                RootNodeId,
                                                RootNodeId,
                                                TreeName,
                                                "Images/view/category.gif",
                                                true, true, "" );
            DocRoot.AppendChild( Root );

            if( TreeView.SelectedNode != null )
            {
                // Make this node and its children
                PriorSelectedValue = SelectedValue;
                PriorSelectedType = SelectedType;
            }
            if( _Type != NodeTypeTreeSelectedType.None )
            {
                // Override what's selected with what someone else thinks should be selected
                PriorSelectedType = _Type;
                PriorSelectedValue = _Value;
            }

            XmlNode CategoryXmlNode = null;
            XmlNode NodeTypeXmlNode = null;
            XmlNode TabXmlNode = null;
            switch( PriorSelectedType )
            {
                case NodeTypeTreeSelectedType.Root:
                    CategoryXmlNode = _makeCategories( XmlDoc, null, Root );
                    break;
                case NodeTypeTreeSelectedType.Category:
                    SelectedCategory = PriorSelectedValue;
                    CategoryXmlNode = _makeCategories( XmlDoc, SelectedCategory, Root );
                    if(CategoryXmlNode != null)
                        NodeTypeXmlNode = _makeNodeTypes( XmlDoc, null, SelectedCategory, CategoryXmlNode );
                    break;
                case NodeTypeTreeSelectedType.NodeTypeBaseVersion:
                    SelectedBaseVersion = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PriorSelectedValue ) );
                    if(SelectedBaseVersion != null)
                        SelectedCategory = SelectedBaseVersion.getNodeTypeLatestVersion().Category;
                    CategoryXmlNode = _makeCategories( XmlDoc, SelectedCategory, Root );
                    if( CategoryXmlNode != null ) 
                        NodeTypeXmlNode = _makeNodeTypes( XmlDoc, SelectedBaseVersion, SelectedCategory, CategoryXmlNode );
                    break;
                case NodeTypeTreeSelectedType.NodeType:
                    SelectedNodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PriorSelectedValue ) );
                    if( SelectedNodeType != null )
                    {
                        SelectedBaseVersion = SelectedNodeType.getFirstVersionNodeType();
                        SelectedCategory = SelectedBaseVersion.getNodeTypeLatestVersion().Category;
                    }
                    CategoryXmlNode = _makeCategories( XmlDoc, SelectedCategory, Root );
                    if( CategoryXmlNode != null ) 
                        NodeTypeXmlNode = _makeNodeTypes( XmlDoc, SelectedNodeType, SelectedCategory, CategoryXmlNode );
                    if( SelectedNodeType != null && NodeTypeXmlNode != null)
                        TabXmlNode = _makeTabs( XmlDoc, null, SelectedNodeType, NodeTypeXmlNode );
                    break;
                case NodeTypeTreeSelectedType.Tab:
                    SelectedTab = _CswNbtResources.MetaData.getNodeTypeTab( CswConvert.ToInt32( PriorSelectedValue ) );
                    if( SelectedTab != null )
                    {
                        SelectedNodeType = SelectedTab.getNodeType();
                        SelectedBaseVersion = SelectedNodeType.getFirstVersionNodeType();
                        SelectedCategory = SelectedBaseVersion.getNodeTypeLatestVersion().Category;
                    }
                    CategoryXmlNode = _makeCategories( XmlDoc, SelectedCategory, Root );
                    if( CategoryXmlNode != null ) 
                        NodeTypeXmlNode = _makeNodeTypes( XmlDoc, SelectedNodeType, SelectedCategory, CategoryXmlNode );
                    if( SelectedNodeType != null && NodeTypeXmlNode != null )
                        TabXmlNode = _makeTabs( XmlDoc, SelectedTab, SelectedNodeType, NodeTypeXmlNode );
                    if(SelectedTab != null && TabXmlNode != null)
                        _makeProperties( XmlDoc, string.Empty, null, SelectedTab, TabXmlNode );
                    break; 
                case NodeTypeTreeSelectedType.Property:
                    string SelectedPropFilter = string.Empty; 
                    SelectedProperty = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PriorSelectedValue ) );
                    if( SelectedProperty != null )
                    {
						SelectedTab = _CswNbtResources.MetaData.getNodeTypeTab( SelectedProperty.FirstEditLayout.TabId );
                        SelectedNodeType = SelectedTab.getNodeType();
                        SelectedBaseVersion = SelectedNodeType.getFirstVersionNodeType();
                        SelectedCategory = SelectedBaseVersion.getNodeTypeLatestVersion().Category;
                        if( SelectedProperty.hasFilter() )
                            SelectedPropFilter = SelectedNodeType.getNodeTypePropByFirstVersionId( SelectedProperty.FilterNodeTypePropId ).PropId.ToString() + "_" + SelectedProperty.FilterNodeTypePropId + "_" + SelectedProperty.getFilterString();
                    }
                    CategoryXmlNode = _makeCategories( XmlDoc, SelectedCategory, Root );
                    if( CategoryXmlNode != null )
                        NodeTypeXmlNode = _makeNodeTypes( XmlDoc, SelectedNodeType, SelectedCategory, CategoryXmlNode );
                    if( SelectedNodeType != null && NodeTypeXmlNode != null )
                        TabXmlNode = _makeTabs( XmlDoc, SelectedTab, SelectedNodeType, NodeTypeXmlNode );
                    if( SelectedTab != null && TabXmlNode != null )
                        _makeProperties( XmlDoc, SelectedPropFilter, SelectedProperty, SelectedTab, TabXmlNode );
                    break;
                case NodeTypeTreeSelectedType.PropertyFilter:
                    SelectedProperty = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PriorSelectedValue.Substring( 0, PriorSelectedValue.IndexOf( "_" ) ) ) );
                    if( SelectedProperty != null )
                    {
                        SelectedTab = _CswNbtResources.MetaData.getNodeTypeTab( SelectedProperty.FirstEditLayout.TabId );
                        SelectedNodeType = SelectedTab.getNodeType();
                        SelectedBaseVersion = SelectedNodeType.getFirstVersionNodeType();
                        SelectedCategory = SelectedBaseVersion.getNodeTypeLatestVersion().Category;
                    }
                    CategoryXmlNode = _makeCategories( XmlDoc, SelectedCategory, Root );
                    if( CategoryXmlNode != null )
                        NodeTypeXmlNode = _makeNodeTypes( XmlDoc, SelectedNodeType, SelectedCategory, CategoryXmlNode );
                    if( SelectedNodeType != null && NodeTypeXmlNode != null )
                        TabXmlNode = _makeTabs( XmlDoc, SelectedTab, SelectedNodeType, NodeTypeXmlNode );
                    if( SelectedTab != null && TabXmlNode != null )
                        _makeProperties( XmlDoc, PriorSelectedValue, SelectedProperty, SelectedTab, TabXmlNode );
                    break;
            }

            TreeView.LoadXml( XmlDoc.InnerXml );
            TreeView.Nodes[0].Expanded = true;

            _setSelectedNode( PriorSelectedType, PriorSelectedValue );

            if( TreeView.SelectedNode != null )
            {
                TreeView.SelectedNode.Expanded = true;
                TreeView.SelectedNode.ExpandMode = TreeNodeExpandMode.ClientSide;
            }

        } // CswNodeTypeTree_DataBinding()



        private XmlNode _makeCategories( XmlDocument XmlDoc, string SelectedCategory, XmlNode ParentNode )
        {
            XmlNode ret = null;
            SortedList CategoryNodes = new SortedList();
            foreach( CswNbtMetaDataNodeType LatestVersionNodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
            {
                if( _IncludeThisNodeType( LatestVersionNodeType ) )
                {
                    string CategoryString = "[no category]";
                    if( LatestVersionNodeType.Category != string.Empty )
                        CategoryString = LatestVersionNodeType.Category;

                    if( !CategoryNodes.ContainsKey( CategoryString ) )
                    {
                        XmlNode CategoryNode = _makeTreeViewXmlNode( XmlDoc,
                                                                     CategoryPrefix + CategoryNodes.Count.ToString(),
                                                                     CategoryPrefix + LatestVersionNodeType.Category,
                                                                     CategoryString,
                                                                     "Images/view/category.gif",
                                                                     true,
                                                                     ( SelectedCategory != null && SelectedCategory == LatestVersionNodeType.Category ),
                                                                     "" );
                        if( SelectedCategory == LatestVersionNodeType.Category || ret == null)
                            ret = CategoryNode;
                        CategoryNodes.Add( CategoryString, CategoryNode );

                    } // if( !CategoryNodes.ContainsKey( LatestVersionNodeType.Category ) )
                } // if( _IncludeThisNodeType( LatestVersionNodeType ) )
            } // foreach( CswNbtMetaDataNodeType LatestVersionNodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )

            foreach( XmlNode CategoryNode in CategoryNodes.Values )
            {
                ParentNode.AppendChild( CategoryNode );
            }
            return ret;
        } //_makeCategories()



        private XmlNode _makeNodeTypes( XmlDocument XmlDoc, CswNbtMetaDataNodeType SelectedNodeType, string Category, XmlNode ParentNode )
        {
            XmlNode ret = null;
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes() )
            {
                CswNbtMetaDataNodeType LatestVersionNodeType = NodeType.getNodeTypeLatestVersion();
                if( ( LatestVersionNodeType.Category == Category || !ShowCategories ) && _IncludeThisNodeType( LatestVersionNodeType ) )
                {
                    // Is this a versioned nodetype?
                    XmlNode BaseVersionNode = null;
                    if( NodeType.VersionNo > 1 || LatestVersionNodeType.NodeTypeId != NodeType.NodeTypeId )
                    {
                        foreach( XmlNode BaseNode in ParentNode.ChildNodes )
                        {
                            if( BaseNode.Attributes["Value"].Value == NodeTypeBaseVersionPrefix + NodeType.FirstVersionNodeTypeId.ToString() )
                            {
                                BaseVersionNode = BaseNode;
                                break;
                            }
                        }
                        if( BaseVersionNode == null )
                        {
                            // Make one
                            BaseVersionNode = _makeTreeViewXmlNode( XmlDoc,
                                                                    NodeTypeBaseVersionPrefix + NodeType.FirstVersionNodeTypeId.ToString(),
                                                                    NodeTypeBaseVersionPrefix + NodeType.FirstVersionNodeTypeId.ToString(),
                                                                    _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeType ).NodeTypeName,
                                                                    CswNbtMetaDataObjectClass.IconPrefix16 + _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeType ).IconFileName,
                                                                    false,
                                                                    ( SelectedNodeType != null && NodeType.NodeTypeId == SelectedNodeType.NodeTypeId ),
                                                                    "" );
                            ParentNode.AppendChild( BaseVersionNode );
                        }
                    }
                    else 
                    {
                        BaseVersionNode = ParentNode;
                    }

                    if( ( NodeType.VersionNo == 1 && LatestVersionNodeType.NodeTypeId == NodeType.NodeTypeId ) ||
                        ( SelectedNodeType != null && NodeType.FirstVersionNodeTypeId == SelectedNodeType.FirstVersionNodeTypeId ) )
                    {

                        // Add this nodetype
                        string NodeTypeName = NodeType.NodeTypeName.ToString();
                        bool IsLatest = NodeType.IsLatestVersion();
                        if( NodeType.VersionNo > 1 || false == IsLatest )
                            NodeTypeName += " (v" + NodeType.VersionNo.ToString() + ")";
                        if( false == IsLatest )
                            NodeTypeName = "<span style=\"color: #6699cc\">" + NodeTypeName + "</span>";

                        XmlNode Node = _makeTreeViewXmlNode( XmlDoc,
                                                            NodeTypePrefix + NodeType.NodeTypeId.ToString(),
                                                            NodeTypePrefix + NodeType.NodeTypeId.ToString(),
                                                            NodeTypeName,
                                                            CswNbtMetaDataObjectClass.IconPrefix16 + NodeType.IconFileName.ToString(),
                                                            true,
                                                            ( SelectedNodeType != null && NodeType.NodeTypeId == SelectedNodeType.NodeTypeId ),
                                                            "" );
                        if( NodeType == SelectedNodeType || ret == null )
                            ret = Node;
                        BaseVersionNode.AppendChild( Node );
                    }
                } // if( ( LatestVersionNodeType.Category == Category || !ShowCategories ) && _IncludeThisNodeType( LatestVersionNodeType ) )
            } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.NodeTypes )
            return ret;
        } // makeNodeTypes()

        private XmlNode _makeTabs( XmlDocument XmlDoc, CswNbtMetaDataNodeTypeTab SelectedTab, CswNbtMetaDataNodeType NodeType, XmlNode ParentNode )
        {
            XmlNode ret = null;
            if( ShowTabsAndProperties )
            {
                foreach( CswNbtMetaDataNodeTypeTab NodeTypeTab in NodeType.getNodeTypeTabs() )
                {
                    XmlNode TabNode = _makeTreeViewXmlNode( XmlDoc,
                                                           NodeTypeTabPrefix + NodeTypeTab.TabId.ToString(),
                                                           NodeTypeTabPrefix + NodeTypeTab.TabId.ToString(),
                                                           NodeTypeTab.TabName.ToString(),
                                                           "Images/view/tab.gif",
                                                           true,
                                                           ( SelectedTab != null && NodeTypeTab.TabId == SelectedTab.TabId ),
                                                           "" );
                    if( NodeTypeTab == SelectedTab || ret == null )
                        ret = TabNode;
                    ParentNode.AppendChild( TabNode );
                }
            }
            return ret;
        }

        private void _makeProperties( XmlDocument XmlDoc, string SelectedPropFilterValue, CswNbtMetaDataNodeTypeProp SelectedProp, CswNbtMetaDataNodeTypeTab Tab, XmlNode ParentNode )
        {
            if( ShowTabsAndProperties )
            {
                // Do non-conditional properties (or all properties if ShowConditionalPropertiesBeneath = false)
                IEnumerable<CswNbtMetaDataNodeTypeProp> PropCollection = null;
                if( PropertySort == PropertySortSetting.DisplayOrder )
                    PropCollection = Tab.getNodeTypePropsByDisplayOrder();
                else
                    PropCollection = Tab.getNodeTypeProps();

                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in PropCollection )
                {
                    if( !ShowConditionalPropertiesBeneath || !NodeTypeProp.hasFilter() )
                    {
                        string PropName = _getPropName( NodeTypeProp );
                        XmlNode PropNode = _makeTreeViewXmlNode( XmlDoc,
                                                                NodeTypePropPrefix + NodeTypeProp.PropId.ToString(),
                                                                NodeTypePropPrefix + NodeTypeProp.PropId.ToString(),
                                                                PropName,
                                                                "Images/view/property.gif",
                                                                true,
                                                                ( SelectedProp != null && NodeTypeProp.PropId == SelectedProp.PropId ),
                                                                "" );
                        ParentNode.AppendChild( PropNode );

                    } // if( !ShowConditionalPropertiesBeneath || !NodeTypeProp.hasFilter() )
                } // foreach (CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeTab.NodeTypeProps)


                // Now do the conditional ones afterward (so that order doesn't matter)
                if( ShowConditionalPropertiesBeneath )
                {
                    foreach( CswNbtMetaDataNodeTypeProp ConditionalProp in PropCollection )
                    {
                        if( ConditionalProp.hasFilter() )
                        {
                            XmlNode PropParentNode = ParentNode;
                            string PropParentNodeValue = string.Empty;
                            foreach( XmlNode OtherPropNode in ParentNode.ChildNodes )
                            {
                                if( OtherPropNode.Attributes["Value"].Value.Substring( 0, NodeTypePropPrefix.Length ) == NodeTypePropPrefix )
                                {
                                    Int32 OtherPropId = CswConvert.ToInt32( OtherPropNode.Attributes["Value"].Value.Substring( NodeTypePropPrefix.Length ) );
                                    CswNbtMetaDataNodeTypeProp OtherProp = _CswNbtResources.MetaData.getNodeTypeProp( OtherPropId );
                                    if( SelectedProp != null &&
                                        ConditionalProp.FilterNodeTypePropId == OtherProp.FirstPropVersionId &&
                                        ( ConditionalProp.PropId == SelectedProp.PropId ||
                                          OtherProp.PropId == SelectedProp.PropId ||
                                          SelectedProp.FilterNodeTypePropId == OtherProp.PropId ||  // BZ 8386
                                          SelectedProp.FilterNodeTypePropId == OtherProp.FirstPropVersionId ) ) // BZ 8476  
                                    {
                                        PropParentNode = null;
                                        foreach( XmlNode OtherPropChildNode in OtherPropNode.ChildNodes )
                                        {
                                            if( OtherPropChildNode.Attributes["Value"].Value == NodeTypePropFilterPrefix + OtherPropId.ToString() + "_" + ConditionalProp.FilterNodeTypePropId + "_" + ConditionalProp.getFilterString() ) 
                                            {
                                                PropParentNode = OtherPropChildNode;
                                                PropParentNodeValue = OtherPropId.ToString() + "_" + ConditionalProp.FilterNodeTypePropId + "_" + ConditionalProp.getFilterString();
                                            }
                                        }
                                        if( PropParentNode == null )
                                        {
                                            CswNbtSubField SubField = _CswNbtResources.MetaData.getNodeTypeProp( ConditionalProp.FilterNodeTypePropId ).getFieldTypeRule().SubFields.Default;
                                            CswNbtPropFilterSql.PropertyFilterMode FilterMode = SubField.DefaultFilterMode;
                                            string FilterValue = null;
                                            ConditionalProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );

                                            PropParentNodeValue = OtherPropId.ToString() + "_" + ConditionalProp.FilterNodeTypePropId + "_" + ConditionalProp.getFilterString();
                                            PropParentNode = _makeTreeViewXmlNode( XmlDoc,
                                                                                   NodeTypePropFilterPrefix + PropParentNodeValue,
                                                                                   NodeTypePropFilterPrefix + PropParentNodeValue,
                                                                                   SubField.Name.ToString() + " " + FilterMode.ToString() + " " + FilterValue,
                                                                                   "Images/view/filter.gif",
                                                                                   false,
                                                                                   ( SelectedProp != null && ConditionalProp.PropId == SelectedProp.PropId ),
                                                                                   "" );
                                            OtherPropNode.AppendChild( PropParentNode );
                                        }
                                    }
                                }
                            }

                            if( SelectedProp != null && SelectedPropFilterValue != string.Empty &&
                                PropParentNodeValue == SelectedPropFilterValue &&
                                ( ConditionalProp.PropId == SelectedProp.PropId ||
                                  ConditionalProp.FilterNodeTypePropId == SelectedProp.PropId ||
                                  ConditionalProp.FilterNodeTypePropId == SelectedProp.FirstPropVersionId ||
                                  ConditionalProp.FilterNodeTypePropId == SelectedProp.FilterNodeTypePropId ) )
                            {
                                string PropName = _getPropName( ConditionalProp );
                                XmlNode PropNode = _makeTreeViewXmlNode( XmlDoc,
                                                                        NodeTypePropPrefix + ConditionalProp.PropId.ToString(),
                                                                        NodeTypePropPrefix + ConditionalProp.PropId.ToString(),
                                                                        PropName,
                                                                        "Images/view/property.gif",
                                                                        true,
                                                                        ( SelectedProp != null && ConditionalProp.PropId == SelectedProp.PropId ),
                                                                        "" );
                                PropParentNode.AppendChild( PropNode );
                            }

                        } // if( NodeTypeProp.FilterNodeTypePropId != Int32.MinValue )
                    } // foreach (CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeTab.NodeTypeProps)
                } // if( ShowConditionalPropertiesBeneath )
            } // if( ShowTabsAndProperties )
        } // makeProperties()

        #endregion Tree Loading

        private bool _IncludeThisNodeType( CswNbtMetaDataNodeType NodeType )
        {
            // BZ 7121 - Must have view permission on the nodetype
			return ( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, NodeType.getFirstVersionNodeType() ) &&
                     ( ( NodeTypeIdsToFilterOut == null || !( delimiter + NodeTypeIdsToFilterOut + delimiter ).Contains( delimiter + NodeType.FirstVersionNodeTypeId.ToString() + delimiter ) ) &&
                       ( NodeTypeIdsToInclude == null || ( delimiter + NodeTypeIdsToInclude + delimiter ).Contains( delimiter + NodeType.FirstVersionNodeTypeId.ToString() + delimiter ) ) &&
                       ( ObjectClassIdsToInclude == null || ( delimiter + ObjectClassIdsToInclude + delimiter ).Contains( delimiter + NodeType.ObjectClassId.ToString() + delimiter ) ) ) );
        }


        private string _getPropName( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            string PropName = string.Empty;
            if( ShowQuestionNumbers )
                PropName = NodeTypeProp.PropNameWithQuestionNo;
            else
                PropName = NodeTypeProp.PropName;
            return PropName;
        }

        public delegate void DataBoundEventHandler();
        public event DataBoundEventHandler DataBound = null;
        protected void OnDataBound()
        {
            if (DataBound != null)
                DataBound();
        }

        void TreeView_DataBound(object sender, EventArgs e)
        {
            OnDataBound();  // this should allow the owner to set the selected node

            // Set default selected node
            if (TreeView.SelectedNode == null)
                _setSelectedNode(NodeTypeTreeSelectedType.Root, RootNodeId);
            //else
            //    _setSelectedNode(SelectedType, SelectedValue);  // this will trigger events
        }

        private RadTreeView _TreeView;
        public RadTreeView TreeView
        {
            get
            {
                EnsureChildControls();
                return _TreeView;
            }
            set
            {
                EnsureChildControls();
                _TreeView = value;
            }
        }

        public RadTreeNode SelectedNode
        {
            get { return TreeView.SelectedNode; }
            //set { TreeView.SelectedNode = value; }
        }

        //public Int32 SelectedNodeTypeId
        //{
        //    get
        //    {
        //        if (SelectedNode != null)
        //            return CswConvert.ToInt32(SelectedNode.Value);
        //        else
        //            return Int32.MinValue;
        //    }
        //    set
        //    {
        //        TreeView.SelectedNode = TreeView.FindNodeById(value.ToString());
        //    }
        //}

        public Int32 getFirstNodeTypeId()
        {
            Int32 FirstNodeTypeId = Int32.MinValue;
            if (TreeView.Nodes.Count >= 1)                             //root
                if (TreeView.Nodes[0].Nodes.Count >= 1)                //first cat
                    if (TreeView.Nodes[0].Nodes[0].Nodes.Count >= 1)   //nodetypes
                        FirstNodeTypeId = CswConvert.ToInt32(getValueOfNode(TreeView.Nodes[0].Nodes[0].Nodes[0]));
            return FirstNodeTypeId;
        }

        //public Int32 getFirstNodeTypeId()
        //{
        //    return getFirstNodeTypeIdRecursive(TreeView.Nodes[0]);
        //}
        //private Int32 getFirstNodeTypeIdRecursive(TreeViewNode Node)
        //{
        //    if (Node.ID.Substring(0, NodeTypePrefix.Length) == NodeTypePrefix)
        //        return CswConvert.ToInt32(Node.Value);
        //    else if (Node.Nodes.Count > 0)
        //        return getFirstNodeTypeIdRecursive(Node.Nodes[0]);
        //    else
        //        return Int32.MinValue;
        //}

        public Int32 getSecondNodeTypeId()
        {
            Int32 SecondNodeTypeId = Int32.MinValue;
            if (TreeView.Nodes.Count >= 1)                                   //root
                if (TreeView.Nodes[0].Nodes.Count >= 1)                      //first cat
                    if (TreeView.Nodes[0].Nodes[0].Nodes.Count > 1)          //nodetypes
                        SecondNodeTypeId = CswConvert.ToInt32(getValueOfNode(TreeView.Nodes[0].Nodes[0].Nodes[1]));
                    else if (TreeView.Nodes[0].Nodes.Count > 1)              // second cat
                        if (TreeView.Nodes[0].Nodes[1].Nodes.Count >= 1)     // nodetypes
                            SecondNodeTypeId = CswConvert.ToInt32(getValueOfNode(TreeView.Nodes[0].Nodes[1].Nodes[0]));

            return SecondNodeTypeId;
        }

        private string _NodeTypeIdsToFilterOut;
        public string NodeTypeIdsToFilterOut
        {
            get { return _NodeTypeIdsToFilterOut; }
            set { _NodeTypeIdsToFilterOut = value; }
        }
        private string _NodeTypeIdsToInclude;
        public string NodeTypeIdsToInclude
        {
            get { return _NodeTypeIdsToInclude; }
            set { _NodeTypeIdsToInclude = value; }
        }
        private string _ObjectClassIdsToInclude;
        public string ObjectClassIdsToInclude
        {
            get { return _ObjectClassIdsToInclude; }
            set { _ObjectClassIdsToInclude = value; }
        }

        //private ComboBox _Combo;
        //private Button _WorkaroundButton;
        protected override void CreateChildControls()
        {
            //_WorkaroundButton = new Button();
            //_WorkaroundButton.ID = "workaroundbutton";
            //_WorkaroundButton.Text = "Clicky!";
            //_WorkaroundButton.CssClass = "Button";
            ////_WorkaroundButton.Style.Add(HtmlTextWriterStyle.Display, "none");
            //_WorkaroundButton.Click += new EventHandler(_WorkaroundButton_Click);
            //this.Controls.Add(_WorkaroundButton);

            //_Combo = new ComboBox();
            //_Combo.Width = Unit.Parse("200px");
            //_Combo.ID = "combo";
            //_Combo.DropDownContent = new ComboBoxContent();
            //_Combo.AutoFilter = false;
            //_Combo.AutoHighlight = false;
            //_Combo.AutoComplete = false;
            //_Combo.TextBoxEnabled = true;
            //_Combo.AutoPostBack = true;
            //_Combo.TextBoxCssClass = "textinput";
            //_Combo.Width = Unit.Parse("200px");
            //_Combo.DropDownCssClass = "selectinput";
            //_Combo.DropDownHeight = 200;
            //_Combo.DropDownWidth = 200;
            //_Combo.DropHoverImageUrl = "Images/combo/drop_hover.gif";
            //_Combo.DropImageUrl = "Images/combo/drop.gif";
            //_Combo.DropDownResizingMode = ComboBoxResizingMode.Corner;
            //this.Controls.Add(_Combo);

            TreeView = new RadTreeView();
            TreeView.ID = "nodetypetree";
            TreeView.CssClass = "Tree";
            TreeView.WebServiceSettings.Method = "GetNodeChildren";
            TreeView.WebServiceSettings.Path = "NodeTypeTreeService.asmx";
            TreeView.NodeClick += new RadTreeViewEventHandler( TreeView_NodeSelected );
            TreeView.DataBound += new EventHandler(TreeView_DataBound);
            TreeView.EnableEmbeddedSkins = false;
            TreeView.Skin = "ChemSW";
            TreeView.OnClientNodePopulating = "NodeTypeTree_OnNodePopulating";
            this.Controls.Add( TreeView );

            base.CreateChildControls();
        }

        //protected void _WorkaroundButton_Click(object sender, EventArgs e)
        //{
        //    TreeViewNodeEventArgs Args = new TreeViewNodeEventArgs();
        //    Args.Node = TreeView.SelectedNode;
        //    TreeView_NodeSelected(sender, Args);
        //}

        protected override void OnPreRender(EventArgs e)
        {
            //if (TreeView.SelectedNode != null)
            //    synchControls(CswConvert.ToInt32(TreeView.SelectedNode.ID));
            //else
            //    synchControls(Int32.MinValue);

            if(TreeView != null && TreeView.SelectedNode != null)
                TreeView.SelectedNode.ExpandParentNodes();

            base.OnPreRender(e);
        }

        private void synchControls(Int32 ViewId)
        {
            //if (ViewId != Int32.MinValue)
            //{
            //    TreeView.SelectedNode = TreeView.FindNodeById(ViewId.ToString());
            //    if (TreeView.SelectedNode != null)
            //        _Combo.Text = TreeView.SelectedNode.Text;
            //}
            //else
            //{
            //    TreeView.SelectedNode = null;
            //    _Combo.Text = string.Empty;
            //}
        }

        //public delegate void NodeTypeTreeRootSelectedEventHandler(object sender, EventArgs e);
        //public event NodeTypeTreeRootSelectedEventHandler RootSelected = null;

        //public delegate void NodeTypeCategorySelectedEventHandler(object sender, NodeTypeCategorySelectedEventArgs e);
        //public event NodeTypeCategorySelectedEventHandler CategorySelected = null;
        //public class NodeTypeCategorySelectedEventArgs : EventArgs
        //{
        //    public string CategoryName;
        //    public NodeTypeCategorySelectedEventArgs(string TheCategoryName)
        //    {
        //        CategoryName = TheCategoryName;
        //    }
        //}

        
        //public delegate void NodeTypeSelectedEventHandler(object sender, NodeTypeSelectedEventArgs e);
        //public event NodeTypeSelectedEventHandler NodeTypeSelected = null;
        //public class NodeTypeSelectedEventArgs : EventArgs
        //{
        //    public Int32 NodeTypeId;
        //    public NodeTypeSelectedEventArgs(Int32 TheNodeTypeId)
        //    {
        //        NodeTypeId = TheNodeTypeId;
        //    }
        //}

        //public delegate void NodeTypeBaseVersionSelectedEventHandler( object sender, NodeTypeBaseVersionSelectedEventArgs e );
        //public event NodeTypeBaseVersionSelectedEventHandler NodeTypeBaseVersionSelected = null;
        //public class NodeTypeBaseVersionSelectedEventArgs : EventArgs
        //{
        //    public Int32 NodeTypeId;
        //    public NodeTypeBaseVersionSelectedEventArgs( Int32 TheNodeTypeId )
        //    {
        //        NodeTypeId = TheNodeTypeId;
        //    }
        //}

        //public delegate void NodeTypeTabSelectedEventHandler(object sender, NodeTypeTabSelectedEventArgs e);
        //public event NodeTypeTabSelectedEventHandler NodeTypeTabSelected = null;
        //public class NodeTypeTabSelectedEventArgs : EventArgs
        //{
        //    public Int32 NodeTypeTabId;
        //    public NodeTypeTabSelectedEventArgs(Int32 TheNodeTypeTabId)
        //    {
        //        NodeTypeTabId = TheNodeTypeTabId;
        //    }
        //}

        //public delegate void NodeTypePropSelectedEventHandler( object sender, NodeTypePropSelectedEventArgs e );
        //public event NodeTypePropSelectedEventHandler NodeTypePropSelected = null;
        //public class NodeTypePropSelectedEventArgs : EventArgs
        //{
        //    public Int32 NodeTypePropId;
        //    public NodeTypePropSelectedEventArgs( Int32 TheNodeTypePropId )
        //    {
        //        NodeTypePropId = TheNodeTypePropId;
        //    }
        //}

        //public delegate void NodeTypePropFilterSelectedEventHandler( object sender, NodeTypePropFilterSelectedEventArgs e );
        //public event NodeTypePropFilterSelectedEventHandler NodeTypePropFilterSelected = null;
        //public class NodeTypePropFilterSelectedEventArgs : EventArgs
        //{
        //    public Int32 NodeTypePropId;
        //    public NodeTypePropFilterSelectedEventArgs( Int32 TheNodeTypePropId )
        //    {
        //        NodeTypePropId = TheNodeTypePropId;
        //    }
        //}

        protected void TreeView_NodeSelected(object sender, RadTreeNodeEventArgs e)
        {
            if (e.Node != null)
            {
                NodeTypeTreeSelectedType Type = getTypeOfNode(e.Node);
                string Value = getValueOfNode(e.Node);

                //switch (Type)
                //{
                //    case NodeTypeTreeSelectedType.Category:
                //        if (CategorySelected != null)
                //            CategorySelected(sender, new NodeTypeCategorySelectedEventArgs(Value));
                //        break;
                //    case NodeTypeTreeSelectedType.NodeType:
                //        if (NodeTypeSelected != null)
                //            NodeTypeSelected(sender, new NodeTypeSelectedEventArgs(CswConvert.ToInt32(Value)));
                //        break;
                //    case NodeTypeTreeSelectedType.Tab:
                //        if (NodeTypeTabSelected != null)
                //            NodeTypeTabSelected(sender, new NodeTypeTabSelectedEventArgs(CswConvert.ToInt32(Value)));
                //        break;
                //    case NodeTypeTreeSelectedType.Property:
                //        if (NodeTypePropSelected != null)
                //            NodeTypePropSelected(sender, new NodeTypePropSelectedEventArgs(CswConvert.ToInt32(Value)));
                //        break;
                //    case NodeTypeTreeSelectedType.Root:
                //        if (RootSelected != null)
                //            RootSelected(sender, new EventArgs());
                //        break;
                //}
                _setSelectedNode( Type, Value );
                DataBind();
            }
        }

        public static NodeTypeTreeSelectedType getTypeOfNode( RadTreeNode TreeNode )
        {
            NodeTypeTreeSelectedType ret = NodeTypeTreeSelectedType.Root;
            if (TreeNode != null)
                ret = getTypeOfNodeValue( TreeNode.Value );
            return ret;
        }
        public static NodeTypeTreeSelectedType getTypeOfNodeValue( string NodeValue )
        {
            NodeTypeTreeSelectedType ret = NodeTypeTreeSelectedType.Root;
            if( NodeValue.Length >= CategoryPrefix.Length &&
                NodeValue.Substring( 0, CategoryPrefix.Length ) == CategoryPrefix )
            {
                ret = NodeTypeTreeSelectedType.Category;
            }
            else if( NodeValue.Length >= NodeTypePrefix.Length &&
                     NodeValue.Substring( 0, NodeTypePrefix.Length ) == NodeTypePrefix )
            {
                ret = NodeTypeTreeSelectedType.NodeType;
            }
            else if( NodeValue.Length >= NodeTypeTabPrefix.Length &&
                     NodeValue.Substring( 0, NodeTypeTabPrefix.Length ) == NodeTypeTabPrefix )
            {
                ret = NodeTypeTreeSelectedType.Tab;
            }
            else if( NodeValue.Length >= NodeTypePropPrefix.Length &&
                     NodeValue.Substring( 0, NodeTypePropPrefix.Length ) == NodeTypePropPrefix )
            {
                ret = NodeTypeTreeSelectedType.Property;
            }
            else if( NodeValue.Length >= NodeTypeBaseVersionPrefix.Length &&
                     NodeValue.Substring( 0, NodeTypeBaseVersionPrefix.Length ) == NodeTypeBaseVersionPrefix )
            {
                ret = NodeTypeTreeSelectedType.NodeTypeBaseVersion;
            }
            else if( NodeValue.Length >= NodeTypePropFilterPrefix.Length &&
                     NodeValue.Substring( 0, NodeTypePropFilterPrefix.Length ) == NodeTypePropFilterPrefix )
            {
                ret = NodeTypeTreeSelectedType.PropertyFilter;
            }
            return ret;
        }


        public static string getValueOfNode( RadTreeNode TreeNode )
        {
            string ret = string.Empty; 
            if( TreeNode != null )
                ret = getValueOfNodeValue( TreeNode.Value );
            return ret;

        }
        
        public static string getValueOfNodeValue( string NodeValue )
        {
            string ret = string.Empty;
            switch( getTypeOfNodeValue( NodeValue ) )
            {
                case NodeTypeTreeSelectedType.Category:
                    ret = NodeValue.Substring( CategoryPrefix.Length );
                    break;
                case NodeTypeTreeSelectedType.NodeTypeBaseVersion:
                    ret = NodeValue.Substring( NodeTypeBaseVersionPrefix.Length );
                    break;
                case NodeTypeTreeSelectedType.NodeType:
                    ret = NodeValue.Substring( NodeTypePrefix.Length );
                    break;
                case NodeTypeTreeSelectedType.Property:
                    ret = NodeValue.Substring( NodeTypePropPrefix.Length );
                    break;
                case NodeTypeTreeSelectedType.PropertyFilter:
                    ret = NodeValue.Substring( NodeTypePropFilterPrefix.Length );
                    break;
                case NodeTypeTreeSelectedType.Root:
                    ret = NodeValue;
                    break;
                case NodeTypeTreeSelectedType.Tab:
                    ret = NodeValue.Substring( NodeTypeTabPrefix.Length );
                    break;
            }
            return ret;
        }


        public enum NodeTypeTreeSelectedType { Category, NodeType, Tab, Property, Root, NodeTypeBaseVersion, PropertyFilter, None }
        public NodeTypeTreeSelectedType SelectedType
        {
            get
            {
                return getTypeOfNode(SelectedNode);
            }
        }
        public string SelectedValue
        {
            get
            {
                if (SelectedNode != null)
                    return getValueOfNode(SelectedNode);
                else
                    return string.Empty;
            }
        }

        private NodeTypeTreeSelectedType _Type = NodeTypeTreeSelectedType.None;
        private string _Value = string.Empty;

        public void setSelectedNode( string NodeValue )
        {
            setSelectedNode( getTypeOfNodeValue( NodeValue ), getValueOfNodeValue( NodeValue ) );
        }

        public void setSelectedNode( NodeTypeTreeSelectedType Type, string Value )
        {
            _Type = Type;
            _Value = Value;
            _setSelectedNode( Type, Value );
        }


        private void _setSelectedNode( string NodeValue )
        {
            _setSelectedNode( getTypeOfNodeValue( NodeValue ), getValueOfNodeValue( NodeValue ) );
        }

        private void _setSelectedNode( NodeTypeTreeSelectedType Type, string Value )
        {
            if( TreeView != null )
            {
                RadTreeNode NodeToSelect = null;

                switch( Type )
                {
                    case NodeTypeTreeSelectedType.Category:
                        NodeToSelect = TreeView.FindNodeByValue( CategoryPrefix + Value );
                        //if( CategorySelected != null )
                        //    CategorySelected( this, new NodeTypeCategorySelectedEventArgs( Value ) );
                        break;
                    case NodeTypeTreeSelectedType.NodeTypeBaseVersion:
                        NodeToSelect = TreeView.FindNodeByValue( NodeTypeBaseVersionPrefix + Value );
                        //if( NodeTypeBaseVersionSelected != null )
                        //    NodeTypeBaseVersionSelected( this, new NodeTypeBaseVersionSelectedEventArgs( CswConvert.ToInt32( Value ) ) );
                        break;
                    case NodeTypeTreeSelectedType.NodeType:
                        NodeToSelect = TreeView.FindNodeByValue( NodeTypePrefix + Value );
                        //if( NodeTypeSelected != null )
                        //    NodeTypeSelected( this, new NodeTypeSelectedEventArgs( CswConvert.ToInt32( Value ) ) );
                        break;
                    case NodeTypeTreeSelectedType.Tab:
                        NodeToSelect = TreeView.FindNodeByValue( NodeTypeTabPrefix + Value );
                        //if( NodeTypeTabSelected != null )
                        //    NodeTypeTabSelected( this, new NodeTypeTabSelectedEventArgs( CswConvert.ToInt32( Value ) ) );
                        break;
                    case NodeTypeTreeSelectedType.Property:
                        NodeToSelect = TreeView.FindNodeByValue( NodeTypePropPrefix + Value );
                        //if( NodeTypePropSelected != null )
                        //    NodeTypePropSelected( this, new NodeTypePropSelectedEventArgs( CswConvert.ToInt32( Value ) ) );
                        break;
                    case NodeTypeTreeSelectedType.PropertyFilter:
                        NodeToSelect = TreeView.FindNodeByValue( NodeTypePropFilterPrefix + Value );
                        //if( NodeTypePropFilterSelected != null )
                        //    NodeTypePropFilterSelected( this, new NodeTypePropFilterSelectedEventArgs( CswConvert.ToInt32( Value ) ) );
                        break;
                    case NodeTypeTreeSelectedType.Root:
                        NodeToSelect = TreeView.FindNodeByValue( RootNodeId );
                        //if( RootSelected != null )
                        //    RootSelected( this, new EventArgs() );
                        break;
                }
                if( NodeToSelect != null )
                    NodeToSelect.Selected = true;
            }
            else
                throw new CswDniException( "_setSelectedNode() was called when TreeView was null" );
        }


        //public void ExpandAll()
        //{
        //    if (TreeView != null)
        //        TreeView.ExpandAllNodes();
        //}

    }
}

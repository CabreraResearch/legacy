using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    public class CswViewListTree : CompositeControl, INamingContainer
    {
        private CswNbtResources _CswNbtResources;
        private ICswWebClientStorage _CswWebClientStorage = null; 

        public static string SessionCachedXmlName = "CswViewListTree_Xml";

        public CswViewListTree( CswNbtResources CswNbtResources, ICswWebClientStorage CswWebClientStorage , bool UseCombo)
        {
            _CswWebClientStorage = CswWebClientStorage;
            _CswNbtResources = CswNbtResources;
            _UseCombo = UseCombo;
            this.DataBinding += new EventHandler( CswViewTree_DataBinding );
        }

        public static void AfterDeleteView( System.Web.SessionState.HttpSessionState Session )
        {
            ClearCache( Session );
        }
        
        // BZ 10048
        public static void AfterModifyReport( System.Web.SessionState.HttpSessionState Session )
        {
            ClearCache( Session );
        }

        public static void ClearCache( System.Web.SessionState.HttpSessionState Session )
        {
            Session[SessionCachedXmlName] = null;
        }

        private bool _UseCombo = false;

        private bool _SearchableViewsOnly = false;
        public bool SearchableViewsOnly
        {
            get { return _SearchableViewsOnly; }
            set { _SearchableViewsOnly = value; }
        }

        private Int32 _ViewIdToSelect;
        public Int32 ViewIdToSelect
        {
            get { return _ViewIdToSelect; }
            set { _ViewIdToSelect = value; }
        }

        private Int32 _ActionIdToSelect = Int32.MinValue;
        public Int32 ActionIdToSelect
        {
            get { return _ActionIdToSelect; }
            set { _ActionIdToSelect = value; }
        }

        public ViewType SelectedType
        {
            get
            {
                if( _TreeView.SelectedValue != string.Empty )
                {
                    string[] SplitValue = _TreeView.SelectedValue.Split( '_' );
                    return (ViewType) Enum.Parse( typeof( ViewType ), SplitValue[0] );
                }
                else
                {
                    return ViewType.Unknown;
                }
            }
        }
        public Int32 SelectedValue
        {
            get
            {
                if( _TreeView.SelectedValue != string.Empty )
                {
                    string[] SplitValue = _TreeView.SelectedValue.Split( '_' );
                    return CswConvert.ToInt32( SplitValue[1] );
                }
                else
                {
                    return Int32.MinValue;
                }
            }
        }

        public object PreviousView1 = null;
        public object PreviousView2 = null;
        public object PreviousView3 = null;
        public object PreviousView4 = null;
        public object PreviousView5 = null;

        public bool IncludeActions = false;
        public bool IncludeReports = false;

        private XmlNode _makePreviousViewTreeViewXmlNode( XmlDocument XmlDoc, object PreviousViewObj )
        {
            XmlNode Ret = null;
            if( PreviousViewObj is CswNbtView )
            {
                CswNbtView PreviousView = PreviousViewObj as CswNbtView;
                Ret = _makeTreeViewXmlNode( XmlDoc, PreviousView.SessionViewId.ToString(), ViewType.RecentView, PreviousView.ViewName, PreviousView.IconFileName, true, PreviousView.IsSearchable() );
            }
            else // Action
            {
                Int32 PreviousActionId = CswConvert.ToInt32( PreviousViewObj.ToString() );
                if( PreviousActionId > 0 )
                {
                    CswNbtAction Action = _CswNbtResources.Actions[PreviousActionId];
                    Ret = _makeTreeViewXmlNode( XmlDoc, Action.ActionId.ToString(), ViewType.Action, Action.Name.ToString().Replace( '_', ' ' ), 
                                                CswNbtMetaDataObjectClass.IconPrefix16 + "wizard.png", true, false );
                }
            }
            return Ret;
        }

        private XmlNode _makeTreeViewXmlNode( XmlDocument XmlDoc, string ID, ViewType Type, string Text, string ImageUrl, bool Selectable, bool Searchable )//, string ClientSideCommand)
        {
            XmlNode Node = XmlDoc.CreateElement( "Node" );
            XmlAttribute IDAttribute = XmlDoc.CreateAttribute( "ID" );
            IDAttribute.Value = Type.ToString() + "_" + ID;
            Node.Attributes.Append( IDAttribute );
            XmlAttribute ValueAttribute = XmlDoc.CreateAttribute( "Value" );
            ValueAttribute.Value = Type.ToString() + "_" + ID;
            Node.Attributes.Append( ValueAttribute );
            XmlAttribute TextAttribute = XmlDoc.CreateAttribute( "Text" );
            TextAttribute.Value = Text;
            Node.Attributes.Append( TextAttribute );
            XmlAttribute ImageUrlAttribute = XmlDoc.CreateAttribute( "ImageUrl" );
            ImageUrlAttribute.Value = ImageUrl;
            Node.Attributes.Append( ImageUrlAttribute );
            XmlAttribute PostBackAttribute = XmlDoc.CreateAttribute( "PostBack" );
            PostBackAttribute.Value = Selectable.ToString().ToLower();
            Node.Attributes.Append( PostBackAttribute );
            XmlAttribute CssClassAttribute = XmlDoc.CreateAttribute( "CssClass" );
            CssClassAttribute.Value = "TreeNode";
            Node.Attributes.Append( CssClassAttribute );
            XmlAttribute HoveredCssClassAttribute = XmlDoc.CreateAttribute( "HoveredCssClass" );
            HoveredCssClassAttribute.Value = "HoverTreeNode";
            Node.Attributes.Append( HoveredCssClassAttribute );
            XmlAttribute SelectedCssClassAttribute = XmlDoc.CreateAttribute( "SelectedCssClass" );
            SelectedCssClassAttribute.Value = "SelectedTreeNode";
            Node.Attributes.Append( SelectedCssClassAttribute );
            XmlAttribute SearchableAttribute = XmlDoc.CreateAttribute( "Searchable" );
            SearchableAttribute.Value = Searchable.ToString().ToLower();
            Node.Attributes.Append( SearchableAttribute );

            //if (ClientSideCommand != string.Empty)
            //{
            //    XmlAttribute ClientSideCommandAttribute = XmlDoc.CreateAttribute("OnClientNodeClicking");
            //    ClientSideCommandAttribute.Value = ClientSideCommand;
            //    Node.Attributes.Append(ClientSideCommandAttribute);
            //}
            return Node;
        }

        public enum ViewType
        {
            Root,
            View,
            //ViewCategory, 
            Category,
            Action,
            Report,
            //ReportCategory, 
            Search,
            RecentView,
            Unknown
        };

        protected void CswViewTree_DataBinding( object sender, EventArgs e )
        {
            try
            {
                CswTimer DataBindTimer = new CswTimer();
                _CswNbtResources.logTimerResult( "CswViewListTree.DataBind() started", DataBindTimer.ElapsedDurationInSecondsAsString );
                EnsureChildControls();

                // don't bother databinding on postback -- this prevents losing the currently selected node
                if( TreeView.SelectedNode == null )
                {

                    // BZ 8686
                    if( Page.Session[SessionCachedXmlName] != null && Page.Session[SessionCachedXmlName].ToString() != string.Empty )
                    {
                        TreeView.LoadXml( Page.Session[SessionCachedXmlName].ToString() );
                    }
                    else
                    {

                        XmlDocument XmlDoc = new XmlDocument();
                        XmlNode DocRoot = XmlDoc.CreateElement( "Tree" );
                        XmlDoc.AppendChild( DocRoot );

                        // Views

                        Dictionary<CswNbtViewId, CswNbtView> Views = _CswNbtResources.ViewSelect.getVisibleViews( "lower(NVL(v.category, v.viewname)), lower(v.viewname)", false );

                        foreach( CswNbtView View in Views.Values )
                        {
                            // BZ 10121
                            // This is a performance hit, but since this view list is cached, it's ok
                            CswNbtView CurrentView = new CswNbtView( _CswNbtResources );
                            CurrentView.LoadXml( View.ToXml() );
                            CurrentView.ViewId = View.ViewId;

                            // if( CurrentView.IsFullyEnabled() ) -- Case 20452: getVisibleViews() does this already
                            XmlNode CategoryNode = _getCategoryNode( DocRoot, View.Category );
                            bool ThisSearchable = View.ToXml().ToString().Contains( "<Filter" );
                            CategoryNode.AppendChild( _makeTreeViewXmlNode( XmlDoc, CurrentView.ViewId.ToString(), ViewType.View, CurrentView.ViewName, CurrentView.IconFileName, true, ThisSearchable ) );
                        }


                        // Actions

                        foreach( CswNbtAction Action in _CswNbtResources.Actions )
                        {
							if( Action.ShowInList &&
								( _CswNbtResources.ConfigVbls.getConfigVariableValue( "loc_use_images" ) != "0" ) &&
								  _CswNbtResources.Permit.can( Action.Name ) )
							{
								XmlNode CategoryNode = _getCategoryNode( DocRoot, Action.Category );
								CategoryNode.AppendChild( _makeTreeViewXmlNode( XmlDoc, Action.ActionId.ToString(), ViewType.Action, Action.Name.ToString().Replace( '_', ' ' ), 
                                                                                CswNbtMetaDataObjectClass.IconPrefix16 + "wizard.png", true, false ) );
							}
                        }


                        // Reports

                        CswNbtView ReportView = new CswNbtView( _CswNbtResources );
                        ReportView.ViewName = "CswViewTree.DataBinding.ReportView";
                        CswNbtMetaDataObjectClass ReportMetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReportClass );
                        CswNbtViewRelationship ReportRelationship = ReportView.AddViewRelationship( ReportMetaDataObjectClass, true );

                        ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( ReportView, true, true, false, false );

                        for( int i = 0; i < ReportTree.getChildNodeCount(); i++ )
                        {
                            ReportTree.goToNthChild( i );

                            CswNbtObjClassReport ReportNode = (CswNbtObjClassReport) ReportTree.getNodeForCurrentPosition();
                            XmlNode CategoryNode = _getCategoryNode( DocRoot, ReportNode.Category.Text );
                            CategoryNode.AppendChild( _makeTreeViewXmlNode( XmlDoc, ReportNode.NodeId.PrimaryKey.ToString(), ViewType.Report, ReportNode.ReportName.Text, "Images/view/report.gif", true, false ) );

                            ReportTree.goToParentNode();
                        }

                        TreeView.LoadXml( XmlDoc.InnerXml );
                        Page.Session.Add( SessionCachedXmlName, XmlDoc.InnerXml );

                    } // if-else( Page.Session[SessionCachedXmlName] != null && Page.Session[SessionCachedXmlName].ToString() != string.Empty )
                } // if(TreeView.SelectedNode == null)

                _CswNbtResources.logTimerResult( "CswViewListTree.DataBind() finished", DataBindTimer.ElapsedDurationInSecondsAsString );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        private Int32 _catcount = 0;
        private XmlNode _getCategoryNode( XmlNode DocRoot, string Category )
        {
            // Find the category
            XmlNode CategoryNode = null;
            if( Category != string.Empty )
            {
                foreach( XmlNode CatNode in DocRoot.ChildNodes )
                {
                    if( CatNode.Attributes["Value"].Value.Length >= ViewType.Category.ToString().Length &&
                        CatNode.Attributes["Value"].Value.Substring( 0, ViewType.Category.ToString().Length ) == ViewType.Category.ToString() &&
                        CatNode.Attributes["Text"].Value == Category )
                    {
                        CategoryNode = CatNode;
                        break;
                    }
                }
                if( CategoryNode == null )
                {
                    // Make one
                    _catcount++;
                    CategoryNode = _makeTreeViewXmlNode( DocRoot.OwnerDocument, _catcount.ToString(), ViewType.Category, Category, "Images/view/category.gif", false, false );
                    DocRoot.AppendChild( CategoryNode );
                }
            }
            else
            {
                CategoryNode = DocRoot;
            }
            return CategoryNode;
        } // _getCategoryNode()

        private RadTreeView _TreeView;
        public RadTreeView TreeView
        {
            get
            {
                EnsureChildControls();
                return _TreeView;
            }
        }

        private HtmlGenericControl Div;
        private CswTreeCombo _TreeCombo;

        protected override void CreateChildControls()
        {
            Div = new HtmlGenericControl( "div" );
            Div.Attributes.Add( "class", "ViewTree" );
            Div.ID = "vtdiv";
            this.Controls.Add( Div );

            _TreeView = new RadTreeView();
            _TreeView.ID = "viewtree";
            _TreeView.CssClass = "Tree";
            _TreeView.AllowNodeEditing = false;
            _TreeView.EnableEmbeddedSkins = false;
            _TreeView.Skin = "ChemSW";

            if( _UseCombo )
            {
                _TreeView.OnClientNodeClicked = "CswViewTree_OnClientNodeClicked";

                _TreeCombo = new CswTreeCombo( _TreeView );
                _TreeCombo.ID = "treecombo";
                _TreeCombo.ShowClear = false;
                _TreeCombo.ShowEdit = false;
                _TreeCombo.KeepExpanded = false;
                Div.Controls.Add( _TreeCombo );
            }
            else
            {
                Div.Controls.Add( _TreeView );
            }

            base.CreateChildControls();
        }

        protected override void OnLoad( EventArgs e )
        {
            // Conditionally assign click event, otherwise click always causes postback
            if( ViewSelected != null )
                _TreeView.NodeClick += new RadTreeViewEventHandler( _TreeView_NodeClick );

            base.OnLoad( e );
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                if( _UseCombo )
                {
                    if( ActionIdToSelect > 0 )
                        _TreeCombo.SelectedTreeViewNodeValue = ViewType.Action.ToString() + "_" + _CswNbtResources.Actions[ActionIdToSelect].Url;
                    else
                        _TreeCombo.SelectedTreeViewNodeValue = ViewType.View.ToString() + "_" + ViewIdToSelect.ToString();
                }

                // Because we cache the XML now, we need to filter these out on display, rather than in the data
                if( !IncludeActions || !IncludeReports || SearchableViewsOnly )
                {
                    _filterOutTreeNodesRecursive( TreeView.Nodes );
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }


        private void _filterOutTreeNodesRecursive( RadTreeNodeCollection Nodes )
        {
            foreach( RadTreeNode TreeNode in Nodes )
            {
                string[] SplitValue = TreeNode.Value.Split( '_' );
                ViewType ThisType = (ViewType) Enum.Parse( typeof( ViewType ), SplitValue[0] );
                if( ( !IncludeReports && ( //ThisType == ViewType.ReportCategory ||
                                           ThisType == ViewType.Report //||
                    //( ThisType == ViewType.Root && TreeNode.Text == "Reports" ) 
                                           ) ) ||
                    ( !IncludeActions && ( ThisType == ViewType.Action //||
                    //( ThisType == ViewType.Root && TreeNode.Text == "Actions" ) 
                                           ) ) ||
                    ( SearchableViewsOnly && ( ThisType == ViewType.View &&
                                               TreeNode.Attributes["Searchable"].ToString() == "false" ) ) )
                {
                    TreeNode.Visible = false;
                }

                if( TreeNode.Nodes.Count > 0 )
                    _filterOutTreeNodesRecursive( TreeNode.Nodes );
            }
        }

        public delegate void ViewSelectedEventHandler( object sender, RadTreeNodeEventArgs e );
        public event ViewSelectedEventHandler ViewSelected = null;

        void _TreeView_NodeClick( object sender, RadTreeNodeEventArgs e )
        {
            if( ViewSelected != null )
                ViewSelected( sender, e );
        }

        // Error handling
        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }
    }
}

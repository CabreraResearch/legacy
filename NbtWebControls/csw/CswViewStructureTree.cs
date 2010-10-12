using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Core;
using Telerik.Web.UI;
using ChemSW.Exceptions;

namespace ChemSW.NbtWebControls
{
    public class CswViewStructureTree : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        public CswViewStructureTree( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }
        
        private RadTreeView _Tree;
        protected override void CreateChildControls()
        {
            try
            {
                _Tree = new RadTreeView();
                _Tree = new RadTreeView();
                _Tree.EnableEmbeddedSkins = false;
                _Tree.Skin = "ChemSW";
                _Tree.ID = "ViewStructureTree";
                _Tree.CssClass = "Tree";
                _Tree.NodeClick += new RadTreeViewEventHandler( _Tree_NodeClick );
                this.Controls.Add( _Tree );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.CreateChildControls();
        }

        public RadTreeNode SelectedNode
        {
            get { return _Tree.SelectedNode; }
        }

        public void setSelectedNode( string NodeValue )
        {
            _Tree.FindNodeByValue( NodeValue ).Selected = true;
        }

        public bool IsRootSelectable = true;
        public bool IsFirstLevelRemovable = true;

        public RadTreeViewEventHandler OnClick = null;
        protected void _Tree_NodeClick( object sender, RadTreeNodeEventArgs e )
        {
            try
            {
                if( OnClick != null )
                    OnClick( sender, e );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else
                throw ex;
        }

        public enum ViewTreeSelectType
        {
            Relationship,
            Property,
            Filter,
            None
        }

        public void reinitTreeFromView( CswNbtView View, CswNbtViewNode ViewNodeToSelect, CswNbtViewNode DefaultNodeToSelect, ViewTreeSelectType SelectType )
        {
            EnsureChildControls();

            // Setup tree and selected node
            string PriorSelectedNodeValue = string.Empty;
            if(_Tree.SelectedNode != null)
                PriorSelectedNodeValue = _Tree.SelectedNode.Value;

            _Tree.Nodes.Clear();
            _initTreeFromViewRecursive( View.Root, _Tree.Nodes, SelectType );

            if( PriorSelectedNodeValue != string.Empty )
            {
                RadTreeNode PotentialNodeMatch = _Tree.FindNodeByValue( PriorSelectedNodeValue );
                if( PotentialNodeMatch != null )
                    PotentialNodeMatch.Selected = true;
            }
            if( ViewNodeToSelect != null )
            {
                RadTreeNode PotentialNodeMatch = _Tree.FindNodeByValue( ViewNodeToSelect.ArbitraryId );
                if( PotentialNodeMatch != null )
                    PotentialNodeMatch.Selected = true;
            }
            if( _Tree.SelectedNode == null && DefaultNodeToSelect != null )
                _Tree.FindNodeByValue( DefaultNodeToSelect.ArbitraryId ).Selected = true;

            if( _Tree.SelectedNode == null && IsRootSelectable )
                _Tree.FindNodeByValue( View.Root.ArbitraryId ).Selected = true;

            _Tree.ExpandAllNodes();
        }


        private void _initTreeFromViewRecursive( CswNbtViewNode ViewNode, RadTreeNodeCollection Nodes, ViewTreeSelectType SelectType )
        {
            RadTreeNode newNode = null;
            newNode = _CreateNode( ViewNode, SelectType );
            Nodes.Add( newNode );

            // Recurse
            if( ViewNode is CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                    _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
            }
            else if( ViewNode is CswNbtViewRelationship )
            {
                foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                    _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
                foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                    _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
            }
            else if( ViewNode is CswNbtViewProperty )
            {
                foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
                    _initTreeFromViewRecursive( Child, newNode.Nodes, SelectType );
            }
        }

        public string SelectableNodeTextPrefix = string.Empty;
        public string SelectableNodeTextSuffix = string.Empty;

        private RadTreeNode _CreateNode( CswNbtViewNode ViewNode, ViewTreeSelectType SelectType )
        {
            RadTreeNode node = new RadTreeNode();
            node.ImageUrl = ViewNode.IconFileName;
            node.Text = ViewNode.TextLabel;
            
            if( SelectableNodeTextPrefix != string.Empty || SelectableNodeTextSuffix != string.Empty )
            {
                if( ( SelectType == ViewTreeSelectType.Relationship && ViewNode is CswNbtViewRelationship &&
                      ( IsFirstLevelRemovable || ViewNode.Parent is CswNbtViewRelationship ) ) ||                // BZ 9203
                    ( SelectType == ViewTreeSelectType.Property && ViewNode is CswNbtViewProperty ) ||
                    ( SelectType == ViewTreeSelectType.Filter && ViewNode is CswNbtViewPropertyFilter ) )
                {
                    {
                        node.Text = node.Text + SelectableNodeTextPrefix;
                        node.Text = node.Text + CswTools.SafeJavascriptParam( ViewNode.ArbitraryId );
                        node.Text = node.Text + SelectableNodeTextSuffix;
                    }
                }
            }
            node.Expanded = true;

            if( SelectType != ViewTreeSelectType.None )
            {
                node.Enabled = false;
                if( ( SelectType == ViewTreeSelectType.Relationship && ( ( ViewNode is CswNbtViewRoot && IsRootSelectable ) || ViewNode is CswNbtViewRelationship ) ) ||
                    ( SelectType == ViewTreeSelectType.Property && ( ViewNode is CswNbtViewRelationship || ViewNode is CswNbtViewProperty ) ) ||
                    ( SelectType == ViewTreeSelectType.Filter && ( ViewNode is CswNbtViewProperty || ViewNode is CswNbtViewPropertyFilter ) ) )
                {
                    node.Enabled = true;
                }
            }

            node.CssClass = "TreeNode";
            node.SelectedCssClass = "SelectedTreeNode";
            node.Value = ViewNode.ArbitraryId;
            return node;
        }

    }
}
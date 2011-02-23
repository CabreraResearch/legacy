using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using Telerik.Web.UI;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Core;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_AssignTests : System.Web.UI.Page
    {

        protected override void OnInit( EventArgs e )
        {
            //this.EnableViewState = false;
            AssignTestsWizard.onPageChange += new CswWizard.CswWizardEventHandler( AssignTestsWizard_onPageChange );
            AssignTestsWizard.OnError += new CswErrorHandler( AssignTestsWizard_OnError );
            AssignTestsWizard.onFinish += new CswWizard.CswWizardEventHandler( AssignTestsWizard_onFinish );
            AssignTestsWizard.onCancel += new CswWizard.CswWizardEventHandler( AssignTestsWizard_onCancel );

            EnsureChildControls();

            _initViewList( _LoadAliquotsViewList );
            _initViewList( _LoadTestsViewList );

            base.OnInit( e );
        }

        private RadTreeView _AliquotsTreeView;
        private RadTreeView _TestsTreeView;
        private Label _LoadAliquotsViewLabel;
        private Label _LoadTestsViewLabel;
        private DropDownList _LoadAliquotsViewList;
        private DropDownList _LoadTestsViewList;

        protected override void CreateChildControls()
        {
            _LoadAliquotsViewLabel = new Label();
            _LoadAliquotsViewLabel.Text = "Select a View of Aliquots:";

            _LoadAliquotsViewList = new DropDownList();
            _LoadAliquotsViewList.ID = "Aliquotsviewlist";
            _LoadAliquotsViewList.CssClass = "selectinput";

            _LoadTestsViewLabel = new Label();
            _LoadTestsViewLabel.Text = "Select a View of Tests:";

            _LoadTestsViewList = new DropDownList();
            _LoadTestsViewList.ID = "testsviewlist";
            _LoadTestsViewList.CssClass = "selectinput";

            _AliquotsTreeView = new RadTreeView();
            _AliquotsTreeView.ID = "AliquotsTreeView";
            _AliquotsTreeView.CheckBoxes = true;
            _AliquotsTreeView.EnableEmbeddedSkins = false;
            _AliquotsTreeView.Skin = "ChemSW";

            _TestsTreeView = new RadTreeView();
            _TestsTreeView.ID = "TestsTreeView";
            _TestsTreeView.CheckBoxes = true;
            _TestsTreeView.EnableEmbeddedSkins = false;
            _TestsTreeView.Skin = "ChemSW";


            Step1PH.Controls.Add( _LoadAliquotsViewLabel );
            Step1PH.Controls.Add( _LoadAliquotsViewList );
            Step2PH.Controls.Add( _AliquotsTreeView );
            Step3PH.Controls.Add( _LoadTestsViewLabel );
            Step3PH.Controls.Add( _LoadTestsViewList );
            Step4PH.Controls.Add( _TestsTreeView );

            base.CreateChildControls();
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }

        void AssignTestsWizard_onPageChange( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            switch( CswWizardEventArgs.NewPage )
            {
                case 1:
                    break;
                case 2:
                    if( !_initTree( CswConvert.ToInt32( _LoadAliquotsViewList.SelectedValue ), _AliquotsTreeView, CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass ) )
                    {
                        Literal ErrorMsg = new Literal();
                        ErrorMsg.Text = "Please select a valid view";
                        Step1PH.Controls.Add( ErrorMsg );

                        AssignTestsWizard.CurrentStep = 1;
                    }
                    break;
                case 3:
                    if( _AliquotsTreeView.CheckedNodes.Count == 0 )
                    {
                        Literal ErrorMsg = new Literal();
                        ErrorMsg.Text = "You must select at least one Aliquot";
                        Step2PH.Controls.Add( ErrorMsg );

                        _TestsTreeView.Visible = false;
                        AssignTestsWizard.CurrentStep = 2;
                    }
                    break;
                case 4:
                    if( !_initTree( CswConvert.ToInt32( _LoadTestsViewList.SelectedValue ), _TestsTreeView, CswNbtMetaDataObjectClass.NbtObjectClass.TestClass ) )
                    {
                        Literal ErrorMsg = new Literal();
                        ErrorMsg.Text = "Please select a valid view";
                        Step3PH.Controls.Add( ErrorMsg );

                        AssignTestsWizard.CurrentStep = 3;
                    }
                    break;
            }
        }

        void AssignTestsWizard_onFinish( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            if( _TestsTreeView.CheckedNodes.Count == 0 )
            {
                Literal ErrorMsg = new Literal();
                ErrorMsg.Text = "You must select at least one Test";
                Step4PH.Controls.Add( ErrorMsg );

                AssignTestsWizard.CurrentStep = 4;
            }
            else
            {
                CswNbtActAssignTests CswNbtActAssignTests = new CswNbtActAssignTests( Master.CswNbtResources );

                CswNbtMetaDataObjectClass AliquotObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass );
                CswNbtMetaDataObjectClass TestObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TestClass );

                Collection<CswNbtNodeKey> AliquotNodeKeys = new Collection<CswNbtNodeKey>();
                Collection<CswNbtNodeKey> TestNodeKeys = new Collection<CswNbtNodeKey>();

                _FetchCheckedNodes( ref AliquotNodeKeys, AliquotObjectClass.ObjectClassId, _AliquotsTreeView );
                _FetchCheckedNodes( ref TestNodeKeys, TestObjectClass.ObjectClassId, _TestsTreeView );

                Collection<CswPrimaryKey> ResultNodeIds = CswNbtActAssignTests.AssignTest( AliquotNodeKeys, TestNodeKeys );

                CswNbtView NewResultsView = _makeNewResultsByAliquotView( AliquotNodeKeys, TestNodeKeys, ResultNodeIds );
                Master.setViewXml( NewResultsView.ToString() );
                Master.GoMain();
            }
        }


        // If a non-matching parent is checked, assume all immediate children of that parent are checked
        private void _FetchCheckedNodes( ref Collection<CswNbtNodeKey> NodeKeyCollection, Int32 ObjectClassIdToInclude, RadTreeView TreeView )
        {
            foreach( RadTreeNode ChildTreeNode in TreeView.Nodes )
            {
                _FetchCheckedNodesRecursive( ref NodeKeyCollection, ObjectClassIdToInclude, TreeView, ChildTreeNode, ChildTreeNode.Checked );
            }
        }
        private void _FetchCheckedNodesRecursive( ref Collection<CswNbtNodeKey> NodeKeyCollection, Int32 ObjectClassIdToInclude, RadTreeView TreeView, RadTreeNode CurrentTreeNode, bool IncludeAllChildren )
        {
            CswNbtNodeKey CurrentNodeKey = new CswNbtNodeKey( Master.CswNbtResources, CurrentTreeNode.Value );
            if( CurrentNodeKey.ObjectClassId == ObjectClassIdToInclude && ( CurrentTreeNode.Checked || IncludeAllChildren ) )
            {
                NodeKeyCollection.Add( CurrentNodeKey );
            }
            // recurse
            foreach( RadTreeNode ChildTreeNode in CurrentTreeNode.Nodes )
            {
                _FetchCheckedNodesRecursive( ref NodeKeyCollection, ObjectClassIdToInclude, TreeView, ChildTreeNode, ChildTreeNode.Checked || IncludeAllChildren );
            }
        }



        void AssignTestsWizard_OnError( Exception ex )
        {
            Master.HandleError( ex );
        }
        void AssignTestsWizard_onCancel( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            Master.GoMain();
        }

        private bool _initViewList( DropDownList ViewList )
        {
            bool ret = false;
            ViewList.Items.Clear();
            DataTable Views = null;
            //if( Master.CswNbtResources.CurrentUser.IsAdministrator() )
            Views = Master.CswNbtResources.ViewSelect.getVisibleViews( false );
            //else
            //Views = CswNbtView.getUserViews( Master.CswNbtResources );

            if( Views.Rows.Count > 0 )
            {
                ret = true;
                foreach( DataRow Row in Views.Rows )
                {
                    ViewList.Items.Add( new ListItem( Row["viewname"].ToString(), Row["nodeviewid"].ToString() ) );
                }
            }
            return ret;
        }

        private bool _initTree( Int32 ViewId, RadTreeView TreeView, CswNbtMetaDataObjectClass.NbtObjectClass FilterToObjectClass )
        {
            bool ret = false;
            if( ViewId > 0 )
            {
                ret = true;
                CswNbtView ViewToLoad = (CswNbtView) CswNbtViewFactory.restoreView( Master.CswNbtResources, ViewId );
                //Filter to nodes of FilterToObjectClass on treeview
                //_setFiltersOnView( ViewToLoad, FilterToObjectClass );

                ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( ViewToLoad, true, true, false, false );
                string Xml = Tree.getTreeAsXml();
                TreeView.LoadXml( Xml );
                TreeView.ExpandAllNodes();
            }
            return ret;
        }

        //private void _setFiltersOnView( CswNbtView ViewToLoad, CswNbtMetaDataObjectClass.NbtObjectClass FilterToObjectClass )
        //{
        //    foreach( CswNbtViewRelationship ChildRelationship in ViewToLoad.Root.ChildRelationships )
        //    {
        //        _setFiltersOnViewRecursive( ChildRelationship, ViewToLoad, FilterToObjectClass );
        //    }
        //}
        //private void _setFiltersOnViewRecursive( CswNbtViewRelationship Relationship, CswNbtView ViewToLoad, CswNbtMetaDataObjectClass.NbtObjectClass FilterToObjectClass )
        //{
        //    if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
        //    {
        //        CswNbtMetaDataObjectClass ObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( Relationship.SecondId ) );
        //        if( ObjectClass.ObjectClass != FilterToObjectClass )
        //            Relationship.ShowInTree = false;
        //    }
        //    else
        //    {
        //        CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( Relationship.SecondId ) );
        //        CswNbtMetaDataObjectClass ObjectClass = NodeType.ObjectClass;
        //        if( ObjectClass.ObjectClass != FilterToObjectClass )
        //            Relationship.ShowInTree = false;
        //    }

        //    foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
        //    {
        //        _setFiltersOnViewRecursive( ChildRelationship, ViewToLoad, FilterToObjectClass );
        //    }
        //}

        private CswNbtView _makeNewResultsByAliquotView( Collection<CswNbtNodeKey> AliquotNodeKeys, Collection<CswNbtNodeKey> TestNodeKeys, Collection<CswPrimaryKey> ResultNodeIds )
        {
            CswNbtMetaDataObjectClass AliquotObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass );
            CswNbtMetaDataObjectClass ResultObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ResultClass );
            CswNbtMetaDataObjectClassProp ResultAliquotProp = ResultObjectClass.getObjectClassProp( CswNbtObjClassResult.AliquotPropertyName );

            CswNbtView View = new CswNbtView( Master.CswNbtResources );
            View.ViewName = "New Results by Aliquot";

            CswNbtViewRelationship AliquotsRel = View.AddViewRelationship( ResultObjectClass, false );
            foreach( CswNbtNodeKey AliquotNodeKey in AliquotNodeKeys )
                AliquotsRel.NodeIdsToFilterIn.Add( AliquotNodeKey.NodeId );

            CswNbtViewRelationship ResultsRel = View.AddViewRelationship( AliquotsRel, CswNbtViewRelationship.PropOwnerType.Second, ResultAliquotProp, false );
            foreach( CswPrimaryKey ResultNodeId in ResultNodeIds )
                ResultsRel.NodeIdsToFilterIn.Add( ResultNodeId );

            return View;
        }
    }
}
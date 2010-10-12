using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_FutureScheduling : System.Web.UI.Page
    {
        private CswAutoTable _StepOneCswAutoTable = new CswAutoTable();
        private CswAutoTable _StepTwoCswAutoTable = new CswAutoTable();

        private RadTreeView _NodesTreeOfGeneratorNodes = null;
        private RadTreeView _NodesTreeOfFutureNodes = null;

        private CswDatePicker _EndDatePicker = null;
        private Label _ViewFutureNodesLabel = null;
        private Label _NoGeneratorsWarning = null;

        private LinkButton _CheckAllLink = null;
        private LinkButton _UnCheckAllLink = null;

        protected void Page_Init( object sender, EventArgs e )
        {
            try
            {
                FutureSchedulingWizard.OnError += new CswErrorHandler( Master.HandleError );
                FutureSchedulingWizard.onCancel += new CswWizard.CswWizardEventHandler( HandleCancel );
                FutureSchedulingWizard.onPageChange += new CswWizard.CswWizardEventHandler( _OnWizardStepChange );
                FutureSchedulingWizard.onFinish += new CswWizard.CswWizardEventHandler( _OnWizardFinish );

                EnsureChildControls();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }//Page_Init()

        protected override void CreateChildControls()
        {
            //Step One ******************************************

            StepOnePlaceHolder.Controls.Add( _StepOneCswAutoTable );

            _NoGeneratorsWarning = new Label();
            _NoGeneratorsWarning.Text = "The currently loaded view contains no Generator nodes";
            _NoGeneratorsWarning.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            _NoGeneratorsWarning.Visible = false;
            _StepOneCswAutoTable.addControl( 0, 1, _NoGeneratorsWarning );

            Label RangeSelectPrompt = new Label();
            RangeSelectPrompt.Text = "Select future date:";
            _StepOneCswAutoTable.addControl( 1, 0, RangeSelectPrompt );

            Label EndSelectLabel = new Label();
            EndSelectLabel.Text = "End Date";
            _StepOneCswAutoTable.addControl( 3, 0, EndSelectLabel );

            _EndDatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, false );
            _EndDatePicker.ID = "EndDatePicker";
            _EndDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup; 
            _StepOneCswAutoTable.addControl( 3, 1, _EndDatePicker );

            Label SelectNodesLabel = new Label();
            SelectNodesLabel.ID = "SelectNodesLabel";
            SelectNodesLabel.Text = "Select Generator Nodes:";

            _NodesTreeOfGeneratorNodes = new RadTreeView();
            _NodesTreeOfGeneratorNodes.ID = "_NodesTreeOfGeneratorNodes";
            _NodesTreeOfGeneratorNodes.CheckBoxes = true;
            _NodesTreeOfGeneratorNodes.EnableEmbeddedSkins = false;
            _NodesTreeOfGeneratorNodes.Skin = "ChemSW";

            _StepOneCswAutoTable.addControl( 4, 0, SelectNodesLabel );
            _StepOneCswAutoTable.addControl( 4, 1, _NodesTreeOfGeneratorNodes );

            _CheckAllLink = new LinkButton();
            _CheckAllLink.Text = "Select All";
            
            _StepOneCswAutoTable.addControl( 4, 2, _CheckAllLink );

            _StepOneCswAutoTable.addControl( 4, 2, new CswLiteralNbsp() );
            _StepOneCswAutoTable.addControl( 4, 2, new CswLiteralNbsp() );

            _UnCheckAllLink = new LinkButton();
            _UnCheckAllLink.Text = "Deselect All";
            _StepOneCswAutoTable.addControl( 4, 3, _UnCheckAllLink );


            ////Step Two ******************************************
            
            StepTwoPlaceHolder.Controls.Add( _StepTwoCswAutoTable );

            _ViewFutureNodesLabel = new Label();
            _ViewFutureNodesLabel.Text = "Future Nodes:";
            _ViewFutureNodesLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            _StepTwoCswAutoTable.addControl( 0, 0, _ViewFutureNodesLabel );

            _NodesTreeOfFutureNodes = new RadTreeView();
            _NodesTreeOfFutureNodes.ID = "_NodesTreeOfFutureNodes";
            _NodesTreeOfFutureNodes.EnableEmbeddedSkins = false;
            _NodesTreeOfFutureNodes.Skin = "ChemSW";
            _StepTwoCswAutoTable.addControl( 1, 0, _NodesTreeOfFutureNodes );

            base.CreateChildControls();

        }//CreateChildControls()


        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                _initGeneratorNodeTree();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }//Page_Load()


        private void _OnWizardFinish( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                Master.Redirect( "Main.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        private void _OnWizardStepChange( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                _initGeneratorNodeTree();  //bz # 5932
                CswNbtActGenerateFutureNodes CswNbtActGenerateFutureNodes = new CswNbtActGenerateFutureNodes( Master.CswNbtResources );

                if( 2 == CswWizardEventArgs.NewPage )
                {
                    ArrayList SelectedGeneratorNodes = new ArrayList();
                    ArrayList AddedKeys = new ArrayList();
                    int TotalNodes = 0;
                    foreach( RadTreeNode CurrentNode in _NodesTreeOfGeneratorNodes.CheckedNodes )
                    {
                        CswNbtNodeKey CurrentNodeKey = new CswNbtNodeKey( Master.CswNbtResources, CurrentNode.Value );
                        if( !AddedKeys.Contains( CurrentNodeKey ) )
                        {
                            AddedKeys.Add( CurrentNodeKey );
                            CswNbtNode CurrentGeneratorNode = Master.CswNbtResources.Nodes[CurrentNodeKey.NodeId];
                            TotalNodes += CswNbtActGenerateFutureNodes.makeNodes( CurrentGeneratorNode, _EndDatePicker.SelectedDate );
                            SelectedGeneratorNodes.Add( CurrentGeneratorNode );
                        }

                    }//iterate selected Generator notes

                    if( TotalNodes > 0 )
                    {
                        if( TotalNodes == 1 )
                            _ViewFutureNodesLabel.Text = TotalNodes.ToString() + " node was created:";
                        else
                            _ViewFutureNodesLabel.Text = TotalNodes.ToString() + " nodes were created:";

                        CswNbtView NodesView = CswNbtActGenerateFutureNodes.getTreeViewOfFutureNodes( SelectedGeneratorNodes );
                        ICswNbtTree NodesTree = Master.CswNbtResources.Trees.getTreeFromView( NodesView, true, true, false, false );
                        string Xml = NodesTree.getTreeAsXml();
                        _NodesTreeOfFutureNodes.LoadXml( Xml );
                        _NodesTreeOfFutureNodes.ExpandAllNodes();

                        //bz # 6141. See notes in bz # 6521 
                        //string ViewXml = NodesView.ToString();
                        Master.setSessionViewId( NodesView.SessionViewId, true );
                    }
                    else
                    {
                        _ViewFutureNodesLabel.Text = "No nodes were created";
                        _NodesTreeOfFutureNodes.Visible = false;
                    }
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }//_OnWizardStepChange()

        protected void HandleCancel( object sender, CswWizardEventArgs e )
        {
            try
            {
                Master.Redirect( "Main.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                _CheckAllLink.OnClientClick = "return setCheckAll('" + _NodesTreeOfGeneratorNodes.ClientID + "', true );";
                _UnCheckAllLink.OnClientClick = "return setCheckAll('" + _NodesTreeOfGeneratorNodes.ClientID + "', false );";
            
                if( FutureSchedulingWizard.CurrentStep == 2 )
                {
                    // Too late to cancel
                    FutureSchedulingWizard.CancelButton.Visible = false;
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnPreRender( e );

        }//OnPreRender()



        private void _initGeneratorNodeTree()
        {
            if( _NodesTreeOfGeneratorNodes.Nodes.Count <= 0 )
            {
                CswNbtView GeneratorView = Master.CswNbtResources.Trees.getTreeViewOfObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                string temp = GeneratorView.ToString();
                GeneratorView.ViewName = "Generators";
                ICswNbtTree GeneratorTree = Master.CswNbtResources.Trees.getTreeFromView( GeneratorView, true, true, false, false );
                string Xml = GeneratorTree.getTreeAsXml();
                _NodesTreeOfGeneratorNodes.LoadXml( Xml );
                _NodesTreeOfGeneratorNodes.ExpandAllNodes();
                _setCheckBoxes();
            }
        }//_initGeneratorNodeTree()

        private void _recurseSetCheckBox( RadTreeNode StartNode )
        {
            foreach( RadTreeNode CurrentNode in StartNode.Nodes )
            {
                if( CurrentNode.Nodes.Count > 0 )
                {
                    _recurseSetCheckBox( CurrentNode );
                }
            }//iterate current node's nodes

            //Currently the only way to get check boxes to show up
            //is to set the check box property on the radtreeview. 
            //accordingly, I've reversed the logic here so that 
            //"checked" means -- leave it checked
            CswNbtNodeKey CswNbtNodeKey = new CswNbtNodeKey( Master.CswNbtResources, StartNode.Value );
            if( Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass ).ObjectClassId == CswNbtNodeKey.ObjectClassId )
            {
                StartNode.Checkable = true;
                //If you set the checkbox state on the server, the client's
                //setting of the checkbox gets hosed.
                //  StartNode.Checked = CheckboxStateChecked;
            }
            else
            {
                StartNode.Checkable = false;
            }

        }//_recurseSetCheckBox

        private void _setCheckBoxes()
        {
            if( null != _NodesTreeOfGeneratorNodes )
            {
                foreach( RadTreeNode CurrentRootLevelNode in _NodesTreeOfGeneratorNodes.Nodes )
                {
                    _recurseSetCheckBox( CurrentRootLevelNode );
                }
            }
        }//_setCheckBoxes()
    }//Act_FutureScheduling

}//ChemSW.Nbt.WebPages

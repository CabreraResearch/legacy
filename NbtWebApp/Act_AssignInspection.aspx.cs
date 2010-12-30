using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

using Telerik.Web.UI;
using ChemSW.CswWebControls;
using ChemSW.NbtWebControls.FieldTypes;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_AssignInspection : System.Web.UI.Page
    {
        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        protected override void CreateChildControls()
        {
            AssignInspectionWizard.onCancel += new CswWizard.CswWizardEventHandler( AssignInspectionWizard_onCancel );
            AssignInspectionWizard.OnError += new CswErrorHandler( Master.HandleError );
            AssignInspectionWizard.onFinish += new CswWizard.CswWizardEventHandler( AssignInspectionWizard_onFinish );
            AssignInspectionWizard.onPageChange += new CswWizard.CswWizardEventHandler( AssignInspectionWizard_onPageChange );

            CreateSelectInspectionStep();
            CreateChooseViewOfTargetsStep();
            CreateChooseTargetsStep();
            CreateSetScheduleStep();

            base.CreateChildControls();
        }

        private DropDownList InspectionDropDown;
        private void CreateSelectInspectionStep()
        {
            CswAutoTable SelectInspectionStepTable = new CswAutoTable();
            SelectInspectionStepTable.ID = "SelectInspectionStepTable";
            SelectInspectionStepTable.OddCellRightAlign = true;
            Step1PH.Controls.Add( SelectInspectionStepTable );

            Literal InspectionLiteral = new Literal();
            InspectionLiteral.Text = "Inspection:";
            SelectInspectionStepTable.addControl( 0, 0, InspectionLiteral );

            InspectionDropDown = new DropDownList();
            InspectionDropDown.ID = "InspectionDropDown";
            InspectionDropDown.CssClass = "selectinput";

            foreach( CswNbtMetaDataNodeType InspectionNodeType in Master.CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                if( InspectionNodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                {
                    InspectionDropDown.Items.Add( new ListItem( InspectionNodeType.NodeTypeName, InspectionNodeType.NodeTypeId.ToString() ) );
                }
            }
            if( CswTools.IsInteger( Session["AssignInspection_SelectedInspectionId"] ) )
                InspectionDropDown.SelectedValue = Session["AssignInspection_SelectedInspectionId"].ToString();

            SelectInspectionStepTable.addControl( 0, 1, InspectionDropDown );
        }

        private DropDownList TargetViewDropDown;
        private void CreateChooseViewOfTargetsStep()
        {
            CswAutoTable ChooseViewOfTargetsStepTable = new CswAutoTable();
            ChooseViewOfTargetsStepTable.ID = "ChooseViewOfTargetsStepTable";
            Step2PH.Controls.Add( ChooseViewOfTargetsStepTable );

            Literal TargetViewLiteral = new Literal();
            TargetViewLiteral.Text = "Choose a view of inspection targets:";
            ChooseViewOfTargetsStepTable.addControl( 0, 0, TargetViewLiteral );

            TargetViewDropDown = new DropDownList();
            TargetViewDropDown.ID = "TargetViewDropDown";
            TargetViewDropDown.CssClass = "selectinput";
            ChooseViewOfTargetsStepTable.addControl( 0, 1, TargetViewDropDown );
        }

        private RadTreeView TargetTreeView;
        private void CreateChooseTargetsStep()
        {
            CswAutoTable ChooseTargetsStepTable = new CswAutoTable();
            ChooseTargetsStepTable.ID = "ChooseTargetsStepTable";
            Step3PH.Controls.Add( ChooseTargetsStepTable );

            TargetTreeView = new RadTreeView();
            TargetTreeView.ID = "TestsTreeView";
            TargetTreeView.CheckBoxes = true;
            TargetTreeView.EnableEmbeddedSkins = false;
            TargetTreeView.Skin = "ChemSW";
            ChooseTargetsStepTable.addControl( 0, 0, TargetTreeView );
        }

        private RadioButton OnceRadio;
        private RadioButton ScheduleRadio;
        //private CswDatePicker StartDatePicker;
        private CswDatePicker EndDatePicker;
        private CswTimeIntervalSelector DueDateInterval;
        private CswAutoTable SubTable;
        private void CreateSetScheduleStep()
        {
            CswAutoTable SetScheduleStepTable = new CswAutoTable();
            SetScheduleStepTable.ID = "SetScheduleStepTable";
            Step4PH.Controls.Add( SetScheduleStepTable );

            OnceRadio = new RadioButton();
            OnceRadio.ID = "OnceRadio";
            OnceRadio.GroupName = "ScheduleStepGroup";
            OnceRadio.Text = "One Inspection Per Target Only";
            OnceRadio.Checked = true;
            SetScheduleStepTable.addControl( 0, 0, OnceRadio );

            ScheduleRadio = new RadioButton();
            ScheduleRadio.ID = "ScheduleRadio";
            ScheduleRadio.GroupName = "ScheduleStepGroup";
            ScheduleRadio.Text = "Recurring On The Following Schedule:";
            SetScheduleStepTable.addControl( 1, 0, ScheduleRadio );

            SubTable = new CswAutoTable();
            SubTable.ID = "SubTable";
            SubTable.OddCellRightAlign = true;
            SubTable.Style.Add( HtmlTextWriterStyle.Display, "none" );
            SetScheduleStepTable.addControl( 2, 0, SubTable );

            //Literal StartDateLiteral = new Literal();
            //StartDateLiteral.Text = "Initial Due Date:";
            //SubTable.addControl( 0, 0, StartDateLiteral );

            //StartDatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, false );
            //StartDatePicker.ID = "StartDatePicker";
            //StartDatePicker.SelectedDate = DateTime.Today;
            //SubTable.addControl( 0, 1, StartDatePicker );

            Literal EndDateLiteral = new Literal();
            EndDateLiteral.Text = "Final Due Date:";
            SubTable.addControl( 1, 0, EndDateLiteral );

            EndDatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, false );
            EndDatePicker.ID = "EndDatePicker";
            EndDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            SubTable.addControl( 1, 1, EndDatePicker );

            Literal DueDateIntervalLiteral = new Literal();
            DueDateIntervalLiteral.Text = "Due Date Interval:";
            SubTable.addControl( 2, 0, DueDateIntervalLiteral );

            DueDateInterval = new CswTimeIntervalSelector( false );
            DueDateInterval.ID = "DueDateInterval";
            SubTable.addControl( 2, 1, DueDateInterval );
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                ScheduleRadio.Attributes.Add( "onclick", "AssignInspection_ScheduleRadio_Click('" + SubTable.ClientID + "');" );
                OnceRadio.Attributes.Add( "onclick", "AssignInspection_OnceRadio_Click('" + SubTable.ClientID + "');" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnPreRender( e );
        }

        #endregion Page Lifecycle

        #region Events

        void AssignInspectionWizard_onPageChange( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                switch( CswWizardEventArgs.NewPage )
                {
                    case 2:
                        TargetViewDropDown.Items.Clear();
                        DataTable Views = null;
                        //if( Master.CswNbtResources.CurrentUser.IsAdministrator() )
                        Views = Master.CswNbtResources.ViewSelect.getVisibleViews( false );
                        //else
                        //Views = CswNbtView.getUserViews( Master.CswNbtResources );
                        if( Views.Rows.Count > 0 )
                        {
                            foreach( DataRow Row in Views.Rows )
                            {
                                TargetViewDropDown.Items.Add( new ListItem( Row["viewname"].ToString(), Row["nodeviewid"].ToString() ) );
                            }
                        }
                        break;
                    case 3:
                        CswNbtView ViewToLoad = (CswNbtView) CswNbtViewFactory.restoreView( Master.CswNbtResources, CswConvert.ToInt32( TargetViewDropDown.SelectedValue ) );
                        ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( ViewToLoad, true, true, false, false );
                        string Xml = Tree.getTreeAsXml();
                        TargetTreeView.LoadXml( Xml );
                        TargetTreeView.ExpandAllNodes();
                        break;
                    case 4:
                        CswNbtMetaDataNodeType InspectionNodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( InspectionDropDown.SelectedValue ) );
                        CswNbtMetaDataNodeTypeProp TargetProp = InspectionNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );

                        Collection<CswNbtNodeKey> CheckedNodes = new Collection<CswNbtNodeKey>();
                        _FetchCheckedNodes( ref CheckedNodes, TargetProp, TargetTreeView );

                        if( CheckedNodes.Count == 0 )
                        {
                            Literal ErrorMessage = new Literal();
                            string TargetText = string.Empty;
                            if( TargetProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                            {
                                CswNbtMetaDataNodeType TargetNodeType = Master.CswNbtResources.MetaData.getNodeType( TargetProp.FKValue );
                                if( TargetNodeType != null )
                                    TargetText = TargetNodeType.NodeTypeName;
                            }
                            else
                            {
                                CswNbtMetaDataObjectClass TargetObjectClass = Master.CswNbtResources.MetaData.getObjectClass( TargetProp.FKValue );
                                if( TargetObjectClass != null )
                                    TargetText = TargetObjectClass.ObjectClass.ToString();
                            }
                            ErrorMessage.Text = "You must select at least one valid target " + TargetText + " for this inspection";
                            Step3PH.Controls.Add( ErrorMessage );
                            AssignInspectionWizard.CurrentStep = 3;
                        }
                        break;
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void AssignInspectionWizard_onFinish( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                CswNbtMetaDataNodeType InspectionNodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( InspectionDropDown.SelectedValue ) );
                CswNbtMetaDataNodeTypeProp TargetProp = InspectionNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );

                Collection<CswNbtNodeKey> CheckedNodes = new Collection<CswNbtNodeKey>();
                _FetchCheckedNodes( ref CheckedNodes, TargetProp, TargetTreeView );

                Collection<CswPrimaryKey> NewChildNodeIds = new Collection<CswPrimaryKey>();
                CswNbtMetaDataNodeType DefaultGeneratorNodeType = null;
                foreach( CswNbtNodeKey TargetNodeKey in CheckedNodes )
                {
                    if( ScheduleRadio.Checked )
                    {
                        // Make a Generator node on this target, using a default Generator NodeType
                        CswNbtMetaDataObjectClass GeneratorObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                        CswNbtMetaDataObjectClass.NbtObjectClass TargetObjectClass = CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass;
                        if( TargetProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                        {
                            TargetObjectClass = Master.CswNbtResources.MetaData.getNodeType( TargetProp.FKValue ).ObjectClass.ObjectClass;
                        }
                        else
                        {
                            TargetObjectClass = Master.CswNbtResources.MetaData.getObjectClass( TargetProp.FKValue ).ObjectClass;
                        }
                        string DefaultGeneratorNodeTypeName = "Inspection Generator for " + TargetObjectClass.ToString();
                        DefaultGeneratorNodeType = Master.CswNbtResources.MetaData.getNodeType( DefaultGeneratorNodeTypeName );
                        if( DefaultGeneratorNodeType == null )
                        {
                            // make one
                            DefaultGeneratorNodeType = Master.CswNbtResources.MetaData.makeNewNodeType( GeneratorObjectClass.ObjectClassId, DefaultGeneratorNodeTypeName, string.Empty );
                            DefaultGeneratorNodeType.NameTemplateText = "Inspection from [Initial Due Date] to [Final Due Date]";
                            CswNbtMetaDataNodeTypeProp OwnerProp = DefaultGeneratorNodeType.getNodeTypeProp( CswNbtObjClassGenerator.OwnerPropertyName );
                            // the owner of the generator is the target of the inspection
                            OwnerProp.SetFK( TargetProp.IsFK, TargetProp.FKType, TargetProp.FKValue, string.Empty, Int32.MinValue );
                        }

                        CswNbtNode NewGeneratorNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( DefaultGeneratorNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
                        CswNbtObjClassGenerator NewGeneratorObjClass = CswNbtNodeCaster.AsGenerator( NewGeneratorNode );
                        NewGeneratorObjClass.DueDateInterval.RateInterval = DueDateInterval.RateInterval;
                        NewGeneratorObjClass.FinalDueDate.DateValue = EndDatePicker.SelectedDate;
                        //NewGeneratorObjClass.InitialDueDate.DateValue = StartDatePicker.SelectedDate;
                        NewGeneratorObjClass.Owner.RelatedNodeId = TargetNodeKey.NodeId;
                        NewGeneratorObjClass.Owner.RefreshNodeName();
                        NewGeneratorObjClass.TargetType.SelectedNodeTypeIds[0] = InspectionNodeType.FirstVersionNodeTypeId.ToString();
                        NewGeneratorObjClass.Enabled.Checked = Tristate.True;
                        NewGeneratorNode.postChanges( true );

                        NewChildNodeIds.Add( NewGeneratorNode.NodeId );
                    }
                    else
                    {
                        // Make an inspection on this target
                        CswNbtNode NewInspectionNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( InspectionNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
                        CswNbtObjClassInspectionDesign NewInspectionObjClass = CswNbtNodeCaster.AsInspectionDesign( NewInspectionNode );
                        NewInspectionObjClass.Target.RelatedNodeId = TargetNodeKey.NodeId;
                        NewInspectionObjClass.GeneratedDate.DateValue = DateTime.Today;
                        NewInspectionNode.postChanges( true );

                        NewChildNodeIds.Add( NewInspectionNode.NodeId );
                    }
                }

                // Make a view of new inspections
                CswNbtView NewView = _makeNewInspectionsView( CheckedNodes, NewChildNodeIds, TargetProp, DefaultGeneratorNodeType );
                Master.setViewXml( NewView.ToString() );
                Master.Redirect( "Main.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void AssignInspectionWizard_onCancel( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
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

        #endregion Events



        private CswNbtView _makeNewInspectionsView( Collection<CswNbtNodeKey> TargetNodeKeys, Collection<CswPrimaryKey> ChildNodeIDs, CswNbtMetaDataNodeTypeProp TargetProp, CswNbtMetaDataNodeType DefaultGeneratorNodeType )
        {
            CswNbtMetaDataNodeType InspectionNodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( InspectionDropDown.SelectedValue ) );

            CswNbtView View = new CswNbtView( Master.CswNbtResources );
            View.ViewName = "New Inspections";

            CswNbtViewRelationship TargetRel;
            if( TargetProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
            {
                CswNbtMetaDataNodeType TargetNodeType = Master.CswNbtResources.MetaData.getNodeType( TargetProp.FKValue );
                TargetRel = View.AddViewRelationship( TargetNodeType, false );
            }
            else //if( TargetProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
            {
                CswNbtMetaDataObjectClass TargetObjectClass = Master.CswNbtResources.MetaData.getObjectClass( TargetProp.FKValue );
                TargetRel = View.AddViewRelationship( TargetObjectClass, false );
            }
            foreach( CswNbtNodeKey TargetNodeKey in TargetNodeKeys )
                TargetRel.NodeIdsToFilterIn.Add( TargetNodeKey.NodeId );

            CswNbtViewRelationship SecondRel;
            if( ScheduleRadio.Checked )
            {
                CswNbtMetaDataNodeTypeProp OwnerProp = DefaultGeneratorNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                SecondRel = View.AddViewRelationship( TargetRel, CswNbtViewRelationship.PropOwnerType.Second, OwnerProp, false );
                //SecondRel.setSecond( DefaultGeneratorNodeType );
                foreach( CswPrimaryKey GeneratorNodeId in ChildNodeIDs )
                    SecondRel.NodeIdsToFilterIn.Add( GeneratorNodeId );
            }
            else
            {
                SecondRel = View.AddViewRelationship( TargetRel, CswNbtViewRelationship.PropOwnerType.Second, TargetProp, false );
                //SecondRel.setSecond( InspectionNodeType );
                foreach( CswPrimaryKey InspectionNodeId in ChildNodeIDs )
                    SecondRel.NodeIdsToFilterIn.Add( InspectionNodeId );

                // BZ 8050
                foreach( CswNbtMetaDataNodeTypeProp InspectionProp in InspectionNodeType.NodeTypeProps )
                {
                    if( InspectionProp.UseNumbering )
                    {
                        CswNbtViewProperty ViewProp = View.AddViewProperty( SecondRel, InspectionProp );
                    }
                }
            }

            return View;
        }

        // If a non-matching parent is checked, assume all immediate children of that parent are checked
        private void _FetchCheckedNodes( ref Collection<CswNbtNodeKey> NodeKeyCollection, CswNbtMetaDataNodeTypeProp TargetProp, RadTreeView TreeView )
        {
            foreach( RadTreeNode ChildTreeNode in TreeView.Nodes )
            {
                _FetchCheckedNodesRecursive( ref NodeKeyCollection, TargetProp, TreeView, ChildTreeNode, ChildTreeNode.Checked );
            }
        }
        private void _FetchCheckedNodesRecursive( ref Collection<CswNbtNodeKey> NodeKeyCollection, CswNbtMetaDataNodeTypeProp TargetProp, RadTreeView TreeView, RadTreeNode CurrentTreeNode, bool IncludeAllChildren )
        {
            CswNbtNodeKey CurrentNodeKey = new CswNbtNodeKey( Master.CswNbtResources, CurrentTreeNode.Value );
            if( ( ( TargetProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() && CurrentNodeKey.NodeTypeId == TargetProp.FKValue ) ||
                  ( TargetProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() && CurrentNodeKey.ObjectClassId == TargetProp.FKValue ) ) &&
                ( CurrentTreeNode.Checked || IncludeAllChildren ) )
            {
                NodeKeyCollection.Add( CurrentNodeKey );
            }
            // recurse
            foreach( RadTreeNode ChildTreeNode in CurrentTreeNode.Nodes )
            {
                _FetchCheckedNodesRecursive( ref NodeKeyCollection, TargetProp, TreeView, ChildTreeNode, ChildTreeNode.Checked || IncludeAllChildren );
            }
        }

    }
}

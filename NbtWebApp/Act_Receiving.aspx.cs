using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;

using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using FarPoint.Web.Spread;
using ChemSW.CswWebControls;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_Receiving : System.Web.UI.Page
    {
        #region Page Lifecycle

        private CswNbtNodeKey _MaterialNodeKey = null;
        private CswNbtNodeKey MaterialNodeKey
        {
            get
            {
                if( _MaterialNodeKey == null )
                {
                    //if( MaterialTree != null && MaterialTree.SelectedValue != null && MaterialTree.SelectedValue != string.Empty )
                    //    _MaterialNodeKey = new CswNbtNodeKey( Master.CswNbtResources, MaterialTree.SelectedValue );
                    if( MaterialGrid != null && MaterialGrid.Grid != null && MaterialGrid.Grid.SelectedValue != null && MaterialGrid.Grid.SelectedValue.ToString() != string.Empty )
                        _MaterialNodeKey = new CswNbtNodeKey( Master.CswNbtResources, MaterialGrid.Grid.SelectedValue.ToString() );
                }
                return _MaterialNodeKey;
            }
        }

        private CswNbtNodeKey _SelectedPackDetailNodeKey = null;
        private CswNbtNodeKey SelectedPackDetailNodeKey
        {
            get
            {
                if( _SelectedPackDetailNodeKey == null )
                {
                    if( SizeGrid != null && SizeGrid.Grid != null && SizeGrid.Grid.SelectedValue != null && SizeGrid.Grid.SelectedValue.ToString() != string.Empty )
                        _SelectedPackDetailNodeKey = new CswNbtNodeKey( Master.CswNbtResources, SizeGrid.Grid.SelectedValue.ToString() );
                }
                return _SelectedPackDetailNodeKey;
            }
        }
        private CswNbtNode _SelectedPackDetailNode = null;
        private CswNbtNode SelectedPackDetailNode
        {
            get
            {
                if( _SelectedPackDetailNode == null )
                {
                    if( SelectedPackDetailNodeKey != null )
                        _SelectedPackDetailNode = Master.CswNbtResources.Nodes[SelectedPackDetailNodeKey];
                }
                return _SelectedPackDetailNode;
            }
        }

        private string _Units = string.Empty;
        private string Units
        {
            get
            {
                if( _Units == string.Empty )
                {
                    if( SelectedPackDetailNode != null )
                    {
                        CswNbtNodePropRelationship UnitProp = SelectedPackDetailNode.Properties[PackDetailUnitProp].AsRelationship;
                        UnitProp.RefreshNodeName();
                        _Units = UnitProp.CachedNodeName;
                    }
                }
                return _Units;
            }
        }


        private CswNbtMetaDataNodeType MaterialNodeType;
        private CswNbtMetaDataNodeType PackageNodeType;
        private CswNbtMetaDataNodeType PackDetailNodeType;
        private CswNbtMetaDataNodeType ContainerNodeType;
        private CswNbtMetaDataNodeTypeProp MaterialNameProp;
        private CswNbtMetaDataNodeTypeProp PackageMaterialProp;
        private CswNbtMetaDataNodeTypeProp PackDetailPackageProp;
        private CswNbtMetaDataNodeTypeProp PackDetailUnitProp;
        private CswNbtMetaDataNodeTypeProp PackDetailCapacityProp;
        private CswNbtMetaDataNodeTypeProp ContainerPackDetailProp;
        private CswNbtMetaDataNodeTypeProp ContainerQtyProp;
        private CswNbtMetaDataNodeTypeProp MaterialSubClassProp;
        private CswNbtMetaDataNodeTypeProp MaterialCasNoProp;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();

                // These will need to change eventually
                MaterialNodeType = Master.CswNbtResources.MetaData.getNodeType( "Chemical Material" );
                PackageNodeType = Master.CswNbtResources.MetaData.getNodeType( "Package" );
                PackDetailNodeType = Master.CswNbtResources.MetaData.getNodeType( "PackDetail" );
                ContainerNodeType = Master.CswNbtResources.MetaData.getNodeType( "Container" );

                // These too
                MaterialNameProp = MaterialNodeType.getNodeTypeProp( "Material Name" );
                MaterialSubClassProp = MaterialNodeType.getNodeTypeProp( "CAS No" );
                MaterialCasNoProp = MaterialNodeType.getNodeTypeProp( "Material Subclass" );
                PackageMaterialProp = PackageNodeType.getNodeTypeProp("Material");
                PackDetailPackageProp = PackDetailNodeType.getNodeTypeProp( "Package" );
                PackDetailUnitProp = PackDetailNodeType.getNodeTypeProp( "Units" );
                PackDetailCapacityProp = PackDetailNodeType.getNodeTypeProp( "Capacity" );
                ContainerPackDetailProp = ContainerNodeType.getNodeTypeProp( "Catalog No" );
                ContainerQtyProp = ContainerNodeType.getNodeTypeProp( "Net Quantity" );

                // This is in init so that the PropFilter has a chance to init() itself first
                MaterialSearchFilter.Set( MaterialNodeType.NodeTypeId, MaterialNameProp.PropName );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                // For Define Container Grid:
                if( SelectedPackDetailNode != null )
                {
                    // Initial size - first cell only
                    CswNbtNodePropNumber CapacityProp = SelectedPackDetailNode.Properties[PackDetailCapacityProp].AsNumber;
                    Sheet1.Cells[0, ContainerQtyColumn].Text = CapacityProp.Value.ToString();

                    // Units - every cell
                    Sheet1.Cells[0, ContainerUnitColumn, numRows - 1, ContainerUnitColumn].Text = Units;
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        private CswWizard _Wizard;
        protected override void CreateChildControls()
        {
            _Wizard = new CswWizard();
            _Wizard.ID = "ReceivingWizard";
            _Wizard.WizardTitle = "Receiving";
            _Wizard.onCancel += new CswWizard.CswWizardEventHandler( _Wizard_onCancel );
            _Wizard.OnError += new CswErrorHandler( Master.HandleError );
            _Wizard.onPageChange += new CswWizard.CswWizardEventHandler( _Wizard_onPageChange );
            _Wizard.onFinish += new CswWizard.CswWizardEventHandler( _Wizard_onFinish );

            CreateSearchMaterialStep();
            CreateSelectMaterialStep();
            CreateChooseSizeStep();
            CreateDefineContainersStep();

            ph.Controls.Add( _Wizard );
        }

        private CswWizardStep SearchMaterialStep;
        private CswPropertyFilter MaterialSearchFilter;
        private void CreateSearchMaterialStep()
        {
            SearchMaterialStep = new CswWizardStep();
            SearchMaterialStep.ID = "ReceivingWizard_SearchMaterialStep";
            SearchMaterialStep.Step = 1;
            SearchMaterialStep.Title = "Search for Material";
            _Wizard.WizardSteps.Add( SearchMaterialStep );

            CswAutoTable SearchMaterialStepTable = new CswAutoTable();
            SearchMaterialStepTable.ID = "SearchMaterialStepTable";
            //SearchMaterialStepTable.OddCellRightAlign = true;
            SearchMaterialStep.Controls.Add( SearchMaterialStepTable );

            MaterialSearchFilter = new CswPropertyFilter( Master.CswNbtResources, Master.AjaxManager, null, false, true, false, false );
            MaterialSearchFilter.ShowSubFieldAndMode = true;
            MaterialSearchFilter.ID = "MaterialSearchFilter";
            MaterialSearchFilter.OnError += new CswErrorHandler( Master.HandleError ); 
            MaterialSearchFilter.UseCheckChanges = false;
            //SearchMaterialStepTable.addControl( 0, 0, MaterialSearchFilter );
            SearchMaterialStepTable.Rows.Add( MaterialSearchFilter ); 
        }

        private CswWizardStep SelectMaterialStep;
        private CswNodesGrid MaterialGrid;
        //private RadTreeView MaterialTree;
        private void CreateSelectMaterialStep()
        {
            SelectMaterialStep = new CswWizardStep();
            SelectMaterialStep.ID = "ReceivingWizard_SelectMaterialStep";
            SelectMaterialStep.Step = 2;
            SelectMaterialStep.Title = "Select Material";
            _Wizard.WizardSteps.Add( SelectMaterialStep );

            CswAutoTable SelectMaterialStepTable = new CswAutoTable();
            SelectMaterialStepTable.ID = "SelectMaterialStepTable";
            //SelectMaterialStepTable.OddCellRightAlign = true;
            SelectMaterialStep.Controls.Add( SelectMaterialStepTable );

            MaterialGrid = new CswNodesGrid( Master.CswNbtResources );
            MaterialGrid.OnError += new CswErrorHandler( Master.HandleError );
            MaterialGrid.ID = "MaterialGrid";
            MaterialGrid.Grid.ClientSettings.Selecting.AllowRowSelect = true;
            //MaterialGrid.Grid.ClientSettings.ClientEvents.OnRowClick = "MainGrid_RowClick";
            SelectMaterialStepTable.addControl( 0, 0, MaterialGrid );

            //MaterialTree = new RadTreeView();
            //MaterialTree.ID = "ReceivingMaterialTree";
            //MaterialTree.Height = 350;
            //MaterialTree.Width = 270;
            //MaterialTree.WebServiceSettings.Method = "GetNodeChildren";
            //MaterialTree.WebServiceSettings.Path = "TreeViewService.asmx";
            //MaterialTree.EnableEmbeddedSkins = false;
            //MaterialTree.Skin = "ChemSW";
            //SelectMaterialStepTable.addControl( 0, 0, MaterialTree );
        }

        private CswWizardStep ChooseSizeStep;
        private CswNodesGrid SizeGrid;
        private void CreateChooseSizeStep()
        {
            ChooseSizeStep = new CswWizardStep();
            ChooseSizeStep.ID = "ReceivingWizard_ChooseSizeStep";
            ChooseSizeStep.Step = 3;
            ChooseSizeStep.Title = "Choose a Size";
            _Wizard.WizardSteps.Add( ChooseSizeStep );

            CswAutoTable ChooseSizeStepTable = new CswAutoTable();
            ChooseSizeStepTable.ID = "ChooseSizeStepTable";
            ChooseSizeStepTable.OddCellRightAlign = true;
            ChooseSizeStep.Controls.Add( ChooseSizeStepTable );

            SizeGrid = new CswNodesGrid( Master.CswNbtResources );
            SizeGrid.OnError += new CswErrorHandler( Master.HandleError );
            SizeGrid.ID = "SizeGrid";
            SizeGrid.Grid.ClientSettings.Selecting.AllowRowSelect = true;
            //SizeGrid.Grid.ClientSettings.ClientEvents.OnRowClick = "MainGrid_RowClick";
            ChooseSizeStepTable.addControl( 0, 0, SizeGrid );

 
        }

        private CswWizardStep DefineContainersStep;
        private FpSpread ContainersSpread;
        private SheetView Sheet1;

        private Int32 ContainerCountColumn = 0;
        private Int32 ContainerQtyColumn = 1;
        private Int32 ContainerUnitColumn = 2;
        private Int32 numColumns = 3;
        private Int32 numRows = 20;
        private void CreateDefineContainersStep()
        {
            DefineContainersStep = new CswWizardStep();
            DefineContainersStep.ID = "ReceivingWizard_DefineContainersStep";
            DefineContainersStep.Step = 4;
            DefineContainersStep.Title = "Define Containers";
            _Wizard.WizardSteps.Add( DefineContainersStep );

            CswAutoTable DefineContainersStepTable = new CswAutoTable();
            DefineContainersStepTable.ID = "DefineContainersStepTable";
            DefineContainersStepTable.OddCellRightAlign = true;
            DefineContainersStep.Controls.Add( DefineContainersStepTable );

            ContainersSpread = new FpSpread();
            ContainersSpread.ID = "ContainersSpread";
            ContainersSpread.Height = 300;
            ContainersSpread.Width = 400;
            ContainersSpread.EditModeReplace = true;
            ContainersSpread.ActiveSheetViewIndex = 0;
            ContainersSpread.EnableAjaxCall = true;
            ContainersSpread.UpdateCommand += new SpreadCommandEventHandler( QuestionSpread_UpdateCommand );
            DefineContainersStepTable.addControl( 0, 0, ContainersSpread );

            Sheet1 = new SheetView();
            Sheet1.SheetName = "Sheet1";
            Sheet1.AllowInsert = true;
            Sheet1.AllowDelete = true;
            Sheet1.AutoGenerateColumns = false;
            ContainersSpread.Sheets.Add( Sheet1 );

            // clear the sheet
            Sheet1.RemoveRows( 0, Sheet1.Rows.Count );
            Sheet1.RemoveColumns( 0, Sheet1.Columns.Count );

            Sheet1.SelectionBackColor = System.Drawing.Color.FromArgb( 255, 255, 0 );
            Sheet1.LockBackColor = System.Drawing.Color.FromArgb( 200, 200, 200 );

            Sheet1.Columns.Add( 0, numColumns );
            Sheet1.Rows.Count = numRows;
            Sheet1.PageSize = 500;

            Sheet1.Columns[ContainerCountColumn].Label = "Count";
            Sheet1.Columns[ContainerQtyColumn].Label = "Qty Per";
            Sheet1.Columns[ContainerUnitColumn].Label = "Unit";
            Sheet1.Columns[ContainerUnitColumn].Locked = true;
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                if( _Wizard.CurrentStep == 2 )
                    _Wizard.NextButton.OnClientClick = "return Spread_Update_Click('" + ContainersSpread.ClientID + "_Update');";
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnPreRender( e );
        }

        #endregion Page Lifecycle

        #region Events

        protected void QuestionSpread_UpdateCommand( object sender, SpreadCommandEventArgs e )
        {
            try
            {
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void _Wizard_onPageChange( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                if( CswWizardEventArgs.NewPage == 2 )
                {
                    CswNbtView MaterialView = MaterialSearchFilter.MakeView();
                    CswNbtViewRelationship MaterialRel = MaterialView.Root.ChildRelationships[0];

                    CswNbtViewProperty MaterialNameViewProp = MaterialView.AddViewProperty( MaterialRel, MaterialNameProp );
                    //MaterialNameViewProp.setProp( MaterialNameProp );
                    //if(!MaterialRel.Properties.Contains(MaterialNameViewProp))
                    //    MaterialRel.Properties.Add(MaterialNameViewProp);

                    CswNbtViewProperty MaterialSubClassViewProp = MaterialView.AddViewProperty( MaterialRel,MaterialSubClassProp );
                    //MaterialSubClassViewProp.setProp( MaterialSubClassProp );
                    //if( !MaterialRel.Properties.Contains( MaterialSubClassViewProp ) )
                    //    MaterialRel.Properties.Add( MaterialSubClassViewProp );

                    CswNbtViewProperty MaterialCasNoViewProp = MaterialView.AddViewProperty( MaterialRel, MaterialCasNoProp );
                    //MaterialCasNoViewProp.setProp( MaterialCasNoProp );
                    //if( !MaterialRel.Properties.Contains( MaterialCasNoViewProp ) )
                    //    MaterialRel.Properties.Add( MaterialCasNoViewProp );

                    ICswNbtTree CswNbtTree = Master.CswNbtResources.Trees.getTreeFromView( MaterialView, false, true, false, false );

                    MaterialGrid.View = MaterialView;
                    MaterialGrid.DataBind();

                    //string XmlStr = CswNbtTree.getTreeAsXml();
                    //MaterialTree.LoadXml( XmlStr );
                    //MaterialTree.ExpandAllNodes();
                }
                if( CswWizardEventArgs.NewPage == 3 )
                {
                    CswNbtView SizeView = new CswNbtView( Master.CswNbtResources );

                    CswNbtViewRelationship PackDetailRel = SizeView.AddViewRelationship( PackDetailNodeType, true );
                    CswNbtViewRelationship PackagesRel = SizeView.AddViewRelationship( PackDetailRel, CswNbtViewRelationship.PropOwnerType.First, PackDetailPackageProp, true );
                    CswNbtViewRelationship MaterialRel = SizeView.AddViewRelationship( PackagesRel, CswNbtViewRelationship.PropOwnerType.First, PackageMaterialProp, true );
                    MaterialRel.NodeIdsToFilterIn.Add( MaterialNodeKey.NodeId );

                    CswNbtViewProperty PackDetailCapacityViewProp = SizeView.AddViewProperty( PackDetailRel, PackDetailNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassPackDetail.CapacityPropertyName ) );
                    CswNbtViewProperty PackDetailUnitsViewProp = SizeView.AddViewProperty( PackDetailRel, PackDetailNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassPackDetail.UnitsPropertyName ) );
                    CswNbtViewProperty PackDetailCatalogNoViewProp = SizeView.AddViewProperty( PackDetailRel, PackDetailNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassPackDetail.CatalogNoPropertyName ) );
                    CswNbtViewProperty PackDetailPackageDescriptionViewProp = SizeView.AddViewProperty( PackDetailRel, PackDetailNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassPackDetail.PackageDescriptionPropertyName ) );
                    
                    SizeGrid.View = SizeView;
                    SizeGrid.DataBind();
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

        }

        void _Wizard_onCancel( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
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



        void _Wizard_onFinish( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            try
            {
                Collection<CswPrimaryKey> NewContainerNodePks = new Collection<CswPrimaryKey>();
                for( Int32 r = 0; r < Sheet1.Rows.Count; r++ )
                {
                    string ContainerCountString = Sheet1.Cells[r, ContainerCountColumn].Text;
                    string ContainerQtyString = Sheet1.Cells[r, ContainerQtyColumn].Text;
                    if( CswTools.IsInteger( ContainerCountString ) && CswTools.IsFloat( ContainerQtyString ) )
                    {
                        for( Int32 c = 0; c < CswConvert.ToInt32( ContainerCountString ); c++ )
                        {
                            CswNbtNode NewContainerNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                            NewContainerNode.Properties[ContainerPackDetailProp].AsRelationship.RelatedNodeId = SelectedPackDetailNodeKey.NodeId;
                            NewContainerNode.Properties[ContainerPackDetailProp].AsRelationship.CachedNodeName = SelectedPackDetailNode.NodeName;
                            NewContainerNode.Properties[ContainerQtyProp].AsQuantity.Quantity = Convert.ToDouble( ContainerQtyString );
                            NewContainerNode.Properties[ContainerQtyProp].AsQuantity.Units = Units;
                            NewContainerNode.postChanges( true );

                            NewContainerNodePks.Add( NewContainerNode.NodeId );
                        }
                    }
                }

                CswNbtView NewContainersView = new CswNbtView( Master.CswNbtResources );
                NewContainersView.ViewName = "New Containers";

                CswNbtViewRelationship ChemicalsRel = NewContainersView.AddViewRelationship( MaterialNodeType, true );
                CswNbtViewRelationship PackagesRel = NewContainersView.AddViewRelationship( ChemicalsRel, CswNbtViewRelationship.PropOwnerType.Second, PackageMaterialProp, true );
                CswNbtViewRelationship PackDetailRel = NewContainersView.AddViewRelationship( PackagesRel, CswNbtViewRelationship.PropOwnerType.Second, PackDetailPackageProp, true );
                CswNbtViewRelationship ContainerRel = NewContainersView.AddViewRelationship( PackDetailRel, CswNbtViewRelationship.PropOwnerType.Second, ContainerPackDetailProp, true );

                ChemicalsRel.NodeIdsToFilterIn.Add( MaterialNodeKey.NodeId );
                ContainerRel.NodeIdsToFilterIn = NewContainerNodePks;
                PackagesRel.ShowInTree = false;
                PackDetailRel.ShowInTree = false;

                Master.setViewXml( NewContainersView.ToString() );
                Master.Redirect( "Main.aspx" );
            }
            catch( Exception ex )
            {
                // rollback
                Master.CswNbtResources.Rollback();
                _Wizard.CurrentStep = 4;
                Master.HandleError( ex );
            }
        }

        #endregion Events

    } // class Act_CreateInspection
} // namespace ChemSW.Nbt.WebPages

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using FarPoint.Web.Spread;
using ChemSW.CswWebControls;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_ImportFireExtinguisher : System.Web.UI.Page
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

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( ImportSpread, Master.ErrorBox );
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
            _Wizard.ID = "ImportFEDataWizard";
            _Wizard.WizardTitle = "Import Fire Extinguisher Data";
            _Wizard.onCancel += new CswWizard.CswWizardEventHandler( _Wizard_onCancel );
            _Wizard.OnError += new CswErrorHandler( Master.HandleError );
            _Wizard.onPageChange += new CswWizard.CswWizardEventHandler( _Wizard_onPageChange );
            _Wizard.onFinish += new CswWizard.CswWizardEventHandler( _Wizard_onFinish );

            LayoutImportSpreadsheet();

            ph.Controls.Add( _Wizard );
        }

        private FpSpread ImportSpread;
        private SheetView Sheet1;
        private CheckBox AssignNextCheckBox;

        private enum ImportColumns
        {
            Building,
            Floor,
            Room,
            Mount_Point_Description,
            Mount_Point_Group,
            Mount_Point_Barcode,
            Type,
            Last_Inspection_Date,
            Last_Inspection_Status,
            Fire_Extinguisher_Description,
            Fire_Extinguisher_Barcode,
            Fire_Extinguisher_Manufacturer,
            Fire_Extinguisher_Model,
            Fire_Extinguisher_Size,
            Fire_Extinguisher_Size_Unit
        }
        private static string ImportColumnsToDisplayString( ImportColumns Column )
        {
            return Column.ToString().Replace( '_', ' ' );
        }
        private static ImportColumns ImportColumnsFromDisplayString( string Column )
        {
            return (ImportColumns) Enum.Parse( typeof( ImportColumns ), Column.Replace( ' ', '_' ), true );
        }

        private Int32 numColumns = 15;

        private void LayoutImportSpreadsheet()
        {
            CswWizardStep ImportDataStep = new CswWizardStep();
            ImportDataStep.ID = "ImportFEDataWizard_ImportDataStep";
            ImportDataStep.Step = 1;
            ImportDataStep.Title = "Import";
            _Wizard.WizardSteps.Add( ImportDataStep );

            CswAutoTable ImportDataStepTable = new CswAutoTable();
            ImportDataStepTable.ID = "ImportDataStepTable";
            ImportDataStep.Controls.Add( ImportDataStepTable );

            ImportSpread = new FpSpread();
            ImportSpread.ID = "ImportSpread";
            ImportSpread.Height = 320;
            ImportSpread.Width = 600;
            ImportSpread.EditModeReplace = true;
            ImportSpread.ActiveSheetViewIndex = 0;
            ImportSpread.EnableAjaxCall = true;
            ImportSpread.UpdateCommand += new SpreadCommandEventHandler( QuestionSpread_UpdateCommand );
            ImportDataStepTable.addControl( 0, 0, ImportSpread );

            Sheet1 = new SheetView();
            Sheet1.SheetName = "Sheet1";
            Sheet1.AllowInsert = true;
            Sheet1.AllowDelete = true;
            Sheet1.AutoGenerateColumns = false;
            ImportSpread.Sheets.Add( Sheet1 );

            // clear the sheet
            Sheet1.RemoveRows( 0, Sheet1.Rows.Count );
            Sheet1.RemoveColumns( 0, Sheet1.Columns.Count );

            Sheet1.SelectionBackColor = System.Drawing.Color.FromArgb( 255, 255, 0 );
            Sheet1.LockBackColor = System.Drawing.Color.FromArgb( 200, 200, 200 );

            Sheet1.Columns.Add( 0, numColumns );
            Sheet1.Rows.Count = 200;
            Sheet1.PageSize = 500;

            Int32 ColNum = 0;
            _Columns = new Dictionary<ImportColumns, Int32>();

            foreach( ImportColumns Col in Enum.GetValues( typeof( ImportColumns ) ) )
            {
                _Columns.Add( Col, ColNum );
                Sheet1.Columns[ColNum].Label = ImportColumnsToDisplayString( Col );
                if( Col == ImportColumns.Building || Col == ImportColumns.Room || Col == ImportColumns.Mount_Point_Description )
                    Sheet1.Columns[ColNum].Label += "*";
                ColNum++;
            }

            // Last Inspection Status options
            string InspStatuses = string.Empty;
            foreach( CswNbtObjClassInspectionDesign.InspectionStatus InspStatus in Enum.GetValues( typeof( CswNbtObjClassInspectionDesign.InspectionStatus ) ) )
            {
                if( InspStatuses != string.Empty ) InspStatuses += ",";
                InspStatuses += CswNbtObjClassInspectionDesign.InspectionStatusAsString( InspStatus );
            }
            string[] InspStatusesArray = CswTools.SplitAndTrim( InspStatuses, ',' );
            FarPoint.Web.Spread.ComboBoxCellType LastInspStatusCell = new FarPoint.Web.Spread.ComboBoxCellType();
            LastInspStatusCell.Items = InspStatusesArray;
            LastInspStatusCell.Values = InspStatusesArray;
            Sheet1.Columns[_Columns[ImportColumns.Last_Inspection_Status]].CellType = LastInspStatusCell;

            // Type options
            CswNbtMetaDataNodeType FireExtNT = Master.CswNbtResources.MetaData.getNodeType( "Fire Extinguisher" );
            if( FireExtNT != null )
            {
                CswNbtMetaDataNodeTypeProp FETypeNTP = FireExtNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassFireExtinguisher.TypePropertyName );
                string[] TypesArray = CswTools.SplitAndTrim( FETypeNTP.ListOptions, ',' );
                FarPoint.Web.Spread.ComboBoxCellType TypeCell = new FarPoint.Web.Spread.ComboBoxCellType();
                TypeCell.Items = TypesArray;
                TypeCell.Values = TypesArray;
                Sheet1.Columns[_Columns[ImportColumns.Type]].CellType = TypeCell;
            }

            ImportDataStep.Controls.Add( new CswLiteralText( "*required" ) );
        } // LayoutImportSpreadsheet

        private Dictionary<ImportColumns, Int32> _Columns;

        protected override void OnPreRender( EventArgs e )
        {
            if( _Wizard.CurrentStep == 2 )
                _Wizard.NextButton.OnClientClick = "return Spread_Update_Click('" + ImportSpread.ClientID + "_Update');";

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
                CswNbtMetaDataNodeType BuildingNT = Master.CswNbtResources.MetaData.getNodeType( "Building" );
                CswNbtMetaDataNodeType FloorNT = Master.CswNbtResources.MetaData.getNodeType( "Floor" );
                CswNbtMetaDataNodeType RoomNT = Master.CswNbtResources.MetaData.getNodeType( "Room" );
                CswNbtMetaDataNodeType MountPointNT = Master.CswNbtResources.MetaData.getNodeType( "Mount Point" );
                CswNbtMetaDataNodeType MountPointGroupNT = Master.CswNbtResources.MetaData.getNodeType( "Mount Point Group" );

                if( BuildingNT != null &&
                    FloorNT != null &&
                    RoomNT != null &&
                    MountPointNT != null &&
                    MountPointGroupNT != null )
                {
                    CswNbtMetaDataNodeTypeProp FloorLocationNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                    CswNbtMetaDataNodeTypeProp RoomLocationNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                    CswNbtMetaDataNodeTypeProp MountPointLocationNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.LocationPropertyName );
                    CswNbtMetaDataNodeTypeProp MountPointGroupNameNTP = MountPointGroupNT.getNodeTypeProp( "Name" );    // BEWARE


                    Collection<CswPrimaryKey> NodeKeysToInclude = new Collection<CswPrimaryKey>();
                    for( Int32 r = 0; r < Sheet1.Rows.Count; r++ )
                    {
                        string BuildingName = Sheet1.Cells[r, _Columns[ImportColumns.Building]].Text;
                        string FloorName = Sheet1.Cells[r, _Columns[ImportColumns.Floor]].Text;
                        string RoomName = Sheet1.Cells[r, _Columns[ImportColumns.Room]].Text;
                        string MountPointGroup = Sheet1.Cells[r, _Columns[ImportColumns.Mount_Point_Group]].Text;
                        string MountPointBarcode = Sheet1.Cells[r, _Columns[ImportColumns.Mount_Point_Barcode]].Text;
                        string MountPointDescription = Sheet1.Cells[r, _Columns[ImportColumns.Mount_Point_Description]].Text;
                        string Type = Sheet1.Cells[r, _Columns[ImportColumns.Type]].Text;

                        if( BuildingName != string.Empty && RoomName != string.Empty && MountPointDescription != string.Empty )  // ignore blank rows
                        {
                            string LastInspectionStatusString = Sheet1.Cells[r, _Columns[ImportColumns.Last_Inspection_Status]].Text;
                            CswNbtObjClassInspectionDesign.InspectionStatus LastInspectionStatus = CswNbtObjClassInspectionDesign.InspectionStatus.Null;
                            if( LastInspectionStatusString != string.Empty )
                                LastInspectionStatus = CswNbtObjClassInspectionDesign.InspectionStatusFromString( LastInspectionStatusString );

                            string LastInspectionDateString = Sheet1.Cells[r, _Columns[ImportColumns.Last_Inspection_Date]].Text;
                            DateTime LastInspectionDate = DateTime.MinValue;
                            DateTime.TryParse( LastInspectionDateString, out LastInspectionDate );

                            // Locations
                            CswNbtNode BuildingNode = _HandleLocation( BuildingNT, BuildingName, null );

                            CswNbtNode FloorNode = null;
                            if( FloorName != string.Empty )
                                FloorNode = _HandleLocation( FloorNT, FloorName, BuildingNode );

                            CswNbtNode RoomNode = null;
                            if( FloorNode != null )
                                RoomNode = _HandleLocation( RoomNT, RoomName, FloorNode );
                            else
                                RoomNode = _HandleLocation( RoomNT, RoomName, BuildingNode );

                            // Mount Point Group
                            CswNbtNode MountPointGroupNode = null;
                            foreach( CswNbtNode ExistingMountPointGroupNode in MountPointGroupNT.getNodes( true, true ) )  // force update to get new ones as we add them
                            {
                                if( ExistingMountPointGroupNode.NodeName == MountPointGroup )
                                    MountPointGroupNode = ExistingMountPointGroupNode;
                            }
                            if( MountPointGroupNode != null )
                            {
                                MountPointGroupNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( MountPointGroupNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                                MountPointGroupNode.Properties[MountPointGroupNameNTP].AsText.Text = MountPointGroup;
                                MountPointGroupNode.postChanges( false );
                            }

                            // Mount Point
                            CswNbtNode MountPointNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( MountPointNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                            CswNbtObjClassMountPoint MountPointAsMP = CswNbtNodeCaster.AsMountPoint( MountPointNode );
                            if( MountPointBarcode != string.Empty )
                                MountPointAsMP.Barcode.SetBarcodeValueOverride( MountPointBarcode, true );
                            MountPointAsMP.Description.Text = MountPointDescription;
                            MountPointAsMP.LastInspectionDate.DateValue = LastInspectionDate;
                            MountPointAsMP.Location.SelectedNodeId = RoomNode.NodeId;
                            MountPointAsMP.Location.CachedNodeName = RoomNode.NodeName;
                            MountPointAsMP.Type.Value = Type;
                            if( LastInspectionStatus == CswNbtObjClassInspectionDesign.InspectionStatus.Action_Required )
                            {
                                MountPointAsMP.Status.Value = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.OOC );
                            }
                            else if( LastInspectionStatus == CswNbtObjClassInspectionDesign.InspectionStatus.Completed ||
                               LastInspectionStatus == CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late )
                            {
                                MountPointAsMP.Status.Value = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.OK );
                            }
                            else
                            {
                                MountPointAsMP.Status.Value = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected );
                            }
                            MountPointNode.postChanges( false );

                            // Fire Extinguisher
                            // not done yet

                            // Store node keys
                            if( BuildingNode != null && !NodeKeysToInclude.Contains( BuildingNode.NodeId ) )
                                NodeKeysToInclude.Add( BuildingNode.NodeId );
                            if( FloorNode != null && !NodeKeysToInclude.Contains( FloorNode.NodeId ) )
                                NodeKeysToInclude.Add( FloorNode.NodeId );
                            if( RoomNode != null && !NodeKeysToInclude.Contains( RoomNode.NodeId ) )
                                NodeKeysToInclude.Add( RoomNode.NodeId );
                            if( MountPointGroupNode != null && !NodeKeysToInclude.Contains( MountPointGroupNode.NodeId ) )
                                NodeKeysToInclude.Add( MountPointGroupNode.NodeId );
                            if( MountPointNode != null && !NodeKeysToInclude.Contains( MountPointNode.NodeId ) )
                                NodeKeysToInclude.Add( MountPointNode.NodeId );

                        } // if( BuildingName != string.Empty )
                    } // for( Int32 r = 0; r < Sheet1.Rows.Count; r++ )

                    CswNbtView NewNodesView = new CswNbtView( Master.CswNbtResources );
                    NewNodesView.ViewName = "New Locations";
                    CswNbtViewRelationship BuildingRel = NewNodesView.AddViewRelationship( BuildingNT, false );
                    CswNbtViewRelationship FloorRel = NewNodesView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, FloorLocationNTP, false );
                    CswNbtViewRelationship RoomRelFloor = NewNodesView.AddViewRelationship( FloorRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, false );
                    CswNbtViewRelationship RoomRelBuilding = NewNodesView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, false );
                    CswNbtViewRelationship MountPointRel1 = NewNodesView.AddViewRelationship( RoomRelFloor, CswNbtViewRelationship.PropOwnerType.Second, MountPointLocationNTP, false );
                    CswNbtViewRelationship MountPointRel2 = NewNodesView.AddViewRelationship( RoomRelBuilding, CswNbtViewRelationship.PropOwnerType.Second, MountPointLocationNTP, false );

                    BuildingRel.NodeIdsToFilterIn = NodeKeysToInclude;
                    FloorRel.NodeIdsToFilterIn = NodeKeysToInclude;
                    RoomRelFloor.NodeIdsToFilterIn = NodeKeysToInclude;
                    RoomRelBuilding.NodeIdsToFilterIn = NodeKeysToInclude;
                    MountPointRel1.NodeIdsToFilterIn = NodeKeysToInclude;
                    MountPointRel2.NodeIdsToFilterIn = NodeKeysToInclude;

                    NewNodesView.SaveToCache();

                    Master.setSessionViewId( NewNodesView.SessionViewId );
                    Master.Redirect( "Main.aspx" );

                } // if nodetypes exist
            }
            catch( Exception ex )
            {
                // rollback
                Master.CswNbtResources.Rollback();
                //_Wizard.CurrentStep = 2;
                Master.HandleError( ex );
            }
        } // _Wizard_onFinish()


        private CswNbtNode _HandleLocation( CswNbtMetaDataNodeType LocationNT, string LocationName, CswNbtNode ParentNode )
        {
            CswNbtNode ThisNode = null;
            foreach( CswNbtNode ExistingNode in LocationNT.getNodes( true, true ) )   // force update to get new ones as we add them
            {
                if( CswNbtNodeCaster.AsLocation( ExistingNode ).Name.Text == LocationName )
                    ThisNode = ExistingNode;
            }
            if( ThisNode == null )
            {
                ThisNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( LocationNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassLocation ThisNodeAsLocation = CswNbtNodeCaster.AsLocation( ThisNode );
                ThisNodeAsLocation.Name.Text = LocationName;
                if( ParentNode != null )
                {
                    ThisNodeAsLocation.Location.SelectedNodeId = ParentNode.NodeId;
                    ThisNodeAsLocation.Location.CachedNodeName = ParentNode.NodeName;
                }
                ThisNode.postChanges( false );
            }
            return ThisNode;
        }

        #endregion Events

    } // class Act_ImportFireExtinguisher
} // namespace ChemSW.Nbt.WebPages

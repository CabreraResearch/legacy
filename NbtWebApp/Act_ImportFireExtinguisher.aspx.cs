using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Exceptions;
using ChemSW.CswWebControls;
using System.Data.OleDb;
using Telerik.Web.UI;

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
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        private CswWizard _Wizard;
        private CswWizardStep _UploadStep;
        private FileUpload _ExcelUpload;
        private CswWizardStep _VerifyStep;
        private Button _DownloadButton;
        private RadGrid _Grid;
        private HiddenField _TempFileNameField;

        protected override void CreateChildControls()
        {
            _Wizard = new CswWizard();
            _Wizard.ID = "ImportFEDataWizard";
            _Wizard.WizardTitle = "Import Fire Extinguisher Data";
            _Wizard.onCancel += new CswWizard.CswWizardEventHandler( _Wizard_onCancel );
            _Wizard.OnError += new CswErrorHandler( Master.HandleError );
            _Wizard.onPageChange += new CswWizard.CswWizardEventHandler( _Wizard_onPageChange );
            _Wizard.onFinish += new CswWizard.CswWizardEventHandler( _Wizard_onFinish );

            // Page 1
            _UploadStep = new CswWizardStep();
            _UploadStep.ID = "ImportFEDataWizard_UploadStep";
            _UploadStep.Step = 1;
            _UploadStep.Title = "Upload Excel Spreadsheet";
            _UploadStep.ShowFinish = false;
            _Wizard.WizardSteps.Add( _UploadStep );

            _UploadStep.Controls.Add( new CswLiteralText( "Download: " ) );

            _DownloadButton = new Button();
            _DownloadButton.ID = "DownloadButton";
            _DownloadButton.Text = "Download Template";
            _DownloadButton.Click += new EventHandler( _DownloadButton_Click );
            _UploadStep.Controls.Add( _DownloadButton );

            _UploadStep.Controls.Add( new CswLiteralBr() );
            _UploadStep.Controls.Add( new CswLiteralBr() );
            _UploadStep.Controls.Add( new CswLiteralText( "Upload: " ) );

            _ExcelUpload = new FileUpload();
            _ExcelUpload.ID = "ExcelUpload";
            _UploadStep.Controls.Add( _ExcelUpload );

            // Page 2
            _VerifyStep = new CswWizardStep();
            _VerifyStep.ID = "ImportFEDataWizard_VerifyStep";
            _VerifyStep.Step = 2;
            _VerifyStep.Title = "Verify Uploaded Data";
            _Wizard.WizardSteps.Add( _VerifyStep );

            _Grid = new RadGrid();
            _Grid.ID = "VerifyGrid";
            _VerifyStep.Controls.Add( _Grid );

            _TempFileNameField = new HiddenField();
            _TempFileNameField.ID = "TempFileNameField";
            ph.Controls.Add( _TempFileNameField );

            ph.Controls.Add( _Wizard );
        }

        private enum ImportColumns
        {
            Building,
            Floor,
            Room,
            Mount_Point_Description,
            Mount_Point_Group,
            Mount_Point_Barcode,
            Mount_Point_Status,
            Type,
            Last_Inspection_Date,
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

        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );
        }

        #endregion Page Lifecycle

        #region Events

        void _DownloadButton_Click( object sender, EventArgs e )
        {
            try
            {
                CswDelimitedString CSVTemplate = new CswDelimitedString( '\t' );
                foreach( ImportColumns Col in Enum.GetValues( typeof( ImportColumns ) ) )
                {
                    CSVTemplate.Add( ImportColumnsToDisplayString( Col ) );
                }

                Response.ClearContent();
                Response.AddHeader( "content-disposition", "attachment;filename=fe_import.xls" );
                Response.ContentType = "application/vnd.ms-excel";
                Response.Write( CSVTemplate.ToString() );
                Response.End();
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
                if( _Wizard.CurrentStep == 2 )
                {
                    DataTable ExcelData = _getUploadedData();
                    _Grid.DataSource = ExcelData;
                    _Grid.DataBind();

                } //  if( _Wizard.CurrentStep == 2 )
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

        }

        private DataTable _getUploadedData()
        {
            DataTable ret = null;
            OleDbConnection ExcelConn = null;

            string TempFileFullName;
            if( _ExcelUpload.HasFile )
            {
                string TempFileName = "temp/excelupload_" + Master.CswNbtResources.CurrentUser.Username + "_" + DateTime.Now.Ticks.ToString();
                TempFileFullName = Server.MapPath( "" ) + "/" + TempFileName;
                _ExcelUpload.SaveAs( TempFileFullName );

                _TempFileNameField.Value = TempFileFullName;
            }
            else
            {
                TempFileFullName = _TempFileNameField.Value;
            }

            if( TempFileFullName != string.Empty )
            {
                try
                {
                    string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + TempFileFullName + ";Extended Properties=Excel 8.0;";
                    ExcelConn = new OleDbConnection( ConnStr );
                    ExcelConn.Open();

                    DataTable ExcelSchemaDT = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
                    string FirstSheetName = ExcelSchemaDT.Rows[0]["TABLE_NAME"].ToString();

                    OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                    OleDbCommand Command1 = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "]", ExcelConn );
                    DataAdapter.SelectCommand = Command1;

                    DataSet ExcelDS = new DataSet();
                    DataAdapter.Fill( ExcelDS );
                    ret = ExcelDS.Tables[0];
                }
                finally
                {
                    if( ExcelConn != null )
                    {
                        ExcelConn.Close();
                        ExcelConn.Dispose();
                    }
                }
            }
            return ret;
        } // _getUploadedData()


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
                CswNbtMetaDataNodeType FireExtNT = Master.CswNbtResources.MetaData.getNodeType( "Fire Extinguisher" );
                CswNbtMetaDataNodeType VendorNT = Master.CswNbtResources.MetaData.getNodeType( "Vendor" );

                if( BuildingNT != null &&
                    FloorNT != null &&
                    RoomNT != null &&
                    MountPointNT != null &&
                    MountPointGroupNT != null )
                {
                    CswNbtMetaDataNodeTypeProp FloorLocationNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                    CswNbtMetaDataNodeTypeProp RoomLocationNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                    CswNbtMetaDataNodeTypeProp MountPointLocationNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.LocationPropertyName );
                    CswNbtMetaDataNodeTypeProp FEMountPointNTP = FireExtNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassFireExtinguisher.MountPointPropertyName );
                    CswNbtMetaDataNodeTypeProp MountPointGroupNameNTP = MountPointGroupNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPointGroup.NamePropertyName );

                    DataTable ExcelData = _getUploadedData();
                    Collection<CswPrimaryKey> NodeKeysToInclude = new Collection<CswPrimaryKey>();

                    String MpLegacyBarcodeName = "Legacy Barcode";
                    String FeLegacyBarcodeName = "Extinguisher Legacy Barcode";
                    String FeBarcodeName = "Barcode";
                    bool hasLegacyBarcode = false;

                    foreach( DataRow Row in ExcelData.Rows )
                    {
                        string BuildingName = Row[ImportColumnsToDisplayString( ImportColumns.Building )].ToString();
                        string FloorName = Row[ImportColumnsToDisplayString( ImportColumns.Floor )].ToString();
                        string RoomName = Row[ImportColumnsToDisplayString( ImportColumns.Room )].ToString();
                        string MountPointGroup = Row[ImportColumnsToDisplayString( ImportColumns.Mount_Point_Group )].ToString();
                        string MountPointBarcode = Row[ImportColumnsToDisplayString( ImportColumns.Mount_Point_Barcode )].ToString();
                        string MountPointDescription = Row[ImportColumnsToDisplayString( ImportColumns.Mount_Point_Description )].ToString();
                        string Type = Row[ImportColumnsToDisplayString( ImportColumns.Type )].ToString();

                        string FEBarcode = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Barcode )].ToString();
                        string FEDescription = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Description )].ToString();
                        string FEManufacturer = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Manufacturer )].ToString();
                        string FEModel = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Model )].ToString();
                        string FESize = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Size )].ToString();
                        string FESizeUnit = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Size_Unit )].ToString();

                        if( BuildingName == string.Empty || RoomName == string.Empty || MountPointDescription == string.Empty )
                        {
                            continue; // Move to next Row in ExcelData.Rows
                        }
                        // Parse values
                        string MountPointStatusString = Row[ImportColumnsToDisplayString( ImportColumns.Mount_Point_Status )].ToString();
                        CswNbtObjClassInspectionDesign.TargetStatus TargetStatus = CswNbtObjClassInspectionDesign.TargetStatusFromString( MountPointStatusString );
                        if( CswNbtObjClassInspectionDesign.TargetStatus.Null == TargetStatus )
                        {
                            TargetStatus = CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected;
                        }
                        string TargetStatusString = CswNbtObjClassInspectionDesign.TargetStatusAsString( TargetStatus );
                        string LastInspectionDateString = Row[ImportColumnsToDisplayString( ImportColumns.Last_Inspection_Date )].ToString();
                        DateTime LastInspectionDate = DateTime.MinValue;
                        DateTime.TryParse( LastInspectionDateString, out LastInspectionDate );

                        // CswNbtObjClassInspectionDesign.TargetStatus TargetStatus = _GetStatus( LastInspectionDate, LastInspectionStatus );


                        // Manufacturer (Vendor)
                        CswNbtNode FEManufacturerNode = null;
                        if( FEManufacturer != string.Empty && VendorNT != null )
                        {
                            CswNbtMetaDataNodeTypeProp VendorNameNTP = VendorNT.getNodeTypeProp( "Vendor Name" );
                            if( VendorNameNTP != null )
                            {
                                foreach( CswNbtNode ExistingVendorNode in VendorNT.getNodes( true, true ) )  // force update to get new ones as we add them
                                {
                                    if( ExistingVendorNode.NodeName.ToLower().Trim() != FEManufacturer.ToLower().Trim() )
                                    {
                                        continue;
                                    }
                                    FEManufacturerNode = ExistingVendorNode;
                                    break;
                                }
                                if( FEManufacturerNode == null )
                                {
                                    FEManufacturerNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( VendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                                    FEManufacturerNode.Properties[VendorNameNTP].AsText.Text = FEManufacturer;
                                    FEManufacturerNode.postChanges( false );
                                }

                            } // if( VendorNameNTP != null )
                        } // if( FEManufacturer != string.Empty && VendorNT != null )


                        // Locations
                        CswNbtNode BuildingNode = _HandleBuilding( BuildingNT, BuildingName, null, _HandleBuildingLevel.Building );

                        CswNbtNode FloorNode = null;
                        if( FloorName != string.Empty )
                        {
                            FloorNode = _HandleBuilding( FloorNT, FloorName, BuildingNode, _HandleBuildingLevel.Floor );
                        }

                        CswNbtNode RoomNode = null;
                        if( RoomName != null )
                        {
                            if( FloorNode != null )
                            {
                                RoomNode = _HandleBuilding( RoomNT, RoomName, FloorNode, _HandleBuildingLevel.Room );
                            }
                            else
                            {
                                RoomNode = _HandleBuilding( RoomNT, RoomName, BuildingNode, _HandleBuildingLevel.Room );
                            }
                        }
                        // Mount Point Group
                        CswNbtNode MountPointGroupNode = null;
                        if( MountPointGroup != string.Empty )
                        {
                            foreach( CswNbtNode ExistingMountPointGroupNode in MountPointGroupNT.getNodes( true, true ) )  // force update to get new ones as we add them
                            {
                                if( CswNbtNodeCaster.AsMountPointGroup( ExistingMountPointGroupNode ).Name.Text.ToLower().Trim() != MountPointGroup.ToLower().Trim() )
                                {
                                    continue;
                                }
                                MountPointGroupNode = ExistingMountPointGroupNode;
                                break;
                            }
                            if( null == MountPointGroupNode )
                            {
                                MountPointGroupNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( MountPointGroupNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                                MountPointGroupNode.Properties[MountPointGroupNameNTP].AsText.Text = MountPointGroup;
                                MountPointGroupNode.postChanges( false );
                            }

                        } // if( MountPointGroup != string.Empty )
                        else
                        {
                            // will use the default value for mount point group on mount point creation
                        }

                        // Mount Point
                        CswNbtMetaDataNodeTypeProp MPLegacyBarcodeNTP = null;
                        bool mpBarcodeExists = false;

                        if( MountPointBarcode != string.Empty )
                        {
                            CswNbtView ExistingBarcodes = new CswNbtView( Master.CswNbtResources );
                            ExistingBarcodes.ViewName = "Barcode Already Exists";
                            CswNbtViewRelationship MountPointViewRel = ExistingBarcodes.AddViewRelationship( MountPointNT, false );
                            CswNbtMetaDataNodeTypeProp MountPointBarcodeNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.BarcodePropertyName );
                            CswNbtViewProperty BarcodeViewProp = ExistingBarcodes.AddViewProperty( MountPointViewRel, MountPointBarcodeNTP );
                            CswNbtViewPropertyFilter BarcodeViewFilt = ExistingBarcodes.AddViewPropertyFilter( BarcodeViewProp, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, MountPointBarcode, false );
                            ICswNbtTree MpTree = Master.CswNbtResources.Trees.getTreeFromView( ExistingBarcodes, true, true, true, false );

                            MpTree.goToRoot();
                            if( MpTree.getChildNodeCount() > 0 ) // A matching barcode already exists
                            {
                                mpBarcodeExists = true;
                                hasLegacyBarcode = true;
                                MPLegacyBarcodeNTP = MountPointNT.getNodeTypeProp( MpLegacyBarcodeName );
                                if( null == MPLegacyBarcodeNTP )
                                    MPLegacyBarcodeNTP = Master.CswNbtResources.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Text, MpLegacyBarcodeName, Int32.MinValue );

                                //Int32 ExistingBarcode = CswConvert.ToInt32( CswNbtNodeCaster.AsMountPoint( MPNode ).Barcode.Barcode );
                                //if( ExistingBarcode >= MpBarcodeVal )
                                //    MpBarcodeVal = ExistingBarcode + 1;
                            }
                        }

                        CswNbtNode MountPointNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( MountPointNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                        CswNbtObjClassMountPoint MountPointAsMP = CswNbtNodeCaster.AsMountPoint( MountPointNode );

                        if( mpBarcodeExists )
                        {
                            MountPointAsMP.Barcode.setBarcodeValue();
                            MountPointNode.Properties[MPLegacyBarcodeNTP].AsText.Text = MountPointBarcode;
                        }
                        else
                        {
                            MountPointAsMP.Barcode.setBarcodeValueOverride( MountPointBarcode, true );
                        }

                        MountPointAsMP.Description.Text = MountPointDescription;
                        MountPointAsMP.LastInspectionDate.DateValue = LastInspectionDate;
                        MountPointAsMP.Location.SelectedNodeId = RoomNode.NodeId;
                        MountPointAsMP.Location.RefreshNodeName();
                        MountPointAsMP.Type.Value = Type;
                        MountPointAsMP.Status.Value = TargetStatusString;
                        //CswNbtMetaDataNodeTypeProp MountPointGroupNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.MountPointGroupPropertyName );
                        if( null != MountPointGroupNode )
                        {
                            MountPointAsMP.MountPointGroup.RelatedNodeId = MountPointGroupNode.NodeId;
                        }
                        MountPointNode.postChanges( false );


                        // Fire Extinguisher
                        CswNbtNode FENode = null;
                        if( FireExtNT != null &&
                            ( FEDescription != string.Empty ||
                              FEBarcode != string.Empty ||
                              FEManufacturer != string.Empty ||
                              FEModel != string.Empty ||
                              FESize != string.Empty ) )
                        {
                            CswNbtMetaDataNodeTypeProp BarcodeNTP = FireExtNT.BarcodeProperty;
                            CswNbtMetaDataNodeTypeProp FELegacyBarcodeNTP = null;
                            bool feBarcodeExists = false;

                            if( FEBarcode != string.Empty )
                            {
                                if( null == BarcodeNTP )
                                    BarcodeNTP = Master.CswNbtResources.MetaData.makeNewProp( FireExtNT, CswNbtMetaDataFieldType.NbtFieldType.Barcode, FeBarcodeName, Int32.MinValue );

                                CswNbtView ExistingBarcodes = new CswNbtView( Master.CswNbtResources );
                                ExistingBarcodes.ViewName = "Barcode Already Exists";
                                CswNbtViewRelationship FireExtViewRel = ExistingBarcodes.AddViewRelationship( FireExtNT, false );
                                CswNbtViewProperty BarcodeViewProp = ExistingBarcodes.AddViewProperty( FireExtViewRel, BarcodeNTP );
                                CswNbtViewPropertyFilter BarcodeViewFilt = ExistingBarcodes.AddViewPropertyFilter( BarcodeViewProp, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, FEBarcode, false );
                                ICswNbtTree FeTree = Master.CswNbtResources.Trees.getTreeFromView( ExistingBarcodes, true, true, true, false );

                                FeTree.goToRoot();
                                if( FeTree.getChildNodeCount() > 0 ) // A matching barcode already exists
                                {
                                    feBarcodeExists = true;
                                    hasLegacyBarcode = true;
                                    FELegacyBarcodeNTP = FireExtNT.getNodeTypeProp( FeLegacyBarcodeName );
                                    if( null == FELegacyBarcodeNTP )
                                        FELegacyBarcodeNTP = Master.CswNbtResources.MetaData.makeNewProp( FireExtNT, CswNbtMetaDataFieldType.NbtFieldType.Text, FeLegacyBarcodeName, Int32.MinValue );

                                    //Int32 ExistingBarcode = CswConvert.ToInt32( ExistingNode.Properties[BarcodeNTP].AsBarcode.Barcode );
                                    //if( ExistingBarcode >= FeBarcodeVal )
                                    //    FeBarcodeVal = ExistingBarcode + 1;
                                }
                            } 
                                
                            FENode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( FireExtNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                            CswNbtObjClassFireExtinguisher FENodeAsFE = CswNbtNodeCaster.AsFireExtinguisher( FENode );

                            if( feBarcodeExists )
                            {
                                FENode.Properties[FELegacyBarcodeNTP].AsText.Text = FEBarcode;
                                FENode.Properties[BarcodeNTP].AsBarcode.setBarcodeValue();
                            }
                            else
                            {
                                FENode.Properties[BarcodeNTP].AsBarcode.setBarcodeValueOverride( FEBarcode, true );
                            }

                            FENodeAsFE.Description.Text = FEDescription;
                            FENodeAsFE.LastInspectionDate.DateValue = LastInspectionDate;
                            FENodeAsFE.Status.Value = TargetStatusString;
                            FENodeAsFE.MountPoint.RelatedNodeId = MountPointNode.NodeId;
                            FENodeAsFE.Type.Value = Type;

                            CswNbtMetaDataNodeTypeProp ManufacturerNTP = FENode.NodeType.getNodeTypeProp( "Manufacturer" );
                            if( FEManufacturerNode != null && ManufacturerNTP != null )
                            {
                                FENode.Properties[ManufacturerNTP].AsRelationship.RelatedNodeId = FEManufacturerNode.NodeId;
                                FENode.Properties[ManufacturerNTP].AsRelationship.CachedNodeName = FEManufacturerNode.NodeName;
                            }

                            CswNbtMetaDataNodeTypeProp ModelNTP = FENode.NodeType.getNodeTypeProp( "Model" );
                            if( FEModel != string.Empty && ModelNTP != null )
                                FENode.Properties[ModelNTP].AsText.Text = FEModel;

                            CswNbtMetaDataNodeTypeProp SizeNTP = FENode.NodeType.getNodeTypeProp( "Size" );
                            if( FESize != string.Empty && FESizeUnit != string.Empty && SizeNTP != null )
                            {
                                FENode.Properties[SizeNTP].AsQuantity.Quantity = CswConvert.ToDouble( FESize );
                                FENode.Properties[SizeNTP].AsQuantity.Units = FESizeUnit;
                            }
                            FENode.postChanges( false );
                        } // if we have an FE field


                        // Store node keys for view
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
                        if( FEManufacturerNode != null && !NodeKeysToInclude.Contains( FEManufacturerNode.NodeId ) )
                            NodeKeysToInclude.Add( FEManufacturerNode.NodeId );
                        if( FENode != null && !NodeKeysToInclude.Contains( FENode.NodeId ) )
                            NodeKeysToInclude.Add( FENode.NodeId );
                    } // foreach(DataRow Row in ExcelData.Rows)

                    CswNbtView NewNodesView = new CswNbtView( Master.CswNbtResources );
                    if( !hasLegacyBarcode )
                    {
                        NewNodesView.ViewName = "New Locations";
                        CswNbtViewRelationship BuildingRel = NewNodesView.AddViewRelationship( BuildingNT, false );
                        CswNbtViewRelationship FloorRel = NewNodesView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, FloorLocationNTP, false );
                        CswNbtViewRelationship RoomRelFloor = NewNodesView.AddViewRelationship( FloorRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, false );
                        CswNbtViewRelationship RoomRelBuilding = NewNodesView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, false );
                        CswNbtViewRelationship MountPointRel1 = NewNodesView.AddViewRelationship( RoomRelFloor, CswNbtViewRelationship.PropOwnerType.Second, MountPointLocationNTP, false );
                        CswNbtViewRelationship MountPointRel2 = NewNodesView.AddViewRelationship( RoomRelBuilding, CswNbtViewRelationship.PropOwnerType.Second, MountPointLocationNTP, false );
                        CswNbtViewRelationship FERel1 = NewNodesView.AddViewRelationship( MountPointRel1, CswNbtViewRelationship.PropOwnerType.Second, FEMountPointNTP, false );
                        CswNbtViewRelationship FERel2 = NewNodesView.AddViewRelationship( MountPointRel2, CswNbtViewRelationship.PropOwnerType.Second, FEMountPointNTP, false );

                        BuildingRel.NodeIdsToFilterIn = NodeKeysToInclude;
                        FloorRel.NodeIdsToFilterIn = NodeKeysToInclude;
                        RoomRelFloor.NodeIdsToFilterIn = NodeKeysToInclude;
                        RoomRelBuilding.NodeIdsToFilterIn = NodeKeysToInclude;
                        MountPointRel1.NodeIdsToFilterIn = NodeKeysToInclude;
                        MountPointRel2.NodeIdsToFilterIn = NodeKeysToInclude;
                        FERel1.NodeIdsToFilterIn = NodeKeysToInclude;
                        FERel2.NodeIdsToFilterIn = NodeKeysToInclude;
                    }
                    else
                    {
                        NewNodesView.ViewName = "Import Results";
                        NewNodesView.ViewMode = NbtViewRenderingMode.Grid;
                        NewNodesView.Width = 150;
                        CswNbtViewRelationship MountPointRel = NewNodesView.AddViewRelationship( MountPointNT, false );
                        CswNbtMetaDataNodeTypeProp MpLocationNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.LocationPropertyName );
                        CswNbtMetaDataNodeTypeProp MpStatusNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.StatusPropertyName );
                        CswNbtMetaDataNodeTypeProp MpTypeNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.TypePropertyName );
                        CswNbtMetaDataNodeTypeProp MpBarcodeNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.BarcodePropertyName );
                        CswNbtMetaDataNodeTypeProp MpLegacyBarcodeNTP = MountPointNT.getNodeTypeProp( MpLegacyBarcodeName );
                        CswNbtViewProperty MpTypeProp = NewNodesView.AddViewProperty( MountPointRel, MpTypeNTP );
                        CswNbtViewProperty MpStatusProp = NewNodesView.AddViewProperty( MountPointRel, MpStatusNTP );
                        CswNbtViewProperty MpLocationProp = NewNodesView.AddViewProperty( MountPointRel, MpLocationNTP );
                        CswNbtViewProperty MpBarcodeProp = NewNodesView.AddViewProperty( MountPointRel, MpBarcodeNTP );
                        CswNbtViewProperty MpLegacyBarProp = NewNodesView.AddViewProperty( MountPointRel, MpLegacyBarcodeNTP );
                        //CswNbtViewRelationship FireExtRel = NewNodesView.AddViewRelationship( MountPointRel, CswNbtViewRelationship.PropOwnerType.Second, FEMountPointNTP, false );
                        //CswNbtMetaDataNodeTypeProp FeBarcodeNTP = FireExtNT.getNodeTypeProp( FeBarcodeName );
                        //CswNbtMetaDataNodeTypeProp FeLegacyBarNTP = FireExtNT.getNodeTypeProp( FeLegacyBarcodeName );
                        //CswNbtViewProperty FeBarcodeProp = NewNodesView.AddViewProperty( FireExtRel, FeBarcodeNTP );
                        //CswNbtViewProperty FeLegacyBarProp = NewNodesView.AddViewProperty( FireExtRel, FeLegacyBarNTP );

                        MountPointRel.NodeIdsToFilterIn = NodeKeysToInclude;
                        //FireExtRel.NodeIdsToFilterIn = NodeKeysToInclude;
                    }

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

        //private CswNbtObjClassInspectionDesign.TargetStatus _GetStatus( DateTime LastInspectionDate, CswNbtObjClassInspectionDesign.InspectionStatus LastInspectionStatus )
        //{
        //    CswNbtObjClassInspectionDesign.TargetStatus ret = CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected;
        //    if( LastInspectionStatus == CswNbtObjClassInspectionDesign.InspectionStatus.Null ||
        //        LastInspectionDate == DateTime.MinValue )
        //    {
        //        ret = CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected;
        //    }
        //    else if( LastInspectionStatus == CswNbtObjClassInspectionDesign.InspectionStatus.Action_Required )
        //    {
        //        ret = CswNbtObjClassInspectionDesign.TargetStatus.OOC;
        //    }
        //    else if( LastInspectionStatus == CswNbtObjClassInspectionDesign.InspectionStatus.Completed ||
        //             LastInspectionStatus == CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late )
        //    {
        //        ret = CswNbtObjClassInspectionDesign.TargetStatus.OK;
        //    }
        //    else
        //    {
        //        ret = CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected;
        //    }
        //    return ret;
        //} // _GetStatus()
        private enum _HandleBuildingLevel { Building, Floor, Room };
        private CswNbtNode _HandleBuilding( CswNbtMetaDataNodeType LocationNT, string LocationName, 
                                            CswNbtNode ParentNode, _HandleBuildingLevel Level )
        {
            CswNbtNode ThisNode = null;

                foreach( CswNbtNode ExistingNode in LocationNT.getNodes( true, true ) )   // force update to get new ones as we add them
                {
                    if( ( Level == _HandleBuildingLevel.Building &&
                          CswNbtNodeCaster.AsLocation( ExistingNode ).Name.Text.ToLower().Trim() == LocationName.ToLower().Trim() ) ||
                        ( ( Level == _HandleBuildingLevel.Floor || Level == _HandleBuildingLevel.Room ) &&
                          CswNbtNodeCaster.AsLocation( ExistingNode ).Location.SelectedNodeId == ParentNode.NodeId &&
                          CswNbtNodeCaster.AsLocation( ExistingNode ).Name.Text.ToLower().Trim() == LocationName.ToLower().Trim() ) )
                    {
                        ThisNode = ExistingNode;
                        break;
                    }
            }
            if( ThisNode == null )
            {
                ThisNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( LocationNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                CswNbtObjClassLocation ThisNodeAsLocation = CswNbtNodeCaster.AsLocation( ThisNode );
                ThisNodeAsLocation.Name.Text = LocationName;
                if( ParentNode != null )
                {
                    ThisNodeAsLocation.Location.SelectedNodeId = ParentNode.NodeId;
                    ThisNodeAsLocation.Location.RefreshNodeName();
                }
                ThisNode.postChanges( false );
            }
            return ThisNode;
        }

        #endregion Events

    } // class Act_ImportFireExtinguisher
} // namespace ChemSW.Nbt.WebPages

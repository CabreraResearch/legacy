using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.CswWebControls;
using Telerik.Web.UI;
using System.Collections.ObjectModel;
using System.Collections;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_ImportFireExtinguisher : System.Web.UI.Page
    {
        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
				Master.CswNbtResources.AuditContext = "Import Fire Extinguisher Data";
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
            Site,
			Building,
            Floor,
            Room,
            Inspection_Point_Description,
            Inspection_Point_Group,
            Inspection_Point_Barcode,
            Inspection_Point_Status,
            Type,
            Last_Inspection_Date,
			//Fire_Extinguisher_Description,
			//Fire_Extinguisher_Barcode,
			//Fire_Extinguisher_Manufacturer,
			//Fire_Extinguisher_Model,
			//Fire_Extinguisher_Size,
			//Fire_Extinguisher_Size_Unit
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
                Master.GoMain();
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
				CswNbtMetaDataNodeType SiteNT = Master.CswNbtResources.MetaData.getNodeType( "Site" );
				CswNbtMetaDataNodeType BuildingNT = Master.CswNbtResources.MetaData.getNodeType( "Building" );
				CswNbtMetaDataNodeType FloorNT = Master.CswNbtResources.MetaData.getNodeType( "Floor" );
                CswNbtMetaDataNodeType RoomNT = Master.CswNbtResources.MetaData.getNodeType( "Room" );
				CswNbtMetaDataNodeType InspectionPointNT = Master.CswNbtResources.MetaData.getNodeType( "FE Inspection Point" );
				CswNbtMetaDataNodeType InspectionPointGroupNT = Master.CswNbtResources.MetaData.getNodeType( "Inspection Group" );
                CswNbtMetaDataNodeType FireExtNT = Master.CswNbtResources.MetaData.getNodeType( "Fire Extinguisher" );
                CswNbtMetaDataNodeType VendorNT = Master.CswNbtResources.MetaData.getNodeType( "Vendor" );

                if( SiteNT != null && 
					BuildingNT != null &&
                    FloorNT != null &&
                    RoomNT != null &&
					InspectionPointNT != null &&
					InspectionPointGroupNT != null )
                {
					CswNbtMetaDataNodeTypeProp BuildingLocationNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
					CswNbtMetaDataNodeTypeProp FloorLocationNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
					CswNbtMetaDataNodeTypeProp RoomLocationNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
					CswNbtMetaDataNodeTypeProp InspectionPointLocationNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
					CswNbtMetaDataNodeTypeProp InspectionPointGroupNameNTP = InspectionPointGroupNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTargetGroup.NamePropertyName );

					CswNbtMetaDataNodeTypeProp FEInspectionPointNTP = null;
					if( FireExtNT != null )
						FEInspectionPointNTP = FireExtNT.getNodeTypeProp( "FE Inspection Point" );

                    DataTable ExcelData = _getUploadedData();
                    Collection<CswPrimaryKey> NodeKeysToInclude = new Collection<CswPrimaryKey>();

                    String MpLegacyBarcodeName = "Legacy Barcode";
                    //String FeLegacyBarcodeName = "Extinguisher Legacy Barcode";
                    //String FeBarcodeName = "Barcode";
                    bool hasLegacyBarcode = false;

                    foreach( DataRow Row in ExcelData.Rows )
                    {
						string SiteName = Row[ImportColumnsToDisplayString( ImportColumns.Site )].ToString();
						string BuildingName = Row[ImportColumnsToDisplayString( ImportColumns.Building )].ToString();
						string FloorName = Row[ImportColumnsToDisplayString( ImportColumns.Floor )].ToString();
                        string RoomName = Row[ImportColumnsToDisplayString( ImportColumns.Room )].ToString();
						string InspectionPointGroup = Row[ImportColumnsToDisplayString( ImportColumns.Inspection_Point_Group )].ToString();
						string InspectionPointBarcode = Row[ImportColumnsToDisplayString( ImportColumns.Inspection_Point_Barcode )].ToString();
						string InspectionPointDescription = Row[ImportColumnsToDisplayString( ImportColumns.Inspection_Point_Description )].ToString();
                        string Type = Row[ImportColumnsToDisplayString( ImportColumns.Type )].ToString();

						//string FEBarcode = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Barcode )].ToString();
						//string FEDescription = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Description )].ToString();
						//string FEManufacturer = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Manufacturer )].ToString();
						//string FEModel = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Model )].ToString();
						//string FESize = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Size )].ToString();
						//string FESizeUnit = Row[ImportColumnsToDisplayString( ImportColumns.Fire_Extinguisher_Size_Unit )].ToString();

						if( SiteName != string.Empty && BuildingName != string.Empty && RoomName != string.Empty && InspectionPointDescription != string.Empty )
                        {

                            // Parse values
							string InspectionPointStatusString = Row[ImportColumnsToDisplayString( ImportColumns.Inspection_Point_Status )].ToString();
							CswNbtObjClassInspectionDesign.TargetStatus TargetStatus = CswNbtObjClassInspectionDesign.TargetStatusFromString( InspectionPointStatusString );
                            if( CswNbtObjClassInspectionDesign.TargetStatus.Null == TargetStatus )
                            {
                                TargetStatus = CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected;
                            }
                            string TargetStatusString = CswNbtObjClassInspectionDesign.TargetStatusAsString( TargetStatus );
                            string LastInspectionDateString = Row[ImportColumnsToDisplayString( ImportColumns.Last_Inspection_Date )].ToString();
                            DateTime LastInspectionDate = DateTime.MinValue;
                            DateTime.TryParse( LastInspectionDateString, out LastInspectionDate );

                            // CswNbtObjClassInspectionDesign.TargetStatus TargetStatus = _GetStatus( LastInspectionDate, LastInspectionStatus );


							//// Manufacturer (Vendor)
							//CswNbtNode FEManufacturerNode = null;
							//if( FEManufacturer != string.Empty && VendorNT != null )
							//{
							//    CswNbtMetaDataNodeTypeProp VendorNameNTP = VendorNT.getNodeTypeProp( "Vendor Name" );
							//    if( VendorNameNTP != null )
							//    {
							//        foreach( CswNbtNode ExistingVendorNode in VendorNT.getNodes( true, true ) ) // force update to get new ones as we add them
							//        {
							//            if( ExistingVendorNode.NodeName.ToLower().Trim() == FEManufacturer.ToLower().Trim() )
							//            {
							//                FEManufacturerNode = ExistingVendorNode;
							//                break;
							//            }
							//        }
							//        if( FEManufacturerNode == null )
							//        {
							//            FEManufacturerNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( VendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
							//            FEManufacturerNode.Properties[VendorNameNTP].AsText.Text = FEManufacturer;
							//            FEManufacturerNode.postChanges( false );
							//        }

							//    } // if( VendorNameNTP != null )
							//} // if( FEManufacturer != string.Empty && VendorNT != null )


							// Locations
							CswNbtNode SiteNode = _HandleLocation( SiteNT, SiteName, null, _HandleLocationLevel.Site );

							// Locations
							CswNbtNode BuildingNode = null;
							if( BuildingName != string.Empty )
							{
								BuildingNode = _HandleLocation( BuildingNT, BuildingName, SiteNode, _HandleLocationLevel.Building );
							}

                            CswNbtNode FloorNode = null;
                            if( FloorName != string.Empty )
                            {
                                FloorNode = _HandleLocation( FloorNT, FloorName, BuildingNode, _HandleLocationLevel.Floor );
                            }

                            CswNbtNode RoomNode = null;
                            if( RoomName != null )
                            {
                                if( FloorNode != null )
                                {
                                    RoomNode = _HandleLocation( RoomNT, RoomName, FloorNode, _HandleLocationLevel.Room );
                                }
                                else
                                {
                                    RoomNode = _HandleLocation( RoomNT, RoomName, BuildingNode, _HandleLocationLevel.Room );
                                }
                            }
							// Inspection Point Group
							CswNbtNode InspectionPointGroupNode = null;
							if( InspectionPointGroup != string.Empty )
                            {
								foreach( CswNbtNode ExistingInspectionPointGroupNode in InspectionPointGroupNT.getNodes( true, true ) ) // force update to get new ones as we add them
                                {
									if( CswNbtNodeCaster.AsInspectionTargetGroup( ExistingInspectionPointGroupNode ).Name.Text.ToLower().Trim() == InspectionPointGroup.ToLower().Trim() )
                                    {
										InspectionPointGroupNode = ExistingInspectionPointGroupNode;
                                        break;
                                    }
                                }
								if( null == InspectionPointGroupNode )
                                {
									InspectionPointGroupNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( InspectionPointGroupNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
									InspectionPointGroupNode.Properties[InspectionPointGroupNameNTP].AsText.Text = InspectionPointGroup;
									InspectionPointGroupNode.postChanges( false );
                                }

							} // if( InspectionPointGroup != string.Empty )

							// Inspection Point
                            CswNbtMetaDataNodeTypeProp MPLegacyBarcodeNTP = null;
                            bool mpBarcodeExists = false;

							if( InspectionPointBarcode != string.Empty )
                            {
                                CswNbtView ExistingBarcodes = new CswNbtView( Master.CswNbtResources );
                                ExistingBarcodes.ViewName = "Barcode Already Exists";
								CswNbtViewRelationship InspectionPointViewRel = ExistingBarcodes.AddViewRelationship( InspectionPointNT, false );
								CswNbtMetaDataNodeTypeProp InspectionPointBarcodeNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
								CswNbtViewProperty BarcodeViewProp = ExistingBarcodes.AddViewProperty( InspectionPointViewRel, InspectionPointBarcodeNTP );
								CswNbtViewPropertyFilter BarcodeViewFilt = ExistingBarcodes.AddViewPropertyFilter( BarcodeViewProp, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, InspectionPointBarcode, false );
                                ICswNbtTree MpTree = Master.CswNbtResources.Trees.getTreeFromView( ExistingBarcodes, true, true, true, false );

                                MpTree.goToRoot();
                                if( MpTree.getChildNodeCount() > 0 ) // A matching barcode already exists
                                {
                                    mpBarcodeExists = true;
                                    hasLegacyBarcode = true;
									MPLegacyBarcodeNTP = InspectionPointNT.getNodeTypeProp( MpLegacyBarcodeName );
                                    if( null == MPLegacyBarcodeNTP )
										MPLegacyBarcodeNTP = Master.CswNbtResources.MetaData.makeNewProp( InspectionPointNT, CswNbtMetaDataFieldType.NbtFieldType.Text, MpLegacyBarcodeName, Int32.MinValue );

                                    //Int32 ExistingBarcode = CswConvert.ToInt32( CswNbtNodeCaster.AsInspectionTarget( MPNode ).Barcode.Barcode );
                                    //if( ExistingBarcode >= MpBarcodeVal )
                                    //    MpBarcodeVal = ExistingBarcode + 1;
                                }
                            }

							CswNbtNode InspectionPointNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( InspectionPointNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
							CswNbtObjClassInspectionTarget InspectionPointAsMP = CswNbtNodeCaster.AsInspectionTarget( InspectionPointNode );

                            if( mpBarcodeExists )
                            {
								InspectionPointAsMP.Barcode.setBarcodeValue();
								InspectionPointNode.Properties[MPLegacyBarcodeNTP].AsText.Text = InspectionPointBarcode;
                            }
                            else
                            {
								InspectionPointAsMP.Barcode.setBarcodeValueOverride( InspectionPointBarcode, true );
                            }

							InspectionPointAsMP.Description.Text = InspectionPointDescription;
							InspectionPointAsMP.LastInspectionDate.DateValue = LastInspectionDate;
                            InspectionPointAsMP.Location.SelectedNodeId = RoomNode.NodeId;
                            InspectionPointAsMP.Location.RefreshNodeName();
                            InspectionPointAsMP.Type.Value = Type;
                            InspectionPointAsMP.Status.Value = TargetStatusString;
                            //CswNbtMetaDataNodeTypeProp InspectionPointGroupNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionPointGroupPropertyName );
                            if( null != InspectionPointGroupNode )
                            {
                                InspectionPointAsMP.InspectionTargetGroup.RelatedNodeId = InspectionPointGroupNode.NodeId;
                            }
                            InspectionPointNode.postChanges( false );


							//// Fire Extinguisher
							//CswNbtNode FENode = null;
							//if( FireExtNT != null &&
							//    ( FEDescription != string.Empty ||
							//      FEBarcode != string.Empty ||
							//      FEManufacturer != string.Empty ||
							//      FEModel != string.Empty ||
							//      FESize != string.Empty ) )
							//{
							//    CswNbtMetaDataNodeTypeProp BarcodeNTP = FireExtNT.BarcodeProperty;
							//    CswNbtMetaDataNodeTypeProp FELegacyBarcodeNTP = null;
							//    bool feBarcodeExists = false;

							//    if( FEBarcode != string.Empty )
							//    {
							//        if( null == BarcodeNTP )
							//            BarcodeNTP = Master.CswNbtResources.MetaData.makeNewProp( FireExtNT, CswNbtMetaDataFieldType.NbtFieldType.Barcode, FeBarcodeName, Int32.MinValue );

							//        CswNbtView ExistingBarcodes = new CswNbtView( Master.CswNbtResources );
							//        ExistingBarcodes.ViewName = "Barcode Already Exists";
							//        CswNbtViewRelationship FireExtViewRel = ExistingBarcodes.AddViewRelationship( FireExtNT, false );
							//        CswNbtViewProperty BarcodeViewProp = ExistingBarcodes.AddViewProperty( FireExtViewRel, BarcodeNTP );
							//        CswNbtViewPropertyFilter BarcodeViewFilt = ExistingBarcodes.AddViewPropertyFilter( BarcodeViewProp, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, FEBarcode, false );
							//        ICswNbtTree FeTree = Master.CswNbtResources.Trees.getTreeFromView( ExistingBarcodes, true, true, true, false );

							//        FeTree.goToRoot();
							//        if( FeTree.getChildNodeCount() > 0 ) // A matching barcode already exists
							//        {
							//            feBarcodeExists = true;
							//            hasLegacyBarcode = true;
							//            FELegacyBarcodeNTP = FireExtNT.getNodeTypeProp( FeLegacyBarcodeName );
							//            if( null == FELegacyBarcodeNTP )
							//                FELegacyBarcodeNTP = Master.CswNbtResources.MetaData.makeNewProp( FireExtNT, CswNbtMetaDataFieldType.NbtFieldType.Text, FeLegacyBarcodeName, Int32.MinValue );

							//            //Int32 ExistingBarcode = CswConvert.ToInt32( ExistingNode.Properties[BarcodeNTP].AsBarcode.Barcode );
							//            //if( ExistingBarcode >= FeBarcodeVal )
							//            //    FeBarcodeVal = ExistingBarcode + 1;
							//        }
							//    } // if( FireExtNT != null && ( FEDescription != string.Empty || FEBarcode != string.Empty || FEManufacturer != string.Empty || FEModel != string.Empty || FESize != string.Empty ) )

							//    FENode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( FireExtNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
							//    CswNbtObjClassInspectionTarget FENodeAsFE = CswNbtNodeCaster.AsInspectionTarget( FENode );

							//    if( feBarcodeExists )
							//    {
							//        FENode.Properties[FELegacyBarcodeNTP].AsText.Text = FEBarcode;
							//        FENode.Properties[BarcodeNTP].AsBarcode.setBarcodeValue();
							//    }
							//    else
							//    {
							//        FENode.Properties[BarcodeNTP].AsBarcode.setBarcodeValueOverride( FEBarcode, true );
							//    }

							//    FENodeAsFE.Description.Text = FEDescription;
							//    FENodeAsFE.LastInspectionDate.DateValue = LastInspectionDate;
							//    FENodeAsFE.Status.Value = TargetStatusString;
							//    //FENodeAsFE.InspectionTarget.RelatedNodeId = InspectionPointNode.NodeId;
							//    FENodeAsFE.Type.Value = Type;

							//    CswNbtMetaDataNodeTypeProp ManufacturerNTP = FENode.NodeType.getNodeTypeProp( "Manufacturer" );
							//    if( FEManufacturerNode != null && ManufacturerNTP != null )
							//    {
							//        FENode.Properties[ManufacturerNTP].AsRelationship.RelatedNodeId = FEManufacturerNode.NodeId;
							//        FENode.Properties[ManufacturerNTP].AsRelationship.CachedNodeName = FEManufacturerNode.NodeName;
							//    }

							//    CswNbtMetaDataNodeTypeProp ModelNTP = FENode.NodeType.getNodeTypeProp( "Model" );
							//    if( FEModel != string.Empty && ModelNTP != null )
							//        FENode.Properties[ModelNTP].AsText.Text = FEModel;

							//    CswNbtMetaDataNodeTypeProp SizeNTP = FENode.NodeType.getNodeTypeProp( "Size" );
							//    if( FESize != string.Empty && FESizeUnit != string.Empty && SizeNTP != null )
							//    {
							//        FENode.Properties[SizeNTP].AsQuantity.Quantity = CswConvert.ToDouble( FESize );
							//        FENode.Properties[SizeNTP].AsQuantity.Units = FESizeUnit;
							//    }
							//    FENode.postChanges( false );
							//} // if we have an FE field


                            // Store node keys for view
							if( SiteNode != null && !NodeKeysToInclude.Contains( SiteNode.NodeId ) )
								NodeKeysToInclude.Add( SiteNode.NodeId );
							if( BuildingNode != null && !NodeKeysToInclude.Contains( BuildingNode.NodeId ) )
								NodeKeysToInclude.Add( BuildingNode.NodeId );
							if( FloorNode != null && !NodeKeysToInclude.Contains( FloorNode.NodeId ) )
                                NodeKeysToInclude.Add( FloorNode.NodeId );
                            if( RoomNode != null && !NodeKeysToInclude.Contains( RoomNode.NodeId ) )
                                NodeKeysToInclude.Add( RoomNode.NodeId );
                            if( InspectionPointGroupNode != null && !NodeKeysToInclude.Contains( InspectionPointGroupNode.NodeId ) )
                                NodeKeysToInclude.Add( InspectionPointGroupNode.NodeId );
                            if( InspectionPointNode != null && !NodeKeysToInclude.Contains( InspectionPointNode.NodeId ) )
                                NodeKeysToInclude.Add( InspectionPointNode.NodeId );
							//if( FEManufacturerNode != null && !NodeKeysToInclude.Contains( FEManufacturerNode.NodeId ) )
							//    NodeKeysToInclude.Add( FEManufacturerNode.NodeId );
							//if( FENode != null && !NodeKeysToInclude.Contains( FENode.NodeId ) )
							//    NodeKeysToInclude.Add( FENode.NodeId );
                        } // if( SiteName != string.Empty && BuildingName != string.Empty && RoomName != string.Empty && InspectionPointDescription != string.Empty )
                    } //foreach( DataRow Row in ExcelData.Rows )

                    CswNbtView NewNodesView = new CswNbtView( Master.CswNbtResources );
                    if( !hasLegacyBarcode )
                    {
                        NewNodesView.ViewName = "New Locations";
						CswNbtViewRelationship SiteRel = NewNodesView.AddViewRelationship( SiteNT, false );
						CswNbtViewRelationship BuildingRel = NewNodesView.AddViewRelationship( SiteRel, CswNbtViewRelationship.PropOwnerType.Second, BuildingLocationNTP, false );
						CswNbtViewRelationship FloorRel = NewNodesView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, FloorLocationNTP, false );
                        CswNbtViewRelationship RoomRelFloor = NewNodesView.AddViewRelationship( FloorRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, false );
                        CswNbtViewRelationship RoomRelBuilding = NewNodesView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, false );
                        CswNbtViewRelationship InspectionPointRel1 = NewNodesView.AddViewRelationship( RoomRelFloor, CswNbtViewRelationship.PropOwnerType.Second, InspectionPointLocationNTP, false );
                        CswNbtViewRelationship InspectionPointRel2 = NewNodesView.AddViewRelationship( RoomRelBuilding, CswNbtViewRelationship.PropOwnerType.Second, InspectionPointLocationNTP, false );

						SiteRel.NodeIdsToFilterIn = NodeKeysToInclude;
						BuildingRel.NodeIdsToFilterIn = NodeKeysToInclude;
						FloorRel.NodeIdsToFilterIn = NodeKeysToInclude;
						RoomRelFloor.NodeIdsToFilterIn = NodeKeysToInclude;
						RoomRelBuilding.NodeIdsToFilterIn = NodeKeysToInclude;
						InspectionPointRel1.NodeIdsToFilterIn = NodeKeysToInclude;
						InspectionPointRel2.NodeIdsToFilterIn = NodeKeysToInclude;

						if( FEInspectionPointNTP != null )
						{
							CswNbtViewRelationship FERel1 = NewNodesView.AddViewRelationship( InspectionPointRel1, CswNbtViewRelationship.PropOwnerType.Second, FEInspectionPointNTP, false );
							CswNbtViewRelationship FERel2 = NewNodesView.AddViewRelationship( InspectionPointRel2, CswNbtViewRelationship.PropOwnerType.Second, FEInspectionPointNTP, false );
							FERel1.NodeIdsToFilterIn = NodeKeysToInclude;
							FERel2.NodeIdsToFilterIn = NodeKeysToInclude;
						}
                    } // if( !hasLegacyBarcode )
                    else
                    {
                        NewNodesView.ViewName = "Import Results";
                        NewNodesView.ViewMode = NbtViewRenderingMode.Grid;
                        NewNodesView.Width = 150;
                        CswNbtViewRelationship InspectionPointRel = NewNodesView.AddViewRelationship( InspectionPointNT, false );
                        CswNbtMetaDataNodeTypeProp MpLocationNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
                        CswNbtMetaDataNodeTypeProp MpStatusNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );
                        CswNbtMetaDataNodeTypeProp MpTypeNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.TypePropertyName );
                        CswNbtMetaDataNodeTypeProp MpBarcodeNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
                        CswNbtMetaDataNodeTypeProp MpLegacyBarcodeNTP = InspectionPointNT.getNodeTypeProp( MpLegacyBarcodeName );
                        CswNbtViewProperty MpTypeProp = NewNodesView.AddViewProperty( InspectionPointRel, MpTypeNTP );
                        CswNbtViewProperty MpStatusProp = NewNodesView.AddViewProperty( InspectionPointRel, MpStatusNTP );
                        CswNbtViewProperty MpLocationProp = NewNodesView.AddViewProperty( InspectionPointRel, MpLocationNTP );
                        CswNbtViewProperty MpBarcodeProp = NewNodesView.AddViewProperty( InspectionPointRel, MpBarcodeNTP );
                        CswNbtViewProperty MpLegacyBarProp = NewNodesView.AddViewProperty( InspectionPointRel, MpLegacyBarcodeNTP );
                        //CswNbtViewRelationship FireExtRel = NewNodesView.AddViewRelationship( InspectionPointRel, CswNbtViewRelationship.PropOwnerType.Second, FEInspectionPointNTP, false );
                        //CswNbtMetaDataNodeTypeProp FeBarcodeNTP = FireExtNT.getNodeTypeProp( FeBarcodeName );
                        //CswNbtMetaDataNodeTypeProp FeLegacyBarNTP = FireExtNT.getNodeTypeProp( FeLegacyBarcodeName );
                        //CswNbtViewProperty FeBarcodeProp = NewNodesView.AddViewProperty( FireExtRel, FeBarcodeNTP );
                        //CswNbtViewProperty FeLegacyBarProp = NewNodesView.AddViewProperty( FireExtRel, FeLegacyBarNTP );

                        InspectionPointRel.NodeIdsToFilterIn = NodeKeysToInclude;
                        //FireExtRel.NodeIdsToFilterIn = NodeKeysToInclude;
                    }

                    NewNodesView.SaveToCache(true);
					//Master.setSessionViewId( NewNodesView.SessionViewId );
					//Master.GoMain();
					Master.Redirect( "Main.html?viewid=" + NewNodesView.SessionViewId );

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
        private enum _HandleLocationLevel { Site, Building, Floor, Room };
        private CswNbtNode _HandleLocation( CswNbtMetaDataNodeType LocationNT, string LocationName, CswNbtNode ParentNode, _HandleLocationLevel Level )
        {
            CswNbtNode ThisNode = null;

            foreach( CswNbtNode ExistingNode in LocationNT.getNodes( true, true ) )   // force update to get new ones as we add them
            {
                if( ( Level == _HandleLocationLevel.Site ||
                      ( ( Level == _HandleLocationLevel.Building || Level == _HandleLocationLevel.Floor || Level == _HandleLocationLevel.Room ) &&
                        CswNbtNodeCaster.AsLocation( ExistingNode ).Location.SelectedNodeId == ParentNode.NodeId ) ) &&
                    CswNbtNodeCaster.AsLocation( ExistingNode ).Name.Text.ToLower().Trim() == LocationName.ToLower().Trim() )
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
				Master.CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.setSequenceValues( ThisNode );
				ThisNode.postChanges( false );
			}
            return ThisNode;
        }

        #endregion Events

    } // class Act_ImportFireExtinguisher
} // namespace ChemSW.Nbt.WebPages

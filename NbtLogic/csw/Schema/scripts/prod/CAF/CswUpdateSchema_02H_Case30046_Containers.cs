using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30046_Containers: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30046; }
        }

        public override string ScriptName
        {
            get { return "02H_Case30046_Containers"; }
        }

        public override string Title
        {
            get { return "CAF Import - Containers"; }
        }

        public override void update()
        {
            _createImportBindings();

        } // update()


        #region import script

        private void _createImportBindings()
        {
            CswNbtSchemaUpdateImportMgr ReceiptLotBindings = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "receipt_lots", "Receipt Lot", "receipt_lots_view", "receiptlotid" );

            ReceiptLotBindings.importBinding( "ReceiptLotNo", CswNbtObjClassReceiptLot.PropertyName.ReceiptLotNo, "" );
            ReceiptLotBindings.importBinding( "CreatedDate", CswNbtObjClassReceiptLot.PropertyName.ManufacturedDate, "" );
            ReceiptLotBindings.importBinding( "ReceiptLotId", CswNbtObjClassReceiptLot.PropertyName.LegacyId, "" );
            ReceiptLotBindings.importBinding( "PackageId", CswNbtObjClassReceiptLot.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );

            ReceiptLotBindings.finalize();



            CswNbtSchemaUpdateImportMgr CofABindings = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "receipt_lots", "C of A Document", "cofa_docs_view", "receiptlotid");

            CofABindings.importBinding( "ReceiptLotId", CswNbtObjClassCofADocument.PropertyName.LegacyId, "" );
            CofABindings.importBinding( "ReceiptLotId", "Receipt Lot", CswEnumNbtSubFieldName.NodeID.ToString() );
            CofABindings.importBinding( "CA_FileName", CswNbtObjClassCofADocument.PropertyName.Title, "" );
            CofABindings.importBinding( "CA_AcquisitionDate", CswNbtObjClassCofADocument.PropertyName.AcquiredDate, "" );
            CofABindings.importBinding( "CA_FileExtension", CswNbtObjClassCofADocument.PropertyName.FileType, "" );
            CofABindings.importBinding( "CA_Content_Type", CswNbtObjClassCofADocument.PropertyName.File, CswEnumNbtSubFieldName.ContentType.ToString() );
            CofABindings.importBinding( "CA_Document", CswNbtObjClassCofADocument.PropertyName.File, CswEnumNbtSubFieldName.Blob.ToString(), BlobTableName : "receipt_lots", LobDataPkColOverride : "receiptlotid" );

            CofABindings.finalize();




            CswNbtSchemaUpdateImportMgr ContainerGroupBindings = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "container_groups", "Container Group", SourceColumn: "ContainerGroupId" );
            
            ContainerGroupBindings.importBinding( "ContainerGroupId", CswNbtObjClassContainerGroup.PropertyName.LegacyId, "" );
            ContainerGroupBindings.importBinding( "ContainerGroupCode", CswNbtObjClassContainerGroup.PropertyName.Name, "" );

            ContainerGroupBindings.finalize();
            


            CswNbtSchemaUpdateImportMgr ContainerBindings = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "Containers", "Container", "containers_view", "containerid" );

            ContainerBindings.importBinding( "ContainerId", CswNbtObjClassContainer.PropertyName.LegacyId, "" );
            ContainerBindings.importBinding( "BarcodeId", CswNbtObjClassContainer.PropertyName.Barcode, "" );
            ContainerBindings.importBinding( "PackDetailId", CswNbtObjClassContainer.PropertyName.Size, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "ParentId", CswNbtObjClassContainer.PropertyName.SourceContainer, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "ContainerGroupId", CswNbtObjClassContainer.PropertyName.ContainerGroup, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "OwnerId", CswNbtObjClassContainer.PropertyName.Owner, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "ContainerStatus", CswNbtObjClassContainer.PropertyName.Status, "" );
            ContainerBindings.importBinding( "ReceiptLotId", CswNbtObjClassContainer.PropertyName.ReceiptLot, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "PackageId", CswNbtObjClassContainer.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "NetQuantity", CswNbtObjClassContainer.PropertyName.Quantity, CswEnumNbtSubFieldName.Value.ToString() );
            ContainerBindings.importBinding( "UnitOfMeasureId", CswNbtObjClassContainer.PropertyName.Quantity, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "ExpirationDate", CswNbtObjClassContainer.PropertyName.ExpirationDate, "" );
            ContainerBindings.importBinding( "LocationId", CswNbtObjClassContainer.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "StorPress", CswNbtObjClassContainer.PropertyName.StoragePressure, "" );
            ContainerBindings.importBinding( "StorTemp", CswNbtObjClassContainer.PropertyName.StorageTemperature, "" );
            ContainerBindings.importBinding( "UseType", CswNbtObjClassContainer.PropertyName.UseType, "" );
            ContainerBindings.importBinding( "ReceivedDate", CswNbtObjClassContainer.PropertyName.DateCreated, "" );
            ContainerBindings.importBinding( "OpenedDate", CswNbtObjClassContainer.PropertyName.OpenedDate, "" );
            ContainerBindings.importBinding( "Concentration", CswNbtObjClassContainer.PropertyName.Concentration, "" );
            ContainerBindings.importBinding( "HomeLocation", CswNbtObjClassContainer.PropertyName.HomeLocation, CswEnumNbtSubFieldName.NodeID.ToString() );
            ContainerBindings.importBinding( "Notes", CswNbtObjClassContainer.PropertyName.Notes, "" );
            ContainerBindings.importBinding( "ProjectId", CswNbtObjClassContainer.PropertyName.Project, "" );
            ContainerBindings.importBinding( "SpecificActivity", CswNbtObjClassContainer.PropertyName.SpecificActivity, "" );
            ContainerBindings.importBinding( "TareQuantity", CswNbtObjClassContainer.PropertyName.TareQuantity, CswEnumNbtSubFieldName.Value.ToString() );
            ContainerBindings.importBinding( "UnitOfMeasureId", CswNbtObjClassContainer.PropertyName.TareQuantity, CswEnumNbtSubFieldName.NodeID.ToString() );

            ContainerBindings.finalize();
        }

        #endregion
    }

}//namespace ChemSW.Nbt.Schema
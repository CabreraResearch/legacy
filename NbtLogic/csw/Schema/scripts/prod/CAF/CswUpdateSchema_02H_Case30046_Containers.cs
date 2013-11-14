using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30046_Containers : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30046; }
        }

        public override string AppendToScriptName()
        {
            return "Containers";
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
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.CAFimportOrder( "Receipt Lot", "receipt_lots", "receipt_lots_view", "receiptlotid" );

            ImpMgr.importBinding( "ReceiptLotNo", CswNbtObjClassReceiptLot.PropertyName.ReceiptLotNo, "" );
            ImpMgr.importBinding( "CreatedDate", CswNbtObjClassReceiptLot.PropertyName.ManufacturedDate, "" );
            ImpMgr.importBinding( "PackageId", CswNbtObjClassReceiptLot.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );



            ImpMgr.CAFimportOrder( "C of A Document", "receipt_lots", "cofa_docs_view", "receiptlotid" );

            ImpMgr.importBinding( "ReceiptLotId", "Receipt Lot", CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "CA_FileName", CswNbtObjClassCofADocument.PropertyName.Title, "" );
            ImpMgr.importBinding( "CA_AcquisitionDate", CswNbtObjClassCofADocument.PropertyName.AcquiredDate, "" );
            ImpMgr.importBinding( "CA_FileExtension", CswNbtObjClassCofADocument.PropertyName.FileType, "" );
            ImpMgr.importBinding( "CA_Content_Type", CswNbtObjClassCofADocument.PropertyName.File, CswEnumNbtSubFieldName.ContentType.ToString() );
            ImpMgr.importBinding( "CA_Document", CswNbtObjClassCofADocument.PropertyName.File, CswEnumNbtSubFieldName.Blob.ToString(), BlobTableName: "receipt_lots", LobDataPkColOverride: "receiptlotid" );



            ImpMgr.CAFimportOrder( "Container Group", "container_groups", PkColumnName: "ContainerGroupId" );

            ImpMgr.importBinding( "ContainerGroupCode", CswNbtObjClassContainerGroup.PropertyName.Name, "" );



            ImpMgr.CAFimportOrder( "Container", "containers", "containers_view", "containerid" );

            ImpMgr.importBinding( "BarcodeId", CswNbtObjClassContainer.PropertyName.Barcode, "" );
            ImpMgr.importBinding( "PackDetailId", CswNbtObjClassContainer.PropertyName.Size, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "ParentId", CswNbtObjClassContainer.PropertyName.SourceContainer, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "ContainerGroupId", CswNbtObjClassContainer.PropertyName.ContainerGroup, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "OwnerId", CswNbtObjClassContainer.PropertyName.Owner, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "ContainerStatus", CswNbtObjClassContainer.PropertyName.Status, "" );
            ImpMgr.importBinding( "ReceiptLotId", CswNbtObjClassContainer.PropertyName.ReceiptLot, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "PackageId", CswNbtObjClassContainer.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "NetQuantity", CswNbtObjClassContainer.PropertyName.Quantity, CswEnumNbtSubFieldName.Value.ToString() );
            ImpMgr.importBinding( "UnitOfMeasureId", CswNbtObjClassContainer.PropertyName.Quantity, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "ExpirationDate", CswNbtObjClassContainer.PropertyName.ExpirationDate, "" );
            ImpMgr.importBinding( "LocationId", CswNbtObjClassContainer.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "StorPress", CswNbtObjClassContainer.PropertyName.StoragePressure, "" );
            ImpMgr.importBinding( "StorTemp", CswNbtObjClassContainer.PropertyName.StorageTemperature, "" );
            ImpMgr.importBinding( "UseType", CswNbtObjClassContainer.PropertyName.UseType, "" );
            ImpMgr.importBinding( "ReceivedDate", CswNbtObjClassContainer.PropertyName.DateCreated, "" );
            ImpMgr.importBinding( "OpenedDate", CswNbtObjClassContainer.PropertyName.OpenedDate, "" );
            ImpMgr.importBinding( "Concentration", CswNbtObjClassContainer.PropertyName.Concentration, "" );
            ImpMgr.importBinding( "HomeLocation", CswNbtObjClassContainer.PropertyName.HomeLocation, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "Notes", CswNbtObjClassContainer.PropertyName.Notes, "" );
            ImpMgr.importBinding( "ProjectId", CswNbtObjClassContainer.PropertyName.Project, "" );
            ImpMgr.importBinding( "SpecificActivity", CswNbtObjClassContainer.PropertyName.SpecificActivity, "" );
            ImpMgr.importBinding( "TareQuantity", CswNbtObjClassContainer.PropertyName.TareQuantity, CswEnumNbtSubFieldName.Value.ToString() );
            ImpMgr.importBinding( "UnitOfMeasureId", CswNbtObjClassContainer.PropertyName.TareQuantity, CswEnumNbtSubFieldName.NodeID.ToString() );

            ImpMgr.finalize();
        }

        #endregion
    }

}//namespace ChemSW.Nbt.Schema
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31264: CswUpdateNbtMasterSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31264; }
        }

        public override string Title
        {
            get { return "CAF Import - Update Size UPC"; }
        }

        public override void doUpdate()
        {
            CswNbtSchemaUpdateImportMgr sizeImporter = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
            sizeImporter.removeImportBinding( "CAF", "upc", "Size", "UPC Barcode", CswEnumNbtSubFieldName.Barcode.ToString() );
            sizeImporter.importBinding( "upc", CswNbtObjClassSize.PropertyName.UPC, "", DestNodeTypeName: "Size" );
            sizeImporter.finalize();
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
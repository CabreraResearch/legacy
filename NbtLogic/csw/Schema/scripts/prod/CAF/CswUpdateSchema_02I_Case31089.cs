using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31089: CswUpdateSchemaTo
    {
        public override string Title { get { return "Add Manufacturer Lot No to Receipt Lot import bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31089; }
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.importBinding( "manufacturerlotno", CswNbtObjClassReceiptLot.PropertyName.ManufacturerLotNo, "", DestNodeTypeName : "Receipt Lot" );

            ImpMgr.finalize();

        }

    }
}
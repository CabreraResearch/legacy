using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31234C: CswUpdateSchemaTo
    {
        public override string Title { get { return "Add missing Chemical bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31234; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.importBinding( "flash_point", CswNbtObjClassChemical.PropertyName.FlashPoint, "", DestNodeTypeName : "Chemical" );
            ImpMgr.importBinding( "materialid", CswNbtObjClassChemical.PropertyName.LegacyMaterialId, "", DestNodeTypeName : "Chemical" );
            ImpMgr.importBinding( "materialid", CswNbtObjClassChemical.PropertyName.ProductDescription, "", DestNodeTypeName : "Chemical" );

            ImpMgr.finalize();

        }

    }
}
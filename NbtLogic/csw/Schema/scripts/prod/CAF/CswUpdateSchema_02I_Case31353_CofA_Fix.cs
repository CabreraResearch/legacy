using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31353: CswUpdateSchemaTo
    {
        public override string Title { get { return "Fix FileExtension column on CofA Imports"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31353; }
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.removeImportBinding( "CAF", "CA_FileExtension", "C of A Document", "File Type", "Value" );
            ImpMgr.importBinding( "FileExtension", CswNbtObjClassCofADocument.PropertyName.FileType, "", "CAF", "C of A Document" );

            ImpMgr.finalize();

        }

    }
}
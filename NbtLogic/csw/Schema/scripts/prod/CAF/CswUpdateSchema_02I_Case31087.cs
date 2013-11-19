using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31087: CswUpdateSchemaTo
    {
        public override string Title { get { return "Add Country to Vendor import bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31087; }
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.importBinding( "country", CswNbtObjClassVendor.PropertyName.Country, "", DestNodeTypeName: "Vendor");

            ImpMgr.finalize();

        }

    }
}
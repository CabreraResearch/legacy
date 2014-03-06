using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ImportExport;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case31893 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31893; }
        }

        public override string Title
        {
            get { return "Update CAF Import Order"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtImportDefOrder.updateOrderEntries( _CswNbtSchemaModTrnsctn );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
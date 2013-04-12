using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_CaseXXXXX : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
        } // update()

    }//class CswUpdateSchema_02B_CaseXXXXX

}//namespace ChemSW.Nbt.Schema
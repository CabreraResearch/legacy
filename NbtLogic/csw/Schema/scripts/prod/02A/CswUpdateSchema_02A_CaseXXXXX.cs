using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02A_CaseXXXXX : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
        } // update()

    }//class CswUpdateSchema_02A_CaseXXXXX

}//namespace ChemSW.Nbt.Schema
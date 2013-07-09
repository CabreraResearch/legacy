using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30123
    /// </summary>
    public class CswUpdateSchema_02D_Case30123: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30123; }
        }

        public override void update()
        {
            

        } // update()

    }//class CswUpdateSchema_02C_Case30123

}//namespace ChemSW.Nbt.Schema
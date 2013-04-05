using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29127
    /// </summary>
    public class CswUpdateSchema_02B_Case29127 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29127; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.addStringColumn( "sessionlist", "lastaccessid", "Last AccessId that the Session was associated with. Used when switching schemata on NBTManager.", false, false, 50 );

        } // update()

    }//class CswUpdateSchema_02B_Case29127

}//namespace ChemSW.Nbt.Schema
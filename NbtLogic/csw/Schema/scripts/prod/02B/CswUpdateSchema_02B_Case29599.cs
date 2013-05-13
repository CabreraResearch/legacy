using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29599
    /// </summary>
    public class CswUpdateSchema_02B_Case29599 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29599; }
        }

        public override void update()
        {
            // Remove the c3_accessid configuration variable from the database
            _CswNbtSchemaModTrnsctn.deleteConfigurationVariable( "C3_AccessId" );
        } // update()

    }//class CswUpdateSchema_02B_Case29599

}//namespace ChemSW.Nbt.Schema
using ChemSW.Config;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29114
    /// </summary>
    public class CswUpdateSchema_01Y_Case29114 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29114; }
        }

        public override void update()
        {
            // Remove the c3urlstem configuration variable from the database
            _CswNbtSchemaModTrnsctn.deleteConfigurationVariable("C3_UrlStem");

        } //Update()

    }//class CswUpdateSchema_01Y_Case29114

}//namespace ChemSW.Nbt.Schema
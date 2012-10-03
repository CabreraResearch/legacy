
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27859
    /// </summary>
    public class CswUpdateSchema_01S_MobileModule_Case27859 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.Modules.EnableModule( CswNbtModuleName.Mobile );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 27859; }
        }

        //Update()

    }//class 

}//namespace ChemSW.Nbt.Schema
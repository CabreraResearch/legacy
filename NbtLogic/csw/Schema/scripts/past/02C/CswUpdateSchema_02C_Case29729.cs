using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29729
    /// </summary>
    public class CswUpdateSchema_02C_Case29729: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29729; }
        }

        public override void update()
        {
            //PCID Sync module is a child of CISPro
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.CISPro, CswEnumNbtModuleName.PCIDSync );

        } // update()

    }//class CswUpdateSchema_02B_Case29729

}//namespace ChemSW.Nbt.Schema
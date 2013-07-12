using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29931
    /// </summary>
    public class CswUpdateSchema_02C_Case29931: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29931; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswEnumNbtModuleName.CISPro, CswEnumNbtActionName.Manage_Locations );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswEnumNbtModuleName.Containers, CswEnumNbtActionName.Manage_Locations );

        } // update()

    }//class CswUpdateSchema_02C_Case29931

}//namespace ChemSW.Nbt.Schema
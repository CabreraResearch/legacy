using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24489 part 2
    /// </summary>
    public class CswUpdateSchema_01U_Case24489_Part2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Reconciliation, true, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Reconciliation );
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 24489; }
        }

    }//class CswUpdateSchema_01U_Case24489_Part2

}//namespace ChemSW.Nbt.Schema
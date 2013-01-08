using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28393
    /// </summary>
    public class CswUpdateSchema_01W_Case28393B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28393; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.HMISReporting, true, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.HMISReporting );
        }//Update()

    }//class CswUpdateSchemaCase_01W_28393B

}//namespace ChemSW.Nbt.Schema
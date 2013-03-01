using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28345
    /// </summary>
    public class CswUpdateSchema_01W_Case28345 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28345; }
        }

        public override void update()
        {

            //Add CISPro: Kiosk Mode action
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.KioskMode, true, "", "Containers" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.KioskMode );

        } //Update()

    }//class CswUpdateSchema_01V_Case28345

}//namespace ChemSW.Nbt.Schema
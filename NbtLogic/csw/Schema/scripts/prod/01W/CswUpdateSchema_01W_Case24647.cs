using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24647
    /// </summary>
    public class CswUpdateSchema_01W_Case24647 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 24647; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Upload_Legacy_Mobile_Data, true, "", "Containers" );

        } //Update()

    }//class CswUpdateSchema_01V_Case24647

}//namespace ChemSW.Nbt.Schema
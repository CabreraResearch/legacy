using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28295
    /// </summary>
    public class CswUpdateSchema_01V_Case28295 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28295; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createModule( "ChemCatCentral Module", CswNbtModuleName.C3.ToString(), false );

        } //Update()

    }//class CswUpdateSchema_01V_Case28295

}//namespace ChemSW.Nbt.Schema
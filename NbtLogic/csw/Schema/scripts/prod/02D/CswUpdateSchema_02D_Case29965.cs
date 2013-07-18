using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29965
    /// </summary>
    public class CswUpdateSchema_02D_Case29965 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29965; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.deleteView( "29684 - Bad Schedules", true );
            _CswNbtSchemaModTrnsctn.deleteView( "Case 20295", true );
        } // update()

    }//class CswUpdateSchema_02C_Case29965

}//namespace ChemSW.Nbt.Schema
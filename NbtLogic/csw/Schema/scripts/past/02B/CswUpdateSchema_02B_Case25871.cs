using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for Case 25871
    /// </summary>
    public class CswUpdateSchema_02B_Case25871 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 25871; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.deleteConfigurationVariable( "NotifyOnSystemFailure" );
        } // update()

    }//class CswUpdateSchema_02B_Case25871

}//namespace ChemSW.Nbt.Schema
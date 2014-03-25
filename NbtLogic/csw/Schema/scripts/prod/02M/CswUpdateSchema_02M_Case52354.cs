using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52354: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 52354; }
        }

        public override string Title
        {
            get { return "Add Block Prior Passwords Config Var"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.ConfigVbls.addNewConfigurationValue( CswEnumNbtConfigurationVariables.password_reuse_count.ToString(), "5", "Number of previous passwords to disallow.", false );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
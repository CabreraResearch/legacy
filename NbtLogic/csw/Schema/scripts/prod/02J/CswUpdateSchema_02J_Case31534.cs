using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case31534: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 31534; }
        }

        public override string Title
        {
            get { return "Change max containers received config var to 250"; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.ConfigVbls.setConfigVariableValue( CswEnumNbtConfigurationVariables.container_receipt_limit.ToString(), "250" );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema
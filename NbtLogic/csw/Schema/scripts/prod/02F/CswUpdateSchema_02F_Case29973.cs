using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case29973 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29973; }
        }

        public override string ScriptName
        {
            get { return "02F_Case29973"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.password_complexity.ToString(), "1" );
            _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.password_length.ToString(), "8" );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
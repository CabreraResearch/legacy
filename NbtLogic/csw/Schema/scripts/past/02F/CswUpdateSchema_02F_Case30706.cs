using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30706 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30706; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30706"; }
        }

        public override void update()
        {
            // Set default value for loc_max_depth to 6
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                _CswNbtSchemaModTrnsctn.ConfigVbls.setConfigVariableValue( CswEnumNbtConfigurationVariables.loc_max_depth.ToString(), "6" );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
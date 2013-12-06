using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31312 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31312; }
        }

        public override string Title
        {
            get { return "Added ChemWatch configuration variables."; }
        }

        public override void update()
        {
            // Add two configuration variables for ChemWatch
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswEnumNbtConfigurationVariables.chemwatchusername, "Username used to connect to ChemWatch.", "", false );
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswEnumNbtConfigurationVariables.chemwatchpassword, "Password used to connect to ChemWatch.", "", false );

        } // update()
    }
}//namespace ChemSW.Nbt.Schema
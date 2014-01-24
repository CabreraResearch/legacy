using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31753 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31753; }
        }

        public override string Title
        {
            get { return "Add 'arielmodules' configuration variable"; }
        }

        public override void update()
        {
            // Create Ariel Modules system configuration variable
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswEnumNbtConfigurationVariables.arielmodules, "List of available regions. Possible values: NA,EU,AP,LA,MA,EE", "NA,EU", true );

        } // update()
    } // class CswUpdateSchema_02K_Case31753
} // namespace
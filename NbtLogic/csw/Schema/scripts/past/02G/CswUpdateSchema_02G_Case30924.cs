using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30924 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {                
            get { return 30924; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Configuration Variable to Set Old Inspections to 'Missed'"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswEnumNbtConfigurationVariables.miss_outdated_inspections, "If set to 1, Inspections will become 'Missed' when an Inspection Schedule generates a new one", "1", false);
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
using ChemSW.Config;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52882 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 52882; }
        }

        public override string Title
        {
            get { return "Add new configuration variable: ChemWatchDomain"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.ConfigVbls.addNewConfigurationValue( CswEnumConfigurationVariableNames.ChemWatchDomain, "", "Domain used to connect to ChemWatch.", false );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
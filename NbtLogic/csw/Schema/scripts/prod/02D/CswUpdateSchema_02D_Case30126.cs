using ChemSW.Config;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30126
    /// </summary>
    public class CswUpdateSchema_02D_Case30126 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30126; }
        }

        public override void update()
        {
            // Create the C3SyncDate configuration variable
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( Name: CswEnumConfigurationVariableNames.C3SyncDate,
                                                                 Description: "The last date that valid Materials were synced with one or all of: FireDb, PCID, LOLI.",
                                                                 VariableValue: null,
                                                                 IsSystem: true );

        } // update()

    }//class CswUpdateSchema_02C_Case30126

}//namespace ChemSW.Nbt.Schema
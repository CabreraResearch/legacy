using ChemSW.Config;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28570
    /// </summary>
    public class CswUpdateSchema_01V_Case28570 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28570; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.C3_AccessId.ToString(), "c3" );
            _CswNbtSchemaModTrnsctn.setConfigVariableValue(CswConfigurationVariables.ConfigurationVariableNames.C3_UrlStem.ToString(), "http://nbtdaily.chemswlive.com/c3/Search.svc");


        } //Update()

    }//class CswUpdateSchema_01V_Case28570

}//namespace ChemSW.Nbt.Schema
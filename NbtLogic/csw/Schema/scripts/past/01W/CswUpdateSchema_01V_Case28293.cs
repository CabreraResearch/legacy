using ChemSW.Config;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28293
    /// </summary>
    public class CswUpdateSchema_01V_Case28293 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28293; }
        }

        public override void update()
        {

            //create the "C3_UrlStem" config variable
            _CswNbtSchemaModTrnsctn.createConfigurationVariable(
                Name: CswConfigurationVariables.ConfigurationVariableNames.C3_UrlStem,
                Description: "Stem of the url used to access c3 web service methods.",
                VariableValue: "http://localhost/c3/Search.svc",
                IsSystem: false );

            //create the "C3_Username" config variable
            _CswNbtSchemaModTrnsctn.createConfigurationVariable(
                Name: CswConfigurationVariables.ConfigurationVariableNames.C3_Username,
                Description: "The customer name that has been granted access to C3.",
                VariableValue: "nbt_test",
                IsSystem: false );

            //create the "C3_Password" config variable
            _CswNbtSchemaModTrnsctn.createConfigurationVariable(
                Name: CswConfigurationVariables.ConfigurationVariableNames.C3_Password,
                Description: "The password for the customer name that has been granted access to C3.",
                VariableValue: "nbt_test",
                IsSystem: false );

            //create the "C3_AccessId" config variable
            _CswNbtSchemaModTrnsctn.createConfigurationVariable(
                Name: CswConfigurationVariables.ConfigurationVariableNames.C3_AccessId,
                Description: "The access id of the ChemCatCentral database.",
                VariableValue: "chemcatcentral",
                IsSystem: false );


        } //Update()

    }//class CswUpdateSchema_01V_Case28293

}//namespace ChemSW.Nbt.Schema
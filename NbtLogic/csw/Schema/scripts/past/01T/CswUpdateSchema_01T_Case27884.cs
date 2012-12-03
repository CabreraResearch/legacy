using ChemSW.Nbt.csw.Dev;
using ChemSW.Config;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27884
    /// </summary>
    public class CswUpdateSchema_01T_Case27884 : CswUpdateSchemaTo
    {
        public override void update()
        {

            _CswNbtSchemaModTrnsctn.createConfigurationVariable(
                Name: CswConfigurationVariables.ConfigurationVariableNames.container_max_depth,
                Description: "How many generations of containers are shown in a container family display",
                VariableValue: "5",
                IsSystem: false );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27884; }
        }

        //Update()

    }//class CswUpdateSchema_01T_Case27884

}//namespace ChemSW.Nbt.Schema
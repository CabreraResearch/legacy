using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 5083
    /// </summary>
    public class CswUpdateSchema_01V_Case5083 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 5083; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswNbtResources.ConfigurationVariables.LocationViewRootName,
                "The name of the root level item on location views",
                "Top",
                false );

        } //Update()

    }//class CswUpdateSchema_01V_Case5083

}//namespace ChemSW.Nbt.Schema
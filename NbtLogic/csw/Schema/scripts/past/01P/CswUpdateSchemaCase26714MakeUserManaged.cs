
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26714
    /// </summary>
    public class CswUpdateSchemaCase26714MakeUserManaged : CswUpdateSchemaTo
    {
        public override void update()
        {

            _CswNbtSchemaModTrnsctn.deleteConfigurationVariable( CswNbtResources.ConfigurationVariables.total_comments_lines );
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswNbtResources.ConfigurationVariables.total_comments_lines, "Total number of lines allowed for comments properties", "10", false );
        }//Update()

    }//class CswUpdateSchemaCase26714MakeUserManaged

}//namespace ChemSW.Nbt.Schema
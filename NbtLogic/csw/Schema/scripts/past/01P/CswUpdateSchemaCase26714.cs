
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26714
    /// </summary>
    public class CswUpdateSchemaCase26714 : CswUpdateSchemaTo
    {
        public override void update()
        {

            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswNbtResources.ConfigurationVariables.total_comments_lines, "Length limit of the comments property", "10", true );
        }//Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema
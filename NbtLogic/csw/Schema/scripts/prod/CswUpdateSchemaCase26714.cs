
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26714
    /// </summary>
    public class CswUpdateSchemaCase26714 : CswUpdateSchemaTo
    {
        public override void update()
        {

            _CswNbtSchemaModTrnsctn.createConfigurationVariable( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.CommentsTruncationLimit, "Length limit of the comments property", "10", true );
        }//Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema
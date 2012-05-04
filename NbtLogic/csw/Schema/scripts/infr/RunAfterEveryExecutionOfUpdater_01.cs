
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Post-schema update script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Post-Script";

        public override void update()
        {
            // case 26029
            // This should always be run after schema updates in order to synchronize enabled nodetypes
            _CswNbtSchemaModTrnsctn.MetaData.ResetEnabledNodeTypes();

        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema



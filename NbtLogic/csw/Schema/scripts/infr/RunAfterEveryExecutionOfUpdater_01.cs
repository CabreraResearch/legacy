
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

            #region 26011
            _CswNbtSchemaModTrnsctn.makeMissingAuditTablesAndColumns();
            #endregion
        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema



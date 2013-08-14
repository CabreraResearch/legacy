using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Adds a new column to the update_history table
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30389 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30389";

        public override string ScriptName
        {
            get { return "02F_Case30389"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30389; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            const string UpdateHistoryTblName = "update_history";
            const string ScriptNameColumn = "scriptname";
            const string SucceededColumn = "succeeded";

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( UpdateHistoryTblName, ScriptNameColumn ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( UpdateHistoryTblName, ScriptNameColumn, "Unique name associated with this schema script.", false, true, 400 );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( UpdateHistoryTblName, SucceededColumn ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( UpdateHistoryTblName, SucceededColumn, "1 if the script ran successfully and 0 otherwise.", false, true );
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30389
}//namespace ChemSW.Nbt.Schema
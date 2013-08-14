using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Adds a new column to the update_history table
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30389 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30389";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30389; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30389"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            const string UpdateHistoryTblName = "update_history";
            const string ScriptNameColumn = "scriptname";

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( UpdateHistoryTblName, "scriptname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( UpdateHistoryTblName, ScriptNameColumn, "Unique name associated with this schema script.", false, true, 400 );
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30389
}//namespace ChemSW.Nbt.Schema
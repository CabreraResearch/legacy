using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02E_Case30500 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30500; }
        }

        public override void update()
        {
            // Add a ScriptName column and a Succeeded column to the update_history table
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
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Runs the initialize script
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {
        public static string Title = FileName;

        private const string FileName = "nbt_initialize_ora.sql";

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.runExternalSqlScript( FileName, ChemSW.Nbt.Properties.Resources.nbt_initialize_ora_sql );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //update()

    }//class RunBeforeEveryExecutionOfUpdater_02

}//namespace ChemSW.Nbt.Schema

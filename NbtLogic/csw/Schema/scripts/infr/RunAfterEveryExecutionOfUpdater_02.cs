using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Runs the finalize script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {
        public static string Title = FileName;

        private const string FileName = "nbt_finalize_ora.sql";

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.runExternalSqlScript( FileName, ChemSW.Nbt.Properties.Resources.nbt_finalize_ora_sql );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        //Update()

    }//class RunAfterEveryExecutionOfUpdater_02

}//namespace ChemSW.Nbt.Schema



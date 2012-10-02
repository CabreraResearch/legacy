using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Applies indexes
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_03 : CswUpdateSchemaTo
    {
        public static string Title = FileName;

        private const string FileName = "indexes.sql";

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.runExternalSqlScript( FileName, ChemSW.Nbt.Properties.Resources.indexes_sql );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        //Update()

    }//class RunAfterEveryExecutionOfUpdater_03

}//namespace ChemSW.Nbt.Schema




using ChemSW.Nbt.csw.Dev;

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


            //case 23784
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update scheduledrules set reprobate=0,totalroguecount=0,failedcount=0" );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        //Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema



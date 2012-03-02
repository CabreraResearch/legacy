
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-01
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {

        public override void update()
        {
            //***************  ADD your own code
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState("Ran run-before script 1");

        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema




namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-01
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {

        public override void update()
        {
            //***************  ADD your own code
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState("Ran before-script 2");

        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema



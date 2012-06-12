
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_03 : CswUpdateSchemaTo
    {
        /* TODO: Delete _03 on moving to Quince */
        public static string Title = "Pre-Script: Make jct_nodes_props Hidden column";

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "jct_nodes_props", "hidden" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "jct_nodes_props", "hidden", "Determines whether property displays.", true, false );
            }

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_03

}//namespace ChemSW.Nbt.Schema



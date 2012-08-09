
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            if(false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase("nodes", "istemp"))
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn("nodes", "istemp", "Node is temporary", false, true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql("update nodes set istemp='0'");
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "jct_nodes_props", "istemp" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "jct_nodes_props", "istemp", "Property is temporary", false, true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update jct_nodes_props set istemp='0'" );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodes", "sessionid" ) )
            {
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodes", "sessionid", "Session ID of temporary node", false, false, "sessionlist", "sessionid" );
            }
        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema



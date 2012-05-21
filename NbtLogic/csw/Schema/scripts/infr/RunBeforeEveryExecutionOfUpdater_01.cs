using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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



            // case 26029
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetypes", "enabled" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetypes", "enabled", "Whether the nodetype is enabled according to module settings", false, false );
            }

            // case 25978
            // FOR PROSPERO
            if( false == _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( "batch" ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( "batch", "batchid" );
                _CswNbtSchemaModTrnsctn.addStringColumn( "batch", "opname", "Batch operation name", false, true, 30 );
                _CswNbtSchemaModTrnsctn.addClobColumn( "batch", "batchdata", "Data for batch operation", false, false );
                _CswNbtSchemaModTrnsctn.addDateColumn( "batch", "startdate", "Date batch operation was created", false, false );
                _CswNbtSchemaModTrnsctn.addDateColumn( "batch", "enddate", "Date batch operation was finished", false, false );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "batch", "userid", "User requesting batch operation", false, false, "nodes", "nodeid" );
                _CswNbtSchemaModTrnsctn.addClobColumn( "batch", "log", "Operation log", false, false );
                _CswNbtSchemaModTrnsctn.addLongColumn( "batch", "priority", "Numeric priority value", false, false );
                _CswNbtSchemaModTrnsctn.addStringColumn( "batch", "status", "Status of operation", false, false, 20 );
            }
            

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema



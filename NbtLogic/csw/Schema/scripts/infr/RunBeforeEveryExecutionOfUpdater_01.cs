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

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema



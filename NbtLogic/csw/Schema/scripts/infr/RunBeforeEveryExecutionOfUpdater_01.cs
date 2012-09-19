using ChemSW.Core;
using ChemSW.Nbt;
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

            #region ROMEO

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "istemp" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes", "istemp", "Node is temporary", logicaldelete: false, required: true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "sessionid" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodes", "sessionid", "Session ID of temporary node", logicaldelete: false, required: false, datatypesize: 50 );
            }
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update nodes set istemp='0', sessionid=''" );
            
            #endregion ROMEO

            #region SEBASTIAN

            //Add 5 generic nodetype prop attribute columns
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute1" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute1", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute2" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute2", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute3" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute3", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute4" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute4", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute5" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute5", "Generic nodetype prop attribute col", false, false, 100 );
            }

            #endregion SEBASTIAN


        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema



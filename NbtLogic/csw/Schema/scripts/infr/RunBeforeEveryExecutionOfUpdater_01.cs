
using ChemSW.Nbt.csw.Dev;

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

            // hack for backwards support of Quince schemata
            if( "01Q-008" == _CswNbtSchemaModTrnsctn.getConfigVariableValue( "schemaversion" ) )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( "schemaversion", "01R-008" );
            }

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
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", "haslabel" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetypes", "haslabel", "Indicated whether the NodeType maps to a print label", false, false );
            }

            #endregion SEBASTIAN

            #region TITANIA

            #endregion TITANIA

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }
        public override int CaseNo
        {
            get { return 0; }
        }
        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema




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

            /* TODO: Delete on moving to Quince */
            //case 26881
            CswSequenceName CswSequenceName = new Nbt.CswSequenceName( "tablecolid" );
            if( false == _CswNbtSchemaModTrnsctn.doesSequenceExist( CswSequenceName ) )
            {
                _CswNbtSchemaModTrnsctn.makeSequence( CswSequenceName, "", "", 0, 100001963 );
            }



            // case 24441
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_props", "textarearows" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class_props", "textarearows", "Height in rows(memo) or pixels(image)", false, false );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_props", "textareacols" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class_props", "textareacols", "Width in characters(memo) or pixels(image)", false, false );
            }

            /* TODO: Delete on moving to Quince */
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "jct_nodes_props", "hidden" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "jct_nodes_props", "hidden", "Determines whether property displays.", true, false );
            }

            // case 26957
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_layout", "tabgroup" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_layout", "tabgroup", "Assign properties into a group on a tab", false, false, 50 );
            }

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema




namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-01
    /// </summary>
    public class CswUpdateSchemaTo01I01 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 01 ); } }
        public CswUpdateSchemaTo01I01( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
            // This script is reserved for schema changes, 
            // such as adding tables or columns, 
            // which need to take place before any other changes can be made.

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "jct_nodes_props", "field2_numeric" ) )
            {
                _CswNbtSchemaModTrnsctn.addDoubleColumn( "jct_nodes_props", "field2_numeric", "A second numeric value", false, false, 6 );
            }

            // case 8411
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "firsttabversionid" ) )
            {
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "firsttabversionid", "Foreign key to original tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "priortabversionid" ) )
            {
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "priortabversionid", "Foreign key to previous tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
            }

            //Case 23132
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "session_data", "keepinquicklaunch" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "session_data", "keepinquicklaunch", "Determines whether to keep the item in User's Quick Launch", true, false );
            }
        }//Update()

    }//class CswUpdateSchemaTo01I01

}//namespace ChemSW.Nbt.Schema



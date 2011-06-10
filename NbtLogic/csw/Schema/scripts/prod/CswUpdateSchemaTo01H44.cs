
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-44
    /// </summary>
    public class CswUpdateSchemaTo01H44 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 44 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H44( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "jct_nodes_props" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "nodes" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "nodetype_props" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "nodetypes" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "object_class" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "object_class_props" );
        } // update()

    }//class CswUpdateSchemaTo01H44

}//namespace ChemSW.Nbt.Schema


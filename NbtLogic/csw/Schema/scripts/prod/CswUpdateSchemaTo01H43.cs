
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-43
    /// </summary>
    public class CswUpdateSchemaTo01H43 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 43 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H43( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            //tables that were previously "autitable" have an auditlevel column that is 
            //to narrow. There may be other inconsistencies. So I am just nuking them
            //here, and will restore them in the next script (because you cannot both drop
            //and add the same table within a transaction). 
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "jct_nodes_props" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "jct_nodes_props_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodes" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodes_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodetype_props" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodetype_props_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodetypes" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodetypes_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "object_class" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "object_class_props" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "object_class_props_audit" );

        } // update()

    }//class CswUpdateSchemaTo01H43

}//namespace ChemSW.Nbt.Schema


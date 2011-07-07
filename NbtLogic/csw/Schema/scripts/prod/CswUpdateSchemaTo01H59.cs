
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-59
    /// </summary>
    public class CswUpdateSchemaTo01H59 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 59 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H59( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            //Case 22712
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( "mobileview_resultlimit", "Number of results to display for views on Mobile", "30", false );

        } // update()


    }//class CswUpdateSchemaTo01H59

}//namespace ChemSW.Nbt.Schema


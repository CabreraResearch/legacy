
using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-37
    /// </summary>
    public class CswUpdateSchemaTo01H37 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 37 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H37( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // case 21582
			// new table for session views / quick launch

			// ordinarily we do schema DDL in schema script 1, however 
			// I'm sure this functionality will not be used by schema updater itself, 
			// and therefore it is safe to add it here

			string SessionViewsTableName = "session_views";
			_CswNbtSchemaModTrnsctn.addTable( SessionViewsTableName, "sessionviewid" );
			_CswNbtSchemaModTrnsctn.addStringColumn( SessionViewsTableName, "viewname", "Name of view", false, false, 30 );
			_CswNbtSchemaModTrnsctn.addStringColumn( SessionViewsTableName, "viewmode", "Rendering Mode for view", false, false, 10 );
			_CswNbtSchemaModTrnsctn.addClobColumn( SessionViewsTableName, "viewxml", "View XML", false, false );
			_CswNbtSchemaModTrnsctn.addStringColumn( SessionViewsTableName, "sessionid", "User Session ID for view", false, true, 50 );


        } // update()

    }//class CswUpdateSchemaTo01H37

}//namespace ChemSW.Nbt.Schema


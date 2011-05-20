
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

			//string SessionViewsTableName = "session_data";
			string Tbl = CswNbtSessionDataMgr.SessionDataTableName;
			_CswNbtSchemaModTrnsctn.addTable( Tbl,  CswNbtSessionDataMgr.SessionDataColumn_PrimaryKey );
			_CswNbtSchemaModTrnsctn.addStringColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_SessionId, "User Session ID for view", false, true, 50 );
			_CswNbtSchemaModTrnsctn.addStringColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_SessionDataType, "Type: view or action", false, false, 10 );
			_CswNbtSchemaModTrnsctn.addStringColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_Name, "Name of view or action", false, false, 30 );
			_CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_ActionId, "Primary key of action", false, false, "actions", "actionid" );
			_CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_ViewId, "Primary key of view", false, false, "node_views", "viewid" );
			_CswNbtSchemaModTrnsctn.addStringColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_ViewMode, "Rendering Mode for view", false, false, 10 );
			_CswNbtSchemaModTrnsctn.addClobColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_ViewXml, "View XML", false, false );
			_CswNbtSchemaModTrnsctn.addBooleanColumn( Tbl, CswNbtSessionDataMgr.SessionDataColumn_QuickLaunch, "Include this in quick launch", false, false );

        } // update()

    }//class CswUpdateSchemaTo01H37

}//namespace ChemSW.Nbt.Schema


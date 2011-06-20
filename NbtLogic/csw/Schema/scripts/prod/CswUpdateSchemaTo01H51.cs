using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-51
    /// </summary>
    public class CswUpdateSchemaTo01H51 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 51 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H51( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

		public void update()
        {
            // case 22238
			// remove invalid welcome components with no nodetypeid, viewid, reportid, or actionid
			CswTableUpdate WelcomeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H51_Welcome_Update", "welcome" );
			DataTable WelcomeTable = WelcomeUpdate.getTable();
			foreach( DataRow WelcomeRow in WelcomeTable.Rows )
			{
				if( WelcomeRow["nodeviewid"].ToString() == string.Empty &&
					WelcomeRow["nodetypeid"].ToString() == string.Empty &&
					WelcomeRow["reportid"].ToString() == string.Empty &&
					WelcomeRow["actionid"].ToString() == string.Empty &&
					WelcomeRow["componenttype"].ToString() != "Text" )
				{
					WelcomeRow.Delete();
				}
			} // foreach( DataRow WelcomeRow in WelcomeTable.Rows )
			WelcomeUpdate.update( WelcomeTable );

		} // update()

    }//class CswUpdateSchemaTo01H51

}//namespace ChemSW.Nbt.Schema


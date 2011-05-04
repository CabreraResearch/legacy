using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-30
    /// </summary>
    public class CswUpdateSchemaTo01H30 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 30 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H30( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {

			// this didn't work on 01H-26 for some reason, so do it again:

			// case 21364
			// fill new viewmode column on node_views

			CswTableUpdate ViewsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H30_update_views", "node_views" );
			DataTable ViewsTable = ViewsUpdate.getTable();
			foreach( DataRow ViewsRow in ViewsTable.Rows )
			{
				CswNbtView ThisView = _CswNbtSchemaModTrnsctn.restoreView( CswConvert.ToInt32( ViewsRow["nodeviewid"] ) );
				ViewsRow["viewmode"] = ThisView.ViewMode.ToString();
			}
			ViewsUpdate.update( ViewsTable );







        } // update()

    }//class CswUpdateSchemaTo01H30

}//namespace ChemSW.Nbt.Schema


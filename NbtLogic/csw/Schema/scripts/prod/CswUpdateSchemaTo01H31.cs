using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-31
	/// </summary>
	public class CswUpdateSchemaTo01H31 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 31 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H31( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

		public void update()
		{
			// case 21519 - find views with the wrong viewmode set

			string SqlText = "select v.nodeviewid " +
							"from nodetype_props p " +
							"join field_types f on p.fieldtypeid = f.fieldtypeid " +
							"join node_views v on p.nodeviewid = v.nodeviewid " +
							"where f.fieldtype = 'Grid' and v.viewmode = 'Tree' ";

			DataTable BadViewsTable = _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSqlSelect( "01H-31_View_Select", SqlText );
			foreach( DataRow BadViewRow in BadViewsTable.Rows )
			{
				CswNbtView BadView = _CswNbtSchemaModTrnsctn.restoreView( new CswNbtViewId( CswConvert.ToInt32( BadViewRow["nodeviewid"] ) ) );
				BadView.ViewMode = NbtViewRenderingMode.Grid;
				BadView.save();
			}


		} // update()

	}//class CswUpdateSchemaTo01H31

}//namespace ChemSW.Nbt.Schema


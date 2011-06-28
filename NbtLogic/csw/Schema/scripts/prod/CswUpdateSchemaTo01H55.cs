using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-55
	/// </summary>
	public class CswUpdateSchemaTo01H55 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 55 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H55( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{
			// case 22484
			// set reprobatethreshold to 300000 (5 minutes)

			CswTableUpdate SRUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H55_ScheduledRules_Update", "scheduledrules" );
			DataTable SRTable = SRUpdate.getTable();
			foreach( DataRow SRRow in SRTable.Rows )
			{
				SRRow["maxruntimems"] = CswConvert.ToDbVal( 300000 );

				// also reset rules that have been marked reprobate
				SRRow["reprobate"] = CswConvert.ToDbVal( 0 );
				SRRow["totalroguecount"] = CswConvert.ToDbVal( 0 );
				SRRow["failedcount"] = CswConvert.ToDbVal( 0 );
			}
			SRUpdate.update(SRTable);



			// case 22010
			CswNbtMetaDataNodeType InspSchedNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType("Physical Inspection Schedule");
			if( InspSchedNT != null )
			{
				InspSchedNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( "Inspection Group" ) + " " + CswNbtMetaData.MakeTemplateEntry( "Summary" );
			}



		} // update()


	}//class CswUpdateSchemaTo01H55

}//namespace ChemSW.Nbt.Schema


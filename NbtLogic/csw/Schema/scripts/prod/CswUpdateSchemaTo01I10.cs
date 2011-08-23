using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01I-10
	/// </summary>
	public class CswUpdateSchemaTo01I10 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 10 ); } }
		public CswUpdateSchemaTo01I10( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}


		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


		public void update()
		{
			// case 23088
			// fix 'getActiveActions' S4

			_CswNbtSchemaModTrnsctn.UpdateS4( "getActiveActions", @"select a.actionid, a.actionname, a.showinlist, a.url, a.category, lower(a.actionname) mssqlorder
  from actions a
 where exists (select m.moduleid
          from modules m, jct_modules_actions jma
         where jma.actionid = a.actionid
           and jma.moduleid = m.moduleid
           and m.enabled = '1')
       or not exists (select m.moduleid
          from modules m, jct_modules_actions jma
         where jma.actionid = a.actionid
           and jma.moduleid = m.moduleid)
 order by lower(a.actionname)" );



		} // Update()


	}//class CswUpdateSchemaTo01I10

}//namespace ChemSW.Nbt.Schema



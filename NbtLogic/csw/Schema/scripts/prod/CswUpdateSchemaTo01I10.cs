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
	public class CswUpdateSchemaTo01I10 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 10 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
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


			// case 22955
			// Add new NFPA fieldtype

			CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I10_FieldTypes_Update", "field_types" );
			DataTable FieldTypeTable = FieldTypesUpdate.getEmptyTable();
			DataRow NewFTRow = FieldTypeTable.NewRow();
			NewFTRow["auditflag"] = "0";
			NewFTRow["datatype"] = "text";
			NewFTRow["deleted"] = CswConvert.ToDbVal( false );
			NewFTRow["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.NFPA.ToString();
			FieldTypeTable.Rows.Add( NewFTRow );
			FieldTypesUpdate.update( FieldTypeTable );

		} // Update()


	}//class CswUpdateSchemaTo01I10

}//namespace ChemSW.Nbt.Schema



using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01I-04
	/// </summary>
	public class CswUpdateSchemaTo01I04 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 04 ); } }

		public override void update()
		{
			// case 22978

			CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I04_FieldTypes_Update", "field_types" );
			DataTable FieldTypeTable = FieldTypesUpdate.getEmptyTable();
			DataRow NewFTRow = FieldTypeTable.NewRow();
			NewFTRow["auditflag"] = "0";
			NewFTRow["datatype"] = "text";
			NewFTRow["deleted"] = CswConvert.ToDbVal( false );
			NewFTRow["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.MultiList.ToString();
			FieldTypeTable.Rows.Add( NewFTRow );
			FieldTypesUpdate.update( FieldTypeTable );
			
			

		} // Update()

	}//class CswUpdateSchemaTo01I04

}//namespace ChemSW.Nbt.Schema



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
	/// Updates the schema to version 01I-14
	/// </summary>
	public class CswUpdateSchemaTo01I14 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 14 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I14_FieldTypes_Update", "field_types" );
			DataTable FieldTypeTable = FieldTypesUpdate.getTable( null, string.Empty, Int32.MinValue, "where fieldtype='" + CswNbtMetaDataFieldType.NbtFieldType.MOL.ToString() + "'", false, null, Int32.MinValue, Int32.MinValue, false );
            if( FieldTypeTable.Rows.Count > 0 )
            {
                FieldTypeTable.Rows[0]["deleted"] = CswConvert.ToDbVal( false );
                FieldTypesUpdate.update( FieldTypeTable );
            }
		} // Update()


	}//class CswUpdateSchemaTo01I14

}//namespace ChemSW.Nbt.Schema



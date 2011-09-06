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
	public class CswUpdateSchemaTo01I14 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 14 ); } }
		public CswUpdateSchemaTo01I14( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}


		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


		public void update()
		{
            CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I14_FieldTypes_Update", "field_types" );
            DataTable FieldTypeTable = FieldTypesUpdate.getTable("where fieldtype='" + CswNbtMetaDataFieldType.NbtFieldType.MOL.ToString() + "'");
            FieldTypeTable.Rows[0]["deleted"] = CswConvert.ToDbVal(false);
            FieldTypesUpdate.update( FieldTypeTable );

		} // Update()


	}//class CswUpdateSchemaTo01I14

}//namespace ChemSW.Nbt.Schema



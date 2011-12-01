using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01K-04
	/// </summary>
	public class CswUpdateSchemaTo01K04 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 04 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 24179
			// Remove grid properties from Preview
			CswNbtMetaDataFieldType GridFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid );
			CswTableUpdate LayoutUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01K04_layout_update", "nodetype_layout" );
			DataTable LayoutTable = LayoutUpdate.getTable( "where layouttype = 'Preview' and nodetypepropid in (select nodetypepropid from nodetype_props where fieldtypeid = " + GridFT.FieldTypeId.ToString() + ")" );
			foreach( DataRow GridLayoutRow in LayoutTable.Rows )
			{
				GridLayoutRow.Delete();
			}
			LayoutUpdate.update( LayoutTable );

		}//Update()

	}//class CswUpdateSchemaTo01K04

}//namespace ChemSW.Nbt.Schema



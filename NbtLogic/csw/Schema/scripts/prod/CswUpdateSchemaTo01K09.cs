using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01K-09
	/// </summary>
	public class CswUpdateSchemaTo01K09 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 09 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 24090
			CswNbtMetaDataNodeType DepartmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Department" );
			CswNbtMetaDataNodeType VendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
			if( DepartmentNT != null || VendorNT != null )
			{
				CswTableUpdate JctModulesNTTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01K09_ModuleJunctionUpdate", "jct_modules_nodetypes" );
				DataTable JctModulesNTDataTable = JctModulesNTTable.getTable();
				foreach( DataRow JctRow in JctModulesNTDataTable.Rows )
				{
					Int32 ThisNodeTypeId = CswConvert.ToInt32( JctRow["nodetypeid"] );
					if( ( DepartmentNT != null && ThisNodeTypeId == DepartmentNT.NodeTypeId ) ||
						( VendorNT != null && ThisNodeTypeId == VendorNT.NodeTypeId ) )
					{
						JctRow.Delete();
					}
				}
				JctModulesNTTable.update( JctModulesNTDataTable );
			}
		}//Update()

	}//class CswUpdateSchemaTo01K09

}//namespace ChemSW.Nbt.Schema



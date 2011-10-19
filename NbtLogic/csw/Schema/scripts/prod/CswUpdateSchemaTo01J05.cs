using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01J-05
	/// </summary>
	public class CswUpdateSchemaTo01J05 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 05 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 23583
			CswNbtMetaDataFieldType LogicalSetFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.LogicalSet );
			CswNbtMetaDataObjectClass CustomerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
			CswNbtMetaDataObjectClassProp ModulesEnabledOCP = CustomerOC.getObjectClassProp( CswNbtObjClassCustomer.ModulesEnabledPropertyName );
			CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01J05_OCP_Update", "object_class_props" );

			// Change 'Modules Enabled' property from 'Static' to 'Logical Set'
			DataTable OCPTable = OCPUpdate.getTable( "objectclasspropid", ModulesEnabledOCP.PropId );
			if( OCPTable.Rows.Count > 0 )
			{
				OCPTable.Rows[0][CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid.ToString()] = CswConvert.ToDbVal( LogicalSetFT.FieldTypeId );
			}

			// Add 'ChemSW Admin Password' field to Customer
			_CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, CustomerOC.ObjectClassId, CswNbtObjClassCustomer.ChemSWAdminPasswordPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Password, Int32.MinValue, Int32.MinValue );

			OCPUpdate.update( OCPTable );

			// If nodetypes exist, delete the existing Modules Enabled property
			if( CustomerOC.NodeTypes.Count > 0 )
			{
				CswTableUpdate JNPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01J05_JNP_Update", "jct_nodes_props" );
				CswTableUpdate NTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01J05_NTP_Update", "nodetype_props" );
				foreach( CswNbtMetaDataNodeType CustomerNT in CustomerOC.NodeTypes )
				{
					if( CustomerNT.IsLatestVersion )
					{
						CswNbtMetaDataNodeTypeProp ModulesEnabledNTP = CustomerNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassCustomer.ModulesEnabledPropertyName );
						DataTable NTPTable = NTPUpdate.getTable( "firstpropversionid", ModulesEnabledNTP.FirstPropVersionId );
						foreach( DataRow NTPRow in NTPTable.Rows )
						{
							DataTable JNPTable = JNPUpdate.getTable( "nodetypepropid", CswConvert.ToInt32( NTPRow["nodetypepropid"] ) );
							foreach( DataRow JNPRow in JNPTable.Rows )
							{
								JNPRow.Delete();
							}
							JNPUpdate.update( JNPTable );
							NTPRow.Delete();
						}
						NTPUpdate.update( NTPTable );
					}
				}
			}
			_CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

		}//Update()

	}//class CswUpdateSchemaTo01J05

}//namespace ChemSW.Nbt.Schema



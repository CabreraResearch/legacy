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
	/// Updates the schema to version 01I-05
	/// </summary>
	public class CswUpdateSchemaTo01I05 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 05 ); } }
		public CswUpdateSchemaTo01I05( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}


		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


		public void update()
		{
			_CswNbtSchemaModTrnsctn.MetaData.refreshAll();

			// case 8411
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "firsttabversionid" ) )
			{
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "firsttabversionid", "Foreign key to original tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
			}
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "priortabversionid" ) )
			{
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "priortabversionid", "Foreign key to previous tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
			}


			CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
			CswNbtMetaDataObjectClassProp RoleNodeTypePermOCP = RoleOC.getObjectClassProp( CswNbtObjClassRole.NodeTypePermissionsPropertyName );
			CswNbtMetaDataObjectClassProp RoleActionPermOCP = RoleOC.getObjectClassProp( CswNbtObjClassRole.ActionPermissionsPropertyName );
			CswNbtMetaDataFieldType MultiListFieldType = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MultiList );

			// Build collection of all permissions currently granted
			Collection<TempPermission> ExistingPermissions = new Collection<TempPermission>();

			foreach( CswNbtMetaDataNodeType RoleNT in RoleOC.NodeTypes )
			{
				foreach( CswNbtNode RoleNode in RoleNT.getNodes( false, true ) )
				{
					DataTable NTPermTable = RoleNode.Properties[CswNbtObjClassRole.NodeTypePermissionsPropertyName].AsLogicalSet.GetDataAsTable( "name", "key" );
					foreach( DataRow NodeTypePermRow in NTPermTable.Rows )
					{
						CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswConvert.ToInt32( NodeTypePermRow["key"] ) );
						foreach( DataColumn Column in NTPermTable.Columns )
						{
							if( Column.ColumnName != "name" &&
								Column.ColumnName != "key" &&
								true == CswConvert.ToBoolean( NodeTypePermRow[Column] ) )
							{
								CswNbtPermit.NodeTypePermission Permission = (CswNbtPermit.NodeTypePermission) Enum.Parse( typeof( CswNbtPermit.NodeTypePermission ), Column.ColumnName );

								TempPermission Perm = new TempPermission();
								Perm.RoleNodeId = RoleNode.NodeId;
								Perm.NodeType = NodeType;
								Perm.Permission = Permission;
								ExistingPermissions.Add( Perm );
							}
						} // foreach( DataColumn Column in NodeTypePermTable.Columns )
					} // foreach( DataRow NodeTypePermRow in NodeTypePermTable.Rows )


					DataTable ActionPermTable = RoleNode.Properties[CswNbtObjClassRole.ActionPermissionsPropertyName].AsLogicalSet.GetDataAsTable( "name", "key" );
					foreach( DataRow ActionPermRow in ActionPermTable.Rows )
					{
						CswNbtAction Action = _CswNbtSchemaModTrnsctn.Actions[CswNbtAction.ActionNameStringToEnum( ActionPermRow["key"].ToString() )];
						foreach( DataColumn Column in ActionPermTable.Columns )
						{
							if( Column.ColumnName != "name" &&
								Column.ColumnName != "key" &&
								true == CswConvert.ToBoolean( ActionPermRow[Column] ) )
							{
								TempPermission Perm = new TempPermission();
								Perm.RoleNodeId = RoleNode.NodeId;
								Perm.Action = Action;
								ExistingPermissions.Add( Perm );
							}
						} // foreach( DataColumn Column in NodeTypePermTable.Columns )
					} // foreach( DataRow NodeTypePermRow in NodeTypePermTable.Rows )
				} // foreach( CswNbtNode RoleNode in RoleNT.getNodes( false, true ) )
			} // foreach( CswNbtMetaDataNodeType RoleNT in RoleOC.NodeTypes )


			// update NodeType Permissions and Action Permissions object class props to be MultiList
			// this also updates the nodetype props

			//CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I05_OCP_Update", "object_class_props" );
			//DataTable OCPTable = OCPUpdate.getEmptyTable();
			_CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RoleNodeTypePermOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, MultiListFieldType.FieldTypeId );
			_CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RoleActionPermOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, MultiListFieldType.FieldTypeId );
			//OCPUpdate.update( OCPTable );

			_CswNbtSchemaModTrnsctn.MetaData.refreshAll();
			_CswNbtSchemaModTrnsctn.Nodes.Clear();

			// clear existing permission values since they are no longer valid
			foreach( CswNbtMetaDataNodeType RoleNT in RoleOC.NodeTypes )
			{
				foreach( CswNbtNode RoleNode in RoleNT.getNodes( false, true ) )
				{
					CswNbtObjClassRole RoleNodeAsRole = CswNbtNodeCaster.AsRole( RoleNode );
					RoleNodeAsRole.NodeTypePermissions.Value = new CswCommaDelimitedString();
					RoleNodeAsRole.ActionPermissions.Value = new CswCommaDelimitedString();
				}
			} // foreach( CswNbtMetaDataNodeType RoleNT in RoleOC.NodeTypes )

			// set new value of permission properties using old value
			Collection<CswNbtNode> RoleNodes = new Collection<CswNbtNode>();
			foreach( TempPermission Perm in ExistingPermissions )
			{
				CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes[Perm.RoleNodeId];
				CswNbtObjClassRole RoleNodeAsRole = CswNbtNodeCaster.AsRole( RoleNode );
				
				if( Perm.NodeType != null )
				{
					//_CswNbtSchemaModTrnsctn.Permit.set( Perm.Permission, Perm.NodeType, RoleNodeAsRole, true );
					RoleNodeAsRole.NodeTypePermissions.AddValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( Perm.NodeType, Perm.Permission ) );
				}
				else if( Perm.Action != null )
				{
					//_CswNbtSchemaModTrnsctn.Permit.set( Perm.Action, RoleNodeAsRole, true );
					RoleNodeAsRole.ActionPermissions.AddValue( CswNbtObjClassRole.MakeActionPermissionValue( Perm.Action ) );
				}
				if( !RoleNodes.Contains( RoleNode ) )
				{
					RoleNodes.Add( RoleNode );
				}
			} // foreach(TempPermission Perm in ExistingPermissions)

			// saving changes at the end is faster
			foreach( CswNbtNode RoleNode in RoleNodes )
			{
				RoleNode.postChanges( false );
			}
			
			// case 8635

			CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I05_FieldTypes_Update", "field_types" );
			DataTable FieldTypeTable = FieldTypesUpdate.getEmptyTable();
			DataRow NewFTRow = FieldTypeTable.NewRow();
			NewFTRow["auditflag"] = "0";
			NewFTRow["datatype"] = "text";
			NewFTRow["deleted"] = CswConvert.ToDbVal(false);
			NewFTRow["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.Button.ToString();
			FieldTypeTable.Rows.Add( NewFTRow );
			FieldTypesUpdate.update( FieldTypeTable );


	
			
		} // Update()


		private class TempPermission
		{
			public CswPrimaryKey RoleNodeId = null;
			public CswNbtMetaDataNodeType NodeType = null;
			public CswNbtPermit.NodeTypePermission Permission;
			public CswNbtAction Action = null;
		}



	}//class CswUpdateSchemaTo01I05

}//namespace ChemSW.Nbt.Schema



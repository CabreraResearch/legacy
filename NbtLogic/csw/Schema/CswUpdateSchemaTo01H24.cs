using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-24
	/// </summary>
	public class CswUpdateSchemaTo01H24 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 24 ); } }
		public CswUpdateSchemaTo01H24( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
			// For HORATIO.2

			// case 20791 - lowercase rate interval XML for compatibility with javascript
			CswCommaDelimitedString InClause = new CswCommaDelimitedString();
			foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )
			{
				foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps )
				{
					if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.TimeInterval )
					{
						InClause.Add( Prop.PropId.ToString() );
					}
				}
			}

			CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H24_Jct_Update", "jct_nodes_props" );
			DataTable JctTable = JctUpdate.getTable( "where nodetypepropid in (" + InClause.ToString() + ")" );
			foreach( DataRow JctRow in JctTable.Rows )
			{
				JctRow["clobdata"] = JctRow["clobdata"].ToString().ToLower();
			}
			JctUpdate.update( JctTable );


			// case 21250
			// User Page Size property
			//_CswNbtSchemaModTrnsctn.addObjectClassPropRow
			CswNbtMetaDataObjectClass UserClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
			CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-24_OCP_Update", "object_class_props" );
			DataTable OCPTable = OCPUpdate.getEmptyTable();

			DataRow PageSizeRow = _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, 
															UserClass.ObjectClassId, 
															CswNbtObjClassUser.PageSizePropertyName, 
															CswNbtMetaDataFieldType.NbtFieldType.Number, 
															Int32.MinValue, Int32.MinValue );
			PageSizeRow[CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberprecision.ToString()] = CswConvert.ToDbVal( 0 );
			PageSizeRow[CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberminvalue.ToString()] = CswConvert.ToDbVal( 1 );
			PageSizeRow[CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numbermaxvalue.ToString()] = CswConvert.ToDbVal( 1000 );
			PageSizeRow[CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired.ToString()] = CswConvert.ToDbVal( true );

			OCPUpdate.update( OCPTable );

			// this does a refreshAll()
			_CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

			// default value is 50
			CswNbtMetaDataObjectClassProp PageSizeOCP = UserClass.getObjectClassProp(CswNbtObjClassUser.PageSizePropertyName);
			_CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( PageSizeOCP, CswNbtSubField.SubFieldName.Value, 50 );

			foreach( CswNbtMetaDataNodeType NodeType in UserClass.NodeTypes )
			{
				CswNbtMetaDataNodeTypeProp PageSizeNTP = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassUser.PageSizePropertyName );
				foreach(CswNbtNode Node in NodeType.getNodes(true, true))
				{
					CswNbtObjClassUser UserNode = CswNbtNodeCaster.AsUser(Node);
					UserNode.PageSizeProperty.Value = 50;
					UserNode.postChanges(false);
				}
			} // foreach( CswNbtMetaDataNodeType NodeType in UserClass.NodeTypes )

		} // update()

	}//class CswUpdateSchemaTo01H24

}//namespace ChemSW.Nbt.Schema


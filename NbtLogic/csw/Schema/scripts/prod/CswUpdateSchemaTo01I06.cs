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
	/// Updates the schema to version 01I-06
	/// </summary>
	public class CswUpdateSchemaTo01I06 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 06 ); } }
		public CswUpdateSchemaTo01I06( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}


		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


		public void update()
		{
			// case 8411
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "firsttabversionid" ) )
			{
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "firsttabversionid", "Foreign key to original tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
			}
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "priortabversionid" ) )
			{
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "priortabversionid", "Foreign key to previous tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
			}

			// Fill in columns with existing data
			// This will not handle tab renaming, but that's ok
			CswTableUpdate TabsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01G06_TabSet_Update", "nodetype_tabset" );
			DataTable TabsTable = TabsUpdate.getTable();
			foreach( DataRow TabRow in TabsTable.Rows )
			{
				Int32 ThisTabId = CswConvert.ToInt32( TabRow["nodetypetabsetid"] );
				TabRow["firsttabversionid"] = CswConvert.ToInt32( ThisTabId );  // default

				CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswConvert.ToInt32( TabRow["nodetypeid"] ) );

				CswNbtMetaDataNodeTypeTab FirstTab = NodeType.FirstVersionNodeType.getNodeTypeTab( TabRow["tabname"].ToString() );
				if( FirstTab != null )
				{
					TabRow["firsttabversionid"] = CswConvert.ToInt32( FirstTab.TabId );
				}

				if( NodeType.PriorVersionNodeType != null )
				{
					CswNbtMetaDataNodeTypeTab PriorTab = NodeType.PriorVersionNodeType.getNodeTypeTab( TabRow["tabname"].ToString() );
					if( PriorTab != null && PriorTab.TabId != ThisTabId )
					{
						TabRow["priortabversionid"] = CswConvert.ToInt32( PriorTab.TabId );
					}
				}
			}
			TabsUpdate.update( TabsTable );

		} // Update()


	}//class CswUpdateSchemaTo01I06

}//namespace ChemSW.Nbt.Schema



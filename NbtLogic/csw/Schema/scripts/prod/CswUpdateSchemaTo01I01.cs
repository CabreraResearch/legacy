using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-01
    /// </summary>
    public class CswUpdateSchemaTo01I01 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 01 ); } }

		public override void update()
        {
            // This script is reserved for schema changes, 
            // such as adding tables or columns, 
            // which need to take place before any other changes can be made.

			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "jct_nodes_props", "field2_numeric" ) )
			{
				_CswNbtSchemaModTrnsctn.addDoubleColumn( "jct_nodes_props", "field2_numeric", "A second numeric value", false, false, 6 );
			}

			// case 8411
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "firsttabversionid" ) )
			{
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "firsttabversionid", "Foreign key to original tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
			}
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_tabset", "priortabversionid" ) )
			{
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( "nodetype_tabset", "priortabversionid", "Foreign key to previous tab version", false, false, "nodetype_tabset", "nodetypetabsetid" );
			}




			// case 22960
			// Change how we store layouts
			// also in 01I-12
			string LayoutTableName = "nodetype_layout";
			if( false == _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( LayoutTableName ) )
			{
				// create new layout table
				_CswNbtSchemaModTrnsctn.addTable( LayoutTableName, "nodetypelayoutid" );
				_CswNbtSchemaModTrnsctn.addStringColumn( LayoutTableName, "layouttype", "Type of Layout", false, true, 15 );
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( LayoutTableName, "nodetypeid", "NodeType", false, true, "nodetypes", "nodetypeid" );
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( LayoutTableName, "nodetypepropid", "Property", false, true, "nodetype_props", "nodetypepropid" );
				_CswNbtSchemaModTrnsctn.addForeignKeyColumn( LayoutTableName, "nodetypetabsetid", "Tab", false, false, "nodetype_tabset", "nodetypetabsetid" );
				_CswNbtSchemaModTrnsctn.addLongColumn( LayoutTableName, "display_row", "Display row", false, false );
				_CswNbtSchemaModTrnsctn.addLongColumn( LayoutTableName, "display_column", "Display Column", false, false );

				// copy existing layouts from nodetype_props
				CswTableSelect PropSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "01I-11_Prop_Select", "nodetype_props" );
				DataTable PropTable = PropSelect.getTable();

				CswTableUpdate LayoutUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I-11_Layout_Update", LayoutTableName );
				DataTable LayoutTable = LayoutUpdate.getEmptyTable();
				foreach( DataRow PropRow in PropTable.Rows )
				{
					DataRow LayoutEditRow = LayoutTable.NewRow();
					LayoutEditRow["layouttype"] = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit.ToString();
					LayoutEditRow["nodetypeid"] = PropRow["nodetypeid"];
					LayoutEditRow["nodetypepropid"] = PropRow["nodetypepropid"];
					LayoutEditRow["nodetypetabsetid"] = PropRow["nodetypetabsetid"];
					LayoutEditRow["display_row"] = PropRow["display_row"];
					LayoutEditRow["display_column"] = PropRow["display_col"];
					LayoutTable.Rows.Add( LayoutEditRow );

					if( CswConvert.ToBoolean( PropRow["setvalonadd"] ) )
					{
						DataRow LayoutAddRow = LayoutTable.NewRow();
						LayoutAddRow["layouttype"] = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add.ToString();
						LayoutAddRow["nodetypeid"] = PropRow["nodetypeid"];
						LayoutAddRow["nodetypepropid"] = PropRow["nodetypepropid"];
						LayoutAddRow["display_row"] = PropRow["display_row_add"];
						LayoutAddRow["display_column"] = PropRow["display_col_add"];
						LayoutTable.Rows.Add( LayoutAddRow );
					}
				} // foreach( DataRow PropRow in PropTable.Rows )
				LayoutUpdate.update( LayoutTable );

				// remove columns
				_CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "display_row" );
				_CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "display_col" );
				_CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "display_row_add" );
				_CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "display_col_add" );
				_CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "setvalonadd" );
			} // if( false == _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( LayoutTableName ) )


		}//Update()

    }//class CswUpdateSchemaTo01I01

}//namespace ChemSW.Nbt.Schema



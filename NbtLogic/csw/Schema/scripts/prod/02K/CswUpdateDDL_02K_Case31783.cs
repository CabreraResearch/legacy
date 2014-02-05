using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02K_Case31783 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31783; }
        }

        public override string Title
        {
            get { return "Add new layout tables"; }
        }

        public override void update()
        {
            const string LayoutTabTableName = "layout_tab";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( LayoutTabTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( LayoutTabTableName, "layout_tab_id" );

                _CswNbtSchemaModTrnsctn.addStringColumn( LayoutTabTableName, "layout_type", "The type of layout to which this tab belongs", true, 100 );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutTabTableName + " add constraint check_layout_type check (layout_type in ('edit','add','preview','search'))" );
                _CswNbtSchemaModTrnsctn.addStringColumn( LayoutTabTableName, "tab_type", "The type of the tab", true, 100 );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutTabTableName + " add constraint check_tab_type check (tab_type in ('main','identity','history'))" );
                _CswNbtSchemaModTrnsctn.addStringColumn( LayoutTabTableName, "tab_name", "The name of the tab", false, 100 );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutTabTableName, "tab_order", "The order of which to display this tab on the given layout", true );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutTabTableName, "metadata_nodetypeid", "The nodetypeid to which this tab belongs", true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutTabTableName + " add foreign key (metadata_nodetypeid) references nodetypes (nodetypeid)" );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutTabTableName, "design_nodetype_nodeid", "The nodeid of the design nodetype node to which this tab relates", true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutTabTableName + " add foreign key (design_nodetype_nodeid) references nodes (nodeid)" );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutTabTableName, "design_nodetypetab_nodeid", "The nodeid of the design nodetypetab node to which this tab relates", true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutTabTableName + " add foreign key (design_nodetypetab_nodeid) references nodes (nodeid)" );
            }

            const string LayoutColumnTableName = "layout_column";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( LayoutColumnTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( LayoutColumnTableName, "layout_column_id" );

                _CswNbtSchemaModTrnsctn.addStringColumn( LayoutColumnTableName, "column_name", "The name of the column", false, 100 );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutColumnTableName, "column_order", "The order in which to display this column with respect to other members of the same parent", true );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutColumnTableName, "parent_id", "The id of the parent", true );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutColumnTableName, "parent_type", "The type of the parent", true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutColumnTableName + " add constraint check_parent_type check (parent_type in ('tab','column'))" );
            }

            const string LayoutPropertyTableName = "layout_property";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( LayoutPropertyTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( LayoutPropertyTableName, "layout_property_id" );

                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutPropertyTableName, "prop_order", "The order in which to display this property with respect to other members of the same parent column", true );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutPropertyTableName, "column_id", "The id of the parent column", true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutPropertyTableName + " add foreign key (column_id) references layout_column (layout_column_id)" );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutPropertyTableName, "metadata_nodetypepropid", "The nodetypepropid to which this property relates", true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutPropertyTableName + " add foreign key (metadata_nodetypepropid) references nodetype_props (nodetypepropid)" );
                _CswNbtSchemaModTrnsctn.addNumberColumn( LayoutPropertyTableName, "design_nodetypeprop_nodeid", "The nodeid of the design nodetypeprop node to which this property relates", true );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table " + LayoutPropertyTableName + " add foreign key (design_nodetypeprop_nodeid) references nodes (nodeid)" );
            }
        }

    }//class CswUpdateDDL_02K_Case31783
}//namespace ChemSW.Nbt.Schema



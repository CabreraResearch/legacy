using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31783B : CswUpdateSchemaTo
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
            get { return "Migrate Layout Data - Layout Column and Layout Property, Pass 1"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswTableUpdate LayoutColumnUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "layout_column_update", "layout_column" );
            DataTable LayoutColumnTable = LayoutColumnUpdate.getEmptyTable();
            CswTableUpdate LayoutPropsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "layout_property_update", "layout_property" );
            DataTable LayoutPropsTable = LayoutPropsUpdate.getEmptyTable();
            CswArbitrarySelect OldLayoutPropsSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "layout_column_update",
@"with layouttabs as
(select lt.*,
  case lt.layout_type
    when 'edit' then lt.tab_name
    else lt.layout_type
  end as uniquename
  from layout_tab lt),
oldlayouttabs as
(select 
  l.layouttype, 
  t.tabname, 
  l.tabgroup, 
  t.taborder,
  l.display_row, 
  l.display_column,
  l.nodetypeid, 
  l.nodetypepropid, 
  l.nodetypetabsetid, 
  case l.layouttype
    when 'Add' then 'add'
    when 'Preview' then 'preview'
    when 'Table' then 'search'
    else t.tabname
  end as tabtype
from nodetype_layout l
left join nodetype_tabset t on l.nodetypetabsetid = t.nodetypetabsetid)
select 
  l.layouttype, 
  l.tabname, 
  l.tabgroup, 
  l.taborder,
  l.display_row, 
  l.display_column,
  n2.nodename nodetype_propname,
  l.nodetypeid, 
  n.nodeid design_nodetype_nodeid,
  l.nodetypepropid, 
  n2.nodeid design_nodetypeprop_nodeid,
  l.nodetypetabsetid, 
  n3.nodeid design_nodetypetab_nodeid,
  lt.layout_tab_id,
  lt.metadata_nodetypeid,
  lt.design_nodetypetab_nodeid,
  lt.uniquename,
  l.tabtype
from oldlayouttabs l
left join nodes n on n.relationalid = l.nodetypeid and n.relationaltable = 'nodetypes'
left join nodes n2 on n2.relationalid = l.nodetypepropid and n2.relationaltable = 'nodetype_props'
left join nodes n3 on n3.relationalid = l.nodetypetabsetid and n3.relationaltable = 'nodetype_tabset'
left join layouttabs lt on 
      lt.layout_type = lower(decode(l.layouttype, 'Table', 'search', l.layouttype)) 
      and lt.metadata_nodetypeid = l.nodetypeid 
where lt.uniquename = l.tabtype
order by l.nodetypeid asc, l.layouttype asc, l.tabgroup desc, l.tabname, l.display_column asc, l.display_row asc" );
            //DataTable OldLayoutPropsTable = OldLayoutPropsSelect.getTable();
            //foreach( DataRow OldLayoutPropRow in OldLayoutPropsTable.Rows )
            //{
            //    DataRow LayoutPropRow = LayoutPropsTable.NewRow();
            //    DataRow LayoutColumnRow = LayoutTabTable.NewRow();
            //    LayoutColumnRow["layout_type"] = OldLayoutRow["layouttype"].ToString().ToLower();
            //    if( LayoutColumnRow["layout_type"].ToString() == "table" )
            //    {
            //        LayoutColumnRow["layout_type"] = "search";
            //    }
            //    LayoutColumnRow["tab_type"] = "main";
            //    if( OldLayoutRow["tabname"].ToString() == "Identity" )
            //    {
            //        LayoutColumnRow["tab_type"] = "identity";
            //    }
            //    LayoutColumnRow["tab_name"] = OldLayoutRow["tabname"];
            //    LayoutColumnRow["tab_order"] = OldLayoutRow["taborder"];
            //    if( LayoutColumnRow["tab_order"].ToString() == "" )
            //    {
            //        LayoutColumnRow["tab_order"] = 1;
            //    }
            //    LayoutColumnRow["metadata_nodetypeid"] = OldLayoutRow["nodetypeid"];
            //    LayoutColumnRow["design_nodetype_nodeid"] = OldLayoutRow["design_nodetype_nodeid"];
            //    LayoutColumnRow["design_nodetypetab_nodeid"] = OldLayoutRow["design_nodetypetab_nodeid"];
            //    LayoutColumnTable.Rows.Add( LayoutColumnRow );
            //}
            //LayoutColumnUpdate.update( LayoutColumnTable );
            //LayoutPropsUpdate.update( LayoutPropsTable );

        } // update()

    } // class CswUpdateSchema_02K_Case31783
} // namespace
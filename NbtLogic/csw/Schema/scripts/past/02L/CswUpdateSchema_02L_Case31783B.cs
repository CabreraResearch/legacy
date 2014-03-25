using System;
using System.Data;
using ChemSW.Core;
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
            string SQLQuery = @"with layouttabs as
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
  l.display_column,
  ntp.propname,
  l.nodetypepropid, 
  n2.nodeid design_nodetypeprop_nodeid,
  lt.layout_tab_id
from oldlayouttabs l
join nodetype_props ntp on l.nodetypepropid = ntp.nodetypepropid
left join nodes n2 on n2.relationalid = l.nodetypepropid and n2.relationaltable = 'nodetype_props'
left join nodes n3 on n3.relationalid = l.nodetypetabsetid and n3.relationaltable = 'nodetype_tabset'
left join layouttabs lt on 
      lt.layout_type = lower(decode(l.layouttype, 'Table', 'search', l.layouttype)) 
      and lt.metadata_nodetypeid = l.nodetypeid 
where lt.uniquename = l.tabtype
order by l.nodetypeid, l.layouttype, l.taborder, l.display_column, l.display_row";
            CswTableUpdate LayoutColumnUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "layout_column_update", "layout_column" );
            DataTable LayoutColumnTable = LayoutColumnUpdate.getEmptyTable();
            CswTableUpdate LayoutPropsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "layout_property_update", "layout_property" );
            DataTable LayoutPropsTable = LayoutPropsUpdate.getEmptyTable();
            CswArbitrarySelect OldLayoutPropsSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "old_layout_props_select", SQLQuery );
            DataTable OldLayoutPropsTable = OldLayoutPropsSelect.getTable();
            int colpk = 1;
            int rownum = 1;
            int colnum = 1;
            string layouttype = "";
            string tabname = "";
            string tabgroup = "";
            foreach( DataRow OldLayoutPropRow in OldLayoutPropsTable.Rows )
            {
                if( CswConvert.ToInt32( OldLayoutPropRow["display_column"] ) != colnum ||
                    OldLayoutPropRow["layouttype"].ToString() != layouttype ||
                    OldLayoutPropRow["tabname"].ToString() != tabname )
                {
                    DataRow LayoutColumnRow = LayoutColumnTable.NewRow();
                    LayoutColumnRow["column_order"] = OldLayoutPropRow["display_column"];
                    LayoutColumnRow["parent_id"] = OldLayoutPropRow["layout_tab_id"];
                    LayoutColumnRow["parent_type"] = "tab";
                    LayoutColumnTable.Rows.Add( LayoutColumnRow );
                    colnum = CswConvert.ToInt32( OldLayoutPropRow["display_column"] );
                    colpk = CswConvert.ToInt32( LayoutColumnRow["layout_column_id"] );
                    rownum = 1;
                    layouttype = OldLayoutPropRow["layouttype"].ToString();
                    tabname = OldLayoutPropRow["tabname"].ToString();
                }
                if( false == String.IsNullOrEmpty( OldLayoutPropRow["tabgroup"].ToString() ) &&
                    OldLayoutPropRow["tabgroup"].ToString() != tabgroup )
                {
                    DataRow LayoutColumnRow = LayoutColumnTable.NewRow();
                    LayoutColumnRow["column_name"] = OldLayoutPropRow["tabgroup"].ToString();
                    LayoutColumnRow["column_order"] = rownum;
                    LayoutColumnRow["parent_id"] = colpk;
                    LayoutColumnRow["parent_type"] = "column";
                    LayoutColumnTable.Rows.Add( LayoutColumnRow );
                    colpk = CswConvert.ToInt32( LayoutColumnRow["layout_column_id"] );
                    tabgroup = OldLayoutPropRow["tabgroup"].ToString();
                    rownum = 1;
                }
                if( String.IsNullOrEmpty( OldLayoutPropRow["tabgroup"].ToString() ) )
                {
                    DataRow LayoutPropRow = LayoutPropsTable.NewRow();
                    LayoutPropRow["prop_order"] = rownum;
                    if( OldLayoutPropRow["propname"].ToString() == "Save" )
                    {
                        LayoutPropRow["prop_order"] = Int32.MaxValue;
                    }
                    LayoutPropRow["column_id"] = colpk;
                    LayoutPropRow["metadata_nodetypepropid"] = OldLayoutPropRow["nodetypepropid"];
                    LayoutPropRow["design_nodetypeprop_nodeid"] = OldLayoutPropRow["design_nodetypeprop_nodeid"];
                    LayoutPropsTable.Rows.Add( LayoutPropRow );
                    rownum++;
                }
            }
            LayoutColumnUpdate.update( LayoutColumnTable );
            LayoutPropsUpdate.update( LayoutPropsTable );
        } // update()

    } // class CswUpdateSchema_02K_Case31783
} // namespace
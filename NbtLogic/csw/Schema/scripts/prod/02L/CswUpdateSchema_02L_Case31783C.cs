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
    public class CswUpdateSchema_02K_Case31783C : CswUpdateSchemaTo
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
            get { return "Migrate Layout Data - Conditional Properties"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
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
  l.display_row, 
  l.display_column,
  n2.nodename nodetype_propname,
  ntp.propname,
  l.nodetypeid, 
  n.nodeid design_nodetype_nodeid,
  l.nodetypepropid, 
  n2.nodeid design_nodetypeprop_nodeid,
  lt.layout_tab_id,
  lc.layout_column_id,
  ntp.filterpropid,
  ntp.filtersubfield,
  ntp.filtermode,
  ntp.filtervalue,
  lp.layout_property_id,
  lp2.layout_property_id parent_property
from oldlayouttabs l
join nodetype_props ntp on l.nodetypepropid = ntp.nodetypepropid
left join nodes n on n.relationalid = l.nodetypeid and n.relationaltable = 'nodetypes'
left join nodes n2 on n2.relationalid = l.nodetypepropid and n2.relationaltable = 'nodetype_props'
left join layouttabs lt on 
      lt.layout_type = lower(decode(l.layouttype, 'Table', 'search', l.layouttype)) 
      and lt.metadata_nodetypeid = l.nodetypeid 
left join layout_column lc on lt.layout_tab_id = lc.parent_id and lc.column_order = l.display_column
left join layout_property lp on lp.metadata_nodetypepropid = ntp.nodetypepropid and lp.column_id = lc.layout_column_id
left join layout_property lp2 on lp2.metadata_nodetypepropid = ntp.filterpropid --and lp2.column_id = lc.layout_column_id
left join layout_column lc2 on lc2.layout_column_id = lp2.column_id
left join layout_tab lt2 on lt2.layout_tab_id = lc2.parent_id
where lt.uniquename = l.tabtype
and ntp.filtersubfield is not null
and lt2.layout_type = lower(decode(l.layouttype, 'Table', 'search', l.layouttype))
order by l.nodetypeid, l.layouttype, l.taborder, ntp.filtervalue, l.display_column, l.display_row";
            CswTableUpdate LayoutColumnUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "layout_column_update", "layout_column" );
            DataTable LayoutColumnTable = LayoutColumnUpdate.getEmptyTable();
            CswArbitrarySelect ConditionalPropsSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "conditional_props_select", SQLQuery );
            DataTable ConditionalPropsTable = ConditionalPropsSelect.getTable();
            int colpk = 1;
            int colorder = 1;
            int rownum = 1;
            int parentpropid = 1;
            string filtervalue = "";
            foreach( DataRow ConditionalPropRow in ConditionalPropsTable.Rows )
            {
                if( CswConvert.ToInt32( ConditionalPropRow["parent_property"] ) != parentpropid ||
                    ConditionalPropRow["filtervalue"].ToString() != filtervalue )
                {
                    if( CswConvert.ToInt32( ConditionalPropRow["parent_property"] ) != parentpropid )
                    {
                        colorder = 1;
                    }
                    DataRow LayoutColumnRow = LayoutColumnTable.NewRow();
                    LayoutColumnRow["column_order"] = colorder;
                    LayoutColumnRow["column_name"] = 
                        ConditionalPropRow["filtersubfield"] + " " +
                        ( String.IsNullOrEmpty( ConditionalPropRow["filtermode"].ToString() ) ? "Null" : ConditionalPropRow["filtermode"] ) + " " + 
                        ConditionalPropRow["filtervalue"];
                    LayoutColumnRow["parent_id"] = ConditionalPropRow["parent_property"];
                    LayoutColumnRow["parent_type"] = "property";
                    LayoutColumnTable.Rows.Add( LayoutColumnRow );
                    parentpropid = CswConvert.ToInt32( ConditionalPropRow["parent_property"] );
                    colpk = CswConvert.ToInt32( LayoutColumnRow["layout_column_id"] );
                    colorder++;
                    filtervalue = ConditionalPropRow["filtervalue"].ToString();
                    rownum = 1;
                }
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update layout_property set prop_order = " + rownum + 
                    ", column_id = " + colpk + " where layout_property_id = " + ConditionalPropRow["layout_property_id"] );
                rownum++;
            }
            LayoutColumnUpdate.update( LayoutColumnTable );

            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "alter table layout_property add foreign key (column_id) references layout_column (layout_column_id)" );
        } // update()

    } // class CswUpdateSchema_02K_Case31783
} // namespace
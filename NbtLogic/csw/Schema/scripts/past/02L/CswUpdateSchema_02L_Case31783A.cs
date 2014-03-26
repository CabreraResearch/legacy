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
    public class CswUpdateSchema_02K_Case31783A : CswUpdateSchemaTo
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
            get { return "Migrate Layout Data - Layout Tab"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswTableUpdate LayoutTabUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "layout_tab_update", "layout_tab" );
            DataTable LayoutTabTable = LayoutTabUpdate.getEmptyTable();
            CswArbitrarySelect OldLayoutTabsSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "layout_tab_update", 
@"select 
  l.layouttype, 
  t.tabname, 
  t.taborder,
  l.nodetypeid, 
  n.nodeid design_nodetype_nodeid,
  l.nodetypetabsetid, 
  n3.nodeid design_nodetypetab_nodeid
from nodetype_layout l
left join nodetype_tabset t on l.nodetypetabsetid = t.nodetypetabsetid
left join nodes n on n.relationalid = l.nodetypeid and n.relationaltable = 'nodetypes'
left join nodes n3 on n3.relationalid = l.nodetypetabsetid and n3.relationaltable = 'nodetype_tabset'
group by l.layouttype, t.tabname, t.taborder, l.nodetypeid, l.nodetypetabsetid, n.nodeid, n3.nodeid
order by l.nodetypeid, l.layouttype" );
            DataTable OldLayoutTabsTable = OldLayoutTabsSelect.getTable();
            foreach( DataRow OldLayoutRow in OldLayoutTabsTable.Rows )
            {
                DataRow LayoutTabRow = LayoutTabTable.NewRow();
                LayoutTabRow["layout_type"] = OldLayoutRow["layouttype"].ToString().ToLower();
                if( LayoutTabRow["layout_type"].ToString() == "table" )
                {
                    LayoutTabRow["layout_type"] = "search";
                }
                LayoutTabRow["tab_type"] = "main";
                if( OldLayoutRow["tabname"].ToString() == "Identity" )
                {
                    LayoutTabRow["tab_type"] = "identity";
                }
                LayoutTabRow["tab_name"] = OldLayoutRow["tabname"];
                LayoutTabRow["tab_order"] = OldLayoutRow["taborder"];
                if( LayoutTabRow["tab_order"].ToString() == "" )
                {
                    LayoutTabRow["tab_order"] = 1;
                }
                LayoutTabRow["metadata_nodetypeid"] = OldLayoutRow["nodetypeid"];
                LayoutTabRow["design_nodetype_nodeid"] = OldLayoutRow["design_nodetype_nodeid"];
                LayoutTabRow["design_nodetypetab_nodeid"] = OldLayoutRow["design_nodetypetab_nodeid"];
                LayoutTabTable.Rows.Add( LayoutTabRow );
            }
            LayoutTabUpdate.update( LayoutTabTable );
        } // update()

    } // class CswUpdateSchema_02K_Case31783
} // namespace
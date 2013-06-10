using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29797
    /// </summary>
    public class CswUpdateSchema_02B_Case29797 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29797; }
        }

        public override void update()
        {
            string SQL = @"select jctnodepropid
                             from (select j1.nodeid, j1.nodetypepropid, max(j1.jctnodepropid) jctnodepropid
                                     from jct_nodes_props j1
                                     join jct_nodes_props j2 on (j1.nodetypepropid = j2.nodetypepropid
                                                                 and j1.nodeid = j2.nodeid 
                                                                 and j1.jctnodepropid <> j2.jctnodepropid)
                                    group by j1.nodeid, j1.nodetypepropid)";

            CswArbitrarySelect DupeSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "29797_select", SQL );
            DataTable DupeTable = DupeSelect.getTable();
            if( DupeTable.Rows.Count > 0 )
            {
                CswCommaDelimitedString JctIds = new CswCommaDelimitedString();
                foreach( DataRow DupeRow in DupeTable.Rows )
                {
                    JctIds.Add( DupeRow["jctnodepropid"].ToString() );
                }

                CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29797_update", "jct_nodes_props" );
                DataTable JctTable = JctUpdate.getTable( "where jctnodepropid in (" + JctIds.ToString() + ")" );
                foreach( DataRow JctRow in JctTable.Rows )
                {
                    JctRow.Delete();
                }
                JctUpdate.update( JctTable );
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29797

}//namespace ChemSW.Nbt.Schema
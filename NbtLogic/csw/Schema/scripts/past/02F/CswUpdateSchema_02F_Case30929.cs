using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30929: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30929; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30929"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp ViewSDSOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.ViewSDS );
            CswNbtMetaDataObjectClassProp RequestOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.Request );

            CswCommaDelimitedString ViewSDSNTPIds = new CswCommaDelimitedString();
            CswCommaDelimitedString RequestNTPIds = new CswCommaDelimitedString();

            foreach( CswNbtMetaDataNodeTypeProp ViewSDSNTP in ViewSDSOCP.getNodeTypeProps() )
            {
                ViewSDSNTPIds.Add( ViewSDSNTP.PropId.ToString() );
            }

            foreach( CswNbtMetaDataNodeTypeProp RequestNTP in RequestOCP.getNodeTypeProps() )
            {
                RequestNTPIds.Add( RequestNTP.PropId.ToString() );
            }

            string sql = @"select j.nodeid, j.nodetypepropid, count(nodeid) as cnt from jct_nodes_props j
                            where j.nodetypepropid in (" + ViewSDSNTPIds.ToString() + ") or j.nodetypepropid in (" + RequestNTPIds.ToString() + ") " +
                         @"group by j.nodeid, j.nodetypepropid
                            having count(nodeid) > 1";
            CswArbitrarySelect arbSel = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "GetDupRowsNodeIds", sql );
            CswCommaDelimitedString NodeIds = new CswCommaDelimitedString();
            DataTable dupTbl = arbSel.getTable();
            foreach( DataRow row in dupTbl.Rows )
            {
                NodeIds.Add( row["nodeid"].ToString() );
            }

            _deleteDuplicateRows( NodeIds, ViewSDSNTPIds );
            _deleteDuplicateRows( NodeIds, RequestNTPIds );


        } // update()

        private void _deleteDuplicateRows( CswCommaDelimitedString NodeIds, CswCommaDelimitedString NTPIds )
        {
            foreach( string id in NodeIds )
            {
                string where = "where nodeid = " + id + " and nodetypepropid in (" + NTPIds.ToString() + ")";
                CswTableUpdate jnpTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "RemoveDups" + id, "jct_nodes_props" );
                DataTable tbl = jnpTU.getTable( where );
                if( tbl.Rows.Count > 1 )
                {
                    bool skip = true;
                    foreach( DataRow row in tbl.Rows )
                    {
                        if( skip ) //leave one row for the prop
                        {
                            skip = false;
                        }
                        else
                        {
                            row.Delete();
                        }
                    }
                    jnpTU.update( tbl );
                }
            }
        }

    }

}//namespace ChemSW.Nbt.Schema
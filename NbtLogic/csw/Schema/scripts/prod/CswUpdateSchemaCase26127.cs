using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26127
    /// </summary>
    public class CswUpdateSchemaCase26127 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswTableUpdate ReportsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "26127_reports", "jct_modules_nodetypes" );
            string WhereClause = @"where jctmodulenodetypeid in 
                (
                    select jctmodulenodetypeid from jct_modules_nodetypes j
                    join nodetypes t on t.nodetypeid = j.nodetypeid
                    join modules m on j.moduleid = m.moduleid
                    where t.nodetypename = 'Report'
                )";
            DataTable JctModulesNodeTypesTable = ReportsUpdate.getTable( WhereClause );
            foreach( DataRow ReportsRow in JctModulesNodeTypesTable.Rows )
            {
                ReportsRow.Delete();
            }
            ReportsUpdate.update( JctModulesNodeTypesTable );
        }//Update()

    }//class CswUpdateSchemaCase26127

}//namespace ChemSW.Nbt.Schema
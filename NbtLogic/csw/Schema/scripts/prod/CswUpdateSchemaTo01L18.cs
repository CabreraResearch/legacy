using System;
using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-18
    /// </summary>
    public class CswUpdateSchemaTo01L18 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 18 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            #region Case 24786

            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_jct_nodes_props_update", "jct_nodes_props" );
            string UpdateWhere = @"where jctnodepropid in ( 
                                        select n.defaultvalueid from nodetype_props n 
                                        join field_types f on n.fieldtypeid=f.fieldtypeid
                                        where lower(f.fieldtype)='question' 
                                  )";
            DataTable UpdateTable = TableUpdate.getTable( UpdateWhere, false );

            foreach( DataRow Row in UpdateTable.Rows )
            {
                Row["field1_date"] = DBNull.Value;
            }
            TableUpdate.update( UpdateTable );


            #endregion Case 24786

        }//Update()

    }//class CswUpdateSchemaTo01L18

}//namespace ChemSW.Nbt.Schema



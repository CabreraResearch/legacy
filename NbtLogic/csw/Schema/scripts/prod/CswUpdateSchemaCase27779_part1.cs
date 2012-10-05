using ChemSW.DB;
using ChemSW.Core;
using System;
using System.Data;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27779_part1
    /// </summary>
    public class CswUpdateSchemaCase27779_part1 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            //for each row where length is not null, copy the value into the attribute1 column - this column is replacing length
            CswTableUpdate nodetype_propsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "removeLength_27779", "nodetype_props" );
            CswCommaDelimitedString selectCols = new CswCommaDelimitedString();
            selectCols.FromString( "length, attribute1" );

            DataTable nodetype_props = nodetype_propsTU.getTable(
                SelectColumns: selectCols,
                FilterColumn: "",
                FilterValue: Int32.MinValue,
                WhereClause: "where length is not null",
                RequireOneRow: false );
            foreach( DataRow row in nodetype_props.Rows ) //copy the value of length into new attribute 1 to preserve the data
            {
                row["attribute1"] = row["length"].ToString();
                row["length"] = DBNull.Value;
            }
            nodetype_propsTU.update( nodetype_props );

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema
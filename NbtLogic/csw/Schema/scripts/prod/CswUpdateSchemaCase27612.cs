using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchemaCase27612 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "retrieve-rule-pk", "scheduledrules" );
            Int32 UpdatePropValsPk = Int32.MinValue;
            DataTable DataTableSelect = CswTableSelect.getTable( @"where lower( rulename ) = 'updtpropvals'" );
            if( 1 == DataTableSelect.Rows.Count )
            {
                UpdatePropValsPk = CswConvert.ToInt32( DataTableSelect.Rows[0]["scheduledruleid"] );
            }


            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "Add-nodes-per-cycle", "scheduledruleparams" );
            DataTable DataTableUpdate = CswTableUpdate.getEmptyTable();
            DataRow NewRow = DataTableUpdate.NewRow();
            NewRow["scheduledruleid"] = UpdatePropValsPk;
            NewRow["paramname"] = "NodesPerCycle";
            NewRow["paramval"] = 25;
            DataTableUpdate.Rows.Add( NewRow );
            CswTableUpdate.update( DataTableUpdate );

        }//Update()

    }//class CswUpdateSchemaCase27612

}//namespace ChemSW.Nbt.Schema
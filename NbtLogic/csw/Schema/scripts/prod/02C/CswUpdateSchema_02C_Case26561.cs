using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02C_Case26561 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 26561; }
        }

        public override void update()
        {
            CswTableUpdate CswTableUpdateAction = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update_for_" + CaseNo.ToString(), "actions" );
            DataTable ActionsTable = CswTableUpdateAction.getTable( "where actionname='Assign Inventory Groups'" );
            if( 1 == ActionsTable.Rows.Count )
            {
                ActionsTable.Rows[0]["actionname"] = "Manage Locations";
            }

            CswTableUpdateAction.update( ActionsTable );


        } // update()

    }//class CswUpdateSchema_02B_CaseXXXXX

}//namespace ChemSW.Nbt.Schema
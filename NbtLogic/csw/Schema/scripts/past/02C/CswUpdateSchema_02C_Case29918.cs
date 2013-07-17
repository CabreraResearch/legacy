using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02C_Case29918 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 29918; }
        }

        public override void update()
        {
            CswTableUpdate CswTableUpdateActions = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update icon case 29918", "actions" );
            DataTable ActionsTable = CswTableUpdateActions.getTable( " where lower(actionname) = 'manage locations' " );
            if( 1 == ActionsTable.Rows.Count )
            {
                ActionsTable.Rows[0]["iconfilename"] = "world.png";
                CswTableUpdateActions.update( ActionsTable );
            }
        } // update()

    }//class CswUpdateSchema_02C_Case29918

}//namespace ChemSW.Nbt.Schema
using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26428
    /// </summary>
    public class CswUpdateSchemaCase26428 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswTableUpdate SessionListUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "26428_sessionlist_delete", "sessionlist" );
            DataTable SessionTable = SessionListUpdate.getTable();
            foreach( DataRow DataRow in SessionTable.Rows )
            {
                DataRow.Delete();
            }
            SessionListUpdate.update( SessionTable );
        }//Update()

    }//class CswUpdateSchemaCase26428

}//namespace ChemSW.Nbt.Schema
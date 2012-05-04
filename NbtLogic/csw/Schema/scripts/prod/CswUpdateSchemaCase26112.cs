using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchemaCase26112 : CswUpdateSchemaTo
    {
        public override void update()
        {

            string FailedRuleName = "scheduledrules";
            CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "scheduledrulesquery", FailedRuleName );
            DataTable DataTableSelect = CswTableSelect.getTable( " where lower(rulename)='selffailed'" );
            if( DataTableSelect.Rows.Count < 1 )
            {
                throw ( new CswDniException( "There is no scheduled rule named" + FailedRuleName ) );
            }



            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "addtestruleparams", "scheduledruleparams" );
            DataTable DataTableUpdate = CswTableUpdate.getEmptyTable();
            DataRow NewRow = DataTableUpdate.NewRow();
            NewRow["scheduledruleid"] = DataTableSelect.Rows[0]["scheduledruleid"];
            NewRow["paramname"] = "arbitrary_status_message";
            NewRow["paramval"] = "Deliberately failed for test purposes"; ;
            DataTableUpdate.Rows.Add( NewRow );
            CswTableUpdate.update( DataTableUpdate );


        }//Update()

    }//class CswUpdateSchemaCase26112

}//namespace ChemSW.Nbt.Schema
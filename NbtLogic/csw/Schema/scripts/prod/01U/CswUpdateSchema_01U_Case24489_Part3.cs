using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24489 part 3
    /// </summary>
    public class CswUpdateSchema_01U_Case24489_Part3 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "expiredContainerSchedRule_28247", "scheduledrules" );
            DataTable ScheduledRules = TableUpdate.getTable();
            DataRow ReconciliationRow = ScheduledRules.NewRow();
            ReconciliationRow["rulename"] = "Reconciliation";
            ReconciliationRow["recurrence"] = "Daily";
            ReconciliationRow["interval"] = "1";
            ReconciliationRow["disabled"] = CswConvert.ToDbVal( false );
            ReconciliationRow["reprobatethreshold"] = "3";
            ReconciliationRow["maxruntimems"] = "600000";
            ScheduledRules.Rows.Add( ReconciliationRow );
            TableUpdate.update( ScheduledRules );
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 24489; }
        }

    }//class CswUpdateSchema_01U_Case24489_Part3

}//namespace ChemSW.Nbt.Schema
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30246 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30246; }
        }

        public override void update()
        {
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "reconciliationSchedRule_30246", "scheduledrules" );
            DataTable ScheduledRules = TableUpdate.getTable( "where rulename = '" + CswEnumNbtScheduleRuleNames.Reconciliation + "'" );
            if( ScheduledRules.Rows.Count > 0 )
            {
                DataRow ReconciliationDataRow = ScheduledRules.Rows[0];
                ReconciliationDataRow["recurrence"] = CswEnumRecurrence.NMinutes;
                ReconciliationDataRow["interval"] = "15";
                TableUpdate.update( ScheduledRules );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
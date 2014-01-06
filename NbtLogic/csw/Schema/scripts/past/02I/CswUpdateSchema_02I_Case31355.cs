using System.Data;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31355 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31355; }
        }

        public override string Title
        {
            get { return "Set ValidateAccessIds rule to daily"; }
        }

        public override void update()
        {
            // Change the schedule of ValidateAccessIds from DayOfWeek=2 to Daily

            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "schema31355_scheduledrules_update", "scheduledrules" );
            DataTable ScheduledRulesTable = TableUpdate.getTable( "where rulename = '" + CswEnumNbtScheduleRuleNames.ValidateAccessIds + "'" );
            if( ScheduledRulesTable.Rows.Count > 0 )
            {
                DataRow row = ScheduledRulesTable.Rows[0];
                row["recurrence"] = CswEnumRecurrence.Daily;
                row["interval"] = "1";
                TableUpdate.update( ScheduledRulesTable );
            }
        } // update()
    }

}//namespace ChemSW.Nbt.Schema
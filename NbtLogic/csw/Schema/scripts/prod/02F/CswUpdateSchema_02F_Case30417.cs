using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30417 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30417; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30417"; }
        }

        public override void update()
        {

            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "validateAccessIdsSchedRule_30417", "scheduledrules" );
            DataTable ScheduledRules = TableUpdate.getTable();
            DataRow ValidateAccessIdsRow = ScheduledRules.NewRow();
            ValidateAccessIdsRow["rulename"] = CswEnumNbtScheduleRuleNames.ValidateAccessIds;
            ValidateAccessIdsRow["recurrence"] = CswEnumRecurrence.DayOfWeek;
            ValidateAccessIdsRow["interval"] = "2"; //Monday
            ValidateAccessIdsRow["disabled"] = CswConvert.ToDbVal( true );
            ValidateAccessIdsRow["reprobatethreshold"] = "3";
            ValidateAccessIdsRow["maxruntimems"] = "300000";
            ScheduledRules.Rows.Add( ValidateAccessIdsRow );
            TableUpdate.update( ScheduledRules );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema
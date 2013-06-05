using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29859
    /// </summary>
    public class CswUpdateSchema_02C_Case29859 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "expiredContainerSchedRule_29859", "scheduledrules" );
            DataTable ScheduledRules = TableUpdate.getTable();
            DataRow PurgeSessionDataRow = ScheduledRules.NewRow();
            PurgeSessionDataRow["rulename"] = CswEnumNbtScheduleRuleNames.PurgeSessionData;
            PurgeSessionDataRow["recurrence"] = CswEnumRecurrence.NHours;
            PurgeSessionDataRow["interval"] = "1";
            PurgeSessionDataRow["disabled"] = CswConvert.ToDbVal( false );
            PurgeSessionDataRow["reprobatethreshold"] = "3";
            PurgeSessionDataRow["maxruntimems"] = "30000";
            ScheduledRules.Rows.Add( PurgeSessionDataRow );
            TableUpdate.update( ScheduledRules );

        } // update()

    }//class CswUpdateSchema_02B_Case29859

}//namespace ChemSW.Nbt.Schema
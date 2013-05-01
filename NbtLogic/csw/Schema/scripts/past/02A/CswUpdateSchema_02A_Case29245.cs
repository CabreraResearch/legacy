using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29245
    /// </summary>
    public class CswUpdateSchema_02A_Case29245 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29245; }
        }

        public override void update()
        {
            // Add ExtChemDataSync Schedule Rule to the ScheduledRules Table
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "extChemDataSyncSchedRule_29245", "scheduledrules" );
            DataTable ScheduledRules = TableUpdate.getTable();
            DataRow ExtChemDataSyncRow = ScheduledRules.NewRow();
            ExtChemDataSyncRow["rulename"] = CswEnumNbtScheduleRuleNames.ExtChemDataSync;
            ExtChemDataSyncRow["recurrence"] = "NSeconds";
            ExtChemDataSyncRow["interval"] = "60";
            ExtChemDataSyncRow["disabled"] = CswConvert.ToDbVal( false );
            ExtChemDataSyncRow["reprobatethreshold"] = "3";
            ExtChemDataSyncRow["maxruntimems"] = "600000";
            ScheduledRules.Rows.Add( ExtChemDataSyncRow );
            TableUpdate.update( ScheduledRules );
        } // update()

    }//class CswUpdateSchema_02A_Case29245

}//namespace ChemSW.Nbt.Schema
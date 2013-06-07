using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29512
    /// </summary>
    public class CswUpdateSchema_02B_Case29512 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29512; }
        }

        public override void update()
        {
            // Update the recurence of the ExtChemDataSync rule
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "extChemDataSyncSchedRule_29512", "scheduledrules" );
            DataTable ScheduleRules = TableUpdate.getTable( "where rulename='" + CswEnumNbtScheduleRuleNames.ExtChemDataSync + "'" );
            if( ScheduleRules.Rows.Count > 0 )
            {
                ScheduleRules.Rows[0]["recurrence"] = "Daily";
                ScheduleRules.Rows[0]["interval"] = "1";
                TableUpdate.update( ScheduleRules );
            }

        } // update()

    }//class CswUpdateSchema_02B_Case29512

}//namespace ChemSW.Nbt.Schema
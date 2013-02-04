using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28247
    /// </summary>
    public class CswUpdateSchema_01V_Case28247 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28247; }
        }

        public override void update()
        {
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "expiredContainerSchedRule_28247", "scheduledrules" );
            DataTable ScheduledRules = TableUpdate.getTable();
            DataRow TierIIRow = ScheduledRules.NewRow();
            TierIIRow["rulename"] = "TierII";
            TierIIRow["recurrence"] = "Daily";
            TierIIRow["interval"] = "1";
            TierIIRow["disabled"] = CswConvert.ToDbVal( false );
            TierIIRow["reprobatethreshold"] = "3";
            TierIIRow["maxruntimems"] = "3600000";
            ScheduledRules.Rows.Add( TierIIRow );
            TableUpdate.update( ScheduledRules );
        }

        //Update()

    }//class CswUpdateSchemaCase_01V_28247

}//namespace ChemSW.Nbt.Schema
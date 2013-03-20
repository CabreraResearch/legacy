using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;
using ChemSW.MtSched.Core;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Data;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28122
    /// </summary>
    public class CswUpdateSchema_01V_Case28122 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28122; }
        }

        public override void update()
        {
            //add the CAF Import Sched Service to the DB
            int ruleId = _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.CAFImport, Recurrence.NSeconds, 180 );
            CswTableUpdate tu = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "setCAFImport_time", "scheduledrules" );
            DataTable scheduledRulesDT = tu.getTable( "where scheduledruleid = " + ruleId );
            foreach( DataRow row in scheduledRulesDT.Rows )
            {
                row["maxruntimems"] = CswConvert.ToDbVal( 3600000 );
                row["disabled"] = "1";
            }
            tu.update( scheduledRulesDT );

        } //Update()

    }//class CswUpdateSchema_01V_Case28122

}//namespace ChemSW.Nbt.Schema
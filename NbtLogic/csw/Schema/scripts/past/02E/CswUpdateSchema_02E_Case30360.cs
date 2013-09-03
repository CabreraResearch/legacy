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
    public class CswUpdateSchema_02E_Case30360: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30360; }
        }

        public override void update()
        {

            CswTableUpdate schedRulesTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "SchemaUpdate.UpdateNodeCountInternval", "scheduledrules" );
            DataTable schedRulesDT = schedRulesTU.getTable( "where rulename = '" + CswEnumNbtScheduleRuleNames.NodeCounts + "'" );
            foreach( DataRow row in schedRulesDT.Rows )
            {
                row["interval"] = CswConvert.ToDbVal( 15 );
                row["recurrence"] = CswConvert.ToDbVal( CswEnumRecurrence.NMinutes );
            }
            schedRulesTU.update( schedRulesDT );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema
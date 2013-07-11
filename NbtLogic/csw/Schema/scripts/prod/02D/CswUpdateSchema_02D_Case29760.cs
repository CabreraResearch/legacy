using ChemSW.MtSched.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29760
    /// </summary>
    public class CswUpdateSchema_02D_Case29760: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29760; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.createScheduledRule( CswEnumNbtScheduleRuleNames.NodeCounts, CswEnumRecurrence.NHours, 1 );

        } // update()

    }//class CswUpdateSchema_02C_Case29760

}//namespace ChemSW.Nbt.Schema
using ChemSW.Nbt.csw.Dev;
using ChemSW.StructureSearch;
using ChemSW.DB;
using ChemSW.Nbt.Sched;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24524
    /// </summary>
    public class CswUpdateSchema_01U_Case24524 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 24524; }
        }

        public override void update()
        {
            #region Fingerprinting scheduled task

            _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.MolFingerprints, Recurrence.Daily, 1 );

            #endregion
        }

        //Update()

    }//class CswUpdateSchemaCase24524

}//namespace ChemSW.Nbt.Schema
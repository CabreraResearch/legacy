using ChemSW.MtSched.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_ScheduledRuleImport : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30041_ScheduledRuleImport"; }
        }

        public override void update()
        {
            // Scheduled rule for CAFImports
            _CswNbtSchemaModTrnsctn.createScheduledRule( CswEnumNbtScheduleRuleNames.CAFImport, CswEnumRecurrence.NHours, 1 );

        } // update()

    } // class CswUpdateSchema_02F_Case30041_ScheduledRuleImport

}//namespace ChemSW.Nbt.Schema
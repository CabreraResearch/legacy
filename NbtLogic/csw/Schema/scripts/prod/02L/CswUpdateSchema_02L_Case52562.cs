using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52562: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52562; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override string Title
        {
            get { return "Create NodeUpdateEvents Scheduled Rule"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createScheduledRule( CswEnumNbtScheduleRuleNames.NodeUpdateEvents, CswEnumRecurrence.NMinutes, 15 );
        } // update()
    }
}//namespace ChemSW.Nbt.Schema
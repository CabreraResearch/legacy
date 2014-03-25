using ChemSW.MtSched.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52280E : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 52280; }
        }

        public override string Title
        {
            get { return "Create SetMaterialObsolete schedule rule"; }
        }

        public override string AppendToScriptName()
        {
            return "E_V2";
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createScheduledRule( CswEnumNbtScheduleRuleNames.SetMaterialObsolete, CswEnumRecurrence.Daily, 1 );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case31231 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31231; }
        }

        public override string AppendToScriptName()
        {
            return "02H_Case" + CaseNo;
        }

        public override string Title
        {
            get { return "lock_inspection_answer config vbl"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswEnumNbtConfigurationVariables.lock_inspection_answer, "If 1, prevent editing answers once an inspection is Action Required", "0", false );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
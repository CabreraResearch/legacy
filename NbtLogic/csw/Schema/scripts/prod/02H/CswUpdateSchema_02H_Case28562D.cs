using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case28562D : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28562; }
        }

        public override string AppendToScriptName()
        {
            return "D";
        }

        public override string Title
        {
            get { return "Remove old HMIS action"; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.deleteAction( "HMIS_Reporting" );

        } // update()
    }

}//namespace ChemSW.Nbt.Schema
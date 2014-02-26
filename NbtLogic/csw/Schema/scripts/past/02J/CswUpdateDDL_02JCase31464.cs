using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02I_Case31464 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31464; }
        }

        public override string Title
        {
            get { return "Add 'Hidden' column to modules table"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "modules", "hidden" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "modules", "hidden", "", true );
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema
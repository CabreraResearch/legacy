using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02I_Case31061A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31061; }
        }

        public override string Title
        {
            get { return "Add new col to jct_nodes_props"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "jct_nodes_props", "field1_big" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "jct_nodes_props", "field1_big", "String column to store very long strings (like URLs)", false, 4000 );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema
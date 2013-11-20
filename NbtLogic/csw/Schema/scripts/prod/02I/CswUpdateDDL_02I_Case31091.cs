using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02I_Case31091: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 31091; }
        }

        public override string Title
        {
            get { return "Add lobdatapkcolname col to import_def_bindings"; }
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "import_def_bindings", "lobdatapkcolname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "import_def_bindings", "lobdatapkcolname", "The name of the PK column for LOB imports", false, 255 );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "import_def_bindings", "legacypropid" ) )
            {
                _CswNbtSchemaModTrnsctn.addNumberColumn( "import_def_bindings", "legacypropid", "The property id for the CAF property", false );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema
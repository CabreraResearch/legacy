using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02G_Case30771 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30771; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30771"; }
        }

        public override string Title
        {
            get { return "Reorganize CAF Import Columns"; }
        }

        public override void update()
        {

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( "import_def" ) )
                _CswNbtSchemaModTrnsctn.addTable( "import_def", "importdefid" );

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF", "TABLENAME" ) )
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF", "TABLENAME", "the source table for this sheet", false, false, 30 );
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF", "VIEWNAME" ) )
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF", "VIEWNAME", "the source view for this sheet", false, false, 30 );
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF", "PKCOLUMNNAME" ) )
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF", "PKCOLUMNNAME", "the column of PKs stored in import queue", false, false, 30 );



        } // update()

    }

}//namespace ChemSW.Nbt.Schema
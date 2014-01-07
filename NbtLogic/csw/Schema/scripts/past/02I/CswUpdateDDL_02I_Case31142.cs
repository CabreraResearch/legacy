using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02I_Case31142 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31142; }
        }

        public override string Title
        {
            get { return "Move Table Columns from IMPORT_DEF to IMPORT_DEF_ORDER"; }
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF_ORDER", "TABLENAME" ) )
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF_ORDER", "TABLENAME", "the CAF table for this nodetype", false, 30 );
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF_ORDER", "VIEWNAME" ) )
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF_ORDER", "VIEWNAME", "the CAF view for this nodetype", false, 30 );
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF_ORDER", "PKCOLUMNNAME" ) )
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF_ORDER", "PKCOLUMNNAME", "the column of PKs stored in CAF import queue", false, 30 );

            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF", "TABLENAME" ) )
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update import_def_order io 
    set tablename = (select tablename from import_def id where io.importdefid = id.importdefid),
        viewname = (select viewname from import_def id where io.importdefid = id.importdefid),
        pkcolumnname = (select pkcolumnname from import_def id where io.importdefid = id.importdefid)" );

                
                _CswNbtSchemaModTrnsctn.dropColumn( "IMPORT_DEF", "TABLENAME" );
                _CswNbtSchemaModTrnsctn.dropColumn( "IMPORT_DEF", "VIEWNAME" );
                _CswNbtSchemaModTrnsctn.dropColumn( "IMPORT_DEF", "PKCOLUMNNAME" );
            }


        } // update()

    }

}//namespace ChemSW.Nbt.Schema
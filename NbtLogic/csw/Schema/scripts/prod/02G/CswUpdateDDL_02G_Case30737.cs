using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for MetaData changes
    /// </summary>
    public class CswUpdateDDL_02G_Case30737 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Add Import_Lob Table"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30737; }
        }

        public override void update()
        {

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( "Import_Lob" ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( "Import_Lob", "importlobid" );

                _CswNbtSchemaModTrnsctn.addBlobColumn( "Import_Lob", "BlobData", "Blob data from CAF", false );

                _CswNbtSchemaModTrnsctn.addClobColumn( "Import_Lob", "ClobData", "Clob data from CAF", false );

                _CswNbtSchemaModTrnsctn.addStringColumn( "Import_Lob", "tablename", "The name of the table this lob came from", true, 100 );

                _CswNbtSchemaModTrnsctn.addNumberColumn( "Import_Lob", "cafpk", "The PK of the row this lob came from", true );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "import_def_bindings", "blobtablename" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "import_def_bindings", "blobtablename", "The name of the table this blob came from", false, 255 );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "import_def_bindings", "clobtablename" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "import_def_bindings", "clobtablename", "The name of the table this clob came from", false, 255 );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "import_def_bindings", "lobdatapkcoloverride" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "import_def_bindings", "lobdatapkcoloverride", "The name of the column that contains the value to use as the PK to the lob data", false, 255 );
            }

        } // update()

    }
}



using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02M_CIS52735: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52735; }
        }

        public override string Title
        {
            get { return "Create mol_data table"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            const string molTblName = "mol_data";

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( molTblName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( molTblName, "moldataid" );

                _CswNbtSchemaModTrnsctn.addBlobColumn( molTblName, "orginalmol", "the orginal unmodified file the user uploaded to the mol property", true );
                _CswNbtSchemaModTrnsctn.addBlobColumn( molTblName, "ctab", "the ACCL Direct formatted mol", false );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( molTblName, "isdirectcompatible", "whether the original mol is readable by ACCL direct functions", false );
                _CswNbtSchemaModTrnsctn.addStringColumn( molTblName, "contentttype", "the type of file in originalmol (.mol or .cdx)", true, 250 );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( molTblName, "jctnodepropid", "foreign key to jct_nodes_props", true, "jct_nodes_props", "jctnodepropid" );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( molTblName, "nodeid", "foreign key to nodes", true, "nodes", "nodeid" );
            }
        }
    }
}



using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52735D: CswUpdateSchemaTo
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
            get { return "Install Direct for user; Add indexes to mol_data"; }
        }

        public override string AppendToScriptName()
        {
            return "D";
        }

        public override void update()
        {
            //this installs Direct for the user, it it safe to run over and over again
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( 
                    @"begin
                        execute c$direct90.mdlauxop.setup;
                      end;" );

            if( false == _doesTblIdxExist( "mol_data_molidx" ) )
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"create index mol_data_molidx
                                                                        on mol_data(ctab)
                                                                        indextype is c$direct90.mxixmdl" );
            }
        }

        private bool _doesTblIdxExist( string IdxName )
        {
            CswArbitrarySelect idxSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "doesIdxExist_52735", "select * from user_indexes ui where lower(ui.index_name) = '" + IdxName + "'" );
            DataTable indexes = idxSelect.getTable();
            return indexes.Rows.Count > 0;
        }
    }
}
using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02M_CIS53175 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 53175; }
        }

        public override string Title
        {
            get { return "Add legacyid column to nodes"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "legacyid" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodes", "legacyid", "Old PK from legacy system", false, 100 );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "CREATE INDEX legacy_nt_id ON nodes (nodetypeid, legacyid) TABLESPACE NBT_INDEX" );
            }
        }
    }
}



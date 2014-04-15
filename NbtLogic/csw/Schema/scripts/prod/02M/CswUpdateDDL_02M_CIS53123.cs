using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02M_CIS53123 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 53123; }
        }

        public override string Title
        {
            get { return "Add field1_numeric index to jct_nodes_props"; }
        }

        public override string AppendToScriptName()
        {
            return "IDX";
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "CREATE INDEX FIELD1_NUMERIC_NTPID ON jct_nodes_props(nodetypepropid, field1_numeric) TABLESPACE NBT_INDEX" );
        }
    }
}



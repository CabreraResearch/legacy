using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30912 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30912; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Remove bad audit records"; }
        }

        public override void update()
        {
            // 1. Delete audit trail records that are empty except for oraviewname or oraviewcolname.
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"delete from nodetypes_audit where objectclassid is null" );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"delete from nodetype_props_audit where fieldtypeid is null" );


        } // update()

    }

}//namespace ChemSW.Nbt.Schema
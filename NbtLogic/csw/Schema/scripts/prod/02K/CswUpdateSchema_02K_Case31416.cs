using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31416: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31416; }
        }

        public override string Title
        {
            get { return "Delete rogue blob data"; }
        }

        public override void update()
        {
            const string sql = @" delete from blob_data bd where not exists (select jnp.jctnodepropid from jct_nodes_props jnp where jnp.jctnodepropid = bd.jctnodepropid)";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( sql );

        } // update()
    } // class CswUpdateSchema_02K_Case31416
} // namespace
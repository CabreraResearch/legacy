using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28075
    /// </summary>
    public class CswUpdateSchema_01U_Case28075 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                @"update jct_nodes_props 
                    set gestalt = 'N' 
                    where jctnodepropid in 
                        (select jnp.jctnodepropid from jct_nodes_props jnp
                            inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                            inner join field_types ft on ntp.fieldtypeid = ft.fieldtypeid
                            where ft.fieldtype = 'Logical' and jnp.field1 = '0')"
                );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                @"update jct_nodes_props 
                    set gestalt = 'Y' 
                    where jctnodepropid in 
                        (select jnp.jctnodepropid from jct_nodes_props jnp
                            inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                            inner join field_types ft on ntp.fieldtypeid = ft.fieldtypeid
                            where ft.fieldtype = 'Logical' and jnp.field1 = '1')"
                );
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28075; }
        }

    }//class CswUpdateSchema_01U_Case28075

}//namespace ChemSW.Nbt.Schema
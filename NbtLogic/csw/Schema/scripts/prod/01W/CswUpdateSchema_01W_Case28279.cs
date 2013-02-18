using ChemSW.Nbt.csw.Dev;
using ChemSW.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01W_Case28279 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28279; }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
            foreach( string CurrentSystemUserName in CswSystemUserNames.getValues() )
            {
                string delete_props_audit_cmd = "delete from jct_nodes_props_audit where audittransactionid in (select audittransactionid from audit_transactions where transactionusername = '" + CurrentSystemUserName + "')";
                string delete_nodes_audit_cmd = "delete from nodes_audit where audittransactionid in (select audittransactionid from audit_transactions where transactionusername = '" + CurrentSystemUserName + "')";
                string delete_transactions_cmd = "delete from audit_transactions where transactionusername = '" + CurrentSystemUserName + "'";

                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( delete_props_audit_cmd );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( delete_nodes_audit_cmd );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( delete_transactions_cmd );
            }
        } //Update()

    }//class CswUpdateSchema_01V_CaseXXXXX

}//namespace ChemSW.Nbt.Schema
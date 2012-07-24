
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchemaCase26867 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update audit_transactions set transactionusername = '" + ChemSW.Nbt.Security.SystemUserNames.SysUsr_SchemaUpdt.ToString() + "' where transactionusername='_SchemaUpdaterUser'" );

        }//Update()

    }//class CswUpdateSchemaCase26867

}//namespace ChemSW.Nbt.Schema
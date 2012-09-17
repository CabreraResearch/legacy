using ChemSW.Audit;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27500
    /// </summary>
    public class CswUpdateSchemaCase27500 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update object_class set auditlevel ='" + AuditLevel.NoAudit + "'" );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update object_class_props set auditlevel ='" + AuditLevel.NoAudit + "'" );
        }//Update()

    }//class CswUpdateSchemaCase27500

}//namespace ChemSW.Nbt.Schema
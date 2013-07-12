using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29234
    /// </summary>
    public class CswUpdateSchema_02C_Case29234 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29234; }
        }

        public override void update()
        {
            CswNbtView ExpiringSDSView = _CswNbtSchemaModTrnsctn.restoreView( "SDS Expiring Next Month" );
            if( null != ExpiringSDSView )
            {
                ExpiringSDSView.Delete();
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29234

}//namespace ChemSW.Nbt.Schema
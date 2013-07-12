using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29499
    /// </summary>
    public class CswUpdateSchema_02D_Case29499B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29499; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.makeTableAuditable( "blob_data" );

        } // update()

    }//class CswUpdateSchema_02C_Case29499

}//namespace ChemSW.Nbt.Schema
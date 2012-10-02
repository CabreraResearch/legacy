
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_029_04 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_029.Purpose, "Drop test table" ) ); } }

        //private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_029 _CswTstCaseRsrc_029 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_029_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_029 = (CswTstCaseRsrc_029) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_029.ArbitraryTableName_01 );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

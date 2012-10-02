using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_017_02 : CswUpdateSchemaTo
    {

        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_017.Purpose, "add test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_017 _CswTstCaseRsrc_017 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_017_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_017 = (CswTstCaseRsrc_017) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_017.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			List<PkFkPair> PairList = _CswTstCaseRsrc_017.getPkFkPairs();
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswTstCaseRsrc.fillTableWithArbitraryData( CurrentPair.PkTableName, _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), 100 );
                _CswTstCaseRsrc.addArbitraryForeignKeyRecords( CurrentPair.PkTableName, CurrentPair.FkTableName, CurrentPair.PkTablePkColumnName, _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), _CswTstCaseRsrc.getTestNameStem( TestNameStem.TestVal ) );
            }
        
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

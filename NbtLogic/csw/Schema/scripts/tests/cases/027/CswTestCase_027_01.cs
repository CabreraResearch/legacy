
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_027_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_027.Purpose, "build test table" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_027 _CswTstCaseRsrc_027 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_027_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_027 = (CswTstCaseRsrc_027) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_027.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total Process Memory before: " + _CswTstCaseRsrc_027.TotalProcessMemory );
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total GC Memory before: " + _CswTstCaseRsrc_027.TotalGCMemorySansCollection);

            _CswTstCaseRsrc_027.memoryTestBegin();

            _CswTstCaseRsrc_027.makeArbitraryTables();

            _CswTstCaseRsrc_027.memoryTestEnd();

            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total Process Memory after:  " + _CswTstCaseRsrc_027.TotalProcessMemory);
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total GC Memory after: " + _CswTstCaseRsrc_027.TotalGCMemorySansCollection);
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": ProcessMemory Delta" + _CswTstCaseRsrc_027.ProcessMemoryDelta);
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": GCMemory Delta" + _CswTstCaseRsrc_027.GCMemoryDelta);





        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

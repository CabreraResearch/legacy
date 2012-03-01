﻿
namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_019_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_019.Purpose, "drop tables and verify drop" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_019 _CswTstCaseRsrc_019 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_019_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_019 = (CswTstCaseRsrc_019) CswTstCaseRsc;

        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_019.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswTstCaseRsrc_019.dropArbitraryTables();

            _CswTstCaseRsrc_019.verifyTablesDropped(); 



        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

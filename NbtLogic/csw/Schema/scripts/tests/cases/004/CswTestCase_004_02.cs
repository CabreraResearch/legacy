﻿using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_004_02 : CswUpdateSchemaTo
    {

        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_004.Purpose, "add test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_004 _CswTstCaseRsrc_004 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_004_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_004 = (CswTstCaseRsrc_004) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_004.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			List<PkFkPair> PairList = _CswTstCaseRsrc_004.getPkFkPairs();
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswTstCaseRsrc.fillTableWithArbitraryData( CurrentPair.PkTableName, _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), 100 );
                _CswTstCaseRsrc.addArbitraryForeignKeyRecords( CurrentPair.PkTableName, CurrentPair.FkTableName, CurrentPair.PkTablePkColumnName, _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), _CswTstCaseRsrc.getTestNameStem( TestNameStem.TestVal ) );
            }
        
        }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

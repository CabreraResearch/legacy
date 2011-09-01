using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_017_02 : CswUpdateSchemaTo
    {

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_017.Purpose, "add test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_017 _CswTstCaseRsrc_017 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_017_02( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_017 = new CswTstCaseRsrc_017( _CswNbtSchemaModTrnsctn );
			
			List<PkFkPair> PairList = _CswTstCaseRsrc_017.getPkFkPairs();
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswTstCaseRsrc.fillTableWithArbitraryData( CurrentPair.PkTableName, _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), 100 );
                _CswTstCaseRsrc.addArbitraryForeignKeyRecords( CurrentPair.PkTableName, CurrentPair.FkTableName, CurrentPair.PkTablePkColumnName, _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), _CswTstCaseRsrc.getTestNameStem( TestNameStem.TestVal ) );
            }
        
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

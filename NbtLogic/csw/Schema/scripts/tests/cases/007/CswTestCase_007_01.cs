using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_007_01 : CswUpdateSchemaTo
    {

        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_007.Purpose, "add columns" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_007 _CswTstCaseRsrc_007 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_007_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_007 = (CswTstCaseRsrc_007) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_007.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			try
            {
                _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc.getRealTestTableName(TestTableNamesReal.DataDictionary), DataDictionaryUniqueType.None, false, string.Empty );
                _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ), DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), DataDictionaryUniqueType.None, false, string.Empty );
            }

            catch ( Exception Exception )
            {
                throw ( new CswDniException( "An unexpected exception was thrown when adding two columns to data_dictionary:"  + Exception.Message ) );
            }//catch()  
        
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

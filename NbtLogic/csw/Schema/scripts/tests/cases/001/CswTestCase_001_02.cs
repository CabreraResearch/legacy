using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 02
    /// </summary>
    public class CswTestCase_001_02 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_001.Purpose, "Verify add operation and add data to columns" ) ); } }


        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrc_001 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_001_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_001 = (CswTstCaseRsrc_001) CswTstCaseRsc;
		}//ctor

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_001.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameOne + " was not created in data base " ) );

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameTwo + " was not created in data base " ) );

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameOne + " was not created in meta data " ) );


            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameTwo + " was not created in meta data " ) );


            _CswTstCaseRsrc_001.testAddColumnValues( TestColumnNamesFake.TestColumn01 );
            _CswTstCaseRsrc_001.testAddColumnValues( TestColumnNamesFake.TestColumn02 );


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

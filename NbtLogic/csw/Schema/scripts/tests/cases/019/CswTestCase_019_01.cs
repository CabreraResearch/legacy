using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_019_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_019.Purpose, "create and use test tables" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_019 _CswTstCaseRsrc_019 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_019_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_019 = (CswTstCaseRsrc_019) CswTstCaseRsc;

        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_019.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswTstCaseRsrc_019.makeArbitraryTables();
            _CswTstCaseRsrc_019.makeArbitraryTableData();

            CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Description, _CswTstCaseRsrc_019.ArbitraryTableName_01 );
            DataTable DataTable = CswTableSelect.getTable();

            if( DataTable.Rows.Count != _CswTstCaseRsrc_019.TotalTestRows )
                throw(new CswDniException("Number of rows in table did not match the number of inserted rows") );

            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                if( CurrentRow[_CswTstCaseRsrc_019.ArbitraryColumnName_01].ToString() != _CswTstCaseRsrc_019.ArbitraryColumnValue )
                {
                    throw ( new CswDniException( "A row value in the test table did not match the inserted data" ) );
                }
            }



        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

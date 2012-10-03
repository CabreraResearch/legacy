using System.Collections.ObjectModel;
using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_012_06 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_012.Purpose, "Verify row 2 value rolled back" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_012 _CswTstCaseRsrc_012 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_012_06( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_012 = (CswTstCaseRsrc_012) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_012.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
            OrderByClauses.Add( new OrderByClause( _CswTstCaseRsrc_012.FakePkColumnName, OrderByType.Ascending ) );
            CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Description, _CswTstCaseRsrc_012.FakeTestTableName );

            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_012.FakeTestTableName );
            CswTableUpdate.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTable = CswTableUpdate.getTable( "where " + _CswTstCaseRsrc_012.FakePkColumnName + "> 0", OrderByClauses );
            
            if( DataTable.Rows[0][_CswTstCaseRsrc_012.FakeValColumnName].ToString() != _CswTstCaseRsrc_012.Val_Row_1 )
                throw ( new CswDniException( "Row one does not have value " + _CswTstCaseRsrc_012.Val_Row_1 ) );

            if( DataTable.Rows[1][_CswTstCaseRsrc_012.FakeValColumnName].ToString() == _CswTstCaseRsrc_012.Val_Row_2 )
                throw ( new CswDniException( "Row two has rolled-back value " + _CswTstCaseRsrc_012.Val_Row_2 + "; it's value should be: " + _CswTstCaseRsrc_012.LocalAribtiraryValue ) );

            if( DataTable.Rows[2][_CswTstCaseRsrc_012.FakeValColumnName].ToString() != _CswTstCaseRsrc_012.Val_Row_3 )
                throw ( new CswDniException( "Row three does not have value " + _CswTstCaseRsrc_012.Val_Row_3 ) );

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

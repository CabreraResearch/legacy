using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_003_04 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_003.Purpose, "verify restoration of dropped column and its data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_003 _CswTstCaseRsrc_003 = null;
 
        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_003_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_003 = (CswTstCaseRsrc_003) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_003.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;


            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc_003.RealTestTableName, _CswTstCaseRsrc_003.RealTestColumnName ) )
                throw ( new CswDniException( "Dropped column " + _CswTstCaseRsrc_003.RealTestColumnName + " was not restored to the database after rollback" ) );

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc_003.RealTestTableName, _CswTstCaseRsrc_003.RealTestColumnName ) )
                throw ( new CswDniException( "Dropped column " + _CswTstCaseRsrc_003.RealTestColumnName + " was not restored to the meta data after rollback" ) );


            CswCommaDelimitedString SelectColsReal = new CswCommaDelimitedString();
            SelectColsReal.Add( _CswTstCaseRsrc_003.RealTestColumnName );
            CswTableSelect CswTableSelectRealTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswTestCase_003_04_010_select2", _CswTstCaseRsrc_003.RealTestTableName );
            DataTable CswTableRealTable = CswTableSelectRealTable.getTable( SelectColsReal, string.Empty, Int32.MinValue, string.Empty, false, new Collection<OrderByClause> { new OrderByClause( _CswTstCaseRsrc_003.RealTestColumnName, OrderByType.Ascending ) } );

            CswCommaDelimitedString SelectColsFake = new CswCommaDelimitedString();
            SelectColsFake.Add( _CswTstCaseRsrc_003.FakeTestColumnName );
            CswTableSelect CswTableSelectFakeTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswTestCase_003_04_010_select2", _CswTstCaseRsrc_003.FakeTestTableName );
            DataTable CswTableFakeTable = CswTableSelectFakeTable.getTable( SelectColsFake, string.Empty, Int32.MinValue, string.Empty, false, new Collection<OrderByClause> { new OrderByClause( _CswTstCaseRsrc_003.FakeTestColumnName, OrderByType.Ascending ) } );

            if( CswTableRealTable.Rows.Count != CswTableFakeTable.Rows.Count )
                throw ( new CswDniException( "number of columns in the pre-drop capture data do not match those of the post-rollback data" ) );

            for( int idx = 0; idx < CswTableRealTable.Rows.Count; idx++ )
            {
                if( CswTableRealTable.Rows[idx][_CswTstCaseRsrc_003.RealTestColumnName].ToString() != CswTableFakeTable.Rows[idx][_CswTstCaseRsrc_003.FakeTestColumnName].ToString() )
                    throw ( new CswDniException( "pre-drop capture data and post-rollback data do not agree at row idx " + idx.ToString() ) );
            }


            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_003.FakeTestTableName ); 

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

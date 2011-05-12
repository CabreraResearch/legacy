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

    public class CswTestCase_003_04 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_003.Purpose, "verify restoration of dropped column and its data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_003 _CswTstCaseRsrc_003 = null;
 
        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_003_04( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_003 =   ( CswTstCaseRsrc_003) CswTstCaseRsrc;
        }//ctor

        public void update()
        {


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

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

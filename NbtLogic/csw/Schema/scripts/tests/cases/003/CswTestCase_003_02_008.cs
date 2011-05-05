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

    public class CswTestCase_003_02_008 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 008 ); } }

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_003.Purpose, "copy data to use for post-rollback verify" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_003 _CswTstCaseRsrc_003 = null;
        public CswTestCase_003_02_008( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_003 = new CswTstCaseRsrc_003( _CswNbtSchemaModTrnsctn );
        }//ctor

        public void update()
        {

            CswTableSelect CswTableSelectRealTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswTestCase_003_02_008_select1", _CswTstCaseRsrc_003.RealTestTableName );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
            SelectCols.Add( _CswTstCaseRsrc_003.RealTestColumnName );
            DataTable DataTable = CswTableSelectRealTable.getTable( SelectCols, string.Empty, Int32.MinValue, string.Empty, false, new Collection<OrderByClause> { new OrderByClause( _CswTstCaseRsrc_003.RealTestColumnName, OrderByType.Ascending ) } );


            CswTableUpdate CswTableUpdateFake = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswTestCase_003_02_008_update", _CswTstCaseRsrc_003.FakeTestTableName );
            DataTable CswTableUpdateFakeTable = CswTableUpdateFake.getEmptyTable();


            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                DataRow NewRow = CswTableUpdateFakeTable.NewRow();
                NewRow[_CswTstCaseRsrc_003.FakeTestColumnName] = CurrentRow[_CswTstCaseRsrc_003.RealTestColumnName];
                CswTableUpdateFakeTable.Rows.Add( NewRow );
            }

            CswTableUpdateFake.update( CswTableUpdateFakeTable );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

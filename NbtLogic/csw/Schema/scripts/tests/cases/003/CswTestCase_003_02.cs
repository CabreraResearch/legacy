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

    public class CswTestCase_003_02 : CswUpdateSchemaTo
    {

        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_003.Purpose, "copy data to use for post-rollback verify" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_003 _CswTstCaseRsrc_003 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_003_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_003 = (CswTstCaseRsrc_003) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_003.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

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

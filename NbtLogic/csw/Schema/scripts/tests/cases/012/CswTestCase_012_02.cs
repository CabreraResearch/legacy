using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_012_02 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_012.Purpose, "Add empty test values" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_012 _CswTstCaseRsrc_012 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_012_02( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_012 = new CswTstCaseRsrc_012( _CswNbtSchemaModTrnsctn );
			
			CswTableUpdate CswArbitraryTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_012.FakeTestTableName );
            CswArbitraryTableUpdate.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTableArbitrary = CswArbitraryTableUpdate.getTable();

            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );


            DataTableArbitrary.Rows[0][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.LocalAribtiraryValue;
            DataTableArbitrary.Rows[1][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.LocalAribtiraryValue;
            DataTableArbitrary.Rows[2][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.LocalAribtiraryValue;

            CswArbitraryTableUpdate.update( DataTableArbitrary );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

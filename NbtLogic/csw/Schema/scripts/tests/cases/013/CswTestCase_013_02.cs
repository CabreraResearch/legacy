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

    public class CswTestCase_013_02 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_013.Purpose, "Set up test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_013 _CswTstCaseRsrc_013 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_013_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_013 = (CswTstCaseRsrc_013) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_013.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_013.FakeTestTableName );
            CswTableUpdate.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTable = CswTableUpdate.getTable();

            DataTable.Rows.Add( DataTable.NewRow() );
            
            DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName01] = _CswTstCaseRsrc_013.LocalAribtiraryValue01;
            DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName02] = _CswTstCaseRsrc_013.LocalAribtiraryValue02;
            CswTableUpdate.update( DataTable );
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

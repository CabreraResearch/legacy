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

    public class CswTestCase_011_03 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_011.Purpose, "Do an insert and cause rollback" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_011 _CswTstCaseRsrc_011 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_011_03( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_011 =   ( CswTstCaseRsrc_011) CswTstCaseRsrc;
        }//ctor

        public void update()
        {
            CswTableUpdate CswUpdateTestTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_011.FakeTestTableName );

            CswUpdateTestTable.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTable = CswUpdateTestTable.getEmptyTable(); 

            DataRow DataRow = DataTable.NewRow();
            DataRow[_CswTstCaseRsrc_011.FakeTestColumnName] = "FOO  ";
            DataTable.Rows.Add( DataRow );

            CswUpdateTestTable.update( DataTable );

            new CswDniExceptionIgnoreDeliberately(); //causes rollback

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

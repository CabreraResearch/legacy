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

    public class CswTestCase_013_05 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_013.Purpose, "Verify that data are as they should be" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_013 _CswTstCaseRsrc_013 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_013_05( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_013 = (CswTstCaseRsrc_013) CswTstCaseRsrc;
        }//ctor

        public void update()
        {
            DataTable DataTable = _CswTstCaseRsrc_013.TheSuspectUpdateTablesUpdater.getTable();
            
            if( DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName02].ToString() != _CswTstCaseRsrc_013.LocalAribtiraryValue02Delta )
                throw ( new CswDniException( "Column  " + _CswTstCaseRsrc_013.FakeValColumnName02 + " does not have the committed value " + _CswTstCaseRsrc_013.LocalAribtiraryValue02Delta ) );

            if( DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName01].ToString() == _CswTstCaseRsrc_013.LocalAribtiraryValue01Delta )
                throw ( new CswDniException( "Column  " + _CswTstCaseRsrc_013.FakeValColumnName01 + " has the rolled back value (with another value modication)" + _CswTstCaseRsrc_013.LocalAribtiraryValue01Delta ) );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

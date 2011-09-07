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

    public class CswTestCase_013_05 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_013.Purpose, "Verify that data are as they should be" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_013 _CswTstCaseRsrc_013 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_013_05( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_013 = (CswTstCaseRsrc_013) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_013.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			DataTable DataTable = _CswTstCaseRsrc_013.TheSuspectUpdateTablesUpdater.getTable();
            
            if( DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName02].ToString() != _CswTstCaseRsrc_013.LocalAribtiraryValue02Delta )
                throw ( new CswDniException( "Column  " + _CswTstCaseRsrc_013.FakeValColumnName02 + " does not have the committed value " + _CswTstCaseRsrc_013.LocalAribtiraryValue02Delta ) );

            if( DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName01].ToString() == _CswTstCaseRsrc_013.LocalAribtiraryValue01Delta )
                throw ( new CswDniException( "Column  " + _CswTstCaseRsrc_013.FakeValColumnName01 + " has the rolled back value (with another value modication)" + _CswTstCaseRsrc_013.LocalAribtiraryValue01Delta ) );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

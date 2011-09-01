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

    public class CswTestCase_012_03 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_012.Purpose, "Commit row one value" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_012 _CswTstCaseRsrc_012 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_012_03( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_012 = new CswTstCaseRsrc_012( _CswNbtSchemaModTrnsctn );

            _CswTstCaseRsrc_012.TheSuspectUpdateTablesUpdater = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_012.FakeTestTableName );
            _CswTstCaseRsrc_012.TheSuspectUpdateTablesUpdater.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            _CswTstCaseRsrc_012.TheSuspectUpdateTable = _CswTstCaseRsrc_012.TheSuspectUpdateTablesUpdater.getTable();

            _CswTstCaseRsrc_012.TheSuspectUpdateTable.Rows[0][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.Val_Row_1;
            _CswTstCaseRsrc_012.TheSuspectUpdateTablesUpdater.update( _CswTstCaseRsrc_012.TheSuspectUpdateTable );


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

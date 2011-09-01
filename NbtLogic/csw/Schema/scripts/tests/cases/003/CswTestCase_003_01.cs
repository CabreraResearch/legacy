﻿using System;
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

    public class CswTestCase_003_01 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_003.Purpose, "add tables to use for post-rollback verify" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_003 _CswTstCaseRsrc_003 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_003_01( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_003 = new CswTstCaseRsrc_003( _CswNbtSchemaModTrnsctn );

            _CswNbtSchemaModTrnsctn.addTable( _CswTstCaseRsrc_003.FakeTestTableName, _CswTstCaseRsrc_003.FakeTestTableName + "id");
            _CswNbtSchemaModTrnsctn.addStringColumn( _CswTstCaseRsrc_003.FakeTestTableName, _CswTstCaseRsrc_003.FakeTestColumnName, "snot", false, false, 240 );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

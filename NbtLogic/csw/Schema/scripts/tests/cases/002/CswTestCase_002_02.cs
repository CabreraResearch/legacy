﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_002_02 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_002.Purpose, "verify rollback" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_002 _CswTstCaseRsrc_002 = null;
        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_002_02( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;            
        }//ctor


        private string _TestColumnName = "test_column";
        private string _TestTableName = "DATA_DICTIONARY";
        //private string _TestValStem = "Test val ";

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_002 = new CswTstCaseRsrc_002( _CswNbtSchemaModTrnsctn );
			
			_CswTstCaseRsrc.assertColumnIsAbsent( _TestTableName, _TestColumnName );
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

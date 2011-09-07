﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 04
    /// </summary>
    public class CswTestCase_001_04 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_001.Purpose, "Verify tear down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrc_001 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_001_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_001 = (CswTstCaseRsrc_001) CswTstCaseRsc;
		}//ctor



        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_001.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            _CswTstCaseRsrc.assertColumnIsAbsent( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne );
            _CswTstCaseRsrc.assertColumnIsAbsent( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

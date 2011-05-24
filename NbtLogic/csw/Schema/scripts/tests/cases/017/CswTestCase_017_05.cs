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

    public class CswTestCase_017_05 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_017.Purpose, "verify test data tear-down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_017 _CswTstCaseRsrc_017 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_017_05( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_017 =   ( CswTstCaseRsrc_017) CswTstCaseRsrc;
        }//ctor

        public void update()
        {
            List<PkFkPair> PairList = _CswTstCaseRsrc_017.getPkFkPairs();

            //Verify that we cleaned up after ourselves
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswTstCaseRsrc.assertTableIsAbsent( CurrentPair.PkTableName );
                _CswTstCaseRsrc.assertTableIsAbsent( CurrentPair.FkTableName );
            }

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

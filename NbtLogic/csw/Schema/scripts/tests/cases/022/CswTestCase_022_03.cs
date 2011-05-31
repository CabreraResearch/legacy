using System;
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
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_022_03 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_022.Purpose, "verify table is not audiable" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_022 _CswTstCaseRsrc_022 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_022_03( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_022 = (CswTstCaseRsrc_022) CswTstCaseRsrc;

        }//ctor


        public void update()
        {
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            _CswTstCaseRsrc.assertColumnIsAbsent( _CswTstCaseRsrc_022.ArbitraryTableName_01, CswAuditMetaData.AuditLevelColName );
            _CswTstCaseRsrc.assertTableIsAbsent( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_022.ArbitraryTableName_01 ) ); 

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

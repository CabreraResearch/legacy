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

    public class CswTestCase_020_04 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_020.Purpose, "tear down test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_020 _CswTstCaseRsrc_020 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_020_04( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;

        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_020 = new CswTstCaseRsrc_020( _CswNbtSchemaModTrnsctn );

            CswAuditMetaData CswAuditMetaData = new Audit.CswAuditMetaData();
            string AuditTableName01 = CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_020.ArbitraryTableName_01 );
            string AuditTableName02 = CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_020.ArbitraryTableName_02 );

            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_020.ArbitraryTableName_01 );
            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_020.ArbitraryTableName_02 );
            _CswNbtSchemaModTrnsctn.dropTable( AuditTableName01 );
            _CswNbtSchemaModTrnsctn.dropTable( AuditTableName02 );


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

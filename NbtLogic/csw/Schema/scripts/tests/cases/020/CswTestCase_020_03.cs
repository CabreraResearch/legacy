using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_020_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_020.Purpose, "test that audit tables were created" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_020 _CswTstCaseRsrc_020 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_020_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_020 = (CswTstCaseRsrc_020) CswTstCaseRsc;

        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_020.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            CswAuditMetaData CswAuditMetaData  = new Audit.CswAuditMetaData();
            string AuditTableName01 = CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_020.ArbitraryTableName_01 );
            string AuditTable01PkColumnName = CswTools.makePkColNameFromTableName( AuditTableName01 );

            string AuditTableName02 = CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_020.ArbitraryTableName_02 );
            string AuditTable02PkColumnName = CswTools.makePkColNameFromTableName( AuditTableName02 );


            //note that these assert() methods are implicitly testing the meta data as well as the actual tables and columns the DB
            _CswTstCaseRsrc.assertTableIsPresent( AuditTableName01 );
            _CswTstCaseRsrc.assertTableIsPresent( AuditTableName02 );


            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName01, CswAuditMetaData.AuditEventTypeColName );
            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName01, AuditTable01PkColumnName );
            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName01, _CswTstCaseRsrc_020.ArbitraryColumnName_01 );
            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName01, _CswTstCaseRsrc_020.ArbitraryColumnName_02 );

            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName02, CswAuditMetaData.AuditEventTypeColName );
            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName02, AuditTable02PkColumnName );
            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName02, _CswTstCaseRsrc_020.ArbitraryColumnName_01 );
            _CswTstCaseRsrc.assertColumnIsPresent( AuditTableName02, _CswTstCaseRsrc_020.ArbitraryColumnName_02 );


        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_009_02 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_009.Purpose, "verify read and tear down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_009 _CswTstCaseRsrc_009 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_009_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_009 = (CswTstCaseRsrc_009) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_009.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;


            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( _CswTstCaseRsrc_009.FakeTestTableName, _CswTstCaseRsrc_009.FakeTestColumnName );

            Int32 TableColId = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;

            if( TableColId <= 0 )
            {
                throw ( new CswDniException( "tableclid received from data dictionry is suspect: " + TableColId.ToString() ) );
            }

            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_009.FakeTestTableName );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

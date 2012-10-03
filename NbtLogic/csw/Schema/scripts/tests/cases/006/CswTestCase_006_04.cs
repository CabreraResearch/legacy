using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_006_04 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_006.Purpose, "verify that constraints are still in place" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_006 _CswTstCaseRsrc_006 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_006_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_006 = (CswTstCaseRsrc_006) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_006.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            List<PkFkPair> PkFkPairs = _CswTstCaseRsrc_006.getPkFkPairs();

            bool ExceptionWasThrown = false;
            try
            {
                CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, PkFkPairs[0].PkTableName );
                DataTable PkTableTable = CswTableUpdate.getTable();

                PkTableTable.Rows[0].Delete();
                CswTableUpdate.update( PkTableTable );
            }//try()

            catch( Exception Exception )
            {
                if( false == _CswTstCaseRsrc.isExceptionRecordDeletionConstraintViolation( Exception ) )
                {
                    throw ( new CswDniException( "An unexpected exception was thrown when deliberately trying to elicit a foreign key constraint violation: " + Exception.Message ) );
                }
                ExceptionWasThrown = true;
            }//catch()

            if( false == ExceptionWasThrown )
                throw ( new CswDniException( "No exception was thrown after the removal of a foreign key constraint was rolled back on table table " + PkFkPairs[0].FkTableName ) );


        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

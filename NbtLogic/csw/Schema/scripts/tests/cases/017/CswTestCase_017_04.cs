using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_017_04 : CswUpdateSchemaTo
    {

        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_017.Purpose, "inspect and test constraints" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_017 _CswTstCaseRsrc_017 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_017_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_017 = (CswTstCaseRsrc_017) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_017.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

			List<PkFkPair> PairList = _CswTstCaseRsrc_017.getPkFkPairs();
            foreach( PkFkPair CurrentPair in PairList )
            {
                bool ExceptionWasThrown = false;

                //Verify that delting a pk table record violates the constraint
                try
                {
                    CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Constraint_AddMultiple_update", CurrentPair.PkTableName );
                    DataTable PkTableTable = CswTableUpdate.getTable();

                    PkTableTable.Rows[ 0 ].Delete();
                    CswTableUpdate.update( PkTableTable );
                }//try()

                catch ( Exception Exception )
                {
                    if( !_CswTstCaseRsrc.isExceptionRecordDeletionConstraintViolation( Exception ) )
                    {
                        throw ( new CswExceptionSchemaUpdate( "An unexpected exception was thrown when deliberately trying to elicit a foreign key constraint violation by deleting a record from table " + CurrentPair.PkTableName + ":" + Exception.Message ) );
                    }
                    ExceptionWasThrown = true;
                }//catch()

                if ( !ExceptionWasThrown )
                    throw ( new CswExceptionSchemaUpdate( "No exception was thrown when deliberately trying to elicit a foreign key constraint violation by deleting a record from table " + CurrentPair.PkTableName ) );

            }//iterate pairs



            //Clean up after ourselves (will verify in next script)
            _CswTstCaseRsrc_017.dropPkFkTables();

        
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

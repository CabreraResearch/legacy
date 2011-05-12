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

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_004_03 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_004.Purpose, "inspect and test constraints" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_004 _CswTstCaseRsrc_004 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_004_03( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_004 =   ( CswTstCaseRsrc_004) CswTstCaseRsrc;
        }//ctor

        public void update()
        {
            List<PkFkPair> PairList = _CswTstCaseRsrc_004.getPkFkPairs();
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
            _CswTstCaseRsrc_004.dropPkFkTables();

        
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

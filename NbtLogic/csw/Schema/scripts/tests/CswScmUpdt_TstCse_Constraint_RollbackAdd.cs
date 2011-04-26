using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswScmUpdt_TstCse_Constraint_RollbackAdd : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Constraint_RollbackAdd( )
            : base( "Rollback Add of Constrained Columns" )
        {
            _ForeignKeyTableFkCol = _PrimeKeyTablePkCol;

        }//ctor

        private string _PrimeKeyTable = "nu_pk_Table";
        private string _PrimeKeyTablePkCol = "nupktableid";
        private string _PrimeKeyTableArbitraryValCol = "arbitraryvalue";

        private string _ForeignKeyTable = "nu_fk_Table";
        private string _ForeignKeyTablePkCol = "nufktableid";
        private string _ForeignKeyTableFkCol = string.Empty;
        private string _ForeignKeyTableArbitraryValCol = "arbitraryvalue";

        private string _TestValStem = "Test val ";

        public override void runTest()
        {
            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.addTable( _PrimeKeyTable, _PrimeKeyTablePkCol );
            _CswNbtSchemaModTrnsctn.addTable( _ForeignKeyTable, _ForeignKeyTablePkCol );

            _CswNbtSchemaModTrnsctn.addColumn( _PrimeKeyTableArbitraryValCol, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _PrimeKeyTable, DataDictionaryUniqueType.None, false, string.Empty );
            _CswNbtSchemaModTrnsctn.addColumn( _ForeignKeyTableArbitraryValCol, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _ForeignKeyTable, DataDictionaryUniqueType.None, false, string.Empty );

            //we add the fk column and fk data in this transaction to set the test up, 
            //but we use another transaction to test the ability to add and rollback a constraint
            _CswNbtSchemaModTrnsctn.addColumn( _ForeignKeyTableFkCol, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.Long, false, false, _ForeignKeyTable, DataDictionaryUniqueType.None, false, string.Empty );
            _CswScmUpdt_TestTools.fillTableWithArbitraryData( _PrimeKeyTable, _PrimeKeyTableArbitraryValCol, _TestValStem, 100 );
            _CswScmUpdt_TestTools.addArbitraryForeignKeyRecords( _PrimeKeyTable, _ForeignKeyTable, _ForeignKeyTableFkCol, _ForeignKeyTableArbitraryValCol, _TestValStem );

            _CswNbtSchemaModTrnsctn.commitTransaction();

            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.makeConstraint( _ForeignKeyTable, _ForeignKeyTableFkCol, _PrimeKeyTable, _PrimeKeyTablePkCol, true );

            bool ExceptionWasThrown = false;
            try
            {
                CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Constraint_RollbackAdd_update1", _PrimeKeyTable );
                DataTable PkTableTable = CswTableUpdate.getTable();

                PkTableTable.Rows[ 0 ].Delete();
                CswTableUpdate.update( PkTableTable );
            }//try()


            catch ( Exception Exception )
            {
                if ( !_CswScmUpdt_TestTools.isExceptionRecordDeletionConstraintViolation( Exception ) )
                {
                    throw ( new CswScmUpdt_Exception( "An unexpected exception was thrown when deliberately trying to elicit a foreign key constraint violation: " + Exception.Message ) );
                }
                ExceptionWasThrown = true;
            }//catch()

            if ( !ExceptionWasThrown )
                throw ( new CswScmUpdt_Exception( "No exception was thrown when deliberately trying to elicit a foreign key constraint violation by deleting a record from table " + _PrimeKeyTable ) );


            _CswNbtSchemaModTrnsctn.rollbackTransaction();


            try
            {
                CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Constraint_RollbackAdd_update2", _PrimeKeyTable );
                DataTable PkTableTable = CswTableUpdate.getTable();

                PkTableTable.Rows[ 0 ].Delete();
                CswTableUpdate.update( PkTableTable );
            }//try()


            catch ( Exception Exception )
            {
                if ( _CswScmUpdt_TestTools.isExceptionRecordDeletionConstraintViolation( Exception ) )
                {
                    throw ( new CswScmUpdt_Exception( "A foreign key constraint violation exception was thrown after the constraint should have been rolled back" ) );
                }
                else
                {
                    throw ( Exception );
                }
            }//catch()



            //Now clean up after ourselves
            //Clean up after ourselves:
            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.dropTable( _PrimeKeyTable );
            _CswNbtSchemaModTrnsctn.dropTable( _ForeignKeyTable );

            _CswNbtSchemaModTrnsctn.commitTransaction();


            _CswNbtSchemaModTrnsctn.rollbackTransaction();

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

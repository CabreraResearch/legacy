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


    public class CswScmUpdt_TstCse_Constraint_AddMultiple : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Constraint_AddMultiple( )
            : base( "Add Multiple Constrained Columns to depth N" )
        {
        }//ctor

        private string _PrimeKeyTableStem = "pk_Table_";
        private string _ArbitraryValCol = "arbitraryvalue";

        private string _ForeignKeyTableStem = "fk_Table_";
        private string _ForeignKeyTableFkCol = string.Empty;

        private string _TestValStem = "Test val ";

        Int32 _TestTableDepth = 6;

        struct PkFkPair
        {
            public string PkTableName;
            public string PkTablePkColumnName;
            public string FkTableName;
            public string FkTablePkColumnName;
        }//

        List<PkFkPair> _PairList = new List<PkFkPair>();

        public override void runTest()
        {
            _CswNbtSchemaModTrnsctn.beginTransaction();

            for ( Int32 idx = 0; idx < _TestTableDepth; idx++ )
            {
                PkFkPair CurrentPair = new PkFkPair();
                CurrentPair.PkTableName = _PrimeKeyTableStem + idx.ToString();
                CurrentPair.PkTablePkColumnName = CurrentPair.PkTableName + "id";
                CurrentPair.FkTableName = _ForeignKeyTableStem + idx.ToString();
                CurrentPair.FkTablePkColumnName = CurrentPair.FkTableName + "id";
                _PairList.Add( CurrentPair );


                _CswNbtSchemaModTrnsctn.addTable( CurrentPair.PkTableName, CurrentPair.PkTablePkColumnName );
                _CswNbtSchemaModTrnsctn.addColumn( _ArbitraryValCol, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, CurrentPair.PkTableName, DataDictionaryUniqueType.None, false, string.Empty );

                _CswNbtSchemaModTrnsctn.addTable( CurrentPair.FkTableName, CurrentPair.FkTablePkColumnName );
                _CswNbtSchemaModTrnsctn.addColumn( _ArbitraryValCol, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, CurrentPair.FkTableName, DataDictionaryUniqueType.None, false, string.Empty );
                _CswNbtSchemaModTrnsctn.addColumn( CurrentPair.PkTablePkColumnName, DataDictionaryColumnType.Fk, 20, 0, "foo", "test column", CurrentPair.PkTablePkColumnName, CurrentPair.PkTableName, true, false, false, string.Empty, false, DataDictionaryPortableDataType.Long, false, false, CurrentPair.FkTableName, DataDictionaryUniqueType.None, false, string.Empty );

                _CswScmUpdt_TestTools.fillTableWithArbitraryData( CurrentPair.PkTableName, _ArbitraryValCol, _TestValStem, 100 );
                _CswScmUpdt_TestTools.addArbitraryForeignKeyRecords( CurrentPair.PkTableName, CurrentPair.FkTableName, CurrentPair.PkTablePkColumnName, _ArbitraryValCol, _TestValStem );

            }//iterate table depth


            _CswNbtSchemaModTrnsctn.commitTransaction();

            foreach ( PkFkPair CurrentPair in _PairList )
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
                    if ( !_CswScmUpdt_TestTools.isExceptionRecordDeletionConstraintViolation( Exception ) )
                    {
                        throw ( new CswScmUpdt_Exception( "An unexpected exception was thrown when deliberately trying to elicit a foreign key constraint violation by deleting a record from table " + CurrentPair.PkTableName + ":" + Exception.Message ) );
                    }
                    ExceptionWasThrown = true;
                }//catch()

                if ( !ExceptionWasThrown )
                    throw ( new CswScmUpdt_Exception( "No exception was thrown when deliberately trying to elicit a foreign key constraint violation by deleting a record from table " + CurrentPair.PkTableName ) );

            }//iterate pairs

            ////Verify that dropping a pk table violates the cosntraint
            //foreach ( PkFkPair CurrentPair in _PairList )
            //{
            //    bool ExceptionWasThrown = false;
            //    try
            //    {
            //        _CswNbtSchemaModTrnsctn.dropTable( CurrentPair.PkTableName );
            //    }//try()

            //    catch ( Exception Exception )
            //    {
            //        if ( !_CswScmUpdt_TestTools.isTableDropConstraintViolation( Exception ) )
            //        {
            //            throw ( new CswScmUpdt_Exception( "An unexpected exception was thrown when deliberately trying to elicit a foreign key constraint violation by deleting a record from table " + CurrentPair.PkTableName + ":" + Exception.Message ) );
            //        }
            //        ExceptionWasThrown = true;
            //    }//catch()
            //}//iterate pairs.

            //Clean up after ourselves:
            foreach ( PkFkPair CurrentPair in _PairList )
            {
                _CswNbtSchemaModTrnsctn.beginTransaction();

                _CswNbtSchemaModTrnsctn.dropTable( CurrentPair.PkTableName );
                _CswNbtSchemaModTrnsctn.dropTable( CurrentPair.FkTableName );

                _CswNbtSchemaModTrnsctn.commitTransaction();
            }//


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

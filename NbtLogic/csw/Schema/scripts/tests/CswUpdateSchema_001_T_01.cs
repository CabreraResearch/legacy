using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswUpdateSchema_001_T_01 : ICswUpdateSchemaTo
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 001, 'T', 01 ); } }


        public CswUpdateSchema_001_T_01( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }//ctor

        private string _TestColumnNameOne = "test_column_one";
        private string _TestColumnNameTwo = "test_column_two";
        private string _TestTableName = "modules";
        private string _TestValStem = "Test val ";

        public void update()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.addColumn( _TestColumnNameOne, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _TestTableName, DataDictionaryUniqueType.None, false, string.Empty );
            _CswNbtSchemaModTrnsctn.addColumn( _TestColumnNameTwo, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _TestTableName, DataDictionaryUniqueType.None, false, string.Empty );


            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _TestTableName, _TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _TestColumnNameOne + " was not created in data base " ) );

            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _TestTableName, _TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _TestColumnNameTwo + " was not created in data base " ) );

            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _TestColumnNameOne + " was not created in meta data " ) );


            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _TestColumnNameTwo + " was not created in meta data " ) );


            _testAddColumnValues( _TestColumnNameOne );
            _testAddColumnValues( _TestColumnNameTwo );

            _CswNbtSchemaModTrnsctn.rollbackTransaction();





        }//runTest()

        private void _testAddColumnValues( string ColumnName )
        {
            Int32 TotalUpdated = 0;
            CswTableUpdate TestTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("testAddColumnValues_update", _TestTableName );
            DataTable TestTable = TestTableUpdate.getTable();
            foreach ( DataRow CurrentRow in TestTable.Rows )
            {
                CurrentRow[ ColumnName ] = "Test val " + TestTable.Rows.IndexOf( CurrentRow ).ToString();
                TotalUpdated++;
            }

            TestTableUpdate.update( TestTable );

            Int32 TotalUpdatedInfact = 0;
            TestTable = TestTableUpdate.getTable();
            foreach ( DataRow CurrentRow in TestTable.Rows )
            {
                if ( ( _TestValStem + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[ ColumnName ].ToString() )
                    TotalUpdatedInfact++;
            }

            if ( TotalUpdatedInfact != TotalUpdated )
                throw ( new CswDniException( "Error adding column " + ColumnName + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value" ) );


        }//_testAddColumnValues

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

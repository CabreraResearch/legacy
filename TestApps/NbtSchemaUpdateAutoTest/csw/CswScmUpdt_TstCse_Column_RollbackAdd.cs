using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_Column_RollbackAdd : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Column_RollbackAdd( )
            : base( "Add column rollback" )
        {
        }//ctor

        private string _TestColumnName = "test_column";
        private string _TestTableName = "DATA_DICTIONARY";
        private string _TestValStem = "Test val ";

        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.addColumn( _TestColumnName, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty,false, DataDictionaryPortableDataType.String, false, false, _TestTableName, DataDictionaryUniqueType.None, false, string.Empty );


            if( ! _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase ( _TestTableName, _TestColumnName ) )
                throw( new CswDniException( "Column " + _TestColumnName + " was not created in data base " ) );

            if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnName ) )
                throw( new CswDniException( "Column " + _TestColumnName + " was not created in meta data " ) );

            CswTableUpdate TestTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Column_RollbackAdd_update", _TestTableName );

            Int32 TotalUpdated = 0;
            DataTable TestTable = TestTableUpdate.getTable();
            foreach ( DataRow CurrentRow in TestTable.Rows )
            {
                CurrentRow[ _TestColumnName ] = "Test val " + TestTable.Rows.IndexOf( CurrentRow ).ToString();
                TotalUpdated++;
            }

            TestTableUpdate.update( TestTable );

            Int32 TotalUpdatedInfact = 0;
            TestTable = TestTableUpdate.getTable();
            foreach ( DataRow CurrentRow in TestTable.Rows )
            {
                if ( ( _TestValStem + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[ _TestColumnName ].ToString() )
                    TotalUpdatedInfact++;
            }

            if( TotalUpdatedInfact != TotalUpdated )
                throw( new CswDniException( "Error adding column " + _TestColumnName + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value"  ) );

            _CswNbtSchemaModTrnsctn.rollbackTransaction();


            if ( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Added column " + _TestColumnName + " was not rolled back from the database " ) );

            if ( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Added column " + _TestColumnName + " was not rolled back from the meta data " ) );



        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema

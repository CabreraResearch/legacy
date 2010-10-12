using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_Column_RollbackDrop : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Column_RollbackDrop( )
            : base( "Drop column rollback" )
        {
        }//ctor

        private string _TestColumnName = "nodename";
        private string _TestTableName = "nodes";
        private string _TestValStem = "Test val ";

        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswScmUpdt_TstCse_Column_RollbackDrop_select1", _TestTableName );
            DataTable DataTable = CswTableSelect.getTable( new StringCollection { _TestColumnName }, string.Empty, Int32.MinValue, string.Empty, false, new Collection<OrderByClause> { new OrderByClause( _TestColumnName, OrderByType.Ascending ) } );
            
            List<string> RetrievedColumnVals = new List<string>();
            foreach ( DataRow CurrentRow in DataTable.Rows )
            {
                RetrievedColumnVals.Add( CurrentRow[ _TestColumnName ].ToString() );
            }

            _CswNbtSchemaModTrnsctn.dropColumn( _TestTableName, _TestColumnName );

            if ( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Column " + _TestColumnName + " was not dropped from the database" ) );

            if ( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Column " + _TestColumnName + " was not dropped from the meta data" ) );


            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            if (! _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Dropped column " + _TestColumnName + " was not restored to the database after rollback" ) );

            if (! _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Dropped column " + _TestColumnName + " was not restored to the meta data after rollback" ) );

            CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswScmUpdt_TstCse_Column_RollbackDrop_select2", _TestTableName );
            DataTable = CswTableSelect.getTable( new StringCollection { _TestColumnName }, string.Empty, Int32.MinValue, string.Empty, false, new Collection<OrderByClause> { new OrderByClause( _TestColumnName, OrderByType.Ascending ) } );

            foreach ( DataRow CurrentRow in DataTable.Rows )
            {
                if( CurrentRow [_TestColumnName].ToString() != RetrievedColumnVals[DataTable.Rows.IndexOf( CurrentRow )] )
                    throw ( new CswDniException( "After rollback the table value at row " + DataTable.Rows.IndexOf( CurrentRow ).ToString() + "(" + CurrentRow[ _TestColumnName ].ToString() + ")  does not match the corresponding value cached prior to dropping the column(" + RetrievedColumnVals[ DataTable.Rows.IndexOf( CurrentRow ) ] + ")" ) );
            }


            
            //_CswNbtSchemaModTrnsctn.addColumn( _TestColumnName, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty,false, DataDictionaryPortableDataType.String, false, false, _TestTableName, DataDictionaryUniqueType.None, false, string.Empty );


            //if( ! _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase ( _TestTableName, _TestColumnName ) )
            //    throw( new CswDniException( "Column " + _TestColumnName + " was not created in data base " ) );

            //if ( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnName ) )
            //    throw( new CswDniException( "Column " + _TestColumnName + " was not created in meta data " ) );

            //CswTableCaddy TestTableCaddy = _CswNbtSchemaModTrnsctn.makeCswTableCaddy( _TestTableName );

            //Int32 TotalUpdated = 0;
            //DataTable TestTable = TestTableCaddy.Table;
            //foreach ( DataRow CurrentRow in TestTable.Rows )
            //{
            //    CurrentRow[ _TestColumnName ] = "Test val " + TestTable.Rows.IndexOf( CurrentRow ).ToString();
            //    TotalUpdated++;
            //}

            //TestTableCaddy.update( TestTable );

            //Int32 TotalUpdatedInfact = 0;
            //TestTable = TestTableCaddy.Table;
            //foreach ( DataRow CurrentRow in TestTable.Rows )
            //{
            //    if ( ( _TestValStem + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[ _TestColumnName ].ToString() )
            //        TotalUpdatedInfact++;
            //}

            //if( TotalUpdatedInfact != TotalUpdated )
            //    throw( new CswDniException( "Error adding column " + _TestColumnName + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value"  ) );

            //_CswNbtSchemaModTrnsctn.rollbackTransaction();


            //if ( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _TestTableName, _TestColumnName ) )
            //    throw ( new CswDniException( "Added column " + _TestColumnName + " was not rolled back from the database " ) );

            //if ( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnName ) )
            //    throw ( new CswDniException( "Added column " + _TestColumnName + " was not rolled back from the meta data " ) );
            


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
